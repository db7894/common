using System;
using System.Collections.Generic;

namespace SharedAssemblies.Core.Conversions
{
    /// <summary>
    /// Generic interface for a translator
    /// </summary>
    /// <typeparam name="TFrom">Type translating from</typeparam>
    /// <typeparam name="TTo">Type translating to</typeparam>
    public interface ITranslator<TFrom, TTo> : IEnumerable<KeyValuePair<TFrom, TTo>>
    {
    	/// <summary>
    	/// Indexes the translator.  On get it will lookup the type calling
    	/// Translate.  On set it will Add a new lookup with the given value.
    	/// </summary>
    	/// <param name="index">The value to lookup</param>
    	/// <returns>The translated value</returns>
    	TTo this[TFrom index]
    	{
    		get; set;
    	}

    	/// <summary>
    	/// Gets the default value used when a lookup fails.
    	/// </summary>
    	TTo DefaultValue
    	{
    		get;
    	}
		
		/// <summary>
        /// Performs a translation using the table, returns the default from value
        /// if cannot find a matching result.
        /// </summary>
        /// <param name="value">Value to translate</param>
        /// <returns>The translated value</returns>
        TTo Translate(TFrom value);

        /// <summary>
        /// Adds a new translation to the translation table
        /// </summary>
        /// <param name="key">The key to search for</param>
        /// <param name="value">The resulting value</param>
        void Add(TFrom key, TTo value);

        /// <summary>
        /// Adds a set of new translations to the translation table
        /// </summary>
		/// <param name="pairs">Adds a sequence of key value pairs</param>
		void Add(IEnumerable<KeyValuePair<TFrom, TTo>> pairs);

		/// <summary>
		/// Method to see if the translator interface contains the given from value.
		/// </summary>
		/// <param name="fromValue">The value to check.</param>
		/// <returns>True if the translator contains the from value.</returns>
    	bool Contains(TFrom fromValue);

        /// <summary>
        /// Clears all existing translations and defaults
        /// </summary>
        void Clear();
    }
}