import scala.util.parsing.combinator._

/**
 * Scala defines a hierarchy of traits that can be used
 * to implement parsers. The top-level one is Parsers, 
 * then RegexParsers, then JavaTokenParsers
 */
class CalcParser extends JavaTokenParsers {
  def expr: Parser[Any]   = term ~ rep("+" ~ term | "-" ~ term)
  def term: Parser[Any]   = factor ~ rep("*" ~ factor | "/" ~ factor)
  def factor: Parser[Any] = floatingPointNumber | "(" ~ expr ~ ")"
}

object CalcParserMain extends CalcParser {
  def main(args: Array[String]) {
    val input = "2 + (2 * 4)"
    println("input : " + input)
    println(parseAll(expr, input))
  }
}
