using System;
using System.Collections.Generic;


namespace SharedAssemblies.Core.Conversions
{
    /// <summary>
    /// A translator that uses a dictionary to do its translations.  Dictionaries
    /// </summary>
    /// <typeparam name="TFrom">Type to translate values from.</typeparam>
    /// <typeparam name="TTo">Type to translate values to.</typeparam>
    public class Translator<TFrom, TTo> : Translator<TFrom, TTo, Dictionary<TFrom, TTo>>
    {
		/// <summary>
		/// Create a translator with a default to value specified and the system 
		/// default for the from type (NULL if reference type and ZERO for value
		/// types)
		/// </summary>
		/// <param name="defaultValue">Value to use if translate fails.</param>
		/// <param name="translations">Sequence of initial translations to add.</param>
		public Translator(TTo defaultValue = default(TTo), IEnumerable<KeyValuePair<TFrom, TTo>> translations = null)
			: base(defaultValue, translations)
		{
		}

        /// <summary>
        /// Create a translator with a default to and from value specified
        /// </summary>
        /// <param name="defaultReverseValue">Value to use if reverse-translate fails</param>
        /// <param name="defaultValue">Value to use if translate fails</param>
		/// <remarks>Deprecated with error in 2.0 in favor of a ReverseTranslator factory.</remarks>
		[Obsolete("This constructor is no longer supported, reverse translation must be accomplished using " +
			"the ReverseTranslator.Create() factory to produce a separate inverse Translator instance.", true)]
		public Translator(TFrom defaultReverseValue, TTo defaultValue)
        {
			throw new NotImplementedException("This constructor is no longer supported.");
        }
    }
}