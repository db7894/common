==================================================
Remote Debugging in VS 2008
==================================================

-----------------------------------------
Prepare the Remote host
-----------------------------------------
    
    1.1  Install Remote Debugging Monitor (msvsmon.exe)

        Source: C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\Remote Debugger\x86

        Note: copy the entire directory

        Target: Anywhere on the remote machine

        Note: Only need to do this once for a specific RM. Always check if it already exists

    1.2  Start the remote debugging monitor - msvsmon.exe


    1.3  Configuring the remote debugging monitor:

        1.3.1        Windows Authentication Mode :

            To debug managed or mixed code, you will need to use authentication mode.

            Important: Both the development and target machines must have an identical admin-level username and password. This will likely require setting up a new
            username on at least one of the machines. It is not neccessary to log in to the remote machine under this account. You can simply choose Run as... from the
            right-click context menu on msvsmon.exe.

            If the remote machine does not have SCOTTRADECORP domain you will definitely need to create a local user on your desktop and run your visual studio under
            this local user.  Be sure to match username and password on your desktop and remote machine.

            Select Options from the Tools menu and check that Windows Authentication is selected.  

            Clck Permissions to open the permissions dialog box.

            Click the Add button to add a user.

            Click Locations, choose the proper domain, and click OK.

            Enter the username (same as above) and click OK.

            Under Debug permissions, ensure thart Allow is checed and click OK again.

    1.3.2        No Authentication Mode:

        In the Remote Debugging Monitor, choose Options on the Tools menu

        * In the Options dialog box, select No Authentication (native only).

        * Check the Allow any user to debug option.

        * If you wish you can change the Maximum idle time to suit your needs (0 for no timeout)

    1.4  Copying the Remote Debugging Components - Dependent files

        1.4.1        Copy the project executables and its dependencies. 

            Source: .exe, .pdb (program database files)

                                     Target: In corresponding bin directory in the remote machine 

            Note: Remember to copy all your dependencies too.

            Alternative Method: To avoid these dependency issues, you may choose to map the local 

            machine from the remote machine, and point the remote debugger to the local machine (See 

            section 2.2). This removes any need to copy the executables and their dependencies to the 

            remote machine.

    1.4.2        Copy Debug Common Run Time libraries (DebugCRT)

        1.4.2.1  Copy the following folders from your local server's C:\WINDOWS\WinSxS

            x86_Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_597c3456

            x86_Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_9.0.30729.1_x-ww_f863c71f

            to the remote server's C:\WINDOWS\WinSxS


    1.4.2.2  Copy the following files from your local server's C:\WINDOWS\WinSxS\Manifests

        x86_Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_597c3456.cat

        x86_Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_9.0.21022.8_x-ww_597c3456.manifest

        x86_Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_9.0.30729.1_x-ww_f863c71f.cat

        x86_Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_9.0.30729.1_x-ww_f863c71f.manifest

        to the remote server's C:\WINDOWS\WinSxS\Manifests

    1.4.2.3  Copy the following folders from your local server's C:\WINDOWS\WinSxS\Policies

        x86_policy.9.0.Microsoft.VC90.DebugCRT_1fc8b3b9a1e18e3b_x-ww_037be232

        to the remote server's C:\WINDOWS\WinSxS\Policies

-----------------------------------------
Prepare the Local host
-----------------------------------------
    2.1  Configure your Project for remote debugging:
        Project -> Properties  -> Configuration Properties -> Debugging


    2.2   Select Remote Windows Debugger under Debugger to Launch
        Remote Command: Path of the exe file you copied on the RM. Alternatively, this path may point back to your local machine if the drive is mapped on the remote
        machine (See Alternative Method in section 1.4.1). For example, \\flmappdevXX.bashwork.com\c$\Program\Program.exe

        Remote Server Name: Enter RM's IP Address.
        Connection: Remote with no authentication (native only) OR Remote with Windows Authentication (managed, mixed, or auto).

    2.3  Compile the debug version of your application.

    2.4  Then set your breakpoints and run your app. J
