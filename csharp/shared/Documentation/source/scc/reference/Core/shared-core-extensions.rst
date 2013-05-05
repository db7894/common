=========================================================================
Core Extensions --- Convenience Extension Methods
=========================================================================
:Assembly: SharedAssemblies.Core.dll
:Namespace: SharedAssemblies.Core.Extensions
:Author: Galen Collins (`gcollins@bashwork.com <mailto:gcollins@bashwork.com>`_)
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.Core.Extensions
   :platform: Windows, .Net
   :synopsis: Extensions - Convenience Extension Methods

.. highlight:: csharp

.. index:: Extensions

Introduction
------------------------------------------------------------

The SharedAssemblies.Core.Extensions namespace contains a set of convenience extension methods
packaged into classes related to the types they extend.  Because extension methods can often pollute the
namespace into which they are imported and cause intellisense to bloat, it is recommended they be used 
judiciously and only where appropriate.

The majority of the extension methods are very simple convenience methods that simplify the reading of a piece of logic
or are simply fluent interface pass-thrus to utility classes such as the TypeConverter in the Conversions namespace or the 
XmlUtility in the Xml namespace.

Usage
------------------------------------------------------------

Since each of the classes in this assembly are generic utility classes, they each 
have their own unique usages that are described in the class sections below.

Classes
--------------------------------------------------------------

.. class:: AssemblyExtensions

.. index::
    pair: Assembly; Extension Methods

The **AssemblyExtensions** static class contains convenience methods that add functionality to the Assembly class that is used in Reflection to get information from a .Net assembly.  
Currently the only methods of **AssemblyExtensions** are convenience methods to make it easy to pull custom assembly-level attributes from an assembly.  

    .. method:: static T GetAttribute<T>(this Assembly callingAssembly) where T : Attribute
    
        :param T: type of the custom attribute to retrieve, must inherit from *Attribute*.
        :param callingAssembly: *(implicit)* the assembly to retrieve the custom attribute from.
        :returns: the custom attribute if found or null if not found.

        The **GetAttribute<T>(...)** searches through an assembly and looks for custom assembly-level attributes
        of the specified type **T** and all subclasses of **T**.  **GetAttribute<T>(...)** returns the first attribute it finds for that type and should only be called if
        you are expecting one and only one of those attributes.  Typically this is where the *AllowMultiple* property on the assembly
        level attribute is set to *false*.
        
        In general if the *AllowMultiple* property of the custom attribute is *true*, it is safer to use **GetAttributes<T>(...)**, and iterate through
        the list to find the desired attribute(s).  If *AllowMultiple* is set to *false*, either method is effective, but **GetAttribute<T>(...)** is cleaner
        since it returns at most one attribute, and null if not found.
        
        .. note:: The generic type parameter **T** in GetAttribute must be derived from the *Attribute* class.
        
        Assuming that the following assembly-level attribute is defined::
        
                [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
                public class MyCustomAttribute : Attribute
                {
                    public string MyName { get; set; }
                }

        And attributes of this type are declared in AssemblyInfo.cs::
        
                [assembly: MyCustomAttribute(MyName = "Jim")]
        
        Then a call to **GetAttribute<T>(...)** will return the first attribute that it finds.  Note that if there are multiples defined, 
        this is **not** necessarily the first one declared in AssemblyInfo.cs, so this should only be used if one attribute of the type is expected::
        
                Assembly thisAssembly = Assembly.GetCallingAssembly();

                // gets the first MyCustomAttribute it runs across in the assembly or null if not found.
                MyCustomAttribute attr = thisAssembly.GetAttribute<MyCustomAttribute>();

                // prints the name in the assembly level MyCustomAttribute, in this case, "Jim"
                Console.WriteLine("The name specified in the assembly was: " + attr.MyName);
                

    .. method:: static IEnumerable<T> GetAttributes<T>(this Assembly callingAssembly) where T : Attribute
    
        :param T: type of the custom attribute(s) to retrieve, must inherit from *Attribute*.
        :param callingAssembly: *(implicit)* the assembly to retrieve the custom attribute(s) from.
        :returns: an iterator over the custom attributes of type *T* that were found in the assembly.
                
        **GetAttributes<T>(...)** allows you to enumerate through all custom assembly attributes of type **T** and subclasses of type **T**, where **T** is derived from the class *Attribute*.  
        
        In general if the *AllowMultiple* property of the custom attribute is *true*, it is safer to use **GetAttributes<T>(...)**, and iterate through
        the list to find the desired attribute(s).  Note that since this returns an iterator, even if no custom attributes are found, it is still safe to process in a foreach loop.  
        
        If *AllowMultiple* is set to *false*, either method is effective, but **GetAttribute<T>(...)** is cleaner
        since it returns at most one attribute, and null if not found.
        
        Assuming that the following assembly-level attribute is defined::
        
                [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
                public class CounterAttribute : Attribute
                {
                    public string CounterName { get; set; }
                }

        And attributes of this type are declared in AssemblyInfo.cs::
        
                [assembly: Counter(CounterName = "Rate")]
                [assembly: Counter(CounterName = "Average")]
                [assembly: Counter(CounterName = "Number")]
            
        Then the following logic can be used to loop through all the custom attributes in the assembly::
        
        
                // get the assembly that called us
                Assembly thisAssembly = Assembly.GetCallingAssembly();
                
                // loop through all the custom attributes of type CounterAttribute (or derived).
                foreach(var attr in thisAssembly.GetAttributes<CounterAttribute>())
                {
                    // prints the name in the assembly level CounterAttribute
                    Console.WriteLine("A counter name specified in the assembly was: " + attr.CounterName);                
                }

.. class:: ConvertibleExtensions

.. index::
    pair: IConvertible; Extension Methods

The **ConvertibleExtensions** static class contains extension methods off of IConvertible objects that transform the TypeConverer's static methods into a more fluent interface.  
Basically, the following extension methods are shortcuts for the TypeConverter methods:

    .. method:: static T ConvertTo<T>(this IConvertible value, T defaultValue)
    
        :param T: the type to convert *value* to.
        :param value: *(implicit)* the *IConvertible* value to convert to type *T*.
        :param defaultValue: the value to use if *value* is *null* or *DBNull.Value*.
        :returns: the converted value of type *T*.
        :raises InvalidCastException: if cannot convert *value* to type *T*.
    
        The **ConvertTo(...)** method converts a value from its current type to the type *T* specified.  If *value* passed in is
        *null* or *DBNull.Value*, then *defaultValue* is returned.  If the conversion is not possible, an *InvalidCastException* is thrown.  
        
        To use *ConvertTo<T>(...)*, the object you are calling it from must implement IConvertible, which includes enums, primitives, and strings.
        
        This is simply a syntactically cleaner call to TypeConverter::
        
            int ssnAsInt = 123456789;
            string ssnAsString;

            // the ConvertTo extension method is much cleaner and easier to read
            ssnAsString = ssnAsInt.ConvertTo<string>();

            // than the explicit call to TypeConverter
            ssnAsString = TypeConverter.ConvertTo<string>(ssnAsInt);        
        
    .. method:: static T? ConvertToNullable<T>(this IConvertible value) where T : struct
    
        :param T: the type to convert *value* to, must be a value type.
        :param value: *(implicit)* the *IConvertible* value to convert to type *System.Nullable<T>*.
        :returns: the converted value of type *System.Nullable<T>*.
        :raises InvalidCastException: if cannot convert *value* to type *T*.
    
        The **ConvertToNullable(...)** method converts a value from its current type to an instance of *System.Nullable<T>* (or *T?* for short).  If the
        *value* passed in is *null* or *DBNull.Value*, then a null *System.Nullable<T>* will be returned.  To use this method, *T* must be a value
        type and not a reference type.  
        
        To use *ConvertTo<T>(...)*, the object you are calling it from must implement IConvertible, which includes enums, primitives, and strings.
        
        This is simply a syntactical shortcut to calling the TypeConverter directly::
        
            // note that it returns a nullable value ('?')
            int? ssnAsInt;
            string ssnAsString = "123456789";

            // the ConvertToNullable extension method is much cleaner and easier to read
            ssnAsInt = ssnAsString.ConvertToNullable<int>();

            // than the explicit call to TypeConverter
            ssnAsInt = TypeConverter.ConvertToNullable<int>(ssnAsString);        

            // if ssnAsString was null or DBNull.Value, ssnAsInt will be null, you can check this
            // by comparing to null or checking the HasValue attribute:
            if(ssnAsInt.HasValue)
            {
                ...
            }
            
        
    .. method:: static T ToEnum<T>(this IConvertible value, T defaultValue)
    
        :param T: the type to convert *value* to, must be an enumerated type (enum).
        :param value: *(implicit)* the *IConvertible* value to convert to type *T*.
        :returns: the converted value of type *System.Nullable<T>*.
        :raises InvalidCastException: if cannot convert *value* to type *T*.
    
        The **ToEnum<T>(...)** extension method converts a primitive or string to an enumerated value of type *T*.  The generic argument *T* must be 
        an enumerated type or an exception will be thrown.  Basically there are two conversion paths here:
        
            * If *value* is a numeric type, it is converted to int and the int is stored as the enum value.
            * If *value* is a string, it is parsed and matched against enum values.
            
        Note that if the numeric value does not exist in the enum, it is cast anyway and stored, this is to allow for logical combination
        of enum values on a *[Flags]* tagged enum and is consistent with the way int to enum casting works natively.
        
        However, if you are converting a string to the enum, it must exist or an exception is thrown.  Note that the *defaultValue* passed in
        is simply for if the *value* is *null* or *DBNull.Value*, it is not returned if an exception is thrown.
        
        This is simply a syntactical shortcut for *TypeConverter.ConvertToEnum<T>(...)*.  Consider the following example where an enum is defined for *Color*::
        
            // assuming this enum exists
            public enum Color
            {
                None,
                Red,
                Blue,
                Green,
                Yellow,
                White,
                Black
            }        
            
        The *ToEnum<T>(...)* method could be used to convert from string or int value to the enum value as follows::
        
            Color colorValue;
            string colorString = "Red";

            // the ConvertToNullable extension method is much cleaner and easier to read
            colorValue = colorString.ToEnum<Color>(Color.None);

            // than the explicit call to TypeConverter
            colorValue = TypeConverter.ConvertToEnum<Color>(colorString, Color.None);    
        
    .. method:: static T? ToNullableEnum<T>(this IConvertible value) where T : struct
    
        The **ToNullableEnum<T>(...)** extension method is nearly identical to **ToEnum<T>(...)** above, except that it returns a *System.Nullable<T>*, where
        *T* is an enumerated type.  If the conversion does not throw, it returns a valid value wrapped in *System.Nullable<T>*, if the *value* is
        *null* or *DBNull.Value*, it returns *null* wrapped as *System.Nullable<T>*.
        
        This is simply a syntactical shortcut for *TypeConverter.ConvertToNullableEnum<T>(...)*::

            // note that it returns a nullable value ('?')
            Color? colorValue;
            string colorString = "Red";

            // the ConvertToNullable extension method is much cleaner and easier to read
            colorValue = colorString.ToNullableEnum<Color>();

            // than the explicit call to TypeConverter
            colorValue = TypeConverter.ConvertToNullableEnum<Color>(colorString);

            // if colorString was null or DBNull.Value, colorValue will be null, you can check this
            // by comparing to null or checking the HasValue attribute:
            if(colorValue.HasValue)
            {
                ...
            }
    

.. class:: DateTimeExtensions

.. index::
    pair: DateTime; Extension Methods

The **DateTimeExtensions** are a set of extension methods that add functionality to the DateTime struct that parallels the old FinanceDateTime
class.  All the methods are invoked off of a DateTime instance.

    .. method:: static string ToString(this DateTime date, DateTimeFormat format)
    
        **ToString(...)** Allows for some specialized serializations using a *DateTimeFormat* enum that is also part of the *Extensions* namespace.  These
        formats parallel existing formats used in *FinanceDateTime*::

            // just invoke ToString, but using DateTimeFormat enum value.  NumericDate format is yyyyMMdd
            Console.WriteLine("DateTime in numeric format is: " 
                + DateTime.Now.ToString(DateTimeFormat.NumericDate));

    .. method:: static DayOfWeek GetDayOfWeekNDaysAgo(this DateTime date, int daysAgo)
    
        **GetDayOfWeekNDaysAgo(...)** determines what day of the week it was *daysAgo* number of days in the past::
                
            // build a DateTime for 12/1/1970 at 8:30 AM
            DateTime then = new DateTime(1970, 12, 1, 8, 30, 00);

            // now calculate some time differences:
            Console.WriteLine("What weekday was it 3 days ago? : " 
                              + now.GetDayOfWeekNDaysAgo(3));

    .. method:: static double GetMinutesSinceMidnight(this DateTime date)
    
        **GetMinutesSinceMidnight(...)** is a *DateTime* extension method which determines how many minutes it has been since midnight
        of the given date::
                              
            Console.WriteLine("How many minutes has it been since midnight today? : "
                              + DateTime.Now.GetMinutesSinceMidnight());

    .. method:: static double GetDaysSince(this DateTime date, DateTime then)
    
        **GetDaysSince(...)** determines the number of days that have passed from *then* to *date*::
    
            Console.WriteLine("How many days has it been from now since then? : "
                              + now.GetDaysSince(then));
                          
    .. method:: static double GetHoursSince(this DateTime date, DateTime then)
    
        **GetHoursSince(...)** determines the number of Hours that have passed from *then* to *date*::
    
            Console.WriteLine("How many Hours has it been from now since then? : "
                              + now.GetHoursSince(then));
                          
    .. method:: static double GetMinutesSince(this DateTime date, DateTime then)
    
        **GetMinutesSince(...)** determines the number of Minutes that have passed from *then* to *date*::
    
            Console.WriteLine("How many Minutes has it been from now since then? : "
                              + now.GetMinutesSince(then));
                          
    .. method:: static double GetSecondsSince(this DateTime date, DateTime then)
    
        **GetSecondsSince(...)** determines the number of Seconds that have passed from *then* to *date*::
    
            Console.WriteLine("How many Seconds has it been from now since then? : "
                              + now.GetSecondsSince(then));                              

.. class:: EnumerableExtensions

.. index::
    pair: IEnumerable; Extension Methods

The **EnumerableExtensions** are a set of extension methods that add functionality to the IEnumerable interface.  These 
extension methods help you process and handle lists of items in new and creative ways.  Some of the methods are actually 
filling in gaps that List<T> has that IEnumerable<T> does not.  All of these methods are chain-able, thus the results of one
method can be passed to another for further processing.

    .. method:: static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)

        The *ForEach(...)* ienumerable extension is probably the most useful and helpful of the bunch, simply put, it applies any Action<T>
        (delegate or lambda) to a complete collection::
        
                IEnumerable<int> somePrimes = new List<int> {2, 3, 5, 7, 9, 11, 13, 17, 19, 23};

                // In this example, we call Console.WriteLine on every int in the list
                somePrimes.ForEach(Console.WriteLine);

                // you can also specify a more advanced delegate
                somePrimes.ForEach(prime => Console.WriteLine("{0} squared is {1}", prime, prime*prime));

        .. note:: ForEach already exists in List<T> in the .Net framework, but it is not part of IEnumerable<T>, this extensions method brings that functionality to all collections that implement IEnumerable<T>.
        
    .. method:: static bool DoesNotContain<T>(this IEnumerable<T> collection, T value)

        The **DoesNotContain(...)** is really just syntactical sugar as a negation of *Contains(...)*::
        
                IEnumerable<int> somePrimes = new List<int> {2, 3, 5, 7, 9, 11, 13, 17, 19, 23};

                // works, but if you miss the ! visually may not realize this is checking for NOT Contains
                if(!somePrimes.Contains(25))
                {
                    Console.WriteLine("25 is not in the list.");
                }

                // reads a little easier
                if(somePrimes.DoesNotContain(25))
                {
                    Console.WriteLine("25 is not in the list.");                
                }
                
    .. method:: static IEnumerable<TResult> Zip<T1,T2,TResult>(this IEnumerable<T1> lhs, IEnumerable<T2> rhs, Func<T1,T2,TResult> combiner)

        The **Zip(...)** extension method takes two collections that may or may not be the same type and performs a combiner function that takes one element
        of each collection and returns an IEnumerable<T> iterator of the combinations::
        
                IEnumerable<int> someNumbers = new List<int>
                                                   {
                                                       1,
                                                       2,
                                                       3,
                                                       4,
                                                       5,
                                                       6,
                                                       7,
                                                       8,
                                                       9,
                                                       10
                                                   };

                IEnumerable<bool> arePrime = new List<bool>
                                                 {
                                                     false,
                                                     true,
                                                     true,
                                                     false,
                                                     true,
                                                     false,
                                                     true,
                                                     false,
                                                     false,
                                                     false
                                                 };


                // takes list of ints and list of bools and combines them with a lambda for results
                foreach(string result in someNumbers.Zip(arePrime, 
                    (num, isPrime) => string.Format("{0} is {1}", num, isPrime ? "prime" : "not prime")))
                {
                    Console.WriteLine(result);
                }         

        .. note:: The **Zip(...)** extension, unlike *ForEach(...)* does not automatically apply the result for all elements, they must be iterated over or pulled through, this is so that you can stop the operation when desired and not incur the cost of construction of the rest of the elements.

    .. method:: static IEnumerable<T> Pipeline<T>(this IEnumerable<T> source, Action<T> action[, Predicate<T> filter])

        The **Pipeline<T>(...)** extension method applies an Action<T> to each element in a collection as they are requested::
        
                IEnumerable<int> somePrimes = new List<int> {2, 3, 5, 7, 9, 11, 13, 17, 19, 23};

                // Pipeline only applies the action as the items are requested, thus
                // if the item is never reached in the collection, the Action<T> is never applied.
                foreach(int item in somePrimes.Pipeline(Console.WriteLine))
                {
                    if (item > 10)
                    {
                        break;
                    }
                }

        .. note:: Like **Zip(...)**, Pipeline only processes items as they are requested.  This is in contrast to *ForEach(...)* where it always applies the Action<T> to all elements.
        
        You cal also supply an optional Predicate<T> to **Pipeline(...)** that allows you to only take the Action<T> on items that meet the Predicate<T> criteria.  If the
        Predicate<T> returns false, the items are still iterated over, but the Action<T> will not be performed::
        
                // an example using a predicate to print only items that are > 10,
                foreach(int item in somePrimes.Pipeline(Console.WriteLine, item => item > 10))
                {                
                }

    .. method:: static void Pull<T>(this IEnumerable<T> source)

        The **Pull()** extension method is kind of a helper that simply performs a foreach loop on every item in the collection.
        Why would you want to do this?  Look at the previous example.  In that one, we had an empty foreach body because the action
        being performed was stated in the *Pipeline(...)* call, thus the foreach was simply a driver to *pull* the items through the *Pipeline*.
        
        Thus, if your actions are all specified and you just need to *pull* a collection through methods such as *Zip(...)* and *Pipeline(...)*, **Pull()** is
        your syntactical sugar::
        
                IEnumerable<int> somePrimes = new List<int> {2, 3, 5, 7, 9, 11, 13, 17, 19, 23};

                // You can use a foreach to pull items through an iterating generator,
                // but the empty body looks odd...
                foreach(int item in somePrimes.Pipeline(Console.WriteLine, item => item > 10))
                {                
                }

                // if all you need is to pull items through an iterating generator, Pull() looks cleaner
                somePrimes.Pipeline(Console.WriteLine, item => item > 10).Pull();
                
    .. method:: static IEnumerable<T> Every<T>(this IEnumerable<T> source, int byEvery)
                
        The **Every(...)** a pipeline extension method can be used to process every nth item in a collection.  By itself, this application may be
        somewhat limited, but combined with other linq or extension operations, this can become quite powerful::
        
                var names = new[]
                                {
                                    "Abby",
                                    "Nicholai",
                                    "Rhett",
                                    "Katie",
                                    "Lily",
                                    "Dobby",
                                    "Phoebe",
                                    "Puddy",
                                    "Hannah",
                                    "Molly",
                                    "Harvey"
                                };

                // prints Abby, Katie, Phoebe, Molly
                foreach(var name in names.Every(3))
                {
                    Console.WriteLine(name);
                }

        As another example, you can use **Every(...)** as a step operator with the *IntExtensions* generator methods::
        
                // outputs 2, 5, 8
                foreach(int i in 2.CountTo(10).Every(3))
                {
                    Console.WriteLine(i);
                }
                
    .. method:: static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> collection, Predicate<T> endCondition)
                
        The **TakeUntil(...)** pipeline extension method will break a collection iteration the moment the condition becomes true.  This is the opposite of 
        the Linq extension *TakeWhile(...)* and
        in fact either could be used in place of the other with the condition negated.  That is::
        
                foreach(someList.TakeWhile(keepGoingCondition))
                
        Is exactly the same as saying::
        
                foreach(someList.TakeUntil(!keepGoingCondition))
        
        It is simply a convenience to allow both to avoid negation logic that may
        obscure the readability.  In general you should pick the one that feels most natural.  Otherwise, it works just like while except the condition is a condition to
        stop upon instead of continue upon::
        
                var primes = new[]
                                {
                                    2,
                                    3,
                                    5,
                                    7,
                                    11,
                                    13,
                                    17,
                                    19,
                                    23,
                                    27,
                                    31
                                };

                // prints all primes <= 10
                foreach (var num in primes.TakeUntil(p => p > 10))
                {
                    Console.WriteLine(num);
                }

.. class:: IntExtensions

.. index:: 
    pair: Int; Extension Methods
    
The **IntExtensions** are integer extension methods that are actually generators.  These can be used to generate a sequence
of numbers that can be consumed in a foreach loop or other places that take an *IEnumerable<int>*.

    .. method:: static IEnumerable<int> Every(this int start[, int byEvery])
        
        The **Every()** method of integer is in parallel to the *EnumerableExtensions.Every()* selector extension method.  The difference
        here is that when invoked from an integer, it begins an unbounded sequence of either every number (no parameter) or every nth number
        if a skip parameter is provided::
        
                // print every number starting from 3 : 3, 4, 5, 6, ..., 100 and then manually break out.
                foreach(var num in 3.Every())
                {
                    // the loop above is infinite, this is just a manual break-out
                    if(num > 100)
                    {
                        break;
                    }

                    Console.WriteLine(num);
                }

        Specifying a skip value allows you to only process every nth number::
        
                // print every 5th number starting from 3 : 3, 8, 13, 18, ..., 98 and then manually break out.
                foreach (var num in 3.Every(5))
                {
                    // the loop above is infinite, this is just a manual break-out
                    if (num > 100)
                    {
                        break;
                    }

                    Console.WriteLine(num);
                }
                
        If this seems dangerous, you're part right, it is the responsibility of the user to bound their query, this can be done with items such as the 
        Linq extension method *Take(...)* or *TakeWhile(...)* among others, or the SharedAssemblies' Core *EnumerationExtensions.TakeUntil(...)* method::

                // takes 10 numbers from 3, counting by 5: 3, 8, 13, 18, 23, 28, 33
                foreach (var num in 3.Every(5).Take(7))
                {
                    Console.WriteLine(num);
                }
                
    .. method:: static IEnumerable<int> To(this IEnumerable<int> collection, int end)

        The **To(...)** extension method works only on integers because of its implied semantics.  This method is another bound method which 
        essentially allows iteration over the list until a value is great than the int specified in **To(...)**::
        
                // Counts from 1 to 20 by 3s: 1, 4, 7, 10, 13, 16, 19
                foreach (var num in 1.Every(3).To(20))
                {
                    Console.WriteLine(num);
                }
        
        Really, this::
        
                foreach(var num in 1.Every(3).To(10))
                
        Is semantically the same (and in fact implemented as) calling::
        
                foreach(var num in 1.Every(3).TakeWhile(i => i <= 10))
                
        Notice that **To(...)** constrains the max value and quits the moment the value is greater than the end value.  If the sequence is unordered, it will 
        essentially quit after it finds the first item greater than the end, regardless of whether items less than the end exist later in the list.
        
        It should also be emphasized that **To(num)** is totally different than **Take(num)**.  
        **To(num)** will take from the sequence until an item greater than num is found, whereas **Take(num)** takes num items from the sequence regardless of their value.  
        Thus:: 
            
                1.Every(3).To(10)

        yields three numbers less than ten (1,4,7) whereas:: 
                
                1.Every(3).Take(10) 
                
        yields ten numbers (1,4,7,10,13,16,19,22,25,28) with no bounds on their values.

    .. method:: static IEnumerable<int> To(this int start, int end)

        **To(...)** when called off of an integer is actually a generator for a bounded sequence::
        
                1.To(10)
                
        Note that this is exactly the same as::
        
                1.Every().To(10)
                
        The difference being when called right from the integer, it immediately creates a bounded sequence.  Whereas calling *Every(...)* creates an unbounded sequence which **To(...)** eventually bounds.
        
        There are many ways these constructs can be combined which effectively net the same results though with different levels of efficiencies, below are two different ways to generate a sequence of 
        numbers from 0 to 50 by twos::
        
                // less efficient because To generates every number from 0 to 50
                // and every filters every other item out.
                foreach(var i in 0.To(50).Every(2))
                {                
                    Console.WriteLine(i);
                }

                // more efficient, only every other number is ever generated, nothing 
                // need be filtered, fewer loop iterations
                foreach(var i in 0.Every(2).To(50))
                {
                    Console.WriteLine(i);
                }
        
        .. note:: It is best to choose the constraints first that most greatly reduce the solution space.  Thus if you have to apply both *Every(...)* and *To(...)* it is better to use the *Every(...)* generator first because it immediately cuts the amount of work in half, whereas *To(...)* must always check every element.
        
.. class:: ObjectExtensions

.. index:: 
    pair: Object; Extension Methods

The **ObjectExtensions** are a set of basic convenience methods that extend the object class.  Authors should be
very careful to limit extension methods on object because they will pollute the intellisense on ALL variables.
Thus extensions methods off of object should be as perfectly generic as possible and applicable to most if not
all objects.

    .. method:: static bool IsDefault<T>(this T value)

        The **IsDefault<T>(...)** method is useful for determining, generically, if the item passed in is the default
        value for its type.  **IsDefault<T>(...)** assumes T implicitly from the extension it's called on, so in that sense its an 
        object extension in that it can be called from any type, but actually it is not off of object itself but a generic off o
        f whatever type it is called from.  This is a minor distinction, but it makes a big difference.  
        
        Why is this important?  If::
        
                int value = 0;
                value.IsDefault();
                
        Were written as::
        
                public bool IsDefault(this object value) { ... }
                
        This would always return false because the primitive 7 would be boxed into an object, which would always return false even
        though zero is the actual default of int.  This is because it becomes an object in this method and the default of object is null.
        
        However, since it is written as a generic::
        
                public bool IsDefault<T>(this T value) { ... }
                
        It works correctly because it will *not* box the primitives, and thus can correctly tell if they are the default of their type 
        (in this case int).
        
        Below are more examples of working with **IsDefault<T>(...)**::
        
                double doubleValue = 0.0;
                bool boolValue = false;
                int intValue = 0;
                string stringValue = null;

                object objectWrappedDouble = doubleValue;

                // these are all true because they are their defaults
                Console.WriteLine("DoubleValue         = " + doubleValue.IsDefault());
                Console.WriteLine("BoolValue           = " + boolValue.IsDefault());
                Console.WriteLine("IntValue            = " + intValue.IsDefault());
                Console.WriteLine("StringValue         = " + stringValue.IsDefault());

                // this is false because the default of object is null (reference type)
                // even though it's boxing a double that is the default.
                Console.WriteLine("ObjectWrappedDouble = " + objectWrappedDouble.IsDefault());    
                

    .. method:: static bool IsNullOrDbNull(this object value)

        The **IsNullOrDbNull(...)** is a convenience method that checks if the current reference is *null* or *DBNull.Value*::
        
                // all primitives will return false because they get boxed to a non-null entity
                object objectValue = 3.14;
                Console.WriteLine("Double         = " + objectValue.IsNullOrDbNull());

                objectValue = false;
                Console.WriteLine("Bool           = " + objectValue.IsNullOrDbNull());

                objectValue = 7;
                Console.WriteLine("Int            = " + objectValue.IsNullOrDbNull());

                // reference types will return true if the reference is null
                objectValue = null;
                Console.WriteLine("Null           = " + objectValue.IsNullOrDbNull());

                // and if the object is DBNull.Value it will return true
                objectValue = DBNull.Value;
                Console.WriteLine("DBNull.Value   = " + objectValue.IsNullOrDbNull());    
                
    .. method:: static T ToType<T>(this object value[, T defaultValue])

        The **ToType<T>(...)** extension methods are syntactical sugar shortcuts to the static class *TypeConverter.ToType<T>(...)* family of methods.
        The code for them was moved to the separate static class (TypeConverter) to comply with the Single Responsibility Principle for the extension method
        class and to allow them to be used (as *TypeConverter*) without polluting the namespace if desired.
        
        The extension methods do provide a much more concise and fluent interface, though, and improve readability::

                // the extension method form of ToType<T>() is much more fluent and short.
                double doubleValue = intValue.ToType<double>();
                
        Is the same thing as saying this::
        
                // the explicit call to TypeConverter is harder to read and less fluent.
                double doubleValue = TypeConverter.ToType<int>(intValue);
        
        And they really improve readability when working with a DataReader, DataSet, or other artifacts
        that store everything as *object*.  The fluent interface exposed by the **ToType<T>(...)** family is much more concise and clean::
        
                // these are much more concise
                string name = reader["name"].ToType<string>();
                double salary = reader["salary"].ToType<double>();
                int yearsOfService = reader["years_of_service"].ToType<int>();
                
        Than the direct calls to the *TypeConverter.ToType<T>(...)* family::
        
                // these are much less fluent and much more bloated
                string name = TypeConverter.ToType<string>(reader["name"]);
                double salary = TypeConverter.ToType<string>(reader["salary"]);
                int yearsOfService = TypeConverter.ToType<string>(reader["years_of_service"]);

        All of the *TypeConverter.ToType<T>()* family are represented in the object extensions.     
        
    .. method:: static T TryToType<T>(this object value[, T defaultValue)
    
        The **TryToType<T>(...)** method is a syntactical shortcut to *TypeConverter.TryToType<T>(...)* and behaves in the same manner, with the
        obvious benefits of conciseness and readability mentioned in the description of method **ToType<T>(...)** above.
        
    .. method:: static T? ToNullableType<T>(this object value)
    
        The **ToNullableType<T>(...)** method is a syntactical shortcut to *TypeConverter.ToNullableType<T>(...)* and behaves in the same manner, with the
        obvious benefits of conciseness and readability mentioned in the description of method **ToType<T>(...)** above.
        
    .. method:: static T? TryToNullableType<T>(this object value)
        
        The **TryToNullableType<T>(...)** method is a syntactical shortcut to *TypeConverter.TryToNullableType<T>(...)* and behaves in the same manner, with the
        obvious benefits of conciseness and readability mentioned in the description of method **ToType<T>(...)** above.
        
    
.. class:: StringExtensions

.. index:: 
    pair: String; Extension Methods

The **StringExtensions** class is a set of basic convenience methods that extend the string class.  Authors should be
very careful to limit extension methods on string as it will pollute intellisense for all strings.  Thus creating an 
extension method for string that only works for telephone numbers is very limiting and doesn't add value to all strings 
in general.  Extension methods to string should be perfectly generic and apply to all strings.

    .. method:: static bool IsNullOrEmpty(this string input)

        The **IsNullOrEmpty(...)** extension method really is just a more fluent call to *string.IsNullOrEmpty(...)*::
        
                // this seems more fluent and natural and a bit shorter
                if(s.IsNullOrEmpty())
                {
                    s = "<n/a>";
                }
                
        Which is logically identical to the *string.IsNullOrEmpty(...)* illustrated below, but more concise::
        
                // this seems a little disjoint and not as fluent, plus a little longer.
                if(string.IsNullOrEmpty(s))
                {
                    s = "<n/a>";
                }
                
                // which itself is really shorthand for:
                if(s == null || s.Length == 0)
                {
                    s = "<n/a>";
                }

    .. method:: static bool IsLengthAtLeast(this string value, int length)

        The **IsLengthAtLeast(...)** extension method allows a way to check to see if a string is non-null and has a length at least as large as the 
        parameter specified::
        
                // checks to see if phone number is non null and length is at least 10 characters
                if(phoneNumber.IsLengthAtLeast(10))
                {
                    throw new ArgumentException("phoneNumber", "Must be at least 10 characters.");                
                }
                
        This is just shorthand for::
        
                // less concise
                if (phoneNumber != null && phoneNumber.Length >= 10)
                {
                    throw new ArgumentException("phoneNumber", "Must be at least 10 characters.");                
                }

    .. method:: static string Mask(this string sourceString[, char maskChar][, int numExposed])

        The **Mask(...)** extension method can be used to mask a string for viewing in a public forum, where you may not want to view all the characters but want an idea of
        what the string is.  Examples of this are account numbers where all but the last four characters are masked on billing statements and on-line applications.
        
        Mask lets you specify a mask character; by default it uses the character specified in *MaskExtensions.DefaultMaskCharacter* which is the asterisk ('*').  It also lets you 
        specify how many characters to leave exposed on the end - by default, it will mask all characters.  If the string to be masked is shorter or equal to the number exposed, no characters 
        are masked.  If the string is longer than the number of characters to expose, then all characters are masked except that last number to leave exposed.
        
        Below are some examples with varying length and mask combinations::
        
                // masks entire account number with default mask (*) = **********
                Console.WriteLine(accountNumber.Mask());

                // masks all but the last 5 character with default mask (*) = *****67890
                Console.WriteLine(accountNumber.Mask(5));

                // masks entire account number with specified mask = XXXXXXXXXX
                Console.WriteLine(accountNumber.Mask('X'));

                // masks all but the last 3 characters with specified mask = XXXXXXX890
                Console.WriteLine(accountNumber.Mask('X', 3));

                // if the number to expose is > length of string, nothing is masked = 1234567890
                Console.WriteLine(accountNumber.Mask(20));
                
    .. method static string Resolve(this string sourceString, IEnumerable<KeyValuePair<string,string>> tokenSet)
                
        The **Resolve(...)** method takes a collection of *KeyValuePair<string,string>* that lists keys to replace and the values to replace them with.  Since it
        takes in an *IEnumerable<KeyValuePair<string, string>>* as the collection, you may pass an array, Dictionary, SortedList, SortedDictionary, etc.::
        
                var substitutions = new Dictionary<string, string>
                                    {
                                        {"USER", "Joshua"},
                                        {"ADDRESS", "123 Memory Lane"},
                                        {"PHONE", "123-456-7890"}
                                    };

                var emailText = "Dear USER, we would like to mail you a brochure to ADDRESS";

                // Dear Joshua, we would like to mail you a brochure to 123 Memory Lane
                Console.WriteLine(emailText.Resolve(substitutions));    
            
.. class:: TypeExtensions

.. index:: 
    pair: Type; Extension Methods

The **TypeExtensions** are a set of basic convenience methods that extend the *Type* class.  Currently, there is only
one method in this collection:

    .. method:: static bool IsNullable(this Type type)

        * **IsNullable()** - *Determines if a given type is a System.Nullable<T> wrapper of a type.*
            
        To use **IsNullable()**, you simply invoke the method off of the Type in question::
        
                // no, int is not a nullable type
                Console.WriteLine("int : " + typeof (int).IsNullable());

                // yes, int? is nullable type (ie it is System.Nullable<int>)
                Console.WriteLine("int? : " + typeof(int?).IsNullable());

                // no, reference types are not System.Nullable wrappers
                Console.WriteLine("string : " + typeof(string).IsNullable());
                
        .. note:: Notice that reference types are not considered a Nullable type even though you can set reference types to null.  IsNullable simply checks to see if a type is a System.Nullable<T> generic specialization.
    
.. class:: XmlExtensions

.. index:: 
    pair: Xml; Extension Methods

The **XmlExtensions** are a set of extension methods that make it easy to call the *SharedAssemblies* *XmlUtility* class off of any object.  

    .. method:: static string ToXml(this object value[, bool shouldPrettyPrint])

        To use **ToXml(...)** you simply call it off of any object with an optional boolean parameter to specify whether to pretty print or not.  If you choose false, or 
        don't specify, this means you will get a string with no indentation or line breaks.  A pretty print parameter of true, on the other hand, will indent and
        separate each element onto its own line.
        
        Assuming we had this 3d Point class::
        
                public class Point
                {
                    public int X { get; set; }
                    public int Y { get; set; }
                    public int Z { get; set; }
                }
                
        We could serialize an instance like so::
        
                // create a Point (3,9,-5)
                var myPoint = new Point()
                                  {
                                      X = 3,
                                      Y = 9,
                                      Z = -5
                                  };

                // the xml will be: 
                // <?xml version="1.0" encoding="utf-8"?><Point><X>3</X><Y>9</Y><Z>-5</Z></Point>
                Console.WriteLine(myPoint.ToXml());

                // the pretty-printed xml will be:
                // <Point>
                //     <X>3</X>
                //     <Y>9</Y>
                //     <Z>-5</Z>
                // </Point>           
                Console.WriteLine(myPoint.ToXml(true));

    .. method:: static void ToXmlFile(this object value, string fileName)

        **ToXmlFile()** works similar to **ToXml()** except that it always pretty-prints and stores the result in a file instead of a string.

For more information, see the `API Reference <../../../../../Api/index.html>`_.        