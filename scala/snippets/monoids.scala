trait Monoid[T] {
  def append(l: T, r: T): T
  def identity: T

  def reduce(list: List[T]) = list match {
    case Nil     => identity
    case x :: xs => append(x, reduce(xs))
  }
}
