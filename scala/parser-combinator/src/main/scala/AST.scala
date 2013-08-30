
trait Node
case class Root(id: String, name: String) extends Node
case class Expr(expr: Node, name: String) extends Node

case class Vertex(name: String, prop: Map[String, Any])
object Vertex {
  def apply(name: String): Vertex = Vertex(name, Map.empty)
}

object AST {
  def getVertex(id: String, name: String) = Vertex(id)
  def getMethod(name: String): Option[(Vertex => Vertex)] = name match {
    case "a" => Some((v:Vertex) => Vertex(v.name, v.prop + ("a" -> true)))
    case "b" => Some((v:Vertex) => Vertex(v.name, v.prop + ("b" -> true)))
    case _   => None
  }
    
  def eval(node: Node): Vertex = node match {
    case Root(id, name)   => getVertex(id, name)
    case Expr(expr, name) => getMethod(name) match {
      case Some(method) => method(eval(expr))
      case _            => throw new Exception("invalid ast")
    }
  }

  def main(args: Array[String]) {
    val expr = Expr(Expr(Root("galen", "customer"), "a"), "b")
    println(expr)
    println(eval(expr))
  }
}
