=================================================================
Code Review Standards
=================================================================
:Author: Jim Hare <jhare@bashwork.com>
:Version: 1.0 2/24/09

.. highlightlang:: csharp
 
Introduction
=================================================================
*A introduction to code review and its benefits*

For another opinion, read the `Why Review Code`_ from SmartBear.

Development Workflow
=================================================================
*A discussion of the standard development workflow at Bashwork*

All code changes are usually associated with a change request ticket in Visual
Intercept or TFS.  Before you begin making code changes, mark the associated
ticket as started by selecting a date for the "Work Start" date field.

When you are ready to submit your code changes for review, you first need to get
a changeset of your files.  This can be done in a few ways, but the recommended
way is to use TFS's **shelve** functionality to create a shelve-set of all
affected files (new and modified). 

Next, launch Code Collaborator Client (CCC) and perform the following steps:

* Select the appropriate SCM configuration. 
* Click on **Add Shelvesets**. 
* Select **Create New Review**. 
* Enter the review info and click on **Next** to attach a shelveset to the review.
* Add any review material and click **Finish**

You must now allow time for CCC to bring down the changes, diff them, and upload
the changes to the Code Collaborator server.

.. note:: This may take a minute as every file that is created will be scanned by
   Mcafee (server and client)!

When the upload is finished, your preferred browser will open directly to the
code review you just created.  Follow the remaining steps to finish setting up
the review:

* Select yourself as **Author** for the review
* Select one or more team members to be the Reviewer 
* Select one or more team member to be the Observer
* Click on **Create and Begin Review**.  
  
* Optionally, click on **Click and Annotate** to add some initial comments
  to your review. This is recommended as it gives the reviewers a guide to
  your code changes.

.. note:: In most cases you will only need one reviewer and one observer (always
   have at least two parties review your code), however, for some components (say
   ones that affect a lot of systems) you may need to add many people to the
   reviews.  Furtermore, a manager may feel the need to bring on extra parties to
   the review for a second opinion (say the system architects).

You will be notified by email and CCC when new comments/defects were posted for
the review or when the review has been completed.  Make the necessary code changes
to resolve any comments/defects, reshelve the new changes and wait for the author
of the comments/defects to check them off. Continue this process as many times as
needed to get your code into an accepted state (usually just one or two rounds).
Once the review is marked **completed**, you are allowed commit your changes into
the source control branch you are working in.

.. note:: For more information on using the various tools, check out the online
   `Code Collaborator Manual`_

How To Perform a Code Review
=================================================================

There are many things to take in mind when performing a code review. For a
quick guide, check out `SmartBear Best Practices`_.

Checklist Guide to Code Review
=================================================================
*What follows is a helpful guide of things to look for in a code review.
It should be noted that this list is not exhaustive*

General Smoke Tests
-----------------------------------------------------------------

Does the code build? No errors should occur when building the source code.
No warnings should be introduced by changes made to the code.  Does the code
execute as expected? When executed, the code does what it is supposed to.

Do you understand the code you are reviewing? As a reviewer, you should
understand the code. If you don't, the review may not be complete, or the
code may not be well commented.

Has the developer tested the code? Insure the developer has unit tested the
code before sending it for review. All the limit cases should have been tested.

Comments and Coding Conventions
-----------------------------------------------------------------

Does the code respect the project coding conventions? Check that the coding
conventions have been followed. Variable naming, indentation, and bracket
style should be used. 

Do all scope blocks begin and end with { and } ? To avoid hard to find bugs,
make sure all scope is clearly defined so that the code is more easily maintained.

Are units of numeric data clearly stated? For example, if a number represents
length, indicate if it is in feet or meters.  This should be done through
the variable name.

Are function parameters used for input or output clearly identified as such?
Make it clear which parameters are used for input and output. If you are taking
a reference to an object, make sure it is const if it is input that should not
be changed.

Are complex algorithms and code optimizations adequately commented? Complex
areas, algorithms, and code optimizations should be sufficiently commented, so
other developers can understand the code and walk through it.  Also, as a general
rule of thumb, if a section of code deems a comment, it should be separated into
it's own method.

If code is being removed from the application, remove the code do not comment it
out.  Let the source control system be your historical record of code that used
to be in the system.

Are comments used to identify missing functionality or unresolved issues in the
code? A comment is required for all code not completely implemented. The comment
should describe what's left to do or is missing. You should also use a distinctive
marker that you can search for later (For example: "TODO:francis"). 

Was code copy and pasted which could be extracted into a method that can be used
in both places? Code is not reused though copy, paste and edit reuse. Yes, this is
the easiest but makes maintenance a nightmare.

Error Handling
-----------------------------------------------------------------

Are errors properly handled each time a function returns? An error should be
detected and handled if it affects the execution of the rest of a routine. For
example, if a resource allocation fails, this affects the rest of the routine if
it uses that resource. This should be detected and proper action taken. In some
cases, the "proper action" may simply be to log the error.

Are resources and memory released in all error paths? Make sure all resources
and memory allocated are released in the error paths.

Are all thrown exceptions handled properly? If the source code uses a routine
that throws an exception, there should be a function in the call stack that
catches it and handles it properly.

Is the function caller notified when an error is detected? Consider notifying
your caller when an error is detected. If the error might affect your caller,
the caller should be notified. For example, the "Open" methods of a file class
should return error conditions. Even if the class stays in a valid state and
other calls to the class will be handled properly, the caller might be interested
in doing some error handling of its own.

Resource Leaks
-----------------------------------------------------------------

Is allocated memory freed? All allocated memory needs to be freed when no longer
needed. Make sure memory is released in all code paths, especially in error code
paths. Everything created in constructor should be deleted in destructor

Are all objects (Database connections, Sockets, Files, etc.) freed even when an
error occurs?  File, Sockets, Database connections, etc. (basically all objects
where a creation and a deletion method exist) should be freed even when an error
occurs. For example, whenever you use "new" in C++, there should be a delete
somewhere that disposes of the object. Resources that are opened must be closed.
For example, when opening a file in most development environments, you need to
call a method to close the file when you're done.

Is the same object released more than once? Make sure there's no code path where
the same object is released more than once. Check error code paths.  Set pointers
to 0 after free/delete

Does the code accurately keep track of reference counting? Frequently a reference
counter is used to keep the reference count on objects (For example, COM objects
or abstracted sockets). The object uses the reference counter to determine when
to destroy itself. In most cases, the developer uses methods to increment or
decrement the reference count. Make sure the reference count reflects the number
of times an object is referred.

Thread Safeness
-----------------------------------------------------------------

Are all global variables thread-safe? If global variables can be accessed by more
than one thread, code altering the global variable should be enclosed using a
synchronization mechanism such as a mutex. Code accessing the variable should be
enclosed with the same mechanism.

Are objects accessed by multiple threads thread-safe? If some objects can be
accessed by more than one thread, make sure member variables are protected by
synchronization mechanisms.

Are locks released in the same order they are obtained? It is important to release
the locks in the same order they were acquired to avoid deadlock situations.
Check error code paths.

Is the workload being minimized in a section of locked code? Make sure that there
isn`t a lot of processing done or any queries of external resources inside of a
locked section of code.

Is there any possible deadlock or lock contention? Make sure there's no possibility
for acquiring a set of locks (mutex, semaphores, etc.) in different orders. For
example, if Thread A acquires Lock #1 and then Lock #2, then Thread B shouldn't
acquire Lock #2 and then Lock #1.

Control Structures
-----------------------------------------------------------------

Are loop ending conditions accurate? Check all loops to make sure they iterate
the right number of times. Check the condition that ends the loop; insure it will
end out doing the expected number of iterations.

Is the code free of unintended infinite loops? Check for code paths that can
cause infinite loops. Make sure end loop conditions will be met unless otherwise
documented.

Performance
-----------------------------------------------------------------

Are whole objects duplicated when only references are needed? This happens
when objects are passed by value when only references are required. This
also applies to algorithms that copy a lot of memory. Consider using algorithm
that minimizes the number of object duplications, reducing the data that needs
to be transferred in memory.

Does the code have an impact on size, speed, or memory use? Can it be optimized?
For instance, if you use data structures with a large number of occurrences,
you might want to reduce the size of the structure.

Are you using blocking system calls when performance is involved? Consider using
a different thread for code making a function call that blocks.

Is the code doing busy waits instead of using synchronization mechanisms or timer
events? Doing busy waits takes up CPU time. It is a better practice to use
synchronization mechanisms.

Is the code free of expensive operations inside of loops? Was this optimization
really needed? Optimizations often make code harder to read and more likely to
contain bugs. Such optimizations should be avoided unless a need has been identified.
Has the code been profiled to prove that the optimization is needed?

Functions
-----------------------------------------------------------------

Are function parameters explicitly verified in the code? This check is encouraged
for functions where you don't control the whole range of values that are sent to
the function. This isn't the case for helper functions, for instance. Each
function should check its parameter for minimum and maximum possible values.
Each pointer or reference should be checked to see if it is null. An error or an
exception should occur if a parameter is invalid.

Are arrays explicitly checked for out-of-bound indexes? Make sure an error message
is returned if an index is out-of-bound.

Are functions returning references to objects declared on the stack? Don't return
references to objects declared on the stack, return references to objects created
on the heap.

Are variables initialized before they are used? Make sure there are no code paths
where variables are used prior to being initialized. If an object is used by more
than one thread, make sure the object is not in use by another thread when you
destroy it. If an object is created by doing a function call, make sure the object
was created before using it.

Does the code re-write functionality that could be achieved by using an existing
API? Don't reinvent the wheel. New code should use existing functionality as much
as possible. Don't rewrite source code that already exists in the project. Code
that is replicated in more than one function should be put in a helper function
for easier maintenance.

Bug Fixes
-----------------------------------------------------------------

Does a fix made to a function change the behavior of caller functions? Sometimes
code expects a function to behave incorrectly. Fixing the function can, in some
cases, break the caller. If this happens, either fix the code that depends on the
function, or add a comment explaining why the code can't be changed. Also, if you
can define a method as const to enforce that it doesn't change any members, do so.

Does the bug fix correct all the occurrences of the bug? If the code you're
reviewing is fixing a bug, make sure it fixes all the occurrences of the bug.

Math
-----------------------------------------------------------------

Is the code doing signed/unsigned conversions? Check all signed to unsigned
conversions: Can sign completion cause problems? Check all unsigned to signed
conversions: Can overflow occur? Test with Minimum and Maximum possible values. 

Further Reading
=================================================================

* `Smart Bear Code Collaborator <http://smartbear.com/codecollab-white-paper.php>`_
  The code review tool that we use internally. The company also releases a book
  that discusses their best practices for code review for free if you request it.
  The following will lead to a collection of high quality white papers on the subject
  of code review including best practices, integration into workflows, and guides.

.. _Why Review Code: http://smartbear.com/white-paper.php?content=docs/articles/WhyReviewCode.html&pageToken=codecollab-docs
.. _SmartBear Best Practices: http://smartbear.com/docs/BestPracticesForPeerCodeReview.pdf
.. _Code Collaborator Manual: http://smartbear.com/docs/collab-manual/index.html
