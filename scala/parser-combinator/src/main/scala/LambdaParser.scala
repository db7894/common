import scala.util.parsing.combinator._

/**
 * Parser for the pure untyped lambda calculus
 */
class LambdaParser extends JavaTokenParsers {

  def expr: Parser[Any] = term ~ (expr | "")
  def term = (
      "fn" ~ ident ~ "=>" ~ expr
    | ident
  )
}

object LambdaParserMain extends LambdaParser {
  def main(args: Array[String]) {
    val input = """
    fn f => fn x =>  x
    """
    println("input : " + input)
    println(parseAll(expr, input))
  }
}
