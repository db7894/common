#include "llvm/DerivedTypes.h"
#include "llvm/ExecutionEngine/ExecutionEngine.h"
#include "llvm/ExecutionEngine/JIT.h"
#include "llvm/Support/IRBuilder.h"
#include "llvm/LLVMContext.h"
#include "llvm/Module.h"
#include "llvm/PassManager.h"
#include "llvm/Analysis/Verifier.h"
#include "llvm/Analysis/Passes.h"
#include "llvm/Target/TargetData.h"
#include "llvm/Transforms/Scalar.h"
#include "llvm/Support/TargetSelect.h"
#include <cstdio>
#include <cstdlib>
#include <string>
#include <map>
#include <vector>

//------------------------------------------------------------
// Lexer
//------------------------------------------------------------
/**
 * @summary The recognized tokens for the lexer
 */
enum Token {
    tok_eof        = -1,
    tok_def        = -2,
    tok_extern     = -3,
    tok_identifier = -4,
    tok_number     = -5
};

static std::string TokStr;
static double TokVal;

/**
 * @summary Get the next token from the stream
 * @returns The next token identifier
 */
static int get_token() {
    static int current = ' ';

    //
    // skip any whitespace
    //
    while (isspace(current)) {
        current = getchar();
    }

    //
    // recognize identifiers
    //
    if (isalpha(current)) {
        TokStr = current;
        while (isalnum((current = getchar()))) {
            TokStr += current;
        }
        if (TokStr == "def") { return tok_def; }
        if (TokStr == "extern") { return tok_extern; }
        return tok_identifier;
    }

    //
    // recognize numbers
    //
    if (isdigit(current) || current == '.') {
        std::string number;
        do {
            number += current;
            current = getchar();
        } while (isdigit(current) || current == '.');

        TokVal = strtod(number.c_str(), 0);
        return tok_number;
    }
    
    //
    // recognize comments
    //
    if (current == '#') {
        do {
            current = getchar();
        } while ((current != EOF) && (current != '\n') && (current != '\r'));

        if (current != EOF) {
            return get_token();
        }
    }

    //
    // recognize end of file
    //
    if (current == EOF) {
        return tok_eof;
    }

    //
    // recognize othere tokens
    //
    int handle = current;
    current = getchar();
    return handle;
}

//------------------------------------------------------------
// Abstract Syntax Tree
//------------------------------------------------------------
/**
 * @summary The base expression node
 */
class ExprNode {
public:
    virtual ~ExprNode() {}
    virtual llvm::Value* Codegen() = 0;
};

/**
 * @summary The expression node for numeric literals
 */
class NumberExprNode : public ExprNode {
    double Value;
public:
    NumberExprNode(double value)
    : Value(value) {}

    virtual llvm::Value* Codegen();
};

/**
 * @summary The expression node for variables
 */
class VariableExprNode : public ExprNode {
    std::string Name;
public:
    VariableExprNode(const std::string& name)
    : Name(name) {}

    virtual llvm::Value* Codegen();
};

/**
 * @summary The expression node for a binary operator
 */
class BinaryExprNode : public ExprNode {
    char Operator;
    ExprNode *LHS, *RHS;
public:
    BinaryExprNode(char op, ExprNode *lhs, ExprNode *rhs)
    : Operator(op), LHS(lhs), RHS(rhs) {}

    virtual llvm::Value* Codegen();
};

/**
 * @summary The expression node for a function call
 */
class CallExprNode : public ExprNode {
    std::string Callee;
    std::vector<ExprNode*> Arguments;
public:
    CallExprNode(const std::string& callee, std::vector<ExprNode*>& args)
    : Callee(callee), Arguments(args) {}

    virtual llvm::Value* Codegen();
};

/**
 * @summary Represents the prototype for the function
 */
class PrototypeNode {
    std::string Name;
    std::vector<std::string> Arguments;
public:
    PrototypeNode(const std::string& name, const std::vector<std::string>& args)
    : Name(name), Arguments(args) {}

    virtual llvm::Function* Codegen();
};

/**
 * @summary Represents the function itself
 */
class FunctionNode {
    PrototypeNode* Proto;
    ExprNode* Body;
public:
    FunctionNode(PrototypeNode* proto, ExprNode* body)
    : Proto(proto), Body(body) {}

    virtual llvm::Function* Codegen();
};

//------------------------------------------------------------
// Parser Utilities
//------------------------------------------------------------
static int TokCur;
static std::map<char, int> BinopPrecedence;

/**
 * @summary Retrieves the next parsed token
 * @returns The next parsed token
 */
static int getNextToken() {
    return TokCur = get_token();
}

ExprNode* Error(const char *message) {
    fprintf(stderr, "Error: %s\n", message);
    return 0;
}

PrototypeNode* ErrorP(const char *message) {
    Error(message);
    return 0;
}

FunctionNode* ErrorF(const char *message) {
    Error(message);
    return 0;
}

llvm::Value* ErrorV(const char *message) {
    Error(message);
    return 0;
}

llvm::Function* ErrorFV(const char *message) {
    Error(message);
    return 0;
}

static int GetTokPrecedence() {
    if (!isascii(TokCur)) {
        return -1;
    }

    int tokprec = BinopPrecedence[TokCur];
    if (tokprec <= 0) { return -1; }
    return tokprec;
}

//------------------------------------------------------------
// Parser
//------------------------------------------------------------
static ExprNode* ParseExpression();
static ExprNode* ParseNumberExpr() {
    ExprNode* result = new NumberExprNode(TokVal);
    getNextToken();
    return result;
}

static ExprNode* ParseParenExpr() {
    getNextToken(); // eat (
    ExprNode* result = ParseExpression();
    if (!result) return 0;

    if (TokCur != ')')
        return Error("expected ')");

    getNextToken(); // eat )
    return result;
}

static ExprNode* ParseIdentifierExpr() {
    std::string id = TokStr;
    getNextToken(); // eat identifier

    if (TokCur != '(')
        return new VariableExprNode(id);

    getNextToken(); // eat (
    std::vector<ExprNode*> args;
    if (TokCur != ')') {
        while (true) {
            ExprNode* arg = ParseExpression();
            if (!arg) return 0;
            args.push_back(arg);

            if (TokCur == ')') break;
            if (TokCur != ',')
                return Error("expected ')' or ',' in argument list");
            getNextToken();
        }
    }

    getNextToken(); // eat )
    return new CallExprNode(id, args);
}

static ExprNode* ParsePrimary() {
    switch (TokCur) {
        case tok_identifier:    return ParseIdentifierExpr();
        case tok_number:        return ParseNumberExpr();
        case '(':               return ParseParenExpr();
        default: return Error("unknown token found when expecting expression");
    }
}

/**
 * @summary
 * prototype ::= id '(' id* ')'
 */
static PrototypeNode* ParsePrototype() {
    if (TokCur != tok_identifier) {
        return ErrorP("expected function name in prototype");
    }

    std::string name = TokStr;
    getNextToken();

    if (TokCur != '(') {
        return ErrorP("expected '(' in prototype");
    }

    std::vector<std::string> arguments;
    while (getNextToken() == tok_identifier) {
        arguments.push_back(TokStr);
    }

    if (TokCur != ')') {
        return ErrorP("expected ')' in prototype");
    }
    getNextToken(); // eat ')'

    return new PrototypeNode(name, arguments);
}

/**
 * @summary
 * binoprhs ::= ('+' primary)*
 */
static ExprNode* ParseBinOpRhs(int lhsprec, ExprNode* lhs) {
    while (true) {
        int rhsprec = GetTokPrecedence();
        if (rhsprec < lhsprec) {
            return lhs;
        }

        int op = TokCur;
        getNextToken(); // eat binop

        ExprNode* rhs = ParsePrimary();
        if (!rhs) { return 0; }

        int nextprec = GetTokPrecedence();
        if (rhsprec < nextprec) {
            rhs = ParseBinOpRhs(rhsprec + 1, rhs);
            if (rhs == 0) { return 0; }
        }

        lhs = new BinaryExprNode(op, lhs, rhs);
    }
}

/**
 * @summary
 * expression ::= primary binoprhs
 */
static ExprNode* ParseExpression() {
    ExprNode* expr = ParsePrimary();
    if (!expr) { return 0; }

    return ParseBinOpRhs(0, expr);
}

/**
 * @summary
 * external ::= 'extern' prototype
 */
static PrototypeNode* ParseExtern() {
    getNextToken(); // eat extern
    return ParsePrototype();
}

/**
 * @summary
 * definition ::= 'def' prototype expression
 */
static FunctionNode* ParseDefinition() {
    getNextToken(); // eat def
    PrototypeNode* proto = ParsePrototype();
    if (proto == 0) return 0;

    if (ExprNode *expr = ParseExpression()) {
        return new FunctionNode(proto, expr);
    }
    return 0;
}

/**
 * @summary
 * toplevelexpr ::= expression
 */
static FunctionNode* ParseTopLevelExpr() {
    if (ExprNode* expr = ParseExpression()) {
        PrototypeNode* proto = new PrototypeNode("", std::vector<std::string>());
        return new FunctionNode(proto, expr);
    }
    return 0;
}

//------------------------------------------------------------
// Code Generator
//------------------------------------------------------------
static llvm::Module *TheModule;
static llvm::IRBuilder<> Builder(llvm::getGlobalContext());
static std::map<std::string, llvm::Value*> NamedValues;


llvm::Value* NumberExprNode::Codegen() {
    return llvm::ConstantFP::get(
        llvm::getGlobalContext(), llvm::APFloat(Value));
}

llvm::Value* VariableExprNode::Codegen() {
    llvm::Value *value = NamedValues[Name];
    return (value) ? value : ErrorV("unknown variable name");
}

llvm::Value* BinaryExprNode::Codegen() {
    llvm::Value *lhs = LHS->Codegen();
    llvm::Value *rhs = RHS->Codegen();
    if (lhs == 0 || rhs == 0) return 0;

    switch (Operator) {
        case '+': return Builder.CreateFAdd(lhs, rhs, "addtmp");
        case '-': return Builder.CreateFSub(lhs, rhs, "subtmp");
        case '*': return Builder.CreateFMul(lhs, rhs, "multmp");
        case '/': return Builder.CreateFDiv(lhs, rhs, "divtmp");
        case '<':
            lhs = Builder.CreateFCmpULT(lhs, rhs, "cmptmp");
            const llvm::Type* type = llvm::Type::getDoubleTy(llvm::getGlobalContext());
            return Builder.CreateUIToFP(lhs, type, "booltmp");
    }
    return ErrorV("invalid binary operator");
}

llvm::Value* CallExprNode::Codegen() {
    llvm::Function* callee = TheModule->getFunction(Callee);
    if (callee == 0) {
        return ErrorV("unknown function referenced");
    }

    if (callee->arg_size() != Arguments.size()) {
        return ErrorV("incorrect number of arguments passed");
    }

    std::vector<llvm::Value*> arguments;
    for (size_t i = 0, e = Arguments.size(); i != e; ++i) {
        arguments.push_back(Arguments[i]->Codegen());
        if (arguments.back() == 0) { return 0; }
    }

    return Builder.CreateCall(callee,
            arguments.begin(), arguments.end(), "calltmp");
}

llvm::Function* PrototypeNode::Codegen() {
    const llvm::Type* dtype = llvm::Type::getDoubleTy(llvm::getGlobalContext());
    std::vector<const llvm::Type*> doubles(Arguments.size(), dtype);
    llvm::FunctionType *type = llvm::FunctionType::get(dtype, doubles, false);
    llvm::Function* func = llvm::Function::Create(type,
        llvm::Function::ExternalLinkage, Name, TheModule);

    // check if a conflicting function got renamed
    if (func->getName() != Name) {
        func->eraseFromParent();
        func = TheModule->getFunction(Name);

        // check if the function has a body
        if (!func->empty()) {
            return ErrorFV("redefinition of function");
        }
        // check if the argument counts match
        if (!func->arg_size() != Arguments.size()) {
            return ErrorFV("redefinition of function with different args count");
        }
    }

    // set names for arguments and add to symbol table
    size_t idx = 0;
    for (llvm::Function::arg_iterator it = func->arg_begin();
         idx != Arguments.size(); ++it, ++idx) {
        it->setName(Arguments[idx]);
        NamedValues[Arguments[idx]] = it;
    }

    return func;
}

llvm::Function* FunctionNode::Codegen() {
    NamedValues.clear();
    llvm::Function *func = Proto->Codegen();
    if (func == 0) { return 0; }

    llvm::BasicBlock *bb = llvm::BasicBlock::Create(
        llvm::getGlobalContext(), "entry", func);
    Builder.SetInsertPoint(bb);

    if (llvm::Value *retval = Body->Codegen()) {
        Builder.CreateRet(retval);
        llvm::verifyFunction(*func);
        TheFPM->run(func);
        return func;
    }

    func->eraseFromParent();
    return 0;
}

//------------------------------------------------------------
// Interpreter
//------------------------------------------------------------
static void HandleDefinition() {
    if (FunctionNode* func = ParseDefinition()) {
        if (llvm::Function* llfunc = func->Codegen()) {
            fprintf(stderr, "Parsed a function definition\n");
            llfunc->dump();
        }
    } else {
        getNextToken(); // try to recover
    }
}

static void HandleExtern() {
    if (PrototypeNode* func = ParseExtern()) {
        if (llvm::Function* llfunc = func->Codegen()) {
            fprintf(stderr, "Parsed an extern\n");
            llfunc->dump();
        }
    } else {
        getNextToken(); // try to recover
    }
}

static void HandleTopLevelExpression() {
    if (FunctionNode* func = ParseTopLevelExpr()) {
        if (llvm::Function* llfunc = func->Codegen()) {
            fprintf(stderr, "Parsed a top-level expr\n");
            llfunc->dump();
        }
    } else {
        getNextToken(); // try to recover
    }
}

/**
 * @summary Top level definition
 * @returns void
 *
 * start -> definition | external | expression | ';'
 */
static void StartExpr() {
    while (true) {
        fprintf(stderr, "ready> ");
        switch (TokCur) {
            case tok_eof:
                return;

            case ';':
                getNextToken();
                break;

            case tok_def:
                HandleDefinition();
                break;

            case tok_extern:
                HandleExtern();
                break;

            default: HandleTopLevelExpression();
                break;
        }
    }
}

//------------------------------------------------------------
// Library
//------------------------------------------------------------
extern "C"
double putchard(double value) {
    putchar((char)value);
    return 0.0;
}

//------------------------------------------------------------
// Main Driver
//------------------------------------------------------------
int main() {
    TheModule = new llvm::Module("jitter", llvm::getGlobalContext());

    // install binary operators
    BinopPrecedence['<'] = 10;
    BinopPrecedence['>'] = 10;
    BinopPrecedence['+'] = 20;
    BinopPrecedence['-'] = 30;
    BinopPrecedence['*'] = 40;
    BinopPrecedence['/'] = 50;

    // prime the interpreter
    fprintf(stderr, "ready> ");
    getNextToken();

    // initialize the optimizer
    llvm::FunctionPassManager FPM(TheModule);
    FPM.add(new llvm::TargetData(*TheExecutionEngine->getTargetData()));
    FPM.add(llvm::createBasicAliasAnalysisPass());
    FPM.add(llvm::createInstructionCombiningPass());
    FPM.add(llvm::createReassociatePass());
    FPM.add(llvm::createGVNPass()):
    FPM.add(llvm::createCFGSimplificationPass());
    FPM.doInitialization();
    TheFPM = &FPM:

    // run the interpreter
    StartExpr();
    TheModule->dump();

    return 0;
}
