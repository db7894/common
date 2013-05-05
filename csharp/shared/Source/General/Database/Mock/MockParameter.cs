using System.Data;
using System.Data.Common;


namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// A fake parameter class that mirrors the System.Data model
    /// </summary>
    public class MockParameter : DbParameter
    {
        /// <summary>The source version of the mock parameter.</summary>
        private DataRowVersion _sourceVersion = DataRowVersion.Current;

        /// <summary>The default type of the mock parameter.</summary>
        private DbType _dbType = DbType.String;

        /// <summary>The default direction of the mock parameter.</summary>
        private ParameterDirection _direction = ParameterDirection.Input;

        /// <summary>The default name of the mock parameter.</summary>
        private string _parameterName = string.Empty;

        /// <summary>The default column name of the mock parameter.</summary>
        private string _sourceColumn = string.Empty;


		/// <summary>
		/// Gets or sets the database type of the parameter.
		/// </summary>
		/// <returns>
		/// One of the DbType values. The default is string.
		/// </returns>
		public override DbType DbType
		{
			get { return _dbType; }
			set { _dbType = value; }
		}


		/// <summary>
		/// Gets or sets a value that indicates whether the parameter is 
		/// input-only, output-only, bidirectional, or a stored procedure return value parameter.
		/// </summary>
		/// <returns>
		/// One of the ParameterDirection values. The default is Input.
		/// </returns>
		public override ParameterDirection Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}


		/// <summary>
		/// Gets or sets a value that indicates whether the parameter accepts null values.
		/// </summary>
		/// <returns>
		/// True if null values are accepted; otherwise false. The default is false.
		/// </returns>
		public override bool IsNullable { get; set; }


		/// <summary>
		/// Gets or sets the name of the DbParameter.
		/// </summary>
		/// <returns>
		/// The name of the DbParameter. The default is an empty string ("").
		/// </returns>
		public override string ParameterName
		{
			get { return _parameterName; }
			set { _parameterName = value; }
		}


		/// <summary>
		/// Gets or sets the name of the source column mapped to the DataSet 
		/// and used for loading or returning the Value.
		/// </summary>
		/// <returns>
		/// The name of the source column mapped to the DataSet. 
		/// The default is an empty string.
		/// </returns>
		public override string SourceColumn
		{
			get { return _sourceColumn; }
			set { _sourceColumn = value; }
		}


		/// <summary>
		/// Gets or sets the DataRowVersion to use when you load Value.
		/// </summary>
		/// <returns>
		/// One of the DataRowVersion values. The default is Current.
		/// </returns>
		public override DataRowVersion SourceVersion
		{
			get { return _sourceVersion; }
			set { _sourceVersion = value; }
		}


		/// <summary>
		/// Gets or sets the value of the parameter.
		/// </summary>
		/// <returns>
		/// An Object that is the value of the parameter. The default value is null.
		/// </returns>
		public override object Value { get; set; }


		/// <summary>
		/// Sets or gets a value which indicates whether the source column is 
		/// nullable. This allows DbCommandBuilder to correctly generate 
		/// Update statements for nullable columns.
		/// </summary>
		/// <returns>
		/// true if the source column is nullable; false if it is not.
		/// </returns>
		public override bool SourceColumnNullMapping { get; set; }


		/// <summary>
		/// Gets or sets the maximum size, in bytes, of the data within the column.
		/// </summary>
		/// <returns>
		/// The maximum size, in bytes, of the data within the column. 
		/// The default value is inferred from the parameter value.
		/// </returns>
		public override int Size { get; set; }
		
		
		/// <summary>
        /// Resets the DbType property to its original settings.
        /// </summary>
        public override void ResetDbType()
        {
            DbType = DbType.String;
        }
    }
}
