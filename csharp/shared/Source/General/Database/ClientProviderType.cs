namespace SharedAssemblies.General.Database
{
    /// <summary>
    /// Enumeration to describe types of connection factories available
    /// </summary>
    public enum ClientProviderType
    {
        /// <summary>
        /// Specifies an unknown connection factory
        /// </summary>
        Unknown,

        /// <summary>
        /// SqlServer connection type is used for SQL Server database connections
        /// </summary>
        SqlServer,

        /// <summary>
        /// ODBC connection type is used for any ODBC database type, though better
        /// to use a direct client if possible for SQL, Oracle.
        /// </summary>
        Odbc,

        /// <summary>
        /// OLE connection type is used for OLE connections
        /// </summary>
        OleDb,

        /// <summary>
        /// A Mock connection type is used for unit testing to test various results
        /// </summary>
        Mock
    }
}
