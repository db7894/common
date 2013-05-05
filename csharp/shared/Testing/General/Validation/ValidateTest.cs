using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.Validation.UnitTests
{
	/// <summary>
	/// ValidateTest contains multiple methods that excersize the Validate, ValidateChain,
	/// ValidationError, and ValidationException.
	/// </summary>
	[TestClass]
	public class ValidateTest
	{
		/// <summary>
		/// That() failure tests.
		/// </summary>
		[TestMethod]
		public void ThatFailureTests()
		{
			Stopwatch st = new Stopwatch();

			try
			{
				Validate.That<string>().IsNotNull().ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>(null).IsNotNull().ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>().IsIn(new string[] { }).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>(null).IsIn(new string[] { }).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>().Matches(string.Empty).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>(null).Matches(string.Empty).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>().Obeys(null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That<string>(null).Obeys(null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// Validation can have multiple calls to the That() method, so
		/// validate that an exception will be generated if the second That()
		/// is passed no parameters or null parameters.
		/// </summary>
		[TestMethod]
		public void SecondThatFailureTests()
		{
			Stopwatch st = new Stopwatch();
			string p1 = "hoser";

			try
			{
				Validate.That(p1).IsNotNull().That().ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(p1).IsNotNull().That(null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ArgumentException ex)
			{
				Assert.IsTrue(
					ex.Message == "Missing parameters to validate!", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// Tests that verify that not asking for the stopwatch will not affect
		/// the outcome.
		/// </summary>
		[TestMethod]
		public void ValidateWithoutStopWatchTests()
		{
			string p1 = "hoser";

			try
			{
				Validate.That(p1).IsNotNull().ThrowOnError();
			}
			catch (ValidationException ex)
			{
				Assert.Fail("Should not have thrown a validation exception: " + ex.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(p1).IsNotNull().ThrowOnError().ThrowOnError();
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Error.MethodName == "ThrowOnError", "MethodName does not equal Validate.");
				Assert.IsTrue(ex.Error.Message == "Already validated. Verify that there is only one " + 
					"ThrowOnError() method at the end.", "Invalid message in exception");
				Assert.IsTrue(ex.Message == "Already validated. Verify that there is only one " +
					"ThrowOnError() method at the end.", "Invalid message in exception");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				p1 = null;
				Validate.That(p1).IsNotNull().ThrowOnError();
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Error.MethodName == "IsNotNull", "MethodName does not equal IsNotNull");
				Assert.IsTrue(ex.Error.Message == "1 parameter failed.", "Invalid exception message.");
				Assert.IsTrue(ex.Message == "1 parameter failed.", "Invalid exception message.");
				Assert.IsNull(ex.InnerException);
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				p1 = null;
				string p2 = null;
				Validate.That(p1, p2).ReportAll().IsNotNull().ThrowOnError();
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Message == "Multiple ValidationExceptions. See InnerExceptions.", 
					"Invalid exception message.");
				Assert.IsNotNull(ex.InnerException);
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// Three parameters with the second as null should generate an
		/// exception in which the second parameter is called out for each
		/// of the four validation methods.
		/// </summary>
		[TestMethod]
		public void SecondParameterExceptionTest()
		{
			string p1 = "test1";
			string p2 = null;
			string p3 = "test3";
			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PassThreeParametersAndCheckTheSecondParameter:");

			try
			{
				Validate.That(p1, p2, p3).IsNotNull().ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Error.MethodName == "IsNotNull", "MethodName not equal to IsNotNull");
				Assert.IsTrue(ex.Error.Message == "2 parameter failed.", "Invalid message in exception");
				Assert.IsTrue(ex.Message == "2 parameter failed.", "Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				string[] whiteList = { "test1", "test3" };
				Validate.That(p1, p2, p3).IsIn(whiteList).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Error.MethodName == "IsIn", "MethodName not equal to IsIn");
				Assert.IsTrue(ex.Error.Message == "2 parameter failed.", "Invalid message in exception");
				Assert.IsTrue(ex.Message == "2 parameter failed.", "Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				string pattern = "^[A-Z,0-9]";
				Validate.That(p1, p2, p3).Matches(pattern).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Error.MethodName == "Matches", "MethodName not equal to Matches");
				Assert.IsTrue(ex.Error.Message == "2 parameter failed.", "Invalid message in exception");
				Assert.IsTrue(ex.Message == "2 parameter failed.", "Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(p1, p2, p3).Obeys(v => v.Length > 0).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Error.MethodName == "Obeys", "MethodName not equal to Obeys");
				Assert.IsTrue(ex.Error.Message == "2 parameter failed.", "Invalid message in exception");
				Assert.IsTrue(ex.Message == "2 parameter failed.", "Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// ReportAll tests.
		/// </summary>
		[TestMethod]
		public void ReportAllTests()
		{
			string p1 = "test1";
			string p2 = null;
			string p3 = "test3";
			string p4 = null;
			string p5 = "test5";

			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PassFiveParametersAndCheckForTwoExceptions:");

			try
			{
				Validate.That(p1, p2, p3, p4, p5).ReportAll().IsNotNull().ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Message == "Multiple ValidationExceptions. See InnerExceptions.", 
					"Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());

				if (ex.ValicationErrors != null)
				{
					Assert.IsTrue(ex.ValicationErrors.Count() == 2, "Inner exceptions do not equal 2");

					foreach (ValidationError ve in ex.ValicationErrors)
					{
						Console.WriteLine(ve.MethodName + ": " + ve.Message);
					}
				}
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				string[] whiteList = { "test1", "test3", "test5" };
				Validate.That(p1, p2, p3, p4, p5).ReportAll().IsIn(whiteList).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Message == "Multiple ValidationExceptions. See InnerExceptions.", 
					"Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());

				if (ex.ValicationErrors != null)
				{
					Assert.IsTrue(ex.ValicationErrors.Count() == 2, "Inner exceptions do not equal 2");

					foreach (ValidationError ve in ex.ValicationErrors)
					{
						Console.WriteLine(ve.MethodName + ": " + ve.Message);
					}
				}
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				string pattern = "^[A-Z,0-9]";
				Validate.That(p1, p2, p3, p4, p5).ReportAll().Matches(pattern).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Message == "Multiple ValidationExceptions. See InnerExceptions.", 
					"Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());

				if (ex.ValicationErrors != null)
				{
					Assert.IsTrue(ex.ValicationErrors.Count() == 2, "Inner exceptions do not equal 2");

					foreach (ValidationError ve in ex.ValicationErrors)
					{
						Console.WriteLine(ve.MethodName + ": " + ve.Message);
					}
				}
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(p1, p2, p3, p4, p5)
					.ReportAll()
					.Obeys(v => v.Length > 0)
					.ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Assert.IsTrue(ex.Message == "Multiple ValidationExceptions. See InnerExceptions.", 
					"Invalid message in exception");
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());

				if (ex.ValicationErrors != null)
				{
					Assert.IsTrue(ex.ValicationErrors.Count() == 2, "Inner exceptions do not equal 2");

					foreach (ValidationError ve in ex.ValicationErrors)
					{
						Console.WriteLine(ve.MethodName + ": " + ve.Message);
					}
				}
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// IsIn Failure Tests
		/// </summary>
		[TestMethod]
		public void IsInFailureTests()
		{
			string p1 = null;
			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PassNullWhiteListToIsInAndFail:");

			try
			{
				Validate.That(p1).IsIn(null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
				Assert.IsTrue(ex.Message == "Failed because whiteList parameter is null.", 
					"Invalid exception message.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(p1).IsIn(new string[] { }).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
				Assert.IsTrue(ex.Message == "Failed because whiteList parameter is null.", 
					"Invalid exception message.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// Matches Failure Tests.
		/// </summary>
		[TestMethod]
		public void MatchesFailureTests()
		{
			string p1 = null;
			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PassNullPatternToMatchesAndFail:");

			try
			{
				Validate.That(p1).Matches((Regex)null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
				Assert.IsTrue(ex.Error.MethodName == "Matches", "Methodname does not equal Matches");
				Assert.IsTrue(ex.Error.Message == "Failed because regex parameter is null.", 
					"Invalid exception message.");
				Assert.IsTrue(ex.Message == "Failed because regex parameter is null.", 
					"Invalid exception message.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(p1).Matches((string)null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
				Assert.IsTrue(ex.Error.MethodName == "Matches", "Methodname does not equal Matches");
				Assert.IsTrue(ex.Error.Message == "Failed because pattern parameter is null.", 
					"Invalid exception message.");
				Assert.IsTrue(ex.Message == "Failed because pattern parameter is null.", 
					"Invalid exception message.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(p1).IsNotNull().Matches((Regex)null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(p1).IsNotNull().Matches((string)null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				string p2 = "hoser";
				Validate.That(p2).IsNotNull().ThrowOnError().Matches((string)null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// Obeys Failure tests.
		/// </summary>
		[TestMethod]
		public void ObeysFailureTests()
		{
			string p1 = null;
			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("PassNullRuleToObeysAndFail:");

			try
			{
				Validate.That(p1).Obeys(null).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
				Assert.IsTrue(ex.Error.MethodName == "Obeys", "MethodName does not equal Obeys");
				Assert.IsTrue(ex.Error.Message == "Failed because ruleBody parameter is null.", 
					"Invalid exception message.");
				Assert.IsTrue(ex.Message == "Failed because ruleBody parameter is null.", 
					"Invalid exception message.");
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(p1).Obeys(v => v.Length > 0).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(p1).IsNotNull().Obeys(v => v.Length > 0).ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// This is a test that mixes the four different validation methods and should
		/// generate 10 errors because ReportAll is present.
		/// </summary>
		[TestMethod]
		public void ReportAllTenExceptionsTest()
		{
			string[] whiteList = { "hoser", "hillbilly", "hangman" };
			string name1 = "hoser";
			string name2 = "goodtext";
			string emailAddress = "jayers@bashwork.com";
			string streetAddess = null;
			Regex reg = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
			Stopwatch st = new Stopwatch();

			try
			{
				Validate.That(name1, name2, emailAddress, streetAddess)
					.ReportAll()
					.IsNotNull()
					.Matches(Validator.EmailRegularExpressionPattern)
					.IsIn(whiteList)
					.Obeys(v => v.Length >= 10)
					.ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				if (ex.ValicationErrors.Count() > 0)
				{
					Assert.IsTrue(ex.ValicationErrors.Count() == 10, "Inner exceptions do not equal 10");
				}
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// ThrowOnError Failure tests.
		/// </summary>
		[TestMethod]
		public void ThrowOnErrorFailureTests()
		{
			string fname = "hoser";
			string[] whiteList = { "hoser", "hillbilly", "hangman" };
			Regex reg2 = new Regex(@"^[A-Z0-9._%+-]", RegexOptions.IgnoreCase);
			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("MisplacedValidateMethod:");

			try
			{
				Validate.That(fname)
					.IsNotNull()
					.Obeys(value => value.Length > 0)
					.IsIn(whiteList)
					.ThrowOnError(t => st = t)
					.Matches(reg2);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(fname)
					.IsNotNull()
					.Obeys(value => value.Length > 0)
					.ThrowOnError(t => st = t)
					.IsIn(whiteList)
					.Matches(reg2);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(fname)
					.IsNotNull()
					.ThrowOnError(t => st = t)
					.Obeys(value => value.Length > 0)
					.IsIn(whiteList)
					.Matches(reg2);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
			try
			{
				Validate.That(fname)
					.ThrowOnError(t => st = t)
					.IsNotNull()
					.Obeys(value => value.Length > 0)
					.IsIn(whiteList)
					.Matches(reg2);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(fname)
					.IsNotNull()
					.Obeys(value => value.Length > 0)
					.IsIn(whiteList)
					.Matches(reg2)
					.ThrowOnError(t => st = t)
					.ThrowOnError(t => st = t);
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}

			try
			{
				Validate.That(fname).IsNotNull().ThrowOnError().ReportAll();
				Assert.Fail("Should have thrown a validation exception");
			}
			catch (ValidationException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine("\tSpeed(ms): " + st.Elapsed.TotalMilliseconds.ToString());
			}
			catch (Exception ex)
			{
				Assert.Fail("Throws wrong exception: " + ex.ToString());
			}
		}

		/// <summary>
		/// These tests should not generate an exception.
		/// </summary>
		[TestMethod]
		public void HappyPath()
		{
			string fn = "hoser";
			string ln = "hillbilly";
			string[] whiteList = { "hoser", "hillbilly", "hangman" };
			string ea1 = "ab@xy.com";
			string ea2 = "cd@xy.com";
			Regex reg = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
			Regex reg2 = new Regex(@"^[A-Z0-9._%+-]", RegexOptions.IgnoreCase);
			int n1 = 1;
			int n2 = 2;
			int[] whiteListInts = { 1, 2, 3, 4, 5 };
			Stopwatch st = new Stopwatch();
			Console.WriteLine("--------------------------------------------------");
			Console.WriteLine("HappyPath:");

			// IsNotNull variations
			Validate.That(fn).IsNotNull().ThrowOnError(t => st = t); 
			Validate.That(fn, ln).IsNotNull().ThrowOnError(t => st = t);
			Validate.That(fn).ReportAll().IsNotNull().ThrowOnError(t => st = t); 
			Validate.That(fn, ln).ReportAll().IsNotNull().ThrowOnError(t => st = t);

			// IsIn variations
			Validate.That(fn).IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn, ln).IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn).ReportAll().IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn, ln).ReportAll().IsIn(whiteList).ThrowOnError(t => st = t);

			// IsNotNull + IsIn variations
			Validate.That(fn, ln).IsNotNull().IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn, ln).IsNotNull().That(ln).IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn).IsNotNull().That(ln).IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn, ln).ReportAll().IsNotNull().IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(fn, ln).ReportAll().IsNotNull().That(ln).IsIn(whiteList).ThrowOnError(t => st = t);

			// Matches variations
			Validate.That(ea1).Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1, ea2).Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1).ReportAll().Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1, ea2).ReportAll().Matches(reg).ThrowOnError(t => st = t);

			// Matches + IsNotNull variations
			Validate.That(ea1).IsNotNull().Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1, ea2).IsNotNull().Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1).ReportAll().IsNotNull().Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1).ReportAll().IsNotNull().That(ea2).Matches(reg).ThrowOnError(t => st = t);
			Validate.That(ea1, ea2).ReportAll().IsNotNull().That(ea2).Matches(reg).ThrowOnError(t => st = t);

			// Rule
			Validate.That(fn).Obeys(value => value.Length > 0).ThrowOnError(t => st = t);
			Validate.That(fn, ln).Obeys(value => value.Length > 0).ThrowOnError(t => st = t);
			Validate.That(fn).ReportAll().Obeys(value => value.Length > 0).ThrowOnError(t => st = t);
			Validate.That(fn, ln).ReportAll().Obeys(value => value.Length > 0).ThrowOnError(t => st = t);
			Validate.That(fn, ln).IsNotNull().That(ln).Obeys(value => value.Length > 0)
				.ThrowOnError(t => st = t);

			// Rule + IsNotNull
			Validate.That(fn).IsNotNull().Obeys(value => value.Length > 0).ThrowOnError(t => st = t);
			Validate.That(fn, ln).IsNotNull().Obeys(value => value.Length > 0).ThrowOnError(t => st = t);
			Validate.That(fn).ReportAll().IsNotNull().Obeys(value => value.Length > 0)
				.ThrowOnError(t => st = t);
			Validate.That(fn, ln).ReportAll().IsNotNull().Obeys(value => value.Length > 0)
				.ThrowOnError(t => st = t);
			Validate.That(fn).IsNotNull().That(ln).Obeys(value => value.Length > 0).IsNotNull()
				.ThrowOnError(t => st = t);

			// Combination variations
			Validate.That(fn, ln).IsNotNull().That(ea1, ea2).Matches(reg).That(fn, ln).IsIn(whiteList)
				.ThrowOnError(t => st = t);
			Validate.That(fn, ln).Obeys(value => value.Length > 0).IsIn(whiteList).ThrowOnError(t => st = t);
			Validate.That(n1, n2).IsIn(whiteListInts).That(n2).Obeys(value => value > 0)
				.ThrowOnError(t => st = t);
			Validate.That(n1, n2).Obeys(value => value > 0).IsIn(whiteListInts).ThrowOnError(t => st = t);
			Validate.That(fn).IsNotNull().Obeys(value => value.Length > 0).IsIn(whiteList).Matches(reg2)
				.ThrowOnError(t => st = t);
		}

		/// <summary>
		/// Tests for the ValidationException class.
		/// </summary>
		[TestMethod]
		public void ValidationExceptionTests()
		{
			ValidationException ve = new ValidationException();
			Assert.IsNotNull(ve.Message, "Message should not be null.");
			Assert.IsNull(ve.InnerException, "InnerException should be null");

			ValidationError verr1 = new ValidationError("MethodName1", "Message1");
			ve = new ValidationException(verr1);
			Assert.IsTrue(ve.Error.MethodName == "MethodName1", "MethodName not equal to MethodName1");
			Assert.IsTrue(ve.Message == "Message1", "Message not equal to Message");
			Assert.IsNull(ve.InnerException, "InnerException should be null");

			ValidationError verr2 = new ValidationError("MethodName2", "Message2");
			ValidationException ve2 = new ValidationException(verr1, verr2);
		}
	}
}
