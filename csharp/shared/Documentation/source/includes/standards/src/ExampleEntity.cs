
namespace SharedAssemblies.Entities.ExampleDomain
{
	/// <summary>
	/// Representation of an Example
	/// </summary>
	public class ExampleEntity
	{
		/// <summary>The unique identifer for this entity</summary>
		public string Identifier { get; set; }

		/// <summary>The some branch code for this entity</summary>
		public string BranchCode { get; set; }

		/// <summary>The some symbol for this entity</summary>
		public string Symbol { get; set; }

		/// <summary>The some quantity for this entity</summary>
		public uint Quantity { get; set; }

		/// <summary>The some price for this entity</summary>
		public double Price { get; set; }
	}
}

