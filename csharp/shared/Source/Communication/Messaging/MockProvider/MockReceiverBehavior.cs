using System.Collections;
using System.Collections.Generic;


namespace SharedAssemblies.Communication.Messaging.MockProvider
{
	/// <summary>
	/// A class that represents the mock behavior for a receiver.
	/// </summary>
	public class MockReceiverBehavior : IEnumerable<Message>
	{
		/// <summary>
		/// True if should throw when receiver starts.
		/// </summary>
		public bool ShouldThrowOnReceiverStart { get; set; }


		/// <summary>
		/// True if should throw when receiver stops.
		/// </summary>
		public bool ShouldThrowOnReceiverStop { get; set; }


		/// <summary>
		/// True if the receiver should throw an exception on receive
		/// </summary>
		public bool ShouldThrowOnReceive { get; set; }


		/// <summary>
		/// The list of messages that were published.
		/// </summary>
		public List<Message> MessagesToReceive { get; private set; }


		/// <summary>
		/// Construct an instance of this mock receiver's behavior.
		/// </summary>
		public MockReceiverBehavior()
		{
			MessagesToReceive = new List<Message>();
		}


		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An IEnumerator that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Message> GetEnumerator()
		{
			return MessagesToReceive.GetEnumerator();
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
