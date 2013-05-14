import scala.util.parsing.combinator.lexical.StdLexical

object SimpleLexer extends StdLexical {

  def loop(s: Scanner, tokens: Seq[Token]): Seq[Token] =
    if (s.atEnd) tokens
    else loop(s.rest, tokens :+ s.first)

  val input = """
    object Main {
      def main(args: Array[String]) {
        println("hello world")
      }
    }
  """

  //def main(args: Array[String]) {
  //  println(loop(new Scanner(input), Vector()))
  //}
}
