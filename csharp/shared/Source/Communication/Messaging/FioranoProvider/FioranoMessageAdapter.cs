using System.Collections.Generic;
using fiorano.csharp.fms;


namespace SharedAssemblies.Communication.Messaging.FioranoProvider
{
	/// <summary>
	/// Class that converts a message from Fiorano provider to our generic message and
	/// vice versa.
	/// </summary>
	public static class FioranoMessageAdapter
	{
		/// <summary>
		/// Converts message from fFiorano specific to generic message.
		/// </summary>
		/// <param name="providerMessage">The Fiorano message</param>
		/// <returns>The generic abstraction message.</returns>
		public static Message Convert(TextMessage providerMessage)
		{
			var message = new Message(providerMessage.getText());

			var enumerator = providerMessage.getPropertyNames();

			// make sure at least one property
			if (enumerator != null && enumerator.MoveNext())
			{
				message.Properties = new Dictionary<string, string>();

				do
				{
					var name = enumerator.Current.ToString();
					var value = providerMessage.getStringProperty(name);

					if (value != null)
					{
						message.Properties.Add(name, value);
					}
				}
				while (enumerator.MoveNext());
			}

			return message;
		}


		/// <summary>
		/// Converts message from generic message to Fiorano specific message.
		/// </summary>
		/// <param name="message">The generic message to convert.</param>
		/// <param name="session">The queue or topic session to use to create the message.</param>
		/// <returns>The Fiorano-specific message.</returns>
		public static TextMessage Convert(Message message, Session session)
		{
			TextMessage result = session.createTextMessage(message.Text);

			// enumerate through string properties
			if (message.Properties != null)
			{
				foreach (var pair in message.Properties)
				{
					result.setStringProperty(pair.Key, pair.Value);
				}
			}

			return result;			
		}
	}
}
