=============================================
Shared Components
=============================================

What is a Shared Component?
-------------------------------------------------------------------------------------------

The **Shared Components Committee** is a group of developers, supervisors, and architects
whose goal is to create, find, and promote generic, reusable, **Shared Components**.  By promoting 
shared components, the goal is that development times will be shortened due to the 
fact that the components have already been developed, and more importantly, fully tested.

There is a lot of benefit in sharing the intellectual property that we, as developers, frequently create.

*	Simplify maintenance by changing implementation in one place
*	Don't reinvent the wheel. Algorithms and standardized logic can be difficult and time-consuming to reproduce.

The current focus of the SCC is on shared assemblies. To qualify as a shared assembly, the assembly must:

*	Have available and comprehensive documentation for other developers
*	Be well-tested with available and comprehensive test cases
*	An available report of known issues and a means to report discovered issues

When using these components, developers can be assured that they will function in a wide
variety of applications since their intent is to be as generic as possible.  

Processes and Procedures
-------------------------------------------------------------------------------------------

The documents below represent the collection of processes and procedures releated to 
using the shared components in your own projects and submitting new component ideas.

.. toctree::
   :maxdepth: 1

   shared-components-versioning.rst   
   shared-components-referencing.rst   
   shared-components-sugesting.rst    
    
Component Catalog
-------------------------------------------------------------------------------------------

The Shared Components Catalog lists all the shared components currently in all of the
libraries with a brief description of each.  

.. toctree::
   :maxdepth: 1

   shared-components-catalog.rst
   
Auto-Generated API Reference Documentation
-------------------------------------------------------------------------------------------

For information on all classes, interfaces, properties, methods, and everything else about
each Shared Component, you can visit our nDoc3 generated documentation:

* `Shared Components API Reference <../../../Api/index.html>`_.

How-To-Use and Code Sample Documentation
--------------------------------------------------------------------------------------------

The following are a collection of usage tutorials developed to help get you started
with using Shared Components in your code.  They are not an exhaustive description of the API,
but more a general "how-to" style of documents.  The complete details of every
class and method may be found in the API Reference.
   
.. toctree::
   :maxdepth: 3

   reference/index.rst
