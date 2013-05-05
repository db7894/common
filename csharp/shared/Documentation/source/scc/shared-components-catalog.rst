=============================================
Shared Components Catalog
=============================================

The following is a brief summary of all of the public components that are part of the Shared
Components Libraries.  You may click on each of the library names to take you to the usage
guides for each library.

This is mainly a list of the primary classes, there are of course internal or minor public classes that
are either not visible or only used in conjunction of one of the major classes.  These are omitted for brevity.

.. note:: For each namespace listed below, the [*] refers to the assembly name.  For example, in the assembly *SharedAssemblies.Core.Dll*, if referring to [*].Containers this would mean the namespace **SharedAssemblies.Core.Containers**.

:doc:`Email <reference/shared-communication-email>`
------------------------------------------------------------------------------

Contains a simple SMTP email client.

**SharedAssemblies.Communication.Email.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | EmailClient                       | Simplifies process of sending emails   |
|                           |                                   | to an SMTP server.                     |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Core Generic Utilities <reference/Core/index>`
------------------------------------------------------------------------------

Contains new generic containers, type conversions and translations, extensions methods, design pattern implementations, and XML serialization helpers.
The **Core** assembly may be used by other shared assemblies.

**SharedAssemblies.Core.Dll**        
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Containers            | Range                             | A class for evaluating values vs. a    |
|                           |                                   | range.                                 |
|                           +-----------------------------------+----------------------------------------+
|                           | SerializableDictionary            | Wraps the standard Dictionary class so |
|                           |                                   | it can be serialized to XML or         |
|                           |                                   | de-serialized from XML.                |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Conversions           | Translator                        | Allows for easy translations from one  |
|                           |                                   | set of values to another.  Use this    |
|                           |                                   | class when the two types have no       |
|                           |                                   | algorithmic conversion and need to be  |
|                           |                                   | "mapped" from one value to another.    |
|                           +-----------------------------------+----------------------------------------+
|                           | TypeConverter                     | Allows for algorithmic conversions     |
|                           |                                   | between compatible types.  This is     |
|                           |                                   | much safer than casting and analyzes   |
|                           |                                   | the types in question to determine the |
|                           |                                   | best translation.  Much safer than     |
|                           |                                   | built-in casting, parsing, or          |
|                           |                                   | conversion.                            |
|                           |                                   | Checks for null and DBNull.Value and   |
|                           |                                   | allows default substitutions.          |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Extensions            | AssemblyExtensions                | Adds methods to Assembly that let you  |
|                           |                                   | easily query custom assembly           |
|                           |                                   | attributes.                            |
|                           +-----------------------------------+----------------------------------------+
|                           | ComparableExtensions              | Adds extensions to any IComparable<T>  |
|                           |                                   | to allow it to be compared to a        |
|                           |                                   | range of values.                       |
|                           +-----------------------------------+----------------------------------------+
|                           | ConvertibleExtensions             | Adds extensions to any IConvertible<T> |
|                           |                                   | that offers syntax shortcuts to the    |
|                           |                                   | *TypeConverter* class.                 |
|                           +-----------------------------------+----------------------------------------+
|                           | DateTimeExtensions                | Adds additional computations to        |
|                           |                                   | *DateTime* and a *ToString(...)*       |
|                           |                                   | overload for common serializations.    |
|                           +-----------------------------------+----------------------------------------+
|                           | EnumerableExtensions              | Adds extensions for new Linq-style     |
|                           |                                   | expressions from IEnumerable<T>        |
|                           +-----------------------------------+----------------------------------------+
|                           | IntExtensions                     | Adds extensions for number generation. |
|                           +-----------------------------------+----------------------------------------+
|                           | ListExtensions                    | Adds extensions for List<T> class      |
|                           |                                   | to simplify common empty/null checks.  |
|                           +-----------------------------------+----------------------------------------+
|                           | ObjectExtensions                  | Adds extensions to object to check     |
|                           |                                   | for default, null, DBNull.Value, and   |
|                           |                                   | syntax shortcuts to *TypeConverter*    |
|                           +-----------------------------------+----------------------------------------+
|                           | StringExtensions                  | Adds extensions to *string* type for   |
|                           |                                   | null/empty check, length check, masking|
|                           |                                   | repeating, and substitutions.          |
|                           +-----------------------------------+----------------------------------------+
|                           | TypeExtensions                    | Adds extension method to check if a    |
|                           |                                   | given type is a System.Nullable<>      |
|                           |                                   | wrapper of another type.               |
|                           +-----------------------------------+----------------------------------------+
|                           | XmlExtensions                     | Adds extension methods that are        |
|                           |                                   | syntax shortcuts to *XmlUtility*       |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Patterns              | Adapter                           | Allows easy, classless construction of |
|                           |                                   | adapters from one type to another by   |
|                           |                                   | providing mapping delegates.           |
|                           +-----------------------------------+----------------------------------------+
|                           | Factory                           | Allows dynamic construction of a type  |
|                           |                                   | from an assembly without needing to    |
|                           |                                   | know assembly and class at compilation.|
|                           +-----------------------------------+----------------------------------------+
|                           | Singleton                         | A lazy-instantiating Singleton         |
|                           |                                   | wrapper for any type that returns a    |
|                           |                                   | single instance of the type.           |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Xml                   | XmlUtility                        | Simplifies task of serializing and     |
|                           |                                   | deserializing objects to and from XML. |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Database <reference/shared-general-database>`
------------------------------------------------------------------------------

A simple, thread-safe, resource-leak-resistant, fully mock-able database utility.

**SharedAssemblies.General.Database.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | DatabaseUtility                   | Implements a thread-safe database      |
|                           |                                   | utility that can be fully mocked for   |
|                           |                                   | unit testing purposes.  It is also     |
|                           |                                   | completely provider neutral.           |
|                           +-----------------------------------+----------------------------------------+
|                           | ParameterSet                      | Used with *DatabaseUtility* as a way   |
|                           |                                   | to create implementation independent   |
|                           |                                   | parameter lists.                       |
|                           +-----------------------------------+----------------------------------------+
|                           | SqlServerServiceBrokerQueue       | Utility to simplify adding and removing|
|                           |                                   | items from a SQL Server Service        |
|                           |                                   | Broker Queue.                          |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Mock                  | MockXxx                           | Mock implementations of all ADO        |
|                           |                                   | artifacts, these can be used directly  |
|                           |                                   | though it is easier and preferred to   |
|                           |                                   | simply use the DatabaseUtility which   |
|                           |                                   | mocks for you.                         |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Unit Test Authoring <reference/shared-testing>`
------------------------------------------------------------------------------

Extensions and utility classes to assist in writing cleaner unit tests.

**SharedAssemblies.General.Testing.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | AssertExtensions                  | Static helper class which allows       | 
|                           |                                   | easier unit test checks for equality,  |
|                           |                                   | exceptions, etc.                       |
|                           +-----------------------------------+----------------------------------------+
|                           | GenericAssert                     | Static helper class with asserts       |
|                           |                                   | designed to operate over generics.     |
|                           +-----------------------------------+----------------------------------------+
|                           | Reflector                         | Static helper class to simplify        |
|                           |                                   | working with private data.             |
+---------------------------+-----------------------------------+----------------------------------------+


:doc:`Multi-Threading and Concurrency <reference/shared-general-threading>`
---------------------------------------------------------------------------------------

Utility classes to assist in multi-threading and concurrency.

**SharedAssemblies.General.Threading.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | Bucket                            | A thread-safe queue of items to        | 
|                           |                                   | process concurrently.  Has options     |
|                           |                                   | to retrieve one at a time or in batch. |
|                           +-----------------------------------+----------------------------------------+
|                           | BufferedBucketConsumer            | Takes items from bucket in batches but |
|                           |                                   | processes them one at a time.          |
|                           +-----------------------------------+----------------------------------------+
|                           | BulkBucketConsumer                | Takes items from bucket in batches and |
|                           |                                   | processes them as a whole.             |
|                           +-----------------------------------+----------------------------------------+
|                           | DiscreteBucketConsumer            | Takes items from bucket one at a time  |
|                           |                                   | and processes them one by one.         |
|                           +-----------------------------------+----------------------------------------+
|                           | TaskConsumer                      | Consumes actions (instead of items)    |
|                           |                                   | over a thread pool.                    |
|                           +-----------------------------------+----------------------------------------+
|                           | TaskRegistration                  | A task registered for execution in a   |
|                           |                                   | bucket.                                |
|                           +-----------------------------------+----------------------------------------+
|                           | TaskThreadPool                    | A pool of threads for executing tasks. |
|                           +-----------------------------------+----------------------------------------+
|                           | ThreadExtensions                  | Extension methods to add functionality |
|                           |                                   | to the Thread class, notably           |
|                           |                                   | JoinOrAbort(...) which attempts to     |
|                           |                                   | Join(...) for a certain time then      |
|                           |                                   | aborts thread if Join(...) fails.      |
+---------------------------+-----------------------------------+----------------------------------------+


:doc:`Log4Net Logging Aides <reference/shared-general-logging>`
------------------------------------------------------------------------------

Extension methods and helper classes for Log4Net.

**SharedAssemblies.General.Logging.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | BlockLogger                       | Logs entry and exit of a block of      | 
|                           |                                   | code using IDisposable paradigm as     |
|                           |                                   | well as the duration of the block.     |
|                           +-----------------------------------+----------------------------------------+
|                           | LogExtensions                     | Log4Net doesn't let you specify log    |
|                           |                                   | level at run-time, these extension     |
|                           |                                   | methods add that capability.           |
+---------------------------+-----------------------------------+----------------------------------------+


:doc:`Validation <reference/shared-validator>`
------------------------------------------------------------------------------

Field-level validations and request validations.

**SharedAssemblies.General.Validations.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | RequestValidator                  | Takes any type of "request" object     | 
|                           |                                   | and runs a series of tests against it  |
|                           |                                   | to determine if it passes all tests.   |
|                           +-----------------------------------+----------------------------------------+
|                           | Validator                         | Validates a string against many        |
|                           |                                   | standard formats including SSN, phone  |
|                           |                                   | number, address, IP, etc.              |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Interceptor Decorators <reference/shared-interceptors>`
------------------------------------------------------------------------------

Dynamic decorator classes that add functionality to interfaces and super-classes with virtual methods.
Uses the *Castle.Core.Dll* and *SharedAssemblies.General.Logging* and *log4net* assemblies.

**SharedAssemblies.General.Interceptors.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | BlockingInterceptor               | Blocks the call to all methods in the  | 
|                           |                                   | wrapped class, returning a default     |
|                           |                                   | return value.                          |
|                           +-----------------------------------+----------------------------------------+
|                           | LoggingInterceptor                | Wraps any class's methods and logs     | 
|                           |                                   | the input parameters and return value  |
|                           |                                   | to the log4net log at the level        |
|                           |                                   | specified.                             |
|                           +-----------------------------------+----------------------------------------+
|                           | ThrowingInterceptor               | Wraps any class's methods and throws   | 
|                           |                                   | an exception whenever any method of the|
|                           |                                   | wrapped class is called.               |
|                           +-----------------------------------+----------------------------------------+
|                           | TimeThresholdInterceptor          | Wraps any class's methods and times    | 
|                           |                                   | the call of the wrapped method.  If the|
|                           |                                   | time to execute the message surpasses  |
|                           |                                   | the specified duration, it will log the|
|                           |                                   | warning at the specified logging level.|
|                           +-----------------------------------+----------------------------------------+
|                           | TimingInterceptor                 | Wraps any class's methods with calls   | 
|                           |                                   | to a *BlockLogger* to log entry, exit, |
|                           |                                   | and duration of method calls.          |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Caching <reference/shared-general-caching>`
------------------------------------------------------------------------------

The caching assembly provides an easy-to-use thread-safe cache that allows you to specify different
cache expiration strategies.

**SharedAssemblies.General.Caching.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | Cache                             | The class that implements the thread   | 
|                           |                                   | safe cache.  On construction you can   |
|                           |                                   | choose expiration strategy and cleanup.|
|                           +-----------------------------------+----------------------------------------+
|                           | CacheExpirationStrategy           | A generic expiration strategy that     | 
|                           |                                   | allows you to specify a delegate       |
|                           |                                   | for item expiration.                   |
|                           +-----------------------------------+----------------------------------------+
|                           | DifferentDayExpirationStrategy    | Cache expiration strategy that expires | 
|                           |                                   | an item at the end of the day.         |
|                           +-----------------------------------+----------------------------------------+
|                           | ICacheExpirationStrategy          | An interface for an expiration         | 
|                           |                                   | strategy.  You can either implement    |
|                           |                                   | this or just provide a delegate to     |
|                           |                                   | the *CacheExpirationStrategy*.         |
|                           +-----------------------------------+----------------------------------------+
|                           | TimeSpanExpirationStrategy        | Expires an item from the cache after   | 
|                           |                                   | it has lived in the cache for the      |
|                           |                                   | given timespan.                        |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Financial Business Logic <reference/shared-general-financial>`
------------------------------------------------------------------------------

A collection of common financial business logic classes.  Currently the only inhabitant is the     
*FinanceDateTime*.                        

**SharedAssemblies.General.Financial.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | FinanceDateTime                   | Wraps a *DateTime* instance and adds   | 
|                           |                                   | a complete set of market date and time |
|                           |                                   | calculations for market open, close,   |
|                           |                                   | holidays, half-holidays, etc.          |
+---------------------------+-----------------------------------+----------------------------------------+


:doc:`Monitoring and Instrumentation <reference/shared-monitoring-autocounters>`
------------------------------------------------------------------------------------------------

An assembly that contains the AutoCounter and supporting classes for instrumenting windows applications.

**SharedAssemblies.Monitoring.AutoCounters.Dll**
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | AutoCounter                       | Wraps a Windows PerformanceCounter     | 
|                           |                                   | which is used to instrument windows    |
|                           |                                   | or web applications.  These are the    |
|                           |                                   | Microsoft preferred way to track       |
|                           |                                   | application health and statistics.     |
|                           |                                   | With these you can track number of     |
|                           |                                   | hits, average time, rate per seconds,  |
|                           |                                   | etc.                                   |
|                           +-----------------------------------+----------------------------------------+
|                           | AutoCounterCache                  | Used to cache AutoCounter instances so | 
|                           |                                   | you can look them up by identifiers    |
|                           |                                   | instead of creating instances.         |
|                           +-----------------------------------+----------------------------------------+
|                           | AutoCounterCacheFactory           | Reads the assembly attributes in the   | 
|                           |                                   | calling assembly to load and create    |
|                           |                                   | an AutoCounterCache.                   |
|                           +-----------------------------------+----------------------------------------+
|                           | AutoCounterCollection             | Used to group a set of AutoCounters    | 
|                           |                                   | together so they're all incremented    |
|                           |                                   | at the same time.                      |
|                           +-----------------------------------+----------------------------------------+
|                           | InstrumentationBlock              | Returned by AutoCounterCache or an     | 
|                           |                                   | AutoCounter to delimit instrumentation |
|                           |                                   | areas with a *using* statement.        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Configuration         | AutoCounterAttribute              | Assembly level attribute that lets     | 
|                           |                                   | you configure a counter to be installed|
|                           |                                   | by the installutil and loaded by the   |
|                           |                                   | AutoCounterCacheFactory.               |
|                           +-----------------------------------+----------------------------------------+
|                           | AutoCounterCategoryAttrbute       | Assembly level attribute to instruct   | 
|                           |                                   | the installutil to create a performance|
|                           |                                   | counter category on the machine.       |
|                           +-----------------------------------+----------------------------------------+
|                           | AutoCounterCollectionAttribute    | Assembly level attribute to define     | 
|                           |                                   | a set of AutoCounters to be treated    |
|                           |                                   | as an AutoCounterCollection by the     |
|                           |                                   | AutoCounterCacheFactory.               |
|                           +-----------------------------------+----------------------------------------+
|                           | AutoHeartbeatAttribute            | Creates a specialized AutoCounter to   | 
|                           |                                   | track application heartbeats.  Also    |
|                           |                                   | starts a timer that pulses the counter |
|                           |                                   | at regular intervals.                  |
+---------------------------+-----------------------------------+----------------------------------------+

:doc:`Security <reference/shared-encryption>`            
------------------------------------------------------------------------------

An assembly that contains a collection of classes and utilities for common security processes.          

**SharedAssemblies.Security.Dll**                
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Authentication        | AuthenticationFactory             | Factory that creates various           | 
|                           |                                   | authentication implementations.        |
|                           +-----------------------------------+----------------------------------------+
|                           | AuthenticationManager             | Manager class for authentication that  | 
|                           |                                   | generates tokens and validates         |
|                           |                                   | sessions.                              |    
|                           +-----------------------------------+----------------------------------------+
|                           | ExternalAuthentication            | Authenticates whether an external      | 
|                           |                                   | user has access.                       |
|                           +-----------------------------------+----------------------------------------+
|                           | InternalAuthentication            | Authenticates whether an internal user | 
|                           |                                   | has access.                            |
|                           +-----------------------------------+----------------------------------------+
|                           | SessionTokenCache                 | Performs smart caching of session      | 
|                           |                                   | tokens for authentication schemes.     |
|                           +-----------------------------------+----------------------------------------+
|                           | TokenAuthentication               | Bashworkr specific authentication     | 
|                           |                                   | method that uses tokens to authenticate|
|                           |                                   | a user.                                |    
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Encryption            | AuthenticationEncryption          | Encryption for authentication tokens   | 
|                           |                                   | and session identifiers.               |
|                           +-----------------------------------+----------------------------------------+
|                           | EncryptionUtility                 | Base class to perform encryption on    | 
|                           |                                   | data using Bashwork security approved |
|                           |                                   | encryption schemes.                    |    
|                           +-----------------------------------+----------------------------------------+
|                           | HashUtility                       | Helper class for hashes.               | 
|                           +-----------------------------------+----------------------------------------+
|                           | HexUtility                        | Helper class to convert to and from    | 
|                           |                                   | hexadecimal strings.                   |
|                           +-----------------------------------+----------------------------------------+
|                           | KeyContainer                      | Manages keys during the encryption     | 
|                           |                                   | process.                               |
|                           +-----------------------------------+----------------------------------------+
|                           | SimpleEncyption                   | A very simple, lightweight encryption  | 
|                           |                                   | scheme.                                |
|                           +-----------------------------------+----------------------------------------+
|                           | WebConfigEncryption               | Configuration scheme using key         | 
|                           |                                   | components from a web config file.     |
|                           +-----------------------------------+----------------------------------------+
|                           | WebConfigMachineKey               | Configuration scheme using key         |
|                           |                                   | components from a machine config file. |
+---------------------------+-----------------------------------+----------------------------------------+
| [*].Nonce                 | SqlNonceDao                       | Creates a nonce (single use numeric    | 
|                           |                                   | token) from a SQL database.            |
|                           +-----------------------------------+----------------------------------------+
|                           | MockNonceDao                      | Mock of the NonceDao for unit testing. | 
|                           +-----------------------------------+----------------------------------------+
|                           | INonceDao                         | Interface for nonce DAO.  All Nonce    | 
|                           |                                   | DAOs implement this interface.         |
+---------------------------+-----------------------------------+----------------------------------------+


:doc:`MVP Framework <reference/shared-mvp>`
------------------------------------------------------

A framework that helps simplify construction of web content following the MVP pattern.                  

**SharedAssemblies.Web.Frameworks.Mvp.Dll**    
                                                                  
+---------------------------+-----------------------------------+----------------------------------------+
| **Namespace**             | **Type**                          | **Description**                        |
+---------------------------+-----------------------------------+----------------------------------------+
| [*]                       | Presenter                         | Base Presenter that all MVP Presenters | 
|                           |                                   | should inherit from.                   |
|                           +-----------------------------------+----------------------------------------+
|                           | ViewPage                          | The base View for web pages.           |
|                           +-----------------------------------+----------------------------------------+
|                           | ViewMasterPage                    | The base View for web master pages.    |
|                           +-----------------------------------+----------------------------------------+
|                           | ViewUserControl                   | The base View for web user controls.   |
+---------------------------+-----------------------------------+----------------------------------------+

