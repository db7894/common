=========================================================================
 Validate.This
=========================================================================

-------------------------------------------------------------------------
 Summary
-------------------------------------------------------------------------

Validate.This is a validation utility library that gives you all the
expressive power of a fluent interface without forcing you into someone
else's conventions. Just tell the library what you would like validated,
and it will do all the hard word for you.

-------------------------------------------------------------------------
 Example Usage
-------------------------------------------------------------------------

Before we go too far, a quick example of how the library is used may
answer a number of questions, so here we go::

public class TestClass
{
  	public List<string> NickNames;
	public string FirstName;
	public int Age;
	public char Field;
	public DateTime Birthday;
	public bool Insured;
	public List<string> Collection;
}

var validator = Validate.That<TestClass>()
	.Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").And()
	.EachItem(x => x.NickNames).IsValid(FormValidationType.Name).And()
	.Property(x => x.Age).IsEven().IsLessThan(65).And()
	.Property(x => x.Field).IsUppercase().And()
	.Property(x => x.Collection).ContainsOneOf("2", "3", "5").And()
	.Property(x => x.Insured).IsTrue().And()
	.Property(x => x.Birthday).IsPast().And()
	.Compile().Value;

var handle = new TestClass { ... };
var result = validator.Validate(handle);

-------------------------------------------------------------------------
 Performance
-------------------------------------------------------------------------

Since I have to compile the expression trees before they can be used, the
first call to a given validator will be about two orders of magnitude
slower than the static validation code. However, after that the value will
be cached and the overhead of using the same validator the extra overhead
of dealing with the framework and generating result objects (1 to 1.5
times manual binary validation code without response objects).

=========================================================================
 The Validation Language
=========================================================================

Every validation expression starts with one of the following which builds
the initial ValidationContext::

  Validate.That<SingleType>()
  Validate.That<SingleType>("name")
  Validate.That<SingleType>("name", new ValidationOptions())

  Validate.ThatAll<CollectionType>()
  Validate.ThatAll<CollectionType>("name2")
  Validate.ThatAll<CollectionType>("name2", new ValidationOptions())

The supplied values are (in order) the value to validate, the optional
cache lookup name override (more on that later), and the ValidationOptions
collection to use (defaults to sane values).

-------------------------------------------------------------------------
 IValidationContext.Property(x => x.Property)
-------------------------------------------------------------------------

This is the main mechanism to validate nested objects and is used as
follows::

    Validate.That<SomeType>()
	    .Property(x => x.FirstName)...

You can then apply the validation rules that should be run against that
property of the parent object.  In order to finish a validation chain,
simply use the `And` method::

    Validate.That<SomeType>()
	    .Property(x => x.FirstName).IsUpperCase().And()
		...

You can overload two parts of the returned error message (in case of
a validation error): the property name and the error message entirely:: 

    .Property(x => x.FirstName, "First Name")
	.And("Validation failed for the First Name property")

-------------------------------------------------------------------------
 IValidationContext.EachItem(values)
-------------------------------------------------------------------------

This is the same as the property method, however, it is used for
property collections. It allows you to use the same validation rules for
each element in the collection by projecting the collection to a single
element for you::

    Validate.That<SomeType>()
	    .EachItem(x => x.Collection).IsUppercase.And()

-------------------------------------------------------------------------
 IValidationContext.This()
-------------------------------------------------------------------------

This is a simple way to project the validate type to the IPropertyContext
without having to use an identity property::

    // instead of this
    Validate.That<string>()
	    .Property(x => x).IsUpperCase().And()
		.Compile();

	// we can use this which reads a little bit better
    Validate.That<string>()
	    .This().IsUpperCase().And()
		.Compile();


-------------------------------------------------------------------------
 IValidationContext.With(options)
-------------------------------------------------------------------------

This is a simple way to add options to the validator in a way that is a
little bit more readable::

    Validate.That<SomeType>(options: new ValidationOptions { ... })
	    .Compile();

	Validate.That<SomeType>()
	   .With(new ValidationOptions { ... })
	   .Compile();

-------------------------------------------------------------------------
 IValidationContext.StopOnFirstFailure()
-------------------------------------------------------------------------

This is a readable way to set the flag in the ValidationOptions which
causes the validation to abort after the first validation error is
encountered.

-------------------------------------------------------------------------
 IValidationContext.ThrowOnFailure()
-------------------------------------------------------------------------

This is a readable way to set the flag in the ValidationOptions which
causes the validation to throw in case a validation error has been
encountered.  The default behavior is to return the errors and not throw.

-------------------------------------------------------------------------
 IValidationContext.Compile()
-------------------------------------------------------------------------

Before the validator can be used, it must be compiled. This is because
expression trees are used for the fluent interface. The result of this
method is an operation result which contains the compiled validator as
well as a boolean result of its compilation::


var validator = Validate.That<TestClass>()
	.Property(x => x.FirstName).IsNotNullOrEmpty().Contains("Mr").And()
	.Compile();

if (validator.IsSuccessful)
{
    return validator.Value;
}

=========================================================================
 Validation Rules
=========================================================================

Although you can specify your own validation functions, you may never
have to as the library already has a large collection of validation rules
that should meet most any need.  What follows is a quick discussion of the
available rules.

-------------------------------------------------------------------------
 Between Validation
-------------------------------------------------------------------------

The between validation allows you to check if a given value is between
two values. Simply define a minimum value, a maximum value, and a
comparison rule of inclusive or exclusive. Note that this works for all
types that implement IComparable:

* IsBetween(min, max, inclusion)
* IsNotBetween(min, max, inclusion)

Example::

    Property(x => x.Value).IsBetween(0, 10)
    Property(x => x.Value).IsBetween(DateTime.Now.AddDays(-1), DateTime.Now)
    Property(x => x.Value).IsBetween(false, true, RangeComparison.Inclusive)
    Property(x => x.Value).IsBetween('A', 'Z', RangeComparison.Exclusive)

**For those of you that forgot, inclusive includes the edge values in the
comparison whie exlusive does not.**

-------------------------------------------------------------------------
 Bool Validation
-------------------------------------------------------------------------

These validations simply check if the specified value is true or false.
There are also overloads to check if a predicate function (Func<bool>)
evaluates to true or false allowing one to use lazy evaluation:

* IsTrue
* IsFalse

Example::

    Property(x => x.Field).IsTrue()
    Property(x => x.Predicate).IsTrue()
    Property(x => x.Field).IsFalse()	
    Property(x => x.Predicate).IsFalse()

-------------------------------------------------------------------------
 Character Validation
-------------------------------------------------------------------------

These validations perform simple checks on single characters:

* IsUppercase
* IsLowercase
* IsDigit
* IsPrintable (between ' ' and '~')

Example::

    Property(x => x.Field).IsUpperCase()
    Property(x => x.Field).IsLowerCase()
    Property(x => x.Field).IsPrintable()
    Property(x => x.Field).IsDigit()

-------------------------------------------------------------------------
ContainsAllElementsValidation
-------------------------------------------------------------------------

These validations allow one to check if a given collection of elements
does or does not contain one or more elements. These can be used in
params or IEnumerable form. Note that this works for all types that
implement IComparable:

* ContainsAllOf(elements)
* DoesNotContainAllOf(elements)

Example::

    Property(x => x.Collection).ContainsAllOf(1,2,3)
    Property(x => x.Collection).ContainsAllOf(new [] { 1,2,3 })
    Property(x => x.Collection).DoesNotContainAllOf(1,2,3)
    Property(x => x.Collection).DoesNotContainAllOf(new [] { 1,2,3 })

-------------------------------------------------------------------------
ContainsAnyElementsValidation
-------------------------------------------------------------------------

These validations allow one to check if a given collection of elements
does or does not contain one element from a list. These can be used in
params or IEnumerable form. Note that this works for all types that
implement IComparable:

* ContainsAnyOf(elements)
* DoesNotContainAnyOf(elements)

Example::

    Property(x => x.Collection).ContainsAnyOf(1,2,3)
    Property(x => x.Collection).ContainsAnyOf(new [] { 1,2,3 })
    Property(x => x.Collection).DoesNotContainAnyOf(1,2,3)
    Property(x => x.Collection).DoesNotContainAnyOf(new [] { 1,2,3 })

-------------------------------------------------------------------------
ContainsElementValidation
-------------------------------------------------------------------------

These validations allow one to check if a given collection of elements
does or does not contain a given element. These can be used in
params or IEnumerable form. Note that this works for all types that
implement IComparable:

* Contains(elements)
* DoesNotContain(elements)

Example::

    Property(x => x.Collection).Contains(1)
    Property(x => x.Collection).DoesNotContain(1)

-------------------------------------------------------------------------
DateTimeValidation
-------------------------------------------------------------------------

These validations allow one to check if a given date occurs in the past
or the future:

* IsFuture()
* IsPast()

Example::

    Property(x => x.Date).IsFuture()
    Property(x => x.Date).IsPast()

-------------------------------------------------------------------------
DoubleValidation
-------------------------------------------------------------------------

These validations allow one to perform fuzzy equality checks on doubles.
This means that you see if a value is equal to another value +/- some
fuzz factor:

* IsNear(value, fuzz=1.0)
* IsNotNear(value, fuzz=1.0)

Example::

    Property(x => x.Number).IsNear(100, 1.25)
    Property(x => x.Number).IsNotNear(0)

-------------------------------------------------------------------------
EqualsOneOfValidation
-------------------------------------------------------------------------

These validations allow one to check if a given element is equal to one
of the values from a list. These can be used in params or IEnumerable
form. Note that this works for all types that implement IComparable:

* EqualsOneOf(elements)
* DoesNotEqualsOneOf(elements)

Example::

    Property(x => x.Value).EqualsOneOF(1,2,3)
    Property(x => x.Value).EqualsOneOF(new [] { 1,2,3 })
    Property(x => x.Value).DoesNotEqualsOneOf(1,2,3)
    Property(x => x.Value).DoesNotEqualsOneOf(new [] { 1,2,3 })

-------------------------------------------------------------------------
EqualToValidation
-------------------------------------------------------------------------

These validations allow one to check if a given element is equal to
another element Note that this works for all types that implement
IComparable:

* IsEqualTo(element)
* IsNotEqualTo(element)

Example::

    Property(x => x.Value).IsEqualTo(1)
    Property(x => x.Value).IsNotEqualTo(1)

-------------------------------------------------------------------------
GreaterThanValidation
-------------------------------------------------------------------------

This validation checks to see if a given element is greater than another
element. Note that this works for all types that implement IComparable:

* IsGreaterThan(value, inclusion)

Example::

    Property(x => x.Value).IsGreaterThan(10)
    Property(x => x.Value).IsGreaterThan(DateTime.Now)
    Property(x => x.Value).IsGreaterThan(false, RangeComparision.Inclusive)
    Property(x => x.Value).IsGreaterThan('A', RangeComparision.Exclusive)

-------------------------------------------------------------------------
IgnoreValidation
-------------------------------------------------------------------------

This validation is used to be explicit that this property is not
validated. It results in a noop:

* IgnoresValidation()

Example::

    Validate.That(value).IgnoresValidation()
    Property(x => x.Value).IgnoresValidation()

-------------------------------------------------------------------------
IntegerValidation
-------------------------------------------------------------------------

This validation is used to perform a number of checks on integer types:

* IsEven
* IsOdd
* IsMultipleOf(value)
* IsNotMultipleOf(value)

Example::

    Property(x => x.Number).IsEven()
    Property(x => x.Number).IsOdd()
    Property(x => x.Number).IsMultipleOf(3)
    Property(x => x.Number).IsNotMultipleOf(9)

-------------------------------------------------------------------------
IsNotNullValidation
-------------------------------------------------------------------------

This validation is used to check if a supplied value is not null or
empty. It works on nullable structs as well as reference types:

* IsNotNull
* IsNotEmpty
* IsNotNullOrEmpty

Example::

    Validate.That<Struct?>(value).IsNotNull()
    Validate.That<Reference>(value).IsNotNull()
    Property(x => x.Struct).IsNotNull()
    Property(x => x.Reference).IsNotNull()
    Property(x => x.Collection).IsNotEmpty()
    Property(x => x.Collection).IsNotNullOrEmpty()    

-------------------------------------------------------------------------
IsValidFormValidation
-------------------------------------------------------------------------

This validation is used to check if a string value is a correct form
element. This uses compiled regular expressions behind the scenes and
there are a number defined:

* IsValid(type)

Example::

    Property(x => x.Field).IsValid(FormValidationType.Address)

The following are valid options for FormValidationType:

* Address,	
* Alpha,
* AlphaAndSpaces,
* AlphaAndDigits,
* AlphaAndDigitsAndSpaces,
* AlphaAndDigitsAndUnderscores,
* AlphaAndDigitsAndUnderscoresAndSpaces,
* AlphaAndDigitsAndSpacesAndSpecialCharacters,
* City,
* Currency,		
* Date,
* Email,
* FreeFormText,
* FreeFormLatinAndChineseText,
* Integer,
* IntegerUnsigned,		
* IpAddressIPv4,
* IpAddressIPv6,		
* Name,
* Number,
* Password,
* Phone,
* SpecialCharacters,
* Ssn,
* TaxIdentificationNumber,
* Url,
* ZipCode

-------------------------------------------------------------------------
LengthValidation
-------------------------------------------------------------------------

This validation is used to check if the supplied collection is within
the specified bounds. Simply define a minimum value, a maximum value, and a
comparison rule of inclusive or exclusive.::

* HasLength(min=int.MinValue, max=int.MaxValue, inclusion)

Example::
 
    Property(x => x.Collection).HasLength(5, 10)
    Property(x => x.Collection).HasLength(max=100)
    Property(x => x.Collection).HasLength(min=20, RangeComparison.Exclusive)

-------------------------------------------------------------------------
LessThanValidation
-------------------------------------------------------------------------

This validation checks to see if a given element is less than another
element. Note that this works for all types that implement IComparable:

* IsLessThan(value, inclusion)

Example::

    Property(x => x.Value).IsLessThan(10)
    Property(x => x.Value).IsLessThan(DateTime.Now)
    Property(x => x.Value).IsLessThan(false, RangeComparision.Inclusive)
    Property(x => x.Value).IsLessThan('A', RangeComparision.Exclusive)

-------------------------------------------------------------------------
RegularExpressionValidation
-------------------------------------------------------------------------

This validation is used to check if a string matches or does not match
the supplied regular expression. This allows you to pass a string or a
pre compiled regular expression::

* Matches(expression)
* DoesNotMatch(expression)

Example::

    Property(x => Name).Matches("Mr\.\s.*)
    Property(x => Name).Matches(new Regex("Mr\.\s.*))
    Property(x => Name).DoesNotMatch("Mr\.\s.*)
    Property(x => Name).DoesNotMatch(new Regex("Mr\.\s.*))

-------------------------------------------------------------------------
StringValidation
-------------------------------------------------------------------------

This validation is used to check perform a number of checks on string
types:

* IsNotNullOrEmpty
* Contains(value)
* DoesNotContain(value)
* StartsWith(value)
* DoesNotStartWith(value)
* EndsWith(value)
* DoesNotEndWith(value)

Example::

    Property(x => x.Field).IsNotNullOrEmpty()
    Property(x => x.Field).Contains("type")
    Property(x => x.Field).DoesNotContain("ssn")
    Property(x => x.Field).StartsWith("Mr.")
    Property(x => x.Field).DoesNotStartWith("Jr.")
    Property(x => x.Field).EndsWith("Jr.")
    Property(x => x.Field).DoesNotEndWith("Mr.")

-------------------------------------------------------------------------
SubPropertyValidation
-------------------------------------------------------------------------

This validation reuses an existing validator to validate this property.
You can use a validation handle or the cached name

Example::

    Property(x => x.Field).WithValidator();
	Property(x => x.Field).WithValidator(validator);
	Property(x => x.Field).WithValidator("Overload");


-------------------------------------------------------------------------
UniqueCollectionValidation
-------------------------------------------------------------------------

This validation checks that the supplied collection does or does not
contain all unique values.

* IsUnique()
* IsNotUnique()

Example::

    Property(x => x.Collection).IsUnique()
	Property(x => x.Collection).IsNotUnique()

=========================================================================
 Internationialization
=========================================================================

As of now, all the error messages are fully i18n certified for the
following languages:

* English
* Spanish
* German
* Italian
* French
* Chinese
* Arabic
* Russian
* Japanese
* Portuguese
* Chinese

In order to switch between the various translations, simply change the
current culture to be the locale that you are interested in::

    using System.Threading;
	using System.Globalization;

    var culture = CultureInfo.GetCultureInfo("es");
    Thread.CurrentThread.CurrentCulture = culture;
    Thread.CurrentThread.CurrentUICulture = culture;

It should be noted that these translations were generated using the
google translate service, so their mileage may vary.  Any additional
translations or corrections to existing ones will be gladly accpted.

=========================================================================
 TODO
=========================================================================

-------------------------------------------------------------------------
 Things That Need To Be Looked At
-------------------------------------------------------------------------

- Overloading messages (error) and Property Names
  - possibly introduce a simple template system: format % (name, value,
    expected)
  - Should I get the input predicate values from the expression or store
    in a predicate context
  - Should an overridden error message only return one failure?

-------------------------------------------------------------------------
 Property Context
-------------------------------------------------------------------------

- needs to collect the rules for each property
  - this is so we can bind overloaded messages, when, unless
  - validation context will iterate through each property context
- check if one or more when clauses are available
  - preconditions before tests (what scope to run in)
  - run before other tests (store in seperate list?)
- set another validation chain or functor as validator (reuse validation)
  - sub property validation rule
- can we go N levels deep in object?