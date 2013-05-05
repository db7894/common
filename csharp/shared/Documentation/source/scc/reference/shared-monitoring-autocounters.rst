=========================================================================
AutoCounters --- Windows Performance Counter Instrumentation
=========================================================================
:Assembly: SharedAssemblies.Monitoring.AutoCounters.dll
:Namespace: SharedAssemblies.Monitoring.AutoCounters
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.Monitoring.AutoCounters
   :platform: Windows, .Net
   :synopsis: AutoCounters - Windows Performance Counter Instrumentation

.. highlight:: csharp

.. index:: 
    pair: Performance Counters; Instrumentation

Introduction
------------------------------------------------------------

Windows Performance Counters are Microsoft's way of instrumenting applications.  It gives your
application the ability to report various statistics to the operating system which are then stored
and can be queried by other programs.

These Performance Counters are visible whenever you pull up the Performance Monitor (perfmon.exe) 
in Windows.  In addition, they can be forwarded or queried remotely.  

The **SharedAssemblies.Monitoring.AutoCounters** assembly is a library that was created to
help ease the creation and use of .NETs **PerformanceCounter** class that deals with underlying Windows
Performance Counters.  This library
wraps the Performance Counter in a class called **AutoCounter** that allows you to install, configure, 
and use a Performance Counter consistently
without ever needing to know how the underlying Performance Counters work.

In addition, this library provides the **AutoCounterCache** to allow you to create a cache
of *AutoCounter* definitions so that your application does not need to inject counters into
various components, but can instead use them across your application easily and effectively.

You will find that the AutoCounter library itself is really a collection of convenience wrappers,
caches, and attributes.  Technically anything you can do with an AutoCounter you can do with the
underlying PerformanceCounters, it's just made much easier and much simpler to configure.

Quick Start Summary
----------------------------------------------------------

This quick summary are best-practices when using the AutoCounter library.   Obviously, the library gives you the power
and flexibility to do as much or as little automation as you want, but these are the recommendations:

    * **Always** declare your **AutoCounters** as assembly-level attributes.
    * **Always** use the .NET Framework **installutil** tool to install the counters.
    * **Strongly Prefer** to use the **AutoCounterCache** to access those counters.
    
Everything in this library has been created and tuned to be driven from the assembly-level attributes.  In this way
you don't have installation, instantiation, and usage all separated in different parts of code.  If you follow these
three steps everything will be automated thus reducing the possibility of errors.    

Performance Counter Installation
----------------------------------------------------------

For a Performance Counter to be used in a program, it first must be installed.  This can be a very heavy
bit of code, and unfortunately it usually can't always be done when a program wants to use a counter
because the program seeking to install the programmer must have Administrator privileges.

So typically, a program just uses a PerformanceCounter and then relies on either manual installation
of the counter, or some other program to do so that is run separately from the command line.

The AutoCounters library relies on the .NET utility **installutil** that comes with every installation
of the .NET Framework on the target machine to do the installation.  However, you must still somehow
tell the installutil program what counters to install.

Originally, this was done in a very clunky text file format.  This has been completely revamped to instead
use assembly-level attributes.  The assembly attributes server two purposes: they tell the installer what
counters will need to be installed, and they tell the **AutoCounterCache** what counters to manage.

To do this, in the assembly that will use the counter, you just define a set of assembly-level attributes
for the category the counters will live under and for the counters themselves.  There are other types of AutoCounter attributes
that we will discuss later, for now let's look at the basics::

    [assembly: AutoCounterCategory("TestProgram")]
    [assembly: AutoCounter(AutoCounterType.TotalCount, "TestProgram", "WidgetCount")]
    
.. note:: You can use the AutoCounterCategory and AutoCounter assembly-level tags to install counters even if you choose to refer to them using the .NET *PerformanceCounter* class or any other means.  

.. note:: Also, you can technically use *AutoCounter* without any assembly-level attributes, but in this case the counter will *not* be in the *AutoCounterCache* nor will it be automatically installed by *installutil*.

This defines a new category in the windows performance counter categories called *TestProgram*, and under that category there
is one performance counter called *WidgetCount* that is a NumberOfItems32 counter.  We'll discuss the various type of counters and 
their calculations later, for now the important thing is that these attributes are used by the installer and the cache 
to install and use, respectively, the counters in the application.

The example above is a single-instance counter category.  This means that there is only one TestProgram:WidgetCount counter for the whole system.
Let's say you want to measure a statistic that has multiple instances (like the processor utilization per CPU).  We could do that
easily by declaring our category to be multi-instance.  For example, let's say our application has 10 queues in it and we want to be able to 
track the depth of each queue individually::

    [assembly: AutoCounterCategory("TestProgramQueues", InstanceType = InstanceType.MultiInstance)]
    [assembly: AutoCounter(AutoCounterType.TotalCount, "TestProgramQueues", "QueueDepth")]
    
Note that the *category* makes the counter multi-instance.  All counters in a multi-instance category will be instanced counters.
You actually declare each instance of the counter at the time you use them.  We'll get into that later when we talk about
instantiating and using them in a program.  For now know that the **InstanceType** enum defines whether all counters in a given 
category have only one instance, or are separated into possibly several per counter.  Also notice that (at least at the time of this writing) categories are not nested.  All categories are always at the root level in the current versions of Windows.

In addition to the InstanceType optional parameter, there are other parameters you can apply to the counter attributes:

    * **IsReadOnly** - Used at run-time to state whether you will be updating the counter or just querying its value.
    * **Description** - Used at install-time to add descriptive help text in the counter registry.
    * **UniqueName** - Used at run-time to give a shorter unique name to the counter.  By default the counter's *UniqueName* in the cache will be *CategoryName:CounterName*, this name is a shorter substitute for Cache queries.

So, once you compile your assembly with these assembly-level attributes defining what counters you intend to use, you install them by invoking the .NET Framework's **installutil** invoked against the **SharedAssemblies.Monitoring.AutoCounters.dll**.  This may sound strange, but it was done as a convenience.  When you invoke *installutil* against the *AutoCounters* assembly, it will scan all assemblies in the current directory to find any that have assembly-level AutoCounter attributes.

So, navigate to the directory where the AutoCounters.dll and your own dlls are, and run::

    C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\installutil SharedAssemblies.Monitoring.AutoCounters.DLL
    
If your DLLs are in a location other than the AutoCounters.DLL you can specify with the /target flag::

    C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727installutil /target="C:\Bashwork" 
        SharedAssemblies.Monitoring.AutoCounters.DLL 
    
Similarly, if you want to uninstall all counters and categories, you can use the /u flag::

    C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\installutil /u SharedAssemblies.Monitoring.AutoCounters.DLL

.. note:: The 3.5 .NET Framework does not have an installutil since it is really just candy added on top of the 2.0 framework.  As such you use installutil in the 2.0 framework directory (or 4.0 if installed).

Counter Types
-------------------------------------------------------------

You declare the type of the counter when you install it, not when you use it.  This is important because if the
underlying counter you're expecting isn't the one that's installed, it can lead to confusion or errors.  That is why a lot of
effort was expended to make all AutoCounter information go in the assembly tags, so it's defined once for both usage and installation
and hopefully, thus, more consistent and less error-prone.

Currently, the AutoCounter library supports the following counter types defined in the enum **AutoCounterType**:

    * **TotalCount** - Used to keep a running count of the number of times something has occurred.
    * **CountsPerSecond** - Used to keep track of the number of times something happens per second.
    * **AverageTime** - Used to keep track of the instantaneous average amount of time it takes to perform something.
    * **RollingAverageTime** - Keeps a rolling average of the last 50 average times it takes to perform something.
    * **ElapsedTime** - Keeps track of the amount of time that has passed since the last time the counter was updated.
    
You will note that some of these have **PerformanceCounterType** parallels and some do not.  We tried to take advantage of existing types 
where possible and abstracted away the base-counter information.  And in some cases we created a new counter type that has no parallel.

For example, *AutoCounterType.AverageTime* is equivalent to the *PerformanceCounterType.AverageTimer32*.  However, the *AutoCounterType.RollingAverageTime*
is a new counter type that only exists in the *AutoCounter* library.  The problem with the old *AverageTimer32* counter is that it is instantaneous.  What
that means is that if no requests are made in any given second, the counter automatically zeros itself.  Thus if you had four requests that took 30 seconds, 20 seconds,
25 seconds, and 10 seconds, but then had no updates in the next second, the counter would go to zero, which may not be desired.  The *AutoCounterType.RollingAverageTime* 
keeps track of its own rolling average of the last 50 updates and then displays that average.

Instantiating a Counter Directly
-------------------------------------------------------------

As stated before, the assembly-level **AutoCounter** attributes are really just installer and cache candy 
to make it easier to use counters.  You can easily instantiate a .NET PerformanceCounter directly::

    // this is NOT the preferred way and in fact this is the last I'll say about using these directly...
    var counter = new PerformanceCounter("TestProgram", "WidgetCount", null, false);
    
However, it is much better to use the AutoCounter wrapper (below) and even better to use the **AutoCounterCache** (next section)::

    var AutoCounter = new AutoCounter("TestProgram", "WidgetCount", false);
    
Notice, once again, we do not declare the type of counter when we instantiate this.  It checks the installed performance counter and uses
whatever type was installed.  Also, it should be noted that if the underlying counter does not exist or was not installed correctly, this
will throw an exception.

Another goal of the AutoCounter library was to abstract a lot of this behavior and make it more gentle.  For example, someone shouldn't need to know
that to do certain counter types you need to actually install two counters (the main counter and a base counter).  The AutoCounter library abstracts
this and creates the appropriate counter and any base counter if required for the type you provide.

Also, if you use AutoCounter you have the choice of deciding whether attempting to instantiate a counter that doesn't exist throws or creates a 
**StubCounterStrategy**.  The enum **CreateFailedAction** lists the options:

    * **ThrowException** - If the underlying counter does not exist or insufficient privileges, will throw an exception.
    * **CreateStub** - If the underlying counter doesn't exist or insufficient privileges, will return a dummy counter that always returns -1.
    * **Default** - Uses the default for this assembly, which unless otherwise specified is *CreateStub*.
    
So when you create an auto counter, you can specify the action to take if the counter wasn't installed or if you don't have sufficient privileges at run-time::

    var AutoCounter = new AutoCounter("TestProgram", "WidgetCount", false, CreateFailedAction.CreateStub); 
    
If you don't specify, the default is **CreateFailedAction.Default** which will check the current assembly for an assembly level default tag::

    [assembly: AutoCounterCreateFailedDefaultAction(CreateFailedAction.ThrowException)]
    
In this case, this means that any AutoCounters created with *CreateFailedAction.Default* that were not installed will throw an exception.  If this
assembly-level attribute is not specified, the default action is *CreateFailedAction.CreateStub*.   

.. note:: -1 was deliberately chosen as the flag value for the *StubCounterStrategy* because we wanted it to be obvious to someone looking at the counter value that it didn't exist.  Choosing zero would be confusing as zero is often a perfectly valid counter value, while -1 rarely ever is as nearly all counters are positive numbers.

Instantiating a Counter From the Cache
--------------------------------------------------------------------

The **AutoCounterCache** was created for two goals:

    1. To isolate counter configuration to just assembly-level attributes so that it only needs to be defined once.
    2. To increase performance by caching references to counters on first-use (lazy-loading).
    
Getting a counter from the cache is very easy.  You use the **AutoCounterCacheFactory** to serve up an instance of the cache to you.  Basically this is much
the same way you use **LogManager** in **log4net** to create an ILog instance.  The *AutoCounterCacheFactory* examines the assembly you are calling it from and
then checks that assembly to see if it already created a cache.  If it did, it returns it.  If not, it creates a cache and loads it with all the assembly-level *AutoCounter* attribute definitions.

To avoid having to do messy injection, I like declaring my cache once per class like in log4net::

    public class SomeApplication
    {
        // checks this assembly and returns the cache or creates a new one if first call from this assembly
        private static readonly ICounterCache _counters = AutoCounterCacheFactory.GetCache();

        ...
    }
    
Once you have the cache, you can access any counter in the cache using the **UniqueName** of the counter.  Remember, by default if you didn't specify *UniqueName* it
will be **CategoryName:CounterName**.  That is, the CategoryName and CounterName concatenated with a ':' in the middle.  So, using the name assigned, you call using the Get() method::

    var myCounter = _counters.Get("TestProgram:WidgetCount");
    
This looks up the counter definition in category *TestProgram* with a counter name of *WidgetCount* in the cache and returns the instance if it has already been created, or creates a new instance if this is the first call to that counter.

Now, having an instance of a counter does nothing, really.  Let's move on to using it.

Measuring Statistics with InstrumentationBlocks
--------------------------------------------------------------

The simplest way to use an AutoCounter is to use the built-in **InstrumentationBlock**.  This is a struct (value-type so doesn't add GC pressure)
that is returned by the AutoCounter when you call **GetBlock()** and implements **IDisposable**.  When used in a *using* block, this will automatically
call **StartBlock()** and **EndBlock()** on the counter. 

For example, this::

    var myCounter = _counters.Get("TestProgram:WidgetCount");
    
    ...
    
    myCounter.StartBlock();
    
    // some code to measure
    
    myCounter.EndBlock();
    
Is completely identical to::

    var myCounter = _counters.Get("TestProgram:WidgetCount");
    
    ...
    
    using (myCounter.GetBlock())
    {
        // some code to measure
    }
        
Personally I prefer the *using* methodology because it doesn't require you to remember to call *EndBlock()* but does it automatically when the scope
is exited.  It also gives you a meaningful idea of the scope of the measurement that you are trying to take.

.. note:: The actually actions performed in **StartBlock()** and **EndBlock()** depend on the type of installed counter.  Different counter types do different measurements at the start and end of a block.

If you are using the **AutoCounterCache**, it's even easier.  You can use the index-operator (i.e. []) to grab an **InstrumentationBlock** from a counter in the cache
as follows::

    // prefer to use the [] indexer
    using (_counters["TestProgram.WidgetCount"])
    {
        // some code to measure
    }

See how easy that is?  We didn't need to grab the counter ahead of time at all!  This is actually equivalent to::

    // this is a bit longer and ugly...
    using (_counters.Get("TestProgram.WidgetCount").GetBlock())
    {
        // some code to measure
    }
    
but the first one is much shorter and more concise!  Using the **AutoCounterCache**, in fact, you need never directly instantiate a counter reference unless
you just happen to want one.

Using AutoCounterCollections
------------------------------------------------------------

Sometimes, you may want to make several measurements on a block of code.  Perhaps for a given method, you wish to know the number of times called, the number of times
per second called, and the average time per call.  If you wanted to update all three statistics, this would be kind-of ugly, even with the cache.

Let's assume we have the following AutoCounters defined in **AssemblyInfo.cs**::

    [assembly: AutoCounterCategory("TestProgram")]
    [assembly: AutoCounter(AutoCounterType.TotalCount, "TestProgram", "WidgetsCreated")]
    [assembly: AutoCounter(AutoCounterType.CountsPerSecond, "TestProgram", "WidgetsPerSecond")]
    [assembly: AutoCounter(AutoCounterType.RollingAverageTime, "TestProgram", "WidgetAvgCreateTime")]

Then, when you go to actually create your widget (assuming *_counters* is an instance to your **AutoCounterCache**) you could say::

    // assuming _counters is your reference to the cache
    using (_counters["TestProgram.WidgetsCreated"])
    using (_counters["TestProgram.WidgetsPerSecond"])
    using (_counters["TestProgram.WidgetAvgCreateTime"])
    {
        // code to create a widget...
    }

Well, that's not horrid, and it's still better than having to instantiate each counter manually and create the blocks, but there's an easier way.  
You can define an **AutoCounterCollection** either manually or in the assembly-level attributes.  

Defining the *AutoCounterCollection* in the assembly attributes is easy.  Simply give the collection a name, state whether it is instanced or not, and 
list the counters that belong to it::

    [assembly: AutoCounterCollection("CreateWidgetStats", "TestProgram:WidgetsCreated", "TestProgram:WidgetsPerSecond",
        "TestProgram.WidgetAvgCreateTime")]
        
What this says is that there is a collection of *AutoCounter*s that goes by the name "CreateWidgetStats".  When you query the *AutoCounterCache* using
that name, it gives back an *InstrumentationBlock* that handles the *StartBlock()* and *EndBlock()* for every counter in the collection.

Thus, the triple-using example above could now be re-written as::

    // assuming _counters is your reference to the cache
    using (_counters["CreateWidgetStats"])
    {
        // code to create a widget...
    }
        
Much cleaner!  Only *AutoCounterCollections* created with the assembly-level attribute will appear in the *AutoCounterCache*.  You can
create your own local *AutoCounterCollection*, manually, but this will not be represented in the cache and is not the preferred method.

Collections can also have parent collections.  For example, let's say producing widgets is one type of request that our program can 
perform, and we want to keep track of all requests including destroy widgets, recycle widgets, etc.  We can group each set of counters
into its own group, and then have them all refer to a parent group for the totals across all requests::

    [assembly: AutoCounterCategory("TestProgram")]
    [assembly: AutoCounter(AutoCounterType.TotalCount, "TestProgram", "WidgetsCreated")]
    [assembly: AutoCounter(AutoCounterType.CountsPerSecond, "TestProgram", "WidgetsPerSecond")]
    [assembly: AutoCounter(AutoCounterType.RollingAverageTime, "TestProgram", "WidgetAvgCreateTime")]
    ...
    [assembly: AutoCounter(AutoCounterType.TotalCount, "TestProgram", "TotalRequsts")]
    ...
    // Currently, parents to collections must be other collections, may enhance this in the future 
    // to be single counters as well...
    [assembly: AutoCounterCollection("TotalRequestStats", "TestProgram:TotalRequests")]
    
    // the sub-collection now refers to a parent collection that it will cascade updates up to.
    [assembly: AutoCounterCollection("CreateWidgetStats", "TestProgram:WidgetsCreated", "TestProgram:WidgetsPerSecond",
        "TestProgram.WidgetAvgCreateTime", ParentCollection = "TotalRequestStats")]
        
Note that the *CreateWidgetStats* collection now has a parent collection called *TotalRequestStats*.  What this means is that
whenever you perform an *InstrumentationBlock* on the *CreateWidgetStats* collection, it will also update the *TotalRequestStats* collection.

This hierarchy is not required, but is a nice way to have higher-level stats that are updated whenever lower-level stats are updated as well.

.. note:: As an alternative, instead of using a *ParentCollection* you could include the *TestProgram:TotalRequests* stat in each sub-collection.  
    
Further Reading
----------------------------------------------------------
For more information, see the `API Reference <../../../../Api/index.html>`_.