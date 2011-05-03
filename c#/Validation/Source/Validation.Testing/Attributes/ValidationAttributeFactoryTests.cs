using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.Validation.Attributes;
using Bashwork.Validation.Tests.Types;

namespace Bashwork.Validation.Tests.Attributes
{
	[TestClass]
	public class ValidationAttributeFactoryTests
	{
		[TestMethod]
		public void ValidationAttributeFactory_FindsValidator_Successfully()
		{
			var handle = new ExampleClassType { Age = 25 };
			var validator = ValidationAttributeFactory.GetValidator<ExampleClassType>();
			var results = validator(handle);
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		public void ValidationAttributeFactory_FindsValidatorInAssembly_Successfully()
		{
			var assembly = Assembly.GetAssembly(typeof(ExampleClassType));
			var handle = new ExampleClassType { Age = 25 };
			var validator = ValidationAttributeFactory.GetValidator<ExampleClassType>(assembly);
			var results = validator(handle);
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ValidationAttributeFactory_FindsValidatorWithoutAttributeType()
		{
			var handle = TimeSpan.FromDays(1);
			var validator = ValidationAttributeFactory.GetValidator<TimeSpan>();
			var results = validator(handle);
			Assert.IsTrue(results.IsSuccessful);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ValidationAttributeFactory_CannotFindValidator()
		{
			var handle = DateTime.Now;
			var validator = ValidationAttributeFactory.GetValidator<DateTime>();
			var results = validator(handle);
			Assert.IsTrue(results.IsSuccessful);
		}
	}
}
