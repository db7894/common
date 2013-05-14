import scala.util.parsing.combinator._

/**
 * Scala defines a hierarchy of traits that can be used
 * to implement parsers. The top-level one is Parsers, 
 * then RegexParsers, then JavaTokenParsers
 */
class GraphParser extends JavaTokenParsers {
  def query: Parser[Any]  = ident ~ opt("." ~ label)
  def label: Parser[Any]  = ident ~ "(" ~ repsep(query, ",") ~ ")"
}

object GraphParserMain extends GraphParser {
  def main(args: Array[String]) {
    val input = "cindy.shared(media,customers.shared())"
    println("input : " + input)
    println(parseAll(query, input))
  }
}
