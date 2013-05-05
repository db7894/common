using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using SharedAssemblies.General.Database.Mock;


namespace SharedAssemblies.General.Database.Factories
{
    /// <summary>
    /// Provider Factory that constructs all database instance objects specific
    /// to a given provider
    /// </summary>
    public static class ClientProviderFactory
    {
        /// <summary>
        /// generate a provider factory based on the type requested
        /// </summary>
        /// <param name="type">Type of provider factory to create</param>
        /// <returns>The resulting factory</returns>
        public static DbProviderFactory Get(ClientProviderType type)
        {
            DbProviderFactory result = null;

            switch (type)
            {
                case ClientProviderType.SqlServer:
                    // return singleton for sql factory
                    result = SqlClientFactory.Instance;
                    break;

                case ClientProviderType.Odbc:
                    // return singleton for odbc factory
                    result = OdbcFactory.Instance;
                    break;

                case ClientProviderType.OleDb:
                    // return singleton for OLE db factory
                    result = OleDbFactory.Instance;
                    break;

                case ClientProviderType.Mock:
                    // return singleton of mock factory
                    result = MockClientFactory.Instance;
                    break;
            }

            return result;
        }
    }
}
