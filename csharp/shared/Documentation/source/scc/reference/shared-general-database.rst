=========================================================================
Database --- Safe & Simple Database Utility
=========================================================================
:Assembly: SharedAssemblies.General.Database.dll
:Namespace: SharedAssemblies.General.Database
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.General.Database
   :platform: Windows, .Net
   :synopsis: Database - Safe & Simply Database Utility

.. highlight:: csharp

.. index:: Database

Introduction
------------------------------------------------------------

The **SharedAssemblies.General.Database** library is a thread-safe and extremely leak-resistant database library that
greatly simplifies the developer's job of retrieving or storing data in a database.  Microsoft, for its part, did a very good job with making their .Net 
ADO classes easy to use, but unfortunately they can cause issues if used incorrectly in a multi-threaded environment, 
and can leak resources if not properly disposed.

This library simplifies the task of using ADO safely by abstracting all the calls needed into the most basically requested ADO functions:

    * Execute a query and get results *(scalar, DataSet, DataReader)*
    * Execute a non-query and get affected rows *(insert, update, delete)*

All resources used are disposed immediately with the exception of the *ExecuteDataReader(...)* method, which returns a DataReader which should be
disposed as soon as it is no longer needed.  Also, since all resources are taken from the pool and released only during the life of the call,
there are no thread-safety concerns and the DatabaseUtility can be used safely across all threads in an application.

Also, this library is completely abstracted from the implementation.  Thus, switching from Sql to Oracle requires only changing the enumeration 
passed to the *DatabaseUtility* constructor and no underlying usage code need change.  In addition, this library supports full mocking capabilities.
All you need do is specify the mock provider in the enum passed to the constructor, and the DatabaseUtility will serve up pre-determined mock data
for easy and thorough unit testing.

The **SharedAssemblies.General.Database** library is also often referred to as the *Shared Database* library.  This is actually a bit of
a misnomer.  The library originated in the TFS location *Assemblies.Shared.Database* hence the short name *Shared Database* library, but
once the *Shared Component Committee* began consolidating shared libraries for reuse, the old library was marked *[Obsolete]* and now should no
longer be used.  Keep in mind, however, that even though the new location is *SharedAssemblies.General.Database*, it is still referred to as
*Shared Database* by many.

Usage
--------------------------------------------------------------

There are three main classes you will deal with when working with the *Database* library.  They are:

    * **DatabaseUtility** - *The utility class for executing all database commands.*
    * **ParameterSet** - *The generic collection that allows you to add parameters to commands.*
    * **MockClientFactory** - *The mock framework for queueing mock db results for use in unit tests.*
    
Instantiating DatabaseUtility
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        
    The **DatabaseUtility** is the primary class that will be invoked by all developers.  It can be used to create parameter sets, and will serve
    up mock results if the mock provider is chosen.  The *DatabaseUtility* is fully thread-safe and is leak-proof.  The only caveat being that if 
    using **ExecuteDataReader(...)** the developer should call *Dispose()* on the returned *DataReader* when finished to return the connection
    to the pool immediately instead of waiting for garbage collection.
    
    .. Note:: *DatabaseUtility* implements **IDatabaseUtility**, this enables interceptors to decorate the class.  

    To use the *DatabaseUtility*, one needs to simply instantiate it with the enum specifying which provider to use.  You can have as many separate instances
    of the *DatabaseUtility* as you wish, they will all use connection pooling as set up by ADO.Net and will all be thread-safe::

        /// <summary>
        /// A theoretical DAO that would return open orders for an account
        /// </summary>
        public class OpenOrderDao
        {
            // instance of the database utility
            private DatabaseUtility _database;

            /// <summary>
            /// It is recommended you inject your provider type and connection string at
            /// construction, in this way you are not hard coded to any definition and can 
            /// inject the mock type instead for unit testing.
            /// </summary>
            public OpenOrderDao(ClientProviderType providerType, string connectionString)
            {
                _database = new DatabaseUtility(providerType, connectionString);
            }

            // ... 
        }

    Notice in the above example how the connection string and provider type are *injected* in the constructor.  This is the best-practice to allow the 
    classes using the *DatabaseUtility* to be fully unit testable.  In this way, the unit test can pass the **ClientProviderType.Mock** to the 
    constructor and be able to fully test the business logic without needing any database connectivity.  If you wish, you can have a second form of the constructor that 
    default the parameter to your connection type of choice, and then just use this form of the constructor for unit testing, like so::

            /// <summary>
            /// Creates a connection using SQL Server as the provider type, cross-calls
            /// to the full constructor.
            /// </summary>
            public OpenOrderDao(string connectionString) :
                this(ClientProviderType.SqlServer, connectionString)
            {
            }

            /// <summary>
            /// The full constructor takes both provider type and connection string, can 
            /// use this one by unit tests to explicitly specify mock.
            /// </summary>
            public OpenOrderDao(ClientProviderType providerType, string connectionString)
            {
                _database = new DatabaseUtility(providerType, connectionString);
            }
        
Executing Statements
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    Once your *DatabaseUtility* instance is declared, you can invoke queries or non-queries in several ways:

        * **ExecuteDataReader** - *Creates a forward-only reader for consuming rows from a query.  Most efficient way to read data, but should always remember to Dispose() reader immediately when done.*
        * **ExecuteDataSet** - *Creates a *DataSet* in memory with all the rows from a query.  This is the most memory intensive and is not very efficient.*
        * **ExecuteScalar** - *Executes a query that returns only one value (for example, "SELECT MAX(SALARY) FROM EMPLOYEE").  Very efficient when calling a statement that returns only one row and one column.*
        * **ExecuteNonQuery** - *Executes a statement that returns nothing, usually modifying the database through inserts, updates, or deletes.  Will return number of rows affected unless SET ROWCOUNT=OFF is in the procedure body, in which case will return -1.*

    .. Note:: **DataSets** are discouraged as they tend to eat up a lot of memory resources, if you just need to read through data in a forward fashion, prefer **DataReaders** instead.
    
    The queries and/or update you perform can be either *stored procedures* or *text*.  Stored procedures are the preferred mechanism as they are less prone to SQL-injection attacks and 
    allow for fine-grained permissions at the database level.

    .. Note:: **NEVER** use dynamic SQL or text SQL commands, **ALWAYS** prefer stored procedures.  There should **NEVER** be a reason to use text over a stored procedure.
    
    For example, to call a stored procedure to get all account ids from the database, we could add a method to our DAO that calls::
    
            // ... All that jazz from the previous example ...

            // retrieve all account ids in the system
            public IEnumerable<string> GetAccountIds()
            {
                // note, it is better to store in a List/Dictionary/etc than a DataSet 
                // as it is lighter.  Also note its better to return an empty 
                // collection than a null one if no results were found.
                var results = new List<string>();

                // execute "sp_get_account_ids" from database as stored procedure, no params
                using (var reader = _database.ExecuteDataReader("sp_get_account_ids", 
                    CommandType.StoredProcedure))
                {
                    // note the using block, this ensures when we're done with the 
                    // reader the connection will get freed.
                    while(reader.Read())
                    {                    
                        results.Add(reader["account_id"].ToString());
                    }
                }

                return results;
            }            

    Only calls to **ExecuteDataReader(...)** need to be wrapped in a *using* block so that the connection is assured to be freed to the connection pool even in the 
    case of an exception being thrown.  All other forms of *Execute...(...)* do not need disposal and so should not be wrapped in a *using* block.
    
    Also note that because the *DatabaseUtility* class constructs connections and commands only at the time of the call to *Execute...(...)*, there are no
    thread safety issues.  Since there is no shared database connection or command, there is no chance of accessing them simultaneously in other threads, hence
    the inherent thread safety.
    
Using Parameters
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    Creating parameters adds a small wrinkle to the process.  Let's say we want to add a new member to this DAO to query the open order ids for a given account::
    
        // Queries for the open order ids given an account id
        public IEnumerable<string> GetOpenOrderIds(string accountId)
        {
            var results = new List<string>();

            // note that we create a new parameter set for each call...
            var parameters = _database.CreateParameterSet();
            parameters.Add("@account_id", DbType.String, accountId);

            using (var reader = _database.ExecuteDataReader("sp_get_open_orders_by_account", 
                CommandType.StoredProcedure, parameters))
            {
                while(reader.Read())
                {
                    results.Add(reader["order_id"].ToString());
                }
            }

            return results;
        }    
        
    You will notice that we call the **CreateParameterSet()** method off of the *DatabaseUtility* instance.  While we could pass in a *DbParameter[]*
    instead, this is not as safe or desirable because it would fail to mock correctly or switch providers correctly.
    
    You can construct a parameter set directly, but if you do so please use the provider type from your *DatabaseUtility* instance to ensure that provider
    switches and mocking work correctly::
    
            // note we are not hard coding provider type, but passing in from our instance
            var parameters = new ParameterSet(_database.ProviderFactory)
                                 {
                                     {"@account_id", DbType.String, accountId}
                                 };

    .. note:: **NEVER** set the provider type in the ParameterSet manually.
    
    Always use **CreateParameterSet()** from the *DatabaseUtility* instance, or pass in the *DatabaseUtility* instance's **ProviderFactory** 
    property to ParameterSet's constructor.  In both of these cases, you will ensure that if the *DatabaseUtility* provider changes, 
    the *ParameterSet* will change as well.

    You can also take advantage of implicit typing on the parameters.  Either the *Add()* call or initializer list above could have avoided the 
    *DbType.String* direct specification of the type and just passed the parameter name and the value.  In these cases, the type is inferred 
    from the value passed in.  When you are specifying input parameters, this is fine.  However, for output parameters, you must always specify a type.
    
    .. note:: **NEVER** reuse your parameters between commands, **ALWAYS** create a new set of parameters per call to *Execute...(...)*.
    
    This bears repeating: SqlParameters are strongly tied to the SqlCommand they are used with.  Once the SqlParameter is used in a command, the same 
    instance of that parameter cannot be re-used on another call to execute, it must be re-created each time.  Just get in the habit of creating 
    your parameter set before each call to Execute...(...) and you'll be fine.
    
Logging Long Queries
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    This really belongs under interceptors, and is explained more in detail there, however it is worth noting here as 
    well that the **TimeThresholdInterceptor** is extremely handy for logging long database query times.  To use this
    you need reference both the *Castle.Core* assembly and the *SharedAssemblies.General.Interceptors* assembly.
    
    This is a brief example, for more information, see the usage guide for the Interceptors::
    
            // create an interceptor around DatabaseUtility that logs calls over 5 seconds.
            IDatabaseUtility dbUtility = TimeThresholdInterceptor.Create<IDatabaseUtility>(
                new DatabaseUtility(ClientProviderType.SqlServer, "server=cgserver001..."),
                TimeSpan.FromSeconds(5));

            // this call to the wrapped DatabaseUtility will now log warnings when any
            // methods called on it exceed 5 seconds.  Useful for detecting long queries.
            dbUtility.ExecuteNonQuery("sp_some_proc", CommandType.StoredProcedure);      
    
Converting Results Using TypeConverter and Extensions
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    You'll notice that the example of getting results earlier just took a string value of the data.  This is really easy to do because all 
    DB primitive types support a ToString() to convert them to a string format.  However, if you want to get data from a database as an integer,
    you now have problems.  Consider this revision of the code to now get a list of account ids that is numeric instead of string::
    
        public IEnumerable<int> GetAccountIds()
        {
            // now a list of int
            var results = new List<int>();

            using (var reader = _database.ExecuteDataReader("sp_get_account_ids", 
                CommandType.StoredProcedure))
            {
                while(reader.Read())
                {                
                    // cast the column type to int
                    results.Add((int)reader["account_id"]);
                }
            }

            return results;
        }        
    
    Will this work?  The answer is a big, resounding, **MAYBE**.  The reason for the uncertainty is that remember that reader[column] returns
    an **object**, and casting an object to anything is not a conversion, but a cast.  Thus the only way this code will succeed is if the 
    database column returned as "account_id" in the stored procedure is the SQL equivalent of type DbType.Int32.  If it's a string, a long, 
    a short, a numeric, a decimal, etc, it will always fail because the cast from object requires it to be the exact type.
    
    .. note:: Casting a column from a DataReader or DataSet must be the **exact** type or it will fail even if a conversion is possible.  Use **TypeConverter** or **ObjectExtensions** instead.
    
    So what do we do?  We don't want to strongly tie our types if possible.  And even if we do, what happens if the column is null?  In this case it will return **DBNull.Value**
    which will definitely NOT match the type.
    
    .. note:: A null column value does not return a null reference, but instead returns **DBNull.Value**.
    
    The answer then is to use the **TypeConverter** or **ObjectExtensions**
    extension method set from the **SharedAssemblies.Core** library.  These allow you to convert any object to any other type as long as a conversion
    exists, and they handle **null** and **DBNull.Value** values correctly.
    
    Instead of the cast, you can use this::

        // convert the column to type int, use -1 if null or DBNull.Value
        results.Add(TypeConverter.ToType<int>(reader["account_id"], -1);
    
    Or better yet, this::
    
        // convert the column to type int, use -1 if null or DBNull.Value
        results.Add(reader["account_id"].ToType<int>(-1));
                        
    The extension syntax is much easier and more fluent to use, you need just add the **SharedAssemblies.Core.Extensions** namespace to your using directives.
    
Mocking Results For Unit Tests
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    While *DatabaseUtility* implements **IDatabaseUtility**, you do not need to mock DatabaseUtility directly.  You may, if you wish, of course, but
    it is unnecessary as *DatabaseUtility* has an extremely robust mocking framework already built into it so that any use of it can be 
    easily mocked with mock results without having to write a bunch of mocking code.
    
    This is the major advantage of this library: the built-in support for mocking results.  This is because inside the library there is a definition of a complete 
    **DbProviderFactory** hierarchy that has mock versions of all db components such as DbConnection, DbCommand, DbParameter, DbDataReader, etc.  Each of these
    mock versions behave like their Sql and Oracle counterparts except that they return canned data queued up by the user. 
    
    To use the mock facilities to unit test code business logic, you need two things:
    
        * **ProviderType must be injected** - *This is the best and cleanest way to enable your code to be fully unit-tested as it allows switching to the mock easily.*
        * **Mock CommandResults must be added** - *For each database command the test is to invoke, an appropriate set of mock results must be created.*
        
    The first of these is easy and straightforward, consider our DAO example from above::
    
        // A theoretical DAO that would return open orders for an account
        public class OpenOrderDao
        {
            // instance of the database utility
            private DatabaseUtility _database; 

            // It is recommended you inject your provider type and connection 
            // string at construction.
            public OpenOrderDao(ClientProviderType providerType, string connectionString)
            {
                _database = new DatabaseUtility(providerType, connectionString);
            }

            // ... 
        }

    Here the ClientProviderType is provided at construction of the class, this allows the class to be unit tested with mock results
    and changed back to the actual provider type for real operation.  You could also alternatively use a property to inject the
    *ClientProviderType* or even a *DatabaseUtility* instance.  As a third method, you could use a config setting to specify the
    provider type, though this is the least recommended approach as classes should not configure themselves or they risk being
    too strongly coupled to an implementation.
    
    Once you have an injectable *ClientProviderType* by whatever means you devise, writing the unit tests becomes easy::
    
        [TestClass]
        public class OpenOrderDaoTest
        {
            // context, part of MSTest
            public TestContext TestContext { get; set; }
            
            // a sample test to invoke our business logic with a mock provider
            [TestMethod]
            public void GetAccountIds_ReturnsEmptyList_IfNoResults()
            {            
                // the important part is passing in the mock provider type, 
                // the connection string is somewhat irrelevant
                var testDao = new OpenOrderDao(ClientProviderType.Mock, 
                    "some connection string");
                
                // set up mock results expected from db
                ...
                
                // invoke the dao, this calls the DatabaseUtility through 
                // the Mock provider type.
                var actual = testDao.GetAccountIds(); 
                
                Assert.AreEqual(actual.Count, 0);
            }
        }
        
    Easy!  Notice that the only thing you did to specify this is now mocked vs SQL Server or some other provider was to 
    change the *ClientProviderType* passed in.  We still need to set up some mock results, but notice that our
    business logic did not have to change at all to accommodate mock data, and now we can test a wide range of circumstances
    including:
    
        * What happens when no results are returned?
        * What happens when too many results are returned?
        * What happens when a column isn't returned?
        * What happens if an exception is thrown?
        * And many more...
    
    Basically, anything a database can do, you can configure the Mock provider to do.  It can throw on Open, on Commit, on Rollback, or on
    Execute.  It can return scalars, DataSets, DataReaders, and rows affected.  It can even queue up multiple sets of results to be
    returned on the same or different connection/command pairs::
    
            // create the results you expect for the command
            MockCommandResults mockResults = new MockCommandResults();

            // in this case, our command is going to return 1 result set containing 5 rows
            mockResults.ResultSet = new DataTable[1];

            // must define columns and rows
            mockResults.ResultSet[0].Columns.Add("account_id", typeof (string));
            mockResults.ResultSet[0].Rows.Add("11111111");
            mockResults.ResultSet[0].Rows.Add("11111112");
            mockResults.ResultSet[0].Rows.Add("11111113");
            mockResults.ResultSet[0].Rows.Add("11111114");
            mockResults.ResultSet[0].Rows.Add("11111115");

            // add to the mock factory for all connections (first null), all commands (second null)
            MockClientFactory.Instance[null][null].Enqueue(mockResults);

    There are a few things of note here.  First of all, you must specify your columns along with your rows (so it knows what column names to return).
    Second, you have the option of specifying a specific connection/command to apply them to, or all (99.9% of the time, you can just use [null][null] to default to all).  
    Finally, note that these results are **enqueued**, this means once they are returned, they are consumed and removed.  This is so you can queue up multiple results in a row to 
    simulate a piece of business logic that may call a query in a loop.
    
    You can also return mock scalars or rows affected, these would be returned by calls to *ExecuteScalar(...)* and *ExecuteNonQuery(...)* respectively::
    
        // this might be your mock result for a query that gets a max value from a column
        MockClientFactory.Instance[null][null].Enqueue(
            new MockCommandResults { ScalarResult = 35.7 });
        
        // or this might be your mock result for an update that indicates 5 rows were affected
        MockClientFactory.Instance[null][null].Enqueue(
            new MockCommandResults { RowsAffected = 5 });
        
    And, you can also specify whether a connection or command should throw::
    
        // tell your mock that it should throw when the connection indicated is opened.
        MockClientFactory.Instance[null].ShouldMockConnectionThrowOnOpen = true;
        
        // tell your mock command results that the Execute should throw 
        MockClientFactory.Instance[null][null].Enqueue(
            new MockCommandResults
                {
                    ShouldMockCommandThrowOnExecute = true
                });
    
Specifying Different Results per Command or Connection
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    The examples above use null in the indexers will give you the default connection and or command.  
    However, there is a mnemonic property as well if you prefer.  These
    two forms are absolutely identical::
    
        // the indexers using null...
        MockClientFactory.Instance[null][null].Enqueue(...)
        
        // are the same as the properties below...
        MockClientFactory.ConnectionResults.CommandResults.Enqueue(...);
        
        // or mix and match, still the same...
        MockClientFactory[null].CommandResults.Enqueue(...);
        MockClientFactory.ConnectionResults[null].Enqueue(...);
                
    This does not mean that a default connection string implies that the command string must be default.  Take these examples::

        // this will apply the mock results for any command, any connection
        MockClientFactory[null][null].Enqueue(...);
        
        // this will apply the mock results for the command 
        // "sp_get_orders" on any connection
        MockClientFactory[null]["sp_get_orders"].Enqueue(...);
        
        // this will apply the mock results for any command on the 
        // connection "server=cgsomedb001,..."
        MockClientFactory["server=cgsomedb001,..."][null].Enqueue(...);
    
    .. Note:: Nearly all the time, you can safely specify [null][null] because most of the time a piece of business logic is only executing against one command, one connection.  The capability was added, though, for those pieces of business logic that query multiple databases or commands that can't be unit tested separately.

Classes
--------------------------------------------------------------

The following are a summary of the key classes in the SharedAssemblies.General.Database library.

.. class:: DatabaseUtility

.. index::
    pair: Database; Utility

The **DatabaseUtility** is the core class for running database commands in a simple, thread-safe, and leak-free manner.  *DatabaseUtility* implements
*IDatabaseUtility* which enables it to be decorated easily.  It does not need to be mocked, itself, as it contains its own mocking framework.

    .. attribute:: DatabaseUtility.ProviderFactory
    
        
        :returns: DbProviderFactory instance of the abstract factory in use.
        :rtype: DbProviderFactory
        
        This attribute is used to return the current provider factory that was instantiated for this database utility's instance.
        
    .. attribute:: DatabaseUtility.ParameterFactory 
    
        :returns: ParameterFactory that creates parameters for the database utility's provider.
        :rtype: ParameterFactory
        
        
        This attribute is used to return the current parameter factory used to generate parameter sets.
        
    .. attribute:: DatabaseUtility.ConnectionString
    
        :returns: The connection string used to instantiate this instance.
        :rtype: string
        
        
        This attribute can be used to get the current connection string of this database utility instance.
        
    .. method:: DatabaseUtility.CreateParameterSet()
    
        :returns: ParameterSet of the same type as the database utility's provider type.
        :rtype: ParameterSet
        
        
        This is one of the preferred ways to create new parameter sets.  It is safe since it will always return a parameter set that is appropriate for
        the provider of this database instance.
        
    .. method:: DatabaseUtility.ExecuteDataSet(commandText, commandType[, parameters][, setToFill])
    
        :param commandText: the stored procedure or sql text to execute
        :type commandText: string
        :param commandType: indicates if commandText is an inline query or a stored procedure.
        :type commandType: CommandType
        :param parameters: a collection of parameter to use in the query or stored procedure.
        :type parameters: ParameterSet
        :param setToFill: an existing data set to load/update results into.
        :type setToFill: DataSet
        :returns: The filled DataSet with the command results.
        :rtype: DataSet
        
        
        This method is used when you want to return an in-memory collection of DataTables mirroring the result sets.  These are very memory
        intensive and should be used with care.  In general, if you only need a subset of the results or need to forward iterate, it is better
        to use *ExecuteDataReader(...)*.
        
    .. method:: DatabaseUtility.ExecuteDataReader(commandText, commandType[, parameters])
    
        :param commandText: the stored procedure or sql text to execute
        :type commandText: string 
        :param commandType: indicates if commandText is an inline query or a stored procedure.
        :type commandType: CommandType 
        :param parameters: a collection of parameter to use in the query or stored procedure.
        :type parameters: ParameterSet 
        :returns: A provider-specific reader that will iterate over the results.
        :rtype: IDataReader 
        
        
        This method is used when you want to process query results in a very efficient, forward-only approach.  For the most part,
        this should be the method of choice when querying data, and then either process items one-by-one or load into a slimmer structure
        such as a List<T>.  
        
        Remember that IDataReader is disposable and must be cleaned up promptly to avoid holding a connection longer than necessary.  Thus it is
        always best to wrap calls to *ExecuteDataReader(...)* in a using block::
        
            using(var reader = myDbUtil.ExecuteDataReader(...))
            {
                ... // process results ...
            }
    
    .. method:: DatabaseUtility.ExecuteScalar(commandText, commandType[, parameters])
    
        :param commandText: the stored procedure or sql text to execute
        :type commandText: string 
        :param commandType: indicates if commandText is an inline query or a stored procedure.
        :type commandType: CommandType 
        :param parameters: a collection of parameter to use in the query or stored procedure.
        :type parameters: ParameterSet 
        :returns: A single value from row 1, column 1 of the results.
        :rtype: object 
        
        
        This method is used when you only care about a single value returned from a query.  Note that this value must
        be actually in the result set of the query and not a stored procedure's return value, which is considered a parameter.
        
    .. method:: DatabaseUtility.ExecuteNonQuery(commandText, commandType[, parameters])
    
        :param commandText: the stored procedure or sql text to execute
        :type commandText: string
        :param commandType: indicates if commandText is an inline query or a stored procedure.
        :type commandType: CommandType
        :param parameters: a collection of parameter to use in the query or stored procedure.
        :type parameters: ParameterSet
        :returns: Number of rows affected by the command.
        :rtype: int
        
        
        This method is used when you need to execute a non-query command that inserts, updates, or deletes rows in the database.  It
        returns the number of rows affected by the command or stored procedure, however if ROWCOUNT is off in the database or turned off 
        in the stored procedure, it will always return -1.
        
.. class:: ParameterSet

.. index::
    pair: Parameter; Database

The **ParameterSet** is a generic collection of *DbParameter* tied to a specific client provider type.  It allows adding parameters in a 
provider-neutral way to allow for maximum re-usability and unit-testability.

    .. attribute:: ParameterSet.ProviderFactory
    
        :returns: A DbProviderFactory instance of the abstract factory in use.
        :rtype: DbProviderFactory 
        
        
        This attribute is used to return the current provider factory that was instantiated for this *ParameterSet* instance.
        
    .. attribute:: ParameterSet.ParameterFactory
    
        :returns: A ParameterFactory that creates parameters for the database utility's provider.
        :rtype: ParameterFactory 
        
        
        This attribute is used to return the current parameter factory used by this *ParameterSet* instance.
        
    .. attribute:: ParameterSet.Count
    
        :returns: The number of parameters in the set.
        :rtype: int
        
        
        Returns the number of parameters currently in the *ParameterSet*.
        
    .. method:: ParameterSet.Item(index | parameterName)
    
        :param index: An integer index of the parameter to return or set.
        :type index: int
        :param parameterName: The name of the parameter to return or set.
        :type parameterName: string
        :returns: The DbParameter referenced by the indexer.
        :rtype: DbParameter
        
        
        Item is the .Net name for the indexer ([]) operator.  This is the property for the *ParameterSet* that will return the parameter specified by name or by index.  In C# it is accessed using the 
        square brackets [] but in VB.Net, it is accessed through the Item property::
        
            DbParameter accountParameter = myParameterSet["@account_id"];
        
    .. method:: ParameterSet.Add(name[, type][, direction][, value])
    
        :param name: the name of the parameter as indicated in the database command.
        :type name: string
        :param type: the database type of the parameter as indicated in the database command.
        :type type: DbType
        :param direction: whether the parameter is input or output or a return value.
        :type direction: ParameterDirection
        :param value: the value to give the input parameter.
        :type value: object
        :returns: A provider-specific DbParameter instance based on type of the ParameterSet.
        :rtype: DbParameter
        
        
        This method is used to add a parameter to a *ParameterSet*.  You must at least specify the parameter name, and all other 
        values can be inferred:
        
            * **type** is inferred from **value** if specified.
            * **direction** is assumed to be Input unless otherwise specified.
            * **value** is assumed to be *DBNull.Value* unless specified.
        
    .. method:: ParameterSet.ToArray()
    
        :returns: An array of DbParameter references in the *ParameterSet*.
        :rtype: DbParameter[]
        
        
        This method is used to convert the *ParameterSet* to an array of *DbParameter*.
        
    .. method:: ParameterSet.GetEnumerator()
    
        :returns: An enumerator over the collection of parameters.
        :rtype: IEnumerator 
        
        
        This method is used to return the enumerator for processing in a *foreach* loop or with initializer lists.  Can also be
        invoked directly if desired.
        
.. class:: MockClientFactory

.. index::
    pair: Database; MockClientFactory

The **MockClientFactory** is the mock implementation of ADO.Net's *DbProviderFactory*.  It is a concrete implementation of the
abstract factory pattern that returns all Mock instances.  This is the provider factory that will be chosen when the 
*DatabaseUtility* is constructed with a *ClientProviderType.Mock*.

These mock types can be used either from *DatabaseUtility* or even directly as long as the code to be mocked uses the common ADO framework.

    .. attribute:: MockClientFactory.ConnectionResults
    
        :returns: The mock connection results for the default connection.
        :rtype: MockConnectionResults 
        
        
        This attribute is a more mnemonic way of calling the indexer with the null value.  It returns the connection results for
        any mock request whose connection string is not currently in the dictionary.
        
    .. attribute:: MockClientFactory.Instance
    
        :returns: the Singleton instance of this class.
        :rtype: MockClientFactory 
        
    
        Returns the static instance of this Singleton.
        
    .. attribute:: MockClientFactory.ConnectionResultsMap
    
        :returns: A dictionary of connection results for each connection string.
        :rtype: Dictionary<string, MockConnectionResults>
        
        
        This attribute returns the current list of all *MockConnectionResults* for each connection string.

    .. method:: MockClientFactory.Item(string connectionString)
    
        :param connectionString: the connection string to set or get results from.
        :type connectionString: string
        :returns: The mock connection results for the connection string provided.
        :rtype: MockConnectionResults
        
        Item is the .Net name for the indexer ([]) operator.  This attribute returns the *MockConnectionResults* that apply to the specified connection string.  If the connection string specified
        is *null*, then the default *MockConnectionResults* instance is returned, which will apply to all unlisted connection strings::
        
            // get results specific for connection string "cgsomerserver001..."
            MockConnectionResults someServerResults = MockClientFactory.Instance["server=cgsomeserver001..."];
            
            // get default results
            MockConnectionResults defaultResults = MockClientFactory.Instance[null];
        
    .. method:: MockClientFactory.CreateCommand()
    
        :returns: A mock implementation of *DbCommand*.
        :rtype: DbCommand
        
        
        Returns a mock implementation of the *DbCommand* abstract type.
    
    .. method:: MockClientFactory.CreateConnection()
    
        :returns: A mock implementation of *DbConnection*.
        :rtype: DbConnection
        
        
        Returns a mock implementation of the *DbConnection* abstract type.
        
    .. method:: MockClientFactory.CreateDataAdapter()
    
        :returns: A mock implementation of *DbDataAdapter*.
        :rtype: DbDataAdapter
        
        
        Returns a mock implementation of the *DbDataAdapter* abstract type.
    
    .. method:: MockClientFactory.CreateParameter()
    
        :returns: A mock implementation of *DbParameter*.
        :rtype: DbParameter
        
        
        Returns a mock implementation of the *DbParameter* abstract type.
    
    .. method:: MockClientFactory.CreateConnectionStringBuilder()
    
        :returns: A mock implementation of *DbConnectionStringBuilder*.
        :rtype: DbConnectionStringBuilder
        
        
        Returns a mock implementation of the *DbConnectionStringBuilder* abstract type.
    
    .. method:: ResetMockResults()
    
        :returns: nothing
        :rtype: void
        
    
        This method is useful for clearing all mock results so you can start each unit test fresh.
        
.. class:: MockConnectionResults

.. index::
    pair: Database; MockConnectionResults

The **MockConnectionResults** is a collection of results that will be returned when actions are executed against the specified connection.

    .. attribute:: MockConnectionResults.CommandResults
    
        :returns: A queue of mock command results for the default command.
        :rtype: Queue<MockCommandResults> 
        
        
        This attribute is a more mnemonic way of calling the indexer with the null value.  It returns the command results for
        any mock request whose command string is not currently in the dictionary.
        
    .. attribute:: MockConnectionResults.ShouldMockConnectionThrowOnOpen
    
        :returns: Whether or not mock connection will throw on open.
        :rtype: bool
        
        
        This attribute allows you to specify if the connection should throw an exception on a call to *Open()*.
        
    .. attribute:: MockConnectionResults.ShouldMockTransactionThrowOnCommit
    
        :returns: Whether any transaction should throw on commit.
        :rtype: bool
        
        
        This attribute allows you to specify if any transaction on this connection should throw an exception when *Commit()* is called.
        
    .. attribute:: MockConnectionResults.ShouldMockTransactionThrowOnRollback
    
        :returns: Whether any transaction should throw on rollback.
        :rtype: bool
        
        
        This attribute allows you to specify if any transaction on this connection should throw an exception when *Rollback()* is called.
        
    .. method:: MockConnectionResults.Item(commandString)
    
        :param commandString: the command string to set or get results from.
        :type commandString: string
        :returns: A queue of mock command results for the command string provided.
        :rtype: Queue<MockCommandResults> 
        
        
        This is the .Net index operator ([]).  This attribute returns the *MockCommandResults* that apply to the specified command string.  If the command string specified
        is *null*, then the default *MockCommandResults* instance is returned, which will apply to all unlisted command strings::
        
            // get the command results specific for command sp_get_all_accounts
            MockCommandResults someServerResults = MockClientFactory.Instance[null]["sp_get_all_accounts"];
            
            // get default results on the default connection
            MockCommandResults defaultResults = MockClientFactory.Instance.[null][null];
        
.. class:: MockCommandResults

.. index::
    pair: Database; MockCommandResults

The **MockCommandResults** is a collection of results for a particular command in the parent *MockConnectionResults*.

    .. attribute:: MockCommandResults.ShouldMockCommandThrowOnExecute
    
        :returns: True if mock command should throw an exception at execution.
        :rtype: bool
        
        
        If this attribute is true, the mock command will throw an exception whenever an *Execute...(...)* method is called.
        
    .. attribute:: MockCommandResults.ResultSet
    
        :returns: Array of results of a mock query
        :rtype: DataTable[]
        
        
        This property is used to specify the multiple result sets that may be returned by a call to *ExecuteDataSet(...)* or *ExecuteDataReader(...)*.  
        Remember to fill in both rows and columns.
        
    .. attribute:: MockCommandResults.RowsAffectedResult
    
        :returns: The number of rows affected
        :rtype: int
        
        
        This property sets the number of rows affected that will be returned by the mock command when executed via *ExecuteNonQuery(...)*.
        
    .. attribute:: MockCommandResults.ScalarResult
    
        :returns: The scalar value to return
        :rtype: object
        
        
        This property sets the scalar result to be returned when a mock command is executed via *ExecuteScalar(...)*.
        
For more information, see the `API Reference <../../../../Api/index.html>`_.        
    
    
        
        
        

