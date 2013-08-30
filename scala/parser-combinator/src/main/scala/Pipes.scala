trait Pipe[-A, +B] extends (Stream[A] => Stream[B])
trait TranformPipe[-A, +B] extends Pipe[A, B]
trait FilterPipe[A] extends Pipe[A, A]
trait SideEffectPipe[A] extends Pipe[A, A]
