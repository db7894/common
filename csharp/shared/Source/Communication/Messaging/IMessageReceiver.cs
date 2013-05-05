using System;


namespace SharedAssemblies.Communication.Messaging 
{
	/// <summary>
	/// The interface for a message receiver that listens to the message bus and
	/// notifies any listeners when a message is received.
	/// </summary>
	public interface IMessageReceiver : IMessagingConnection
	{
		/// <summary>
		/// Gets the properties for this receiver.
		/// </summary>
		ReceiverProperties ReceiverProperties { get;  }


		/// <summary>
		/// Gets the number of messages received but not yet processed.
		/// </summary>
		long AwaitingProcessingCount { get; }


		/// <summary>
		/// Gets the number of messages completely processed.
		/// </summary>
		long ProcessedCount { get; }
	}
}
