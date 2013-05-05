============================================================
Interceptors --- Generic Interface Method Decorators
============================================================
:Author: Galen Collins <gcollins@bashwork.com>
:Assembly: SharedAssemblies.General.Interceptors
:Namespace: SharedAssemblies.General.Interceptors
:Date: |today|

.. module:: SharedAssemblies.General.Interceptors
   :synopsis: Interceptors - Dynamic interface decorators
   :platform: Windows, .Net
   
.. highlight:: csharp

The Problem
------------------------------------------------------------
*Background:  We have a collection of services that we need to optionally
audit and test performance.*

Here is the current solution to this problem.  First we extracted interfaces from
all of our services and had everything implement themselves in terms of said
interfaces. We then created a Logging/Performance decorator class that wraps every
method invocation in an interface with logging of all inputs, outputs, and a function
execution time.  We then optionally injected a decorated version of the implementation
or the implementation itself.

The problem with this solution is that we have to implement the same decorated logging
code for every method in every interface we have.  Instead of copying all of that
boilerplate code for each implementation, wouldn't it be great to simply have a generic
decorator that we can apply to an interface?

The Solution
------------------------------------------------------------

A library from the `Castle Project <http://www.castleproject.org/index.html>`_
called *Dynamic Proxy* solves this problem for us using C# reflection abilities::

    using Castle.DynamicProxy;
    
    public class SimpleInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.Write("Method [{0}]", invocation.Method.name);
            Invocation.Proceed();
        }
    }

It allows us to easily create proxies against an interface (with or without a target) so
that we can control what before, during, and after a method invocation. For the rest of
the document I will refer to this as an Interceptor (it will be more apparent in a minute). 
A simple example will help to show how to use this library; assume the same structure as above::

    using Castle.DynamicProxy;
    
    public class MyApplication
    {
        static readonly ProxyGenerator _generator = new ProxyGenerator();

        public static Type Create<Type>(Type instance)
            where Type : class
        {
            return _generator.CreateInterfaceProxyWithTarget
                <Type>(instance, new SimpleInterceptor());
        }
        
        public static void Main()
        {
            IType handle = Create<IType>(new TypeImp());
            handle.method();
        }
    }

*This method will be called for every invocation in the decorated interface.*

Although the example may not express it, we now have complete control over not
only what happens before and after a method invocation, but also the supplied
parameters, the return value, or even if the method is called at all!  Needless
to say there is a great deal more power that Dynamic Proxy supplies (interceptor
chains, method filtering, dynamic interface implementation, etc) for our purposes
this is more than enough.

Class Library
------------------------------------------------------------

In the current Shared.Interceptors library, I have supplied four interceptors (as
well as passable unit tests).  It should be noted, not all of these may find a good
use or even be useful; they are supplied simply to give a few more ideas of how to
use Dynamic Proxy:

.. class:: BlockingInterceptor

   The point of this interceptor is to simply block a list
   of supplied methods from being called. A simple use case is a current unbreakable
   implementation with an expensive, but not necessary, call (e.g. logging, performance).
   Without changing the interface or code, we can simply supply a list of methods to
   block from running.

   **Example**

   A simple example of using this is as follows::

       /// This will allow you to wrap an existing instance in the chosen interceptor
       var blockedMethods = new List<MethodInfo> {
           typeof(TInterface).GetMethod("BadMethod")
       };

       /// This will allow you to wrap an existing instance in the chosen interceptor
       TInterface instance1 = BlockingInterceptor.Create<TInterface>(new Implementation(),
          blockedMethods);
       
       /// This will allow you to wrap a new instance in the chosen interceptor
       TInterface instance2 = BlockingInterceptor.Create<TInterface, Implementation>(
          blockedMethods);

.. class:: TimingInterceptor

   This interceptor will simply clock and log every method that
   is invoked. Handy for testing an implementation without riddling it with extraneous
   performance and logging code.
   
   Note that this interceptor simply logs entry and exit of a method, for logging when
   method times exceed a threshold, see the *TimeThresholdInterceptor*.

   **Example**

   A simple example of using this is as follows::

       /// This will allow you to wrap an existing instance in the chosen interceptor
       TInterface instance = TimingInterceptor.Create<TInterface>(new Implementation())
       
       /// This will allow you to wrap a new instance in the chosen interceptor
       TInterface instance = TimingInterceptor.Create<TInterface, Implementation>()
       
.. class:: TimeThresholdInterceptor

    While the *TimingInterceptor* simply
    logs the entry and exit of a method, however this interceptor computes the duration of the 
    method and logs a message if it exceeds a threshold.
    
    This is extremely helpful for logging warnings when database or other external resource bound methods
    are taking longer than usual.
    
    Assuming you wanted to log any *DatabaseUtility* calls that take more than 5 seconds, you could do the following::
    
            // create an interceptor around DatabaseUtility that logs calls over 5 seconds.
            IDatabaseUtility dbUtility = TimeThresholdInterceptor.Create<IDatabaseUtility>(
                new DatabaseUtility(ClientProviderType.SqlServer, "server=cgserver001..."),
                TimeSpan.FromSeconds(5));

            // this call to the wrapped DatabaseUtility will now log warnings when any
            // methods called on it exceed 5 seconds.  Useful for detecting long queries.
            dbUtility.ExecuteNonQuery("sp_some_proc", CommandType.StoredProcedure);        

.. class:: LoggingInterceptor

   This interceptor de-serializes and logs every input and
   output from a method. Handy for quick service auditing.

   **Example**

   A simple example of using this is as follows::

       /// This will allow you to wrap an existing instance in the chosen interceptor
       TInterface instance = LoggingInterceptor.Create<TInterface>(new Implementation())
       
       /// This will allow you to wrap a new instance in the chosen interceptor
       TInterface instance = LoggingInterceptor.Create<TInterface, Implementation>()

.. class:: ThrowingInterceptor

   This is the same as the blocking, but we actually throw when
   said method is called. A simple use case for this is an interface that cannot be
   changed but an implementation that is severely deprecated or dangerous.

   **Example**

   A simple example of using this is as follows::

       /// This will allow you to wrap an existing instance in the chosen interceptor
       var blockedMethods = new List<MethodInfo> {
           typeof(TInterface).GetMethod("BadMethod")
       };

       /// This will allow you to wrap an existing instance in the chosen interceptor
       TInterface instance1 = ThrowingInterceptor.Create<TInterface>(new Implementation(),
          blockedMethods);
       
       /// This will allow you to wrap a new instance in the chosen interceptor
       TInterface instance2 = ThrowingInterceptor.Create<TInterface, Implementation>(
          blockedMethods);

.. note:: This library also requires you to include a reference to Castle.Core.dll

.. To use these interceptors, all you have to do is include references to Castle.Core and
.. Shared.Interceptors. Each interceptor has two static constructors (two more if they
.. take parameters) that let you wrap your interface::
.. 
..     /// This will allow you to wrap an existing instance in the chosen interceptor
..     TInterface instance = LoggingInterceptor.Create<TInterface>(new Implementation())
..     
..     /// This will allow you to wrap a new instance in the chosen interceptor
..     TInterface instance = LoggingInterceptor.Create<TInterface, Implementation>()

Going Further
------------------------------------------------------------

If you are still reading, then you obviously want to learn a bit more, have fun:

* `Aspect Oriented Programming(AOP) <http://en.wikipedia.org/wiki/Aspect-oriented_programming>`_
  Extract cross cutting concerns from code and make
  them reusable across the board (logging, performance, security, etc).

* `Moq <http://code.google.com/p/moq/>`_
  A mocking framework written with C#3.5 candy and Dynamic Proxy; makes unit
  testing quick and eliminates a source tree full of mocks/stubs/fakes/etc.

* `Castle Project <http://www.castleproject.org/index.html>`_
  Think boost for C#, just a bit more high level.

* `Windsor Container <http://www.castleproject.org/container/gettingstarted/index.html>`_
  Inversion of Control container for supplying facilities and
  components their implementation via configuration files.

For more information, see the `API Reference <../../../../Api/index.html>`_.