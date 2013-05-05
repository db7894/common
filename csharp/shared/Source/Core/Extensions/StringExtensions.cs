using System;
using System.Collections.Generic;
using System.Text;

namespace SharedAssemblies.Core.Extensions
{
	/// <summary>
	/// Utility class for string manipulation.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Default masking character used in a mask.
		/// </summary>
		public static readonly char DefaultMaskCharacter = '*';


		/// <summary>
		/// Returns true if the current string is null or empty.
		/// </summary>
		/// <param name="input">The string to test.</param>
		/// <returns>True if null or empty, false otherwise.</returns>
		public static bool IsNullOrEmpty(this string input)
		{
			return string.IsNullOrEmpty(input);
		}

		/// <summary>
		/// Version of IsNullOrWhitespace until we can get all implementations up to .NET 4.0
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>True if the string is just null or whitespace</returns>
		public static bool IsNullOrWhitespace(this string value)
		{
			if (value != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsWhiteSpace(value[i]))
					{
						return false;
					}
				}
			}

			return true;			
		}

		/// <summary>
		/// The negation of IsNullOrWhitespace.  Returns true if the string is not null and has a non-whitespace
		/// character.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>False if the string is just null or whitespace</returns>
		public static bool IsNotNullOrWhitespace(this string value)
		{
			return !value.IsNullOrWhitespace();
		}

		/// <summary>
		/// Returns false if the current string is null or empty.
		/// </summary>
		/// <param name="input">The string to test.</param>
		/// <returns>False if null or empty, true otherwise.</returns>
		public static bool IsNotNullOrEmpty(this string input)
		{
			return !string.IsNullOrEmpty(input);
		}


		/// <summary>
		/// Trims a string.  If the string is null, it will set to string.Empty instead.
		/// </summary>
		/// <remarks>Version 1.1</remarks>
		/// <param name="input">The string to trim.</param>
		/// <returns>The trimmed string or string.Empty if input is null.</returns>
		public static string NullSafeTrim(this string input)
		{
			return input != null ? input.Trim() : string.Empty;
		}


		/// <summary>
		/// Returns a null-safe length of the string.  If the string is null, returns zero.
		/// </summary>
		/// <remarks>Version 1.2</remarks>
		/// <param name="input">The string to check length upon.</param>
		/// <returns>The length of the string or zero if null.</returns>
		public static int NullSafeLength(this string input)
		{
			return input != null ? input.Length : 0;
		}


		/// <summary>
		/// Returns true if the string is non-null and at least the specified number of characters.
		/// </summary>
		/// <param name="value">The string to check.</param>
		/// <param name="length">The minimum length.</param>
		/// <returns>True if string is non-null and at least the length specified.</returns>
		/// <exception>throws ArgumentOutOfRangeException if length is not a non-negative number.</exception>
		public static bool IsLengthAtLeast(this string value, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, 
													  "The length must be a non-negative number.");    
			}

			return value != null
					   ? value.Length >= length
					   : false;
		}

		/// <summary>
		/// Mask the source string with the mask char except for the last exposed digits.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="maskChar">The character to use to mask the source.</param>
		/// <param name="numExposed">Number of characters exposed in masked value.</param>
		/// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, char maskChar, int numExposed, MaskStyle style)
		{
			var maskedString = sourceValue;

			if (sourceValue.IsLengthAtLeast(numExposed))
			{
				var builder = new StringBuilder(sourceValue.Length);
				int index = maskedString.Length - numExposed;

				if(style == MaskStyle.AlphaNumericOnly)
				{
					CreateAlphaNumMask(builder, sourceValue, maskChar, index);
				}
				else
				{
					builder.Append(maskChar, index);
				}

				builder.Append(sourceValue.Substring(index));
				maskedString = builder.ToString();
			}

			return maskedString;
		}

		/// <summary>
		/// Mask the source string with the mask char except for the last exposed digits.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="maskChar">The character to use to mask the source.</param>
		/// <param name="numExposed">Number of characters exposed in masked value.</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, char maskChar, int numExposed)
		{
			return Mask(sourceValue, maskChar, numExposed, MaskStyle.All);
		}

		/// <summary>
		/// Mask the source string with the mask char.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="maskChar">The character to use to mask the source.</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, char maskChar)
		{
			return Mask(sourceValue, maskChar, 0, MaskStyle.All);
		}

		/// <summary>
		/// Mask the source string with the default mask char except for the last exposed digits.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="numExposed">Number of characters exposed in masked value.</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, int numExposed)
		{
			return Mask(sourceValue, DefaultMaskCharacter, numExposed, MaskStyle.All);
		}

		/// <summary>
		/// Mask the source string with the default mask char.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue)
		{
			return Mask(sourceValue, DefaultMaskCharacter, 0, MaskStyle.All);
		}

		/// <summary>
		/// Mask the source string with the mask char.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="maskChar">The character to use to mask the source.</param>
		/// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, char maskChar, MaskStyle style)
		{
			return Mask(sourceValue, maskChar, 0, style);
		}

		/// <summary>
		/// Mask the source string with the default mask char except for the last exposed digits.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="numExposed">Number of characters exposed in masked value.</param>
		/// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, int numExposed, MaskStyle style)
		{
			return Mask(sourceValue, DefaultMaskCharacter, numExposed, style);
		}

		/// <summary>
		/// Mask the source string with the default mask char.
		/// </summary>
		/// <param name="sourceValue">Original string to mask.</param>
		/// <param name="style">The masking style to use (all characters or just alpha-nums).</param>
		/// <returns>The masked account number.</returns>
		public static string Mask(this string sourceValue, MaskStyle style)
		{
			return Mask(sourceValue, DefaultMaskCharacter, 0, style);
		}

		/// <summary>
		/// Resolve place holders within the dataStr using the list of KeyValuePairs provided.
		/// </summary>
		/// <param name="sourceValue">The string with tokens to substitute.</param>
		/// <param name="tokenSet">The matching values of the tokens to substitute.</param>
		/// <returns>A resulting string with its place holders resolved.</returns>
		public static string Resolve(this string sourceValue,
									 IEnumerable<KeyValuePair<string, string>> tokenSet)
		{
			string resolvedString = null;

			if (sourceValue != null && tokenSet != null)
			{
				var builder = new StringBuilder(sourceValue);

				tokenSet.ForEach(entry => builder.Replace(entry.Key, entry.Value));

				resolvedString = builder.ToString();
			}

			return resolvedString;
		}

		/// <summary>
		/// Repeat the given string the specified number of times.
		/// </summary>
		/// <param name="input">The string to repeat.</param>
		/// <param name="count">The number of times to repeat the string.</param>
		/// <returns>The concatenated repeated string.</returns>
		public static string Repeat(this string input, int count)
		{
			var buffer = new StringBuilder();

			for (int i = count; i > 0; --i)
			{
				buffer.Append(input);
			}

			return buffer.ToString();
		}

		/// <summary>
		/// Repeat the given char the specified number of times.
		/// </summary>
		/// <param name="input">The char to repeat.</param>
		/// <param name="count">The number of times to repeat the string.</param>
		/// <returns>The repeated char string.</returns>
		public static string Repeat(this char input, int count)
		{
			return new string(input, count);
		}


		/// <summary>
		/// Returns a null-safe version of the string which returns self if not null
		/// and string.Empty if null.
		/// </summary>
		/// <param name="input">The string to check.</param>
		/// <returns>The current string or string.Empty if current string is null.</returns>
		/// <remarks>Version 1.2</remarks>
		public static string NullSafe(this string input)
		{
			return input ?? string.Empty;
		}

		/// <summary>
		/// If the string is longer than the specified number of chars, will truncate at that point and append an
		/// ellipsis instead.
		/// </summary>
		/// <param name="text">The text to truncate.</param>
		/// <param name="maxLength">The max length to display.</param>
		/// <returns>The truncated display string.</returns>
		public static string Truncate(this string text, int maxLength)
		{
			if (maxLength < 0)
			{
				throw new ArgumentOutOfRangeException("maxLength");
			}

			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			if (!string.IsNullOrEmpty(text))
			{
				if (text.Length > maxLength)
				{
					return text.Remove(maxLength) + "...";
				}
			}

			return text;
		}

		/// <summary>
		/// Masks all characters for the specified length.
		/// </summary>
		/// <param name="buffer">String builder to store result in.</param>
		/// <param name="source">The source string to pull non-alpha numeric characters.</param>
		/// <param name="mask">Masking character to use.</param>
		/// <param name="length">Length of the mask.</param>
		private static void CreateAlphaNumMask(StringBuilder buffer, string source, char mask, int length)
		{
			for(int i = 0; i < length; i++)
			{
				buffer.Append(char.IsLetterOrDigit(source[i])
								? mask
								: source[i]);
			}
		}
	}
}