abstract class Tree {
  def include(x: Int): Tree
  def contains(x: Int): Boolean
}

object EmptyTree extends Tree {
  def contains(x: Int): false
  def include(x: Int): new ValueTree(x, new EmptyTree, new EmptyTree)
  override def toString() = "{}"
}

class ValueTree(x: Int, left:Tree, right: Tree) extends Tree {
  def contains(x: Int) =
    if (x > this.x) right contains x
    else if (x < this.x) left contains x
    else true

  def include(x: Int):
    if (x > this.x) new ValueTree(x, left, right include x)
    else if (x < this.x) new ValueTree(x, left include x, right)
    else this

  override def toString() =
    left.toString + ".{" + x "}." + right.toString
}
