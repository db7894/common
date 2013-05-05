using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Core.Conversions
{
	/// <summary>
	/// <para>
	/// Creates a translator that converts from an enumeration to a different type.  This is a specialized translator
	/// that is extremely performant and uses an array as the backing data store for the enumeration values.
	/// </para>
	/// <para>
	/// It should be noted that this version is a more efficient use of space if the enumeration is very packed,
	/// that is, if there are very few gaps numerically between the values.
	/// </para>
	/// </summary>
	/// <typeparam name="TEnum">The "FROM" type of the translation pair, must be an enumeration type.</typeparam>
	/// <typeparam name="TTo">The "TO" type of the translation pair</typeparam>
	public sealed class EnumTranslator<TEnum, TTo> : ITranslator<TEnum, TTo> where TEnum : struct
	{
		/// <summary>default if can't translation</summary>
		private readonly TTo _defaultValue;

		/// <summary>Contains flag as to whether the value has translation.</summary>
		private readonly bool[] _hasValues;

		/// <summary>Contains the values to translate to.</summary>
		private readonly TTo[] _toValues;

		/// <summary>This is the offset to normalize the minimal enum value back to zero.</summary>
		private readonly int _startingOffset;

		/// <summary>
		/// Gets the default to value if a lookup fails
		/// </summary>
		public TTo DefaultValue
		{
			get { return _defaultValue; }
		}

		/// <summary>
		/// Property that returns the number of elements in the lookup table
		/// </summary>
		public int TranslationCount
		{
			get { return _hasValues.Count(v => v); } // Number of defined values
		}

		/// <summary>
		/// Indexes the translator.  On get it will lookup the type calling
		/// Translate.  On set it will Add a new lookup with the given value.
		/// </summary>
		/// <param name="index">The value to lookup</param>
		/// <returns>The translated value</returns>
		public TTo this[TEnum index]
		{
			get { return Translate(index); }
			set { Add(index, value); }
		}

		/// <summary>
		/// Create a translator with a default to and from value specified
		/// </summary>
		/// <param name="defaultValue">Value to use if translate fails.</param>
		/// <param name="translations">An optional sequence of initial translations to add to this translator.</param>
		public EnumTranslator(TTo defaultValue = default(TTo), IEnumerable<KeyValuePair<TEnum, TTo>> translations = null)
		{
			_defaultValue = defaultValue;

			// get the type of TEnum
			var type = typeof(TEnum);

			// make sure that the type of TEnum is indeed an enumeration.
			if (!type.IsEnum || type.GetEnumUnderlyingType() != typeof(int) || type.GetAttribute<FlagsAttribute>() != null)
			{
				throw new InvalidOperationException("Generic type argument TEnum must be non-flags enum with int underlying type.");
			}

			// find the bounds of the enumeration
			var range = type.GetEnumValues().Cast<int>().GetRange();
			var length = range.Item2 - range.Item1 + 1;

			// set up arrays
			_startingOffset = range.Item1;
			_hasValues = new bool[length];
			_toValues = new TTo[length];

			// pre-set values
			Clear();

			// if initial translations were passed in, add them
			if (translations != null)
			{
				Add(translations);
			}
		}

		/// <summary>
		/// Performs a translation using the table, returns the default from value
		/// if cannot find a matching result.
		/// </summary>
		/// <param name="value">Value to translate</param>
		/// <returns>The translated value</returns>
		public TTo Translate(TEnum value)
		{
			// loop through table looking for result
			var reducedIndex = EnumConverter<TEnum>.ToInt(value) - _startingOffset;

			try
			{
				return _toValues[reducedIndex];
			}

			catch (IndexOutOfRangeException)
			{
				return _defaultValue;
			}
		}

		/// <summary>
		/// Adds a new translation to the translation table
		/// </summary>
		/// <param name="key">The key to search for</param>
		/// <param name="value">The resulting value</param>
		public void Add(TEnum key, TTo value)
		{
			var reducedIndex = EnumConverter<TEnum>.ToInt(key) - _startingOffset;

			if (reducedIndex < 0 || reducedIndex >= _toValues.Length)
			{
				throw new ArgumentOutOfRangeException("key", "Value of key must be within min and max for enumeration.");
			}

			if (_hasValues[reducedIndex])
			{
				throw new InvalidOperationException("Cannot add key, key already exists");
			}

			_hasValues[reducedIndex] = true;
			_toValues[reducedIndex] = value;
		}

		/// <summary>
		/// Adds a set of new translations to the translation table
		/// </summary>
		/// <param name="pairs">Adds a sequence of key value pairs</param>
		public void Add(IEnumerable<KeyValuePair<TEnum, TTo>> pairs)
		{
			if (pairs == null)
			{
				throw new ArgumentNullException("pairs");
			}

			pairs.ForEach(pair => Add(pair.Key, pair.Value));
		}

		/// <summary>
		/// Method to see if the translator interface contains the given from value.
		/// </summary>
		/// <param name="fromValue">The value to check.</param>
		/// <returns>True if the translator contains the from value.</returns>
		public bool Contains(TEnum fromValue)
		{
			// see if the enum has a definition already...
			return _hasValues[EnumConverter<TEnum>.ToInt(fromValue) - _startingOffset];			
		}

		/// <summary>
		/// Clears all existing translations and defaults
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < _toValues.Length; i++)
			{
				_hasValues[i] = false;
				_toValues[i] = _defaultValue;
			}
		}

		/// <summary>
		/// Get an enumerator to walk through the list
		/// </summary>
		/// <returns>An enumerator around the translator map</returns>
		public IEnumerator<KeyValuePair<TEnum, TTo>> GetEnumerator()
		{
			for (int i = 0; i < _toValues.Length; i++)
			{
				if (_hasValues[i])
				{
					yield return new KeyValuePair<TEnum, TTo>(EnumConverter<TEnum>.ToEnum(i), _toValues[i]);
				}
			}
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
