==================================================
Changing Hostname in a TFS Workspace
==================================================

If a machine changes hostnames (due to, say, moving to a new building) any workspaces mapped on that machine must be changed to reflect the new machine name. Here's how:

Make sure Team Foundation Power Tools is installed
http://msdn.microsoft.com/en-us/teamsystem/bb980963.aspx (this is "approved" software)

Navigate to Start->Programs->Microsoft Visual Studio 2008->Visual Studio Tools->Visual Studio 2008 Comand Prompt

Execute the following command: 

* tfpt workspace /updatecomputername [workspacename];[youruserid] /server:cgtmfndation001

where, shockingly, workspacename is the name of the workspace being remapped.

This command must be executed for each workspace mapped on the machine.

