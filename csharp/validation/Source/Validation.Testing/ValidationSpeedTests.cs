using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bashwork.General.Validation.Rules;
using Bashwork.General.Validation.Tests.Types;

namespace Bashwork.General.Validation.Tests
{
	/// <summary>
	/// Code to test that the performance of the validator is up to snuff
	/// </summary>
	[TestClass]
	public class ValidationSpeedTests
	{
		[TestMethod]
		public void Validation_IsFastEnough()
		{
			int iterations = 1000;
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Person",
				Age = 24,
				Collection = new [] { 'a', 'b', 'c' },
				Insured = true,
				Birthday = DateTime.Now.AddYears(-1),
			};
			var validator = Validate.That<ExampleClassType>()
				.Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").And()
				.Property(x => x.Age).IsEven().IsLessThan(65).And()
				.Property(x => x.Collection).ContainsAnyOf('a', 'z', 'y').And()
				.Property(x => x.Insured).IsTrue().And()
				.Property(x => x.Birthday).IsPast().And()	// very slow
				.Compile().Value;

			var watch = Stopwatch.StartNew();
			for (var count = 0; count <= iterations; ++count)
			{
				var result = validator.Validate(handle);
			}
			watch.Stop();

			Assert.IsTrue(watch.ElapsedMilliseconds  / iterations <= 1);
		}

		[TestMethod]
		public void Validation_Against_Baseline()
		{
			int iterations = 1000;
			var handle = new ExampleClassType
			{
				FirstName = "Mr. Person",
				Age = 24,
				Collection = new[] { 'a', 'b', 'c' },
				Insured = true,
				Birthday = DateTime.Now.AddYears(-1),
			};			
			var contains = new[] { 'a', 'z', 'y' };
	
			var watch = Stopwatch.StartNew();
			for (var count = 0; count <= iterations; ++count)
			{
				var result = (!string.IsNullOrEmpty(handle.FirstName) && handle.FirstName.Contains("Mr"))
					&& ((handle.Age % 2 == 0) && (handle.Age < 65))
					&& (contains.Any(v => handle.Collection.Contains(v)))
					&& (handle.Insured)
					&& (handle.Birthday < DateTime.Now);	// very slow
			}
			watch.Stop();

			Assert.IsTrue(watch.ElapsedMilliseconds / iterations <= 1);
		}
	}
}
