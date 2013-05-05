using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace SharedAssemblies.General.Logging
{
	/// <summary>
	/// This class creates and serializes a list of logging properties for aid in debugging.
	/// </summary>
	public class LogMessageProperties : IEnumerable<KeyValuePair<string, object>>
	{
		/// <summary>
		/// Constant representing the common key for account
		/// </summary>
		public const string AccountKey = "Account";

		/// <summary>
		/// Constant representing the common key for source IP
		/// </summary>
		public const string IpKey = "Ip";

		/// <summary>
		/// Constant representing the common key for user name
		/// </summary>
		public const string UserKey = "User";

		/// <summary>
		/// Constant representing the common key for event or action
		/// </summary>
		public const string EventKey = "Event";

		// internal storage of properties
		private readonly Dictionary<string, object> _properties = new Dictionary<string, object>(13);

		/// <summary>
		/// Gets or sets the account for the logging properties bag.
		/// </summary>
		public object Account
		{
			get { return _properties[AccountKey]; }
			set { _properties[AccountKey] = value; }
		}

		/// <summary>
		/// Gets or sets the IP for the logging properties bag.
		/// </summary>
		public object Ip
		{
			get { return _properties[IpKey]; }
			set { _properties[IpKey] = value; }
		}

		/// <summary>
		/// Gets or sets the user for the logging properties bag.
		/// </summary>
		public object User
		{
			get { return _properties[UserKey]; }
			set { _properties[UserKey] = value; }
		}

		/// <summary>
		/// Gets or sets the account for the logging properties bag.
		/// </summary>
		public object Event
		{
			get { return _properties[EventKey]; }
			set { _properties[EventKey] = value; }
		}

		/// <summary>
		/// Gets or sets an arbitrary key in the logging properties.
		/// </summary>
		/// <param name="key">The key to get or store the property as.</param>
		/// <returns>The property value for the key.</returns>
		public object this[string key]
		{
			get { return _properties[key]; }
			set { _properties[key] = value; }
		}

		/// <summary>
		/// Adds a value into the logging properties with the given key.
		/// </summary>
		/// <param name="key">The key of the value.</param>
		/// <param name="value">The value associated with the key.</param>
		public void Add(string key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			_properties[key] = value;
		}

		/// <summary>
		/// Adds a value into the logging properties with the given key.
		/// </summary>
		/// <param name="pair">The key value pair.</param>
		public void Add(KeyValuePair<string, object> pair)
		{
			Add(pair.Key, pair.Value);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An  enumerator that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An  enumerator that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Converts the logging properties collection to a string for logging.
		/// </summary>
		/// <returns>A string representation of the properties list</returns>
		public override string ToString()
		{
			bool first = true;
			var builder = new StringBuilder(100);

			builder.Append('{');

			// could use string.Join(), but on timing this is more efficient, and for logging want to keep light.)
			foreach (var pair in _properties)
			{
				// don't put in first '|' for first item
				if (first)
				{
					first = false;
				}
				else
				{
					builder.Append('|');
				}

				builder.Append(Scrub(pair.Key));
				builder.Append('=');

				if (pair.Value == null)
				{
					builder.Append("null");
				}
				else
				{
					builder.Append('"');
					builder.Append(Scrub(pair.Value.ToString()));
					builder.Append('"');
				}
			}

			builder.Append('}');

			return builder.ToString();
		}

		/// <summary>
		/// Scrubs the string of characters we don't want since they're special for our purposes.
		/// </summary>
		/// <param name="text">The text to scrub.</param>
		/// <returns>The scrubbed text.</returns>
		private static string Scrub(string text)
		{
			return text.Replace('|', '/')
				.Replace('=', '-')
				.Replace('"', '\'')
				.Replace('{', '[')
				.Replace('}', ']');
		}
	}
}
