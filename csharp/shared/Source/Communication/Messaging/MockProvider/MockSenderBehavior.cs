using System.Collections;
using System.Collections.Generic;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// A class that represents the mock behavior for a sender.
	/// </summary>
	public class MockSenderBehavior : IEnumerable<Message>
	{
		/// <summary>
		/// True if should throw when sender starts
		/// </summary>
		public bool ShouldThrowOnSenderStart { get; set; }


		/// <summary>
		/// True if should throw when sender stops
		/// </summary>
		public bool ShouldThrowOnSenderStop { get; set; }


		/// <summary>
		/// True if sender should throw on send.
		/// </summary>
		public bool ShouldThrowOnSend { get; set; }


		/// <summary>
		/// The list of messages that were published.
		/// </summary>
		public List<Message> SentMessages { get; private set; }


		/// <summary>
		/// Construct an instance of the mock sender behavior.
		/// </summary>
		public MockSenderBehavior()
		{
			SentMessages = new List<Message>();	
		}


		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An IEnumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Message> GetEnumerator()
		{
			return SentMessages.GetEnumerator();
		}


		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
