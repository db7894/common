using System;


namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// Factory that generates composite keys
	/// </summary>
	public static class CompositeKeyFactory
	{
		/// <summary>
		/// Constructs a composite key given the arguments provided.  Makes it easier to generate a composite key by inferring type.
		/// </summary>
		/// <typeparam name="TPrimary">The primary key type.</typeparam>
		/// <typeparam name="TSecondary">The secondary key type.</typeparam>
		/// <param name="primary">The primary key value.</param>
		/// <param name="secondary">The secondary key value.</param>
		/// <returns>The composite key.</returns>
		public static CompositeKey<TPrimary, TSecondary> Create<TPrimary, TSecondary>(TPrimary primary, TSecondary secondary)
			where TPrimary : IEquatable<TPrimary>, IComparable<TPrimary>
			where TSecondary : IEquatable<TSecondary>, IComparable<TSecondary>
		{
			return new CompositeKey<TPrimary, TSecondary>(primary, secondary);
		}

		/// <summary>
		/// Constructs a composite key given the arguments provided.  Makes it easier to generate a composite key by inferring type.
		/// </summary>
		/// <typeparam name="TPrimary">The primary key type.</typeparam>
		/// <typeparam name="TSecondary">The secondary key type.</typeparam>
		/// <typeparam name="TTernary">The ternary key type.</typeparam>
		/// <param name="primary">The primary key value.</param>
		/// <param name="secondary">The secondary key value.</param>
		/// <param name="ternary">The ternary key value.</param>
		/// <returns>The composite key.</returns>
		public static CompositeKey<TPrimary, TSecondary, TTernary> Create<TPrimary, TSecondary, TTernary>(TPrimary primary, TSecondary secondary, TTernary ternary)
			where TPrimary : IEquatable<TPrimary>, IComparable<TPrimary>
			where TSecondary : IEquatable<TSecondary>, IComparable<TSecondary>
			where TTernary : IEquatable<TTernary>, IComparable<TTernary>
		{
			return new CompositeKey<TPrimary, TSecondary, TTernary>(primary, secondary, ternary);
		}
	}
}
