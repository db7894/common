using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharedAssemblies.Core.Containers
{
	/// <summary>
	/// Dictionary class that knows how to serialize itself since the
	/// generic Dictionary class is not serializable by default
	/// </summary>
	[XmlRoot("Dictionary")]
	[Serializable]
	public class SerializableDictionary : Dictionary<string, string>,
		  IXmlSerializable
	{
		/// <summary>Constant for the entry xml tag.</summary>
		private const string _entryTag = "Entry";

		/// <summary>Constant for the key xml tag.</summary>
		private const string _keyTag = "Key";

		/// <summary>Constant for the value xml tag.</summary>
		private const string _valueTag = "Value";

		/// <summary>
		/// Initializes an instance that creates an empty serializable dictionary
		/// </summary>
		public SerializableDictionary()
			: base()
		{
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary populated with another
		/// dictionary's key-value pairs.
		/// </summary>
		/// <param name="dictionary">The dictionary to use to populate this dictionary.</param>
		public SerializableDictionary(IDictionary<string, string> dictionary)
			: base(dictionary)
		{
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary populated with another
		/// dictionary's key-value pairs and an equality comparer.
		/// </summary>
		/// <param name="comparer">The comparer to use to determine equality.</param>
		/// <param name="dictionary">The dictionary to use to populate this dictionary.</param>
		public SerializableDictionary(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer)
			: base(dictionary, comparer)
		{
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary given an equality comparer.
		/// </summary>
		/// <param name="comparer">The comparer to use to determine equality.</param>
		public SerializableDictionary(IEqualityComparer<string> comparer)
			: base(comparer)
		{
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary with a given capacity.
		/// </summary>
		/// <param name="capacity">The initial capacity of the dictionary.</param>
		public SerializableDictionary(int capacity)
			: base(capacity)
		{
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary with a given capacity.
		/// </summary>
		/// <param name="capacity">The initial capacity of the dictionary.</param>
		/// <param name="comparer">The comparer to use to determine equality.</param>
		public SerializableDictionary(int capacity, IEqualityComparer<string> comparer)
			: base(capacity, comparer)
		{
		}
	
		/// <summary>
		/// Return the schema for the serializable type
		/// </summary>
		/// <returns>The XSD schema representing this XML</returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Read the xml from the serialization stream
		/// </summary>
		/// <param name="reader">The xml reader to read the xml from.</param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();

			// if was not empty element, read each key
			if (!wasEmpty)
			{
				// loop through all elements
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					var isSelfClosing = reader.IsEmptyElement;

					// read the key
					var key = reader.GetAttribute(_keyTag);
					var value = reader.GetAttribute(_valueTag);

					// add to our dictionary, overwrite existing key
					// if it's a duplicate, otherwise inserts.
					if (key != null)
					{
						this[key] = value;
					}

					// end the entry
					reader.Read();

					if (!isSelfClosing)
					{
						reader.ReadEndElement();
					}
				}

				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Write the XML to the serialization stream
		/// </summary>
		/// <param name="writer">The XML Writer to write the XML too.</param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			// write each entry
			foreach (var key in Keys)
			{
				// write the tag for the entry 
				writer.WriteStartElement(_entryTag);

				// write the key attribute
				writer.WriteAttributeString(_keyTag, key);

				// write the value attribute
				writer.WriteAttributeString(_valueTag, this[key]);

				// end the entry tag
				writer.WriteEndElement();
			}
		}
	}
}