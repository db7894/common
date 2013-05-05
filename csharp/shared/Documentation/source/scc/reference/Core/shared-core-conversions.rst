============================================================================
Core Conversions --- Type Conversions and Value Translations
============================================================================
:Assembly: SharedAssemblies.Core.dll
:Namespace: SharedAssemblies.Core.Conversions
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. highlight:: csharp

.. index:: Conversions

.. index:: Translations

.. module:: SharedAssemblies.Core.Conversions
   :platform: Windows, .Net
   :synopsis: Conversions - Type conversions and translations

Introduction
------------------------------------------------------------

The SharedAssemblies.Core.Conversions namespace has a set of utility classes that help in the common tasks of attempting
to convert a piece of data from one type to another.  Basically, there are two different types of operations:

* **Translation** - *Translates a piece of data from one type to another piece of data by specifying a translation between the two types and is not a mathematical or parsing operation*.
* **Conversion** - *Converts a piece of data from one type to another by parsing or performing a mathematical operation.*

Basically, Conversion is used to change an integer to a string representation and back or an int to a double, etc.
These conversions are purely mathematical or parsing operations and need no further information.  As an example,
the string "3.14" can be converted to a double 3.14 or an integer 3 without needing any additional information,
it is simply parsing or mathematical operations.

Translations, however, have no direct, inherent conversion and thus you must specify what each translation is.  As
an example, you may set up translations of a character type to an enum.  Perhaps where 'N' == OrderType.NewOrder, etc.
In this case, there is no direct parsing or conversion that can infer that, it simply is a mapping of possible values
to translated values.

Usage
------------------------------------------------------------

Since each of the classes in this assembly are generic utility classes, they each 
have their own unique usages that are described in the class sections below.

Classes
------------------------------------------------------------

.. class:: SharedAssemblies.Core.Conversions.Translator

    The **Translator<TFrom,TTo>** class implements the ITranslator interface, this interface contains method prototypes for:

    * **Translate** - *Translates an object of the TFrom type to an object of the TTo type if a translation exists.*
    * **ReverseTranslate** - *Performs a reverse translation from the TTo type to the TFrom type, depending on the underlying data structure, this may be less efficient than the forward translation.*
    * **Add** - *Adds a new translation to the translator.*
    * **Clear** - *Removes all translations from the translator.*
    * **[]** - *Index operator as a convenience in syntax, calls the Translate method.*

    Translators are very handy for mapping items from one type to another when there is no inherent parsing or
    other mathematical calculation that can be used to achieve the mapping.  For instance, a product ID of 100 may
    be used in one system to represent an AccountType.Checking in another system.  There is no way to parse or otherwise
    interpret 100 to achieve that enum value except hard-coding the enum value to 100, which is bad form as it raises a high
    level of coupling.

    You can convert any two sets of values, they can even have the same types.  For instance you could map an int product id from one system
    to an int product id of another system, or string abbreviations for month names to full month names ("Jan" => "January", etc).

    The Translator is the basic translator class, it currently has an underlying Dictionary<TFrom, TTo> as its key/value store which
    gives the best performance for nearly all list sizes.  The constructor of the Translator also allows you to specify *not-found defaults*
    for translation and reverse translation.  These are the values that will be returned if the object to be translated is not
    found in the translation list.  If these are not specified, they will be default(TTo) for Translate and default(TFrom) for ReverseTranslate.
    Note that for reference types, this will in effect be null and for primitives will be 0, and structures will be a structure with all members defaulted.

    Typically, since translations do not generally change, a Translator should be created *static readonly* so that it only
    exists once and never changes.  Once a translator is loaded, it is completely thread-safe, so any number of threads may access it
    simultaneously (as long as you do not call Add or Clear while other threads are using it of course).

    When you create the Translator, to ensure it is loaded only once, you should use the enumerable initializer syntax or load the items in the
    class static constructor.  Do not load a static Translator in a normal constructor or it will re-load it every time.  You can have an instance
    Translator if you wish, but it's really redundant to re-load it for each instance if the translation maps never change.  That's why we
    recommend having Translators as *static readonly* unless there is an overriding reason not to and using the initializer syntax (strongly preferred to reduce clutter) or a
    static constructor.

    The following examples show the many ways a Translator can be instantiated.  The first example shows how to instantiate a
    Translator using the enumerable initializer syntax, this behind the scenes is equivalent to creating the object and then
    calling Add(...) repeatedly on the Translator for each key/value pair.  The nice thing about using this syntax is it allows you
    to Add the items at the declaration of the Translator, avoiding clutter in a static constructor::

        // typically you want to make a translator static readonly as it is thread-safe once
        // loaded and you only need load it once for all uses
        private static readonly Translator<int, AccountType> _accountTranslator
            = new Translator<int, AccountType>
                {
                    {100, AccountType.Checking},
                    {200, AccountType.Savings},
                    {210, AccountType.MoneyMarket},
                    {1000, AccountType.Cd},
                    {2000, AccountType.Loan},
                    {2100, AccountType.RevolvingCredit}
                };

    Note that this will have a default translation of whatever the default(TFrom) and default(TTo) are.  So if you wish to override these,
    you can specify them in the constructor parameter list like so::

            // adding parameters to the constructor lets you specify a default value
            // for reverse translation if not found (in this case 0), and a default
            // value for translation if not found (in this case AccountType.Unknown)
            private static readonly Translator<int, AccountType> _accountTranslator
                = new Translator<int, AccountType>(0, AccountType.Unknown)
                    {
                        // etc...
                    };

    If you wish a value type to instead return null (instead of the default enum value or default structure etc) you can use a nullable as your translator type.
    In this way it will return null instead of a default enum value if not found::

            // by specifying a nullable enum, null will be returned if translation not found
            // instead of whatever the zero-value of the enum is.
            private static readonly Translator<int, AccountType> _accountTranslator 
                = new Translator<int, AccountType?>();

    The initializer at time of declaration of the Translator above is preferred, however you can use
    a static constructor if you prefer::

            // static constructors are run only once when a class is first accessed
            static MyClass()
            {
                // alternatively, you can do your Adds in a static constructor if you wish,
                // though this seems more cluttered than just using an initializer
                _accountTranslator = new Translator<int, AccountType>
                    {
                        {100, AccountType.Checking},
                        {200, AccountType.Savings},
                        {210, AccountType.MoneyMarket},
                        {1000, AccountType.Cd},
                        {2000, AccountType.Loan},
                        {2100, AccountType.RevolvingCredit}
                    };

                // == OR, if you really hate initializer syntax... ==

                // alternatively, you can do your Adds in a static constructor if you wish,
                // though this seems more cluttered than just using an initializer
                _accountTranslator = new Translator<int, AccountType>();
                _accountTranslator.Add(100, AccountType.Checking);
                _accountTranslator.Add(200, AccountType.Savings);
                _accountTranslator.Add(210, AccountType.MoneyMarket);
                _accountTranslator.Add(1000, AccountType.Cd);
                _accountTranslator.Add(2000, AccountType.Loan);
                _accountTranslator.Add(2100, AccountType.RevolvingCredit);
            }

    And the various ways it can be used to forward and reverse translate values::

            // you can use the [] operator as a short-cut for Translate.
            Console.WriteLine("Product ID 100 is: " + _accountTranslator[100]);

            // or you can call Translate explicitly.
            Console.WriteLine("Product ID 2100 is: " + _accountTranslator.Translate(2100));

            // if the translation is not found, the default value will be returned.
            Console.WriteLine("Product ID 999 is: " + _accountTranslator[999]);

            // you can also request a reverse translation:
            Console.WriteLine("Checking has a Product ID of: "
                + _accountTranslator.ReverseTranslate(AccountType.Checking));

    There is also a **Translator<TFrom,TTo,TDictionary>** which is a more generic Translator and allows the user to specify whatever type they want to use as a translation
    dictionary as long as that type implements the IDictionary interface.  This includes such types as Dictionary, SortedList,
    SortedDictionary, etc.

    .. note:: The Translator<TFrom, TTo> is actually just a "shortcut" for Translator<TFrom, TTo, Dictionary<TFrom, TTo>>.

    The following examples show the many ways the Translator can be instantiated with a SortedList dictionary type::

            private static readonly Translator<string, string, SortedList<string, string>> _monthTranslator
                = new Translator<string, string, SortedList<string, string>
                    {
                        { "Jan", "January" },
                        { "Feb", "February" },
                        // etc.
                    };

.. class:: SharedAssemblies.Core.Conversions.TypeConverter

    The **TypeConverter** is a static utility class that performs conversions from one type to another type.  Such conversions involve either IConvertibles,
    parsing of string values to numerics, or casting as appropriate.

    .. note:: In all cases of conversion, The value *DBNull.Value* is considered the same as if a null reference were passed in.

    In general the, **ToType<T>** method line should be the preferred method when you are unsure what type of value you will be given.  
    This is because they examine the from and to types in question and choose the correct method of conversion and attempt a conversion.  

    .. method:: T ToType<T>(object value, T defaultValue)

        The **ToType<T>** method line always attempts to choose the best conversion possible by trying the following in order:

        * **Value is null or DBNull.Value** - *Returns the default value (provided or implicit).*
        * **T is string** - *Returns result of value.ToString().*
        * **Value is IConvertible and T is enumerated** - *Returns result of TypeConverter.ConvertToEnum<ToType>(value).*
        * **Value is IConvertible and T is not enumerated** - *Returns result of TypeConverter.CovertTo<ToType>(value).*
        * **All other cases** - *Attempts to cast and return value to type T*.

        When using the **ToType<T>** static method, all you need provide is the type you would like to convert to.  If a conversion is successful, the
        converted value will be returned.  If the conversion is not possible, an InvalidCastException is thrown in the final stage when it attempts to
        cast to type *T* as a last-ditch conversion effort.  
        
        .. note:: It may seem like a negative to throw instead of returning a default, but this was a design decision intended to make sure that logically impossible conversions caused by faulty logic were not being swept under the carpet via a default value.  
        
        The following are several examples of using **ToType<T>**::

                // recognizes "3.14" is string (IConvertible) and parses
                double result = TypeConverter.ToType<double>("3.14");

                // recognizes type T is string and uses ToString()
                string stringResult = TypeConverter.ToType<string>(DateTime.Now);

                // create a sub-class and store as a base reference for illustrative purposes
                BaseClass subAsBaseReference = new SubClass();

                // since T is not IConvertible or string, attempts a cast, in this case is good.
                SubClass subReference = TypeConverter.ToType<SubClass>(subAsBaseReference);

                // since T is not ICOnvertibe or string, attempts a cast, in this case it's bad and throws
                // because there is no conversion to int from SubClass
                int intResult = TypeConverter.ToType<int>(subReference);
                
        .. note:: If the *TypeConverter.ToType<T>(...)* syntax seems tedious, please note that there are extension methods (described below in the Extensions namespace) that access this class more fluently and naturally.
        
        The following are examples of implicit and explicit default values if value is *null* or *DBNull.Value*::
        
                // note that here the implicit default will be default(T) which is 0.0 for doubles.
                double result = TypeConverter.ToType<double>(value);

                // note that since value here is null, the explicit default of -1.0 will be returned
                result = TypeConverter.ToType<double>(value, -1.0);

                // Also note that DBNull.Value is treated the same as null
                result = TypeConverter.ToType<double>(DBNull.Value, -1.0);
                
                // in this case, T is a reference type, so the implicit default(T) will be null.
                string strResult = TypeConverter.ToType<string>(value);
                
        Note that **ToType<T>** is especially handy when the value you are trying to convert is an object or otherwise
        unknown.  This is especially true when dealing with DataReaders and DataSets where the underlying values
        in the rows and columns are stored as object::
        
                // ToType<T> comes in most handy when processing data reader or other database results
                // or object references where you usually do not know the exact underlying type,
                // in these cases you COULD cast, but it would be very dangerous as a cast from
                // a double in an object reference to an int will FAIL!
                double rateOfPay = TypeConverter.ToType<double>(myDataReader["rate_of_pay"]);
                
        When pulling from these type of artifacts, a direct cast can
        be deadly since you must have your type exact. Consider that if you store a double in an object, and then cast that object to int, it will *FAIL*.  This
        may seem counter-intuitive as direct casts from double to int will succeed.  The reason for this is that 
        once the double is stored in an object, it is boxed in a new class, and to convert that class to int
        will always fail.  **ToType<T>**, however, insulates you from this and handles it correctly::
        
                // create a double and store it in an object reference
                object doubleAsObject = 3.14;

                try
                {
                    // direct cast will crash and burn because object stores a double, not an int
                    // even though there exists a conversion.
                    int badIntResult = (int) doubleAsObject;
                }
                catch(InvalidCastException) { }

                // but the ToType<T> family of methods insulates against this by recognizing the
                // object contains an IConvertible (double) and attempts instead a conversion
                // instead of a direct cast.
                int goodIntResult = TypeConverter.ToType<int>(doubleAsObject);           

    .. method:: T? ToNullableType<T>(object value)

        **ToNullableType<T>** behaves the same as *ToType<T>* except that T must be a value type and it will return a *System.Nullable<T>* instead of *T*.  
        If the value is *null* or *DBNull.Value*, it will return *null* instead of *default(T)*::

                // put a null in the value
                string value = null;

                // notice that the default value that will get returned from ToNullableType<T>
                // is always null, this is because if a different default were needed you
                // can simply call ToType<T> with that default, the only reason to want
                // ToNullableType<T> is if you want null as a default.
                intValue = TypeConverter.ToNullableType<int>(value);

    .. method:: T TryToType(object value T defaultValue)

        **TryToType<T>** perform the same conversion algorithm as *ToType<T>*, the difference being that 
        the *TryToType<T>* and methods will return the default on failure to convert as opposed to the InvalidCastException
        that *ToType<T>*::
        
                int intResult;
                
                // ToType<T> throws InvalidCastException if no conversion possible
                // even if a default is specified
                intResult = TypeConverter.ToType<int>(subReference, -1);

                // TryToType<T> instead returns the default if no conversion possible
                intResult = TypeConverter.TryToType<int>(subReference, -1);
                
        .. note:: Even though ToType<T> throws, it is **strongly** preferred to TryToType<T> because the defaulting behavior of TryToType<T> may accidentally mask some severe logic problems in the code where a conversion may never be possible and hide it with a tidy default.

    .. method:: T? TryToNullableType(object value)

        **TryToNullableType<T>** perform the same conversion algorithm as *ToNullableType<T>*, the difference being that 
        the *TryToNullableType<T>* and methods will return **null** on failure to convert as opposed to the InvalidCastException
        that *ToNullableType<T>*::
        
                int? intResult;
                
                // ToNullableType<T> throws InvalidCastException if no conversion possible
                // even if a default is specified
                intResult = TypeConverter.ToNullableType<int>(subReference);

                // TryToNullableType<T> instead returns null if no conversion possible
                intResult = TypeConverter.TryToNullableType<int>(subReference);
                
        .. note:: Even though ToNullableType<T> throws, it is **strongly** preferred to TryToNullableType<T> because the defaulting behavior of TryToNullableType<T> may accidentally mask some severe logic problems in the code where a conversion may never be possible and hide it with a tidy default.

    .. method:: T ConvertTo<T>(IConvertible value, T defaultValue)

        The **ConvertTo<T>** method converts an IConvertible value to another type using the .Net core type converter.
        So why does **ConvertTo<T>** exist when we already have *ToType<T>* and related methods?
        The reason is simple, **ConvertTo<T>** is *much* more efficient if you already know you have an IConvertible 
        values (all primitives, enums, and strings).  This is because it doesn't have to go through the 4-stage algorithm 
        *ToType<T>* does to determine the correct conversion for the given value and to-type.  
        
        However, that said, ToType<T> is much safer and should be used whenever the type of the given value is unknown or uncertain
        as in database results.

        **ConvertTo<T>** is invoked just like *ToType<T>*, except that the value must be *IConvertible*::

                // note that ConvertTo<T> only accepts IConvertible values, not object.
                int result = TypeConverter.ConvertTo<int>("3.14", -1);
                
    .. method:: T? ConvertToNullable(IConvertible value)
                
        **ConvertToNullable<T>** is the *ConvertTo<T>* counterpart of *ToNullableType<T>*, except that the value must be *IConvertible*::

                // note no default because the default will be null on nullables.
                int? nullableResult = TypeConverter.ConvertToNullabe<int>("3.14");
                

    .. method:: T ConvertToEnum<T>(IConvertible value, T defaultValue)

        **ConvertToEnum<T>** attempts to convert an IConvertible type to an enumerated value of the given type.  
        If the type specified is not an enumerated type, an ArgumentException will be thrown.  If the value passed in is a string, then the 
        string will be parsed to see if it matches one of the enum's values.  If the value passed in is not a string, it will be converted to
        an integer and then that integer value of the enum will be used::
        
                // assuming this enum
                enum ActionType
                {
                    Unknown,
                    New,
                    Change,
                    Delete
                }

                ...

                // result will be ActionType.New since "New" is a string and will parse.
                ActionType result = TypeConverter.ConvertToEnum<ActionType>("New", ActionType.Unknown);

                // here the 2.4 is converted to int value of 2, which equates to ActionType.Change
                result = TypeConverter.ConvertToEnum<ActionType>(2.4, ActionType.Unknown);

        .. note:: If a string value is provided that cannot be parsed to an enum value, an exception is thrown.  However, if an out-of-range numeric value is given, it will return that value as an integer in the enum result.  This is consistent with casting behavior between ints and enums.

    .. method:: T? ConvertToNullableEnum(IConvertible value)

        **ConvertToNullableEnum<T>** is the nullable counterpart to *ConvertToEnum<T>*.
        
For more information, see the `API Reference <../../../../../Api/index.html>`_.        