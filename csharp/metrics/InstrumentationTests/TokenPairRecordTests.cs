using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InstrumentationTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public partial class TokenPairRecordTests
	{
		[TestMethod]
		public void CheckTokenPairRecordCreation ()
		{
			// arrange
			//     0           128        48           0         0         0         0         4
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0000|0000|0100
			//      2               3                                4
			var token1 = new InstrumentationLib.Token(2, 3, 4);
			//     0           128        48           0         0         0         0         5
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0000|0000|0101
			//      2               3                                5
			const long timestamp = (long)0x00ee0000000000ff;

			byte[] ba =
			{
			  4, 0, 0, 0, 0, 48, 128, 0,                  	
			  5, 0, 0, 0, 0, 48, 128, 0,
			  0xff, 0, 0, 0, 0, 0, 0xee, 0
			};

			// act
			var record = new InstrumentationLib.TokenPairRecord(ba, 0);
			// assert
			Assert.AreEqual((short)2, record.T1.AppId);
			Assert.AreEqual((byte)3, record.T1.EventId);
			Assert.AreEqual((ulong)4, record.T1.Id);
			Assert.AreEqual((short)2, record.T2.AppId);
			Assert.AreEqual((byte)3, record.T2.EventId);
			Assert.AreEqual((ulong)5, record.T2.Id);
			Assert.AreEqual(timestamp, record.Ts);
		}

		[TestMethod]
		[HostType("Moles")]
		public void CheckToByteArray ()
		{
			// arrange
			//     0           128        48           0         0         0         0         4
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0000|0000|0100
			//      2               3                                4
			var token1 = new InstrumentationLib.Token(2, 3, 4);
			//     0           128        48           0         0         0         0         5
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0000|0000|0101
			//      2               3                                5
			var token2 = new InstrumentationLib.Token(2, 3, 5);
			const long timestamp = (long)0x00ee0000000000ff;

			InstrumentationLib.Moles.MTokenPairRecord.AllInstances.TimerTicksPerMicroGet = (_) => { return 1.0; };
			var record = new InstrumentationLib.TokenPairRecord(token1, token2, timestamp);
			// act
			byte[] ba = record.ToByteArray();
			// assert
			// Token1
			Assert.AreEqual(0, ba[7]);
			Assert.AreEqual(128, ba[6]);
			Assert.AreEqual(48, ba[5]);
			Assert.AreEqual(0, ba[4]);
			Assert.AreEqual(0, ba[3]);
			Assert.AreEqual(0, ba[2]);
			Assert.AreEqual(0, ba[1]);
			Assert.AreEqual(4, ba[0]);
			// Token2
			Assert.AreEqual(0, ba[15]);
			Assert.AreEqual(128, ba[14]);
			Assert.AreEqual(48, ba[13]);
			Assert.AreEqual(0, ba[12]);
			Assert.AreEqual(0, ba[11]);
			Assert.AreEqual(0, ba[10]);
			Assert.AreEqual(0, ba[9]);
			Assert.AreEqual(5, ba[8]);
			// Timestamp
			Assert.AreEqual(0, ba[23]);
			Assert.AreEqual(0xee, ba[22]);
			Assert.AreEqual(0, ba[21]);
			Assert.AreEqual(0, ba[20]);
			Assert.AreEqual(0, ba[19]);
			Assert.AreEqual(0, ba[18]);
			Assert.AreEqual(0, ba[17]);
			Assert.AreEqual(0xff, ba[16]);
		}


	}
}
