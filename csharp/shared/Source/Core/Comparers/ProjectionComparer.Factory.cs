using System;


namespace SharedAssemblies.Core.Comparers
{
	/// <summary>
	/// Helper factory class that infers the types of the ProjectionComparer from the types of the projection.
	/// </summary>
	public static class ProjectionComparer
	{
		/// <summary>
		/// Factory method to generate the comparer for the projection using type inference.
		/// </summary>
		/// <typeparam name="TCompare">The type of objects to compare.</typeparam>
		/// <typeparam name="TProjected">The type of the object projection result.</typeparam>
		/// <param name="projection">The projection from TCompare to TProjected.</param>
		/// <returns>A ProjectionComparer for the given type.</returns>
		public static ProjectionComparer<TCompare, TProjected> Create<TCompare, TProjected>(Func<TCompare, TProjected> projection)
		{
			return new ProjectionComparer<TCompare, TProjected>(projection);
		}
	}
}
