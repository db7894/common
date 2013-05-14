import scala.util.parsing.combinator._

/**
 * value   := object | array | stringLiteral
 *          | floatingPointNumber | "null"
 *          | "true" | "false"
 * object  := "{" members? "}"
 * array   := "[" values? "]"
 * members := member ("," member)*
 * member  := stringLiteral ":" value
 * values  := value ("," value)*
 */
class JsonParser extends JavaTokenParsers {

  def value: Parser[Any] = (
    objekt
  | array
  | stringLiteral
  | floatingPointNumber ^^ (_.toDouble)
  | "null"  ^^ (_ => null)
  | "true"  ^^ (_ => true)
  | "false" ^^ (_ => false)
  )

  def objekt: Parser[Map[String, Any]] =
    "{" ~> repsep(member, ",") <~ "}" ^^ (Map() ++ _)

  def array: Parser[List[Any]] =
    "[" ~> repsep(value, ",") <~ "]"

  def member: Parser[(String, Any)] =
    stringLiteral ~ ":" ~ value ^^
    { case name ~ ":" ~ value => (name, value) }
}

/**
 * Summary of the parser combinator operations::
 *
 * "..."            literal
 * "...".r          regular expression
 * P~Q              sequential composition
 * P <~ Q, P ~> Q   sequential composition; keep left/right only
 * P | Q            alternative
 * opt(P)           option
 * rep(P)           repetition
 * repsep(P, Q)     interleaved repetition
 * P ^^ f           result conversion
 */
object JsonParserMain extends JsonParser {
  def main(args: Array[String]) {
    val input = """
    {
      "address book": {
        "name": "John Smith",
        "address": {
          "street": "10 Market Street",
          "city"  : "San Francisco, CA",
          "zip"   : 94111
        },
        "phone numbers": [
          "408 338-4238",
          "408 111-6892"
        ]
      }
    }
    """
    println("input : " + input)
    println(parseAll(value, input))
  }
}
