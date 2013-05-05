namespace SharedAssemblies.Core.UnitTests.TestClasses
{
	/// <summary>
	/// Simple test class representation of a person.
	/// </summary>
	internal sealed class Person
	{
		/// <summary>
		/// The name of the person .
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The home address of the person.
		/// </summary>
		public Address HomeAddress { get; set; }
	}
}
