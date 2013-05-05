============================================================
Input Validation --- General Input Validation
============================================================
:Author: Jeff Ayers <jayers@bashwork.com>
:Assembly: SharedAssemblies.General.Validation
:Namespace: SharedAssemblies.General.Validation
:Date: |today|

.. module:: SharedAssemblies.General.Validation
   :synopsis: Validation - General data validation
   :platform: Windows, .Net

.. highlight:: csharp

Introduction
------------------------------------------------------------

Input validation is the strongest security countermeasure that can be implemented by 
a developer and the one most overlooked.  If implemented correctly, input validation 
can eliminate more than 50% of all vulnerabilities and is the first security issue 
that Application Security will look for. Unfortunately, input validation is messy and 
the number of lines could exceed all others in a method.

In order to facilitate validation, this library was designed using the fluent design 
pattern because it produces clean English like lines of code. With intellisense as an 
aid, a developer can quickly write one line of code to replace dozens of lines of input
validation code. In addition, this input validation library should be used in conjunction 
with other shared assemblies like Validator regular expressions.

Simple Example
------------------------------------------------------------

Let's start with a simple example::

    string fn = "hoser";
    Validate.That(fn).IsNotNull().ThrowOnError();

The code above reads, "validate that 'fn' is not null" and if it is null throw a
ValidationException.  The ThrowOnError() method at the end will throw the exception,
but you may not want to handle an exception. Alternatively::

    var v = Validate.That(fn).IsNotNull();
    foreach (ValidationError ve in v.ValidationErrors)
    {
        Console.WriteLine(ve.MethodName + ": " + ve.Message);
    }

In this fluent design, each method in the chain returns the same **ValidationChain** 
*type*, which contains zero or more ValidationError instances. Each ValidationError 
contains the MethodName and Message that generated the error. For example, if 
IsNotNull() fails, then the MethodName would equal "IsNotNull" and the message would 
contain the error.

It is understandable that you would want to validate multiple input parameters at the 
same time. The That() method handles this by using the **params** keyword. Thus, the
following is valid::

    Validate.That(fn, ln).IsNotNull().ThrowOnError();

The code above will validate both 'fn' and 'ln' for nullness. Important note: all 
parameters passed to the That() method must be of the same *type*.

Matches
------------------------------------------------------------
Regular expressions are very useful for input validation and the validation library 
allows the developer to take advantage of that fact::

    string ea1 = "ab@xy.com";
    Validate.That(ea1).Matches(Validator.EmailRegularExpressionPattern).ThrowOnError();

The above code reads "validate that 'ea1' email address matches the specified email 
address regular expression" and if not throw an exception because the ThrowOnError() 
method is present. Notice that the code makes use of pre-existing regular expression 
pattern in **SharedAssemblies.General.Validation** assembly. Matches() works against any
number of parameters::

    Validate.That(ea1,ea2).Matches(Validator.EmailRegularExpressionPattern).ThrowOnError();

IsIn (White List)
------------------------------------------------------------
White lists are a powerful way to eliminate all doubt about a specific input. Either 
a value matches against a list or it does not::

    string fn = "hoser";
    string[] whiteList = { "hoser", "hillbilly", "hangman" };
    Validate.That(fn).IsIn(whiteList).ThrowOnError();

The code reads "validate that 'fn' is in the specified list" and if not throw an 
exception because the ThrowOnError() method is present. IsIn() method works against 
any number of parameters::

    Validate.That(fn,ln).IsIn(whiteList).ThrowOnError();

Obeys (Caller written rules)
------------------------------------------------------------
Of course, it is impossible to write an all-encompassing validation library that 
can foresee all possible validation tests. Thus, the library includes an Obeys() method
that will allow the developer to write their own validation rules. For example::

    Validate.That(fn).Obeys(value => value.Length > 0).ThrowOnError();

The above code reads "validate that 'fn' obeys the rule 'its length must be greater 
than zero'" and if not throw an exception because the ThrowOnError() method is 
present.  The rule works against any number of parameters::

    Validate.That(fn,ln).Obeys(value => value.Length > 0).ThrowOnError();

That
------------------------------------------------------------
On occasion, you might want to change the parameters in midstream. For example::

    Validate
        .That(fn,ln).IsNotNull()
        .That(ea1,ea2).Matches(reg)
        .That(x,y).IsIn(whiteList).ThrowOnError();

Notice that there are three That() method calls in the chain. Also notice the 
formatting where each That() method is followed by the validation tests for the 
specified variables. Thus, variable fn and ln are checked for nullness, ea1 and ea2 
are matched against a regular expression, and finally, x and y are compared against 
a white list.

ReportAll
------------------------------------------------------------
The default behavior for the validation library is to return immediately on the first 
failed test. However, there are occasions when a developer might want to know all 
input validation failures::

    Validate.That(ea1, ea2).ReportAll().IsNotNull().Matches(reg).ThrowOnError();

The code reads, "validate that, report all errors, where 'ea1' or 'ea2' are not
null and matches a regular expression" and if not then throw an exception that 
contains all the failures. If there is more than one error, the exception message
will read "Multiple ValidationExceptions. See InnerExceptions." The InnerExceptions
property returns the **IEnumerable<ValidationError>** instance.

ThrowOnError
------------------------------------------------------------
The developer can control whether or not to throw an exception if there is a
validation error. ThrowOnError() always throws an instance of *type* 
**ValidationException**. If not used, then each method in the chain returns an instance
of **ValidationChain** *type*, which contains zero or more **ValidationError** instances.

Performance is a major concern in an environment like Bashwork. Therefore, the
caller can retrieve a performance counter from the ThrowOnError() method::

    Stopwatch st = new Stopwatch();
    Validate.That(fn).IsNotNull().Validate(t => st = t);
    Console.WriteLine("Speed(ms): " + st.Elapsed.TotalMilliseconds.ToString());

This way a developer can verify that the validation code is not a CPU hog.

Combinations (The real power)
------------------------------------------------------------
A combination of multiple parameters and multiple input validations demonstrates the 
expressive power of the library. With several input parameters, for example::

    Validate
        .That(fn, ln, ea1, ea2).IsNotNull()
        .That(ea1, ea2).Matches(Validator.EmailRegularExpressionPattern)
        .That(fn, ln).IsIn(whiteList).Obeys(v => v.IsLengthAtLeast(10))
        .ThrowOnError();

The above code reads "validate that all four parameters are not null, and that 'ea1' 
and 'ea2' match an email regular expression, that 'fn' and 'ln' are in a white list 
and obey a minimum length rule. Notice how easy it is to read and yet how much code 
this represents.


For more information, see the `API Reference <../../../../Api/index.html>`_.
