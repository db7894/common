using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using SharedAssemblies.General.Database.Factories;


namespace SharedAssemblies.General.Database
{
    /// <summary>
    /// This is a collection of generic parameters
    /// </summary>
    public class ParameterSet : IEnumerable<DbParameter>
    {
        /// <summary>
        /// A private collection of parameters
        /// </summary>
        private readonly List<DbParameter> _parameters;


		/// <summary>
		/// Indexer to get the parameter at a given position
		/// </summary>
		/// <param name="index">The index of the parameter</param>
		/// <returns>The DbParameter instance</returns>
		public DbParameter this[int index]
		{
			get { return _parameters[index]; }
		}


		/// <summary>
		/// Indexer to get the parameter at a given position
		/// </summary>
		/// <param name="parameterName">The name of the parameter</param>
		/// <returns>The DbParameter instance</returns>
		public DbParameter this[string parameterName]
		{
			get { return GetParameter(parameterName); }
		}


		/// <summary>
		/// Public property to get the count of parameters in the set
		/// </summary>
		public int Count
		{
			get { return _parameters.Count; }
		}


		/// <summary>
		/// Property to get/set the provider factory
		/// </summary>
		public DbProviderFactory ProviderFactory { get; private set; }


		/// <summary>
		/// Property to get/set the parameter factory
		/// </summary>
		public ParameterFactory ParameterFactory { get; private set; }
		
		
		/// <summary>
        /// Construct a new parameter set given a provider type
        /// </summary>
        /// <param name="type">Enumeration to specify provider type</param>
        public ParameterSet(ClientProviderType type)
            : this(ClientProviderFactory.Get(type))
        {
        }


        /// <summary>
        /// Construct a new parameter set given a provider factory
        /// </summary>
        /// <param name="factory">Instance of a provider factory</param>
        public ParameterSet(DbProviderFactory factory)
        {
            ProviderFactory = factory;
            ParameterFactory = new ParameterFactory(factory);

            // new empty parm list
            _parameters = new List<DbParameter>();
        }


        /// <summary>
        /// Create an input parameter of the specified name with no type or value
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <returns>Input parameter of specified name with no type and no value</returns>
        public DbParameter Add(string parameterName)
        {
            DbParameter parm = ParameterFactory.Create(parameterName);

            _parameters.Add(parm);

            return parm;
        }




        /// <summary>
        /// Create an input parameter of the specified name and value, the type is inferred
        /// from the type of the value passed in.  This is assumed to be an input parameter
        /// since you're giving it a value.
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="value">Value of parameter</param>
        /// <returns>Input parameter of specified name and value and inferred type</returns>
        public DbParameter Add(string parameterName, object value)
        {
            DbParameter parm = ParameterFactory.Create(parameterName, value);

            _parameters.Add(parm);

            return parm;
        }


        /// <summary>
        /// Create an input parameter of the specified type with no value
        /// </summary>
        /// <param name="parameterName">Name of parameter</param>
        /// <param name="dbType">Type of parameter</param>
        /// <returns>Input parameter of specified name and type with no value</returns>
        public DbParameter Add(string parameterName, DbType dbType)
        {
            DbParameter parm = ParameterFactory.Create(parameterName, dbType);

            _parameters.Add(parm);

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
        public DbParameter Add(string parameterName, DbType dbType, ParameterDirection direction)
        {
            DbParameter parm = ParameterFactory.Create(parameterName, dbType, direction);

            _parameters.Add(parm);

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
        public DbParameter Add(string parameterName, DbType dbType, object value)
        {
            DbParameter parm = ParameterFactory.Create(parameterName, dbType, value);

            _parameters.Add(parm);

            return parm;
        }


        /// <summary>
        /// Adds a collection of parameters over to the parameter set.
        /// </summary>
        /// <param name="parameterSet">A set of parameters</param>
        public void Add(IEnumerable<DbParameter> parameterSet)
        {
            _parameters.AddRange(parameterSet);
        }


        /// <summary>
        /// Get the parameter with a given name
        /// </summary>
        /// <param name="parameterName">The name of the parameter</param>
        /// <returns>The db parameter instance</returns>
        public DbParameter GetParameter(string parameterName)
        {
            return _parameters.Find(p => p.ParameterName.Equals(parameterName));
        }


        /// <summary>
        /// Converts the parameter set to an array of parameters
        /// </summary>
        /// <returns>Array of DbParameter</returns>
        public DbParameter[] ToArray()
        {
            return _parameters.ToArray();
        }


        /// <summary>
        /// Create an implicit conversion for ParameterSet to DbParameter[]
        /// </summary>
        /// <param name="parameters">a set of parameters</param>
        /// <returns>A fixed-length array of parameters</returns>
        public static implicit operator DbParameter[](ParameterSet parameters)
        {
            return parameters.ToArray();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be 
        /// used to iterate through the collection.
        /// </returns>
        public IEnumerator<DbParameter> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be 
        /// used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
