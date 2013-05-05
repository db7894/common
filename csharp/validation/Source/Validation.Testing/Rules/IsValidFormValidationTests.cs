using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Tests.Internal;
using Bashwork.General.Validation.Tests.Types;
using Bashwork.General.Validation.Rules;
using System.Collections.Generic;

namespace Bashwork.General.Validation.Tests.Rules
{
	/// <summary>
	/// A collection of validation methods that check if the supplied
	/// input string is one of the available valid types (backed by regular expressions)
	/// </summary>
	[TestClass]
	public class IsValidFormValidationTests
	{
		[TestMethod]
		public void ValidateThat_IsValidFormValidation_ThrowsWithNullContext()
		{
			PropertyContext<ExampleClassType, string> context = null;

			AssertEx.Throws<ArgumentNullException>(() => context.IsValid(FormValidationType.Address));
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsValidForm()
		{
			var handle = new ExampleClassType { FirstName = "name@email.com" };
			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsValid(FormValidationType.Email).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "name email com" };
			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsValid(FormValidationType.AlphaAndSpaces).And()
				.Compile().Value.Validate(handle);
			Assert.IsTrue(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_SomeValue_IsInValidForm()
		{
			var handle = new ExampleClassType { FirstName = "!@#!@#%$" };
			var result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsValid(FormValidationType.Alpha).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);

			handle = new ExampleClassType { FirstName = "Hello World" };
			result = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsValid(FormValidationType.IpAddressIPv4).And()
				.Compile().Value.Validate(handle);
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_InvalidForms_AreInValid()
		{
			var result = Validate.That<string>()
				.This().IsValid((FormValidationType)0x1234).And()
				.Compile().Value.Validate("doesn't matter");
			Assert.IsFalse(result.IsSuccessful);
		}

		[TestMethod]
		public void ValidateThat_AllForms_AreValid()
		{
			foreach (var validation in _validations)
			{
				var result = Validate.That<string>()
					.This().IsValid(validation.Value).And()
					.Compile().Value.Validate(validation.Key);
				Assert.IsTrue(result.IsSuccessful);
			}
		}

		/// <summary>
		/// Examples of all the form validations to test with
		/// </summary>
		private static IDictionary<string, FormValidationType> _validations
			= new Dictionary<string, FormValidationType>
		{
			{ "1234 Some Street Jacksonville, FL, 12345", FormValidationType.Address },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz", FormValidationType.Alpha },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz ", FormValidationType.AlphaAndSpaces },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", FormValidationType.AlphaAndDigits },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz 0123456789", FormValidationType.AlphaAndDigitsAndSpaces },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz_0123456789", FormValidationType.AlphaAndDigitsAndUnderscores },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz_ 0123456789", FormValidationType.AlphaAndDigitsAndUnderscoresAndSpaces },
			{ "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz 0123456789!@#$%^&*~", FormValidationType.AlphaAndDigitsAndSpacesAndSpecialCharacters },
			{ "St. Louis", FormValidationType.City },
			{ "$22.00", FormValidationType.Currency },
			{ "11-30-2001", FormValidationType.Date },
			{ "john@doe.com", FormValidationType.Email },
			{ "example a", FormValidationType.FreeFormText },
			{ "example b", FormValidationType.FreeFormLatinAndChineseText },
			{ "-12345", FormValidationType.Integer },
			{ "12345", FormValidationType.IntegerUnsigned },
			{ "127.0.0.1", FormValidationType.IpAddressIPv4 },
			{ "fe80::1", FormValidationType.IpAddressIPv6 },
			{ "John Doe", FormValidationType.Name },
			{ "22.23", FormValidationType.Number },
			{ "p4$$w0rD", FormValidationType.Password },
			{ "123-456-7890", FormValidationType.Phone },
			{ "!@#$%^&*", FormValidationType.SpecialCharacters },
			{ "123-45-6789", FormValidationType.Ssn },
			{ "http://www.google.com", FormValidationType.Url },
			{ "54321", FormValidationType.ZipCode },
		};
	}
}
