==================================================
Windows Debugging
==================================================

--------------------------------------------------
Using Visual Studio
--------------------------------------------------
*A quick discussion of debugging with visual studio*

--------------------------------------------------
Using Windbg
--------------------------------------------------
*An indepth discussion of debugging with visual studio*

Setup
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

.sympath SRV*c:\localsymbols*http://msdl.microsoft.com/download/symbols

Commands
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

What follows is a listing of the most used windbg functions::

    +-------------------------------------------------+
    | Parameters                                      |
    +--------------------+----------------------------+
    | range  = <addr> <offset>                        |
    | offset = L<bytes> or <type>+0n010               |
    | addr   = memory address or <type>               |
    | type   = any instance in the current scope      |
    | ? type = address of instance                    |
    |                                                 |
    +-------------------------------------------------+
    | Reading memory                                  |
    +--------------------+----------------------------+
    | Disassemble        | u <addr> L<num instrs>     |
    | Disas Function     | uf <func>                  |
    | Dump byte          | db <addr> L<num bytes>     |
    | Dump word          | dw <addr> L<num words>     |
    | Dump dword         | dd <addr> L<num dwords>    |
    | Dump string        | ds <addr> L<num dwords>    |
    | Dump mem with syms | dps <addr or range>        |
    | Dump & dereference | dp[a|p|u] <addr>           |
    | Dump structure     | dt <addr>                  |
    | Search memory      | s [a|b|w|d] <addr> pattern |
    | Compare memory     | c <range> <addr>           |
    | Fill memory        | f <range> pattern          |
    |                    |                            |
    +-------------------------------------------------+
    | Writing memory                                  |
    +--------------------+----------------------------+
    | Copy memory        | m <range> <dst addr>       |
    | Edit byte          | eb <addr> 0x41 'b' ...     |
    | Edit word          | ew <addr> 0x1234 ...       |
    | Edit dword         | ed <addr> 0x12345678 ..    |
    |                    |                            |
    +-------------------------------------------------+
    | Breakpoints                                     |
    +--------------------+----------------------------+
    | Add breakpoint     | bp <addr|sym>              |
    | Break on access    | ba [w|r|e|i] <addr> L<len> |
    | Disable breakpoint | bd <breakpoint num>        |
    | Enable breakpoint  | be <breakpoint num>        |
    | List breakpoints   | bl                         |
    | Remove breakpoint  | bc <breakpoint num>        |
    |                    |                            |
    +--------------------+----------------------------+
    | Tracing                                         |
    +--------------------+----------------------------+
    | Continue execution | g or F5                    |
    | Display call stack | k                          |
    | Display registers  | r                          |
    | Step into          | t or F11                   |
    | Step out           | Shift + F11                |
    | Step over          | p or F10                   |
    | Step to branch     | tb or ph                   |
    | Step to call       | tc or pc                   |
    | Step to return     | tt or pt                   |
    |                    |                            |
    +--------------------+----------------------------+
    | Advanced commands                               |
    +--------------------+----------------------------+
    | Attach to process  | F6                         |
    | Conditional        | j (condition) '<t>'; '<f>' |
    | Display PEB        | !peb                       |
    | Display TEB        | !teb                       |
    | Display type       | dt <struct name> [<addr>]  |
    | Display stacks     | ~*k                        |
    | Trace and watch    | wt                         |
    |                    |                            |
    +--------------------+----------------------------+ 
    | Working With Variables                          |
    +--------------------+----------------------------+
    | Display Locals     | dv                         |
    | Examine Symbols    | x                          |
    |                    |                            |
    +--------------------+----------------------------+ 
    | Working With Source Code                        |
    +--------------------+----------------------------+
    | List source line   | ls                         |
    | List source        | lsc                        |
    |                    |                            |
    +--------------------+----------------------------+


^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
Meta Commands 
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. :command: .cls  Clear the screen
.. :command: .call Call a function
.. :command: .closehandle Close a handle
.. :command: .create Create a process
