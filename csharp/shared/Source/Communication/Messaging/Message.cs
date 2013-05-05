using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharedAssemblies.Core.Containers;
using SharedAssemblies.Core.Extensions;

namespace SharedAssemblies.Communication.Messaging
{
	/// <summary>
	/// Class that represents a basic text message sent by a messaging layer.
	/// </summary>
	public class Message : IEnumerable<KeyValuePair<string, string>>
	{
		// empty enumerator if no properties
		private static readonly IEnumerator<KeyValuePair<string, string>> _empty =
			Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();


		/// <summary>
		/// Gets or sets the message text.
		/// </summary>
		public string Text { get; set; }


		/// <summary>
		/// Gets the message properties.
		/// </summary>
		public Dictionary<string, string> Properties { get; set; }


		/// <summary>
		/// Constructs an empty JMS message 
		/// </summary>
		public Message() 
			: this(string.Empty)
		{
		}


		/// <summary>
		/// Constructs a JMS message from a string
		/// </summary>
		/// <param name="message">Text message</param>
		public Message(string message)
		{
			Text = message;
		}


		/// <summary>
		/// Constructs a JMS message from a string
		/// </summary>
		/// <param name="message">Text message.</param>
		/// <param name="property">A list of properties for the message.</param>
		public Message(string message, KeyValuePair<string, string> property)
			: this(message)
		{
			Add(property);
		}


		/// <summary>
		/// Constructs a JMS message from a string
		/// </summary>
		/// <param name="message">Text message.</param>
		/// <param name="properties">A list of properties for the message.</param>
		public Message(string message, IEnumerable<KeyValuePair<string, string>> properties)
			: this(message)
		{
			AddRange(properties);
		}


		/// <summary>
		/// Adds a property to the message using the specified KeyValuePair.
		/// </summary>
		/// <param name="property">The KeyValuePair representing the property key and value.</param>
		public void Add(KeyValuePair<string, string> property)
		{
			Add(property.Key, property.Value);
		}


		/// <summary>
		/// Adds a property to the message with the specified key and value.
		/// </summary>
		/// <param name="key">The key of the property.</param>
		/// <param name="value">The value associated with the key.</param>
		public void Add(string key, string value)
		{
			// lazy construct properties dictionary only when needed.
			if (Properties == null)
			{
				Properties = new Dictionary<string, string>();
			}

			Properties.Add(key, value);
		}


		/// <summary>
		/// Adds an enumeration of  properties to the message with the specified keys and values.
		/// </summary>
		/// <param name="properties">The set of properties to add.</param>
		public void AddRange(IEnumerable<KeyValuePair<string, string>> properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}

			properties.ForEach(pair => Add(pair.Key, pair.Value));
		}


		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An IEnumerator object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return (Properties != null) ? Properties.GetEnumerator() : _empty;
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


		/// <summary>
		/// Converts the message to string by returning the text value of the message.
		/// </summary>
		/// <returns>The text of the message.</returns>
		public override string ToString()
		{
			return string.Format("Message [Text [{0}], Properties [{1}]]",
				Text ?? "null", Properties.Summarize(10));
		}
	}
}
