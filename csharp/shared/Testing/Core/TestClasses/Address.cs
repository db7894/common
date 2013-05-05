namespace SharedAssemblies.Core.UnitTests.TestClasses
{
	/// <summary>
	/// A simple address class for testing purposes.
	/// </summary>
	internal sealed class Address
	{
		/// <summary>
		/// Street line of the address.
		/// </summary>
		public string Line { get; set; }

		/// <summary>
		/// City of the address.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// State of the address.
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// Zip code of the address.
		/// </summary>
		public string Zip { get; set; }
	}
}
