using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
    /// <summary>
    /// This is a test class for ParameterSetTest and is intended
    /// to contain all ParameterSetTest Unit Tests
    /// </summary>
    [TestClass]
    public class ParameterSetTest
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
        public void ProviderFactoryTest()
        {
            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer);

            Assert.AreEqual(SqlClientFactory.Instance, target.ProviderFactory);
        }


        /// <summary>
        /// A test for ParameterFactory
        /// </summary>
        [TestMethod]
        public void ParameterFactoryTest()
        {
            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer);

            Assert.AreEqual(SqlClientFactory.Instance, target.ParameterFactory.ProviderFactory);
        }


        /// <summary>
        /// A test for Item
        /// </summary>
        [TestMethod]
        public void ItemByIndexTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String }
                                      };

            DbParameter expectedParm = target.Add(expectedName, expectedType);
            target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter actualParm = target[1];

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm, actualParm);
            Assert.AreEqual(expectedName, actualParm.ParameterName);
            Assert.AreEqual(expectedType, actualParm.DbType);
            Assert.AreEqual(ParameterDirection.Input, actualParm.Direction);
            Assert.IsFalse(actualParm.IsNullable);
            Assert.AreEqual(0, actualParm.Size);
            Assert.AreEqual(null, actualParm.Value);
            Assert.AreEqual(string.Empty, actualParm.SourceColumn);
            Assert.AreEqual(false, actualParm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, actualParm.SourceVersion);
        }


        /// <summary>
        /// A test for Item
        /// </summary>
        [TestMethod]
        public void ItemByNameTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String }
                                      };
            DbParameter expectedParm = target.Add(expectedName, expectedType);
            target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter actualParm = target[expectedName];

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm, actualParm);
            Assert.AreEqual(expectedName, actualParm.ParameterName);
            Assert.AreEqual(expectedType, actualParm.DbType);
            Assert.AreEqual(ParameterDirection.Input, actualParm.Direction);
            Assert.IsFalse(actualParm.IsNullable);
            Assert.AreEqual(0, actualParm.Size);
            Assert.AreEqual(null, actualParm.Value);
            Assert.AreEqual(string.Empty, actualParm.SourceColumn);
            Assert.AreEqual(false, actualParm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, actualParm.SourceVersion);
        }


        /// <summary>
        /// A test for ToArray
        /// </summary>
        [TestMethod]
        public void ToArrayTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer);

            DbParameter expectedParm1 = target.Add("OtherParmaeter", DbType.String);
            DbParameter expectedParm2 = target.Add(expectedName, expectedType);
            DbParameter expectedParm3 = target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter[] actualParms = target.ToArray();

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm1, actualParms[0]);
            Assert.AreEqual(expectedParm2, actualParms[1]);
            Assert.AreEqual(expectedParm3, actualParms[2]);
        }


        /// <summary>
        /// A test for System.Collections.IEnumerable.GetEnumerator
        /// </summary>
        [TestMethod]
        public void GetEnumeratorTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String },
                                          { expectedName, expectedType },
                                          { "AnotherParmaeter", DbType.Int32 }
                                      };

            DbParameter[] expectedParms = target.ToArray();

            IEnumerator actual = target.GetEnumerator();

            foreach (DbParameter expectedParm in expectedParms)
            {
                Assert.IsTrue(actual.MoveNext());
                Assert.AreEqual(expectedParm, actual.Current);
            }
        }


        /// <summary>
        /// A test for op_Implicit
        /// </summary>
        [TestMethod]
        public void ImplicitConversionTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer);

            DbParameter expectedParm1 = target.Add("OtherParmaeter", DbType.String);
            DbParameter expectedParm2 = target.Add(expectedName, expectedType);
            DbParameter expectedParm3 = target.Add("AnotherParmaeter", DbType.Int32);

            // implicit conversion
            DbParameter[] actualParms = target;

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm1, actualParms[0]);
            Assert.AreEqual(expectedParm2, actualParms[1]);
            Assert.AreEqual(expectedParm3, actualParms[2]);
        }


        /// <summary>
        /// A test for GetParameter
        /// </summary>
        [TestMethod]
        public void GetParameterTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String }
                                      };
            DbParameter expectedParm = target.Add(expectedName, expectedType);
            target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter actualParm = target.GetParameter(expectedName);

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm, actualParm);
            Assert.AreEqual(expectedName, actualParm.ParameterName);
            Assert.AreEqual(expectedType, actualParm.DbType);
            Assert.AreEqual(ParameterDirection.Input, actualParm.Direction);
            Assert.IsFalse(actualParm.IsNullable);
            Assert.AreEqual(0, actualParm.Size);
            Assert.AreEqual(null, actualParm.Value);
            Assert.AreEqual(string.Empty, actualParm.SourceColumn);
            Assert.AreEqual(false, actualParm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, actualParm.SourceVersion);
        }


        /// <summary>
        /// A test for GetEnumerator
        /// </summary>
        [TestMethod]
        public void GetTypedEnumeratorTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String },
                                          { expectedName, expectedType },
                                          { "AnotherParmaeter", DbType.Int32 }
                                      };

            DbParameter[] expectedParms = target.ToArray();

            IEnumerator<DbParameter> actual = target.GetEnumerator();

            foreach (DbParameter expectedParm in expectedParms)
            {
                Assert.IsTrue(actual.MoveNext());
                Assert.AreEqual(expectedParm, actual.Current);
            }
        }


        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod]
        public void AddWithNameAndTypeTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String }
                                      };
            DbParameter expectedParm = target.Add(expectedName, expectedType);
            target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter actualParm = target[expectedName];

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm, actualParm);
            Assert.AreEqual(expectedName, actualParm.ParameterName);
            Assert.AreEqual(expectedType, actualParm.DbType);
            Assert.AreEqual(ParameterDirection.Input, actualParm.Direction);
            Assert.IsFalse(actualParm.IsNullable);
            Assert.AreEqual(0, actualParm.Size);
            Assert.AreEqual(null, actualParm.Value);
            Assert.AreEqual(string.Empty, actualParm.SourceColumn);
            Assert.AreEqual(false, actualParm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, actualParm.SourceVersion);
        }


        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod]
        public void AddWithNameAndTypeAndDirectionTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;
            ParameterDirection expectedDirection = ParameterDirection.Output;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String }
                                      };
            DbParameter expectedParm = target.Add(expectedName, expectedType, expectedDirection);
            target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter actualParm = target[expectedName];

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm, actualParm);
            Assert.AreEqual(expectedName, actualParm.ParameterName);
            Assert.AreEqual(expectedType, actualParm.DbType);
            Assert.AreEqual(expectedDirection, actualParm.Direction);
            Assert.IsFalse(actualParm.IsNullable);
            Assert.AreEqual(0, actualParm.Size);
            Assert.AreEqual(null, actualParm.Value);
            Assert.AreEqual(string.Empty, actualParm.SourceColumn);
            Assert.AreEqual(false, actualParm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, actualParm.SourceVersion);
        }


        /// <summary>
        /// A test for Add
        /// </summary>
        [TestMethod]
        public void AddTest()
        {
            string expectedName = "SomeParameter";
            DbType expectedType = DbType.Double;
            double expectedValue = 3.1415927;

            // only need to test sql server since the ClientProviderFactory does
            // the type to instance conversion and has its own unit tests
            ParameterSet target = new ParameterSet(ClientProviderType.SqlServer)
                                      {
                                          { "OtherParmaeter", DbType.String } 
                                      };
            DbParameter expectedParm = target.Add(expectedName, expectedType, expectedValue);
            target.Add("AnotherParmaeter", DbType.Int32);

            DbParameter actualParm = target[expectedName];

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(expectedParm, actualParm);
            Assert.AreEqual(expectedName, actualParm.ParameterName);
            Assert.AreEqual(expectedType, actualParm.DbType);
            Assert.AreEqual(ParameterDirection.Input, actualParm.Direction);
            Assert.IsFalse(actualParm.IsNullable);
            Assert.AreEqual(0, actualParm.Size);
            Assert.AreEqual(expectedValue, actualParm.Value);
            Assert.AreEqual(string.Empty, actualParm.SourceColumn);
            Assert.AreEqual(false, actualParm.SourceColumnNullMapping);
            Assert.AreEqual(DataRowVersion.Current, actualParm.SourceVersion);
        }


        /// <summary>
        /// A test to see if an integer type is correctly inferred for dbtype
        /// </summary>
        [TestMethod]
        public void Add_InfersIntegerType_WhenGivenIntegerValue()
        {
            DbType expectedType = DbType.Int32;

            var parameterSet = new ParameterSet(ClientProviderType.SqlServer);
            var parm = parameterSet.Add("@hello", 17);

            Assert.AreEqual(parm.DbType, expectedType);
            Assert.AreEqual(parameterSet[0].DbType, expectedType);
        }




        /// <summary>
        /// A test to see if an integer type is correctly inferred for dbtype
        /// </summary>
        [TestMethod]
        public void Add_InfersIntegerType_WhenGivenDataTableValue()
        {
            SqlDbType expectedType = SqlDbType.Structured;
            DbType expectedDbType = DbType.Object;

            var parameterSet = new ParameterSet(ClientProviderType.SqlServer);
            var parm = parameterSet.Add("@hello", new DataTable()) as SqlParameter;

            Assert.IsNotNull(parm);
            Assert.AreEqual(parm.SqlDbType, expectedType);
            Assert.AreEqual(parm.DbType, expectedDbType);
        }


        /// <summary>
        /// A test to see if an integer type is correctly inferred for dbtype
        /// </summary>
        [TestMethod]
        public void Add_InfersCorrectTypes_WhenInvokedThroughInitializerList()
        {
            var parameterSet = new ParameterSet(ClientProviderType.SqlServer)
                                   {
                                       { "@hello", 17 },
                                       { "@Hiya", "John Smith" },
                                       { "@ThisShouldStayDouble", DbType.Double, 5 },
                                       { "@Table", new DataTable() }
                                   };

            SqlParameter sp1 = parameterSet[0] as SqlParameter;
            SqlParameter sp2 = parameterSet[1] as SqlParameter;
            SqlParameter sp3 = parameterSet[2] as SqlParameter;
            SqlParameter sp4 = parameterSet[3] as SqlParameter;

            Assert.IsNotNull(parameterSet[0]);
            Assert.IsNotNull(parameterSet[1]);
            Assert.IsNotNull(parameterSet[2]);
            Assert.IsNotNull(parameterSet[3]);

            Assert.AreEqual(sp1.SqlDbType, SqlDbType.Int);
            Assert.AreEqual(sp1.DbType, DbType.Int32);
            Assert.AreEqual(sp2.SqlDbType, SqlDbType.NVarChar);
            Assert.AreEqual(sp2.DbType, DbType.String);
            Assert.AreEqual(sp3.SqlDbType, SqlDbType.Float);
            Assert.AreEqual(sp3.DbType, DbType.Double);
            Assert.AreEqual(sp4.SqlDbType, SqlDbType.Structured);
            Assert.AreEqual(sp4.DbType, DbType.Object);
        }


        /// <summary>
        /// A test to see if an integer type is correctly inferred for dbtype
        /// </summary>
        [TestMethod]
        public void Value_ChangesTypes_WhenValueChanged()
        {
            var parameterSet = new ParameterSet(ClientProviderType.SqlServer)
                                   {
                                       { "@hello", 17 }
                                   };

            SqlParameter sp1 = parameterSet[0] as SqlParameter;

            Assert.IsNotNull(parameterSet[0]);

            Assert.AreEqual(sp1.SqlDbType, SqlDbType.Int);
            Assert.AreEqual(sp1.DbType, DbType.Int32);

            // changing
            sp1.Value = new DataTable();

            Assert.AreEqual(sp1.SqlDbType, SqlDbType.Structured);
            Assert.AreEqual(sp1.DbType, DbType.Object);
        }
    }
}
