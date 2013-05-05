=========================================================================
Active Directory --- Active Directory Helper Class
=========================================================================

:Assembly: SharedAssemblies.General.ActiveDirectory.dll
:Namespace: SharedAssemblies.General.ActiveDirectory
:Author: Adam Bleser (`ableser@bashwork.com <mailto:ableser@bashwork.com>`_)
:Date: |today|

.. module:: SharedAssemblies.General.ActiveDirectory
   :platform: Windows, .Net
   :synopsis: ActiveDirectory - ActiveDirectory Helper Class

.. highlight:: csharp

.. index:: ActiveDirectory

Introduction
------------------------------------------------------------

The DirectoryServicesAdapter class provides basic read-only access to common Active
Directory information within the SCOTTRADECORP domain. This is the standard way of
accessing employee information and is the preferred method over querying any database
(such as the Bashwork_Intranet database). 

User information (name, department, title, employee ID, etc) is provided based on a
single Windows user identity or a search string to return a collection of users (first
name first format). Additionally, a search can be performed for a given department or
branch, and finally a unique list of branches or departments can also be queried.

Default LDAP connection and search path information is stored with the component in
a resource file. The values can be overridden in an overloaded constructor.

.. note:: The GetBranches() and GetDepartments() methods are slow to respond since
   they loop through all Active Directory users in order to get all unique branches/departments.
   Response times are typically 30 to 60 seconds.

Usage
------------------------------------------------------------

What follows is a simple example of using the library::

    using SharedAssemblies.General.ActiveDirectory;

    namespace SomeProject.DataAccess
    {
        public class AdSecurityDao : IAdSecurityDao
        {
            private DirectoryServicesAdapter _directoryServices =
                new DirectoryServicesAdapter();
    
            public User GetUser(string identityName)
            {
                User user = null;
                
                ActiveDirectoryUser adUser = _directoryServices.GetUser(identityName);
                
    	        // Example code to populate local user entity with AD entity's information
    	        if (adUser != null)
                {
                    user = new User() 
                    {
                        AccountName = adUser.UserName,
                        Branch = adUser.BranchId,
                        Department = adUser.Department,
                        EmployeeId = adUser.EmployeeId,
                        Name = string.Concat(adUser.FirstName, " ", adUser.LastName),
                        Title = adUser.Title,
                        Email = adUser.Email
                    };
                }
                
                return user;
            }
        }
    }

