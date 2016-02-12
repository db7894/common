object functions extends App {
  val double = (i: Integer) => i * 2
  val double_to_string = double andThen (i => i.toString)
  val double_to_phrase = double_to_string andThen (s => s"the result is $s")

  println(double(5))
  println(double_to_string(5))
  println(double_to_phrase(5))
}
