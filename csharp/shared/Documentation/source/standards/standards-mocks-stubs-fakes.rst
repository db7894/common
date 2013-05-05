==================================================
Mocks, Fakes, and Stubs
==================================================
:Author: Todd Niemeyer <tniemeyer@bashwork.com>
:Date: 3/9/2009

Introduction
--------------------------------------------------

To eliminate the dependency on systems/objects not suited for testing.
The reasons include that the actual system/object:

* Is not available
* Does not return data needed for test
* Cannot be used without undesirable side effects

To verify the system/object being tested is using other systems/objects correctly  

Types of Test Doubles
--------------------------------------------------
*What follows is a description of the various forms of test doubles*

* **Dummy**
  A double that does nothing.  These usually just fill parameter lists

* **Stub**
  An object which provides canned answers ignoring input from the
  SUT (system under test).
 
* **Spy**
  Is basically a stub with the added ability to record information about
  how it was used so the use can be verified later by the test.

* **Mock**
  An object which is basically a Spy but is preprogrammed with expectations
  about how it will be called so that it can itself verify the SUT used it
  correctly.

* **Fake**
  An object which replaces an actual system/object with an alternate implementation
  for added speed and/or to eliminate the dependency of the test on an external
  system.  Fakes can be viewed as more intelligent Stubs

Mocking Frameworks
--------------------------------------------------

Waste of time to create and update stub/mock/spy each time interface changes.
Overkill, instead use a generic framework.

For more information on the suggested mocking frameworks for each development
environment, check the `Further Reading`_.

External Links
--------------------------------------------------

* `Moq <http://code.google.com/p/moq/wiki/QuickStart>`_
  A new mocking framework for the .Net framework that uses a fluent interface
  to provide for record/replay and verification.

* `Mockito <http://code.google.com/p/mockito/>`_
  The currently favored java mocking framework chock full of puns.

* `Mocks Aren't Stubs <http://martinfowler.com/articles/mocksArentStubs.html>`_
  Martin Fowlers writeup discussing the various forms of test doubles.

