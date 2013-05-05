==================================================
Inversion of Control
==================================================
:Author: James Hare <jhare@bashwork.com>
:Date: |today|

Introduction
--------------------------------------------------

What is loose coupling?  Coupling or dependency is the degree to which two program modules rely on each other:
* Loose - little dependency, few assumptions, isolated changes
* Benefits - easier to maintain, understand, reuse, and unit test

The problem with tight coupling is that tight coupling creates dependencies between systems:
* Changing one requires you change the other - harder to maintain
* Unit testing becomes difficult - it is hard to isolate one system from the other

How does DI make unit tests easier to write?
* Isolates each system - allows you to test one at a time
* You can easily inject mocks, fakes, and stubs that behave like you expect
* Allows you to easily test every scenario

Dependency Injection
--------------------------------------------------
*A quick discussion of dependency injection*

The process of supplying an external dependency to a software component
Rather than letting the component gain access itself, you provide the access

Constructor Injection
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Passing the external dependency via the constructor::

    public DataConsumer()
      : this(new DatabaseProducer())
    { }
    
    public DataConsumer(IProducer producer)
    {
    	_producer = producer;
    }
    
    private IProducer _producer;

Setter Injection
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Providing a method or property that allows you to set the external dependency::

    public DataConsumer()
    { }
    
    public IProducer Producer
    {
    	set { _producer = value; }
    }
    
    private IProducer _producer = new DatabaseProducer();

Inversion of Control Containers
--------------------------------------------------
*A quick discussion of ioc containers*

Refactoring For Dependency Injection
--------------------------------------------------

Extract Interface
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

For external dependencies, use interfaces
Allows for the behavior to be easily changed
Inherit stubs, mocks, or fakes from the interface
Examples: IConnectionFactory, IAccountInfoRetriever

Subclass and Override Method
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Extract the call to an external object into a new method.
Create a testing subclass (a class that extends the original class)
Override the extracted method
You can alter the method to have the behavior you desire


Parameterize Constructor or Method
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

Pass the external dependency as a parameter to the method or the constructor
Requires a change to the signature of the method


External Links
--------------------------------------------------

* `Inversion of Control <http://martinfowler.com/articles/injection.html>`_
  Dependency injection in reverse, instead have your libraries ask
  for their own dependencies.
