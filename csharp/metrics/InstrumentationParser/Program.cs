using System;
using System.IO;
using System.Text;

using InstrumentationLib;

namespace InstrumentationParser
{
	/// <summary>
	/// Simple program to merely parse the file data into a csv file.
	/// Used for debugging purposes.
	/// </summary>
	class Program
	{
		private static string _inFile;
		private static string _outFile;
		private static FileStream _inStream;
		private static FileStream _outStream;

		static void Usage ()
		{
			Console.WriteLine("Usage: InstrumentationParser -i=inputfile -o=outputfile");
		}

		static bool ValidateInFile ( string inFile )
		{
			if ( inFile.Length < 4 )
			{
				Console.WriteLine("Invalid argument \'{0}\'", inFile);
				return false;
			}
			_inFile = inFile.Substring(3);
			if ( !File.Exists(_inFile) )
			{
				Console.WriteLine("Input file does not exist \'{0}\'", _inFile);
			}
			
			return true;
		}

		static bool ValidateOutFile ( string outFile )
		{
			if ( outFile.Length < 4 )
			{
				Console.WriteLine("Invalid argument \'{0}\'", outFile);
				return false;
			}
			_outFile = outFile.Substring(3);
			return true;
		}

		static bool OpenInFile ()
		{
			try
			{
				_inStream = File.Open(_inFile, FileMode.Open);
			}
			catch ( Exception ex )
			{
				Console.WriteLine("Exception opening \'{0}\': {1}", _inFile, ex.Message);
				return false;
			}
			return true;
		}

		static bool OpenOutFile ()
		{
			try
			{
				_outStream = File.Open(_outFile, FileMode.Create);
			}
			catch ( Exception ex )
			{
				Console.WriteLine("Exception opening \'{0}\': {1}", _outFile, ex.Message);
				return false;
			}
			return true;
		}

		static bool ParseArgs ( string[] args )
		{
			bool retVal = true;
			if ( args.Length != 2 )
			{
				Console.WriteLine("Invalid arguments");
				retVal = false;
			}
			else
			{
				foreach ( string s in args )
				{
					if ( s[0] != '-' )
					{
						Console.WriteLine("Invalid argument \'{0}\'", s);
						retVal = false;
						break;
					}
					switch ( s[1] )
					{
						case 'i':
							if ( !ValidateInFile(s) )
							{
								retVal = false;
							}
							break;
						case 'o':
							if ( !ValidateOutFile(s) )
							{
								retVal = false;
							}
							break;
						default:
							Console.WriteLine("Invalid argument \'{0}\'", s);
							retVal = false;
							break;
					}
					if ( !retVal )
					{
						break;
					}
				}
			}

			if ( !retVal )
			{
				Usage();
			}

			return retVal;
		}

		static void OutputToken ( Token token )
		{
			var ae = new ASCIIEncoding();

			byte[] outBytes = ae.GetBytes(token.AppId.ToString());
			_outStream.Write(outBytes, 0, outBytes.Length);
			_outStream.WriteByte((byte)',');
			outBytes = ae.GetBytes(token.EventId.ToString("g"));
			_outStream.Write(outBytes, 0, outBytes.Length);
			_outStream.WriteByte((byte)',');
			outBytes = ae.GetBytes(token.Id.ToString());
			_outStream.Write(outBytes, 0, outBytes.Length);
		}

		public static void OutputTimeStamp ( long timestamp )
		{
			var ae = new ASCIIEncoding();

			byte[] outBytes = ae.GetBytes(timestamp.ToString());
			_outStream.Write(outBytes, 0, outBytes.Length);
		}

		static void DoWork ()
		{
			var bytes = new byte[TokenPairRecord.TokenPairRecSize];
			bool done = false;
			long writeCount = 0;

			while ( !done ) {
				int countRead = _inStream.Read(bytes, 0, TokenPairRecord.TokenPairRecSize);
				if ( countRead == TokenPairRecord.TokenPairRecSize )
				{
					var record = new TokenPairRecord(bytes, 0);
					if ( record.T2.AppId > 0 )
					{
						if ( writeCount > 0 )
						{
							_outStream.WriteByte((byte)'\n');
						}
						OutputToken(record.T1);
						_outStream.WriteByte((byte)',');
						OutputToken(record.T2);
						_outStream.WriteByte((byte)',');
						OutputTimeStamp(record.Ts);
						++writeCount;
					}
				}
				else if ( countRead == 0 ) {
					done = true;
					Console.WriteLine("Wrote {0} records.", writeCount);
				}
				else {
					done = true;
					Console.WriteLine("Invalid input file");
					Console.WriteLine("Wrote {0} records.", writeCount);
				}
			}
		}

		static void Main ( string[] args )
		{
			if ( ParseArgs(args) )
			{
				Console.WriteLine("InstrumentationParser starting....");
				if ( OpenInFile() && OpenOutFile() )
				{
					DoWork();
				}
			}
		}
	}
}
