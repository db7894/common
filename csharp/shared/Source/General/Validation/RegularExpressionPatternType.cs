﻿
namespace SharedAssemblies.General.Validation
{
	/// <summary>
	/// Enumeration to specify the regular expression pattern to use when matching.
	/// </summary>
	public enum RegularExpressionPatternType
	{
		/// <summary>
		/// Validates an eight-digit number matching Bashwork's 
		/// standard account eight-digit account
		/// Ex. Matches: 11111148 | 11111178 | 11113456
		/// </summary>
		AccountNumber,

		/// <summary>
		/// Validates a ten-digit number matching a back-office 
		/// standard account (eight-digit account + two-digit check number) 
		/// Ex. Matches: 1111114800 | 1111117801 | 1111134521
		/// </summary>
		AccountNumberBackOffice,

		/// <summary>
		/// <para>
		/// Validates a rough match for a standard US Address.
		/// Ex. Matches: 
		/// 1234 Some Street CityName, State, Zip
        /// </para>
        /// <para>
		/// 1234 Flushing Meadows Dr. 
		/// St. Louis, MO 63131
        /// </para>
		/// </summary>
		Address,	

		/// <summary>
		/// Validates any alpha string of characters. 
		/// Ex. Matches: ABCD | abcd | ABcd
		/// </summary>
		Alpha,

		/// <summary>
		/// Validates any alpha string of characters and allows spaces.
		/// Ex. Matches: AB CD | abcd | A Bcd
		/// </summary>
		AlphaAndSpaces,

		/// <summary>
		/// Validates any alpha and digits string of any length. 
		/// Ex. matches: ABCD | ABC123 | 12345abcd
		/// </summary>
		AlphaAndDigits,

		/// <summary>
		/// Validates an alpha and digits and spaces string:
		/// Ex. Matches: abc defgh | ABCD | ABC123
		/// </summary>
		AlphaAndDigitsAndSpaces,

		/// <summary>
		/// Validates any string with alpha, digits, and underscore characters of any length 
		/// Ex. matches: ABCD | ABC_123 | 12345abcd_
		/// </summary>
		AlphaAndDigitsAndUnderscores,

		/// <summary>
		/// Validates any string with alpha, digits, space and underscore characters of any length 
		/// Ex. matches: ABCD | ABC_123 | 12345 abcd_
		/// </summary>
		AlphaAndDigitsAndUnderscoresAndSpaces,

		/// <summary>
		/// Validates any string with alpha, digits, space and special characters of any length 
		/// Ex. Matches: abc defgh | ABCD | ABC123 | *~@#$%^
		/// </summary>
		AlphaAndDigitsAndSpacesAndSpecialCharacters,

		/// <summary>
		/// Validates a rough match for a city name. Does not perform length validation.
		/// Ex. Matches: St. Louis | Detroit | New Orleans
		/// </summary>
		City,

		/// <summary>
		/// Validates valid currency values. Negative symbol (-) and dollar sign ($) always optional.
		/// .00 is optional but if included must be exactly two digits.
		/// 0,###(,### repeats 0-* times).00
		/// </summary>
		Currency,		

		/// <summary>
		/// Validates a date string in various formats:
		/// Ex. Matches 01.1.02 | 11-30-2001 | 2/29/2000 
		/// Non-Matches 02/29/01 | 13/01/2002 | 11/00/02 
		/// </summary>
		Date,

		/// <summary>
		/// Validates that email string adheres directly to the specification for email address naming. 
		/// It allows for everything from ipaddress and country-code domains, 
		/// to very rare characters in the username.
		/// Ex. Matches asmith@mactec.com | foo12@foo.edu | bob.smith@foo.tv 
		/// Non-Matches joe | @foo.com | a@a 
		/// </summary>
		Email,

		/// <summary>
		/// Matches a standard equity symbol in the US stock market.
		/// Examples: F	GOOG	CMG
		/// </summary>
		EquitySymbol,

		/// <summary>
		/// Matches a standard equity symbol in the US or CA stock market.
		/// Examples: F   GOOG   CMG   .SAE   SAE.CA   SAE/AA   SAE/B
		/// </summary>		
		EquitySymbolUsAndCanadian,

		/// <summary>
		/// Designed to allow text that a user would type into a standard free-form text box.
		/// Just about every character on the standard U.S. keyboard is allowed. Does not perform 
		/// length validation.
		/// </summary>
		FreeFormText,

		/// <summary>
		/// Designed to allow text that a user would type into a standard free-form text box.
		/// Characters from Latin and Chinese languages are allowed. Does not perform length 
		/// validation.
		/// ASCII Range Allowed: 20-7E
		/// Unicode Ranges Allowed: u2000-u206f, u3000-u303F, u3200-u32FF, u3300-u33FF, u4e00-u9fa5, 
		/// uFF00-uFFEF
		/// </summary>
		FreeFormLatinAndChineseText,

		/// <summary>
		/// Validates any Integer. Does not perform length validation.
		/// Ex. Matches: 1 | 100 | -500
		/// </summary>
		Integer,

		/// <summary>
		/// Validates any positive Integer. Does not perform length validation.
		/// Ex. Matches: 1 | 100 | 500
		/// </summary>
		IntegerUnsigned,		
	
		/// <summary>
		/// Validates a standard 32-bit IP Address (IPv4)
		/// Examples: 127.0.0.1, 201.45.68.12
		/// </summary>
		IpAddressIPv4,

		/// <summary>
		/// <para>
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
		IpAddressIPv6,		

		/// <summary>
		/// Validates a rough match for a name allowing up to 100 characters.
		/// Ex. Matches: Bob Smith | Sean O'Grady | Bob.Smith
		/// </summary>
		Name,

		/// <summary>
		/// Validates a number or decimalized number. Does not perform length validation.
		/// Ex. Matches: 5.5 | 20 | -1.0 nonMatches: 1,000
		/// </summary>
		Number,

		/// <summary>
		/// Matches the new OSI/OCC standard option format (BackOffice flavor). 
		/// Example: APV   100220P00130000
		/// </summary>
		OptionSymbolBackOffice,

		/// <summary>
		/// Matches the new OSI/OCC standard option format (Displayable flavor). 
		/// Examples: F 2.50 JAN 10 C | GE 25.00 DEC 08 WK1 AJ9 NS C | GE 25.00 DEC 08 NS C
		/// </summary>
		OptionSymbolDisplayable,

		/// <summary>
		/// Matches the new OSI/OCC standard option format (Bashwork flavor). 
		/// Example: .APV   100220P00130000
		/// </summary>
		OptionSymbolBashwork,		

		/// <summary>
		/// Validates an order id that is on orders.
		/// Ex. Matches: XAC206120091102 | XAC211E20091116 | XAC208E20091104 | XAC20F420091110
		/// </summary>
		OrderId,

		/// <summary>
		/// This regular expression pattern exceeds the Bashwork Security standards and enforces
		/// the following password properties:
		/// • At least eight characters long
		/// • Contains each of the following four character types:
		/// • Upper Case letters (i.e. A-Z)
		/// • Lower Case letters (i.e. a-z)
		/// • Numbers
		/// • Special Characters such as !@#$%^&amp;*()_+|~-=\`{}[]:";'&lt;&gt;?,./ (includes spaces)
		/// </summary>
		Password,

		/// <summary>
		/// Gets the Bashwork Account Default Password pattern.
		/// Login Security Admin resets a customer's account to this password pattern format
		/// when needed to be reset for the customer.
		/// Minimum one, up to first four characters of last name plus DOB in mmddyyyy format. 
		/// The first character must be in upper case.
		/// </summary>
		PasswordBashworkAccountDefault,

		/// <summary>
		/// Validates a US phone number with or without area code and extension. 
        /// Ex. Matches: ###-####, 1-###-###-#### , (###)-###-####, ###-###-#### x###
		/// </summary>
		Phone,

        /// <summary>
        /// Validates a US phone number with an area code and optional extension. 
        /// Ex. Matches: ###-###-#### , 1-(###)-###-####, ###-###-#### x###
        /// </summary>
        PhoneWithAreaCode,

		/// <summary>
		/// Validates of you have any of the following characters in your string:
		/// *~!@#$%^&amp;*()|\\?/&lt;&gt;=+_
		/// </summary>
		SpecialCharacters,

		/// <summary>
		/// Validates a US SSN Ex. Matches: ###-##-####, #########
		/// </summary>
		Ssn,

		/// <summary>
		/// Validates TIN Ex. Matches: ##-####### | #########
		/// </summary>
		TaxIdentificationNumber,

		/// <summary>
		/// Validates a URL in the standard format http://XXX.XXX Allows URL encoded spaces and variables etc
		/// Ex. Matches: http://www.xyz.com | http://xxx.xxx/x.htm?a=123456%20Blah
		/// https:// also allowed
		/// </summary>
		Url,

		/// <summary>
		/// Validates US Zipcode Ex. Matches: #####-#### | #####
		/// </summary>
		ZipCode
	}
}
