using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Validation.UnitTests
{
	/// <summary>
	/// Class containing all unit tests for the regular expression patterns used 
	/// in the Validator class. Both success and fail tests for every pattern are included.
	/// </summary>
	[TestClass]
	public class ValidateInputTests
	{
		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAccountNumberSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("11111327",
				RegularExpressionPatternType.AccountNumber));
			Assert.IsTrue(Validator.ValidateInput("11111888",
				RegularExpressionPatternType.AccountNumber));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAccountNumberFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.AccountNumber));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.AccountNumber));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.AccountNumber));
			Assert.IsFalse(Validator.ValidateInput("Hello!12",
				RegularExpressionPatternType.AccountNumber));
			Assert.IsFalse(Validator.ValidateInput("123456789101", 
				RegularExpressionPatternType.AccountNumber));
			Assert.IsFalse(Validator.ValidateInput("1111132701",
				RegularExpressionPatternType.AccountNumber));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAccountNumberBackOfficeSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("1111132710",
			    RegularExpressionPatternType.AccountNumberBackOffice));
			Assert.IsTrue(Validator.ValidateInput("1111188801",
			    RegularExpressionPatternType.AccountNumberBackOffice));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAccountNumberBackOfficeFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null, 
				RegularExpressionPatternType.AccountNumberBackOffice));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.AccountNumberBackOffice));
			Assert.IsFalse(Validator.ValidateInput("1234",
			     RegularExpressionPatternType.AccountNumberBackOffice));
			Assert.IsFalse(Validator.ValidateInput("Hello!12",
			     RegularExpressionPatternType.AccountNumberBackOffice));
			Assert.IsFalse(Validator.ValidateInput("12345678",
			     RegularExpressionPatternType.AccountNumberBackOffice));
			Assert.IsFalse(Validator.ValidateInput("11111327",
			     RegularExpressionPatternType.AccountNumberBackOffice));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAddressSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("1234 My Street",
				RegularExpressionPatternType.Address));
			Assert.IsTrue(Validator.ValidateInput("1234 My Street, St. Louis, MO, 12345",
			    RegularExpressionPatternType.Address));
			Assert.IsTrue(Validator.ValidateInput("1234 My Street, St. Louis, MO",
			    RegularExpressionPatternType.Address));
			Assert.IsTrue(Validator.ValidateInput("1234 O'Mang Street, St. Louis, MO",
			    RegularExpressionPatternType.Address));
			Assert.IsTrue(
				Validator.ValidateInput(
					@"1234 O'Mang Street,
                                                    St. Louis, MO",
					RegularExpressionPatternType.Address));
            Assert.IsTrue(Validator.ValidateInput("1234 MyStreet,Town & Country,MO",
                  RegularExpressionPatternType.Address));
            // Yes, this is an actual street in Houston!
            Assert.IsTrue(Validator.ValidateInput("511 15 1/2 Street, Houston, TX",
                  RegularExpressionPatternType.Address));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAddressFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Address));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Address));
			Assert.IsFalse(Validator.ValidateInput("1234 <My Street>",
				RegularExpressionPatternType.Address));
			Assert.IsFalse(Validator.ValidateInput("1234 My Street, St. Louis, MO, 12345?",
			     RegularExpressionPatternType.Address));
			Assert.IsFalse(Validator.ValidateInput("1234%MyStreet,St.Louis,MO",
			     RegularExpressionPatternType.Address));
			Assert.IsFalse(Validator.ValidateInput("1234	MyStreet,St.Louis,MO",
			     RegularExpressionPatternType.Address));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("ABCZ",
				RegularExpressionPatternType.Alpha));
			Assert.IsTrue(Validator.ValidateInput("abcz",
				RegularExpressionPatternType.Alpha));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Alpha));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Alpha));
			Assert.IsFalse(Validator.ValidateInput("A BCZ",
				RegularExpressionPatternType.Alpha));
			Assert.IsFalse(Validator.ValidateInput("abcz1",
				RegularExpressionPatternType.Alpha));
			Assert.IsFalse(Validator.ValidateInput("abc&zd",
				RegularExpressionPatternType.Alpha));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndSpacesSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("ABCZ",
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("AB CZ",
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("abcz",
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ab cz",
				RegularExpressionPatternType.AlphaAndSpaces));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndSpacesFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsFalse(Validator.ValidateInput(string.Empty, 
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ",
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("ab cz1",
				RegularExpressionPatternType.AlphaAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("a bc&zd",
				RegularExpressionPatternType.AlphaAndSpaces));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("A",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsTrue(Validator.ValidateInput("ABCZ4",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsTrue(Validator.ValidateInput("ABCZ1234",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsTrue(Validator.ValidateInput("abcz",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsTrue(Validator.ValidateInput("1abcz123",
				RegularExpressionPatternType.AlphaAndDigits));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsFalse(Validator.ValidateInput(string.Empty, 
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ#",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsFalse(Validator.ValidateInput("ab cz1+",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsFalse(Validator.ValidateInput("abc&zd1",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsFalse(Validator.ValidateInput("abc zd1",
				RegularExpressionPatternType.AlphaAndDigits));
			Assert.IsFalse(Validator.ValidateInput("HJL Z31 F",
				RegularExpressionPatternType.AlphaAndDigits));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndSpacesSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("A",
				RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ABCZ4",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ABCZ1234",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ab cz",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("1abcz12 3",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndSpacesFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null, 
				RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ#",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("ab cz1+",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("abc&zd1",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("abc_zd1",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("HJL Z31_F",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpaces));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndSpacesAndSpecialCharactersSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("A",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("ABCZ4",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("ABCZ 1234",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("abcz",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("1abcz 123",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("A B1CZ#",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("ab cz1+",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("abc&zd1",
			    RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndSpacesAndSpecialCharactersFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
			     RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ,",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("ab cz1\"\"",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("<script lang=\"javasrcript\">",
			     RegularExpressionPatternType.AlphaAndDigitsAndSpacesAndSpecialCharacters));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndUnderscoresSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("A",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("ABCZ4",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("ABCZ1234",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("abcz",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("1abcz123",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("AB1CZ_",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("abcz1_",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsTrue(Validator.ValidateInput("abc_zd1",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndUnderscoresFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ<>",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("A B_1CZ<>",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("A B_1CZ@#",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("ab cz1\"\"",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ_",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("ab cz1_",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
			Assert.IsFalse(Validator.ValidateInput("<script lang=\"javasrcript\">",
				RegularExpressionPatternType.AlphaAndDigitsAndUnderscores));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndUnderscoresAndSpacesSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("A",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ABCZ4",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ABCZ 1234",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("abcz",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("1abcz 123",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("A B1CZ_",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("ab cz1_",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsTrue(Validator.ValidateInput("abc_zd1",
			    RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAlphaAndDigitsAndUnderscoresAndSpacesFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("A B1CZ<>",
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("A B_1CZ<>",
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("A B_1CZ@#",
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("ab cz1\"\"",
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
			Assert.IsFalse(Validator.ValidateInput("<script lang=\"javasrcript\">",
			     RegularExpressionPatternType.AlphaAndDigitsAndUnderscoresAndSpaces));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateCitySuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("St. Louis",
				RegularExpressionPatternType.City));
			Assert.IsTrue(Validator.ValidateInput("Chicago",
				RegularExpressionPatternType.City));
			Assert.IsTrue(Validator.ValidateInput("Kansas City",
				RegularExpressionPatternType.City));
			Assert.IsTrue(Validator.ValidateInput("St. Paul",
				RegularExpressionPatternType.City));
			Assert.IsTrue(Validator.ValidateInput("O'Fallon",
				RegularExpressionPatternType.City));
			Assert.IsTrue(Validator.ValidateInput("Town & Country",
				RegularExpressionPatternType.City));							
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateCityFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.City));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.City));
			Assert.IsFalse(Validator.ValidateInput("<none>",
				RegularExpressionPatternType.City));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.City));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.City));
			Assert.IsFalse(Validator.ValidateInput("St. 45",
				RegularExpressionPatternType.City));
			Assert.IsFalse(Validator.ValidateInput("St.		45",
				RegularExpressionPatternType.City));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateCurrencySuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("$45",
				RegularExpressionPatternType.Currency));
			Assert.IsTrue(Validator.ValidateInput("$45.00",
				RegularExpressionPatternType.Currency));
			Assert.IsTrue(Validator.ValidateInput("$0.45",
				RegularExpressionPatternType.Currency));
			Assert.IsTrue(Validator.ValidateInput("$45,000.00",
				RegularExpressionPatternType.Currency));
			Assert.IsTrue(Validator.ValidateInput("$45,000,955.00",
				RegularExpressionPatternType.Currency));
			Assert.IsTrue(Validator.ValidateInput("$45,000,955",
				RegularExpressionPatternType.Currency));
			Assert.IsTrue(Validator.ValidateInput("45,000,955",
				RegularExpressionPatternType.Currency));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateCurrencyFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("45.",
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("$45.000",
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("$45.A5",
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("$.254",
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("$0.254",
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("$1,00,000",
				RegularExpressionPatternType.Currency));
			Assert.IsFalse(Validator.ValidateInput("$0,000,000",
				RegularExpressionPatternType.Currency));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateDateSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12/10/99",
				RegularExpressionPatternType.Date));
			Assert.IsTrue(Validator.ValidateInput("10.15.09",
				RegularExpressionPatternType.Date));
			Assert.IsTrue(Validator.ValidateInput("04-05-08",
				RegularExpressionPatternType.Date));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateDateFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("12345-1",
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("00.01.00",
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("13.15.99",
				RegularExpressionPatternType.Date));
			Assert.IsFalse(Validator.ValidateInput("12.48.99",
				RegularExpressionPatternType.Date));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateEmailSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("joesmith@hotmail.com", 
				RegularExpressionPatternType.Email));
			Assert.IsTrue(Validator.ValidateInput("joe.smith45@gmail.net", 
				RegularExpressionPatternType.Email));
			Assert.IsTrue(Validator.ValidateInput("mary.joe-willkins45@gmail.net",
			    RegularExpressionPatternType.Email));
			Assert.IsTrue(Validator.ValidateInput("mary.joe-willkins45@gmail.MUSEUM",
				RegularExpressionPatternType.Email));
			Assert.IsTrue(Validator.ValidateInput("mary.joe-willkins45@gmail.456",
				RegularExpressionPatternType.Email));
			Assert.IsTrue(Validator.ValidateInput("mary.joe-willkins45@gmail.net123",
				RegularExpressionPatternType.Email));
			Assert.IsTrue(Validator.ValidateInput("mary.joe-willkins45@g-mail.biz",
				RegularExpressionPatternType.Email));				
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateEmailFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("1234@",
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("@A2345",
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("12345-1",
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("joesmith@hotmail.c",
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("joe.smith45@gmail",
				RegularExpressionPatternType.Email));
			Assert.IsFalse(Validator.ValidateInput("<joe.smith45>@gmail.com",
			     RegularExpressionPatternType.Email));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateEquitySymbolSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("F",
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsTrue(Validator.ValidateInput("GOOG",
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsTrue(Validator.ValidateInput("CMG",
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsTrue(Validator.ValidateInput("AAPL",
				RegularExpressionPatternType.EquitySymbol));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateEquitySymbolFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				 RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput("123456",
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput("F1",
				 RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput("CSCOGG",
				 RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput("55CMG",
				 RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput(".SAE",
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput("SAE.CA",
				RegularExpressionPatternType.EquitySymbol));
			Assert.IsFalse(Validator.ValidateInput("SAE/B",
				RegularExpressionPatternType.EquitySymbol));				 
		}				
		
		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateEquitySymbolUsAndCanadianSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("F",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsTrue(Validator.ValidateInput("GOOG",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsTrue(Validator.ValidateInput("CMG",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsTrue(Validator.ValidateInput("AAPL",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsTrue(Validator.ValidateInput(".SAE",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsTrue(Validator.ValidateInput("SAE.CA",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsTrue(Validator.ValidateInput("SAE/B",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));				
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateEquitySymbolUsAndCanadianFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsFalse(Validator.ValidateInput("123456",
				RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsFalse(Validator.ValidateInput("F1",
				 RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsFalse(Validator.ValidateInput("CSCOGG",
				 RegularExpressionPatternType.EquitySymbolUsAndCanadian));
			Assert.IsFalse(Validator.ValidateInput("55CMG",
				 RegularExpressionPatternType.EquitySymbolUsAndCanadian));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateFreeFormLatinAndChineseTextSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12345",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("Hello Mom! I love you.",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("Why can't we all get along?",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("However, it's 'good' and \"bad\".",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("Yes; (thinks hard) $400.00 @ 4 each!",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("~300#, 20% & 56*2 - 5 + 38 = _45 / 2 + \\3.",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("Hello: 5 < 10; 6 > 2; 67 - 24.",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("所有信函和合約所載資訊的僅為方便理解而提供。 ",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("本公司的中文網站、表格、",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));				
			Assert.IsTrue(Validator.ValidateInput("表格、信函和合約所載資",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("可能存在不盡相同之處。本公司客戶須以英文版本為標準。",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("經紀產品和服務由市考特證券公司提供。– FINRA  以及 SIPC成員",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsTrue(Validator.ValidateInput("2009史考特證券版權所有。查看我們的  隱私條例 . ",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateFreeFormLatinAndChineseTextFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsFalse(Validator.ValidateInput("Kelime çifti bulmaca nasıl oynanır",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsFalse(Validator.ValidateInput("αδφγβμλποιτε",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsFalse(Validator.ValidateInput("§6 + §8",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));
			Assert.IsFalse(Validator.ValidateInput("Copyright © 2009 Bashwork",
				RegularExpressionPatternType.FreeFormLatinAndChineseText));				
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateFreeFormTextSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12345",
				RegularExpressionPatternType.FreeFormText));
			Assert.IsTrue(Validator.ValidateInput("Hello Mom! I love you.",
			    RegularExpressionPatternType.FreeFormText));
			Assert.IsTrue(Validator.ValidateInput("Why can't we all get along?",
			    RegularExpressionPatternType.FreeFormText));
			Assert.IsTrue(Validator.ValidateInput("However, it's 'good' and \"bad\".",
			    RegularExpressionPatternType.FreeFormText));
			Assert.IsTrue(Validator.ValidateInput("Yes; (thinks hard) $400.00 @ 4 each!",
			    RegularExpressionPatternType.FreeFormText));
			Assert.IsTrue(Validator.ValidateInput("~300#, 20% & 56*2 - 5 + 38 = _45 / 2 + \\3.",
			    RegularExpressionPatternType.FreeFormText));
			Assert.IsTrue(Validator.ValidateInput("Hello: 5 < 10; 6 > 2; 67 - 24.",
			    RegularExpressionPatternType.FreeFormText));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateFreeFormTextFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.FreeFormText));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.FreeFormText));
			Assert.IsFalse(Validator.ValidateInput("Hi there | No there",
			     RegularExpressionPatternType.FreeFormText));
			Assert.IsFalse(Validator.ValidateInput("Hello \t Goodbye",
			     RegularExpressionPatternType.FreeFormText));
			Assert.IsFalse(Validator.ValidateInput("{45} + [27]",
				RegularExpressionPatternType.FreeFormText));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateIntegerSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12345",
				RegularExpressionPatternType.Integer));
			Assert.IsTrue(Validator.ValidateInput("0",
				RegularExpressionPatternType.Integer));
			Assert.IsTrue(Validator.ValidateInput("-5",
				RegularExpressionPatternType.Integer));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateIntegerFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Integer));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Integer));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Integer));
			Assert.IsFalse(Validator.ValidateInput("4.8",
				RegularExpressionPatternType.Integer));
			Assert.IsFalse(Validator.ValidateInput(".5",
				RegularExpressionPatternType.Integer));
		}

		/// <summary>
		/// Tests the input validation method for unsigned integers
		/// </summary>
		[TestMethod]
		public void ValidateIntegerUnsignedSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12345",
				RegularExpressionPatternType.IntegerUnsigned));
			Assert.IsTrue(Validator.ValidateInput("0",
				RegularExpressionPatternType.IntegerUnsigned));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateIntegerUnsignedFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.IntegerUnsigned));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.IntegerUnsigned));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.IntegerUnsigned));
			Assert.IsFalse(Validator.ValidateInput("4.8",
				RegularExpressionPatternType.IntegerUnsigned));
			Assert.IsFalse(Validator.ValidateInput(".5",
				RegularExpressionPatternType.IntegerUnsigned));
			Assert.IsFalse(Validator.ValidateInput("-5",
				RegularExpressionPatternType.IntegerUnsigned));				
		}		

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateIpAddressIpv4SuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("127.0.0.1",
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsTrue(Validator.ValidateInput("168.15.10.68", 
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsTrue(Validator.ValidateInput("255.255.255.255",
			    RegularExpressionPatternType.IpAddressIPv4));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateIpAddressIpv4FailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("255.255.255.256",
			     RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("127.0.0.",
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("5.-1.0.1",
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("-5.1.0.1",
				RegularExpressionPatternType.IpAddressIPv4));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.ReadabilityAnalyzer",
			"ST2002:MethodCannotExceedMaxLines", Justification = "Unit Test.")]
		[TestMethod]
		public void ValidateIpAddressIpv6SuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("2001:0DB8:AC10:FE01:0000:0000:0000:0000",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80:0:0:0:204:61ff:fe9d:f156",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::204:61ff:fe9d:f156",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80:0000:0000:0000:0204:61ff:fe9d:f156",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80:0:0:0:204:61ff:254.157.241.86",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::204:61ff:254.157.241.86",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2001:0000:1234:0000:0000:C1C0:ABCD:0876",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("3ffe:0b00:0000:0000:0001:0000:0000:000a",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("FF02:0000:0000:0000:0000:0000:0000:0001",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("0000:0000:0000:0000:0000:0000:0000:0001",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("0000:0000:0000:0000:0000:0000:0000:0000",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::ffff:192.168.1.26",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2001:0000:1234:0000:0000:C1C0:ABCD:0876",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2::10",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("ff02::1",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2002::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2001:db8::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2001:0db8:1234::",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::ffff:0:0",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::1",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::ffff:192.168.1.1",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5:6:7:8",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5:6::8",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5::8", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4::8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3::8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2::8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::2:3:4:5:6:7",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::2:3:4:5:6", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::2:3:4:5",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::2:3:4",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::2:3",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::2:3:4:5:6:7:8",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::2:3:4:5:6:7", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::2:3:4:5:6",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::2:3:4:5",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::2:3:4",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::2:3",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5:6::", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5::7:8",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4::7:8", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3::7:8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2::7:8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::7:8",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5:6:1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4:5::1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4::1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3::1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2::1.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::1.2.3.4",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3:4::5:1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2:3::5:1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1:2::5:1.2.3.4",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::5:1.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("1::5:11.22.33.44",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::217:f2ff:254.7.237.98",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::217:f2ff:fe07:ed62",
			    RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("2001:DB8:0:0:8:800:200C:417A",
			    RegularExpressionPatternType.IpAddressIPv6));

			// unicast, full
			Assert.IsTrue(Validator.ValidateInput("FF01:0:0:0:0:0:0:101",
				RegularExpressionPatternType.IpAddressIPv6));

			// multicast, full
			Assert.IsTrue(Validator.ValidateInput("0:0:0:0:0:0:0:1",
				RegularExpressionPatternType.IpAddressIPv6));

			// loopback, full
			Assert.IsTrue(Validator.ValidateInput("0:0:0:0:0:0:0:0",
				RegularExpressionPatternType.IpAddressIPv6));

			// unspecified, full
			Assert.IsTrue(Validator.ValidateInput("2001:DB8::8:800:200C:417A",
				RegularExpressionPatternType.IpAddressIPv6));

			// unicast, compressed
			Assert.IsTrue(Validator.ValidateInput("FF01::101",
				RegularExpressionPatternType.IpAddressIPv6));

			// multicast, compressed
			Assert.IsTrue(Validator.ValidateInput("::1",
				RegularExpressionPatternType.IpAddressIPv6));

			// loopback, compressed, non-routable
			Assert.IsTrue(Validator.ValidateInput("::",
				RegularExpressionPatternType.IpAddressIPv6));

			// unspecified, compressed, non-routable
			Assert.IsTrue(Validator.ValidateInput("0:0:0:0:0:0:13.1.68.3",
				RegularExpressionPatternType.IpAddressIPv6));

			// IPv4-compatible IPv6 address, full, deprecated
			Assert.IsTrue(Validator.ValidateInput("0:0:0:0:0:FFFF:129.144.52.38",
				RegularExpressionPatternType.IpAddressIPv6));

			// IPv4-mapped IPv6 address, full
			Assert.IsTrue(Validator.ValidateInput("::13.1.68.3",
				RegularExpressionPatternType.IpAddressIPv6));

			// IPv4-compatible IPv6 address, compressed, deprecated
			Assert.IsTrue(Validator.ValidateInput("::FFFF:129.144.52.38",
				RegularExpressionPatternType.IpAddressIPv6));

			// IPv4-mapped IPv6 address, compressed
			Assert.IsTrue(Validator.ValidateInput("fe80:0000:0000:0000:0204:61ff:fe9d:f156",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80:0:0:0:204:61ff:fe9d:f156",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::204:61ff:fe9d:f156",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80:0:0:0:204:61ff:254.157.241.86",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::204:61ff:254.157.241.86",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("::1",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsTrue(Validator.ValidateInput("fe80::1",
				RegularExpressionPatternType.IpAddressIPv6));

			// default unicast route address
			Assert.IsTrue(Validator.ValidateInput("::/0",
				RegularExpressionPatternType.IpAddressIPv6));

			// unspecified address
			Assert.IsTrue(Validator.ValidateInput("::/128",
				RegularExpressionPatternType.IpAddressIPv6));

			// loopback
			Assert.IsTrue(Validator.ValidateInput("::1/128",
				RegularExpressionPatternType.IpAddressIPv6));

			// multicast address
			Assert.IsTrue(Validator.ValidateInput("ff02::1:ff00:0/104",
				RegularExpressionPatternType.IpAddressIPv6));

			// link-local with zone index (windows interface number)
			Assert.IsTrue(Validator.ValidateInput("fe80::3%1",
				RegularExpressionPatternType.IpAddressIPv6));

			// link-local with zone index (linux, BSD and MAC)
			Assert.IsTrue(Validator.ValidateInput("fe80::3%eth0",
				RegularExpressionPatternType.IpAddressIPv6));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[SuppressMessage("SharedAssemblies.Tools.StyleCopRules.ReadabilityAnalyzer",
			"ST2002:MethodCannotExceedMaxLines", Justification = "Unit Test.")]
		[TestMethod]
		public void ValidateIpAddressIpv6FailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.IpAddressIPv4));
			Assert.IsFalse(Validator.ValidateInput("02001:0000:1234:0000:0000:C1C0:ABCD:0876",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("2001:0000:1234:0000:00001:C1C0:ABCD:0876",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(" 2001:0000:1234:0000:0000:C1C0:ABCD:0876  0",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("2001:0000:1234: 0000:0000:C1C0:ABCD:0876",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("3ffe:0b00:0000:0001:0000:0000:000a",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("FF02:0000:0000:0000:0000:0000:0000:0000:0001",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("3ffe:b00::1::a",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1111:2222:3333:4444:5555:6666::",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:400.2.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:260.2.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:256.2.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.256.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.2.256.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.2.3.256",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:300.2.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.300.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.2.300.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.2.3.300",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:900.2.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.900.3.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.2.900.4",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:1.2.3.900",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:300.300.300.300",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::5:3000.30.30.30",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::400.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::260.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::256.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.256.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.2.256.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.2.3.256", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::300.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.300.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.2.300.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.2.3.300", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::900.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.900.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.2.900.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::1.2.3.900", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::300.300.300.300",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1::3000.30.30.30",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::400.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::260.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::256.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.256.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.2.256.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.2.3.256", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::300.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.300.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.2.300.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.2.3.300", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::900.2.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.900.3.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.2.900.4", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::1.2.3.900", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::300.300.300.300",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::3000.30.30.30",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("2001:DB8:0:0:8:800:200C:417A:221",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("FF01::101::2", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1111:2222:3333:4444::5555:",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1111:2222:3333::5555:",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1111:2222::5555:",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1111::5555:", 
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("::5555:",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1111:",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":1111:2222:3333:4444::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":1111:2222:3333::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":1111:2222::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":1111::5555",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":::5555",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput(":::",
				RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1.2.3.4:1111:2222:3333:4444::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1.2.3.4:1111:2222:3333::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1.2.3.4:1111:2222::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1.2.3.4:1111::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1.2.3.4::5555",
			     RegularExpressionPatternType.IpAddressIPv6));
			Assert.IsFalse(Validator.ValidateInput("1.2.3.4::",
				RegularExpressionPatternType.IpAddressIPv6));

			// zone index too long
			Assert.IsFalse(Validator.ValidateInput("::%1345",
				RegularExpressionPatternType.IpAddressIPv6));

			//zone index too long
			Assert.IsFalse(Validator.ValidateInput("12ab:0:0:1::2%eth2000",
				RegularExpressionPatternType.IpAddressIPv6));

			//can't exceed 128
			Assert.IsFalse(Validator.ValidateInput("::/129",
				RegularExpressionPatternType.IpAddressIPv6));

			//invalid zone index
			Assert.IsFalse(Validator.ValidateInput("fe80::3%ethx0",
				RegularExpressionPatternType.IpAddressIPv6));

			//invalid zone index
			Assert.IsFalse(Validator.ValidateInput("fe80::3%%",
				RegularExpressionPatternType.IpAddressIPv6));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateNameSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("Mary Joe",
				RegularExpressionPatternType.Name));
			Assert.IsTrue(Validator.ValidateInput("Mary",
				RegularExpressionPatternType.Name));
			Assert.IsTrue(Validator.ValidateInput("Bob O'Fallon",
				RegularExpressionPatternType.Name));
            Assert.IsTrue(Validator.ValidateInput("D.",
                RegularExpressionPatternType.Name));
            Assert.IsTrue(Validator.ValidateInput("Jackie-O'Lantern",
                RegularExpressionPatternType.Name));
            Assert.IsTrue(Validator.ValidateInput("Smith, John",
                RegularExpressionPatternType.Name));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateNameFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Name));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Name));
			Assert.IsFalse(Validator.ValidateInput("45",
				RegularExpressionPatternType.Name));
			Assert.IsFalse(Validator.ValidateInput("<Mary>",
				RegularExpressionPatternType.Name));
			Assert.IsFalse(Validator.ValidateInput("Donald+Duck",
				RegularExpressionPatternType.Name));
			Assert.IsFalse(Validator.ValidateInput("Mark?Snyder",
				RegularExpressionPatternType.Name));
			Assert.IsFalse(Validator.ValidateInput("Mark	Snyder",
				RegularExpressionPatternType.Name));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateNumberSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12345",
				RegularExpressionPatternType.Number));
			Assert.IsTrue(Validator.ValidateInput("-12345",
				RegularExpressionPatternType.Number));				
			Assert.IsTrue(Validator.ValidateInput("12.345",
				RegularExpressionPatternType.Number));
			Assert.IsTrue(Validator.ValidateInput("-12.345",
				RegularExpressionPatternType.Number));
			Assert.IsTrue(Validator.ValidateInput("+12.345",
				RegularExpressionPatternType.Number));				
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateNumberFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput("1,234",
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput("--2345",
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput("++2345",
				RegularExpressionPatternType.Number));
			Assert.IsFalse(Validator.ValidateInput("+-2345",
				RegularExpressionPatternType.Number));								
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidatePhoneSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("(888) 888-8888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("888 888.8888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("888-888-8888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("314-965-1555",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("1 (888) 888 8888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("341-8888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("3418888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("8773418888",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("8773418888 x25",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("8773418888 X2121",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("8773418888 x4",
				RegularExpressionPatternType.Phone));
			Assert.IsTrue(Validator.ValidateInput("1 (877) 341.8888 x4141",
			    RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1 (877) 341.8888 x41415",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("2223333 X1234",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("2223333 x1234",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("2223333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("888-222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("8882223333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("(888)-222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("(888).222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1 (888).222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1-(888).222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1.(888)2223333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("(888).222.3333 x1234",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("8882223333 x4",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("8882223333 X2121",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("888 222 3333 x25",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("888-222-3333 x25",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("888.222.3333 x25",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1 888.222.3333 x4141",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1 888.222.3333x4141",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1 888.222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1.888.222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1-888.222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1-(888).222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("1 (888) 222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("888 222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("(888) 222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("12223334444",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("12223334444x55555",
                RegularExpressionPatternType.Phone));
            Assert.IsTrue(Validator.ValidateInput("12223334444 x55555",
                RegularExpressionPatternType.Phone));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidatePhoneFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("1000) 341.5666",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("34555555",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("2 (344) 455 5555",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("1 (888) 888  8888",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("34555555 ext 4545",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("34555555x5",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("34555555 x45-45",
				RegularExpressionPatternType.Phone));
			Assert.IsFalse(Validator.ValidateInput("1 (877) 341.8888 x414155",
				RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("12213333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("22",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("Asd-asd-asdf",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("888 222 333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("(888) 222     3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("888)222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("(888222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("888) 222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("1888)-222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("1-888)-222.3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("1 (888) -222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("12223334444x555555",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("1-444-5555",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("1-444-5555",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("4444",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("188-222-3333",
                RegularExpressionPatternType.Phone));
            Assert.IsFalse(Validator.ValidateInput("888-122-3333",
                RegularExpressionPatternType.Phone));	
		}

        /// <summary>
        /// Tests the input validation method
        /// </summary>
        [TestMethod]
        public void ValidatePhoneWithAreaCodeSuccessTest()
        {
            Assert.IsTrue(Validator.ValidateInput("(888) 888-8888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888 888.8888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888-888-8888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("314-965-1555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 (888) 888 8888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8773418888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8773418888 x25",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8773418888 X2121",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8773418888 x4",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 (877) 341.8888 x4141",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 (877) 341.8888 x41415",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888-222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8882223333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("(888)-222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("(888).222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 (888).222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1-(888).222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1.(888)2223333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("(888).222.3333 x1234",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8882223333 x4",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("8882223333 X2121",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888 222 3333 x25",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888-222-3333 x25",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888.222.3333 x25",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 888.222.3333 x4141",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 888.222.3333x4141",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 888.222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1.888.222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1-888.222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1-(888).222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("1 (888) 222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("888 222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("(888) 222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("12223334444",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("12223334444x55555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsTrue(Validator.ValidateInput("12223334444 x55555",
                RegularExpressionPatternType.PhoneWithAreaCode));
        }

        /// <summary>
        /// Tests the input validation method
        /// </summary>
        [TestMethod]
        public void ValidatePhoneWithAreaCodeFailTest()
        {
            Assert.IsFalse(Validator.ValidateInput(null,
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput(string.Empty,
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("Hello Mom",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("341-8888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("3418888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("2223333 X1234",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("2223333 x1234",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("2223333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1000) 341.5666",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("34555555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("2 (344) 455 5555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1 (888) 888  8888",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("34555555 ext 4545",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("34555555x5",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("34555555 x45-45",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1 (877) 341.8888 x414155",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("12213333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("22",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("Asd-asd-asdf",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("888 222 333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("(888) 222     3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("888)222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("(888222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("888) 222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1888)-222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1-888)-222.3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1 (888) -222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("12223334444x555555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1-444-5555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("1-444-5555",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("4444",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("188-222-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
            Assert.IsFalse(Validator.ValidateInput("888-122-3333",
                RegularExpressionPatternType.PhoneWithAreaCode));
        }		

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOptionSymbolBackOfficeSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("F     123456P12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsTrue(Validator.ValidateInput("F     123456p12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsTrue(Validator.ValidateInput("F     123456C12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsTrue(Validator.ValidateInput("F     123456c12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsTrue(Validator.ValidateInput("CMG   123456P12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsTrue(Validator.ValidateInput("AAPL  123456P12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsTrue(Validator.ValidateInput("GOOG  123456C12345678",
			    RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsTrue(Validator.ValidateInput("GOOG1 123456C12345678",
                RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsTrue(Validator.ValidateInput("AAPLL 123456C12345678",
                RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsTrue(Validator.ValidateInput("CMG4  123456C12345678",
                RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsTrue(Validator.ValidateInput("F5    123456C12345678",
                RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsTrue(Validator.ValidateInput("GE1   123456C12345678",
                RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsTrue(Validator.ValidateInput("GE9   123456C12345678",
                RegularExpressionPatternType.OptionSymbolBackOffice));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOptionSymbolBackOfficeFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsFalse(Validator.ValidateInput("123456",
			     RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsFalse(Validator.ValidateInput(".GOOG  123456C12345678",
			     RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsFalse(Validator.ValidateInput(".GOOG  123456 12345678",
			     RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsFalse(Validator.ValidateInput(".GOOG123456C12345678",
			     RegularExpressionPatternType.OptionSymbolBackOffice));
			Assert.IsFalse(Validator.ValidateInput("GOOG  12E456C12345678",
			     RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput("1GOOG 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput("GO OG 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput("GOO 2 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput("GE 4  12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput("GE12  12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput("GOOGO312E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
            Assert.IsFalse(Validator.ValidateInput(" GOOG 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBackOffice));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOptionSymbolBashworkSuccessTest()
		{
            Assert.IsTrue(Validator.ValidateInput(".F     123456P12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".F     123456p12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".F     123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".F     123456c12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".CMG   123456P12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".AAPL  123456P12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".GOOG  123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".GOOG1 123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".AAPLL 123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".CMG4  123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".F5    123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".GE1   123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsTrue(Validator.ValidateInput(".GE9   123456C12345678",
                RegularExpressionPatternType.OptionSymbolBashwork));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOptionSymbolBashworkFailTest()
		{
            Assert.IsFalse(Validator.ValidateInput(null,
                RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(string.Empty,
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput("123456",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput("GOOG  123456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput("GOOG  123456 12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput("GOOG123456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".GOOG  12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".1GOOG 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".GO OG 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".GOO 2 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".GE 4  12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".GE12  12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(".GOOGO312E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
            Assert.IsFalse(Validator.ValidateInput(". GOOG 12E456C12345678",
                 RegularExpressionPatternType.OptionSymbolBashwork));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOptionSymbolDisplayableSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("F 2.50 JAN 10 C",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsTrue(Validator.ValidateInput("GOOG 2.50 JAN 10 WK3 C",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsTrue(Validator.ValidateInput("GE 25.00 DEC 08 WK1 AJ9 NS C",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsTrue(Validator.ValidateInput("GE 25.00 DEC 08 AJ9 NS C",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsTrue(Validator.ValidateInput("GE 25.00 DEC 08 WK1 AJ9 C",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsTrue(Validator.ValidateInput("GE 25.00 DEC 08 WK1 AJ9 NS P",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsTrue(Validator.ValidateInput("GE 25.00 DEC 08 WK1 AJ9 NS p",
			    RegularExpressionPatternType.OptionSymbolDisplayable));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOptionSymbolDisplayableFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("123456",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE .00 DEC 08 WK1 AJ9 NS C",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE 25.00 DC 08 WK1 AJ9 NS C",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE 25.00 DEC 08 WK133 AJ9 NS C",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE 25.00 DEC 08 WK1 NSC",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE 25.00 DEC 08 WK1 NS Q",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE 25.00 DEC 08 WK1 AJ C",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
			Assert.IsFalse(Validator.ValidateInput("GE 25.00 DEC 08 WK1 AJ1 NSC",
			     RegularExpressionPatternType.OptionSymbolDisplayable));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOrderIdSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("XAC211E20091116",
				RegularExpressionPatternType.OrderId));
			Assert.IsTrue(Validator.ValidateInput("PM1211E20091116",
				RegularExpressionPatternType.OrderId));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateOrderIdFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.OrderId));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.OrderId));
			Assert.IsFalse(Validator.ValidateInput("123456",
				RegularExpressionPatternType.OrderId));
			Assert.IsFalse(Validator.ValidateInput("1AC211E200911165",
				RegularExpressionPatternType.OrderId));
			Assert.IsFalse(Validator.ValidateInput("XAC211E200911165",
				RegularExpressionPatternType.OrderId));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidatePasswordSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("This 1 is a strong password!",
			    RegularExpressionPatternType.Password));
			Assert.IsTrue(Validator.ValidateInput("Pr0c3ss!",
				RegularExpressionPatternType.Password));
			Assert.IsTrue(Validator.ValidateInput("(100% Happy Times.)",
			    RegularExpressionPatternType.Password));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidatePasswordFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("Password",
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("MyPassword",
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("(Bad1!)",
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("pr0c3ss!",
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("PR0C3SS!",
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("PRoCeSS!",
				RegularExpressionPatternType.Password));
			Assert.IsFalse(Validator.ValidateInput("PRoCeSSes",
				RegularExpressionPatternType.Password));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidatePasswordBashworkAccountDefaultSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("Smit01011975",
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
			Assert.IsTrue(Validator.ValidateInput("Snud01011975",
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
			Assert.IsTrue(Validator.ValidateInput("Ba10102000",
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidatePasswordBashworkAccountDefaultFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
			Assert.IsFalse(Validator.ValidateInput("smit01011975",
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
			Assert.IsFalse(Validator.ValidateInput("Smit111975",
				RegularExpressionPatternType.PasswordBashworkAccountDefault));
			Assert.IsFalse(Validator.ValidateInput("SMIT01011975",
				RegularExpressionPatternType.PasswordBashworkAccountDefault));				
		}		

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateSpecialCharactersSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("*",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("~",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("!",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("@",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("#",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("$",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("%",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("^",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("&",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("*",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("(",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput(")",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("|",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput(@"\",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("?",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("/",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("<",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput(">",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("=",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("+",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsTrue(Validator.ValidateInput("_",
				RegularExpressionPatternType.SpecialCharacters));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateSpecialCharactersFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.SpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
			     RegularExpressionPatternType.SpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.SpecialCharacters));
			Assert.IsFalse(Validator.ValidateInput("12345-1",
				RegularExpressionPatternType.SpecialCharacters));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateSsnSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("123-45-6789",
				RegularExpressionPatternType.Ssn));
			Assert.IsTrue(Validator.ValidateInput("123456789",
				RegularExpressionPatternType.Ssn));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateSsnFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("123 45 6789",
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("123-45 6789",
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("123 45-6789",
				RegularExpressionPatternType.Ssn));
			Assert.IsFalse(Validator.ValidateInput("1234567890",
				RegularExpressionPatternType.Ssn));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateTaxIdentificationNumberSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12-3456789",
			    RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsTrue(Validator.ValidateInput("123456789",
			    RegularExpressionPatternType.TaxIdentificationNumber));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateTaxIdentificationNumberFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
			     RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
			     RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsFalse(Validator.ValidateInput("1234",
			     RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsFalse(Validator.ValidateInput("A2345",
			     RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsFalse(Validator.ValidateInput("12-34567890",
			     RegularExpressionPatternType.TaxIdentificationNumber));
			Assert.IsFalse(Validator.ValidateInput("1234567890",
			     RegularExpressionPatternType.TaxIdentificationNumber));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateUrlSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("http://www.google.com",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://www.google.com",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://www.google.net",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://www.google.org",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://www.google.eu",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("https://www.google.com",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://flmwebdev00",
				RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://flmwebdev00/VirtualDirectory/",
			    RegularExpressionPatternType.Url));
			Assert.IsTrue(Validator.ValidateInput("http://flmwebdev00/VirtualDirectory/?var=56",
			    RegularExpressionPatternType.Url));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateUrlFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("12345-1",
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("google.com",
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("www.google.com",
				RegularExpressionPatternType.Url));
			Assert.IsFalse(Validator.ValidateInput("ftp://google.net",
				RegularExpressionPatternType.Url));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateZipCodeSuccessTest()
		{
			Assert.IsTrue(Validator.ValidateInput("12345-1234",
				RegularExpressionPatternType.ZipCode));
			Assert.IsTrue(Validator.ValidateInput("12345",
				RegularExpressionPatternType.ZipCode));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateZipCodeFailTest()
		{
			Assert.IsFalse(Validator.ValidateInput(null,
				RegularExpressionPatternType.ZipCode));
			Assert.IsFalse(Validator.ValidateInput(string.Empty,
				RegularExpressionPatternType.ZipCode));
			Assert.IsFalse(Validator.ValidateInput("Hello Mom",
				RegularExpressionPatternType.ZipCode));
			Assert.IsFalse(Validator.ValidateInput("1234",
				RegularExpressionPatternType.ZipCode));
			Assert.IsFalse(Validator.ValidateInput("A2345",
				RegularExpressionPatternType.ZipCode));
			Assert.IsFalse(Validator.ValidateInput("12345-1",
				RegularExpressionPatternType.ZipCode));
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAddHeadingTrailingSpaceSuccessTest()
		{
			// Account Number
			Assert.IsTrue(Regex.Match("11111327",
				Validator.AccountNumberRegularExpressionPattern).Success);
			Assert.IsFalse(Regex.Match(" 11111327 ", 
				Validator.AccountNumberRegularExpressionPattern).Success);
			
			string accountNumberWithSpacePattern = 
				Validator.AddHeadingTrailingSpace(Validator.AccountNumberRegularExpressionPattern);
			Assert.IsTrue(Regex.Match(" 11111327 ", accountNumberWithSpacePattern).Success);
			
			// Email
			Assert.IsTrue(Regex.Match("joesmith@gmail.com",
				Validator.EmailRegularExpressionPattern).Success);
			Assert.IsFalse(Regex.Match("joesmith@gmail.com ",
				Validator.EmailRegularExpressionPattern).Success);

			string emailWithSpacePattern =
				Validator.AddHeadingTrailingSpace(Validator.EmailRegularExpressionPattern);
			Assert.IsTrue(Regex.Match("joesmith@gmail.com ", emailWithSpacePattern).Success);
			
			// Equity Symbol
			Assert.IsTrue(Regex.Match("GOOG",
				Validator.EquitySymbolRegularExpressionPattern).Success);
			Assert.IsFalse(Regex.Match(" GOOG",
				Validator.EquitySymbolRegularExpressionPattern).Success);

			string equitySymbolWithSpacePattern =
				Validator.AddHeadingTrailingSpace(Validator.EquitySymbolRegularExpressionPattern);
			Assert.IsTrue(Regex.Match(" GOOG", equitySymbolWithSpacePattern).Success);
		}

		/// <summary>
		/// Tests the input validation method
		/// </summary>
		[TestMethod]
		public void ValidateAddHeadingTrailingSpaceFailTest()
		{
			// Account Number
			string accountNumberWithSpacePattern =
				Validator.AddHeadingTrailingSpace(Validator.AccountNumberRegularExpressionPattern);
			Assert.IsTrue(Regex.Match("   11111327    ", accountNumberWithSpacePattern).Success);
			Assert.IsFalse(Regex.Match("5 11111327 ", accountNumberWithSpacePattern).Success);

			// Email
			string emailWithSpacePattern =
				Validator.AddHeadingTrailingSpace(Validator.EmailRegularExpressionPattern);
			Assert.IsTrue(Regex.Match(" joesmith@gmail.com ", emailWithSpacePattern).Success);
			Assert.IsFalse(Regex.Match("joesmith@gmail@.com ", emailWithSpacePattern).Success);

			// Equity Symbol
			string equitySymbolWithSpacePattern =
				Validator.AddHeadingTrailingSpace(Validator.EquitySymbolRegularExpressionPattern);
			Assert.IsTrue(Regex.Match(" GOOG", equitySymbolWithSpacePattern).Success);
			Assert.IsFalse(Regex.Match(" GOOG GOOG ", equitySymbolWithSpacePattern).Success);
		}

		/// <summary>
		/// Validates that the null input is valid.
		/// </summary>
		[TestMethod]
		public void ValidateOnNullInputIsValid()
		{
			// Arrange
			string input = null;

			// Act - call overload to make null or empty valid
			bool actual = Validator.ValidateInput(input, RegularExpressionPatternType.Alpha, true);

			// Assert
			Assert.IsTrue(actual);
		}

		/// <summary>
		/// Validates that the empty input is valid.
		/// </summary>
		[TestMethod]
		public void ValidateOnEmptyInputIsValid()
		{
			// Arrange
			string input = string.Empty;

			// Act - call overload to make null or empty valid
			bool actual = Validator.ValidateInput(input, RegularExpressionPatternType.Alpha, true);

			// Assert
			Assert.IsTrue(actual);
		}

		/// <summary>
		/// Validates that input of just whitespace is still invalid.
		/// </summary>
		[TestMethod]
		public void ValidateOnSpacesInputIsInvalid()
		{
			// Arrange
			string input = " ";

			// Act - call overload to make null or empty is valid
			bool actual = Validator.ValidateInput(input, RegularExpressionPatternType.Alpha, true);

			// Assert
			Assert.IsFalse(actual);
		}

		/// <summary>
		/// Validates that the null input is invalid.
		/// </summary>
		[TestMethod]
		public void ValidateOnNullInputIsInvalid()
		{
			// Arrange
			string input = null;

			// Act - call overload to make null or empty invalid
			bool actual = Validator.ValidateInput(input, RegularExpressionPatternType.Alpha, false);

			// Assert
			Assert.IsFalse(actual);
		}

		/// <summary>
		/// Validates that the empty input is invalid.
		/// </summary>
		[TestMethod]
		public void ValidateOnEmptyInputIsInvalid()
		{
			// Arrange
			string input = string.Empty;

			// Act - call overload to make null or empty invalid
			bool actual = Validator.ValidateInput(input, RegularExpressionPatternType.Alpha, false);

			// Assert
			Assert.IsFalse(actual);
		}
	}
}