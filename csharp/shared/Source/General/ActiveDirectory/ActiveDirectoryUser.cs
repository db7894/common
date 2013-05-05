using System.Collections.Generic;
using System.Security.Principal;

namespace SharedAssemblies.General.ActiveDirectory
{
    /// <summary>
    /// Contains Active Directory information for a user.
    /// </summary>
    public class ActiveDirectoryUser
    {
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; internal set; }

        /// <summary>
		/// Gets the user's employee ID.
        /// </summary>
		/// <value>The user's employee ID.</value>
        public string EmployeeId { get; internal set; }

        /// <summary>
		/// Gets the user's first name.
        /// </summary>
		/// <value>The user's first name.</value>
        public string FirstName { get; internal set; }

        /// <summary>
		/// Gets the user's last name.
        /// </summary>
		/// <value>The user's last name.</value>
        public string LastName { get; internal set; }

        /// <summary>
		/// Gets the user's branch ID.
        /// </summary>
		/// <value>The user's branch ID.</value>
        public string BranchId { get; internal set; }

        /// <summary>
		/// Gets the user's department.
        /// </summary>
        /// <value>The user's department.</value>
        public string Department { get; internal set; }

        /// <summary>
		/// Gets the user's email address.
        /// </summary>
        /// <value>The user's email address.</value>
        public string Email { get; internal set; }

		/// <summary>
		/// Gets the user's title.
		/// </summary>
		/// <value>The user's title.</value>
		public string Title { get; internal set; }

        /// <summary>
		/// Gets the user's security groups.
        /// </summary>
		/// <value>The user's security groups.</value>
        public List<string> SecurityGroups { get; internal set; }

		/// <summary>
		/// Gets the security identifier.
		/// </summary>
		/// <value>The user's security identifier.</value>
		public byte[] SecurityIdentifier { get; internal set; }
    }
}