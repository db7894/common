using System;
using Bashwork.Validation.Attributes;

namespace Bashwork.Validation.Tests.Types
{
	[Validator]
	public static class TestAttributeValidators
	{
		[ValidatorMethod(typeof(ExampleClassType))]
		public static ValidationResult Check(ExampleClassType input)
		{
			return Validate.That(input)
				.Property(x => x.Age).Obeys(x => x < 100).And()
				.Validate();
		}

		[ValidatorMethod(null)]
		public static ValidationResult Check(TimeSpan input)
		{
			return Validate.That(input)
				.Property(x => x.Days).Obeys(x => x < 100).And()
				.Validate();
		}
	}
}
