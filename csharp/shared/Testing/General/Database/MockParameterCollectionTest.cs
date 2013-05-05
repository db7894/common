using System.Data.SqlClient;
using System.Collections;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
    /// This is a test class for MockParameterCollectionTest and is intended
    /// to contain all MockParameterCollectionTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockParameterCollectionTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for SyncRoot
        /// </summary>
        [TestMethod]
        public void SyncRootTest()
        {
            MockParameterCollection target = new MockParameterCollection();
            Assert.IsNotNull(target.SyncRoot);
        }


        /// <summary>
        /// A test for IsSynchronized
        /// </summary>
        [TestMethod]
        public void IsSynchronizedTest()
        {
            MockParameterCollection target = new MockParameterCollection();
            Assert.IsFalse(target.IsSynchronized);
        }


        /// <summary>
        /// A test for IsReadOnly
        /// </summary>
        [TestMethod]
        public void IsReadOnlyTest()
        {
            MockParameterCollection target = new MockParameterCollection();
            Assert.IsFalse(target.IsReadOnly);
        }


        /// <summary>
        /// A test for IsFixedSize
        /// </summary>
        [TestMethod]
        public void IsFixedSizeTest()
        {
            MockParameterCollection target = new MockParameterCollection();
            Assert.IsFalse(target.IsFixedSize);
        }


        /// <summary>
        /// A test for Count
        /// </summary>
        [TestMethod]
        public void CountTest()
        {
            MockParameterCollection target = new MockParameterCollection();

            Assert.AreEqual(0, target.Count);

            target.Add(new MockParameter { ParameterName = "One" });
            target.Add(new MockParameter { ParameterName = "Two" });
            target.Add(new MockParameter { ParameterName = "Three" });

            Assert.AreEqual(3, target.Count);
        }


        /// <summary>
        /// AddRange adds all parameters
        /// </summary>
        [TestMethod]
        public void AddRange_AddsAll_OnCall()
        {
            MockParameter[] parms = new[]
                                        {
                                            new MockParameter { ParameterName = "One" },
                                            new MockParameter { ParameterName = "Two" },
                                            new MockParameter { ParameterName = "Three" },
                                        };
            MockParameterCollection target = new MockParameterCollection();

            target.AddRange(parms);

            Assert.AreEqual(3, target.Count);
        }


        /// <summary>
        /// AddRange adds nothing when called with emtyp collection
        /// </summary>
        [TestMethod]
        public void AddRange_AddsNothing_OnCallWithEmpty()
        {
            MockParameter[] parms = new MockParameter[0];
            MockParameterCollection target = new MockParameterCollection();

            target.AddRange(parms);

            Assert.AreEqual(0, target.Count);
        }


        /// <summary>
        /// AddRange Throws with bad arguments
        /// </summary>
        [TestMethod]
        public void AddRange_Throws_OnBadArgument()
        {
            double[] parms = new[] { 1.0, 2.0, 3.0 };

            MockParameterCollection target = new MockParameterCollection();

			AssertEx.Throws(() => target.AddRange(parms));
        }


        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [TestMethod]
        public void RemoveAtByIndexTest()
        {
            MockParameterCollection target = new MockParameterCollection
                                                 {
                                                     new MockParameter { ParameterName = "One" },
                                                     new MockParameter { ParameterName = "Two" },
                                                     new MockParameter { ParameterName = "Three" }
                                                 };

            target.RemoveAt(0);

            Assert.AreEqual(2, target.Count);
            Assert.AreEqual(target["Two"], target[0]);
        }


        /// <summary>
        /// A test for RemoveAt
        /// </summary>
        [TestMethod]
        public void RemoveAtTest()
        {
            MockParameterCollection target = new MockParameterCollection
                                                 {
                                                     new MockParameter { ParameterName = "One" },
                                                     new MockParameter { ParameterName = "Two" },
                                                     new MockParameter { ParameterName = "Three" }
                                                 };

            target.RemoveAt("One");

            Assert.AreEqual(2, target.Count);
            Assert.AreEqual("Two", target[0].ParameterName);
        }


        /// <summary>
        /// A test for Remove
        /// </summary>
        [TestMethod]
        public void RemoveTest()
        {
            MockParameterCollection target = new MockParameterCollection
                                                 {
                                                     new MockParameter { ParameterName = "One" },
                                                     new MockParameter { ParameterName = "Two" },
                                                     new MockParameter { ParameterName = "Three" }
                                                 };

            MockParameter removeMe = new MockParameter { ParameterName = "Four" };
            target.Add(removeMe);

            Assert.AreEqual(4, target.Count);

            target.Remove(removeMe);

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(-1, target.IndexOf(removeMe));
        }


        /// <summary>
        /// A test for Insert
        /// </summary>
        [TestMethod]
        public void InsertTest()
        {
            MockParameterCollection target = new MockParameterCollection
                                                 {
                                                     new MockParameter { ParameterName = "One" },
                                                     new MockParameter { ParameterName = "Two" },
                                                     new MockParameter { ParameterName = "Three" }
                                                 };

            target.Insert(0, new MockParameter { ParameterName = "Four" });

            Assert.AreEqual(4, target.Count);
            Assert.AreEqual("Four", target[0].ParameterName);
        }


        /// <summary>
        /// A test for IndexOf
        /// </summary>
        [TestMethod]
        public void IndexOfByNameTest()
        {
            MockParameterCollection target = new MockParameterCollection
                                                 {
                                                     new MockParameter { ParameterName = "One" },
                                                     new MockParameter { ParameterName = "Two" },
                                                     new MockParameter { ParameterName = "Three" }
                                                 };

            Assert.AreEqual(0, target.IndexOf("One"));
            Assert.AreEqual(1, target.IndexOf("Two"));
            Assert.AreEqual(2, target.IndexOf("Three"));
        }


        /// <summary>
        /// A test for IndexOf
        /// </summary>
        [TestMethod]
        public void IndexOfTest()
        {
            MockParameterCollection target = new MockParameterCollection();
            MockParameter parm1 = new MockParameter { ParameterName = "One" };
            MockParameter parm2 = new MockParameter { ParameterName = "Two" };
            MockParameter parm3 = new MockParameter { ParameterName = "Three" };

            target.Add(parm1);
            target.Add(parm2);
            target.Add(parm3);

            Assert.AreEqual(0, target.IndexOf(parm1));
            Assert.AreEqual(1, target.IndexOf(parm2));
            Assert.AreEqual(2, target.IndexOf(parm3));
        }


        /// <summary>
        /// A test for GetEnumerator
        /// </summary>
        [TestMethod]
        public void GetEnumeratorTest()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            target.AddRange(expected);

            IEnumerator enumerator = target.GetEnumerator();

            foreach (MockParameter parm in expected)
            {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreEqual(parm, enumerator.Current);
            }
        }


        /// <summary>
        /// A test for CopyTo
        /// </summary>
        [TestMethod]
        public void CopyToTest()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            target.AddRange(expected);

            MockParameter[] actual = new MockParameter[3];
            target.CopyTo(actual, 0);

            for (int i = 0; i < 3; ++i)
            {
                Assert.AreEqual(target[i], actual[i]);
            }
        }


        /// <summary>
        /// A test for Contains
        /// </summary>
        [TestMethod]
        public void ContainsTest()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            target.AddRange(expected);

            Assert.IsTrue(target.Contains("Two"));
            Assert.IsFalse(target.Contains("Four"));
        }


        /// <summary>
        /// A test for Contains
        /// </summary>
        [TestMethod]
        public void Contains_ReturnsFalse_IfNotFound()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            MockParameter notFound = new MockParameter { ParameterName = "Nope" };

            target.AddRange(expected);

            Assert.IsFalse(target.Contains(notFound));
        }


        /// <summary>
        /// A test for Contains
        /// </summary>
        [TestMethod]
        public void SetParameter_Replaces_IfFound()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            MockParameter newOne = new MockParameter { ParameterName = "Alpha", Value = 3.14 };

            target.AddRange(expected);
            target[0] = newOne;

            Assert.IsTrue(target.Contains(3.14));
        }

        /// <summary>
        /// A test for Contains
        /// </summary>
        [TestMethod]
        public void GetParameter_ReturnsNull_IfNotFoundByName()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            target.AddRange(expected);
            Assert.IsNull(target["Beta"]);
        }


        /// <summary>
        /// A test for Contains
        /// </summary>
        [TestMethod]
        public void SetParameter_Replaces_IfFoundByName()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            MockParameter newOne = new MockParameter { ParameterName = "Alpha" };

            target.AddRange(expected);
            target["Two"] = newOne;

            Assert.IsTrue(target.Contains(newOne.ParameterName));
        }


        /// <summary>
        /// A test for Contains
        /// </summary>
        [TestMethod]
        public void ContainsByValueTest()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One", Value = 7 },
                                             new MockParameter { ParameterName = "Two", Value = 3 },
                                             new MockParameter { ParameterName = "Three", Value = 5 }
                                         };

            target.AddRange(expected);

            Assert.IsTrue(target.Contains(7));
            Assert.IsFalse(target.Contains("Four"));
        }


        /// <summary>
        /// A test for Clear
        /// </summary>
        [TestMethod]
        public void ClearTest()
        {
            MockParameterCollection target = new MockParameterCollection();

            MockParameter[] expected = new[]
                                         {
                                             new MockParameter { ParameterName = "One" },
                                             new MockParameter { ParameterName = "Two" },
                                             new MockParameter { ParameterName = "Three" }
                                         };

            target.AddRange(expected);

            Assert.AreEqual(3, target.Count);

            target.Clear();

            Assert.AreEqual(0, target.Count);
        }


        /// <summary>
        /// A test for consistent construction with sql artifacts
        /// </summary>
        [TestMethod]
        public void ConsistentConstructionTest()
        {
            SqlParameterCollection actual = new SqlCommand().Parameters;
            MockParameterCollection target = new MockParameterCollection();

            Assert.AreEqual(actual.Count, target.Count);
            Assert.AreEqual(actual.IsFixedSize, target.IsFixedSize);
            Assert.AreEqual(actual.IsReadOnly, target.IsReadOnly);
            Assert.AreEqual(actual.IsSynchronized, target.IsSynchronized);
        }
    }
}
