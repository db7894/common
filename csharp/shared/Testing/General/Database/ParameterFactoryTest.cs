using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using SharedAssemblies.General.Database.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for ParameterFactoryTest and is intended
    /// to contain all ParameterFactoryTest Unit Tests
    /// </summary>
    [TestClass]
    public class ParameterFactoryTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for ProviderFactory
        /// </summary>
        [TestMethod]
        public void ProviderFactoryByTypeTest()
        {
            // only need to test the one since we know from unit tests of
            // ClientProviderFactory that it can do all the others
            ParameterFactory target = new ParameterFactory(ClientProviderType.SqlServer);

            Assert.AreEqual(SqlClientFactory.Instance, target.ProviderFactory);
        }


        /// <summary>
        /// A test for ProviderFactory
        /// </summary>
        [TestMethod]
        public void ProviderFactoryTest()
        {
            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);

            Assert.AreEqual(SqlClientFactory.Instance, target.ProviderFactory);
        }


        /// <summary>
        /// A test for CreateSet
        /// </summary>
        [TestMethod]
        public void CreateSetTest()
        {
            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);

            ParameterSet parms = target.CreateSet();

            Assert.AreEqual(target.ProviderFactory, parms.ProviderFactory);
        }


        /// <summary>
        /// A test for Create
        /// </summary>
        [TestMethod]
        public void CreateEmptyTest()
        {
            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);
            DbParameter parm = target.Create();

            // check for assigned and default values
            Assert.IsInstanceOfType(parm, typeof(SqlParameter));
            Assert.AreEqual(string.Empty, parm.ParameterName);
            Assert.AreEqual(DbType.String, parm.DbType);
            Assert.AreEqual(ParameterDirection.Input, parm.Direction);
            Assert.IsFalse(parm.IsNullable);
            Assert.AreEqual(0, parm.Size);
            Assert.AreEqual(null, parm.Value);
            Assert.AreEqual(string.Empty, parm.SourceColumn);
            Assert.AreEqual(false, parm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, parm.SourceVersion);
        }


        /// <summary>
        /// A test for Create
        /// </summary>
        [TestMethod]
        public void CreateWithNameAndTypeTest()
        {
            string expectedName = "SomeParm";
            DbType expectedType = DbType.Double;

            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);
            DbParameter parm = target.Create(expectedName, expectedType);

            // check for assigned and default values
            Assert.IsInstanceOfType(parm, typeof(SqlParameter));
            Assert.AreEqual(expectedName, parm.ParameterName);
            Assert.AreEqual(expectedType, parm.DbType);
            Assert.AreEqual(ParameterDirection.Input, parm.Direction);
            Assert.IsFalse(parm.IsNullable);
            Assert.AreEqual(0, parm.Size);
            Assert.AreEqual(null, parm.Value);
            Assert.AreEqual(string.Empty, parm.SourceColumn);
            Assert.AreEqual(false, parm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, parm.SourceVersion);
        }


        /// <summary>
        /// A test for Create
        /// </summary>
        [TestMethod]
        public void CreateWithNameAndTypeAndValueTest()
        {
            string expectedName = "SomeParm";
            DbType expectedType = DbType.Double;
            double expectedValue = 3.1415927;

            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);
            DbParameter parm = target.Create(expectedName, expectedType, expectedValue);

            // check for assigned and default values
            Assert.IsInstanceOfType(parm, typeof(SqlParameter));
            Assert.AreEqual(expectedName, parm.ParameterName);
            Assert.AreEqual(expectedType, parm.DbType);
            Assert.AreEqual(ParameterDirection.Input, parm.Direction);
            Assert.IsFalse(parm.IsNullable);
            Assert.AreEqual(0, parm.Size);
            Assert.AreEqual(expectedValue, parm.Value);
            Assert.AreEqual(string.Empty, parm.SourceColumn);
            Assert.AreEqual(false, parm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, parm.SourceVersion);
        }


        /// <summary>
        /// A test for Create
        /// </summary>
        [TestMethod]
        public void CreateWithNameAndTypeAndDirectionTest()
        {
            string expectedName = "SomeParm";
            DbType expectedType = DbType.Double;
            ParameterDirection expectedDirection = ParameterDirection.Output;

            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);
            DbParameter parm = target.Create(expectedName, expectedType, expectedDirection);

            // check for assigned and default values
            Assert.IsInstanceOfType(parm, typeof(SqlParameter));
            Assert.AreEqual(expectedName, parm.ParameterName);
            Assert.AreEqual(expectedType, parm.DbType);
            Assert.AreEqual(expectedDirection, parm.Direction);
            Assert.IsFalse(parm.IsNullable);
            Assert.AreEqual(0, parm.Size);
            Assert.AreEqual(null, parm.Value);
            Assert.AreEqual(string.Empty, parm.SourceColumn);
            Assert.AreEqual(false, parm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, parm.SourceVersion);
        }




        /// <summary>
        /// A test to see if an integer type is correctly inferred for dbtype
        /// </summary>
        [TestMethod]
        public void Add_InfersIntegerType_WhenGivenIntegerValue()
        {
            DbType expectedType = DbType.Int32;

            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);
            var parm = target.Create("@hello", 17);

            Assert.AreEqual(parm.DbType, expectedType);
        }




        /// <summary>
        /// A test to see if an integer type is correctly inferred for dbtype
        /// </summary>
        [TestMethod]
        public void Add_InfersIntegerType_WhenGivenDataTableValue()
        {
            SqlDbType expectedType = SqlDbType.Structured;
            DbType expectedDbType = DbType.Object;

            ParameterFactory target = new ParameterFactory(SqlClientFactory.Instance);
            var parm = target.Create("@hello", new DataTable()) as SqlParameter;

            Assert.IsNotNull(parm);
            Assert.AreEqual(parm.SqlDbType, expectedType);
            Assert.AreEqual(parm.DbType, expectedDbType);
        }
    }
}
