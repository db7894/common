=============================================
Versioning Shared Component Releases
=============================================

All shared components live in Bashwork's **SharedAssemblies** Library. This repository can be found in TFS under **$/SharedAssemblies**.

All shared component binaries are in *$/SharedAssemblies/Binaries/<release>* folder where *<release>* is version.  In general,
the version number of a shared component will reflect major and minor changes:

**Major changes** --- will increment the whole number part of the version (ex. 1.0 to 2.0):
    * When new .Net Framework is available.
    * When interface-breaking changes are introduced.
    
**Minor changes** --- will increment the fractional number part of the version (ex. 1.0 to 1.1):
    * When new libraries are added or changes in functionality occur that are fairly minor in nature.
    
**Bug-fix changes** --- will not increment the fractional number part of the version (ex. 1.0 to 1.0):
    * Only when repairing a bug that does not change or add any unexpected behaviors.
    
The *Shared Components Committee* will maintain the previous **three** version branches at all times.  If at any time
a code branch becomes deprecated as it is more than three versions old, the users should consider upgrading to a newer
branch.

The components will maintain two version ids: *assembly version* and *file version*.  The *assembly version* will be
equal to the version of the branch itself, and the *file version* will update with each official build so that even if
it is officially the same version of the assembly, minor build changes can be tracked.
    
