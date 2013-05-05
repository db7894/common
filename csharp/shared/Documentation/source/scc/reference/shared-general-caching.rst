=========================================================================
Caching --- General Cache Class and Strategies
=========================================================================
:Assembly: SharedAssemblies.General.Caching.dll
:Namespace: SharedAssemblies.General.Caching
:Author: Bob Cunningham (`bcunningham@bashwork.com <mailto:bcunningham@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.General.Caching
   :platform: Windows, .Net
   :synopsis: Caching - General Cache Class and Strategies

.. highlight:: csharp

.. index:: Cache

.. index::
    pair: Caching; Strategy

Introduction
------------------------------------------------------------

The purpose of the **SharedAssemblies.General.Caching** library is to provide a generic, thread-safe cache
for applications to use that supports readers and writers efficiently and effectively.

The **Cache** class is the primary class of the assembly and is used to store and retrieve items.  You may
specify different cache expiration strategies that determine how long an item stays in the cache before it is
considered *expired* and no longer valid.

There are three canned strategies that come with the Caching library which include:

    * **CacheExpirationStrategy** - *Takes any user-defined predicate to determine if an item is expired*.
    * **DifferentDayExpirationStrategy** - *Marks an item as expired if it is accessed on a different day than it was added to the cache*.
    * **TimeSpanExpirationStrategy** - *Marks an item as expired if it has been in the cache for a given TimeSpan*.

The predicate gives you great flexibility in designing your own expiration strategy without needing to create a class.  However,
if a sufficiently complex expiration strategy is needed, you simply need to create a class that implements **ICacheExpirationStrategy** and
supply that to the constructor of the Cache.

You may also specify an interval that the Cache is cleaned up.  Note that even though items may expire due to their strategy, that does not
necessarily mean that they will be immediately removed.  They will be marked as expired and then cleaned up at the next cleanup cycle (the default is *1 hour*).
If an item is in the Cache and is marked expired, it will be returned on calls to **Get(...)**, but not on calls to **GetValidValue(...)**.

Usage
---------------------------------------------------------------

To use the **Cache**, you simply instantiate it with your chosen expiration strategy (the default is that items never expire) and your cleanup interval (the 
default is one hour).  

Let's assume that you have some accounts that you want to cache to prevent having to reload them if they are accessed repetitively.  You may have a 
class such as this::

    public class Account
    {
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        // ...
    }
    
Now, if you wanted to create a *Cache* of *Account* based on the *string* *AccountId*.  You would declare the key as type *string* and the value as type
*Account*.  You could then choose your expiration strategy from one of the canned or default ones::

    // this is a cache that never expires and never automatically cleans up 
    var nonExpiringCache = new Cache<string, Account>();

    // this is a cache that expires items when the date changes and cleans up every hour
    var nextDayExpiringCache = new Cache<string, Account>(
        new DifferentDayExpirationStrategy<Account>());

    // this is a cache that expires items after 1 hour and cleans up every hour
    var timeSpanExpiringCache = new Cache<string, Account>(
        new TimeSpanExpirationStrategy<Account>(TimeSpan.FromHours(1)));

Note that these are just the canned strategies, you can also apply your own using predicates or predicate-style lambdas::

    // expires an item in the cache if the name is ever null.
    var noNameExpiration = new Cache<string, Account>(item => item.Value.Name == null);

.. Note:: 
    If you pass a strategy in the constructor, the default automatic cleanup time is **1 hour**.  
    However if you do not specify a strategy, the default automatic cleanup is **never**.

Optionally, you can specify alternate automatic cleanup times on constructors that take strategies::

    // this is a cache that expires items daily and automatically cleans up every 5 minutes.
    var nextDayExpiringCache = new Cache<string, Account>(
        new DifferentDayExpirationStrategy<Account>(), 
            TimeSpan.FromMinutes(5));

    // this is a cache that expires items after 1 hour and cleans up every 30 minutes.
    var timeSpanExpiringCache = new Cache<string, Account>(
        new TimeSpanExpirationStrategy<Account>(new TimeSpan(0, 1, 0)), 
            TimeSpan.FromMinutes(30));
            
    // this cache expires items after 15 minutes, but no automatic clean-up
    var fiveMinuteExpireManualCache = new Cache<string, Account>(
        new TimeSpanExpirationStrategy<Account>(TimeSpan.FromMinutes(15)),
            new TimeSpan(0));
            
To specify that your strategy should never automatically clean-up, you can specify a **new TimeSpan(0)** which is 
zero ticks.  Any timespan of zero or less means that the automatic timespan never happens and you must kick off
cleanup manually, if desired.

Once your Cache is instantiated, all you need do is add items to it and get them out.  There are three types of gets:

    * **Get(...)** - *Retrieves the value in a CachedItem<T> wrapper.*
    * **GetValue(...)*** - *Retrieves the value in the cache even if expired but not cleaned up.*
    * **GetValidValue(...)*** - *Retrieves the value only if not expired.*
    
Most of the time, you will always use **GetValidValue(...)** unless there is some overriding reason why
you would need an item that has already expired.

The main usage of the cache follows the following steps:

    1. Check cache for value, if value exists return the result
    2. If value not in cache, load it from data source
    3. If load was successful, add to cache and return the result
    
Here is an example of using the Cache in a decorator.  Writing a Dao is very straightforward,
however a typical Dao will always load the item even if the item never changes.  It is easy
to abstract an interface from our Dao (using Resharper or any refactoring tool).  Using that
interface, create a decorator that wraps the DAO and adds caching to it while still satisfying
the same interface.  This is a standard use of the decorator pattern and allows you to use the DAO
either cached or uncached without needing to change the caller::

    /// <summary>
    /// A theoretical DAO that would return open orders for an account
    /// </summary>
    public class CachedAccountDaoDecorator : IAccountDao
    {
        // member for cache of accounts - accounts expire when in cache for 5 min
        private readonly Cache<string, Account> _accountCache = new Cache<string, Account>(
            new TimeSpanExpirationStrategy<Account>(TimeSpan.FromMinutes(5)));

        // the dao that will load the data if not in cache
        private readonly IAccountDao _accountDao;


        /// <summary>
        /// Wrap the account dao with this cache adapter
        /// </summary>
        public CachedAccountDaoDecorator(IAccountDao wrappedDao)
        {
            _accountDao = wrappedDao;
        }


        /// <summary>
        /// We are a dao decorator that checks our cache first, and then if not in the
        /// cache load from the wrapped dao .
        /// </summary>
        public Account GetAccount(string accountId)
        {
            // check cache for existence of item
            Account result = _accountCache.GetValidValue(accountId);
            
            // if in cache, return it, but if not, get from dao
            if(result != null)
            {
                // load account from wrapped dao
                result = _accountDao.GetAccount(accountId);

                if(result != null)
                {
                    // when we get a good load, put it in the cache for next time
                    _accountCache.Add(accountId, result);
                }
            }

            return result;
        }
    }

For more information, see the `API Reference <../../../../Api/index.html>`_.