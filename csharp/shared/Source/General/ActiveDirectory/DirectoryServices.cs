using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace SharedAssemblies.General.ActiveDirectory
{
	/// <summary>
	/// Internal class responsible for direct interaction with Active Directory.
	/// </summary>
    internal class DirectoryServices
    {
        #region Member Variables
		
		/// <summary>Active Directory key name for department value.</summary>
        private readonly string _departmentPropertyName = "department";
        
		/// <summary>Active Directory key name for physical delivery office name value.</summary>
        private readonly string _branchPropertyName = "physicalDeliveryOfficeName";
        
		/// <summary>Active Directory key name for Bashwork Account Name value.</summary>
        private readonly string _bashworkAccountNamePropertyName = "sAMAccountName";
        
		/// <summary>Active Directory key name for surname value.</summary>
        private readonly string _surnamePropertyName = "sn";
        
		/// <summary>Active Directory key name for given name value.</summary>
        private readonly string _givenNamePropertyName = "givenName";
        
		/// <summary>Active Directory key name for employee ID value.</summary>
        private readonly string _employeeIdPropertyName = "employeeid";
        
		/// <summary>Active Directory key name for email value.</summary>
        private readonly string _emailPropertyName = "mail";
        
		/// <summary>Active Directory key name for title value.</summary>
		private readonly string _titlePropertyName = "title";
		
		/// <summary>Active Directory key name for member of (security group) values.</summary>
        private readonly string _memberOfPropertyName = "memberOf";

		/// <summary>Active Directory key name for AD security identifier value.</summary>
		private readonly string _objectSidPropertyName = "objectSid";
		
		/// <summary>
		/// Regular Expression to ensure query parameters are valid for specific parameter 
		/// searches.
		/// </summary>
        private static readonly Regex _userNamePattern = 
			new Regex(@"^[A-Za-z0-9_\.\-\s]+$", RegexOptions.Compiled 
												| RegexOptions.CultureInvariant);

		/// <summary>
		/// Regular Expression to ensure query parameters are valid for wildcard 
		/// searches.
		/// </summary>												
        private static readonly Regex _userSearchPattern = 
			new Regex(@"^[A-Za-z0-9_*\.\-\s]+$", RegexOptions.Compiled 
												 | RegexOptions.CultureInvariant);
												 
		/// <summary>
		/// Extracts the security group name from the fully-qualifed AD security group.
		/// </summary>
		private static readonly Regex _securityGroupCapturePattern =
			new Regex(@"^CN=(?<SecurityGroup>.+?),", RegexOptions.Compiled
													 | RegexOptions.CultureInvariant
													 | RegexOptions.ExplicitCapture);


        #endregion

        #region Internal Methods

        /// <summary>
        /// Loads the branch names into the specified Dictionary object.
        /// </summary>
        /// <param name="ldapPath">The LDAP path.</param>
		/// <param name="branches">The list to populate with unique branches.</param>
		internal void GetBranches(string ldapPath, ICollection<string> branches)
        {
            GetList(ldapPath, _branchPropertyName, branches);
        }

        /// <summary>
        /// Loads the department names into the specified Dictionary object.
        /// </summary>
        /// <param name="ldapPath">The LDAP path.</param>
		/// <param name="departments">The list to populate with unique departments.</param>
		internal void GetDepartments(string ldapPath, ICollection<string> departments)
        {
            GetList(ldapPath, _departmentPropertyName, departments);
        }

        /// <summary>
        /// Loads the matching users into the specified List object. 
        /// </summary>
        /// <param name="searchString">The search string.</param>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="users">The users.</param>
		internal void FindUsers(string searchString, string ldapPath, 
								ICollection<ActiveDirectoryUser> users) 
        {
            if (_userSearchPattern.Match(searchString).Success)
            {
                GetUsers(String.Format("(&(objectCategory={0})(name={1}))", 
									   "user", searchString), ldapPath, users);
            }
        }

        /// <summary>
        /// Loads the users from the specified department into the specified List object. 
        /// </summary>
        /// <param name="searchString">The department.</param>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="users">The users.</param>
        internal void FindUsersInDepartment(string searchString, string ldapPath, 
											ICollection<ActiveDirectoryUser> users)
        {
            if (_userNamePattern.Match(searchString).Success)
            {
                GetUsers(String.Format("(&(objectCategory={0})(department={1}))", 
									   "user", searchString), ldapPath, users);
            }
        }

        /// <summary>
        /// Loads the users from the specified branch into the specified List object. 
        /// </summary>
        /// <param name="searchString">The branch.</param>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="users">The users.</param>
        internal void FindUsersInBranch(string searchString, string ldapPath,
										ICollection<ActiveDirectoryUser> users)
        {
            if (_userNamePattern.Match(searchString).Success)
            {
                GetUsers(String.Format("(&(objectCategory={0})(physicalDeliveryOfficeName={1}))",
									   "user", searchString), ldapPath, users);
            }
        }

        /// <summary>
        /// Finds the user by employee ID and loads the specified ActiveDirectoryUser object.
        /// </summary>
        /// <param name="employeeId">The employee id.</param>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="activeDirectoryUser">The active directory user.</param>
        /// <returns>
        /// 	<c>true</c> if the user was found; <c>false</c> otherwise.
        /// </returns>
        internal bool FindUser(int employeeId, string ldapPath, ActiveDirectoryUser activeDirectoryUser)
        {
            return FindUser(employeeId.ToString(), "employeeID", ldapPath, activeDirectoryUser);
        }

        /// <summary>
        /// Finds the user and loads the specified ActiveDirectoryUser object.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="activeDirectoryUser">The user info object into which the values are
        /// updated.</param>
        /// <returns>
        /// 	<c>true</c> if the user was found; <c>false</c> otherwise.
        /// </returns>
        internal bool FindUser(string userName, string ldapPath, ActiveDirectoryUser activeDirectoryUser)
        {
            return _userNamePattern.Match(userName).Success
                && FindUser(userName, "sAMAccountName", ldapPath, activeDirectoryUser);
        }

		/// <summary>
		/// Finds the user by security identifier and loads the specified ActiveDirectoryUser object.
		/// </summary>
		/// <param name="securityIdentifier">The security identifier.</param>
		/// <param name="ldapPath">The LDAP path.</param>
		/// <param name="activeDirectoryUser">The active directory user.</param>
		/// <returns>
		/// 	<c>true</c> if the user was found; <c>false</c> otherwise.
		/// </returns>
		internal bool FindUser(byte[] securityIdentifier, string ldapPath, ActiveDirectoryUser activeDirectoryUser)
		{
			return FindUser(new SecurityIdentifier(securityIdentifier, 0).Value, "objectSid", ldapPath, activeDirectoryUser);
		}

        #endregion

        #region Private Methods
        /// <summary>
        /// Finds the user and loads the specified ActiveDirectoryUser object.
        /// </summary>
        /// <param name="queryValue">The user query value.</param>
        /// <param name="queryAttribute">The user query attribute.</param>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="activeDirectoryUser">The user info object into which the values are updated.</param>
        /// <returns>
        /// 	<c>true</c> if the user was found; <c>false</c> otherwise.
        /// </returns>
        private bool FindUser(string queryValue, string queryAttribute, string ldapPath, ActiveDirectoryUser activeDirectoryUser)
        {
            bool wasUserFound = false;

            using (DirectoryEntry directoryEntry = new DirectoryEntry())
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
                directoryEntry.Path = ldapPath;

                using (DirectorySearcher directorySearcher = new DirectorySearcher())
                {
                    directorySearcher.PageSize = 1000;
                    directorySearcher.SearchRoot = directoryEntry;
                    directorySearcher.Filter =
                        string.Format("(&(objectCategory={0})({1}={2}))",
                                        "user", queryAttribute, queryValue);
                    directorySearcher.PropertyNamesOnly = true;

                    AddADPropertiesForUserInfo(directorySearcher);

                    using (SearchResultCollection results = directorySearcher.FindAll())
                    {
                        foreach (SearchResult result in results)
                        {
                            using (DirectoryEntry resultEntry = result.GetDirectoryEntry())
                            {
                                MapADEntryToUserInfo(resultEntry, activeDirectoryUser);
                                wasUserFound = true;
                            }
                            break;
                        }
                    }
                }
            }

            return wasUserFound;
        }

		/// <summary>
		/// Gets a list of ActiveDirectoryUsers based on the provided search string.
		/// </summary>
		/// <param name="searchString">Search parameters to use.</param>
		/// <param name="ldapPath">Path to search in.</param>
		/// <param name="users">User collection to populate.</param>
        private void GetUsers(string searchString, string ldapPath, ICollection<ActiveDirectoryUser> users)
        {
            using (DirectoryEntry directoryEntry = new DirectoryEntry())
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
                directoryEntry.Path = ldapPath;

                using (DirectorySearcher directorySearcher = new DirectorySearcher())
                {
                    directorySearcher.PageSize = 1000;
                    directorySearcher.SearchRoot = directoryEntry;
                    directorySearcher.Filter = searchString;
                    directorySearcher.PropertyNamesOnly = true;

                    AddADPropertiesForUserInfo(directorySearcher);

                    using (SearchResultCollection results = directorySearcher.FindAll())
                    {
                        foreach (SearchResult result in results)
                        {
                            using (DirectoryEntry resultEntry = result.GetDirectoryEntry())
                            {
                                ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                                MapADEntryToUserInfo(resultEntry, activeDirectoryUser);
								if (!users.Contains(activeDirectoryUser))
                                {
									users.Add(activeDirectoryUser);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the list of unique AD items from all users for the specified AD property name.
        /// </summary>
        /// <param name="ldapPath">The LDAP path.</param>
        /// <param name="propertyName">Name of the property.</param>
		/// <param name="list">List to populate with unique string objects for the 
		/// given ldapPath.</param>
        private void GetList(string ldapPath, string propertyName, ICollection<string> list)
        {
            using (DirectoryEntry directoryEntry = new DirectoryEntry())
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
                directoryEntry.Path = ldapPath;

                using (DirectorySearcher directorySearcher = new DirectorySearcher())
                {
                    directorySearcher.PageSize = 1000;
                    directorySearcher.SearchRoot = directoryEntry;
                    directorySearcher.Filter = String.Format("(&(objectCategory={0})(name={1}))", 
															 "user", "*");
                    directorySearcher.PropertyNamesOnly = true;
                    directorySearcher.PropertiesToLoad.Add(propertyName);

                    using (SearchResultCollection results = directorySearcher.FindAll())
                    {
                        foreach (SearchResult result in results)
                        {
                            using (DirectoryEntry resultEntry = result.GetDirectoryEntry())
                            {
                                // Add item to the list if not already added
                                if (resultEntry.Properties.Contains(propertyName))
                                {
                                    string itemName = 
										resultEntry.Properties[propertyName][0].ToString();

                                    if (!list.Contains(itemName))
                                    {
                                        list.Add(itemName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
		
		// Helper method to load up the properties we're requesting.
        private void AddADPropertiesForUserInfo(DirectorySearcher directorySearcher)
        {
            directorySearcher.PropertiesToLoad.AddRange(new string[] 
            {
				_bashworkAccountNamePropertyName,
				_employeeIdPropertyName,
				_givenNamePropertyName,
				_surnamePropertyName,
				_branchPropertyName,
				_departmentPropertyName,
				_emailPropertyName,
				_titlePropertyName,
				_memberOfPropertyName,
				_objectSidPropertyName
			});
        }
		
		/// <summary>
		/// Takes an AD DirectoryEntry object and maps the requested properties to our local object.
		/// </summary>
		/// <param name="resultEntry">DirectoryEntry to map.</param>
		/// <param name="activeDirectoryUser">ActiveDirectoryUser object to map to.</param>
        private void MapADEntryToUserInfo(DirectoryEntry resultEntry, 
										  ActiveDirectoryUser activeDirectoryUser)
        {
			activeDirectoryUser.UserName = 
				GetDirectoryEntryProperty(resultEntry, _bashworkAccountNamePropertyName);
			activeDirectoryUser.EmployeeId = 
				GetDirectoryEntryProperty(resultEntry, _employeeIdPropertyName);
			activeDirectoryUser.FirstName = 
				GetDirectoryEntryProperty(resultEntry, _givenNamePropertyName);
			activeDirectoryUser.LastName = 
				GetDirectoryEntryProperty(resultEntry, _surnamePropertyName);
			activeDirectoryUser.BranchId = 
				GetDirectoryEntryProperty(resultEntry, _branchPropertyName);
			activeDirectoryUser.Department = 
				GetDirectoryEntryProperty(resultEntry, _departmentPropertyName);
            activeDirectoryUser.Email = 
				GetDirectoryEntryProperty(resultEntry, _emailPropertyName);
			activeDirectoryUser.Title = 
				GetDirectoryEntryProperty(resultEntry, _titlePropertyName);
			activeDirectoryUser.SecurityIdentifier = resultEntry.Properties.Contains(_objectSidPropertyName)
				   ? (byte[])resultEntry.Properties[_objectSidPropertyName].Value
				   : null;				

            // Security Groups
            if (resultEntry.Properties.Contains(_memberOfPropertyName))
            {
                activeDirectoryUser.SecurityGroups = 
					ParseSecurityGroups(resultEntry.Properties[_memberOfPropertyName]);
            }
        }
        
        /// <summary>
        /// Checks if the property exists in the directory entry and returns it if it does; 
		/// otherwise returns string.Empty.
        /// </summary>
		/// <param name="directoryEntry">DirectoryEntry to source the property from.</param>
		/// <param name="propertyName">Property to extract.</param>
		/// <returns>The property if found; otherwise string.Empty.</returns>
        private string GetDirectoryEntryProperty(DirectoryEntry directoryEntry, string propertyName)
        {
			return directoryEntry.Properties.Contains(propertyName)
				   ? directoryEntry.Properties[propertyName][0].ToString()
				   : string.Empty;
        }
		
		/// <summary>
		/// Extracts the "memberOf" properties and compilies them into a string collection. 
		/// </summary>
		/// <param name="propertyValues">Property collectoin to parse.</param>
		/// <returns>Parsed property collection string List.</returns>
        private List<string> ParseSecurityGroups(PropertyValueCollection propertyValues)
        {
            List<string> groups = new List<string>();
			
			// Format of PropertyValue string is as follows:
			// CN=SomeGroupName,OU=Security Groups,OU=Accounts,OU=Bashwork,DC=bashwork,DC=com
			// This foreach loop extracts the "SomeGroupName" from the first element and adds it to 
			// the list.
            foreach (string propertyValue in propertyValues)
            {
				MatchCollection matches = _securityGroupCapturePattern.Matches(propertyValue);
				if (matches.Count == 1)
				{
					groups.Add(matches[0].Groups["SecurityGroup"].Value);
				}
            }

            return groups;
        }
        #endregion
    }
}
