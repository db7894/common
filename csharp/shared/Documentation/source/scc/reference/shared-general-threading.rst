=========================================================================
Threading --- Multi-threading and Concurrency
=========================================================================
:Assembly: SharedAssemblies.General.Threading.dll
:Namespace: SharedAssemblies.General.Threading
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.General.Threading
   :platform: Windows, .Net
   :synopsis: Threading - Multi-threading and Concurrency Helpers

.. highlight:: csharp

.. index:: Threading

.. index:: Tasks
    

Introduction
------------------------------------------------------------

The **SharedAssemblies.General.Threading** assembly is a collection of utility classes
that make it easier to do multi-threaded and concurrent processing.  The threaded
*Consumers* and *Buckets* are useful for doing true multi-threaded processing of a queue of items, and the 
*Task* classes are useful for processing a set of items over a limited pool of threads.

Each methodology has their strengths:

    **Use Buckets and Consumers if...**
        * *You want to consume data.*
        * *Consumption tends to be more long-term (application scope).*

    **Use Tasks if...**
        * *You want to consume delegates (tasks).*
        * *Consumption tends to be more short-term (method scope).*

Using Buckets and Consumers
------------------------------------------------------------

Basically, a **Bucket<T>** is a thread-safe queue of items to process of any type.  It has methods to add and remove items,
and the **Consumers** of these items get from the *Bucket* in a thread-safe way.  It is best used for homogeneous processing
of like data (such as processing a stream of quote updates) that tend to live for the life of an application.

        
Using Tasks
-------------------------------------------------------------

The **Task** classes are a thread-safe way to consume a set of heterogeneous delegates.  This
can also be accomplished by the .Net *ThreadPool* class, but unfortunately the implementation of *ThreadPool*
in .Net 3.5 and earlier is sub-optimal as it all uses the same thread pool as the application itself which can cause
contention.  The thread-pool used by the *Task* classes was designed to be independent so that items can be processed
concurrently without interfering with the built-in *ThreadPool*.

.. Note:: The *Task* classes will be somewhat obsolete with .Net 4.0.  They will be maintained for backward compatibility, though.  *Buckets* and *Consumers* will not be affected.


Further Reading
----------------------------------------------------------
For more information, see the `API Reference <../../../../Api/index.html>`_.