namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// An empty array singleton for the given type.
	/// </summary>
	/// <typeparam name="T">Type of array to hold.</typeparam>
	public static class EmptyArray<T>
	{
		/// <summary>
		/// An instance of an empty array of type T.
		/// </summary>
		private static readonly T[] _instance = new T[0];

		/// <summary>
		/// Gets the singleton instance of the empty array
		/// </summary>
		public static T[] Instance
		{
			get { return _instance; }
		}
	}
}
