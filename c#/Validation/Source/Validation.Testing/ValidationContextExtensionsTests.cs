using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Validation.Tests
{
	/// <summary>
	/// Code to test the various extension methods attatched to the validation context.
	/// </summary>
	[TestClass]
	public class ValidationContextExtensionsTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidationContext_StopOnFirstFailure_ThrowsWithNullContext()
		{
			ValidationContext<string> context = null;
			context.StopOnFirstFailure();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidationContext_Property_ThrowsWithNullContext()
		{
			ValidationContext<string> context = null;
			context.Property(x => x.Length);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidationContext_This_ThrowsWithNullContext()
		{
			ValidationContext<string> context = null;
			context.This();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidationContext_Property_ThrowsWithNullExpression()
		{
			ValidationContext<string> context = new ValidationContext<string>(string.Empty);
			context.Property<string, int>(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidationContext_ThrowOnFailure_ThrowsWithNullContext()
		{
			ValidationContext<string> context = null;
			context.ThrowOnFailure();
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ValidationContext_With_ThrowsWithNullContext()
		{
			ValidationContext<string> context = null;
			context.With(null);
		}

		[TestMethod]
		public void ValidationContext_With_WorksCorrectly()
		{
			var options = ValidationOptions.Default;
			var context = new ValidationContext<string>(null);

			context.With(null);
			Assert.AreEqual(context.Options.CascadeBehavior, options.CascadeBehavior);
			Assert.AreEqual(context.Options.ReportingBehavior, options.ReportingBehavior);

			context.With(new ValidationOptions { CascadeBehavior = CascadeMode.Continue });
			Assert.AreEqual(context.Options.CascadeBehavior, CascadeMode.Continue);
		}

		[TestMethod]
		public void ValidationContext_OptionExtensionMethods_WorkCorrectly()
		{
			var context = new ValidationContext<string>(null);
			Assert.AreEqual(context.Options.CascadeBehavior, CascadeMode.Continue);
			context.StopOnFirstFailure();
			Assert.AreEqual(context.Options.CascadeBehavior, CascadeMode.StopOnFirstFailure);

			Assert.AreEqual(context.Options.ReportingBehavior, ReportingMode.ReturnValidationResults);
			context.ThrowOnFailure();
			Assert.AreEqual(context.Options.ReportingBehavior, ReportingMode.ThrowValidationException);
		}
	}
}
