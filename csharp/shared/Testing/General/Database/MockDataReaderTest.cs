using System;
using System.Data;
using SharedAssemblies.General.Database.Mock;
using SharedAssemblies.General.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharedAssemblies.General.Database.UnitTests
{
	/// <summary>
    /// This is a test class for MockDataReaderTest and is intended
    /// to contain all MockDataReaderTest Unit Tests
    /// </summary>
    [TestClass]
    public class MockDataReaderTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// A test for RecordsAffected
        /// </summary>
        [TestMethod]
        public void RecordsAffectedTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int expected = 7;
            target.MockRecordsAffected = expected;

            int actual = target.RecordsAffected;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// A test for Item
        /// </summary>
        [TestMethod]
        public void ItemByIndexTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            const int resultSetIndex = 0;
            string expected = "Hello World";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn("Column Name", typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expected });

            Assert.IsTrue(target.Read());

            Assert.AreEqual(expected, target[0]);
        }


        /// <summary>
        /// GetEnumerator() returns enumerator to data
        /// </summary>
        [TestMethod]
        public void GetEnumerator_ReturnsEnumeratorToData_OnCall()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            const int resultSetIndex = 0;
            string expected = "Hello World";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn("Column Name", typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expected });

            Assert.AreEqual(target.GetEnumerator(), target.MockData[0].Rows.GetEnumerator());
        }


        /// <summary>
        /// HasRows returns true if data
        /// </summary>
        [TestMethod]
        public void HasRows_ReturnsTrue_IfData()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            const int resultSetIndex = 0;
            int expectedNumOfRows = 0;

            target.MockData = new[] { new DataTable("Table 1") };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn("Column Name", typeof(string)));
            System.Data.DataRow expectedRow = target.MockData[resultSetIndex].NewRow();
            expectedRow.SetField("Column Name", "Row 1");
            target.MockData[resultSetIndex].Rows.Add(expectedRow);
            ++expectedNumOfRows;

            Assert.AreEqual(expectedNumOfRows, target.MockData[resultSetIndex].Rows.Count);
        }

        
        /// <summary>
        /// HasRows returns false if no data
        /// </summary>
        [TestMethod]
        public void HasRows_ReturnsFalse_IfNoData()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            const int resultSetIndex = 0;
            const int expectedNumOfRows = 0;

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn("Column Name", typeof(string)));

            Assert.AreEqual(expectedNumOfRows, target.MockData[resultSetIndex].Rows.Count);
        }


        /// <summary>
        /// A test for Item
        /// </summary>
        [TestMethod]
        public void ItemByNameTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expected = "Hello World";
            string columnName = "Test Column";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnName, typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expected });

            target.Read();

            Assert.AreEqual(expected, target[columnName]);
        }


        /// <summary>
        /// A test for IsClosed
        /// </summary>
        [TestMethod]
        public void IsClosedTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            Assert.IsFalse(target.IsClosed);

            target.Close();

            Assert.IsTrue(target.IsClosed);
        }


        /// <summary>
        /// A test for FieldCount
        /// </summary>
        [TestMethod]
        public void FieldCountTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            const int resultSetIndex = 0;
            const int fieldCount = 1;

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn("Test Column", typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { "Hello World" });

            Assert.AreEqual(fieldCount, target.FieldCount);
        }


        /// <summary>
        /// A test for Depth
        /// </summary>
        [TestMethod]
        public void DepthTest()
        {
            MockDataReader target = CreateMockReader();

            const int expectedDepth = 7;
            target.MockDepth = expectedDepth;

            Assert.AreEqual(expectedDepth, target.Depth);
        }


        /// <summary>
        /// A test for Read
        /// </summary>
        [TestMethod]
        public void ReadTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expected = "Hello World";
            string columnName = "Test Column";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnName, typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expected });

            // first read should succeed since 1 row of data
            Assert.IsTrue(target.Read());

            // second read should fail since no more data
            Assert.IsFalse(target.Read());
        }


        /// <summary>
        /// A test for NextResult
        /// </summary>
        [TestMethod]
        public void NextResultTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expected = "Hello World";
            string columnOneName = "Test Column 1";
            string columnTwoName = "Test Column 2";
            string columnThreeName = "Test Column 3";

            target.MockData = new[] { new DataTable(), new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expected });
            target.MockData[++resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expected, expected });

            // first read should succeed since 1 row of data
            Assert.IsTrue(target.Read());

            // first result set has 1 column
            Assert.AreEqual(target.FieldCount, 1);

            // second read should fail since no more data
            Assert.IsFalse(target.Read());

            // go to next result set
            Assert.IsTrue(target.NextResult());

            // first read should succeed since 1 row of data
            Assert.IsTrue(target.Read());

            // second result set has 2 columns
            Assert.AreEqual(target.FieldCount, 2);

            // second read should fail since no more data
            Assert.IsFalse(target.Read());

            // no more result sets so should fail
            Assert.IsFalse(target.NextResult());
        }


        /// <summary>
        /// A test for IsDBNull
        /// </summary>
        [TestMethod]
        public void IsDbNullTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expected = "Hello World";
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { null, expected });

            // first read should succeed since 1 row of data
            Assert.IsTrue(target.Read());

            // check column for null
            Assert.IsFalse(target.IsDBNull(1));
        }


        /// <summary>
        /// A test for GetValues
        /// </summary>
        [TestMethod]
        public void GetValuesTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = string.Empty;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();

            object[] actual = new object[3];
            target.GetValues(actual);

            Assert.AreEqual(actual[0], expectedOne);
            Assert.AreEqual(actual[1], expectedTwo);
            Assert.AreEqual(actual[2], expectedThree);
        }


        /// <summary>
        /// A test for GetValue
        /// </summary>
        [TestMethod]
        public void GetValueTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = string.Empty;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();

            Assert.AreEqual(target.GetValue(0), expectedOne);
            Assert.AreEqual(target.GetValue(1), expectedTwo);
            Assert.AreEqual(target.GetValue(2), expectedThree);
        }


        /// <summary>
        /// A test for GetString
        /// </summary>
        [TestMethod]
        public void GetStringTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = string.Empty;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();

            Assert.AreEqual(target.GetString(0), expectedOne);
            Assert.AreEqual(target.GetString(1), expectedTwo);
            Assert.AreEqual(target.GetString(2), expectedThree.ToString());
        }


        /// <summary>
        /// A test for GetSchemaTable
        /// </summary>
        [TestMethod]
        public void GetSchemaTableTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

			AssertEx.Throws<NotImplementedException>(() =>
                target.GetSchemaTable());
        }


        /// <summary>
        /// A test for GetOrdinal
        /// </summary>
        [TestMethod]
        public void GetOrdinalTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = null;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();
            Assert.AreEqual(target.GetOrdinal(columnOneName), 0);
            Assert.AreEqual(target.GetOrdinal(columnTwoName), 1);
            Assert.AreEqual(target.GetOrdinal(columnThreeName), 2);
        }


        /// <summary>
        /// A test for GetName
        /// </summary>
        [TestMethod]
        public void GetNameTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = null;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();
            Assert.AreEqual(target.GetName(0), columnOneName);
            Assert.AreEqual(target.GetName(1), columnTwoName);
            Assert.AreEqual(target.GetName(2), columnThreeName);
        }


        /// <summary>
        /// A test for GetInt64
        /// </summary>
        [TestMethod]
        public void GetInt64Test()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            long expectedOne = 7;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(long)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetInt64(0), expectedOne);
        }


        /// <summary>
        /// A test for GetInt32
        /// </summary>
        [TestMethod]
        public void GetInt32Test()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            int expectedOne = 7;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetInt32(0), expectedOne);
        }


        /// <summary>
        /// A test for GetInt16
        /// </summary>
        [TestMethod]
        public void GetInt16Test()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            short expectedOne = 7;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(short)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetInt16(0), expectedOne);
        }


        /// <summary>
        /// A test for GetGuid
        /// </summary>
        [TestMethod]
        public void GetGuidTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            Guid expectedOne = new Guid();
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(Guid)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetGuid(0), expectedOne);
        }


        /// <summary>
        /// A test for GetFloat
        /// </summary>
        [TestMethod]
        public void GetFloatTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            float expectedOne = 3.1415927f;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(float)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetFloat(0), expectedOne);
        }


        /// <summary>
        /// A test for GetFieldType
        /// </summary>
        [TestMethod]
        public void GetFieldTypeTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = null;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();
            Assert.AreEqual(target.GetFieldType(0), typeof(string));
            Assert.AreEqual(target.GetFieldType(1), typeof(string));
            Assert.AreEqual(target.GetFieldType(2), typeof(int));
        }


        /// <summary>
        /// A test for GetDouble
        /// </summary>
        [TestMethod]
        public void GetDoubleTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            double expectedOne = 3.1415927;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(double)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetDouble(0), expectedOne);
        }


        /// <summary>
        /// A test for GetDecimal
        /// </summary>
        [TestMethod]
        public void GetDecimalTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            decimal expectedOne = new decimal(3.1415927);
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(decimal)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetDecimal(0), expectedOne);
        }


        /// <summary>
        /// A test for GetDateTime
        /// </summary>
        [TestMethod]
        public void GetDateTimeTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            DateTime expectedOne = DateTime.Now;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(DateTime)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetDateTime(0), expectedOne);
        }


        /// <summary>
        /// A test for GetDataTypeName
        /// </summary>
        [TestMethod]
        public void GetDataTypeNameTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = null;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();
            Assert.AreEqual(target.GetDataTypeName(0), typeof(string).FullName);
            Assert.AreEqual(target.GetDataTypeName(1), typeof(string).FullName);
            Assert.AreEqual(target.GetDataTypeName(2), typeof(int).FullName);
        }


        /// <summary>
        /// A test for GetData
        /// </summary>
        [TestMethod]
        public void GetDataTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            string expectedOne = null;
            string expectedTwo = "Hello World";
            int expectedThree = 7;
            string columnOneName = "Test Column One";
            string columnTwoName = "Test Column Two";
            string columnThreeName = "Test Column Three";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnTwoName, typeof(string)));
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnThreeName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(
				new object[] { expectedOne, expectedTwo, expectedThree });

            target.Read();

			AssertEx.Throws<NotSupportedException>(() => target.GetData(2));
        }


        /// <summary>
        /// A test for GetChars
        /// </summary>
        [TestMethod]
        public void GetCharsTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

			AssertEx.Throws<NotImplementedException>(() =>
                target.GetChars(0, 0, null, 0, 0));
        }


        /// <summary>
        /// A test for GetChar
        /// </summary>
        [TestMethod]
        public void GetCharTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            char expectedOne = 'X';
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(char)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetChar(0), expectedOne);
        }


        /// <summary>
        /// A test for GetBytes
        /// </summary>
        [TestMethod]
        public void GetBytesTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

			AssertEx.Throws<NotImplementedException>(() =>
                target.GetBytes(0, 0, null, 0, 0));
        }


        /// <summary>
        /// A test for GetByte
        /// </summary>
        [TestMethod]
        public void GetByteTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            byte expectedOne = 7;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(byte)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetByte(0), expectedOne);
        }


        /// <summary>
        /// A test for GetBoolean
        /// </summary>
        [TestMethod]
        public void GetBooleanTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            bool expectedOne = true;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(bool)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            target.Read();
            Assert.AreEqual(target.GetBoolean(0), expectedOne);
        }


        /// <summary>
        /// A test for Dispose
        /// </summary>
        [TestMethod]
        public void DisposeTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            int expectedOne = 7;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            // dispose should close reader
            target.Dispose();
            Assert.IsTrue(target.IsClosed);
        }


        /// <summary>
        /// A test for Close
        /// </summary>
        [TestMethod]
        public void CloseTest()
        {
            // generate a mock reader
            MockDataReader target = CreateMockReader();

            int resultSetIndex = 0;
            int expectedOne = 7;
            string columnOneName = "Test Column One";

            target.MockData = new[] { new DataTable() };
            target.MockData[resultSetIndex].Columns.Add(new DataColumn(columnOneName, typeof(int)));
            target.MockData[resultSetIndex].Rows.Add(new object[] { expectedOne });

            // Close() should close reader
            target.Close();
            Assert.IsTrue(target.IsClosed);
        }


        /// <summary>
        /// concrete method to create a new mock reader
        /// </summary>
        /// <returns>a mock data reader</returns>
        private static MockDataReader CreateMockReader()
        {
            return new MockDataReader(new MockCommand(new MockConnection()));
        }
    }
}
