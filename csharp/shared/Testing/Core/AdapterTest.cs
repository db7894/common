using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedAssemblies.Core.Patterns;
using SharedAssemblies.Core.UnitTests.TestClasses;


namespace SharedAssemblies.Core.UnitTests
{
    /// <summary>
    /// Test fixture for the adapter generic class
    /// </summary>
    [TestClass]
    public class AdapterTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Test that constructor gives non-null list
        /// </summary>
        [TestMethod]
        public void Constructor_TranslationsListNotNull_OnConstruction()
        {
            var actual = new Adapter<Point, Pair>();

            Assert.IsNotNull(actual.Translations);
        }

        
        /// <summary>
        /// Test that constructor has no translations
        /// </summary>
        [TestMethod]
        public void Constructor_YieldsNoTranslations_OnConstruction()
        {
            var actual = new Adapter<Point, Pair>();

            Assert.AreEqual(0, actual.Translations.Count);
        }


        /// <summary>
        /// Test constructor will add translations with initializers
        /// </summary>
        [TestMethod]
        public void Constructor_AddsTranslations_WithInitializer()
        {
            var actual = new Adapter<Point, Pair>
                             {
                                 (from, to) => to.First = from.X.ToString(),
                                 (from, to) => to.Second = from.Y.ToString()
                             };

            Assert.AreEqual(2, actual.Translations.Count);
        }


        /// <summary>
        /// Test add adds a translation
        /// </summary>
        [TestMethod]
        public void Add_AddsTranslation_OnCall()
        {
            var actual = new Adapter<Point, Pair>();

            actual.Add((from, to) => to.First = from.X.ToString());

            Assert.AreEqual(1, actual.Translations.Count);            
        }

        
        /// <summary>
        /// Test that add adds a translation with predicate
        /// </summary>
        [TestMethod]
        public void Add_AddsTranslationWithPredicate_OnCall()
        {
            var actual = new Adapter<Point, Pair>();

            actual.Add(to => true, (from, to) => to.First = from.X.ToString());

            Assert.AreEqual(1, actual.Translations.Count);
        }


        /// <summary>
        /// Test that adapt calls translators
        /// </summary>
        [TestMethod]
        public void Adapt_CallsTranslators_OnCall()
        {
            var target = new Point { X = 6, Y = 3 };

            var actual = new Adapter<Point, Pair>
                             {
                                 (from, to) => to.First = from.X.ToString(),
                                 (from, to) => to.Second = from.Y.ToString()
                             };

            var result = actual.Adapt(target);

            Assert.AreEqual("6", result.First);
            Assert.AreEqual("3", result.Second);
        }

        
        /// <summary>
        /// Test that adapt calls predicates
        /// </summary>
        [TestMethod]
        public void Adapt_CallsPredicateTranslators_OnCallIfTrue()
        {
            var target = new Point { X = 6, Y = 3 };

            var actual = new Adapter<Point, Pair>
                             {
                                 { from => true, (from, to) => to.First = "Yes X" },
                                 { from => false, (from, to) => to.First = "No X" },
                                 { from => true, (from, to) => to.Second = "Yes Y" },
                                 { from => false, (from, to) => to.Second  = "No Y" },
                             };

            var result = actual.Adapt(target);

            Assert.AreEqual("Yes X", result.First);
            Assert.AreEqual("Yes Y", result.Second);
        }
    }
}
