using System.Reflection;
using System.Runtime.InteropServices;
using SharedAssemblies.Monitoring.AutoCounters.Configuration;


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("$safeprojectname$")]
[assembly: AssemblyDescription("A generic Windows service.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bashwork")]
[assembly: AssemblyProduct("$safeprojectname$")]
[assembly: AssemblyCopyright("Copyright © Bashwork 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]


// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0858b9d3-058c-4535-9f5a-6a8a32280651")]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]


// Add custom autocounters desired here.  If you change the name of ServiceImplementation
// you should change it in the counter category and all other counter references below.
[assembly: AutoCounterCategory("$safeprojectname$")]

// defines a heartbeat that will automatically update every 3000 milliseconds.
[assembly: AutoHeartbeat("$safeprojectname$", "LastHeartbeat", 3000)]
