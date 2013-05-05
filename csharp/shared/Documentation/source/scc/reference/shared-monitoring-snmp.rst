=========================================================================
Snmp --- Utility for Sending SNMP Traps
=========================================================================
:Assembly: SharedAssemblies.Monitoring.Snmp.dll
:Namespace: SharedAssemblies.Monitoring.Snmp
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.Monitoring.Snmp
   :platform: Windows, .Net
   :synopsis: Snmp - Utility for Sending SNMP Traps

.. highlight:: csharp

.. index:: 
    pair: SNMP; Traps

Introduction
------------------------------------------------------------

The **SharedAssemblies.Monitoring.Snmp** library simplifies the process of sending **SNMP** (Simple Network Management Protocol) traps.  Basically,
a trap is an alert that the Computer Operations group watches to see if a process or server is having an issue.  
By throwing these traps when critical events occur in an application, we can help enable more proactive monitoring of services.

There are two main classes in this assembly: *SnmpTraps* and *SnmpTrapThreshold*.

.. class:: SnmpTraps

    This class is a static utility class that simplifies the process of sending SNMP traps in an application.  Basically,
    when an event occurs, you simply send the desired trap number and error text.  There are also static properties to 
    override the default agent and target servers.
    
    .. attribute:: TrapAgentServer
    
        :returns: The SNMP mailbox for the agent server.
        :rtype: string
        
    .. attribute: TrapTargetServer
    
        :returns: The SNMP mailbox for the target server.
        :rtype: string
        
    .. attribute: TrapErrorText
        
        :returns: The default error text for the trap if none provided on *SendTrap(...)*.
        :rtype: string
    
    .. method:: SendTrap(trapNumber[, trapMessage])
    
        :param trapNumber: The SNMP trap number.
        :type trapNumber: int
        :param trapMessage: The text for the SNMP trap.
        :type trapMessage: string
        :returns: True if trap sends successfully.
        :rtype: bool
        
        This method sends a trap with an optional trap message.  If no trap message is chosen, a default is used instead::
        
            if (averageTime > 100)
            {
                SnmpTraps.SendTrap(100, "OrderServer average time > 100 ms");
            }        
            
.. class:: SnmpTrapThreshold

    This class evaluates a statistic against a threshold and throws an SNMP trap if that threshold is violated.  This is useful for throwing
    traps if a statistic gets too high or too low.
    
    There are two ways to design your threshold, you can use either a pair of delegates: one to extract, and one to compare like this::
    
            // creates a threshold named "Average Time" with a threshold of 100
            // that sends a trap of 200:Average Time Exceeded by comparing
            // the OrderProcessor's AverageTime static field using the > operator
            var orderTimeChecker = new SnmpTrapThreshold("Average Time", 100, 
                 200, "Average Time Exceeded",
                 () => OrderProcessor.AverageTime, 
                 (val, thresh) => val > thresh);
            
    Or, using a single evaluator instead of separate extract & compares::
    
            // alternatively, we can combine the extractor and comparer in one delegate:
            var orderTimeChecker = new SnmpTrapThreshold("Average Time", 100,
                 200, "Average Time Exceeded",
                 (thresh) => OrderProcessor.AverageTime > thresh);  
                 
    Regardless, one created, you will check to see if the threshold is exceeded at regular intervals and then
    tell the threshold to throw if so::
        
            while(doingWork)
            {
                // do the work

                if(orderTimeChecker.IsThresholdExceeded())
                {
                    orderTimeChecker.SendTrap();
                }
            }
            
    .. note:: The SnmpTrapThreshold is general purpose and does not use events to automatically notify the program, this is to give it maximum flexibility and to keep overhead low.  It is simply called when you want it to be called.
    
    So you may ask why isn't there just one method?  Why one to check and one to send?  This was to break a dependency on logging.  In this way, you can log if the send fails.
    
    .. attribute:: Name
     
        :returns: The name of the threshold.
        :rtype: string
        
        Gets the name of the threshold
        
    .. attribute:: Threshold
    
        :returns: The threshold value to compare the statistic to.
        :rtype: double
        
        Gets the threshold value to compare the statistic to.
        
    .. attribute:: TrapNumber
    
        :returns: The SNMP trap number to send if threshold exceeded.
        :rtype: int
        
        Gets the trap number to send if threshold is exceeded.
        
    .. attribute:: TrapMessage
    
        :returns: The SNMP trap message to send if threshold exceeded.
        :rtype: string
        
        Gets the trap message that is sent if threshold is exceeded.
        
    .. attribute:: TresholdEvaluator
    
        :returns: The evaluator that compares a current value to the threshold.
        :rtype: Func<bool>        
        
        Gets the method that will evaluate the value and the threshold.
        
    .. method:: IsThresholdExceeded()
    
        :returns: True if threshold is exceeded.
        :rtype: bool
        
        Checks the *ThresholdEvaluator* to see if threshold is exceeded and returns true if so.
        
    .. method:: SendTrap()
    
        :returns: True if trap was sent successfully.
        :rtype: bool
        
        Sends a trap given the configuration information passed in the constructor.
        

For more information, see the `API Reference <../../../../Api/index.html>`_.