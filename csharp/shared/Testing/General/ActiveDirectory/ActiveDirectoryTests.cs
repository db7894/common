using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedAssemblies.General.ActiveDirectory.UnitTests
{
	/// <summary>
	/// Unit tests for the SharedAssemblies.General.ActiveDirectory project. These are "live" tests
	/// that actually hit Active Directory and take about a minute to run. Active Directory is the
	/// same in all datacenters (CG and PD) and uses the same connections regardless of where it is
	/// run, so it should be safe to run these from anywhere and have them pass successfully 
	/// unless Active Directory is actually down, which would be a big problem for all applications.
	/// 
	/// These tests are flagged with 'Ignore' for the aftormentioned reasons. We do not need to re-prove
	/// that these tests pass by hitting an external data store everytime a new build is queued.
	/// 
	/// If you are making changes to the AD component and want to ensure it still works as expected, 
	/// run these locally but don't check them in without the 'Ignore' attribute set.
	/// 
	/// </summary>
	[TestClass]
	[Ignore] // for now these take too long to run
	public class ActiveDirectoryTests
	{
		/// <summary>
		/// Global adapter to use in all tests.
		/// </summary>
		private static DirectoryServicesAdapter _directoryServicesAdapter = 
			new DirectoryServicesAdapter();
		
		/// <summary>
		/// Global departments collection to be used by various tests.
		/// </summary>
		private static List<string> _departments = null;
		
		/// <summary>
		/// Global branch collection to be used by various tests.
		/// </summary>
		private static List<string> _branches = null;
		
		/// <summary>
		/// Tests getting an active directory user by user name and ensures the data is as expected.
		/// </summary>
		[TestMethod]
		public void GetActiveDirectoryUserByUserName_Success()
		{
			string expectedUserName = GetRandomUser().UserName;

			ActiveDirectoryUser user = _directoryServicesAdapter.GetUser(expectedUserName);
			Assert.IsNotNull(user);
			Assert.AreEqual(expectedUserName, user.UserName);
		}

		/// <summary>
		/// Tests getting an active directory user by security identifier and ensures the data is as expected.
		/// </summary>
		[TestMethod]
		public void GetActiveDirectoryUserBySecurityIdentifer_Success()
		{
			var expectedSecurityIdentifier = GetRandomUser().SecurityIdentifier;

			ActiveDirectoryUser user = _directoryServicesAdapter.GetUser(expectedSecurityIdentifier);
			Assert.IsNotNull(user);
			Assert.IsTrue(expectedSecurityIdentifier.SequenceEqual(user.SecurityIdentifier));
		}

        /// <summary>
        /// Tests getting an active directory user by employee ID and ensures the data is as expected.
        /// </summary>
        [TestMethod]
        public void GetActiveDirectoryUserByEmployeeId_Success()
        {
            var randomUser = GetRandomUser();

            int employeeIdQuery = 0;
            Assert.IsTrue(int.TryParse(randomUser.EmployeeId, out employeeIdQuery));

            var user = _directoryServicesAdapter.GetUser(employeeIdQuery);
            Assert.IsNotNull(user);
            Assert.IsTrue(string.Compare(user.EmployeeId, employeeIdQuery.ToString()) == 0);
            Assert.IsTrue(string.Compare(randomUser.FirstName, user.FirstName) == 0);
            Assert.IsTrue(string.Compare(randomUser.LastName, user.LastName) == 0);
            Assert.IsTrue(string.Compare(randomUser.Email, user.Email) == 0);
            Assert.IsTrue(string.Compare(randomUser.UserName, user.UserName) == 0);
            Assert.IsTrue(string.Compare(randomUser.Department, user.Department) == 0);
            Assert.IsTrue(string.Compare(randomUser.BranchId, user.BranchId) == 0);
        }

        /// <summary>
        /// This test is checked-in with the "Ignore" attribute since it is impossible to guarantee
        /// that the users looked up by the test will always exist going forward.
        /// However, if someone wants to validate the paths of built-in LDAP, enable this test
        /// and ensure each user still exists in these paths before considering the paths valid.
        /// </summary>
        [TestMethod]
        public void GetSpecificUsers_Success()
        {
            // Location at time of writing: 
            // OU=Protected,OU=Accounts,OU=Bashwork,DC=bashwork,DC=com,bashwork.com
            var user = _directoryServicesAdapter.GetUser("asmall");
            Assert.IsNotNull(user);
            Assert.IsTrue(string.Compare(user.UserName, "asmall", true) == 0);

            // Location at time of writing: 
            // OU=Protected,OU=User,OU=Accounts,OU=Bashwork,DC=bashwork,DC=com,bashwork.com
            user = _directoryServicesAdapter.GetUser("acole");
            Assert.IsNotNull(user);
            Assert.IsTrue(string.Compare(user.UserName, "acole", true) == 0);

            // Location at time of writing: 
            // OU=Corporate,OU=User,OU=Accounts,OU=Bashwork,DC=bashwork,DC=com,bashwork.com
            user = _directoryServicesAdapter.GetUser("dgann");
            Assert.IsNotNull(user);
            Assert.IsTrue(string.Compare(user.UserName, "dgann", true) == 0);

            // Location at time of writing: 
            // OU=Branch,OU=User,OU=Accounts,OU=Bashwork,DC=bashwork,DC=com,bashwork.com
            user = _directoryServicesAdapter.GetUser("jdienes");
            Assert.IsNotNull(user);
            Assert.IsTrue(string.Compare(user.UserName, "jdienes", true) == 0);

            // Location at time of writing: 
            // OU=Service Center,OU=Branch,OU=User,OU=Accounts,OU=Bashwork,DC=bashwork,DC=com,bashwork.com
            user = _directoryServicesAdapter.GetUser("jcampuzano");
            Assert.IsNotNull(user);
            Assert.IsTrue(string.Compare(user.UserName, "jcampuzano", true) == 0);

            // Location at time of writing: 
            // Does not exist!
            user = _directoryServicesAdapter.GetUser("jiiuezpls");
            Assert.IsNull(user);
        }
		
		/// <summary>
		/// Tests getting an active directory user using a custom LDAP path.
		/// </summary>
		[TestMethod]
		public void GetActiveDirectoryUser_CustomLdapPath_Success()
		{
			DirectoryServicesAdapter customAdapter = 
				new DirectoryServicesAdapter("LDAP://bashwork.com/", 
											 "OU=Accounts,OU=Bashwork,DC=bashwork,DC=com",
											 "OU=Corporate,OU=User,");

			List<ActiveDirectoryUser> users = customAdapter.GetUsers("S*");
			Assert.IsTrue(users.Count > 0);
			Assert.IsTrue(users.TrueForAll(user => user.FirstName.ToUpper().StartsWith("S")));
		}

		/// <summary>
		/// Tests that an exepected ArgumentException is thrown based on invalid input.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetActiveDirectoryUser_CustomLdapPathNoPrefix_Fail()
		{
			DirectoryServicesAdapter customAdapter =
				new DirectoryServicesAdapter(string.Empty,
											 "OU=Accounts,OU=Bashwork,DC=bashwork,DC=com",
											 "OU=Corporate,OU=User,");
		}
		
		/// <summary>
		/// Tests that an exepected ArgumentException is thrown based on invalid input.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetActiveDirectoryUser_CustomLdapPathNoOU_Fail()
		{
			DirectoryServicesAdapter customAdapter =
				new DirectoryServicesAdapter("LDAP://bashwork.com/",
											 "OU=Accounts,OU=Bashwork,DC=bashwork,DC=com",
											 "Corporate");
		}
		
		/// <summary>
		/// Searches for users based on criteria that should always be found (as it already has
		/// been in the random user method).
		/// </summary>
		[TestMethod]
		public void SearchForActiveDirectoryUsers_WithoutWildcard_Success()
		{
			// Find a random user with a first name longer than one character
			ActiveDirectoryUser randomUser = GetRandomUser();
			while (randomUser.FirstName.Length < 3)
			{
				randomUser = GetRandomUser();
			}

			string searchParameter = randomUser.FirstName.Remove(new Random().Next(2,
																 randomUser.FirstName.Length));
			List<ActiveDirectoryUser> users =
				_directoryServicesAdapter.GetUsers(searchParameter);
				
			Assert.IsNotNull(users);
			Assert.IsTrue(users.Count > 0);
			bool foundRandomUser = false;
			foreach (ActiveDirectoryUser user in users)
			{
				if (user.UserName.Equals(randomUser.UserName))
				{
					foundRandomUser = true;
					break;
				}
			}
			Assert.IsTrue(foundRandomUser, "Could not find the user we searched by.");
			foreach (ActiveDirectoryUser user in users)
			{
				Assert.IsTrue(string.Compare(user.FirstName.Substring(0, searchParameter.Length),
								  searchParameter,
								  true) == 0,
								  string.Format("Expected: {0}, found: {1}",
												searchParameter,
												user.FirstName.Substring(0,
																		 searchParameter.Length)));
			}
		}
		
		/// <summary>
		/// Ensure users exist in the branch that we have already found.
		/// </summary>
		[TestMethod]
		public void SearchForActiveDirectoryUsersInBranch_Success()
		{
			if (_branches == null)
			{
				PopulateActiveDirectoryBranches();
			}
			
			List<ActiveDirectoryUser> users = 
				_directoryServicesAdapter.GetUsersInBranch(_branches[0]);
			
			Assert.IsNotNull(users);
			Assert.IsTrue(users.Count > 0);
		}
		
		/// <summary>
		/// Ensure that users exist in the department that we have already found.
		/// </summary>
		[TestMethod]
		public void SearchForActiveDirectoryUsersInDepartment_Success()
		{
			if (_departments == null)
			{
				PopulateActiveDirectoryDepartments();
			}
			
			List<ActiveDirectoryUser> users = 
				_directoryServicesAdapter.GetUsersInDepartment(_departments[0]);
			
			Assert.IsNotNull(users);
			Assert.IsTrue(users.Count > 0);
		}
		
		#region Private Methods
		/// <summary>
		/// Populates the global branch collection with all branches in Active Directory.
		/// </summary>
		private static void PopulateActiveDirectoryBranches()
		{
			_branches = _directoryServicesAdapter.GetBranches();
			Assert.IsTrue(_branches.Count > 0, 
						  "Cannot find any branches in provided LDAP paths.");
		}
		
		/// <summary>
		/// Populates the global department collection with all departments in Active Directory.
		/// </summary>
		private static void PopulateActiveDirectoryDepartments()
		{
			_departments = _directoryServicesAdapter.GetDepartments();
			Assert.IsTrue(_departments.Count > 0, 
						  "Cannot find any departments in provided LDAP paths.");
		}
		
		/// <summary>
		/// Grabs a random user from a random branch in Active Directory.
		/// </summary>
		/// <returns>Populated random <see cref="ActiveDirectoryUser">ActiveDirectoryUser</see>
		/// object.</returns>
		private ActiveDirectoryUser GetRandomUser()
		{
			if (_branches == null)
			{
				PopulateActiveDirectoryBranches();
			}

			List<ActiveDirectoryUser> branchUsers = new List<ActiveDirectoryUser>();
			while (branchUsers.Count == 0)
			{
				branchUsers = _directoryServicesAdapter.GetUsersInBranch(_branches[new Random()
																		 .Next(_branches.Count)]);
			}					

			return branchUsers[new Random().Next(branchUsers.Count)];
		}
		#endregion
	}
}
