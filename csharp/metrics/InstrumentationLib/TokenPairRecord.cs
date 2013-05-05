using System;
using System.Diagnostics;

namespace InstrumentationLib
{
	/// <summary>
	/// Class to represent a token record in the file.
	/// </summary>
	public class TokenPairRecord : ITokenPairRecord
	{
		/// <summary>
		/// Used to determine ticks per micro based on the installed clock.
		/// </summary>
		private static readonly double _timerTicksPerMicro = Stopwatch.Frequency / (1000.0 * 1000.0);

		/// <summary>
		/// Used to determine ticks per micro based on the installed clock.
		/// </summary>
		public double TimerTicksPerMicro
		{
			get { return _timerTicksPerMicro; }
		}

		/// <summary>
		/// The size of the marshaled data.
		/// </summary>
		public const int TokenPairRecSize = 2 * Token.TokenSize + sizeof(long);

		/// <summary>
		/// Provides a way for the C++ client types to wrap the interesting token of the record (token context).
		/// </summary>
		public ulong TokenContext
		{
			get { return T2.Value; }
		}

		/// <summary>
		/// Convenience Constructor
		/// </summary>
		public TokenPairRecord(Token t1, Token t2, long timestamp)
		{
			T1 = t1;
			T2 = t2;
			Ts = timestamp;
		}

		/// <summary>
		/// Construct from a tokenContext.
		/// This is a kluge to take the uint64 from an external C++ struct to our true token context.
		/// The timestamp is irrelavant because it never gets emitted.  The only thing that is
		/// important is T2 to carry on the chain.
		/// </summary>
		/// <param name="tokenContext">the encoded token context</param>
		public TokenPairRecord(ulong tokenContext)
			: this(Token.Null, new Token(tokenContext), Stopwatch.GetTimestamp())
		{
		}

		/// <summary>
		/// Construct from marshaled data
		/// </summary>
		/// <param name="byteArray">The marshaled data.</param>
		/// <param name="offset">The starting offset.</param>
		public TokenPairRecord(byte[] byteArray, int offset)
		{
			if (byteArray.Length - offset < TokenPairRecSize)
			{
				throw new ArgumentException(string.Format("Array length must be >= {0}", TokenPairRecSize));
			}
			T1 = new Token(byteArray, offset);
			T2 = new Token(byteArray, Token.TokenSize + offset);
			offset += (2 * Token.TokenSize);
			Ts = 0;
			// little Endian
			for (int index = 0; index < sizeof(long); ++index)
			{
				Ts |= ((long)byteArray[offset + index] << (index * 8));
			}
		}

		#region ITokenPairRecord Members

		/// <summary>
		/// The first Token.
		/// </summary>
		public Token T1 { get; set; }

		/// <summary>
		/// The second Token.
		/// </summary>
		public Token T2 { get; set; }

		/// <summary>
		/// A time stamp.
		/// </summary>
		public long Ts { get; set; }

		#endregion

		#region IOutputRecord Members

		/// <summary>
		/// The length of the marshaled data.
		/// </summary>
		public int Length { get { return TokenPairRecSize; } }

		/// <summary>
		/// The offset of the marshaled data.
		/// </summary>
		public int Offset { get { return 0; } }

		/// <summary>
		/// Marshal the data to a byte array
		/// </summary>
		/// <returns>A byte array containing the marshaled data</returns>
		public byte[] ToByteArray()
		{
			byte[] result = new byte[TokenPairRecSize];

			byte[] t1 = T1.ToByteArray();
			Buffer.BlockCopy(t1, 0, result, 0, t1.Length);

			byte[] t2 = T2.ToByteArray();
			Buffer.BlockCopy(t2, 0, result, t1.Length, t2.Length);
			long ticks = (long)(Ts / TimerTicksPerMicro);
			byte[] ts = BitConverter.GetBytes(ticks);
			if (BitConverter.IsLittleEndian)
			{
				Buffer.BlockCopy(ts, 0, result, t1.Length + t2.Length, ts.Length);
			}
			else
			{
				int outIndex = 0;
				for (int index = ts.Length - 1; index >= 0; --index)
				{
					result[t1.Length + t2.Length + outIndex] = ts[index];
					++outIndex;
				}
			}

			return result;
		}

		#endregion
	}

}
