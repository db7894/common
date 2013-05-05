using System;
using SharedAssemblies.General.Caching;
using SharedAssemblies.Security.Authentication.DataAccess;
using SharedAssemblies.Security.Authentication.Entities;

namespace SharedAssemblies.Security.Authentication
{
	/// <summary>
	/// Class to perform smart caching of the sessions
	/// </summary>
	internal sealed class SessionTokenCache 
	{
		/// <summary>
		/// Handle to our underlying cache.
		/// </summary>
        private static Cache<string, SessionEntity> _cache;

		/// <summary>
		/// Handle to the underlying data access object
		/// </summary>
		private readonly IAuthenticationDao _database;

        /// <summary>
        /// Amount of time the item should live in the cache
        /// </summary>
	    private readonly TimeSpan _cacheDuration;
        
		/// <summary>
		/// Initializes a new instance of the <see cref="SessionTokenCache"/> class
		/// </summary>
		/// <param name="database">the dao to get the cache from</param>
		/// <param name="cacheDuration">the duration of the cache items</param>
		/// <param name="cacheCleanUpInterval">the cleanup interval for the cache</param>
		public SessionTokenCache(IAuthenticationDao database, TimeSpan cacheDuration,
			TimeSpan cacheCleanUpInterval)
		{
		    _database = database;
		    _cacheDuration = cacheDuration;
		    _cache = new Cache<string, SessionEntity>(
				new TimeSpanExpirationStrategy<SessionEntity>(_cacheDuration), cacheCleanUpInterval);
		}

		/// <summary>
		/// Retrieve the <see cref="SessionEntity"/> from the cache if it
		/// exists or from the database
		/// </summary>
		/// <param name="vendor">The vendor this session is associated with</param>
		/// <param name="session">The session to retrieve</param>
        /// <param name="loginIdentifier">username or account</param>
        /// <param name="sessionType">Type of session</param>
		/// <returns>The <see cref="SessionEntity"/> linked to the session</returns>
		public SessionEntity GetSession(string vendor, string session, string loginIdentifier,
			SessionType sessionType)
		{
			//Get any cached value this includes values that would be expired
		    SessionEntity result = _cache.GetValue(session);

			// Nothing found so go look up in DB
			if (result == null)
			{
				result = _database.GetSessionById(vendor, sessionType, loginIdentifier);
				
                if (result != null)
				{
					AddSessionToCache(result);
				}
			}
			// Session found
			else
			{
				// Session found but is marked invalid which basically means its older than the set timespan
				if (!_cache.IsValid(result.SessionValue))
				{
					//Since we dont know if the session we got is still good we need to check the DB
					var dbResult = _database.GetSessionById(vendor, sessionType, loginIdentifier);

					//result found in db
					if (dbResult != null)
					{
						//Checking to see if the session found in the cache matches the one in the DB
						if (result.SessionValue != dbResult.SessionValue)
						{
							//The session do not match so need to check if the cache session has already been marked as expired
							if (!result.IsExpired)
							{
								result.ExpireTime = DateTime.Now.AddMinutes(1);
								result.IsExpired = true;
							}
						}
						else
						{
							//if they do match then just need to update the cache
							AddSessionToCache(dbResult);
						}
					}
					//no result found which means that there is no active session so no old sessions should be allowed either
					else
					{
						result = null;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Add a session to the session cache.
		/// </summary>
		/// <param name="session">The session to add</param>
		public void AddSessionToCache(SessionEntity session)
		{
			_cache.Add(session.SessionValue, session);
		}
	}
}
