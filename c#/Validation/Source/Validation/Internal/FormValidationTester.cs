using System.Text.RegularExpressions;

namespace SharedAssemblies.General.Validation.Internal
{
	/// <summary>
	/// <para>
	/// Validator is a general validation class that performs pre-defined input string validation 
	/// and exposes standard regular expression patters for use.
	/// </para>
	/// <para>
	/// To perform hands-off validation, simply pass in the string you want to check and the
	/// enumeration value (FormValidationType) specifying what type of validation to
	/// perform. Null values are handled by always returning false from the validation method.
	/// </para>
	/// <para>
	/// You can also tie these standard patterns to ASP.NET RegularExpressionValidator controls by
	/// specifying the pattern from this class (via the public property) and assigning it in the
	/// code-behind class via the RegularExpressionValidtor.ValidationExpression property.
	/// </para>
	/// </summary>
	internal static class FormValidationTester
	{
		#region Private Member Regular Expression String Patterns

		/// <summary>Regular expression pattern for an address</summary>
		private static readonly string _address = @"^[#0-9a-zA-Z' \n\r,\.\-&/]+$";

		/// <summary>Regular expression pattern for alphabetic entries</summary>
		private static readonly string _alpha = @"^[A-Za-z]+$";

		/// <summary>Regular expression pattern for alphabetic entries with spaces</summary>
		private static readonly string _alphaAndSpaces = @"^[A-Za-z ]+$";

		/// <summary>Regular expression pattern for alpha-numeric entries</summary>
		private static readonly string _alphaAndDigits = @"^[A-Za-z0-9]+$";

		/// <summary>Regular expression pattern for alpha-numeric with spaces</summary>
		private static readonly string _alphaAndDigitsAndSpaces = @"^[A-Za-z0-9 ]+$";

		/// <summary>Regular expression pattern alpha-numeric with underscores</summary>
		private static readonly string _alphaAndDigitsAndUnderscores = @"^[A-Za-z0-9_]+$";

		/// <summary>Regular expression pattern for alpha-numeric with space and underscores</summary>
		private static readonly string _alphaAndDigitsAndUnderscoresAndSpaces = @"^[A-Za-z0-9 _]+$";

		/// <summary>Regular expression pattern for alpha-numeric with special chars</summary>
		private static readonly string _alphaAndDigitsAndSpacesAndSpecialCharacters =
				@"^[A-Za-z0-9 *~!@#$%^&*()|\\?/<>=+_]+$";

		/// <summary>Regular expression pattern for cities</summary>
		private static readonly string _city = @"^[#a-zA-Z' ,\.\-\&]+$";

		/// <summary>Regular expression pattern for currency amounts</summary>
		private static readonly string _currency =
			@"^-?\$?(0\.[0-9]{2}|[1-9]{1}[0-9]{0,2}(,[0-9]{3})*(\.[0-9]{2})?)$";

		/// <summary>Regular expression pattern for date fields</summary>
		private static readonly string _date =
			@"^(?:(?:(?:0?[13578]|1[02])(\/|-|\.)31)\1|(?:(?:0?[13-9]|1[0-2])(\/|-|\.)(?:29|30)\2))" 
			+ @"(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:0?2(\/|-|\.)29\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|" 
			+ @"[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:"
			+ @"1[0-2]))(\/|-|\.)(?:0?[1-9]|1\d|2[0-8])\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$";

		/// <summary>Regular expression pattern for email addresses</summary>
		private static readonly string _email =
			@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))"
			+ @"([0-9a-zA-Z]{2,6})(\]?)$";

		/// <summary>Regular expression pattern for freeform text fields</summary>
		private static readonly string _freeFormText = 
			@"^[A-Za-z0-9 _.,'?!;:/\\()&*%^$#@~\-+=<>\r\n""]+$";

		/// <summary>Regular expression pattern for freeform text fields allowing all Latin and 
		/// Chinese characters.</summary>
		private static readonly string _freeFormLatinAndChineseText =
			@"^[\x20-\x7E\u2000-\u201f\u3000-\u303F\u3200-\u32FF\u3300-\u33FF\u4e00-\u9fa5"
			+ @"\uFF00-\uFFEF\r\t\n]+$";

		/// <summary>Regular expression pattern for an integer entry field</summary>
		private static readonly string _integer = @"^-?\d+$";

		/// <summary>Regular expression pattern for unsigned interger numbers.</summary>
		private static readonly string _integerUnsigned = @"^\d+$";		

		/// <summary>Regular expression pattern for an IPv4 address</summary>
		private static readonly string _ipAddressIpv4 =
			@"^\b(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\b$";

		/// <summary>Regular expression pattern for an IPv6 address</summary>
		private static readonly string _ipAddressIpv6 =
			@"^((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|" 
			+ @"((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-" 
			+ @"Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(.(25[0-5" 
			+ @"]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|" 
			+ @"((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(.(25[0-5]|2[0-4]\d|1\d\d|[1-9]" 
			+ @"?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){" 
			+ @"0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(" 
			+ @"([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2" 
			+ @"[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4" 
			+ @"}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1" 
			+ @"-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:" 
			+ @"[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-" 
			+ @"9]?\d)){3}))|:)))(%.+)?$";

		/// <summary>Regular expression pattern for a name entry</summary>
		private static readonly string _name = @"^[a-zA-Z' \-]+$";

		/// <summary>Regular expression pattern for a number field</summary>
		private static readonly string _number = @"^[-+]?(\d+|\d*\.\d+)$";

		/// <summary>Regular expression pattern for a valid password</summary>
		private static readonly string _password =
			@"\A(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[\.\$\^\{\}\[\]\(\)\|\*\+\?\\;:,=@#%&!""' ])" 
			+ @"[a-zA-Z0-9\.\$\^\{\}\[\]\(\)\|\*\+\?\\;:,=@#%&!""' ]{8,}\z";

		/// <summary>Regular expression pattern for a phone number</summary>
		private static readonly string _phone =
			@"^(1? ?\(?([1-9]\d{2})\)?( ?| ?[-.] ?)?)?([1-9]\d{2})( ?| ?[-.] ?)?(\d{4})( [xX]\d{1,5})?$";

		/// <summary>Regular expression pattern for special characters</summary>
		private static readonly string _specialCharacters = @"[*~!@#$%^&*()|\\?/<>=+_]";

		/// <summary>Regular expression pattern for a social security number</summary>
		private static readonly string _ssn = @"^\d{3}(-\d{2}-|\d{2})\d{4}$";

		/// <summary>Regular expression pattern for a tax id number</summary>
		private static readonly string _tin = @"^\d{2}-?\d{7}$";

		/// <summary>Regular expression pattern for a URL</summary>
		private static readonly string _url =
			@"^https?://(([0-9]{1,3}\.){3}[0-9]{1,3}|([0-9a-z])+/?|([0-9a-z_!~*'()-]+\.)*([0-9a-z]" 
			+ @"[0-9a-z-]{0,61})?[0-9a-z]\.[a-z]{2,6})(:[0-9]{1,4})?((/?)|(/[0-9a-zA-Z_!~*'().;?:@" 
			+ @"&=+$,%#-]+)+/?)$";

		/// <summary>Regular expression pattern a zip code</summary>
		private static readonly string _zipCode = @"^(\d{5}-\d{4}|\d{5}|\d{9})$";

		#endregion

		#region Private Member Regular Expression Objects

		/// <summary>Compiled regular expression for an address</summary>
		private static readonly Regex _addressPattern = 
			new Regex(_address, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for alphabetic only entry</summary>
		private static readonly Regex _alphaPattern = 
			new Regex(_alpha, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for alphabetic entry with spaces</summary>
		private static readonly Regex _alphaAndSpacesPattern = 
			new Regex(_alphaAndSpaces, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for alphanumeric entry</summary>
		private static readonly Regex _alphaAndDigitsPattern = 
			new Regex(_alphaAndDigits, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expressio for alphanumeric with spacesn</summary>
		private static readonly Regex _alphaAndDigitsAndSpacesPattern = 
			new Regex(_alphaAndDigitsAndSpaces, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for alphanumeric with underscores</summary>
		private static readonly Regex _alphaAndDigitsAndUnderscoresPattern =
			new Regex(_alphaAndDigitsAndUnderscores, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for alphanumeric with underscores and spaces</summary>
		private static readonly Regex _alphaAndDigitsAndUnderscoresAndSpacesPattern =
			new Regex(_alphaAndDigitsAndUnderscoresAndSpaces,
			          RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for alphanumeric with spaces and special chars</summary>
		private static readonly Regex _alphaAndDigitsAndSpacesAndSpecialCharactersPattern =
			new Regex(_alphaAndDigitsAndSpacesAndSpecialCharacters,
			          RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for city entry</summary>
		private static readonly Regex _cityPattern = 
			new Regex(_city, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for currency entry</summary>
		private static readonly Regex _currencyPattern = 
			new Regex(_currency, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for date entry</summary>
		private static readonly Regex _datePattern = 
			new Regex(_date, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for email address entry</summary>
		private static readonly Regex _emailPattern = 
			new Regex(_email, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for a free-form text entry</summary>
		private static readonly Regex _freeFormTextPattern = 
			new Regex(_freeFormText, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for a free-form text entry</summary>
		private static readonly Regex _freeFormLatinAndChineseTextPattern = 
			new Regex(_freeFormLatinAndChineseText, 
					  RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for an integer entry</summary>
		private static readonly Regex _integerPattern = 
			new Regex(_integer, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for an unsigned integer entry</summary>
		private static readonly Regex _integerUnsignedPattern =
			new Regex(_integerUnsigned, RegexOptions.Compiled | RegexOptions.CultureInvariant);			

		/// <summary>Compiled regular expression for an IPv4 address</summary>
		private static readonly Regex _ipAddressIpv4Pattern = 
			new Regex(_ipAddressIpv4, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for an IPv6 address</summary>
		private static readonly Regex _ipAddressIpv6Pattern = 
			new Regex(_ipAddressIpv6, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for a name entry</summary>
		private static readonly Regex _namePattern = 
			new Regex(_name, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for numeric entry</summary>
		private static readonly Regex _numberPattern = 
			new Regex(_number, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for valid passwords</summary>
		private static readonly Regex _passwordPattern = 
			new Regex(_password, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for phone number entry</summary>
		private static readonly Regex _phonePattern = 
			new Regex(_phone, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for special characters</summary>
		private static readonly Regex _specialCharactersPattern = 
			new Regex(_specialCharacters, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for social security numbers</summary>
		private static readonly Regex _ssnPattern = 
			new Regex(_ssn, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for tax id numbers</summary>
		private static readonly Regex _tinPattern = 
			new Regex(_tin, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for a URL</summary>
		private static readonly Regex _urlPattern = 
			new Regex(_url, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for a zip code</summary>
		private static readonly Regex _zipCodePattern = 
			new Regex(_zipCode, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		#endregion

		/// <summary>
		/// Function used to add heading/trailing space to the expression. This is mainly used to 
		/// populate the regular expression validator for the client side. In those situations,  
		/// we might want to allow heading/trailing space for input. On server side, we can always 
		/// call Trim() before it passed to the validation function or when extracting from input
		/// controls.
		/// </summary>
		/// <param name="patternString">Original regular expression to convert to allowing heading/
		/// trailing spaces.</param>
		/// <returns>New regular expression pattern which is the original plus allowing for heading
		/// /trailing spaces.</returns>
		public static string AddHeadingTrailingSpace(string patternString)
		{
			string newPattern = string.Empty;
			Regex capturePattern = new Regex(@"^[\^]?(?<expression>.+?)[\$]?$", 
											 RegexOptions.Compiled 
											 | RegexOptions.CultureInvariant 
											 | RegexOptions.ExplicitCapture);

			MatchCollection collections = capturePattern.Matches(patternString);
			if (collections.Count > 0)
			{
				Match collection = collections[0];
				newPattern = string.Format(@"^[ ]*{0}[ ]*$", collection.Groups["expression"].Value);
			}

			return newPattern;
		}


		/// <summary>
		/// Validates the input string with the specified pattern.
		/// </summary>
		/// <param name="input">The input to validate against.</param>
		/// <param name="whichPattern">The pattern to use.</param>
		/// <returns><c>true</c> if input matches pattern; <c>false</c> otherwise.</returns>
		public static bool ValidateInput(string input, FormValidationType whichPattern)
		{
			bool isValid = false;

			// Check for null first since Regex.Match throws an execption if the input is null.
			if (input != null)
			{
				switch (whichPattern)
				{
					case FormValidationType.Address:
						isValid = _addressPattern.Match(input).Success;
						break;
					case FormValidationType.Alpha:
						isValid = _alphaPattern.Match(input).Success;
						break;
					case FormValidationType.AlphaAndSpaces:
						isValid = _alphaAndSpacesPattern.Match(input).Success;
						break;
					case FormValidationType.AlphaAndDigits:
						isValid = _alphaAndDigitsPattern.Match(input).Success;
						break;
					case FormValidationType.AlphaAndDigitsAndSpaces:
						isValid = _alphaAndDigitsAndSpacesPattern.Match(input).Success;
						break;
					case FormValidationType.AlphaAndDigitsAndSpacesAndSpecialCharacters:
						isValid = _alphaAndDigitsAndSpacesAndSpecialCharactersPattern.Match(input).Success;
						break;
					case FormValidationType.AlphaAndDigitsAndUnderscoresAndSpaces:
						isValid = _alphaAndDigitsAndUnderscoresAndSpacesPattern.Match(input).Success;
						break;
					case FormValidationType.AlphaAndDigitsAndUnderscores:
						isValid = _alphaAndDigitsAndUnderscoresPattern.Match(input).Success;
						break;
					case FormValidationType.City:
						isValid = _cityPattern.Match(input).Success;
						break;
					case FormValidationType.Currency:
						isValid = _currencyPattern.Match(input).Success;
						break;
					case FormValidationType.Date:
						isValid = _datePattern.Match(input).Success;
						break;
					case FormValidationType.Email:
						isValid = _emailPattern.Match(input).Success;
						break;
					case FormValidationType.FreeFormText:
						isValid = _freeFormTextPattern.Match(input).Success;
						break;
					case FormValidationType.FreeFormLatinAndChineseText:
						isValid = _freeFormLatinAndChineseTextPattern.Match(input).Success;
						break;						
					case FormValidationType.Integer:
						isValid = _integerPattern.Match(input).Success;
						break;
					case FormValidationType.IntegerUnsigned:
						isValid = _integerUnsignedPattern.Match(input).Success;
						break;						
					case FormValidationType.IpAddressIPv4:
						isValid = _ipAddressIpv4Pattern.Match(input).Success;
						break;
					case FormValidationType.IpAddressIPv6:
						isValid = _ipAddressIpv6Pattern.Match(input).Success;
						break;
					case FormValidationType.Name:
						isValid = _namePattern.Match(input).Success;
						break;
					case FormValidationType.Number:
						isValid = _numberPattern.Match(input).Success;
						break;
					case FormValidationType.Password:
						isValid = _passwordPattern.Match(input).Success;
						break;
					case FormValidationType.Phone:
						isValid = _phonePattern.Match(input).Success;
						break;
					case FormValidationType.SpecialCharacters:
						isValid = _specialCharactersPattern.Match(input).Success;
						break;
					case FormValidationType.Ssn:
						isValid = _ssnPattern.Match(input).Success;
						break;
					case FormValidationType.TaxIdentificationNumber:
						isValid = _tinPattern.Match(input).Success;
						break;
					case FormValidationType.Url:
						isValid = _urlPattern.Match(input).Success;
						break;
					case FormValidationType.ZipCode:
						isValid = _zipCodePattern.Match(input).Success;
						break;
					default:
						break;
				}
			}
			return isValid;
		}
	}
}