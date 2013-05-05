using System;
using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.Core.Containers;


namespace SharedAssemblies.Core.Conversions
{
	/// <summary>
	/// Factory class that creates translators for given translations.
	/// </summary>
	public static class EnumTranslator
	{
		/// <summary>
		/// <para>
		/// This static method constructs an inverse EnumTranslator to the EnumTranslator instance passed in.  This creates the inverse translator at the
		/// time the Create() method is called, which means any translations added to the original after construction will not be in the inverse
		/// EnumTranslator instance.
		/// </para>
		/// <para>
		/// Note, if there are duplicate TTo values, an exception will be thrown on construction.
		/// </para>
		/// </summary>
		/// <typeparam name="TFrom">The original from type.</typeparam>
		/// <typeparam name="TTo">The original to type.</typeparam>
		/// <param name="translator">The original EnumTranslator instance from TFrom to TTo.</param>
		/// <param name="defaultValue">The default value if translation in the reverse translator fails.</param>
		/// <returns>A new inverse EnumTranslator instance from TTo to TFrom.</returns>
		public static EnumTranslator<TTo, TFrom> Reverse<TFrom, TTo>(ITranslator<TFrom, TTo> translator, TFrom defaultValue = default(TFrom))
			where TTo : struct
		{
			return Create(translator.Select(pair => KeyValuePairFactory.Create(pair.Value, pair.Key)), defaultValue);
		}

		/// <summary>
		/// <para>
		/// This static method constructs a EnumTranslator based on the translator's sequence of translations and default value passed in.  
		/// </para>
		/// <para>
		/// Note, if there are duplicate TFrom values, an exception will be thrown on construction.
		/// </para>
		/// </summary>
		/// <typeparam name="TFrom">The original from type.</typeparam>
		/// <typeparam name="TTo">The original to type.</typeparam>
		/// <param name="translator">The translation to copy initial translations and default value from.</param>
		/// <returns>A new translator loaded with the sequence of initial translations and default value.</returns>
		public static EnumTranslator<TFrom, TTo> Create<TFrom, TTo>(ITranslator<TFrom, TTo> translator)
			where TFrom : struct
		{
			if (translator == null)
			{
				throw new ArgumentNullException("translator");
			}

			return Create(translator, translator.DefaultValue);
		}

		/// <summary>
		/// <para>
		/// This static method constructs a EnumTranslator based on the sequence of translations and default value passed in.  
		/// </para>
		/// <para>
		/// Note, if there are duplicate TFrom values, an exception will be thrown on construction.
		/// </para>
		/// </summary>
		/// <typeparam name="TFrom">The original from type.</typeparam>
		/// <typeparam name="TTo">The original to type.</typeparam>
		/// <param name="translations">The initial translations to use in the new translator.</param>
		/// <param name="defaultValue">The default value to use if no translation is found.</param>
		/// <returns>A new translator loaded with the sequence of initial translations and default value.</returns>
		public static EnumTranslator<TFrom, TTo> Create<TFrom, TTo>(IEnumerable<KeyValuePair<TFrom, TTo>> translations,
			TTo defaultValue = default(TTo)) where TFrom : struct
		{
			if (translations == null)
			{
				throw new ArgumentNullException("translations");
			}

			return new EnumTranslator<TFrom, TTo>(defaultValue, translations);
		}
	}
}