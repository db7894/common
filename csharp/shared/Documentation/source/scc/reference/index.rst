=============================================
Shared Components Usage Guides
=============================================

Introduction 
-------------------------------------------------------------------------------------------

The **SharedAssemblies** library is a set of common, reusable **C#** **shared components** that have been
developed and/or approved by the **Shared Components Committee**.  The following are
guides that have been developed to help developers understand how the shared components
can be used in everyday coding.  

These guides are not exhaustive views of every component and method, but more informal
tutorials and examples.  For the detailed descriptions of each class, method, etc. please see the API
documentation.

Core Library
-------------------------------------------------------
Purely generic utility classes usable in any task.

.. toctree::
   :maxdepth: 3

   Core/index.rst

General Libraries
-------------------------------------------------------
General use libraries that solve a specific, general-purpose need.

.. toctree::
   :maxdepth: 2

   shared-general-caching 
   shared-general-advanced-caching 
   shared-general-database
   shared-general-financial
   shared-activedirectory
   shared-interceptors
   shared-general-logging
   shared-testing
   shared-general-threading
   shared-validator
   shared-input-validation

Communication Libraries
------------------------------------------------------- 
Libraries specific to handling different application communication needs.

.. toctree::
   :maxdepth: 2

   shared-communication-email
   
Monitoring Libraries
-------------------------------------------------------
Libraries specific to monitoring and instrumenting application events and statistics.

.. toctree::
   :maxdepth: 2

   shared-monitoring-autocounters
   shared-monitoring-snmp 
   
Security Libraries
-------------------------------------------------------
Libraries specific to handling security needs.

.. toctree::
   :maxdepth: 2

   shared-encryption
   
Web Libraries
-------------------------------------------------------
Libraries specific to designing and implementing web applications.

.. toctree::
   :maxdepth: 2

   shared-mvp
