#include "TokenPairRec.h"
#include "Token.h"

using namespace NativeInstrumentation;

const int TokenPairRec::TokenPairRecSize = 2 * Token::TokenSize + sizeof(unsigned long long);