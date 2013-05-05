using System.Collections.Generic;
using SharedAssemblies.Security.Authentication.Entities;

namespace SharedAssemblies.Security.Authentication.DataAccess
{
	/// <summary>
	/// Interface for an security data access object
	/// </summary>
	public interface IAuthenticationDao 
	{
		/// <summary>
		/// Inserts a new session into the database
		/// </summary>
		/// <param name="session">The new session to insert in the database</param>
		/// <returns>The result of the operation</returns>
		bool InsertSession(SessionEntity session);

		/// <summary>
		/// Deletes the given session if it exists
		/// </summary>
		/// <param name="vendor">The unique identifier for this vender</param>
		/// <param name="identifier">The unique indentifier for this user</param>
		/// <returns>The result of the operation</returns>
		bool DeleteSession(string vendor, string identifier);
		
		/// <summary>
		/// Gets the session id if it exists
		/// </summary>
		/// <param name="vendor">The unique identifier for this vender</param>
		/// <param name="sessionType">The type of session</param>
		/// <param name="identifier">The unique indentifier for this user</param>
		/// <returns>The session associate with this user</returns>
		SessionEntity GetSessionById(string vendor, SessionType sessionType, string identifier);

		/// <summary>
		/// Get All Account Associated with the account passed.
		/// </summary>
		/// <param name="account">The account id</param>
		/// <returns>list of associated accounts</returns>
		IEnumerable<string> GetLinkedAccounts(string account);
	}
}