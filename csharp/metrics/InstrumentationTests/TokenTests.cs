using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InstrumentationTests
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public partial class TokenTests
	{
		[TestMethod]
		public void CheckTokenCreation1 ()
		{
			// arrange
			// act
			var token = new InstrumentationLib.Token(2, 3, 456);
			// assert
			Assert.AreEqual(2, token.AppId);
			Assert.AreEqual(3, token.EventId);
			Assert.AreEqual((ulong)456, token.Id);
		}

		[TestMethod]
		public void CheckTokenCreation2 ()
		{
			// arrange
			//     0           128        48           0         0         0         1        200
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0001|1100|1000
			//      2               3                                456
			byte[] ba = { 200, 1, 0, 0, 0, 48, 128, 0 };
			var token = new InstrumentationLib.Token(2, 3, 456);
			// act
			var token2 = new InstrumentationLib.Token(ba);
			// assert
			Assert.AreEqual(2, token2.AppId);
			Assert.AreEqual(3, token2.EventId);
			Assert.AreEqual((ulong)456, token2.Id);
		}

		[TestMethod]
		public void CheckTokenCreation3 ()
		{
			// arrange
			//     0           128        48           0         0         0         0         4
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0000|0000|0100
			//      2               3                                4
			byte[] ba = {4,0,0,0,0,48,128,0};
			// act
			var token = new InstrumentationLib.Token(ba, 0);
			// assert
			Assert.AreEqual(2, token.AppId);
			Assert.AreEqual(3, token.EventId);
			Assert.AreEqual((ulong)4, token.Id);
		}

		[TestMethod]
		[ExpectedExceptionAttribute(typeof(System.ArgumentException))]
		public void CheckTokenCreationWithBadLength ()
		{
			// arrange
			var token = new InstrumentationLib.Token(2, 3, 456);
			byte[] ba = token.ToByteArray();
			// act
			var token2 = new InstrumentationLib.Token(ba, 3);
			// assert
		}

		[TestMethod]
		public void CheckAddEvent ()
		{
			// arrange
			// act
			var token = new InstrumentationLib.Token(2, 3, 456)
			            	{
			            		EventId = 4
			            	};
			// assert
			Assert.AreEqual(2, token.AppId);
			Assert.AreEqual(4, token.EventId);
			Assert.AreEqual((ulong)456, token.Id);
		}

		[TestMethod]
		public void CheckToByteArray ()
		{
			// arrange
			//     0           128        48           0         0         0         0         4
			// 0000|0000|10   00|0000|0011|   0000|0000|0000|0000|0000|0000|0000|0000|0000|0000|0100
			//      2               3                                4
			var token = new InstrumentationLib.Token(2, 3, 4);
			// act
			byte[] ba = token.ToByteArray();
			// assert
			Assert.AreEqual(0, ba[7]);
			Assert.AreEqual(128, ba[6]);
			Assert.AreEqual(48, ba[5]);
			Assert.AreEqual(0, ba[4]);
			Assert.AreEqual(0, ba[3]);
			Assert.AreEqual(0, ba[2]);
			Assert.AreEqual(0, ba[1]);
			Assert.AreEqual(4, ba[0]);
		}


	}
}
