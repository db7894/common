using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// <para>
	/// Validator is a general validation class that performs pre-defined input string validation 
	/// and exposes standard regular expression patters for use.
	/// </para>
	/// <para>
	/// To perform hands-off validation, simply pass in the string you want to check and the
	/// enumeration value (RegularExpressionPatternType) specifying what type of validation to
	/// perform. Null values are handled by always returning false from the validation method.
	/// </para>
	/// <para>
	/// You can also tie these standard patterns to ASP.NET RegularExpressionValidator controls by
	/// specifying the pattern from this class (via the public property) and assigning it in the
	/// code-behind class via the RegularExpressionValidtor.ValidationExpression property.
	/// </para>
	/// </summary>
	public static class Validator
	{
		#region Private Member Regular Expression String Patterns

		/// <summary>Regular expression pattern for a brokerage account number</summary>
		private static readonly string _accountNumber = @"^[0-9]{8}$";

		/// <summary>Regular expression pattern for a back-office account number</summary>
		private static readonly string _accountNumberBackOffice = @"^[0-9]{10}$";

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

		/// <summary>Regular expression pattern to match a typical US equity symbol.</summary>
		private static readonly string _equitySymbol = @"^[A-Za-z]{1,5}$";

		/// <summary>Regular expression pattern to match US and CA equity symbols.</summary>
		private static readonly string _equitySymbolUsAndCanadian = 
			@"^(\.?[A-Za-z]{1,5}((/(A{1,3}|B))|(\.CA))?)$";

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

		/// <summary> Varying formats of regular expressions to represent all possible IPV6 address types. </summary>
		private static readonly string _ipv6Hex =
			@"(?!.{{47,}})(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}(?:(?:%[0-9]{1,3})?|(?:%eth[0-9]{1,3})?|(?:/(?:1[0,1]{1}\d|12[0-8]{1}|0?\d{1,2}))?)";
		private static readonly string _ipv6HexCompressed =
			@"(?!.{{47,}})(?:(?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)::(?:(?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)(?:(?:%[0-9]{1,3})?|(?:%eth[0-9]{1,3})?|(?:/(?:1[0,1]{1}\d|12[0-8]{1}|0?\d{1,2}))?)";
		private static readonly string _ipv6Ipv4 =
			@"(?!.{{46,}})(?:(?:[0-9A-Fa-f]{1,4}:){6,6})(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)";
		private static readonly string _ipv6Ipv4Compressed =
			@"(?!.{{46,}})(?:(?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)::(?:(?:[0-9A-Fa-f]{1,4}:)*)(?:(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|[01]?\d\d?)";

		private static readonly string _ipAddressIpv6 = string.Format("(^{0}$)|(^{1}$)|(^{2}$)|(^{3}$)",
			_ipv6Hex, _ipv6HexCompressed, _ipv6Ipv4, _ipv6Ipv4Compressed);

		/// <summary>Regular expression pattern for a name entry</summary>
        private static readonly string _name = @"^[a-zA-Z' \-\.,]+$";

		/// <summary>Regular expression pattern for a number field</summary>
		private static readonly string _number = @"^[-+]?(\d+|\d*\.\d+)$";

		/// <summary>
        /// Regular expression pattern for option symbols at the back-office.
        /// Basic explanation:
        /// <code>^</code>: Match the start of the line or string (depending on whether you set the option RegexOptions.Multiline or not).
        /// <code>(?=.{5}\s\S)</code>: Assert that the symbol match is exactly five characters long followed by a 
        /// space and then a non-space using lookahead.
        /// <code>[A-Za-z]{1,5}\d? +</code>: Match 1 to 5 alpha characters and an optional digit, followed by at least one space
        /// <code>\b</code>: Assert that we are now at a word boundary (i.e., that the next character is alphanumeric)
        /// </summary>
        private static readonly string _optionSymbolBackOffice = @"^(?=.{5}\s\S)([A-Za-z]{1,5}\d? +\b)[0-9]{6}[CcPp][0-9]{8}$";

		/// <summary>
        /// Regular expression pattern for bashwork option symbols. 
        /// Same as backoffice plus a leading period and lookahead has a length of 6.
        /// </summary>
        private static readonly string _optionSymbolBashwork = @"^(?=.{6}\s\S)\.([A-Za-z]{1,5}\d? +\b)[0-9]{6}[CcPp][0-9]{8}$";

		/// <summary>Regular expression pattern for displayable option symbols</summary>
		private static readonly string _optionSymbolDisplayable =
			@"^[A-Za-z0-9 ]{1,6} [0-9]{1,5}\.[0-9]{2,3} [A-Za-z]{3} [0-9]{2} ((WK|Q){1}[0-9]{1} ){0,1}" 
			+ @"(AJ{0,1}[0-9]{1} ){0,1}(NS{1} ){0,1}[CcPp]{1}$";

		/// <summary>Regular expression pattern for an order id</summary>
		private static readonly string _orderId =
			@"^[a-z|A-Z][a-z|A-Z|0-9]{6}(19|20)[0-9]{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$";

		/// <summary>Regular expression pattern for a valid password</summary>
		private static readonly string _password =
			@"\A(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[\.\$\^\{\}\[\]\(\)\|\*\+\?\\;:,=@#%&!""' ])" 
			+ @"[a-zA-Z0-9\.\$\^\{\}\[\]\(\)\|\*\+\?\\;:,=@#%&!""' ]{8,}\z";

		/// <summary>Regular expression pattern for a default Bashwork Account password</summary>
		private static readonly string _passwordBashworkAccountDefault =
			@"^[A-Z]{1}[a-z]{0,3}[0-1]{1}[0-9]{1}[0-3]{1}[0-9]{1}[0-9]{4}$";			

		/// <summary>Regular expression pattern for a phone number</summary>
		private static readonly string _phone =
            @"^(?:(?:1(?:\.| |-|))?(?:\([2-9]\d{2}\)|[2-9]\d{2})(?:\.| |-|))?[2-9]\d{2}(?:\.| |-|)\d{4}(?: ?[xX]\d{1,5})?$";

        /// <summary>Regular expression for a phone number with an area code</summary>
        private static readonly string _phoneWithAreaCode =
            @"^(?:1(?:\.| |-|))?(?:\([2-9]\d{2}\)|[2-9]\d{2})(?:\.| |-|)[2-9]\d{2}(?:\.| |-|)\d{4}(?: ?[xX]\d{1,5})?$";

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

		/// <summary>Compiled regular expression for an account number</summary>
		private static readonly Regex _accountNumberPattern =
			new Regex(_accountNumber, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for a back-office account number</summary>
		private static readonly Regex _accountNumberBackOfficePattern = 
			new Regex(_accountNumberBackOffice, RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

		/// <summary>Compiled regular expression for US equity symbols</summary>
		private static readonly Regex _equitySymbolPattern = 
			new Regex(_equitySymbol, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for US and CA equity symbols</summary>
		private static readonly Regex _equitySymbolUsAndCanadianPattern =
			new Regex(_equitySymbolUsAndCanadian, RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

		/// <summary>Compiled regular expression for option symbols in the back office</summary>
		private static readonly Regex _optionSymbolBackOfficePattern = 
			new Regex(_optionSymbolBackOffice, 
					  RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for option symbols in bashwork format</summary>
		private static readonly Regex _optionSymbolBashworkPattern = 
			new Regex(_optionSymbolBashwork, 
					  RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for option symbols in displayable format</summary>
		private static readonly Regex _optionSymbolDisplayablePattern = 
			new Regex(_optionSymbolDisplayable, 
					  RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for order ids</summary>
		private static readonly Regex _orderIdPattern = 
			new Regex(_orderId, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for valid passwords</summary>
		private static readonly Regex _passwordPattern = 
			new Regex(_password, RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>Compiled regular expression for valid Bashwork Account Default 
		/// password</summary>
		private static readonly Regex _passwordBashworkAccountDefaultPattern =
			new Regex(_passwordBashworkAccountDefault, RegexOptions.Compiled 
														| RegexOptions.CultureInvariant);			

		/// <summary>Compiled regular expression for phone number entry</summary>
		private static readonly Regex _phonePattern = 
			new Regex(_phone, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>Compiled regular expression for phone number with area code</summary>
        private static readonly Regex _phoneWithAreaCodePattern =
            new Regex(_phoneWithAreaCode, RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

		#region Public Property Regular Expression Patterns

		/// <summary>
		/// Gets the account number regular expression pattern.
		/// Validates an eight-digit number matching Bashwork's 
		/// standard account eight-digit account
		/// Ex. Matches: 11111148 | 11111178 | 11113456
		/// </summary>
		/// <value>The account number regular expression pattern.</value>
		public static string AccountNumberRegularExpressionPattern
		{
			get { return _accountNumber; }
		}

		/// <summary>
		/// Gets the account number back office regular expression pattern.
		/// Validates a ten-digit number matching a back-office 
		/// standard account (eight-digit account + two-digit check number) 
		/// Ex. Matches: 1111114800 | 1111117801 | 1111134521
		/// </summary>
		/// <value>The account number back office regular expression pattern.</value>
		public static string AccountNumberBackOfficeRegularExpressionPattern
		{
			get { return _accountNumberBackOffice; }
		}

		/// <summary>
		/// <para>
		/// Gets the address regular expression pattern.
		/// Validates a rough match for a standard US Address.
		/// Ex. Matches: 
		/// 1234 Some Street CityName, State, Zip
		/// </para>
		/// <para>
		/// 1234 Flushing Meadows Dr. 
		/// St. Louis, MO 63131
		/// </para>
		/// </summary>
		/// <value>The address regular expression pattern.</value>
		public static string AddressRegularExpressionPattern
		{
			get { return _address; }
		}

		/// <summary>
		/// Gets the alpha and digits regular expression pattern.
		/// Validates any alpha and digits string of any length and allows spaces. 
		/// Ex. matches: ABCD | ABC 123 | 123 45abcd
		/// </summary>
		/// <value>The alpha and digits regular expression pattern.</value>
		public static string AlphaAndDigitsRegularExpressionPattern
		{
			get { return _alphaAndDigits; }
		}

		/// <summary>
		/// Gets the alpha and digits and spaces regular expression pattern.
		/// Validates an alpha and digits and spaces string:
		/// Ex. Matches: abc defgh | ABCD | ABC123
		/// </summary>
		/// <value>The alpha and digits and spaces regular expression pattern.</value>
		public static string AlphaAndDigitsAndSpacesRegularExpressionPattern
		{
			get { return _alphaAndDigitsAndSpaces; }
		}

		/// <summary>
		/// Gets the alpha and digits and spaces and special chars regular expression pattern.
		/// Validates an alpha and digits string and allows some special characters:
		/// Ex. Matches: abc defgh | ABCD | ABC123 | *~!@#$%^&amp;*()|\\?/&lt;&gt;=+_
		/// </summary>
		/// <value>The alpha and digits and spaces and special chars regular expression pattern.</value>
		public static string AlphaAndDigitsAndSpacesAndSpecialCharactersRegularExpressionPattern
		{
			get { return _alphaAndDigitsAndSpacesAndSpecialCharacters; }
		}

		/// <summary>
		/// Gets the alpha and digits with underscores allow space regular expression pattern.
		/// Validates any alpha and digits string of any length 
		/// and allows underscore characters and spaces. Ex. matches: ABCD | ABC_123 | 12345 abcd_
		/// </summary>
		/// <value>
		/// The alpha and digits with underscores allow space regular expression pattern.
		/// </value>
		public static string AlphaAndDigitsAndUnderscoresAndSpacesRegularExpressionPattern
		{
			get { return _alphaAndDigitsAndUnderscoresAndSpaces; }
		}

		/// <summary>
		/// Gets the alpha and digits with underscores regular expression pattern.
		/// Validates any alpha and digits string of any length 
		/// and allows underscore characters. Ex. matches: ABCD | ABC_123 | 12345 abcd_
		/// </summary>
		/// <value>The alpha and digits with underscores regular expression pattern.</value>
		public static string AlphaAndDigitsAndUnderscoresRegularExpressionPattern
		{
			get { return _alphaAndDigitsAndUnderscores; }
		}

		/// <summary>
		/// Gets the alpha regular expression pattern.
		/// Validates any alpha string of characters. Ex. Matches: ABCD | abcd | ABcd
		/// </summary>
		/// <value>The alpha regular expression pattern.</value>
		public static string AlphaRegularExpressionPattern
		{
			get { return _alpha; }
		}

		/// <summary>
		/// Gets the alpha with spaces regular expression pattern.
		/// Validates any alpha string of characters and allows spaces.
		/// Ex. Matches: AB CD | abcd | A Bcd
		/// </summary>
		/// <value>The alpha with spaces regular expression pattern.</value>
		public static string AlphaAndSpacesRegularExpressionPattern
		{
			get { return _alphaAndSpaces; }
		}

		/// <summary>
		/// Gets the city regular expression pattern.
		/// Validates a rough match for a city name. Does not perform length validation.
		/// Ex. Matches: St. Louis | Detroit | New Orleans
		/// </summary>
		/// <value>The city regular expression pattern.</value>
		public static string CityRegularExpressionPattern
		{
			get { return _city; }
		}

		/// <summary>
		/// Gets the currency regular expression pattern.
		/// Validates valid currency values. Negative symbol (-) and dollar sign ($) always optional.
		/// .00 is optional but if included must be exactly two digits.
		/// 0,###(,### repeats 0-* times).00
		/// </summary>
		/// <value>The currency regular expression pattern.</value>
		public static string CurrencyRegularExpressionPattern
		{
			get { return _currency; }
		}

		/// <summary>
		/// Gets the date regular expression pattern.
		/// Validates a date string in various formats:
		/// Ex. Matches 01.1.02 | 11-30-2001 | 2/29/2000 
		/// Non-Matches 02/29/01 | 13/01/2002 | 11/00/02 
		/// </summary>
		/// <value>The date regular expression pattern.</value>
		public static string DateRegularExpressionPattern
		{
			get { return _date; }
		}

		/// <summary>
		/// Gets the email regular expression pattern.
		/// Validates that email string adheres directly to the specification for email address naming. 
		/// It allows for everything from ipaddress and country-code domains, 
		/// to very rare characters in the username.
		/// Ex. Matches asmith@mactec.com | foo12@foo.edu | bob.smith@foo.tv 
		/// Non-Matches joe | @foo.com | a@a 
		/// </summary>
		/// <value>The email regular expression pattern.</value>
		public static string EmailRegularExpressionPattern
		{
			get { return _email; }
		}

		/// <summary>
		/// Matches a standard equity symbol in the US stock market.
		/// Examples: F	GOOG	CMG
		/// </summary>
		public static string EquitySymbolRegularExpressionPattern
		{
			get { return _equitySymbol; }
		}

		/// <summary>
		/// Matches a standard equity symbol in the US or CA stock market.
		/// Examples: F   GOOG   CMG   .SAE   SAE.CA   SAE/AA   SAE/B
		/// </summary>
		public static string EquitySymbolUsAndCanadianRegularExpressionPattern
		{
			get { return _equitySymbolUsAndCanadian; }
		}

		/// <summary>
		/// Designed to allow text that a user would type into a standard free-form text box.
		/// Just about every character on the standard U.S. keyboard is allowed. Does not perform 
		/// length validation.
		/// </summary>
		public static string FreeFormTextRegularExpressionPattern
		{
			get { return _freeFormText; }
		}

		/// <summary>
		/// Designed to allow text that a user would type into a standard free-form text box.
		/// Characters from Latin and Chinese languages are allowed. Does not perform length 
		/// validation.
		/// ASCII Range Allowed: 20-7E
		/// Unicode Ranges Allowed: u2000-u206f, u3000-u303F, u3200-u32FF, u3300-u33FF, u4e00-u9fa5, 
		/// uFF00-uFFEF
		/// </summary>
		public static string FreeFormLatinAndChineseTextRegularExpressionPattern
		{
			get { return _freeFormLatinAndChineseText; }
		}		

		/// <summary>
		/// Gets the integer regular expression pattern. Does not perform length validation.
		/// Validates any Integer Ex. Matches: 1 | 100 | -500
		/// </summary>
		/// <value>The integer regular expression pattern.</value>
		public static string IntegerRegularExpressionPattern
		{
			get { return _integer; }
		}

		/// <summary>
		/// Gets the unsigned integer regular expression pattern. Does not perform length 
		/// validation.
		/// Validates any positive Integer Ex. Matches: 1 | 100 | 500
		/// </summary>
		public static string IntegerUnsignedRegularExpressionPattern
		{
			get { return _integerUnsigned; }
		}

		/// <summary>
		/// Gets the IP Address (IPv4) regular expression pattern.
		/// Validates a standard 32-bit IP Address (IPv4)
		/// Examples: 127.0.0.1, 201.45.68.12
		/// </summary>
		/// <value>The IP Address (IPv4) regular expression pattern</value>
		public static string IpAddressIpv4RegularExpressionPattern
		{
			get { return _ipAddressIpv4; }
		}

		/// <summary>
		/// <para>
		/// Gets the IP Address (IPv6) regular expression pattern.
		/// Validates a standard 128-bit IP Address (IPv6)
		/// Note that these are all the same address:
		/// fe80:0000:0000:0000:0204:61ff:fe9d:f156 // full form of IPv6
		/// fe80:0:0:0:204:61ff:fe9d:f156 // drop leading zeroes
		/// fe80::204:61ff:fe9d:f156 // collapse multiple zeroes to :: in the IPv6 address
		/// fe80:0000:0000:0000:204:61ff:254.157.241.086 // IPv4 dotted quad at the end
		/// fe80:0:0:0:0204:61ff:254.157.241.86 // drop leading zeroes, IPv4 dotted quad at the end
		/// fe80::204:61ff:254.157.241.86 // dotted quad at the end, multiple zeroes collapsed
		/// </para>
		/// <para>
		/// In addition, the regular expression matches these IPv6 forms:
		/// ::1 // localhost
		/// fe80:: // link-local prefix
		/// 2001:: // global unicast prefix 
		/// </para>
		/// </summary>
		/// <value>The IP Address (IPv6) regular expression pattern</value>
		public static string IpAddressIpv6RegularExpressionPattern
		{
			get { return _ipAddressIpv6; }
		}

		/// <summary>
		/// Gets the name regular expression pattern.
		/// Validates a rough match for a name allowing up to 100 characters.
		/// Ex. Matches: Bob Smith | Sean O'Grady | Bob.Smith
		/// </summary>
		/// <value>The name regular expression pattern.</value>
		public static string NameRegularExpressionPattern
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the number regular expression pattern.
		/// Validates a number or decimalized number.  Does not perform length validation.
		/// Ex. Matches: 5.5 | 20 | -1.0 nonMatches: 1,000
		/// </summary>
		/// <value>The number regular expression pattern.</value>
		public static string NumberRegularExpressionPattern
		{
			get { return _number; }
		}

		/// <summary>
		/// Matches the new OSI/OCC standard option format (BackOffice flavor). 
		/// Example: APV   100220P00130000
		/// </summary>
		public static string OptionSymbolBackOffice
		{
			get { return _optionSymbolBackOffice; }
		}

		/// <summary>
		/// Matches the new OSI/OCC standard option format (Displayable flavor). 
		/// Examples: F 2.50 JAN 10 C | GE 25.00 DEC 08 WK1 AJ9 NS C | GE 25.00 DEC 08 NS C
		/// </summary>
		public static string OptionSymbolDisplayable
		{
			get { return _optionSymbolDisplayable; }
		}

		/// <summary>
		/// Matches the new OSI/OCC standard option format (Bashwork flavor). 
		/// Example: .APV   100220P00130000
		/// </summary>
		public static string OptionSymbolBashwork
		{
			get { return _optionSymbolBashwork; }
		}

		/// <summary>
		/// Gets the order id regular expression pattern.
		/// Ex. Matches: XAC206120091102 | XAC211E20091116 | XAC208E20091104 | XAC20F420091110
		/// </summary>
		/// <value>The order id regular expression pattern.</value>
		public static string OrderIdRegularExpressionPattern
		{
			get { return _orderId; }
		}

		/// <summary>
		/// Gets the password regular expression pattern.
		/// This regular expression pattern exceeds the Bashwork Security standards and enforces
		/// the following password properties:
		/// • At least eight characters long
		/// • Contains each of the following four character types:
		/// • Upper Case letters (i.e. A-Z)
		/// • Lower Case letters (i.e. a-z)
		/// • Numbers
		/// • Special Characters such as !@#$%^&amp;*()_+|~-=\`{}[]:";'&lt;&gt;?,./ (includes spaces)
		/// </summary>
		/// <value>The password regular expression pattern.</value>
		public static string PasswordRegularExpressionPattern
		{
			get { return _password; }
		}

		/// <summary>
		/// Gets the Bashwork Account Default Password pattern.
		/// Login Security Admin resets a customer's account to this password pattern format
		/// when needed to be reset for the customer.
		/// Minimum one, up to first four characters of last name plus DOB in mmddyyyy format. 
		/// The first character must be in upper case.
		/// </summary>
		public static string PasswordBashworkAccountDefaultRegularExpressionPattern
		{
			get { return _passwordBashworkAccountDefault; }
		}		

		/// <summary>
		/// Gets the phone regular expression pattern.
		/// Validates a US phone number with or without area code/extension. 
        /// Ex. Matches: ###-####, 1-###-###-#### , (###)-###-####, ###-###-#### x###
		/// </summary>
		/// <value>The phone regular expression pattern.</value>
		public static string PhoneRegularExpressionPattern
		{
			get { return _phone; }
		}

        /// <summary>
        /// Gets the phone with area code regular expression pattern.
        /// Validates a US phone number with an area code and optional extension. 
        /// Ex. Matches: ###-###-#### , 1-(###)-###-####, ###-###-#### x###
        /// </summary>
        /// <value>The phone with area code regular expression pattern.</value>
        public static string PhoneWithAreaCodeRegularExpressionPattern
        {
            get { return _phoneWithAreaCode; }
        }

		/// <summary>
		/// Gets the special chars regular expression pattern.
		/// Validates of you have any of the following characters in your string:
		/// *~!@#$%^&amp;*()|\\?/&lt;&gt;=+_
		/// </summary>
		/// <value>The special chars regular expression pattern.</value>
		public static string SpecialCharactersRegularExpressionPattern
		{
			get { return _specialCharacters; }
		}

		/// <summary>
		/// Gets the social security number (SSN) regular expression pattern.
		/// Validates a US SSN Ex. Matches: ###-##-####, #########
		/// </summary>
		/// <value>The SSN basic regular expression pattern.</value>
		public static string SsnRegularExpressionPattern
		{
			get { return _ssn; }
		}

		/// <summary>
		/// Gets the tax identification number (TIN) regular expression pattern.
		/// Validates TIN Ex. Matches: ##-####### | #########
		/// </summary>
		/// <value>The tax identification number regular expression pattern.</value>
		public static string TinRegularExpressionPattern
		{
			get { return _tin; }
		}

		/// <summary>
		/// Gets the URL regular expression pattern.
		/// Validates a URL in the standard format http://XXX.XXX 
		/// Allows URL encoded spaces and variables etc
		/// Ex. Matches: http://www.xyz.com | http://xxx.xxx/x.htm?a=123456%20Blah
		/// </summary>
		/// <value>The URL regular expression pattern.</value>
		public static string UrlRegularExpressionPattern
		{
			get { return _url; }
		}

		/// <summary>
		/// Gets the zip code regular expression pattern.
		/// Validates US Zipcode Ex. Matches: #####-#### | #####
		/// </summary>
		/// <value>The zip code regular expression pattern.</value>
		public static string ZipCodeRegularExpressionPattern
		{
			get { return _zipCode; }
		}

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
		/// Validates the input string with the specified pattern. Null input is considered invalid,
		/// and most patterns consider empty string to also be invalid.
		/// </summary>
		/// <param name="input">The input to validate.</param>
		/// <param name="whichPattern">The pattern to use.</param>
		/// <returns>
		/// 	<c>true</c> if input matches pattern; <c>false</c> otherwise.
		/// </returns>
		public static bool ValidateInput(string input, RegularExpressionPatternType whichPattern)
		{
			// By default, null input is considered invalid, and most patterns consider empty string to be invalid as well.
			return ValidateInput(input, whichPattern, false);
		}

		/// <summary>
		/// Validates the input string with the specified pattern. Caller-specified null/empty validity.
		/// </summary>
		/// <param name="input">The input to validate.</param>
		/// <param name="whichPattern">The pattern to use.</param>
		/// <param name="isNullOrEmptyValid">If set to <c>true</c>, null or empty input is considered valid.</param>
		/// <returns>
		/// 	<c>true</c> if input matches pattern; <c>false</c> otherwise.
		/// </returns>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.ReadabilityAnalyzer",
			"ST2002:MethodCannotExceedMaxLines",
			Justification = "Can't break down switch to smaller size, could refactor to Translator "
			                + "to improve readability but would have to see how speed affected.")]
		public static bool ValidateInput(string input, RegularExpressionPatternType whichPattern, bool isNullOrEmptyValid)
		{
			// Assign initial validity - based on if the user considers null/empty values to be OK
			bool isValid = isNullOrEmptyValid && string.IsNullOrEmpty(input);

			// Check for null first since Regex.Match throws an execption if the input is null.
			if (!isValid && input != null)
			{
				switch (whichPattern)
				{
					case RegularExpressionPatternType.AccountNumber:
						isValid = _accountNumberPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AccountNumberBackOffice:
						isValid = _accountNumberBackOfficePattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Address:
						isValid = _addressPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Alpha:
						isValid = _alphaPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AlphaAndSpaces:
						isValid = _alphaAndSpacesPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AlphaAndDigits:
						isValid = _alphaAndDigitsPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AlphaAndDigitsAndSpaces:
						isValid = _alphaAndDigitsAndSpacesPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters:
						isValid = _alphaAndDigitsAndSpacesAndSpecialCharactersPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces:
						isValid = _alphaAndDigitsAndUnderscoresAndSpacesPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.AlphaAndDigitsAndUnderscores:
						isValid = _alphaAndDigitsAndUnderscoresPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.City:
						isValid = _cityPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Currency:
						isValid = _currencyPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Date:
						isValid = _datePattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Email:
						isValid = _emailPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.EquitySymbol:
						isValid = _equitySymbolPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.EquitySymbolUsAndCanadian:
						isValid = _equitySymbolUsAndCanadianPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.FreeFormText:
						isValid = _freeFormTextPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.FreeFormLatinAndChineseText:
						isValid = _freeFormLatinAndChineseTextPattern.Match(input).Success;
						break;						
					case RegularExpressionPatternType.Integer:
						isValid = _integerPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.IntegerUnsigned:
						isValid = _integerUnsignedPattern.Match(input).Success;
						break;						
					case RegularExpressionPatternType.IpAddressIPv4:
						isValid = _ipAddressIpv4Pattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.IpAddressIPv6:
						isValid = _ipAddressIpv6Pattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Name:
						isValid = _namePattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Number:
						isValid = _numberPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.OrderId:
						isValid = _orderIdPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.OptionSymbolBackOffice:
						isValid = _optionSymbolBackOfficePattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.OptionSymbolDisplayable:
						isValid = _optionSymbolDisplayablePattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.OptionSymbolBashwork:
						isValid = _optionSymbolBashworkPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Password:
						isValid = _passwordPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.PasswordBashworkAccountDefault:
						isValid = _passwordBashworkAccountDefaultPattern.Match(input).Success;
						break;						
					case RegularExpressionPatternType.Phone:
						isValid = _phonePattern.Match(input).Success;
						break;
                    case RegularExpressionPatternType.PhoneWithAreaCode:
                        isValid = _phoneWithAreaCodePattern.Match(input).Success;
                        break;
					case RegularExpressionPatternType.SpecialCharacters:
						isValid = _specialCharactersPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Ssn:
						isValid = _ssnPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.TaxIdentificationNumber:
						isValid = _tinPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.Url:
						isValid = _urlPattern.Match(input).Success;
						break;
					case RegularExpressionPatternType.ZipCode:
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