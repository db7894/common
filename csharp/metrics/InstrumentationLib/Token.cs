using System;

namespace InstrumentationLib
{
	/// <summary>
	/// A wrapper for a 64 bit token value that represents an instrumentation event.
	/// </summary>
	public struct Token
	{
		/// <summary>
		/// The token.
		/// </summary>
		private ulong _baseToken;

		/// <summary>
		/// Application Id, 10 most significant bits
		/// </summary>
		public short AppId
		{
			get { return (short)((_baseToken & 0xffC0000000000000) >> 54); }
		}
		/// <summary>
		/// Event Id, 10 bits.
		/// </summary>
		public byte EventId
		{
			get { return (byte)((_baseToken & 0x003ff00000000000) >> 44); }
			set
			{
				_baseToken &= 0xffc00fffffffffff;
				_baseToken |= (((ulong)value & 0x3ff) << 44);
			}
		}
		/// <summary>
		/// Token id, 44 least significant bits
		/// </summary>
		public ulong Id
		{
			get { return (_baseToken & 0x00000fffffffffff); }
		}

		/// <summary>
		/// The token as a 64 bit value.
		/// </summary>
		public ulong Value
		{
			get { return _baseToken; }
		}

		/// <summary>
		/// Size of the token in bytes.
		/// </summary>
		public const int TokenSize = sizeof(ulong);

		/// <summary>
		/// A representation of a null Token
		/// </summary>
		public static readonly Token Null = new Token(0);

		/// <summary>
		/// Constructor from a token value (technically a copy constructor).
		/// </summary>
		/// <param name="tokenValue">The token value.</param>
		public Token(ulong tokenValue)
		{
			_baseToken = tokenValue;
		}

		/// <summary>
		/// Constructor from the token parts.
		/// </summary>
		/// <param name="appId">The app id.</param>
		/// <param name="eventId">The event id.</param>
		/// <param name="id">The token id.</param>
		public Token(int appId, byte eventId, ulong id)
		{
			_baseToken = ((ulong)appId << 54) | ((ulong)eventId << 44) | (id & 0x00000fffffffffff);
		}

		/// <summary>
		/// Demarshal constructor.
		/// </summary>
		/// <param name="byteArray">The token representation in byte form.</param>
		public Token(byte[] byteArray)
			: this(byteArray, 0)
		{
		}

		/// <summary>
		/// Demarshal constructor.
		/// </summary>
		/// <param name="byteArray">The token representation in byte form.</param>
		/// <param name="offset">The starting offset.</param>
		public Token(byte[] byteArray, int offset)
		{
			if (byteArray.Length - offset < TokenSize)
			{
				throw new ArgumentException(string.Format("Array length must be >= {0}", TokenSize));
			}
			_baseToken = 0;
			// little Endian
			for (int index = 0; index < TokenSize; ++index)
			{
				_baseToken |= ((ulong)(byteArray[offset + index]) << (index * 8));
			}
		}

		/// <summary>
		/// Test for a null token.
		/// </summary>
		/// <returns>True if null.</returns>
		public bool IsNull()
		{
			return (_baseToken == 0);
		}

		/// <summary>
		/// Marshal to a byte array.
		/// </summary>
		/// <returns></returns>
		public byte[] ToByteArray()
		{
			var result = new byte[TokenSize];
			for (var index = 0; index < TokenSize; ++index)
			{
				result[index] = (byte)(_baseToken >> ((index * 8)));
			}
			return result;
		}
	}
}
