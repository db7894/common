using System;
using System.Collections;
using System.Collections.Generic;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Core.Conversions
{
	/// <summary>
	/// <para>
	/// Creates a translator that converts from/to different values, useful for
	/// converting from an enum to a string or int to an enum or whatever.
	/// </para>
	/// <para>
	/// This is the generic form of the translator where you specify the translation dictionary
	/// in the signature of the translator.  If you do not specify a TDictionary, you will
	/// get the default of Dictionary{TFrom,TTo}.
	/// </para>
	/// </summary>
	/// <typeparam name="TFrom">The "FROM" type of the translation pair</typeparam>
	/// <typeparam name="TTo">The "TO" type of the translation pair</typeparam>
	/// <typeparam name="TDictionary">The type of dictionary to hold the translator.</typeparam>
	public class Translator<TFrom, TTo, TDictionary> : ITranslator<TFrom, TTo> 
		where TDictionary : IDictionary<TFrom, TTo>, new() 
	{
		/// <summary>default if can't translation</summary>
		private readonly TTo _defaultValue;

		/// <summary>The translation table</summary>
		private readonly IDictionary<TFrom, TTo> _map;

		/// <summary>
		/// Property to get/set the default from value if a reverse lookup fails
		/// </summary>
		/// <remarks>Deprecated with error in 2.0 in favor of a ReverseTranslator.Create() factory to create inverse translator.</remarks>
		[Obsolete("This property is obsolete since ReverseTranslate() no longer supported, " +
				  "Use ReverseTranslator.Create() to generate an inverse Translator instead.", true)]
		public TFrom DefaultReverseValue
		{
			get { throw new NotImplementedException("The DefaultReverseValue property is no longer supported."); }
		}

		/// <summary>
		/// Property to get/set the default to value if a lookup fails
		/// </summary>
		/// <remarks>The setter is deprecated in the 2.0 release, set using constructor.</remarks>
		public TTo DefaultValue
		{
			get { return _defaultValue; }
		}

		/// <summary>
		/// Property that returns the number of elements in the lookup table
		/// </summary>
		public int TranslationCount
		{
			get { return _map.Count; }
		}

		/// <summary>
		/// Indexes the translator.  On get it will lookup the type calling
		/// Translate.  On set it will Add a new lookup with the given value.
		/// </summary>
		/// <param name="index">The value to lookup</param>
		/// <returns>The translated value</returns>
		public TTo this[TFrom index]
		{
			get { return Translate(index); }
			set { Add(index, value); }
		}

		/// <summary>
		/// Create a translator with a default to value specified and the system 
		/// default for the from type (NULL if reference type and ZERO for value
		/// types)
		/// </summary>
		/// <param name="defaultValue">Value to use if translate fails.</param>
		/// <param name="translations">Sequence of initial translations to add.</param>
		public Translator(TTo defaultValue = default(TTo), IEnumerable<KeyValuePair<TFrom, TTo>> translations = null)
		{
			_defaultValue = defaultValue;

			_map = new TDictionary();

			// add initial translations, if any
			if (translations != null)
			{
				Add(translations);
			}
		}

		/// <summary>
		/// Create a translator with a default to and from value specified.
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

		/// <summary>
		/// Performs a translation using the table, returns the default from value
		/// if cannot find a matching result.
		/// </summary>
		/// <param name="value">Value to translate</param>
		/// <returns>The translated value</returns>
		public TTo Translate(TFrom value) 
		{
			TTo result;

			// loop through table looking for result
			if (value == null || !_map.TryGetValue(value, out result))
			{
				result = _defaultValue;
			}

			return result;
		}

		/// <summary>
		/// Performs a reverse translation using the table, returns the default to
		/// value if cannot find a matching result.
		/// </summary>
		/// <remarks>
		/// This is not extremely efficient - O(n) - so if you need quick reverse translation, it is recommended you 
		/// create a second translator in the opposite direction.
		/// </remarks>
		/// <param name="value">Value to translate in reverse</param>
		/// <returns>The un-translated value</returns>
		/// <remarks>Deprecated with error in 2.0 in favor of a ReverseTranslator.Create() factory to create inverse translator.</remarks>
		[Obsolete("This method is obsolete since ReverseTranslate() no longer supported, " +
				  "Use ReverseTranslator.Create() to generate an inverse Translator instead.", true)]
		public TFrom ReverseTranslate(TTo value)
		{
			throw new NotImplementedException("This method is obsolete and no longer implemented.");
		}

		/// <summary>
		/// Adds a new translation to the translation table
		/// </summary>
		/// <param name="key">The key to search for</param>
		/// <param name="value">The resulting value</param>
		public void Add(TFrom key, TTo value)
		{
			_map.Add(key, value);
		}

		/// <summary>
		/// Adds a new translation to the translation table
		/// </summary>
		/// <param name="key">The key to search for</param>
		/// <param name="value">The resulting value</param>
		/// <remarks>Deprecated with error in 2.0 in favor of Add() for collection initializers.</remarks>
		[Obsolete("This legacy method is no longer used, use Add() instead.", true)]
		public void AddTranslation(TFrom key, TTo value)
		{
			throw new NotImplementedException("This method is obsolete and no longer implemented.");
		}

		/// <summary>
		/// Adds a set of new translations to the translation table
		/// </summary>
		/// <param name="pairs">Adds a sequence of key value pairs</param>
		public void Add(IEnumerable<KeyValuePair<TFrom, TTo>> pairs)
		{
			if (pairs == null)
			{
				throw new ArgumentNullException("pairs");
			}

			pairs.ForEach(pair => _map.Add(pair));
		}

		/// <summary>
		/// Method to see if the translator interface contains the given from value.
		/// </summary>
		/// <param name="fromValue">The value to check.</param>
		/// <returns>True if the translator contains the from value.</returns>
		public bool Contains(TFrom fromValue)
		{
			return _map.ContainsKey(fromValue);
		}

		/// <summary>
		/// Adds a set of new translations to the translation table
		/// </summary>
		/// <param name="pairs">Adds an array of key value pairs</param>
		/// <remarks>Deprecated with error in 2.0 in favor of Add() for collection initializers.</remarks>
		[Obsolete("This legacy method is no longer used, use Add() instead.", true)]
		public void AddTranslations(KeyValuePair<TFrom, TTo>[] pairs)
		{
			throw new NotImplementedException("This method is obsolete and no longer implemented.");
		}

		/// <summary>
		/// Clears all existing translations and defaults
		/// </summary>
		public void Clear()
		{
			_map.Clear();
		}

		/// <summary>
		/// Get an enumerator to walk through the list
		/// </summary>
		/// <returns>An enumerator around the translator map</returns>
		public IEnumerator<KeyValuePair<TFrom, TTo>> GetEnumerator()
		{
			return _map.GetEnumerator();
		}

		/// <summary>
		/// Get an enumerator to walk through the list
		/// </summary>
		/// <returns>An enumerator around the translator map</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}