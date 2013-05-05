=================================================================
Data Access Object Standards
=================================================================
:Author: Galen Collins <gcollins@bashwork.com>
:Version: 1.0 11/02/09

.. highlightlang:: csharp
 
Introduction
-------------

*What follows are a set of guidelines that should be followed when creating
shared data access objects that will be used between applications.*

Before we get into detail, a few quick statements should be made.
First In order to decouple the implementations, all the DAOs must implement
an interface that is defined in the same namespace.  By doing this, users of
the DAO will be able to easily mock the dependency for testing and debugging.
Next, instead of attempting to roll your own, use the SharedAssemblies.General.DatabaseUtility
as it has received considerable review, testing, and production exposure. Finally,
it is Bashwork's database policy to only use stored procedures for database
operations.  This should be followed unless an explicit need is found for other
solutions (bulk inserts for example).

What Operations Should Be Supplied
----------------------------------------

In order to keep operations seperated into their logical domain, the DAO should
only expose the neccessary data operations: create, retrieve, update, and delete.
These are commonly referred to as CRUD operations. There are a few rules regarding
each method that must be followed so that the general interface of shared DAOs
will be consistent.  This allow all users of the DAO to make certain assumptions
about the underlying code and therefore use them in a consistent manner.

Create Operation
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Each DAO should have a single create operation if and only if new instances of
the given entity should be created in the database.  All create operations should
return a string that represents the unique identifier representing this instance.

Update Operation
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Each DAO should have a single update operation if and only if new instances of
the given entity should be allowed to be updated.  All update operations should
return a bool value that represents the result of the operation: true if the
update succeeded, false if it failed.

Return bool status result

Delete Operation
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Each DAO should have a one or more delete operations if and only if existing
instances of the given entity should be allowed to be deleted.  All delete
operations should return a bool value that represents the result of the
operation: true if the delete succeeded, false if it failed.

Retrive Operation
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Each DAO should have a one or more retrieve operations if and only if existing
instances of the given entity should be allowed to be retrieved. All retrieve
operations should return a single or collection of the requested entity populated
as much as required for the operation.

To simplify the interface name, we substitute the **retrieve** method name with **get**.
Continuing, since one can retrieve an entity by a number of methods (id, timestamp,
etc), the method of retrieval must be added to the retrieve name:

* GetById
* GetByTimeStamp

.. note:: Don't create retrieve operations for every property on the entity! 
   Use the YAGNI principal and only define the retrieve operations that you
   will need.

Exception Handling
----------------------------------------

In order to balance the objective of hiding concerns with that of not
silencing possibly informative exceptions, we have decided upon the following
scheme.  First, all calls to the database should be wrapped with a try/catch block.
Next, the function should capture the lowest exception involving the database call
and in the catch block, it should store the default return value (usually null).
Finally, the function should catch all Exceptions, log that they occurred, and wrap
their call stack in a application level exception that can be caught by preceding code.
In all the scheme should look like the following::

    /// <summary>
    /// A template for SharedAssemblies.DataAccess DAO method
    /// </summary>
    public string Retrieve(SomeEntity signature)
    {
        SomeEntity result = null;
    
        try
        {
            // ...the database operation...
        }
        catch (System.Exception ex)
        {
            _log.Error(SqlConnectionException, ex); // handle to logger is needed
            throw;
        }
        
        return result;
    }

In this way, we can effectively return a tri-state result to the higher level code:

* If the operation succeeded, the requested information will be returned
* If the operation failed, the resulting information will be null
* If there was an error in the operation, an exception will be thrown
 
Generic Template
----------------------------------------

In order to facilitate creating a DAO object, the following template might be of use:

.. literalinclude:: ../includes/standards/src/ExampleEntity.cs
.. literalinclude:: ../includes/standards/src/IExampleEntityDao.cs

.. literalinclude:: ../includes/standards/src/SqlExampleEntityDao.cs
.. literalinclude:: ../includes/standards/src/ExampleEntityUtility.cs

Further Reading
----------------------------------------

* `Wikipedia Entry on DDD <http://en.wikipedia.org/wiki/Domain-driven_design>`_
* `Domain Driven Design Portal <http://domaindrivendesign.org/>`_

