using System.Data;
using System.Data.Common;


namespace SharedAssemblies.General.Database.Factories
{
    /// <summary>
    /// This class is used to create provider specific parameters in a neutral way
    /// </summary>
    public class ParameterFactory
    {
		/// <summary>
		/// Property to get/set the client provider factory
		/// </summary>
		public DbProviderFactory ProviderFactory { get; private set; }
		
		
		/// <summary>
        /// Create a parameter factory that conforms to a provider type
        /// </summary>
        /// <param name="type">Enum specifying provider type</param>
        public ParameterFactory(ClientProviderType type)
            : this(ClientProviderFactory.Get(type))
        {
        }


        /// <summary>
        /// Create a parameter factory given a specific client provider factory
        /// </summary>
        /// <param name="clientFactory">Instance of a client provider factory</param>
        public ParameterFactory(DbProviderFactory clientFactory)
        {
            ProviderFactory = clientFactory;
        }


        /// <summary>
        /// Returns a parameter set for the given provider factory
        /// </summary>
        /// <returns>A ParameterSet geared to the correct provider factory</returns>
        public ParameterSet CreateSet()
        {
            return new ParameterSet(ProviderFactory);
        }


        /// <summary>
        /// Create an empty input parameter.  Assumed to be type string.
        /// </summary>
        /// <returns>Empty input parameter of type string</returns>
        public DbParameter Create()
        {
            return ProviderFactory.CreateParameter();
        }


        /// <summary>
        /// Create an empty input parameter.  Assumed to be type string.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to create.</param>
        /// <returns>Empty input parameter of type string</returns>
        public DbParameter Create(string parameterName)
        {
            DbParameter parm = Create();
            parm.ParameterName = parameterName;

            return parm;
        }


        /// <summary>
        /// Create an input parameter of the specified value, the type is inferred
        /// from the value's type.
        /// </summary>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Input parameter of specified name with given value</returns>
        public DbParameter Create(string parameterName, object value)
        {
            DbParameter parm = Create(parameterName);
            parm.Value = value;

            return parm;
        }


        /// <summary>
        /// Create an input parameter of the specified type with no value
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="dbType">Type of parameter</param>
        /// <returns>Input parameter of specified name and type with no value</returns>
        public DbParameter Create(string parameterName, DbType dbType)
        {
            DbParameter parm = Create();
            parm.ParameterName = parameterName;
            parm.DbType = dbType;

            return parm;
        }


        /// <summary>
        /// Create a parameter of the specified name, type, and direction.  Since we're allowing
        /// direction, which could be output, no value is specified, though can be set using 
        /// the Value property.
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="dbType">Type of parameter</param>
        /// <param name="direction">Direction (input, output, etc) of parameter</param>
        /// <returns>Parameter of the specified name, type, and direction</returns>
        public DbParameter Create(string parameterName, DbType dbType, ParameterDirection direction)
        {
            DbParameter parm = Create(parameterName, dbType);
            parm.Direction = direction;

            return parm;
        }


        /// <summary>
        /// Create an input parameter of the specified name, type, and value.  This is assumed
        /// to be an input parameter since you're giving it a value.
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="dbType">Type of parameter</param>
        /// <param name="value">Value of parameter</param>
        /// <returns>Input parameter of specified name, type, and value</returns>
        public DbParameter Create(string parameterName, DbType dbType, object value)
        {
            DbParameter parm = Create(parameterName, dbType);
            parm.Value = value;

            return parm;
        }
    }
}
