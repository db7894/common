using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SharedAssemblies.Security.Authentication.DataAccess;
using SharedAssemblies.Security.Authentication.Entities;
using SharedAssemblies.Security.Encryption;
using SharedAssemblies.Security.Encryption.DataAccess;

namespace SharedAssemblies.Security.Authentication
{
	/// <summary>
	/// Implementation of an authentication manager
	/// </summary>
	public class AuthenticationManager : IAuthenticationManager
	{
		#region Private Fields

		/// <summary>
		/// Handle to the underlying data access object
		/// </summary>
		private readonly IAuthenticationDao _authenticationDao;

		/// <summary>
		/// Handle to key manager for getting encryption and signing keys
		/// </summary>
		private readonly AbstractKeyManager _keyManager;

		/// <summary>
		/// Handle to a session cache to prevent hitting the database
		/// more than once for a long term session.
		/// </summary>
		private static SessionTokenCache _sessionCache;

		/// <summary>
		/// The static list of key components that we will use for creating
		/// our authentication keys.
		/// </summary>
		private static readonly IEnumerable<int> _keyComponentIdentifiers =
			new List<int> { 134, 26, 443 };

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationManager"/>
		/// </summary>
		/// <param name="authenticationDao">DAO for authentication</param>
		/// <param name="encryptionKeyDao">DAO for key retrieval</param>
		/// The implementation of <see cref="IAuthenticationDao"/> to use to persist
		/// the generated tokens.
		public AuthenticationManager(IAuthenticationDao authenticationDao,
			IEncryptionKeyDao encryptionKeyDao)
			: this(authenticationDao, encryptionKeyDao,
				new TimeSpan(0, 0, 0), new TimeSpan(0, 0, 0))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthenticationManager"/>
		/// </summary>
		/// <param name="authenticationDao">DAO for authentication</param>
		/// <param name="encryptionKeyDao">DAO for key retrieval</param>
		/// <param name="cacheDuration">Duration for sessions to live in cache</param>
		/// <param name="cacheCleanUpInterval">Duration between cache cleans</param>
		/// The implementation of <see cref="IAuthenticationDao"/> to use to persist
		/// the generated tokens.
		public AuthenticationManager(IAuthenticationDao authenticationDao,
			IEncryptionKeyDao encryptionKeyDao, TimeSpan cacheDuration,
			TimeSpan cacheCleanUpInterval)
		{
			_authenticationDao = authenticationDao;
			_sessionCache = new SessionTokenCache(_authenticationDao, cacheDuration, cacheCleanUpInterval);
			_keyManager = new SqlKeyManager(encryptionKeyDao, _keyComponentIdentifiers);
			_keyManager.Initialize(encryptionKeyDao);
		}

		#region Implementation of IAuthenticationManager

		/// <summary>
		/// Generates an authentication token with the specified data based
		/// on the authentication method specified by <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of authentication performed</param>
		/// <param name="vendorId">The vendor to link to the token</param>
		/// <param name="account">The account to link to the token</param>
		/// <returns>The generated token associated with the specified data</returns>
		/// <throws>ArgumentException</throws>
		/// <throws>DataAccessException</throws>
		public string GenerateToken(SessionType type, string vendorId, string account)
		{
			SessionEntity session = new SessionEntity
			{
				LoginIdentifier = account,
				VenderIdentifier = vendorId,
				SessionValue = EncryptionUtility.GenerateRandomStringToken(16),
			};
			
			switch (type)
			{
				case SessionType.Session:
					session.Type = SessionType.Session;
					session.ShouldEncrypt = true;
					break;

				case SessionType.SingleSignOn:
					session.Type = SessionType.SingleSignOn;
					session.ShouldEncrypt = true;
					break;

				case SessionType.StreamingSession:
					session.Type = SessionType.StreamingSession;
					session.ShouldEncrypt = false;
					break;

				default:
					throw new ArgumentException("Invalid type of authentication specified");
			}

			return AddSessionAndGenerateToken(session);
		}

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
		public bool Validate(SessionType type, string token, string vendorId, string account,
			bool shouldValidateChildren, out AuthenticationError authenticationError)
		{
			bool result;
			authenticationError = AuthenticationError.Other;

			switch (type)
			{
				case SessionType.Session:
					result = ValidateToken(token, vendorId, account, SessionType.Session,
						true, shouldValidateChildren, out authenticationError);
					break;

				case SessionType.SingleSignOn:
					result = ValidateToken(token, vendorId, account, SessionType.SingleSignOn,
						false, false, out authenticationError);
					break;

				case SessionType.StreamingSession:
					result = ValidateStreamSession(token, vendorId, account);
					break;

				default:
					throw new ArgumentException("Invalid type of authentication specified");
			}

			return result;
		}

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
		public bool Validate(SessionType type, string token, string vendorId, string account)
		{
			return Validate(type, token, vendorId, account, false);
		}

		/// <summary>
		/// Validates the authentication token with the specified data based
		/// on the authentication method specified by <paramref name="type"/>
		/// </summary>
		/// <param name="type">The type of authentication performed</param>
		/// <param name="token">The token to validate</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <param name="shouldValidateChildren">should manager cycle through children accounts to 
		/// check if they are the requesting account</param>
		/// <returns>The result of the validation</returns>
		/// <throws>ArgumentException</throws>
		/// <throws>DataAccessException</throws>
		public bool Validate(SessionType type, string token, string vendorId, string account,
			bool shouldValidateChildren)
		{
			AuthenticationError authenticationError;
			return Validate(type, token, vendorId, account, shouldValidateChildren, out authenticationError);
		}

		/// <summary>
		/// Method takes a token and returns the components that make it up
		/// </summary>
		/// <param name="token">The token to break down</param>
		/// <returns>The Session Entity</returns>
		public SessionEntity RetrieveSessionEntity(string token)
		{
			return GetSessionData(token);
		}

		#endregion

		#region Token Generation

		/// <summary>
		/// Helper method to initialize the authenticated session and create
		/// the resulting token.
		/// </summary>
		/// <param name="session">The session for the token</param>
		/// <returns>The generated token associated with the specified data</returns>
		private string AddSessionAndGenerateToken(SessionEntity session)
		{
			string token = null;

			if (_authenticationDao.InsertSession(session))
			{
				token = session.ShouldEncrypt 
				? CreateHashToken(session.SessionValue, session.VenderIdentifier, session.LoginIdentifier) 
				: session.SessionValue;
			}

			if (session.Type == SessionType.Session)
			{
				_sessionCache.AddSessionToCache(session);
			}

			return token;
		}

		#endregion

		#region Token Validation

		/// <summary>
		/// Helper method to validate the token
		/// </summary>
		/// <param name="token">The token to validate</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <param name="sessionType">The type of session to be validated</param>
		/// <param name="shouldUseCache">should it use the cache</param>
		/// <param name="shouldValidateChildren">set to true to check for valid child accounts</param>
		/// <param name="authenticationError">The reason the authentication failed</param>
		/// <returns>If the token is valid</returns>
		private bool ValidateToken(string token, string vendorId, string account,
			SessionType sessionType, bool shouldUseCache, bool shouldValidateChildren,
			out AuthenticationError authenticationError)
		{
			bool isValid = false;
			SessionEntity decryptedSession = GetSessionData(token);
			authenticationError = AuthenticationError.Other;

			if (decryptedSession != null)
			{
				SessionEntity storedSession;

				if (shouldUseCache)
				{
					storedSession = _sessionCache.GetSession(vendorId, decryptedSession.SessionValue,
						decryptedSession.LoginIdentifier, sessionType);
				}
				else
				{
					storedSession = _authenticationDao.GetSessionById(vendorId, sessionType,
						decryptedSession.LoginIdentifier);
				}

				isValid = ValidateSession(decryptedSession, storedSession, out authenticationError)
					&& IsValidAccount(decryptedSession.LoginIdentifier, account, shouldValidateChildren, out authenticationError);
			}

			return isValid;
		}

		/// <summary>
		/// Helper method to validate a stream session
		/// </summary>
		/// <param name="streamSession">string representing the stream session</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <returns>if it is a valid stream session</returns>
		private bool ValidateStreamSession(string streamSession, string vendorId, string account)
		{
			SessionEntity decryptedSession = new SessionEntity
			{
				LoginIdentifier = account,
				SessionValue = streamSession,
				VenderIdentifier = vendorId
			};

			SessionEntity storedSession = _authenticationDao.GetSessionById(vendorId,
				SessionType.StreamingSession, account);

			AuthenticationError authenticationError;
			return ValidateSession(decryptedSession, storedSession, out authenticationError);
		}

		#endregion

		#region Private Helpers

		/// <summary>
		/// helper method for verifying if account is valid
		/// </summary>
		/// <param name="actualAccount">account from token</param>
		/// <param name="requestAccount">account from request</param>
		/// <param name="shouldValidateChildren">if should check children for valid</param>
		/// <param name="authenticationError">The reason the authentication failed</param>
		/// <returns>true or false</returns>
		private bool IsValidAccount(string actualAccount, string requestAccount,
			bool shouldValidateChildren, out AuthenticationError authenticationError)
		{
			bool isValid = false;
			authenticationError = AuthenticationError.InvalidAccount;

			if (actualAccount == requestAccount)
			{
				isValid = true;
			}
			else if (shouldValidateChildren)
			{
				isValid = _authenticationDao.GetLinkedAccounts(actualAccount)
					.Any(child => child == requestAccount);
			}

			return isValid;
		}

		/// <summary>
		/// compares passed entity with what is expected.
		/// </summary>
		/// <param name="actualEntity">Entity built from token</param>
		/// <param name="expectedEntity">Entity from cache or db</param>
		/// <param name="authenticationError">The reason the authentication failed</param>
		/// <returns>true if the values compare correctly</returns>
		private static bool ValidateSession(SessionEntity actualEntity,
			SessionEntity expectedEntity, out AuthenticationError authenticationError)
		{
			authenticationError = AuthenticationError.InvalidSession;
			bool isValid = false;

			// Right now this only validates the session value but this is
			// where all validations should occur other than account, example
			// ipaddress although this wont work now because we dont insert that value.
			if (expectedEntity != null)
			{
				if (!expectedEntity.IsExpired)
				{
					isValid = (actualEntity.SessionValue == expectedEntity.SessionValue);
				}
				else
				{
					isValid = DateTime.Now < expectedEntity.ExpireTime;
				}
			}

			return isValid;
		}
		

		/// <summary>
		/// Helper method to decode and compare encrypted data
		/// </summary>
		/// <param name="token">The token to validate</param>
		/// <returns>The session key if it exists</returns>
		private SessionEntity GetSessionData(string token)
		{
			SessionEntity session = null;
			KeyContainer keys = _keyManager.GetKey();

			byte[] decryptedArray = AuthenticatedEncryption.ValidateAuthenticationCodeThenDecrypt(
				HexUtility.Convert(token), keys.EncryptionKey, keys.SigningKey);

			if (decryptedArray != null)
			{
				var pieces = Encoding.UTF8.GetString(decryptedArray).Split('^');

				if (pieces.Length == 4)
				{
					session = new SessionEntity
					{
						LoginIdentifier = pieces[1],
						VenderIdentifier = pieces[2],
						SessionValue = pieces[3],
						Token = token
					};
				}
			}

			return session;
		}

		/// <summary>
		/// Helper method create a hash token with the supplied data
		/// </summary>
		/// <param name="session">The generated session</param>
		/// <param name="vendorId">The vendor to validate against</param>
		/// <param name="account">The account to validate against</param>
		/// <returns>A computed token with the specified data</returns>
		private string CreateHashToken(string session, string vendorId, string account)
		{
			string token = null;
			string plainText = string.Format("{0}^{1}^{2}^{3}",
				EncryptionUtility.GenerateRandomStringToken(16), account, vendorId, session);

			KeyContainer keys = _keyManager.GetKey();

			if (keys != null)
			{
				var rawToken = AuthenticatedEncryption.EncryptThenAppendAuthenticationCode(
					Encoding.UTF8.GetBytes(plainText), keys.EncryptionKey, keys.SigningKey);
				token = HexUtility.Convert(rawToken);
			}

			return token;
		}

		#endregion
	}
}
