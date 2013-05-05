============================================================
Visual Studio FAQs
============================================================
:Author: Galen Collins <gcollins@bashwork.com>
:Date: |today|

.. highlight:: csharp

Intellisense
==================================================
*A collection of tips and solutions dealing with intellisense*

How come my referenced dll doesn't include my XML comments?
------------------------------------------------------------

This is because the comments are stored in an exported XML file that
is loaded by visual studio on first use of the assembly. In order for
your comments to be viewable, they must be in the same directory as the
referenced dll::

    SharedAssemblies.General.Validation.dll // The referenced dll
    SharedAssemblies.General.Validation.xml // The referenced dll's comments

