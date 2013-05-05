=========================================================================
Core Containers --- Supplemental Container Classes
=========================================================================
:Assembly: SharedAssemblies.Core.dll
:Namespace: SharedAssemblies.Core.Containers
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. highlight:: csharp

.. index:: Containers

.. module:: SharedAssemblies.Core.Containers
   :platform: Windows, .Net
   :synopsis: Containers - Supplemental Containers

Introduction
------------------------------------------------------------

The SharedAssemblies.Core.Containers namespace is a set of utility classes that
implement containers.  Container classes are basic utility classes that have
the task of containing a generic type of element and operating on the collection in
different ways specific to the container.

These containers are not meant to replace the basic .Net generic containers in any way,
but generally just add some functionality to existing containers or provide a new
container that is not part of the .Net container namespace.

Currently, this namespace only holds two containers: Range and Serializable Dictionary.

Usage
------------------------------------------------------------

Since each of the classes in this assembly are generic utility classes, they each 
have their own unique usages that are described in the class sections below.

Classes
------------------------------------------------------------

.. class:: Range
    The **Range<T>** class is used to declare a continuum of values over a type *T* where *T* can be
    any type that implements *IComparable*.  When you instantiate a range, you declare it to be from
    a start to end point, which forms the range of [start, end] inclusively.

    The set of operations that range then operates on uses the implementation of *IComparable* to
    determine if the value compared against the range is within [start, end] inclusively.  Note
    that all basic numeric, character, and string types already implement IComparable, so it uses the built in
    comparison operations

    Here is a quick example of using the Range to verify if numbers fall within a given range::

            // creates a range over [3,7] inclusively
            var range = new Range<int>(3, 7);

            if(range.IsInRange(5))
            {
                Console.WriteLine("5 is in the range [3,7]");
            }

            if(!range.IsInRange(8))
            {
                Console.WriteLine("8 is NOT in the range [3,7]");
            }

    You can use custom types with Range, in which case the only thing that is required is that they should implement
    IComparable (though it is often recommended that when you implement IComparable you also implement IComparable<T> as well.

    Here is an example using a basic class called Fraction that represents a rational type (fraction) with a numerator (the top
    number of the fraction) and a denominator (the bottom number of the fraction).  So 1/2 would be represented by a Fraction
    with Numerator == 1 and Denominator == 2::

            // sample custom class that implements IComparable
            class Fraction : IComparable<Fraction>, IComparable
            {
                // the numerator (top number) of the fraction
                public int Numerator { get; set; }

                // the denominator (bottom number) of the fraction
                public int Denominator { get; set; }

                public double Irrational { get { return Numerator/(double) Denominator; } }

                // Compares the current object with another object of the same type.
                public int CompareTo(Fraction other)
                {
                    if (other == null)
                    {
                        throw new ArgumentNullException("other");
                    }

                    return Irrational.CompareTo(other.Irrational);
                }

                // generic compare-to
                public int CompareTo(object other)
                {
                    return CompareTo(other as Fraction);
                }
            }

    Notice that int IComparable.CompareTo(object) is implemented.  It should
    return -1 if the current object is < the argument, and 0 if the objects are equal, and +1 if the current object is > the argument.
    Using this, we can declare and use a range like so::

        // create a range of fractions from 1/8 to 3/4
        var fractionRange = new Range<Fraction>(
            new Fraction { Numerator = 1, Denominator = 8 },
            new Fraction { Numerator = 3, Denominator = 4 });

        if(fractionRange.IsInRange(new Fraction { Numerator=1, Denominator=2 }))
        {
            Console.WriteLine("1/2 is in the range [1/8, 3/4]");
        }


.. class:: SerializableDictionary

    The **SerializableDictionary<TKey,TValue>** class was an attempt to create a Dictionary that can be serialized to and deserialized from
    Xml.  The standard .Net System.Collections.Generic.Dictionary does not provide Xml serialization, thus it is not
    possible to serialize/deserialize to/from Xml.  This dictionary is meant to merely extend the standard Dictionary
    to add that ability.

    ..note:: The SerializableDictionary is being kept for backward compatibility for classes already using it, however
    it is not the most elegant when serialized, and the act of serializing entire dictionaries to Xml is questionable at
    best.

    Using a SerializableDictionary is just like using the standard System.Collections.Generic.Dictionary::

            // create a new serializable dictionary and load with preliminary values
            // this is all standard C# initialization, nothing fancy here.
            var myDictionary = new SerializableDictionary<string, string>
                                   {
                                       {"Author", "John Doe"},
                                       {"Published", "2010"}
                                   };

    The key difference is that now you can serialize the dictionary to Xml::

            // see the Xml namespace of Core for a description of the Xml serializer utility,
            // though in truth it is just a pretty wrapper over core .Net Xml functionality
            string result = XmlUtility.PrettyPrintFromType(myDictionary);

            // this will print the Xml:
            //<Dictionary>
            //    <Entry>
            //        <Key>
            //            <string>Author</string>
            //        </Key>
            //        <Value>
            //            <string>John Doe</string>
            //        </Value>
            //    </Entry>
            //    <Entry>
            //        <Key>
            //            <string>Published</string>
            //        </Key>
            //        <Value>
            //            <string>2010</string>
            //        </Value>
            //    </Entry>
            //</Dictionary>
            Console.WriteLine("The xml for this dictionary is: ");
            Console.WriteLine(result);

    This example uses the
    *XmlUtility* class in the Xml namespace of Core, but it really just allows it to be serialized from .Net using
    the standard mechanisms.

    You can also de-serialize a SerializableDictionary from Xml once again either using .Net
    or the *XmlUtility* helper class in Core::

            // you can also de-serialize a dictionary from Xml by using:
            var dict = XmlUtility.TypeFromXml<SerializableDictionary<string, string>>(result);

            // this will show the value "John Doe".
            Console.WriteLine("The Author key has the value: " + dict["Author"]);

For more information, see the `API Reference <../../../../../Api/index.html>`_.