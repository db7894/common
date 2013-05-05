using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Conversions;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Test fixture for translator
    /// </summary>
    [TestClass]
    public class TranslatorTest
    {
        /// <summary>
        /// Sample translator reference
        /// </summary>
        private Translator<TestEnum, string> _refTranslator;

        /// <summary>
        /// Sample translator value
        /// </summary>
        private Translator<TestEnum, int> _valTranslator;

        /// <summary>
        /// Sample translator nullable
        /// </summary>
        private Translator<string, TestEnum?> _nullTranslator;

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
			_refTranslator = new Translator<TestEnum, string>();
			_refTranslator[TestEnum.One] = "One";
            _refTranslator[TestEnum.Two] = "Two";
            _refTranslator[TestEnum.Three] = "Three";

			_valTranslator = new Translator<TestEnum, int>();
			_valTranslator.Add(TestEnum.One, 1);
            _valTranslator.Add(TestEnum.Two, 2);
            _valTranslator.Add(TestEnum.Three, 3);

			_nullTranslator = new Translator<string, TestEnum?>();
			_nullTranslator.Add(new[]
                 {
                     new KeyValuePair<string, TestEnum?>("One", TestEnum.One),
                     new KeyValuePair<string, TestEnum?>("Two", TestEnum.Two),
                     new KeyValuePair<string, TestEnum?>("Three", TestEnum.Three)
                 });

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
        	var translator = Translator.Create(_refTranslator, "HI");

			Assert.AreEqual(3, translator.TranslationCount);
			Assert.AreEqual("HI", translator.DefaultValue);

			translator.Clear();

			Assert.AreEqual(0, translator.TranslationCount);
			Assert.AreEqual("HI", translator.DefaultValue);

            // test the value lookup
            var valTranslator = Translator.Create(_valTranslator, -999);

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
            Assert.AreEqual((object) null, _refTranslator.Translate(TestEnum.Unknown));

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
            Assert.AreEqual((object) null, _refTranslator[TestEnum.Unknown]);

            // default for val types are zero
            Assert.AreEqual(0, _valTranslator[TestEnum.Unknown]);
        }

        /// <summary>
        /// Test lookup fails with defaults
        /// </summary>
        [TestMethod]
        public void TestLookupFailureWithDefaults()
        {
            // re-establish initial setup w/o defaults
        	var refTranslator = Translator.Create(_refTranslator, "OOOPS");
        	var valTranslator = Translator.Create(_valTranslator, -1);

            // default for ref types are null
            Assert.AreEqual("OOOPS", refTranslator.Translate(TestEnum.Unknown));

            // default for val types are zero
            Assert.AreEqual(-1, valTranslator.Translate(TestEnum.Unknown));
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
            var refTranslator = Translator.Create(_refTranslator, "OOOPS");
            var valTranslator = Translator.Create(_valTranslator, -1);

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
        /// Test nullable to types
        /// </summary>
        [TestMethod]
        public void TestNullableToType()
        {
            // w/o defaults should return null for a nullable type
            Assert.AreEqual<TestEnum>(TestEnum.One, _nullTranslator["One"].Value);
            Assert.AreEqual<TestEnum>(TestEnum.Two, _nullTranslator["Two"].Value);
            Assert.AreEqual<TestEnum>(TestEnum.Three, _nullTranslator["Three"].Value);
            Assert.AreEqual(false, _nullTranslator["FOUR"].HasValue);
            Assert.AreEqual((object)null, _nullTranslator["FOUR"]);

            // now ad defaults
            var newDefault = Translator.Create(_nullTranslator, new TestEnum?());

            // w/o defaults should return null for a nullable type
			Assert.AreEqual<TestEnum>(TestEnum.One, newDefault["One"].Value);
			Assert.AreEqual<TestEnum>(TestEnum.Two, newDefault["Two"].Value);
			Assert.AreEqual<TestEnum>(TestEnum.Three, newDefault["Three"].Value);
			Assert.AreEqual(false, newDefault["FOUR"].HasValue);
			Assert.AreEqual((object)null, newDefault["FOUR"]);
        }

        /// <summary>
        /// Test nullable from types
        /// </summary>
        [TestMethod]
        public void TestNullableFromType()
        {
			var nullTranslator = Translator.Reverse(_nullTranslator);

			// w/o defaults should return null for a nullable type
            Assert.AreEqual("One", nullTranslator.Translate(TestEnum.One));
            Assert.AreEqual("Two", nullTranslator.Translate(TestEnum.Two));
            Assert.AreEqual("Three", nullTranslator.Translate(TestEnum.Three));
            Assert.AreEqual((object)null, nullTranslator.Translate(null));
            Assert.AreEqual((object)null, nullTranslator.Translate(new TestEnum?()));

            // now ad defaults
			nullTranslator = Translator.Reverse(_nullTranslator, "OOOPS");

            // w/o defaults should return null for a nullable type
            Assert.AreEqual("One", nullTranslator.Translate(TestEnum.One));
            Assert.AreEqual("Two", nullTranslator.Translate(TestEnum.Two));
            Assert.AreEqual("Three", nullTranslator.Translate(TestEnum.Three));
            Assert.AreEqual("OOOPS", nullTranslator.Translate(null));
            Assert.AreEqual("OOOPS", nullTranslator.Translate(new TestEnum?()));
        }

		/// <summary>
		/// Test for invalid enums.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestEnumTranslatorReverseThrowsOnDuplicates()
		{
			Translator.Reverse(_duplicateTranslator);
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
	}
}