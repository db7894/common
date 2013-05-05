using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using InstrumentationLib;

namespace InstrumentationTests
{
	[TestClass]
	public partial class TokenServiceTests
	{
		[TestMethod]
		[HostType("Moles")]
		public void TestGetToken () 
		{
			//arrange
			var list = new List<ITokenPairRecord>();
			var processor = new InstrumentationLib.Moles.MMemoryFile<ITokenPairRecord>()
			{
				Open = () => true,
				WriteT = ( b ) => { list.Add(b); return true; }
			};
			var service = new InstrumentationLib.TokenService(1000, processor.Instance) {Enabled = true};
			const int eventId = 3;
			//act
			var tokenPairRec = service.GetToken(eventId);
			//assert
			System.Threading.Thread.Sleep(1000);
			Assert.AreEqual(1, list.Count, "list count");

			Assert.AreEqual((ulong)1, tokenPairRec.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec.T2.AppId);
			Assert.AreEqual(eventId, tokenPairRec.T2.EventId);

			Assert.AreEqual((ulong)1, list.Select(r => r.T2.Id).First());
			Assert.AreEqual((short)1000, list.Select(r => r.T2.AppId).First());
			Assert.AreEqual(eventId, list.Select(r => r.T2.EventId).First());

			Assert.AreEqual(true, list.Select(r => r.T1.IsNull()).First());
		}

		[TestMethod]
		[HostType("Moles")]
		public void TestGetTokenSplit ()
		{
			//arrange
			var list = new List<ITokenPairRecord>();
			var processor = new InstrumentationLib.Moles.MMemoryFile<ITokenPairRecord>()
			{
				Open = () => true,
				WriteT = ( b ) => { list.Add(b); return true; }
			};
			var service = new InstrumentationLib.TokenService(1000, processor.Instance) { Enabled = true };
			const int eventId1 = 3;
			const int eventId2 = 4;
			var tokenPairRec1 = service.GetToken(eventId1);
			//act
			var tokenPairRec2 = service.SplitToken(eventId2, tokenPairRec1);
			//assert
			System.Threading.Thread.Sleep(2000);
			Assert.AreEqual(2, list.Count());

			Assert.AreEqual((ulong)1, tokenPairRec1.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec1.T2.AppId);
			Assert.AreEqual(eventId1, tokenPairRec1.T2.EventId);

			Assert.AreEqual((ulong)2, tokenPairRec2.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec2.T2.AppId);
			Assert.AreEqual(eventId2, tokenPairRec2.T2.EventId);

			Assert.AreEqual(1, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).Count());
			Assert.AreEqual((short)1000, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.AppId);
			Assert.AreEqual(eventId1, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.EventId);

			Assert.AreEqual(1, list.Where(r => r.T2.Id == 2 && r.T1.Id == 1).Count());
			Assert.AreEqual((short)1000, list.Where(r => r.T2.Id == 2 && r.T1.Id == 1).First().T1.AppId);
			Assert.AreEqual(eventId1, list.Where(r => r.T2.Id == 2 && r.T1.Id == 1).First().T1.EventId);
			Assert.AreEqual((short)1000, list.Where(r => r.T2.Id == 2 && r.T1.Id == 1).First().T2.AppId);
			Assert.AreEqual(eventId2, list.Where(r => r.T2.Id == 2 && r.T1.Id == 1).First().T2.EventId);

		}

		[TestMethod]
		[HostType("Moles")]
		public void TestTokenJoin ()
		{
			//arrange
			var list = new List<ITokenPairRecord>();
			var processor = new InstrumentationLib.Moles.MMemoryFile<ITokenPairRecord>()
			{
				Open = () => true,
				WriteT = ( b ) => { list.Add(b); return true; }
			};
			var service = new InstrumentationLib.TokenService(1000, processor.Instance) { Enabled = true };
			const int eventId1 = 3;
			const int eventId2 = 4;
			const int eventId3 = 5;
			var tokenPairRec1 = service.GetToken(eventId1);
			var tokenPairRec2 = service.GetToken(eventId2);
			//act
			var tokenPairRec3 = service.JoinTokens(eventId3, tokenPairRec1, tokenPairRec2);
			//assert
			System.Threading.Thread.Sleep(10000);
			Assert.AreEqual(3, list.Count());

			Assert.AreEqual((ulong)1, tokenPairRec1.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec1.T2.AppId);
			Assert.AreEqual(eventId1, tokenPairRec1.T2.EventId);

			Assert.AreEqual((ulong)2, tokenPairRec2.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec2.T2.AppId);
			Assert.AreEqual(eventId2, tokenPairRec2.T2.EventId);

			Assert.AreEqual((ulong)1, tokenPairRec3.T1.Id);
			Assert.AreEqual((short)1000, tokenPairRec3.T1.AppId);
			Assert.AreEqual(eventId3, tokenPairRec3.T1.EventId);
			Assert.AreEqual((ulong)2, tokenPairRec3.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec3.T2.AppId);
			Assert.AreEqual(eventId3, tokenPairRec3.T2.EventId);

			Assert.AreEqual((ulong)1, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.Id);
			Assert.AreEqual((short)1000, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.AppId);
			Assert.AreEqual(eventId1, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.EventId);

			Assert.AreEqual((ulong)2, list.Where(r => r.T2.Id == 2 && r.T1.IsNull()).First().T2.Id);
			Assert.AreEqual((short)1000, list.Where(r => r.T2.Id == 2 && r.T1.IsNull()).First().T2.AppId);
			Assert.AreEqual(eventId2, list.Where(r => r.T2.Id == 2 && r.T1.IsNull()).First().T2.EventId);

			Assert.AreEqual(1, list.Where(r => r.T1.Id == 1 && r.T2.Id == 2).Count());
			Assert.AreEqual((short)1000, list.Where(r => r.T1.Id == 1 && r.T2.Id == 2).First().T1.AppId);
			Assert.AreEqual(eventId3, list.Where(r => r.T1.Id == 1 && r.T2.Id == 2).First().T1.EventId);
			Assert.AreEqual((short)1000, list.Where(r => r.T1.Id == 1 && r.T2.Id == 2).First().T2.AppId);
			Assert.AreEqual(eventId3, list.Where(r => r.T1.Id == 1 && r.T2.Id == 2).First().T2.EventId);
		}

		[TestMethod]
		[HostType("Moles")]
		public void TestAddEvent ()
		{
			//arrange
			var list = new List<ITokenPairRecord>();
			var processor = new InstrumentationLib.Moles.MMemoryFile<ITokenPairRecord>()
			{
				Open = () => true,
				WriteT = ( b ) => { list.Add(b); return true; }
			};
			var service = new InstrumentationLib.TokenService(1000, processor.Instance) { Enabled = true };
			const int eventId1 = 3;
			const int eventId2 = 4;
			var tokenPairRec1 = service.GetToken(eventId1);
			//act
			var tokenPairRec2 = service.AddEvent(eventId2, tokenPairRec1);
			//assert
			System.Threading.Thread.Sleep(5000);
			Assert.AreEqual(2, list.Count());

			Assert.AreEqual((ulong)1, tokenPairRec1.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec1.T2.AppId);
			Assert.AreEqual(eventId1, tokenPairRec1.T2.EventId);

			Assert.AreEqual((ulong)1, tokenPairRec2.T1.Id);
			Assert.AreEqual((short)1000, tokenPairRec2.T1.AppId);
			Assert.AreEqual(eventId1, tokenPairRec2.T1.EventId);
			Assert.AreEqual((ulong)1, tokenPairRec2.T2.Id);
			Assert.AreEqual((short)1000, tokenPairRec2.T2.AppId);
			Assert.AreEqual(eventId2, tokenPairRec2.T2.EventId);

			Assert.AreEqual(1, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).Count());
			Assert.AreEqual((short)1000, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.AppId);
			Assert.AreEqual(eventId1, list.Where(r => r.T2.Id == 1 && r.T1.IsNull()).First().T2.EventId);

			Assert.AreEqual(1, list.Where(r => r.T1.Id == 1 && r.T2.Id == 1).Count());
			Assert.AreEqual((short)1000, list.Where(r => r.T1.Id == 1 && r.T2.Id == 1).First().T2.AppId);
			Assert.AreEqual(eventId2, list.Where(r => r.T1.Id == 1 && r.T2.Id == 1).First().T2.EventId);
		}

	}
}
