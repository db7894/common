using System;
using System.Collections.Concurrent;

namespace Bashwork.General.Validation.Internal
{
	/// <summary>
	/// A singleton cache of previously generated validators by type or name.
	/// </summary>
	internal sealed class ValidatorTypeCache
	{
		/// <summary>
		/// Handle to the dictionary of overloaded name, TypeValidator validators.
		/// This is so users can add differnt validators for common types. Using
		/// object to create a bag.
		/// </summary>
		private static readonly Lazy<ConcurrentDictionary<string, object>> _cache
			= new Lazy<ConcurrentDictionary<string, object>>();

		/// <summary>
		/// Retrieves a validator for the specified type
		/// </summary>
		/// <typeparam name="TObject">The type to get a validator for</typeparam>
		/// <param name="name">The name of the validator to retrieve</param>
		/// <returns>The specified validator or null if not available</returns>
		public IValidator<TObject> Get<TObject>(string name = null)
		{
			object output = null;
			var lookup = BuildUniqueName<TObject>(name);
			return _cache.Value.TryGetValue(lookup, out output)
				? output as IValidator<TObject>
				: null;
		}

		/// <summary>
		/// Adds a validator by the supplied name
		/// </summary>
		/// <typeparam name="TObject">The type to get a validator for</typeparam>		
		/// <param name="validator">The actual validator</param>
		/// <param name="name">The name of the validator</param>
		/// <returns>The result of adding the validator</returns>
		public bool Add<TObject>(IValidator<TObject> validator,
			string name = null)
		{
			var lookup = BuildUniqueName<TObject>(name);
			var converted = validator as object; // pay no attention...
			var result = _cache.Value.AddOrUpdate(lookup, converted, (k, v) => converted);

			return (result == validator);
		}

		/// <summary>
		/// Helper method to create a non-colliding name
		/// </summary>
		/// <typeparam name="TType">The type to create a name for</typeparam>
		/// <param name="name">The unique name to prevent clashing</param>
		/// <returns>The resulting populated name</returns>
		private string BuildUniqueName<TType>(string name)
		{
			// performing this step doubles the lookup time for a type without
			// a name from 100 nS to 200 nS. This should be counted as noise,
			// however a split cache can be used to increase speed.
			return typeof(TType).ToString() + "." + (name ?? string.Empty);
		}
	}
}
