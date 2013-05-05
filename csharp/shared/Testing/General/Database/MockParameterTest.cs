using System.Data;
using SharedAssemblies.General.Database.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for MockParameterTest and is intended
    /// to contain all MockParameterTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockParameterTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for Value
        /// </summary>
        [TestMethod]
        public void ValueTest()
        {
            string expectedValue = "test";
            MockParameter target = new MockParameter();
            target.Value = expectedValue;

            Assert.AreEqual(expectedValue, target.Value);
        }


        /// <summary>
        /// A test for SourceVersion
        /// </summary>
        [TestMethod]
        public void SourceVersionTest()
        {
            MockParameter target = new MockParameter();
            DataRowVersion expected = DataRowVersion.Original;
            target.SourceVersion = expected;

            Assert.AreEqual(expected, target.SourceVersion);
        }


        /// <summary>
        /// A test for SourceColumnNullMapping
        /// </summary>
        [TestMethod]
        public void SourceColumnNullMappingTest()
        {
            MockParameter target = new MockParameter();
            target.SourceColumnNullMapping = false;
            Assert.IsFalse(target.SourceColumnNullMapping);
            target.SourceColumnNullMapping = true;
            Assert.IsTrue(target.SourceColumnNullMapping);
        }


        /// <summary>
        /// A test for SourceColumn
        /// </summary>
        [TestMethod]
        public void SourceColumnTest()
        {
            MockParameter target = new MockParameter();
            string expected = "ColumnName";

            target.SourceColumn = expected;
            Assert.AreEqual(expected, target.SourceColumn);
        }


        /// <summary>
        /// A test for Size
        /// </summary>
        [TestMethod]
        public void SizeTest()
        {
            MockParameter target = new MockParameter();
            int expected = 42;

            target.Size = expected;
            Assert.AreEqual(expected, target.Size);
        }


        /// <summary>
        /// A test for ParameterName
        /// </summary>
        [TestMethod]
        public void ParameterNameTest()
        {
            MockParameter target = new MockParameter();
            string expected = "ParameterName";

            target.ParameterName = expected;
            Assert.AreEqual(expected, target.ParameterName);
        }


        /// <summary>
        /// A test for IsNullable
        /// </summary>
        [TestMethod]
        public void IsNullableTest()
        {
            MockParameter target = new MockParameter();
            Assert.IsFalse(target.IsNullable);
            target.IsNullable = true;
            Assert.IsTrue(target.IsNullable);
        }


        /// <summary>
        /// A test for Direction
        /// </summary>
        [TestMethod]
        public void DirectionTest()
        {
            MockParameter target = new MockParameter();
            ParameterDirection expected = ParameterDirection.Output;

            target.Direction = expected;
            Assert.AreEqual(expected, target.Direction);
        }


        /// <summary>
        /// A test for DbType
        /// </summary>
        [TestMethod]
        public void DbTypeTest()
        {
            MockParameter target = new MockParameter();
            DbType expected = DbType.Boolean;

            target.DbType = expected;
            Assert.AreEqual(expected, target.DbType);
        }


        /// <summary>
        /// A test for ResetDbType
        /// </summary>
        [TestMethod]
        public void ResetDbTypeTest()
        {
            MockParameter target = new MockParameter();
            target.ResetDbType();
            Assert.AreEqual(string.Empty, target.SourceColumn);
        }


        /// <summary>
        /// A test for MockParameter Constructor
        /// </summary>
        [TestMethod]
        public void MockParameterConstructorTest()
        {
            MockParameter target = new MockParameter();
            Assert.AreEqual(DbType.String, target.DbType);
            Assert.AreEqual(ParameterDirection.Input, target.Direction);
            Assert.IsFalse(target.IsNullable);
            Assert.AreEqual(string.Empty, target.ParameterName);
            Assert.AreEqual(0, target.Size);
            Assert.AreEqual(string.Empty, target.SourceColumn);
            Assert.IsFalse(target.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, target.SourceVersion);
            Assert.AreEqual(null, target.Value);
        }


        /// <summary>
        /// A test for MockParameter Constructor
        /// </summary>
        [TestMethod]
        public void MockParameterConsistentConstructorTest()
        {
            MockParameter target = new MockParameter();
            System.Data.SqlClient.SqlParameter actual = new System.Data.SqlClient.SqlParameter();

            Assert.AreEqual(actual.DbType, target.DbType);
            Assert.AreEqual(actual.Direction, target.Direction);
            Assert.AreEqual(actual.IsNullable, target.IsNullable);
            Assert.AreEqual(actual.ParameterName, target.ParameterName);
            Assert.AreEqual(actual.Size, target.Size);
            Assert.AreEqual(actual.SourceColumn, target.SourceColumn);
            Assert.AreEqual(actual.SourceColumnNullMapping, target.SourceColumnNullMapping);
            Assert.AreEqual(actual.SourceVersion, target.SourceVersion);
            Assert.AreEqual(actual.Value, target.Value);
        }
    }
}
