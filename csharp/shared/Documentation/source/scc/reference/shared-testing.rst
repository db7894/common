============================================================
Testing --- Unit Test Helpers and Extensions
============================================================
:Author: Galen Collins <gcollins@bashwork.com>
:Assembly: SharedAssemblies.General.Testing
:Namespace: SharedAssemblies.General.Testing
:Date: |today|

.. module:: SharedAssemblies.General.Testing
   :synopsis: Testing - Unit test helpers and extensions
   :platform: Windows, .Net

.. highlight:: csharp

Introduction
------------------------------------------------------------

When working with MStest, it becomes clear very quickly that the framework
is missing some simple pieces of functionality. This library helps fill in the
gaps between MStest and some of the other testing frameworks 
(see :ref:`testing-further-reading`).

The first thing we supply are a collection of assert methods that are available in
nUnit and xUnit but not available in MStest (for example Assert.Throws). Next, we
provide a generic assert that can be used to completely compare any two serializable
types in one assert. Finally, we supply a reflection wrapper that can assist in
comparing hidden fields while keeping encapsulation.

Class Library
------------------------------------------------------------

*The following represent the current functionality available in the library*

.. class:: AssertExtensions

   This simply provides the following extension methods that didn't appear to quite
   make it into MStest's assert:

   * **Pass** - Explicitly pass a test
   * **Throws** - Check if the specified method will throw
   * **Throws<T>** - Check if the specified method will throw a specific exception
   * **DoesNotThrow** - Check if the specified method will not throw
   * **DoesNotThrow<T>** - Check if the specified method will not throw a specific exception
   * **InRange** - Check if two values are within a range of each other (cast to double)
   * **NotInRange** - Check if two values are not within a range of each other (cast to double)
   * **IsNaN** - Check if a double value is NaN
   * **GreaterThan** - Check if a given value is greater than another
   * **LessThan** - Check if a given value is less than another

   **Example**

   A quick run through of the aforementioned methods is as follows::

       AssertExtensions.Pass();
       AssertExtensions.Throws(() => throw new SystemException());
       AssertExtensions.Throws<SystemException>(() => throw new SystemException());
       AssertExtensions.DoesNotThrow(() => true);
       AssertExtensions.DoesNotThrow<SystemException>(() => throw new NullArgumentException());
       AssertExtensions.InRange(22.3, 22.9, 1.0);
       AssertExtensions.NotInRange(25.3, 22.9, 1.0);
       AssertExtensions.IsNaN(double.NaN);
       AssertExtensions.GreaterThan(100, 10);
       AssertExtensions.LessThan('A', 'Z');

.. class:: GenericAssert

   There are many times when one needs to compare many values of a resulting object.
   The traditional way is to do the following::

       Assert.AreEqual("Expected1", instance.Value1);
       Assert.AreEqual("Expected2", instance.Value2.Value);
       Assert.AreEqual("Expected3", instance.Value3.SubType.Value);
       // ...

	Instead of doing this, we can simply follow the record/replay model and create
	exactly what our response should be and compare it once. We can take this same
	idea a step further and compare if two function calls behave the same (i.e. do
	they return the same value, do they throw the same exceptions, etc).

   **Example**

   A simple example comparing result types is as follows::

       /// This will allow you to wrap an existing instance in the chosen interceptor
	   var expected = new SomeType {
		   Value1 = "Expected1",
		   Value2 = new SomeSubType { Value = "Expected2" },
		   Value3 = new SomeSubType { SubType = new SomeSubSubtype { Value = "Expected3" }},
	   };
	   var actual = instance.MethodCall();
	   GenericAssert.AreEqual(expected, actual);

   A simple example comparing the result of two method calls is as follows::

       /// This will allow you to wrap an existing instance in the chosen interceptor
	   var actual = instance.MethodCall();
	   GenericAssert.AreBehaviorsEqual(
           () => throw new SystemException("Some Exception Message"),
           SomeMethodThatMayThrow);

.. note:: In order for types to be compared in this way, they must be serializable
   (dictionary I am looking at you!).

.. class:: Reflector

   This is a simple helper for reflection on hidden values in a class. Many times
   it is useful to say retrieve a private const value for comparison or the value
   of an internal list. Now you can simply pull that value (without making it public)
   and without having to repeat the same reflection logic.

   **Examples**

   An example of retrieving fields is as follows::

       // Example retrieving from an type instance
	   var instance = new SomeClassType();
	   var result1 = (string)Reflector.GetField(instance, "SomePrivateField");

       // Example retrieving generically
	   var result2 = (string)Reflector.GetField<SomeClassType>("SomePrivateField");

       // Example retrieving from the class type
	   var result3 = (string)Reflector.GetField(typeof(SomeClassType), "SomePrivateField");

   An example of retrieving properties is as follows::

       // Example retrieving from an type instance
	   var instance = new SomeClassType();
	   var result1 = (string)Reflector.GetProperty(instance, "SomePrivateProperty");

       // Example retrieving generically
	   var result2 = (string)Reflector.GetProperty<SomeClassType>("SomePrivateProperty");

       // Example retrieving from the class type
	   var result3 = (string)Reflector.GetProperty(typeof(SomeClassType), "SomePrivateProperty");

   An example of setting fields is as follows::

       // Example retrieving from an type instance
	   var instance = new SomeClassType();
	   Reflector.SetField(instance, "SomePrivateProperty", "NewValue");

   An example of setting properties is as follows::

       // Example retrieving from an type instance
	   var instance = new SomeClassType();
	   Reflector.SetProperty(instance, "SomePrivateProperty", "NewValue");

.. _testing-further-reading:

Further Reading
------------------------------------------------------------

If you are still reading, then you obviously want to learn a bit more, have fun:

* `NUnit <www.nunit.org>`_
  The original .Net unit testing framework with a great deal more functionality
  than MStest.

* `xUnit <http://www.codeplex.com/xunit>`_
  A mocking framework written using the lessons learned from all the previous
  frameworks and building off of their successes and failures.

* `MBunit <http://www.mbunit.com/>`_
  Another .Net testing framework which let to the creation of Gallio

* `Gallio <http://www.gallio.org/>`_
  A test framework agnostic test running framework. Also integrates with a wealth
  of other tools.

For more information, see the `API Reference <../../../../Api/index.html>`_.