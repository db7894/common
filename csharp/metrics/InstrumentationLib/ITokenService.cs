using System;

namespace InstrumentationLib
{
	/// <summary>
	/// Interface to a token service implementation
	/// </summary>
	interface ITokenService
	{
		/// <summary>
		/// Enables and disables instrumentation processing.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Constructs and queues a new token with a null token as a token pair rec.
		/// This represents a 'start' event.
		/// </summary>
		/// <param name="eventId">The event id portion of the new token.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		ITokenPairRecord GetToken(byte eventId, Func<Token, Token, long, ITokenPairRecord> factory = null);

		/// <summary>
		/// Changes the event state of the token.
		/// </summary>
		/// <param name="eventId">The new event id.</param>
		/// <param name="currentRecord">The token.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		ITokenPairRecord AddEvent(byte eventId, ITokenPairRecord currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null);

		/// <summary>
		/// Changes the event state of the token.
		/// </summary>
		/// <param name="eventId">The new event id.</param>
		/// <param name="currentRecord">The original token value.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		ITokenPairRecord AddEvent(byte eventId, ulong currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null);

		/// <summary>
		/// Constructs and queues a new token with the original token as a token pair rec.
		/// This represents a 'split' in the event chain.
		/// </summary>
		/// <param name="eventId">The event id porition of the new token.</param>
		/// <param name="currentRecord">The original TokenRecord.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		ITokenPairRecord SplitToken(byte eventId, ITokenPairRecord currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null);

		/// <summary>
		/// Constructs and queues a new token with the original token as a token pair rec.
		/// This represents a 'split' in the event chain.
		/// This signature is for the C++ wrapper types.
		/// </summary>
		/// <param name="eventId">The event id porition of the new token.</param>
		/// <param name="currentRecord">The original token value.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		ITokenPairRecord SplitToken(byte eventId, ulong currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null);

		/// <summary>
		/// Constructs and queues a token pair rec with two original tokens.
		/// This is a 'join' of two event chains.
		/// </summary>
		/// <param name="eventId">The event id representing the join event of both tokens.</param>
		/// <param name="leftRecord">The original token.</param>
		/// <param name="rightRecord">The new token that represents the joined chain.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>The ITokenPairRecord that continues the chain.</returns>
		ITokenPairRecord JoinTokens(byte eventId, ITokenPairRecord leftRecord, ITokenPairRecord rightRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null);
	}
}
