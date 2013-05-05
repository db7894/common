using System;
using System.Collections;
using System.Data;
using System.Data.Common;


namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// Mock class that emulates a data reader
    /// </summary>
    public class MockDataReader : DbDataReader
    {
        /// <summary>
        /// True if data reader is closed
        /// </summary>
        private bool _isClosed;


		/// <summary>
		/// Current row in fake data table 
		/// </summary>
		public int CurrentRow { get; private set; }


		/// <summary>
		/// Current row in fake data table 
		/// </summary>
		public int CurrentResultSet { get; private set; }


		/// <summary>
		/// Property to get/set fake data table as results of "query"
		/// </summary>
		public DataTable[] MockData { get; set; }


		/// <summary>
		/// Link back to the fake command that generated this reader
		/// </summary>
		public MockCommand MockCommand { get; private set; }


		/// <summary>
		/// property to get/set fake number of records affected
		/// </summary>
		public int MockRecordsAffected { get; set; }


		/// <summary>
		/// property to get/set fake number of records affected
		/// </summary>
		public int MockDepth { get; set; }


		/// <summary>
		/// Gets a value indicating the depth of nesting for the current row.
		/// </summary>
		/// <returns>
		/// The depth of nesting for the current row.
		/// </returns>
		public override int Depth
		{
			get { return MockDepth; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Data.Common.DbDataReader" /> 
		/// is closed.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Data.Common.DbDataReader" /> is closed; 
		/// otherwise false.
		/// </returns>
		public override bool IsClosed
		{
			get { return _isClosed; }
		}


		/// <summary>
		/// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement. 
		/// </summary>
		/// <returns>
		/// The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected or the statement failed.
		/// </returns>
		public override int RecordsAffected
		{
			get { return MockRecordsAffected; }
		}


		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		/// <returns>
		/// The number of columns in the current row.
		/// </returns>
		public override int FieldCount
		{
			get { return MockData[CurrentResultSet].Columns.Count; }
		}


		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="ordinal">
		/// The zero-based column ordinal.
		/// </param>
		public override object this[int ordinal]
		{
			get { return MockData[CurrentResultSet].Rows[CurrentRow][ordinal]; }
		}


		/// <summary>
		/// Gets the value of the specified column as an instance of <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		/// The value of the specified column.
		/// </returns>
		/// <param name="name">
		/// The name of the column.
		/// </param>
		public override object this[string name]
		{
			get { return MockData[CurrentResultSet].Rows[CurrentRow][name]; }
		}


		/// <summary>
		/// Gets a value that indicates whether this <see cref="T:System.Data.Common.DbDataReader" /> 
		/// contains one or more rows.
		/// </summary>
		/// <returns>
		/// true if the <see cref="T:System.Data.Common.DbDataReader" /> contains one 
		/// or more rows; otherwise false.
		/// </returns>
		public override bool HasRows
		{
			get { return CurrentRow < MockData[CurrentResultSet].Rows.Count; }
		}


		/// <summary>
        /// Constructor to create the mock data reader
        /// </summary>
        /// <param name="command">The command that the mock data reader will read from.</param>
        public MockDataReader(MockCommand command)
        {
            CurrentRow = -1;
            CurrentResultSet = 0;
            MockCommand = command;
        }


        /// <summary>
        /// Closes the <see cref="T:System.Data.Common.DbDataReader" /> object.
        /// </summary>
        public override void Close()
        {
            _isClosed = true;
        }


        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the 
        /// column metadata of the <see cref="T:System.Data.Common.DbDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException("MockDataReader does not implement the " +
                "GetSchemaTable member function.");
        }


        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch 
        /// of statements.
        /// </summary>
        /// <returns>
        /// true if there are more result sets; otherwise false.
        /// </returns>
        public override bool NextResult()
        {
            CurrentRow = -1;
            return ++CurrentResultSet < MockData.Length;
        }


        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise false.
        /// </returns>
        public override bool Read()
        {
            return ++CurrentRow < MockData[CurrentResultSet].Rows.Count;
        }


        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override bool GetBoolean(int i)
        {
            return (bool)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override byte GetByte(int i)
        {
            return (byte)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Reads a stream of bytes from the specified column offset 
        /// into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <param name="fieldOffset">the offset of the field.</param>
        /// <param name="buffer">the buffer to get into.</param>
        /// <param name="bufferoffset">the offset to write in the buffer.</param>
        /// <param name="length">the number of bytes to write.</param>
        /// <returns>Nothing, not implemented for mock.</returns>
        /// <exception cref="NotImplementedException">Always throws.</exception>
        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException("MockDataReader does not implement the " +
                "GetBytes member function.");
        }


        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override char GetChar(int i)
        {
            return (char)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <param name="fieldOffset">the offset of the field.</param>
        /// <param name="buffer">the buffer to get into.</param>
        /// <param name="bufferoffset">the offset to write in the buffer.</param>
        /// <param name="length">the number of bytes to write.</param>
        /// <returns>Nothing, not implemented for mock.</returns>
        /// <exception cref="NotImplementedException">Always throws.</exception>
        public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException("MockDataReader does not implement the " +
                "GetChars member function.");
        }


        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override Guid GetGuid(int i)
        {
            return (Guid)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override short GetInt16(int i)
        {
            return (short)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override int GetInt32(int i)
        {
            return (int)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override long GetInt64(int i)
        {
            return (long)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override float GetFloat(int i)
        {
            return (float)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override double GetDouble(int i)
        {
            return (double)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override string GetString(int i)
        {
            return MockData[CurrentResultSet].Rows[CurrentRow][i].ToString();
        }


        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override decimal GetDecimal(int i)
        {
            return (decimal)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override DateTime GetDateTime(int i)
        {
            return (DateTime)MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override bool IsDBNull(int i)
        {
            return (MockData[CurrentResultSet].Rows[CurrentRow][i] == null);
        }


        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <returns>
        /// The name of the specified column.
        /// </returns>
        /// <param name="ordinal">
        /// The zero-based column ordinal.
        /// </param>
        public override string GetName(int ordinal)
        {
            return MockData[CurrentResultSet].Columns[ordinal].ColumnName;
        }


        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <returns>
        /// The zero-based column ordinal.
        /// </returns>
        /// <param name="name">
        /// The name of the column.
        /// </param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// The name specified is not a valid column name.
        /// </exception>
        public override int GetOrdinal(string name)
        {
            return MockData[CurrentResultSet].Columns[name].Ordinal;
        }


        /// <summary>
        /// Gets name of the data type of the specified column.
        /// </summary>
        /// <returns>
        /// A string representing the name of the data type.
        /// </returns>
        /// <param name="ordinal">
        /// The zero-based column ordinal.
        /// </param>
        public override string GetDataTypeName(int ordinal)
        {
            return MockData[CurrentResultSet].Columns[ordinal].DataType.FullName;
        }


        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <returns>
        /// The data type of the specified column.
        /// </returns>
        /// <param name="ordinal">
        /// The zero-based column ordinal.
        /// </param>
        public override Type GetFieldType(int ordinal)
        {
            return MockData[CurrentResultSet].Columns[ordinal].DataType;
        }


        /// <summary>
        /// Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate 
        /// through the rows in the data reader.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate 
        /// through the rows in the data reader.
        /// </returns>
        public override IEnumerator GetEnumerator()
        {
            return MockData[CurrentResultSet].Rows.GetEnumerator();
        }


        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The column index.</param>
        /// <returns>value from column i.</returns>
        public override object GetValue(int i)
        {
            return MockData[CurrentResultSet].Rows[CurrentRow][i];
        }


        /// <summary>
        /// Gets all the attribute fields in the collection for the current record.
        /// </summary>
        /// <param name="values">The array to fill with values from row.</param>
        /// <returns>Number of items returned.</returns>
        public override int GetValues(object[] values)
        {
            MockData[CurrentResultSet].Rows[CurrentRow].ItemArray.CopyTo(values, 0);

            return MockData[CurrentResultSet].Columns.Count;
        }
    }
}
