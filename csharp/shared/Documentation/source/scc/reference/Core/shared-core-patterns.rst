=========================================================================
Core Patterns --- Generic Design Pattern Implementations
=========================================================================
:Assembly: SharedAssemblies.Core.dll
:Namespace: SharedAssemblies.Core.Patterns
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. highlight:: csharp

.. index::Design Patterns

.. module:: SharedAssemblies.Core.Patterns
   :platform: Windows, .Net
   :synopsis: Patterns - Generic Design Patterns

Introduction
------------------------------------------------------------

The **SharedAssemblies.Core.Patterns** namepsace is a collection of generic implementations of classical design patterns.
While some design patterns cannot be implemented in a fully generic way, there are several that can.  This namespace, as 
part of the *Core* will contain these generic patterns as they are built and tested.

Currently, three basic design patterns call the *Core* *Patterns* namespace their home:

* **Adapter** - *A generic implementation of an object adapter that maps items from one type to another.*
* **Factory** - *A generic implementation of a factory pattern that creates items dynamically.*
* **Singleton** - *A generic implementation of the singleton pattern that is thread safe and uses lazy instantiation.*
    
Usage
------------------------------------------------------------

Since each of the classes in this assembly are generic utility classes, they each 
have their own unique usages that are described in the class sections below.

Classes
------------------------------------------------------------

.. class:: Adapter

.. index:
    pair: Design Patterns; Adapter

The **Adapter** class is an attempt to reduce the work needed in writing separate adapter classes by providing an 
adapter framework that can be assigned mappings to convert objects from type **TFrom** to type **TTo**.

.. note:: The **TTo** type you wish to convert to must be a class.  Structs and other value types are not allowed because it would not work with the mapping framework.

To create an *Adapter*, you simply instantiate the *Adapter* class, providing it the *TFrom* type you are converting from and the *TTo* type
you are converting to.  Suppose there were two classes, one of which is from a generated proxy, and one is the application domain class.  Obviously, you 
do not want your application to depend on the generated type as that could move unexpectedly, so you plan on using the application domain class and
adapting between them::

    // generated from a web service proxy
    public class EmployeeType
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int Ssn { get; set; }

        // etc...
    }


    // the local domain object used by the application
    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string TaxId { get; set; }
    }

You can define mappings between these types.  With the **Adapter** class, to keep it simple, you define one instance per direction.  That is, if you want to go from Type1 to Type2, you'd
define an *Adapter<Type1, Type2>*, if you want the ability to also adapt from Type2 to Type2, you'd also define *Adapter<Type2, Type1>*.

You define an adapter by creating the appropriate *Adapter* instance and then adding mappings.  These mappings can be instance methods, static methods, anonymous delegates, or lambdas.  
It is generally preferred to use Lambdas for their simplicity and conciseness, but truly any of those methods are fine.  

    You can either add the mappings explicitly by using
    the **Add()** method, or by using the type initializer list (which under the covers calls the **Add()** method as well.

    For example, if adapting from EmpoyeeType to Employee (say taking a web service result and building our domain object from it), we could have::

            private static readonly Adapter<EmployeeType, Employee> _domainAdapter
                = new Adapter<EmployeeType, Employee>
                      {
                          (f, t) => t.Name = f.LastName + ", " + f.FirstName,
                          (f, t) => t.Age = f.Age,
                          (f, t) => t.TaxId = f.Ssn.ToString()
                      };

Note that this adapter was defiend as static readonly, it's very rare that one would ever need to change an adapter, so reinstantiating it
is meaningless and unnecessary.  It is typically a best practice to define it once and load it statically (either using the initializer list
on the static declaration as shown above, or it could also be done in the static constructor as below::

    public class SomeClassThatUsesTheAdapter
    {
        // in this example, just declaring adapter here, then loading in static constructor
        private static readonly Adapter<EmployeeType, Employee> _domainAdapter;


        // static constructor gets called only once when class first loaded
        static SomeClassThatUsesTheAdapter()
        {
            _domainAdapter = new Adapter<EmployeeType, Employee>
                  {
                      (f, t) => t.Name = f.LastName + ", " + f.FirstName,
                      (f, t) => t.Age = f.Age,
                      (f, t) => t.TaxId = f.Ssn.ToString()
                  };

        }
    }

The initializer form is preferred because it helps reinforce the initialize once/use many concept.  As long as the adapter is loaded once and never changed, 
it is thread-safe.

Note the format of the lambda expressions::

    (f, t) => t.SomeOtherField = f.SomeField;
    
The **f** and **t** variables are chosen for simplicity to represent *from* and *to* respectively, but since it's lambda syntax, you can choose whatever
you deem most meaningful and it will have no effect.  The following lambdas are all completely identical::

    (from, to) => to.SomeOtherField = from.SomeField
    (f, t) => t.SomeOtherField = f.SomeField
    (x, y) => x.SomeOtherField = y.SomeField
    (valueToGet, valueToPut) = valueToPut.SomeOtherField = valueToGet.SomeField
    
As you can see, the variable names mearly are placeholders.  You should choose meaningful values, but should avoid getting too wordy or the lambda will 
loose its conciseness.  We recomment either **(f,t)** or **(from,to)** for a good level of conciseness and simplicity while still providing a decent level of semantic meaning.

Also notice that you can specify as many or as few lambdas (or delegates, etc) as you wish.  I tend to prefer one per translation to keep them succinct and explicit, but you 
could also define one large lambda that does all the work.  For example, the two below are identical in function::

            // this adapter has each conversion in its own lambda
            _domainAdapter = new Adapter<EmployeeType, Employee>
                  {
                      (f, t) => t.Name = f.LastName + ", " + f.FirstName,
                      (f, t) => t.Age = f.Age,
                      (f, t) => t.TaxId = f.Ssn.ToString()
                  };
                  
            // alternatively, you can write it all in one lambda (or any combination in between)
            _domainAdapter = new Adapter<EmployeeType, Employee>
                  {
                      (f, t) =>
                          {
                              t.Name = f.LastName + ", " + f.FirstName;
                              t.Age = f.Age;
                              t.TaxId = f.Ssn.ToString();
                          }
                  };
            
Regardless of how the adapter is loaded, you convert objects using the adapter by calling the **Adapt(...)** method::

            // supposing this record was returned from a proxy...
            var proxyRecord = new EmployeeType
                                  {
                                      FirstName = "John",
                                      LastName = "Smith",
                                      Age = 31,
                                      Ssn = 111223333
                                  };

            // we can convert it using Adapt(...)
            Employee domainRecord = _domainAdapter.Adapt(proxyRecord);
    
After this example, domainRecord will have a Name field of "Smith, John", an age of 31, and a TaxId field that is a string containing "111223333".

    .. method:: void Add([Predicate<TFrom> conditional,] Action<TFrom,TTo> mapping)
    
        :param TFrom: the type being adapted from.
        :param TTo: the type being adapted to.
        :param conditional: *(optional)* conditional test on *TFrom* value, if true will execute mapping.
        :param mapping: mapping from *TFrom* type to *TTo* type.
        
        Adds a new mapping to the *Adapter*.  This is either called explicitly or implicitly using an initializer list.
        
    .. method:: TTo Adapt(TFrom sourceObject)
    
        :param TFrom: the type being adapted from.
        :param TTo: the type being adapted to.
        :param sourceObject: the value to adapt to the *TTo* type.

        Performs the actual adaptation from *TFrom* to *TTo*.

.. class:: Factory

The **Factory** class's **Create<T>()** method is used to dynamically create objects of type *T* from the given assembly and class name.  This comes in handy
when you need to load an implementation of an interface or superclass from an assembly, but do not have a reference to the actual type at compile time (assembly could be 
dynamically specified by a config file or some other dynamic means.

    .. method:: static T Create<T>(string assembly, string concreteClass[, bool shouldThrowOnFailure])
    
        :param T: the supertype or interface that the concrete class to create implements or extends.
        :param assembly: the name of the assembly the class to instantiate is in.
        :param concreteClass: the name of the concrete class to instantiate.
        :param shouldThrowOnFailure: *(optional)* if true will throw if class cannot be instantiated; if false will return *null*
    
        To create an instance dynamically, just specify the generic interface or superclass type as type *T* in the <> brackets, and then specify the assembly and actual
        class name to create as the parameters to the Create() method::
        

                    var adapter = Factory.Create<IEmailer>("Assemblies.Adapters.dll", "EmailAdapter");
                    
        The default behavior of this method is to throw an exception if the class cannot be dynamically created or is the wrong type.  However, this can
        be suppressed to always return null instead of throwing with an optional third parameter::

                    var adapter = Factory.Create<IEmailer>("Assemblies.Adapters.dll", "EmailAdapter", false);

        Notice that in either case you specify the type and the class name, this has two purposes:

            1) To verify that the class named in the assembly is the right type, and
            2) To allow the developer to specify a superclass or interface to reduce coupling

.. class:: Singleton

The **Singleton** class is a thread-safe, lazy implementation of the *Singleton* design pattern.  It accomplishes thread-safety and the lazy load without locking.  Thus
making it ideal for all uses unless it is absolutely essential that the class be loaded sooner, which is not usually the case.  

The mechanism by which the lazy-load takes place is the inner class.  Static inner class members are not instantiated by default until 
with a static Constructor, this prevents the static field from being initialized before it is called.  The first time the **Instance** property is
called, it will instantiate the inner class which holds the class to be a singleton.
    
    .. method:: T Instance
    
        **Instance** is the property which retrieves the singleton instnace in a lazy-load manner on first call.
        
To use the singleton, simply use the type you wish to wrap in a singleton::

    Singleton<SnmpUtil>.Instance.SendTrap(100, "Service is Down");
    
You are free to store this singleton or just use the Instance whenver needed.  As long as it is accessed or created through the Instance
property in the same *ApplicationDomain*, it will be the same instance.  This is consistent with the behavior of all static instances.  It
is a common misconception that static instances are one per application, but it is really one per application domain which 
can cause confusion because an application *may* have more than one *ApplicationDomain* and more than one application may share
the same *ApplicationDomain*.  

Typically, console applications and services run in their own *ApplicationDomains* and web applications are configured as to whether they
share or have their own domains.  Usually this is not an issue, but should be kept in mind to avoid incorrect assumptions.

.. note:: Though using *Instance* to create or access the singleton will guarantee one instance per *ApplicationDomain*, this does not prevent the user from creating other instances of type *T* directly.

For more information, see the `API Reference <../../../../../Api/index.html>`_.
