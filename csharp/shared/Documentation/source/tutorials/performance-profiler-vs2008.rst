=====================================================
Using the Performance Profiler in Visual Studio 2008
=====================================================

-------------------
Setup
-------------------

* Open the project to be profiled in Visual Studio
* Navigate to Analyze->Windows->Performance Explorer

.. image:: ../includes/images/performance-explorer.jpg

* Click the "Launch Performance Wizard" icon in the top left corner of the Performance Explorer window.
* Ensure that the desired project is selected as the target and click Next.

.. image:: ../includes/images/performance-wizard-1.jpg

* Choose a profiling method: Sampling or Instrumentation, and click Next (Read about these options below)

.. image:: ../includes/images/performance-wizard-2.jpg

* Click Finish to create the profile

-------------------
Profiling Method
-------------------

Two profiling methods are available: Statistical Sampling and Code Instrumentation. 

*Statistical Sampling:* This method runs a separate process which periodically takes snapshots of the execution and provides relative frequencies of execution times. Functions whose relative execution time is small may receive minimal attention or be missed altogether. This method has line by line level resolution and has minimal impact on code execution. A sample summary showing the relative frequencies of samples of various types of assignment statements is shown below.

.. image:: ../includes/images/sample-profiling.jpg

*Code Instrumentation:* This method injects extra timing code at function entry and exit points to generate actual execution times. It has only function-level resolution, is very disruptive, and may significantly increase execution time. A sample summary using instrumentation for the same project is shown below. Notice that it provides the exact number of function calls and exact time of execution, not simply relative frequencies.

.. image:: ../includes/images/instrumentation-profiling.jpg

-------------------
Launch
-------------------

* Click the "Launch with Profiling" icon at the top of the Performance Explorer window.
* The program will be executed. Interact with the program as necessary to ensure that all desired
  execution paths are followed. The amount of time spent performing each task will depend on the sampling method.
* Close the program, or allow execution to terminate normally. Visual studio will build a report of the session.
 
-------------------
Analyze
-------------------

* Each "Launch with Profiling" session will create a separate report which can be opened from
  the Performance Explorer window; Double-click a report to display a summary.
* The report view can be changed to acquire more information by selecting from the "Current View"
  dropdown box at the top of the report. 
* Double-clicking a function or a process will also display further details.
 
-------------------
Definitions
-------------------

When viewing a particular function or module in a report, the following information is displayed:

* Exclusive Samples: The number of samples in which the function itself was executing
* Inclusive Samples: The number of samples in which the function or any of its children in the call stack were executing
* Exclusive Sample %: The percentage of samples in which the function itself was executing
* Inclusive Sample %: The percentage of samples in which the function or any of its children in the callstack were executing

Additional columns can be added by right-clicking any column and choosing Add/Remove Columns:

* Source File: The file containing the function definition
* Function Line Number: The line number of the beginning of the function in the source file
* Function Address: The function address or token
* Process Name: The name of the process
* Process ID: The process ID

