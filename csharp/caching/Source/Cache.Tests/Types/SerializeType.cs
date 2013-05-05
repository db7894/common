using System;

namespace SharedAssemblies.General.Caching.Tests.Types
{
	/// <summary>
	/// A simple type that we can use to test the serializers
	/// </summary>
	[Serializable]
	public class SerializeType
	{
		public string Name;
		public int Age;
		public DateTime Birthday;
	}
}
