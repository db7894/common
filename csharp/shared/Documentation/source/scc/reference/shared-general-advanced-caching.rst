=========================================================================
Advanced Caching --- Advanced Cache Class and Strategies
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

================================================================================
 Cache Interfaces
================================================================================

--------------------------------------------------------------------------------
 ICache
--------------------------------------------------------------------------------

There are a number of methods in the interface for the cache, however it is
still quite simple.  The get method simply returns the cached value by key and is
what you will use most of the time.  The add methods are there to add values which
is mainly used for the check and set cache and the specific caches. It is
overloaded because different cache implementations can make use of the lazy
behavior to save some processing time.

The cleaning functions work as follows: clean removes all the expired entries
while clear removes everything in the cache::

		int Count { get; }
		bool Clean();
		bool Clear();
		TValue Get(TKey key);
		void Add(TKey key, TValue value);
		void Add(TKey key, Func<TKey, TValue> callback);
		void Add(TKey key, TValue value, Func<TKey, TValue> callback);

--------------------------------------------------------------------------------
 IObjectCache
--------------------------------------------------------------------------------

The interface for the object cache is quite simple. There is a method to get the
current cached value and a method to force a refresh::

    bool Refresh();
    TValue Value { get; }

================================================================================
 Cache Types
================================================================================

In order to provide the best tool for the job, the library contains a number of
caches that are aimed at solving a particular problem.  What follows is a brief
summary of each cache and what it is used for.

--------------------------------------------------------------------------------
 GenericOnDemandRefreshCache
--------------------------------------------------------------------------------

The point of this cache is to supply the backing refresh method to the cache
such that attempting to pull a value from the cache that is not present or is
expired will automatically generate the given value and then insert it into
the cache.

It should be noted that every missed lookup will use the same factory callback
(which is supplied upon creation) to generate the new cached value. This
prevents the classic check and set code from proliferating throughout one's
business logic.

Furthermore, this cache only makes sense if a factory can generate the new
backing value given only the key to the requested value (say getting a qoute
for a supplied symbol). If the key is simply a unique value that doesn't help
generate the new value, then one may be forced to use the push cache.

What follows is an example of using this cache::

    var cache = new GenericOnDemandRefreshCache<string, Something>(
	    Something.Factory, Expires.Never<Something>());
    var value1 = cache.Get("key");
    var value2 = cache.Get("key");
    var value3 = cache.Get("new-key");
	Console.WriteLine("Same key values are the same: " + value1.Equals(value2));
    Console.WriteLine("Different key values are the different: " + !value1.Equals(value3));

--------------------------------------------------------------------------------
 SpecificOnDemandRefreshCache
--------------------------------------------------------------------------------

The point of this cache is to supply the backing refresh method to the cache
such that attempting to pull a value from the cache that is not present or is
expired will automatically generate the given value and then insert it into
the cache.

This differs from the ConsistentPullCache in that every key value uses its
own unique factory to generate the backing value. This also prevents
the classic check and set code from proliferating throughout one's
business logic. It is advised to iniitalize all possible values before
using the cache or supplying a better default callback for when a key is
not found (by default we throw a KeyNotFoundException).

Furthermore, this cache only makes sense if a factory can generate the new
backing value given only the key to the requested value (say getting a qoute
for a supplied symbol). If the key is simply a unique value that doesn't help
generate the new value, then one may be forced to use the push cache.

What follows is an example of using this cache::

    var cache = new GenericOnDemandRefreshCache<string, Something>(
	    Something.Factory, Expires.Never<Something>());
    var value1 = cache.Get("key");
    var value2 = cache.Get("key");
    var value3 = cache.Get("new-key");
	Console.WriteLine("Same key values are the same: " + value1.Equals(value2));
    Console.WriteLine("Different key values are the different: " + !value1.Equals(value3));


--------------------------------------------------------------------------------
 GenericBackgroundRefreshCache
--------------------------------------------------------------------------------

The point of this cache is to supply the backing refresh method to the cache
such that attempting to pull a value from the cache that is not present or is
expired will automatically generate the given value and then insert it into
the cache.

This differs from the GenericOnDemandRefreshCache in that even if a value is
expired, it will not be refreshed until the background cleanup method is run
to renew expired values. This is helpful when it may not matter if a value is
a little old and the cost of generating a new value is quite high and is better
done offline on a spare thread.

Furthermore, this cache only makes sense if a factory can generate the new
backing value given only the key to the requested value (say getting a qoute
for a supplied symbol). If the key is simply a unique value that doesn't help
generate the new value, then one may be forced to use the push cache.

--------------------------------------------------------------------------------
 SpecificBackgroundRefreshCache
--------------------------------------------------------------------------------

The point of this cache is to supply the backing refresh method to the cache
such that attempting to pull a value from the cache that is not present or is
expired will automatically generate the given value and then insert it into
the cache.

This differs from the ConsistentPullCache in that every key value uses its
own unique factory to generate the backing value. This also prevents
the classic check and set code from proliferating throughout one's
business logic. It is advised to iniitalize all possible values before
using the cache or supplying a better default callback for when a key is
not found (by default we throw a KeyNotFoundException).

Continuing, This differs from the SpecificOnDemandRefreshCache in that even
if a value is expired, it will not be refreshed until the background cleanup
method is run to renew expired values. This is helpful when it may not matter
if a value is a little old and the cost of generating a new value is quite high
and is better done offline on a spare thread.

Furthermore, this cache only makes sense if a factory can generate the new
backing value given only the key to the requested value (say getting a qoute
for a supplied symbol). If the key is simply a unique value that doesn't help
generate the new value, then one may be forced to use the push cache.

--------------------------------------------------------------------------------
 CheckAndSetCache
--------------------------------------------------------------------------------

This is the standard cache that we all know and love. It will not manage any
of the backing type creation for you, however, it will manage the expiration
policy and alert you when a value is expired.

To use it, you must perform a check and set (meaning check if the value is in
the cache, and if not, then create it and set it).

--------------------------------------------------------------------------------
 OnDemandObjectCache
--------------------------------------------------------------------------------

The object cache is useful when you have a value or collection of values that
you need to periodically refresh (say the current administrative flags). This
lets you abstract that behind a simple flags.Value call.

The on demand cache refreshes the value of the cache on the next cache.Value
call after the underlying value has expired.

What follows is an example of using this cache::

    var cache = new OnDemandObjectCache<List<string>>(() =>
        Enumerable.Range(0, new Random().Next(10)).Select(id => id.ToString()).ToList(),
        Expires.Always<List<string>>());
    var object1 = cache.Value;
    var object2 = cache.Value;
    Console.WriteLine("Different objects are different: {0}", !object1.Equals(object2));

--------------------------------------------------------------------------------
 BackgroundObjectCache
--------------------------------------------------------------------------------

The object cache is useful when you have a value or collection of values that
you need to periodically refresh (say the current administrative flags). This
lets you abstract that behind a simple flags.Value call.

The background cache refreshes the value of the cache on a background thread
when the specified condition has been met. So from the point of the user, this
cache is always up to date and never blocks (unless a call and an update collide).

What follows is an example of using this cache::

    var cache = new BackgroundObjectCache<List<string>>(() =>
        Enumerable.Range(0, new Random().Next(10)).Select(id => id.ToString()).ToList(),
        TimeSpan.FromMinutes(1));
    var object1 = cache.Value;
    var object2 = cache.Value;
    Console.WriteLine("Same objects are same: {0}", object1.Equals(object2));

================================================================================
 Cache Expiration Strategies
================================================================================

The cache expiration strategy defines a test to check if a given cached value
is expired or not.  The library uses this test to rebuild the backing value or
return expired.

--------------------------------------------------------------------------------
 IExpirationStrategy
--------------------------------------------------------------------------------

The interface for an expiration stategy is pretty simple, you are given a
cached value and you return true if it is expired or false if it is not::

    bool IsExpired(CachedValue<TItem> item)

If you are averse to creating a new class just to specify a simple delegate, you
can use the GenericExpirationStrategy which allows you to supply a predicate and
will then fullfill the interface for you::

    var factory  = SomeTypeFactory;  // object creation factory
    var strategy = new GenericExpirationStategy<SomeType>(item =>
	    item.Value == null);
    var cache = new GenericOnDemandRefreshCache(factory, strategy);

--------------------------------------------------------------------------------
 Expires Strategies
--------------------------------------------------------------------------------

In order to prevent everyone from writing the same generic expiration strategies
over and over, we have defined a large number of common strategies that should
fit all but the rarest of cases.  What follows is a list of what is available:

* Never
  
  This strategy ensures that a value placed in the cache will never expire until
  it is manually flushed from the cache.

* Always
  
  This strategy ensures that a value placed in the cache will expire every single
  time that it is pulled from the cache.

* When(predicate)
  
  This strategy ensures that a value placed in the cache will expire once some
  external condition has been met (that doesn't involve the cache value).

* Hits(count)
  
  This strategy ensures that a value placed in the cache will expire once it
  has been hit the supplied number of times.

* Timespan(timespan)
  
  This strategy ensures that a value placed in the cache will expire at a given
  time span after it has been placed in the cache.

* NotUsedIn(timespan)
  
  This strategy ensures that a value placed in the cache will expire at a given
  time span after it has not been requested from the cache.

* NextDay
  
  This strategy ensures that a value placed in the cache will expire exactly one
  day after it has been placed in the cache.

* At(timespan)
  
  This strategy ensures that a value placed in the cache will expire at the
  specified time of day.

* Introspect
  
  This strategy ensures that a value placed in the cache will expire once the value
  actually says it can be expired. This is achieved by having the cached value
  implement the IExpireStrategy interface.

================================================================================
 Cache Cleanup
================================================================================

--------------------------------------------------------------------------------
 CacheJanitor
--------------------------------------------------------------------------------

This is a singleton thread that will cleanup a each cache that has been
registered with the strategy they registered with. It runs at the frequency
of the fastest supplied cleanup strategy, however, it respects the frequency
defined by each cache. It cleans up using the supplied interface supplied
by the cache provider.

--------------------------------------------------------------------------------
 ICleanupStrategy
--------------------------------------------------------------------------------

The cleanup strategy interface is a simple wrapper for a cache and the strategy
it chose to clean it up. How the cleanup is run is controlled by the options
accessor::

    CleanupOptions Options { get; }

And the CacheJanitor makes use of the cleanup method to perform the actual
cache cleanup::

    bool PerformCleanup();

What follows is an example of registering a cleanup strategy for a cache::

    var factory  = SomeTypeFactory;  // object creation factory
    var strategy = new GenericExpirationStategy<SomeType>(item =>
	    item.Value == null);
    var cache = new GenericOnDemandRefreshCache(factory, strategy)
		.Cleanup(cache => Cleanup.Expired(cache)); // register cleaning strategy	

--------------------------------------------------------------------------------
 Cleanup Strategies
--------------------------------------------------------------------------------

Right now the following strategies are provided for cleaning up a cache:

* Nothing
		
  A janitor strategy that does absolutely no cleaning.	
  
* LeastPopular(count)

  A janitor strategy that cleans up the N least hit elements
  int the cache (least popular).

* AllButMostPopular(count)

  A janitor strategy that cleans up all but the N most hit
  elements in the cache (most popular).

* LeastRecentlyUsed(count)

  A janitor strategy that cleans up the N least recently used
  elements in the cache.	

* AllButMostRecentlyUsed(count)

  A janitor strategy that cleans up all but the N most recently
  used elements in the cache.

* BoundedAtFifo(count)

  A janitor strategy that keeps the cache at a specified size
  by removing the oldest elements.

* BoundedAtLifo(count)

  A janitor strategy that keeps the cache at a specified size
  by removing the newest elements.

* Expired

  A janitor strategy that cleans all the the expired
  elements in one sweep.

* OlderThan(TimeSpan span)

  A janitor strategy that cleans all elements that
  are past a specified age.

* YoungerThan(TimeSpan span)
	
  A janitor strategy that cleans all elements that
  are younger than a specified age.

* NotUsedIn(TimeSpan span)

  A janitor strategy that cleans all elements that
  haven't been touched in a while.

