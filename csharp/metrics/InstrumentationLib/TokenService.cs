using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace InstrumentationLib
{
	/// <summary>
	/// Service through which Tokens are created, split, joined, and changed
	/// </summary>
	public class TokenService : ITokenService
	{
		#region Private Member Fields

		/// <summary>
		/// The logger
		/// </summary>
		private static readonly ILog _log = LogManager.GetLogger(typeof(TokenService));

		/// <summary>
		/// Synchronization for enabling/disabling.
		/// </summary>
		private readonly object _locker = new object();

		/// <summary>
		/// The random number generator used for sampling the distribution
		/// </summary>
		private readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() =>
			new Random(Environment.TickCount));

		/// <summary>
		/// Enabled state.
		/// </summary>
		private bool _isEnabled;

		/// <summary>
		/// The rate at which we should sample the distribution
		/// </summary>
		private readonly double _samplingRate;

		/// <summary>
		/// App id this service is run under.
		/// </summary>
		private readonly int _applicationId = -1;

		/// <summary>
		/// Memory mapped file instance.
		/// </summary>
		private readonly IPostProcessor<ITokenPairRecord> _processor;

		/// <summary>
		/// One up number for next token id.
		/// </summary>
		private long _previousId = 0L;

		/// <summary>
		/// Thread to process queue.
		/// </summary>
		private Thread _processingThread;

		/// <summary>
		/// The concurrent queue to store records.
		/// </summary>
		private ConcurrentQueue<ITokenPairRecord> _processingQueue;

		/// <summary>
		/// Factory used to build a new token pair record
		/// </summary>
		readonly Func<Token, Token, long, ITokenPairRecord> _defaultFactory =
			(t1, t2, s) => new TokenPairRecord(t1, t2, s);
		
		//
		// Constants to hide magic values
		//
		private const ulong SkippedTokenValue = 0UL;
		private const ITokenPairRecord SkippedTokenReference = null;
		private const int OneSecond = 1000;
		private const double SamplingDisabled = 1.0;

		#endregion

		/// <summary>
		/// Enables and disables instrumentation processing.
		/// </summary>
		public bool Enabled
		{
			get { return _isEnabled; }
			set
			{
				if (_isEnabled != value)
				{
					lock (_locker)
					{
						_isEnabled = (value) ? StartProcessing() : StopProcessing();
					}
				}
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="applicationId">Application id this instance runs under.</param>
		/// <param name="processor">A IPostProcessor instance to write to.</param>
		/// <param name="samplingRate">An optional rate to sample with.</param>
		public TokenService(int applicationId, IPostProcessor<ITokenPairRecord> processor,
			double samplingRate = SamplingDisabled)
		{
			_applicationId = applicationId;
			_processor = processor;
			_samplingRate = samplingRate;
			Enabled = false;
		}

		/// <summary>
		/// Constructs and queues a new token with a null token as a token pair rec.
		/// This represents a 'start' event.
		/// </summary>
		/// <param name="eventId">The event id portion of the new token.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		public ITokenPairRecord GetToken(byte eventId, Func<Token, Token, long, ITokenPairRecord> factory = null)
		{
			if (!_isEnabled || ShouldNotSample())
				return SkippedTokenReference;

			long timestamp = Stopwatch.GetTimestamp();
			long id = Interlocked.Increment(ref _previousId);
			var nextToken = new Token(_applicationId, eventId, (ulong)id);
			var nextRecord = (factory ?? _defaultFactory)(Token.Null, nextToken, timestamp);
			_processingQueue.Enqueue(nextRecord);
			return nextRecord;
		}


		/// <summary>
		/// Constructs and queues a new token with the original token as a token pair rec.
		/// This represents a 'split' in the event chain.
		/// </summary>
		/// <param name="eventId">The event id porition of the new token.</param>
		/// <param name="currentRecord">The original TokenRecord.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		public ITokenPairRecord SplitToken(byte eventId, ITokenPairRecord currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null)
		{
			if (!_isEnabled
				|| (currentRecord == SkippedTokenReference)
				||  ShouldNotSample())
				return SkippedTokenReference;

			var currentToken = currentRecord.T2;
			long timestamp = Stopwatch.GetTimestamp();
			long id = Interlocked.Increment(ref _previousId);
			var nextToken = new Token(_applicationId, eventId, (ulong)id);			
			var nextRecord = (factory ?? _defaultFactory)(currentToken, nextToken, timestamp);
			_processingQueue.Enqueue(nextRecord);
			return nextRecord;
		}

		/// <summary>
		/// Constructs and queues a new token with the original token as a token pair rec.
		/// This represents a 'split' in the event chain.
		/// This signature is for the C++ wrapper types.
		/// </summary>
		/// <param name="eventId">The event id porition of the new token.</param>
		/// <param name="currentRecord">The original token value.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		public ITokenPairRecord SplitToken(byte eventId, ulong currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null)
		{
			if (!_isEnabled
				|| (currentRecord == SkippedTokenValue)
				||  ShouldNotSample())
				return SkippedTokenReference;

			var currentToken = new Token(currentRecord);
			long timestamp = Stopwatch.GetTimestamp();
			long id = Interlocked.Increment(ref _previousId);
			var nextToken = new Token(_applicationId, eventId, (ulong)id);			
			var nextRecord = (factory ?? _defaultFactory)(currentToken, nextToken, timestamp);
			_processingQueue.Enqueue(nextRecord);
			return nextRecord;
		}

		/// <summary>
		/// Constructs and queues a token pair rec with two original tokens.
		/// This is a 'join' of two event chains.
		/// </summary>
		/// <param name="eventId">The event id representing the join event of both tokens.</param>
		/// <param name="leftRecord">The original token.</param>
		/// <param name="rightRecord">The new token that represents the joined chain.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>The ITokenPairRecord that continues the chain.</returns>
		public ITokenPairRecord JoinTokens(byte eventId,
			ITokenPairRecord leftRecord, ITokenPairRecord rightRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null)
		{
			if (!_isEnabled
				|| (leftRecord == SkippedTokenReference)
				|| (rightRecord == SkippedTokenReference))
				return SkippedTokenReference;

			long timestamp = Stopwatch.GetTimestamp();
			var leftToken = leftRecord.T2;
			var rightToken = rightRecord.T2;
			leftToken.EventId = eventId;
			rightToken.EventId = eventId;
			var nextRecord = (factory ?? _defaultFactory)(leftToken, rightToken, timestamp);
			_processingQueue.Enqueue(nextRecord);
			return nextRecord;
		}

		/// <summary>
		/// Changes the event state of the token.
		/// </summary>
		/// <param name="eventId">The new event id.</param>
		/// <param name="currentRecord">The token.</param>
		/// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
		/// <returns>A new ITokenPairRecord.</returns>
		public ITokenPairRecord AddEvent(byte eventId, ITokenPairRecord currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null)
		{
			if (!_isEnabled || (currentRecord == SkippedTokenReference))
				return SkippedTokenReference;

			long timestamp = Stopwatch.GetTimestamp();
			var currentToken = currentRecord.T2;
			var nextToken = new Token(currentToken.Value) { EventId = eventId };
			var nextRecord = (factory ?? _defaultFactory)(currentToken, nextToken, timestamp);
			_processingQueue.Enqueue(nextRecord);
			return nextRecord;
		}

        /// <summary>
        /// Changes the event state of the token.
        /// </summary>
        /// <param name="eventId">The new event id.</param>
        /// <param name="currentRecord">The original token value.</param>
        /// <param name="factory">An ITokenPairRecord factory(T1, T2, Ts).</param>
        /// <returns>A new ITokenPairRecord.</returns>
        public ITokenPairRecord AddEvent(byte eventId, ulong currentRecord,
			Func<Token, Token, long, ITokenPairRecord> factory = null)
        {
			if (!_isEnabled || (currentRecord == SkippedTokenValue))
				return SkippedTokenReference;

            long timestamp = Stopwatch.GetTimestamp();
            var currentToken = new Token(currentRecord);
            var nextToken = new Token(currentToken.Value) { EventId = eventId };
            var nextRecord = (factory ?? _defaultFactory)(currentToken, nextToken, timestamp);
            _processingQueue.Enqueue(nextRecord);
            return nextRecord;
        }

        /// <summary>
		///  Thread procedure to write the data from the queue to the post processor.
		/// </summary>
		private void TokenServiceProcessor()
		{
			try
			{
				while (_isEnabled)
				{
					ITokenPairRecord record;
					if (_processingQueue.TryDequeue(out record))
					{
						if (!_processor.Write(record))
						{
							Enabled = false;
						}
					}
					else
					{
						Thread.Sleep(OneSecond);
					}
				}
			}
			catch (ThreadAbortException)
			{}
		}

		/// <summary>
		/// Checks to see if the next token requested should be
		/// serviced or not
		/// </summary>
		/// <returns>true if this should not be sampled, otherwise false</returns>
		private bool ShouldNotSample()
		{
			return (_samplingRate != SamplingDisabled)
				&& (_samplingRate < _random.Value.NextDouble());
		}

		/// <summary>
		/// Stop the token processing service
		/// </summary>
		/// <returns>true if running, false otherwise</returns>
		private bool StopProcessing()
		{
			if (_processingThread != null)
			{
				_processingThread.Abort();
			}
			_processingThread = null;

			if (_processor != null)
			{
				_log.InfoFormat("Closing Post Processor.");
				_processor.Close();
				_processingQueue = null;
			}

			return false;
		}

		/// <summary>
		/// Start the token service processing
		/// </summary>
		/// <returns>true if running, false otherwise</returns>
		private bool StartProcessing()
		{
			var result = false;

			if (_processor != null)
			{
				if (_processor.Open())
				{
					_processingQueue = new ConcurrentQueue<ITokenPairRecord>();
					if (_processingThread == null)
					{
						_processingThread = new Thread(TokenServiceProcessor);
						_processingThread.Start();
						result = true;
					}
				}
			}

			return result;
		}
	}
}
