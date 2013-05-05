using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Conversions;


namespace SharedAssemblies.Core.UnitTests
{
	/// <summary>
	/// Test fixture for translator
	/// </summary>
	[TestClass]
	public class EnumTranslatorTest
	{
		/// <summary>
		/// Sample translator reference
		/// </summary>
		private EnumTranslator<TestEnum, string> _refTranslator;

		/// <summary>
		/// Sample translator value
		/// </summary>
		private EnumTranslator<TestEnum, int> _valTranslator;

		/// <summary>
		/// Sample translator for reversing to EnumTranslator
		/// </summary>
		private Translator<int, TestEnum> _otherTranslator;

		/// <summary>
		/// Sample translator for reversing to EnumTranslator
		/// </summary>
		private Translator<int, TestEnum> _duplicateTranslator;

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		/// construct the tests
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
			_otherTranslator = new Translator<int, TestEnum>
			                   	{
			                   		{ 1, TestEnum.One },
			                   		{ 2, TestEnum.Two },
			                   		{ 3, TestEnum.Three }
			                   	};

			_refTranslator = new EnumTranslator<TestEnum, string>();
			_refTranslator[TestEnum.One] = "One";
			_refTranslator[TestEnum.Two] = "Two";
			_refTranslator[TestEnum.Three] = "Three";

			_valTranslator = new EnumTranslator<TestEnum, int>();
			_valTranslator.Add(TestEnum.One, 1);
			_valTranslator.Add(TestEnum.Two, 2);
			_valTranslator.Add(TestEnum.Three, 3);

			_duplicateTranslator = new Translator<int, TestEnum>
			                       	{
			                       		{ 0, TestEnum.Unknown },
			                       		{ 1, TestEnum.One },
			                       		{ 2, TestEnum.Two },
			                       		{ 3, TestEnum.Three },
			                       		{ 4, TestEnum.Unknown },
			                       	};
		}

		/// <summary>
		/// Clear should destroy all
		/// </summary>
		[TestMethod]
		public void TestClearNukesTranslations()
		{
			// test the reference lookup
			var translator = EnumTranslator.Create(_refTranslator, "HI");

			Assert.AreEqual(3, translator.TranslationCount);
			Assert.AreEqual("HI", translator.DefaultValue);

			translator.Clear();

			Assert.AreEqual(0, translator.TranslationCount);
			Assert.AreEqual("HI", translator.DefaultValue);

			// test the value lookup
			var valTranslator = EnumTranslator.Create(_valTranslator, -999);

			Assert.AreEqual(3, valTranslator.TranslationCount);
			Assert.AreEqual(-999, valTranslator.DefaultValue);

			valTranslator.Clear();

			Assert.AreEqual(0, valTranslator.TranslationCount);
			Assert.AreEqual(-999, valTranslator.DefaultValue);
		}

		/// <summary>
		/// test lookup success
		/// </summary>
		[TestMethod]
		public void TestLookupSuccess()
		{
			Assert.AreEqual("One", _refTranslator.Translate(TestEnum.One));
			Assert.AreEqual("Two", _refTranslator.Translate(TestEnum.Two));
			Assert.AreEqual("Three", _refTranslator.Translate(TestEnum.Three));

			Assert.AreEqual(1, _valTranslator.Translate(TestEnum.One));
			Assert.AreEqual(2, _valTranslator.Translate(TestEnum.Two));
			Assert.AreEqual(3, _valTranslator.Translate(TestEnum.Three));
		}

		/// <summary>
		/// Test lookup with failure without defaults
		/// </summary>
		[TestMethod]
		public void TestLookupFailureWithoutDefaults()
		{
			// default for ref types are null
			Assert.AreEqual((object)null, _refTranslator.Translate(TestEnum.Unknown));

			// default for val types are zero
			Assert.AreEqual(0, _valTranslator.Translate(TestEnum.Unknown));
		}

		/// <summary>
		/// Test lookup fails with indexers without defaults
		/// </summary>
		[TestMethod]
		public void TestLookupFailureWithIndexersWithoutDefaults()
		{
			// default for ref types are null
			Assert.AreEqual((object)null, _refTranslator[TestEnum.Unknown]);
			Assert.AreEqual((object)null, _refTranslator[(TestEnum)(-9)]);

			// default for val types are zero
			Assert.AreEqual(0, _valTranslator[TestEnum.Unknown]);
			Assert.AreEqual(0, _valTranslator[(TestEnum)(-9)]);
		}

		/// <summary>
		/// Test lookup fails with defaults
		/// </summary>
		[TestMethod]
		public void TestLookupFailureWithDefaults()
		{
			// re-establish initial setup w/o defaults
			var refTranslator = EnumTranslator.Create(_refTranslator, "OOOPS");
			var valTranslator = EnumTranslator.Create(_valTranslator, -1);

			// default for ref types are null
			Assert.AreEqual("OOOPS", refTranslator.Translate(TestEnum.Unknown));
			Assert.AreEqual("OOOPS", refTranslator.Translate((TestEnum)(-9)));

			// default for val types are zero
			Assert.AreEqual(-1, valTranslator.Translate(TestEnum.Unknown));
			Assert.AreEqual(-1, valTranslator.Translate((TestEnum)(-9)));
		}

		/// <summary>
		/// Test lookup success with indexers
		/// </summary>
		[TestMethod]
		public void TestLookupSuccessWithIndexers()
		{
			Assert.AreEqual("One", _refTranslator[TestEnum.One]);
			Assert.AreEqual("Two", _refTranslator[TestEnum.Two]);
			Assert.AreEqual("Three", _refTranslator[TestEnum.Three]);

			Assert.AreEqual(1, _valTranslator[TestEnum.One]);
			Assert.AreEqual(2, _valTranslator[TestEnum.Two]);
			Assert.AreEqual(3, _valTranslator[TestEnum.Three]);
		}

		/// <summary>
		/// Test lookup fail with indexers with defaults
		/// </summary>
		[TestMethod]
		public void TestLookupFailureWithIndexersWithDefaults()
		{
			var refTranslator = EnumTranslator.Create(_refTranslator, "OOOPS");
			var valTranslator = EnumTranslator.Create(_valTranslator, -1);

			// default for ref types are null
			Assert.AreEqual("OOOPS", refTranslator[TestEnum.Unknown]);

			// default for val types are zero
			Assert.AreEqual(-1, valTranslator[TestEnum.Unknown]);
		}

		/// <summary>
		/// Test indexer to add translations
		/// </summary>
		[TestMethod]
		public void TestIndexerToAddTranslation()
		{
			_refTranslator[TestEnum.Unknown] = "Unknown";
			_valTranslator[TestEnum.Unknown] = -999;

			Assert.AreEqual("Unknown", _refTranslator[TestEnum.Unknown]);
			Assert.AreEqual(-999, _valTranslator[TestEnum.Unknown]);
		}

		/// <summary>
		/// Test reverse translate works
		/// </summary>
		[TestMethod]
		public void TestReverseTranslateSuccess()
		{
			var refTranslator = Translator.Reverse(_refTranslator);
			Assert.AreEqual(TestEnum.One, refTranslator.Translate("One"));
			Assert.AreEqual(TestEnum.Two, refTranslator.Translate("Two"));
			Assert.AreEqual(TestEnum.Three, refTranslator.Translate("Three"));

			var valTranslator = Translator.Reverse(_valTranslator);
			Assert.AreEqual(TestEnum.One, valTranslator.Translate(1));
			Assert.AreEqual(TestEnum.Two, valTranslator.Translate(2));
			Assert.AreEqual(TestEnum.Three, valTranslator.Translate(3));
		}

		/// <summary>
		/// Test reverse translate works
		/// </summary>
		[TestMethod]
		public void TestReverseEnumTranslatorWithNoDefault()
		{
			var refTranslator = EnumTranslator.Reverse(_otherTranslator);
			Assert.AreEqual(1, refTranslator[TestEnum.One]);
			Assert.AreEqual(2, refTranslator[TestEnum.Two]);
			Assert.AreEqual(3, refTranslator[TestEnum.Three]);
			Assert.AreEqual(0, refTranslator[TestEnum.Unknown]);
		}

		/// <summary>
		/// Test reverse translate works
		/// </summary>
		[TestMethod]
		public void TestReverseEnumTranslatorWithDefault()
		{
			var refTranslator = EnumTranslator.Reverse(_otherTranslator, -999);
			Assert.AreEqual(1, refTranslator[TestEnum.One]);
			Assert.AreEqual(2, refTranslator[TestEnum.Two]);
			Assert.AreEqual(3, refTranslator[TestEnum.Three]);
			Assert.AreEqual(-999, refTranslator[TestEnum.Unknown]);
		}

		/// <summary>
		/// Test reverse translation failures without defaults
		/// </summary>
		[TestMethod]
		public void TestReverseTranslateFailureWithoutDefaults()
		{
			var refTranslator = Translator.Reverse(_refTranslator);
			var valTranslator = Translator.Reverse(_valTranslator);

			Assert.AreEqual((TestEnum)0, refTranslator.Translate("OOOPS"));
			Assert.AreEqual((TestEnum)0, valTranslator.Translate(-999));
		}

		/// <summary>
		/// Test reverse translation failures with defaults
		/// </summary>
		[TestMethod]
		public void TestReverseTranslateFailureWithDefaults()
		{
			var refTranslator = Translator.Reverse(_refTranslator, TestEnum.Unknown);
			var valTranslator = Translator.Reverse(_valTranslator, TestEnum.Unknown);

			Assert.AreEqual(TestEnum.Unknown, refTranslator.Translate("OOOPS"));
			Assert.AreEqual(TestEnum.Unknown, valTranslator.Translate(-999));
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorThrowsOnNonIntEnum()
		{
			new EnumTranslator<TestLongEnum, int>();
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorThrowsOnFlagsEnum()
		{
			new EnumTranslator<TestFlagsEnum, int>();
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorThrowsOnNonEnum()
		{
			new EnumTranslator<long, int>();
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorReverseThrowsOnNonIntEnum()
		{
			var translator = new Translator<int, TestLongEnum>();
			EnumTranslator.Reverse(translator);
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorReverseThrowsOnDuplicates()
		{
			EnumTranslator.Reverse(_duplicateTranslator);
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorReverseThrowsOnFlagsEnum()
		{
			var translator = new Translator<int, TestFlagsEnum>();
			EnumTranslator.Reverse(translator);
		}

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestEnumTranslatorReverseThrowsOnNonEnum()
		{
			var translator = new Translator<long, int>();
			EnumTranslator.Reverse(translator);
		}

		/// <summary>
		/// A testing enumeration
		/// </summary>
		private enum TestEnum
		{
			/// <summary>
			/// One, singular sensation
			/// </summary>
			One,

			/// <summary>
			/// Two is the loneliest number since the number one
			/// </summary>
			Two,

			/// <summary>
			/// Three is a magic number
			/// </summary>
			Three,

			/// <summary>
			/// To boldly go where no one has gone before
			/// </summary>
			Unknown
		}

		/// <summary>
		/// A testing enumeration
		/// </summary>
		private enum TestLongEnum : long
		{
			/// <summary>
			/// One, singular sensation
			/// </summary>
			One,

			/// <summary>
			/// Two is the loneliest number since the number one
			/// </summary>
			Two,

			/// <summary>
			/// Three is a magic number
			/// </summary>
			Three,

			/// <summary>
			/// To boldly go where no one has gone before
			/// </summary>
			Unknown
		}

		/// <summary>
		/// A testing enumeration
		/// </summary>
		[Flags]
		private enum TestFlagsEnum : int
		{
			/// <summary>
			/// One, singular sensation
			/// </summary>
			One,

			/// <summary>
			/// Two is the loneliest number since the number one
			/// </summary>
			Two,

			/// <summary>
			/// Three is a magic number
			/// </summary>
			Three,

			/// <summary>
			/// To boldly go where no one has gone before
			/// </summary>
			Unknown
		}
	}
}