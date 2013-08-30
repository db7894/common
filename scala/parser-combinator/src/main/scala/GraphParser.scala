import scala.util.parsing.combinator._

/**
 * Scala defines a hierarchy of traits that can be used
 * to implement parsers. The top-level one is Parsers, 
 * then RegexParsers, then JavaTokenParsers
 */
class GraphParserV1 extends JavaTokenParsers {
  def query: Parser[Any] =
    ident ~ opt("." ~ label)
  def label: Parser[Any] =
    ident ~ "(" ~ repsep(query, ",") ~ ")"
}

case class Query(field: String, vertex: String, fields: List[String])

/**
 * query  := string "(" roots ")" labels*
 * labels := "." lables
 * roots  := string ("," string)*
 * 
 */
class GraphParser extends JavaTokenParsers {
  def query: Parser[Query] =
    ident ~ root ~ "." ~ labels ^^
    { case name ~ root ~ "." ~ lables => Query(name, root, lables) }
  def root: Parser[String] = "(" ~> ident <~ ")"
  //def roots: Parser[List[String]] =
  //  "(" ~> repsep(ident, ",") <~ ")" ^^ (List() ++ _)
  def labels: Parser[List[String]] =
    repsep(ident, ".") ^^ (List() ++ _)
}

object GraphParserMain extends GraphParser {
  def main(args: Array[String]) {
    val input = "share(id).customers.uniq"
    println("input : " + input)
    println(parseAll(query, input))
  }
}
