=========================================================================================================
Core --- The Generic Utility Classes
=========================================================================================================
:Assembly: SharedAssemblies.Core
:Namespace: SharedAssemblies.Core
:Date: |today|

.. highlight:: csharp

.. index:: Core
.. index:: Utilities
 
Introduction 
-------------------------------------------------------------------------------------------

The SharedAssemblies.Core assembly is a collection of common generic utility classes
available for use in all projects including other shared components.  The SharedAssemblies.Core
assembly should be referenced along with any other SharedAssembly in use.  This is not always
strictly necessary, but is a good rule of thumb habit.

There are other SharedAssemblies which may be required by some specialized SharedAssemblies
such as the Encryption library requiring the Database library, but these are a very small
minority.

Everything in the SharedAssemblies.Core assembly is self-contained and perfectly generic;
nothing in it requires other references or is specific to any business logic or 
application technology.

Anything that is a candidate for inclusion in SharedAssemblies.Core should follow these
rules before it can be considered.

.. note:: The terms **Core** and **Core Assembly** in this documentation are understood to always refer to the **SharedAssemblies.Core** and all classes included therein.

The Core assembly is logically divided into five separate namespaces:

* **SharedAssemblies.Core.Containers** - *A set of supplemental, generic containers that either add functionality to built-in containers or define new ones.*
* **SharedAssemblies.Core.Conversions** - *A set of utilities that simplify type conversion and value translation.*
* **SharedAssemblies.Core.Extensions** - *A collection of helpful extensions methods that add functionality to existing classes.*
* **SharedAssemblies.Core.Patterns** - *A set of generic implementations of some of the classical design patterns.*
* **SharedAssemblies.Core.Xml** - *An Xml utility that simplify the process of converting an object to Xml and back.*

Contents
----------------------------------------------------------------------------------------------------

.. toctree::
   :maxdepth: 3

   shared-core-containers
   shared-core-conversions
   shared-core-extensions
   shared-core-patterns
   shared-core-xml
