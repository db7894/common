=================================================================
Third Party Resource Standards
=================================================================
:Author: Galen Collins <gcollins@bashwork.com>
:Version: 1.0 11/02/09

.. highlightlang:: csharp
 
Introduction
-------------

Long story short, there is a wealth of open source code available
that already solves a number of problems.  The downside is that we
cannot use some of it without putting Bashwork into questionable
legal quandaries.

What Open Source Can We Use
----------------------------------------

To keep things simple, our legal department has agreed that we can use
code, tools, and libraries marked with the following licenses:

* apache
* bsd
* mit
* codeplex

If you have found a resource that you feel could greatly improve our
system but is not tagged with one of the previous licenses, you will
need to present it to your team lead who will have to take it up with
the powers that be to decide if it is allowed or not.
 
Adding To SharedAssemblies
----------------------------------------

If we choose to use a given library, the following things must be placed in
source control with the library:

* The compiled assembly (the release version)
* The pdb file associated with the assembly
* The license file for the library
* The documentation for the library (usually a chm file)

Further Reading
----------------------------------------

* `Wikipedia License List <http://en.wikipedia.org/wiki/Open_source_license>`_
* `Wikipedia License Comparison <http://en.wikipedia.org/wiki/Comparison_of_free_software_licenses>`_
