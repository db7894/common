using SharedAssemblies.Security.Authentication.Entities;

namespace SharedAssemblies.Security.Authentication
{
	/// <summary>
	/// Interface for generating and persisting authentication tokens
	/// </summary>
	public interface IAuthenticationManager
	{
		/// <summary>
		/// Generates an authentication token with the specified data based
		/// on the authentication method specified by <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of authentication performed</param>
		/// <param name="vendorId">The vendor to link to the token</param>
		/// <param name="account">The account to link to the token</param>
		/// <returns>The generated token associated with the specified data</returns>
		string GenerateToken(SessionType type, string vendorId, string account);

		/// <summary>
		/// Validates the authentication token with the specified data based
		/// on the authentication method specified by <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of authentication performed</param>
		/// <param name="token">The token to validate</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <param name="shouldValidateChildren">
		/// Should the children accounts be validated for the active session
		/// </param>
		/// <param name="authenticationError">The reason the authentication failed</param>
		/// <returns>The result of the validation</returns>
		bool Validate(SessionType type, string token, string vendorId, string account,
			bool shouldValidateChildren, out AuthenticationError authenticationError);

		/// <summary>
		/// Validates the authentication token with the specified data based
		/// on the authentication method specified by <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of authentication performed</param>
		/// <param name="token">The token to validate</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <param name="shouldValidateChildren">
		/// Should the children accounts be validated for the active session
		/// </param>
		/// <returns>The result of the validation</returns>
		bool Validate(SessionType type, string token, string vendorId, string account,
			bool shouldValidateChildren);

		/// <summary>
		/// Validates the authentication token with the specified data based
		/// on the authentication method specified by <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of authentication performed</param>
		/// <param name="token">The token to validate</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <returns>The result of the validation</returns>
		/// <throws>ArgumentException</throws>
		/// <throws>DataAccessException</throws>
		bool Validate(SessionType type, string token, string vendorId, string account);

		/// <summary>
		/// Method takes a token and returns the components that make it up
		/// </summary>
		/// <param name="token">The token to break down</param>
		/// <returns>The Session Entity</returns>
		SessionEntity RetrieveSessionEntity(string token);
	}
}
