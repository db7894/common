using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Resources;
using System.Security.Principal;

namespace SharedAssemblies.General.ActiveDirectory
{
    /// <summary>
    /// Provides access to Active Directory for the purpose of retrieving user, department or branch 
	/// information.
    /// </summary>
    public class DirectoryServicesAdapter
    {
        #region Private Members
		
		/// <summary>
		/// The object used to talk to ActiveDirectory.
		/// </summary>
        private readonly DirectoryServices _directoryServices = new DirectoryServices();
        
        /// <summary>
        /// The resource manager object which contains our default connection settings.
        /// </summary>
        private readonly ResourceManager _resourceManager =
			new ResourceManager("SharedAssemblies.General.ActiveDirectory.Resources", 
								Assembly.GetExecutingAssembly());

        /// <summary>
        /// The LDAP search paths container for use with the DirectoryServices object.
        /// </summary>
        private readonly string[] _ldapSearchPaths;
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryServicesAdapter"/> class.
		/// The basic constructor uses the embedded resource file for all connection values.
        /// </summary>
		/// <exception cref="ArgumentException">Thrown if resource file parameters are empty/null 
		/// or not in the correct format.</exception>
        public DirectoryServicesAdapter()
        {
            // Search paths
            string ldapSearchPaths = ConfigurationManager.AppSettings["ldapSearchPaths"];
            if (string.IsNullOrEmpty(ldapSearchPaths))
            {
                ldapSearchPaths = _resourceManager.GetString("ldapSearchPaths");
            }
			
			// Since _ldapSearchPaths is marked readonly we must do this in the constructor.
			if (!string.IsNullOrEmpty(ldapSearchPaths))
			{
				_ldapSearchPaths = ldapSearchPaths.Split(new string[] { "|" },
														 StringSplitOptions.RemoveEmptyEntries);
			}

            // Connection prefix
            string ldapConnectionPrefix = ConfigurationManager.AppSettings["ldapConnectionPrefix"];
            if (string.IsNullOrEmpty(ldapConnectionPrefix))
            {
                ldapConnectionPrefix = _resourceManager.GetString("ldapConnectionPrefix");
            }

            // Connection suffix
            string ldapConnectionSuffix = ConfigurationManager.AppSettings["ldapConnectionSuffix"];
            if (string.IsNullOrEmpty(ldapConnectionSuffix))
            {
                ldapConnectionSuffix = _resourceManager.GetString("ldapConnectionSuffix");
            }

            ConstructLdapSearchPathCollection(ldapConnectionPrefix, ldapConnectionSuffix);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryServicesAdapter"/> class.
        /// </summary>
        /// <param name="ldapConnectionPrefix">The LDAP connection prefix in LDAP://YourPath/ 
		/// format.</param>
        /// <param name="ldapConnectionSuffix">The LDAP connection suffix in OU=YourOU1,OU=YourOU2,
		/// DC=YourDC1,DC=YourDC2 format.</param>
        /// <param name="ldapSearchPaths">The LDAP search paths separated by pipes in OU=First OU 
		/// To Search,|OU=Next OU To Search,|OU=Next OU To Search, format.</param>
		/// <exception cref="ArgumentException">Thrown if provided parameters are empty/null or not 
		/// in the correct LDAP format.</exception>
        public DirectoryServicesAdapter(string ldapConnectionPrefix, 
										string ldapConnectionSuffix, 
										string ldapSearchPaths)
        {
            if (string.IsNullOrEmpty(ldapConnectionPrefix) 
                || string.IsNullOrEmpty(ldapConnectionSuffix) 
                || string.IsNullOrEmpty(ldapSearchPaths)
                || !ldapConnectionPrefix.Contains("LDAP://")
                || !ldapConnectionSuffix.Contains("OU=")
                || !ldapSearchPaths.Contains("OU="))
            {
                throw new ArgumentException("Please provide valid LDAP connection parameters.");
            }

            _ldapSearchPaths = ldapSearchPaths.Split(new string[] { "|" }, 
													 StringSplitOptions.RemoveEmptyEntries);
			
			ConstructLdapSearchPathCollection(ldapConnectionPrefix, ldapConnectionSuffix);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets an ActiveDirectoryUser object for the specified user name.
        /// </summary>
		/// <param name="userName">Windows logon user name of the user to get.</param>
		/// <returns>Populated <c>ActiveDirectoryUser</c> object if found; otherwise, <c>null</c>.
		/// </returns>
        public ActiveDirectoryUser GetUser(string userName)
        {
            ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
            bool wasUserFound = false;
			
			// User name is required
			if (!string.IsNullOrEmpty(userName))
			{
				// Search each path until user is found
				foreach (string searchPath in _ldapSearchPaths)
				{
					if (_directoryServices.FindUser(userName, searchPath, activeDirectoryUser))
					{
						wasUserFound = true;
						break;
					}
				}
			}
            
            return wasUserFound ? activeDirectoryUser : null;
        }

		/// <summary>
		/// Gets an ActiveDirectoryUser object for the specified security identifier.
		/// </summary>
		/// <param name="securityIdentifier">The security identifier.</param>
		/// <returns>Populated <c>ActiveDirectoryUser</c> object if found; otherwise, <c>null</c>.</returns>
		public ActiveDirectoryUser GetUser(byte[] securityIdentifier)
		{
			ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
			bool wasUserFound = false;

			// Secruity Identifier is required
			if (securityIdentifier != null)
			{
				// Search each path until user is found
				foreach (string searchPath in _ldapSearchPaths)
				{
					if (_directoryServices.FindUser(securityIdentifier, searchPath, activeDirectoryUser))
					{
						wasUserFound = true;
						break;
					}
				}
			}

			return wasUserFound ? activeDirectoryUser : null;
		}

        /// <summary>
        /// Gets an ActiveDirectoryUser object for the specified employee ID.
        /// 
        /// A word about this method of finding employee IDs:
        /// Active Directory is always looking at "production" data as it is using the same connection 
        /// no matter where your code is calling it (PD, CG, QA, DEV, etc.). 
        /// 
        /// Bashwork has always had an "issue" of employee IDs in various environments, e.g. the same
        /// employee might have different employee IDs from database to database. Although some efforts
        /// have been tried to keep them in sync, it is generally not the case, especially for newer employees.
        /// 
        /// In general, if you have an employee ID in a data row in a table in QA and then use this 
        /// method to look up who that employee is, it may or may not be accurate.
        /// 
        /// Rules:
        /// - If you are in a production database environment, this component can be considered 100% accurate as
        ///   the IDs in the tables will match to AD.
        /// - When in QA or some other environment, however:
        ///         - If you write the employee ID of the AD record to a table, you can 100% guarantee you will get the 
        ///           correct employee back when reading the ID using this method.
        ///         - If you cannot guarantee the ID source, you may get ID mismatches using this method. For example,
        ///           jsmith in Prod might be ID 9000, but in QA is 7000. If you look up employee 7000 using this component 
        ///           you will not get the jsmith you're looking for, but rather whoever is ID 7000 in production.
        ///  
        /// In general, all of these issues pertain to QA environments only. Production will be fine. Issues only arise
        /// when a user's "environment" employee ID does not match with production.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        /// <returns>
        /// Populated <c>ActiveDirectoryUser</c> object if found; otherwise, <c>null</c>.
        /// </returns>
        public ActiveDirectoryUser GetUser(int employeeId)
        {
            ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
            bool wasUserFound = false;

            // User name is required
            if (employeeId > 0)
            {
                // Search each path until user is found
                foreach (string searchPath in _ldapSearchPaths)
                {
                    if (_directoryServices.FindUser(employeeId, searchPath, activeDirectoryUser))
                    {
                        wasUserFound = true;
                        break;
                    }
                }
            }

            return wasUserFound ? activeDirectoryUser : null;
        }

        /// <summary>
        /// Gets a list of ActiveDirectoryUser objects that match the provided search string.
        /// </summary>
        /// <param name="searchString">The search string, searching by name, first name 
		/// first.</param>
		/// <returns>Populated collection of <c>ActiveDirectoryUser</c> objects found; otherwise, 
		/// an empty list.</returns>
        public List<ActiveDirectoryUser> GetUsers(string searchString)
        {
            List<ActiveDirectoryUser> users = new List<ActiveDirectoryUser>();

            // Search string (the filter) is required
            if (!string.IsNullOrEmpty(searchString))
            {
				// Trim it before inspection
				searchString = searchString.Trim();
				
                // Add wildcard character
                if (!searchString.EndsWith("*"))
                {
                    searchString += "*";
                }

                // Search all paths for all matching users
                foreach (string searchPath in _ldapSearchPaths)
                {
                    _directoryServices.FindUsers(searchString, searchPath, users);
                }
            }
            
            return users;
        }

        /// <summary>
        /// Gets a list of ActiveDirectoryUser objects for users in the department specified in the 
		/// search string.
        /// </summary>
        /// <param name="searchString">The search string.</param>
		/// <returns>Populated collection of <c>ActiveDirectoryUser</c> objects found; otherwise, 
		/// an empty list.</returns>
        public List<ActiveDirectoryUser> GetUsersInDepartment(string searchString)
        {
            List<ActiveDirectoryUser> users = new List<ActiveDirectoryUser>();

            // Search string (the filter) is required
            if (!string.IsNullOrEmpty(searchString))
            {
                // Search all paths for all matching users
                foreach (string searchPath in _ldapSearchPaths)
                {
                    _directoryServices.FindUsersInDepartment(searchString, searchPath, users);
                }
            }
            return users;
        }

        /// <summary>
        /// Gets a list of ActiveDirectoryUser objects for users in the branch specified in the 
		/// search string.
        /// </summary>
        /// <param name="searchString">The search string.</param>
		/// <returns>Populated collection of <c>ActiveDirectoryUser</c> objects found; otherwise, 
		/// an empty list.</returns>
        public List<ActiveDirectoryUser> GetUsersInBranch(string searchString)
        {
            List<ActiveDirectoryUser> users = new List<ActiveDirectoryUser>();

            // Search string (the filter) is required
            if (!string.IsNullOrEmpty(searchString))
            {
                // Search all paths for all matching users
                foreach (string searchPath in _ldapSearchPaths)
                {
                    _directoryServices.FindUsersInBranch(searchString, searchPath, users);
                }
            }
            return users;
        }

        /// <summary>
        /// Gets a list of unique branch IDs.
        /// </summary>
        /// <returns>List of unique branch IDs.</returns>
        public List<string> GetBranches()
        {
            List<string> branches = new List<string>();

            // Search all paths for all unique branches
            foreach (string searchPath in _ldapSearchPaths)
            {
                _directoryServices.GetBranches(searchPath, branches);
            }

            return branches;
        }

        /// <summary>
        /// Gets a list of unique department names.
        /// </summary>
		/// <returns>List of unique department names.</returns>
        public List<string> GetDepartments()
        {
            List<string> departments = new List<string>();

            // Search all paths for all unique departments
            foreach (string searchPath in _ldapSearchPaths)
            {
                _directoryServices.GetDepartments(searchPath, departments);
            }

            return departments;
        }

        #endregion
        
        #region Private Methods
        
        /// <summary>
		/// Helper method used by constructors to build global ldap path collection.
        /// </summary>
        /// <param name="ldapConnectionPrefix">Connection prefix to use.</param>
		/// <param name="ldapConnectionSuffix">Connection suffix to use.</param>
		/// <exception cref="ArgumentException">Thrown if provided parameters are empty/null or not 
		/// in the correct LDAP format.</exception>
		private void ConstructLdapSearchPathCollection(string ldapConnectionPrefix,
													   string ldapConnectionSuffix)
		{
			if (_ldapSearchPaths == null || _ldapSearchPaths.Length == 0)
			{
				throw new ArgumentException("Valid LDAP connection paths not found.");
			}

			for (int i = 0; i < _ldapSearchPaths.Length; i++)
			{
				_ldapSearchPaths[i] = string.Format("{0}{1}{2}",
													ldapConnectionPrefix,
													_ldapSearchPaths[i],
													ldapConnectionSuffix);
			}
		}
		
        #endregion
    }
}
