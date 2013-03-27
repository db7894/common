using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalShare
{
	/// <summary>
	/// Interface for a tree node
	/// </summary>
	public interface INode<T>
		where T : IComparable<T>
	{
		bool IsEmpty { get; }
		T Value { get; }
		INode<T> Left { get; }
		INode<T> Right { get; }
	}

	/// <summary>
	/// Represents an empty leaf
	/// </summary>
	public class EmptyNode<T> : INode<T>
		where T : IComparable<T>
	{
		public static INode<T> Instance = new EmptyNode<T>();
		public bool IsEmpty { get { return true; } }
		public T Value { get { throw new ArgumentNullException(); } }
		public INode<T> Left { get { throw new ArgumentNullException(); } }
		public INode<T> Right { get { throw new ArgumentNullException(); } }
	}

	/// <summary>
	/// Represents a node in the tree with a value and possibly
	/// some children
	/// </summary>
	public class ValueNode<T> : INode<T>
		where T : IComparable<T>
	{
		public ValueNode(T value)
			: this(value, EmptyNode<T>.Instance, EmptyNode<T>.Instance)
		{}

		public ValueNode(T value, INode<T> left, INode<T> right)
		{
			Value = value;
			Left = left;
			Right = right;
		}

		public bool IsEmpty { get { return false; } }
		public T Value { get; private set; }
		public INode<T> Left { get; private set; }
		public INode<T> Right { get; private set; }
	}

	/// <summary>
	/// An example of using the tree functionally
	/// </summary>
	public static class TreeExample
	{
		public static INode<T> insert<T>(INode<T> tree, T value)
			where T : IComparable<T>
		{
			if (tree.IsEmpty)
				return new ValueNode<T>(value);

			switch (tree.Value.CompareTo(value))
			{
				case  0: return tree;
				case -1: return new ValueNode<T>(tree.Value, tree.Left, insert(tree.Right, value));
				case  1: return new ValueNode<T>(tree.Value, insert(tree.Left, value), tree.Right);
			}

			throw new ArgumentNullException("well huh, that sucks");
		}

		/// <summary>
		/// A simple in-order visitor
		/// </summary>
		public static void visit<T>(INode<T> tree)
			where T : IComparable<T>
		{
			if (!tree.Left.IsEmpty) visit(tree.Left);

			Console.WriteLine(tree.Value.ToString());

			if (!tree.Right.IsEmpty) visit(tree.Right);
		}

		/// <summary>
		/// A simple in-order visitor
		/// </summary>
		public static int total(INode<int> tree)
		{
			return (tree.IsEmpty)
				? 0
				: tree.Value + total(tree.Left) + total(tree.Right);
		}

		/// <summary>
		/// A simple in-order visitor, but now is able to be tail call recursive
		/// </summary>
		public static int totalTC(INode<int> tree, int sum = 0)
		{
			return (tree.IsEmpty)
				? sum
				: totalTC(tree.Right, totalTC(tree.Left, tree.Value + sum));
		}

		/// <summary>
		/// Tree monoid!
		/// </summary>
		public static void main()
		{
			var values = new List<int> { 4, 2, 3, 7, 5, 1, 9, 22, 11, 6 };
			var tree = values.Aggregate<int, INode<int>>(EmptyNode<int>.Instance, insert);
			visit(tree);
			Console.WriteLine("total sum of the tree is {0}", total(tree));
			
			var tree2 = insert(tree, 17);
			visit(tree2);
			Console.WriteLine("total sum of the tree is {0}", total(tree2));
			Console.WriteLine("total sum of the tree is {0}", totalTC(tree2));
		}
	}
}
