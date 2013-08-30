package bashwork

/**
 *
 */
object RegexpParsers {
  implicit def keyword(string: String) = new Parser[String] {
    def apply(in: Stream[Character]) = {
      val trunc = in take string.length
      lazy val errorMessage = "Expected '%s' got '%s'".format(string, trunc.mkString)
 
      if ((trunc lengthCompare string.length) != 0) 
        Failure(errorMessage, in)
      else {
        val succ = trunc.zipWithIndex forall {
          case (c, i) => c == string(i)
        }
 
        if (succ) Success(string, in drop string.length)
        else Failure(errorMessage, in)
      }
    }
  }
}

/**
 * This takes two parsers and returns a new parser that
 * is the logical and of those two parsers. It is
 * represented like::
 *
 *     S := ID1 ~ ID2
 *
 * It should be noted that since the left and right parser
 * are applied lazily, they are not evaluated until they
 * are applied to a specific stream. This prevents them
 * from infinitely recursing in a rule like::
 *
 *     S := Sa | b
 */
class SequenceParser[+A, +B](l: Parser[A], r: Parser[B])
  extends Parser[(A, B)] {

  lazy val left  = l
  lazy val right = r

  def apply(in: Stream[Character]) = left(in) match {
    case Success(x, remainder) => right(remainder) match {
      case Success(y, remainder) => Success((x, y), remainder)
      case failure: Failure => failure
    }
    case failure: Failure => failure
  }
}

/**
 * The disjuction parser handles the or parsing rule::
 *
 *     S := a | b
 */
class DisjunctionParser[+A](left: Parser[A], right: Parser[A]) extends Parser[A] {
  def apply(in: Stream[Character]) = left match {
    case result:  Success[A] => result
    case failure: Failure => right(in)
  }
}

/**
 * The top level parser trait
 * Here outer can be thought of as the following::
 *
 * var outer = this
 */
trait Parser[+A] extends (Stream[Character] => Result[A]) { self =>

  def  ~[B](that: => Parser[B]) = new SequenceParser(self, that)
  def <~[B](that: => Parser[B]) = (self ~ that) ^^ { case (x ~ y) => x }
  def ~>[B](that: => Parser[B]) = (self ~ that) ^^ { case (x ~ y) => y }
  def  |   (that: => Parser[A]) = new DisjunctionParser(self, that)
  def * = rep _

  def ^^[B](f: A => B) = new Parser[B] {
    def apply(in: Stream[Character]) = self(in) match {
      case Success(x, remainder) => Success(f(x), remainder)
      case failure: Failure => failure
    }
  }

  def ^^^[B](f: => B) = new Parser[B] {
    def apply(in: Stream[Character]) = self(in) match {
      case Success(_, remainder) => Success(f, remainder)
      case failure: Failure => failure
    }
  }

  def success[A](value: A) = new Parser[A] {
    def apply(in: Stream[Character]) = Success(value, in)
  }

  def failure(message: String) = new Parser[Nothing] {
    def apply(in: Stream[Character]) = Failure(message, in)
  }

  def opt[A](p: Parser[A]): Parser[Option[A]] = (
      p ^^ { Some(_) }
    | success(None)
  )

  def rep[A](p: Parser[A]): Parser[List[A]] = (
      p ~ rep(p) ^^ { case x ~ xs => x :: xs }
    | success(List())
  )

  def repsep[B, C](p: Parser[B], q: Parser[B]): Parser[List[B]] = (
    p ~ rep(q ~> p) ^^ { case x ~ xs => x :: xs }
    | success(List())
  )

}

/**
 * A collection of parser results
 */
sealed trait Result[+A]
case class Success[+A](value: A, remainder: Stream[Character]) extends Result[A]
case class Failure(message: String, remainder: Stream[Character]) extends Result[Nothing]

//import scala.util.parsing.combinator.RegexParsers
//
//object SimpleScala extends RegexParsers {
//  val ID = """[a-zA-z]([a-zA-Z0-9)*"""r
//  val NUM = """[1-9][0-9]*"""r
//  def program = clazz*
//  def classPrefix = "class" ~ ID ~ "(" ~ formals ~ ")"
//  def classExt = "extends" ~ ID ~ "(" ~ actuals ~ ")"
//  def clazz = classPrefix ~ opt(classExt) ~ "{" ~(members)* ~ ")"
//  def formals = repsep(ID ~ ":" ~ ID, ",")
//  def actuals = expr*
//  def members = (
//      "val" ~ ID ~ ":" ~ ID ~ "=" ~ expr
//    | "var" ~ ID ~ ":" ~ ID ~ "=" ~ expr
//    | "def" ~ ID ~ "(" ~ formals ~ ")" ~ ":" ~ ID ~ "=" ~ expr
//    | "def" ~ ID ~ ":" ~ ID ~ "=" ~ expr
//    | "type" ~ ID ~ "=" ~ ID
//  )
//
//  def expr: Parser[Expr] = factor ~ (
//      "+" ~ factor
//    | "-" ~ factor
//  )*
//
//  def factor = term ~ ("." ~ ID ~ "(" ~ actuals ~ ")")*
//  def term = (
//      "(" ~ expr ~ ")"
//    | ID
//    | NUM
//  )
//}
