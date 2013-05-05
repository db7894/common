=============================================
Referencing Shared Components
=============================================

To reference a shared component, the user of the component, the user should add references in their projects to the 
binaries directory associated with the release.  

How you reference a Shared Component is fairly open, the only rules are:

* You must reference the Shared Component Assemblies by their *binaries (DLLs)*, and not by their *project files (csproj)*.
* Only the last 3 versions of Shared Components will be officially supported, after that consider upgrading.

So as long as you are referencing one of the three supported versions of the binaries, how you reference them in your projects
is largely up to you.  Here are three possibilities to consider, though there are many more:

Map Workspace to Binaries Root
------------------------------------------------------------------
In this method, you set up a workspace mapping from *$/SharedAssemblies/Binaries/* to your local source location.
This has the advantage of giving you access to all the different versions of the binaries and making it 
less likely that you'll pick up a new version unexpectedly.

**Pros:**
    * Easy to do get-latest to get newest version of DLLs.
    * Workspace mapping does not change when moving to new versions.
    * Can use different versions of components.

**Cons:**
    * May have unexpected binary drift if binaries change on TFS.
    * Must manually update references when moving to new versions.
    * New projects/developers must have workspaces mapped.


Map Workspace to Specific Binaries Version
------------------------------------------------------------------
You can set up a workspace mapping to *$/SharedAssemblies/Binaries/<version>* to your local directory.  This has the
benefit of being extremely simple to work with and allows you to do a "get latest" to pull the latest version of those 
assemblies, but you will not be able to use different versions of the same libraries if desired.

**Pros:**
    * Easy to do get-latest to get newest version of DLLs.
    * No updating references in the project when moving to new versions.

**Cons:**
    * May have unexpected binary drift if binaries change on TFS.
    * New projects must have workspaces mapped.
    * Moving to a new version of Shared Components involves changing mapping.
    * Can't use different versions of components.

Manually Copy Binaries
-----------------------------------------------------------------
In this option, the consumers of the library will get the latest version of the binaries and manually copy them
into a directory under their project such as *C:\SoureCode\SomeProject\Bin*.  References are then created to 
these binaries locally.

**Pros:**
    * No workspace mapping needed for binaries.
    * No unexpected binary drift if binaries change on TFS.

**Cons:**
    * Binaries checked into TFS in multiple places.
    * Won't pick up needed bug-fix changes to binaries automatically.
    * Manual copy of binaries can potentially be error prone.

Summary
-----------------------------------------------------------------
As you can see, there are benefits and disadvantages to each of these methods.  Ultimately, it is up to each team to
decide how they want to use the binaries.
