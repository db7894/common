using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharedAssemblies.Core.Containers
{
    /// <summary>
    /// Dictionary class that knows how to serialize itself since the
    /// generic Dictionary class is not serializable by default
    /// </summary>
    /// <typeparam name="TKey">The type of key to store values under.</typeparam>
    /// <typeparam name="TValue">The type of values to store.</typeparam>
    [XmlRoot("Dictionary")]
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>,
          IXmlSerializable
    {
        /// <summary>Constant for the entry xml tag.</summary>
        private const string _entryTag = "Entry";

        /// <summary>Constant for the key xml tag.</summary>
        private const string _keyTag = "Key";

		/// <summary>
		/// Initializes an instance that creates an empty serializable dictionary
		/// </summary>
		public SerializableDictionary() : base()
		{			
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary populated with another
		/// dictionary's key-value pairs.
		/// </summary>
		/// <param name="dictionary">The dictionary to use to populate this dictionary.</param>
		public SerializableDictionary(IDictionary<TKey,TValue> dictionary) : base(dictionary)
		{			
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary populated with another
		/// dictionary's key-value pairs and an equality comparer.
		/// </summary>
		/// <param name="comparer">The comparer to use to determine equality.</param>
		/// <param name="dictionary">The dictionary to use to populate this dictionary.</param>
		public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		/// <summary>
		/// Initializes an instance that creates a serializable dictionary given an equality comparer.
		/// </summary>
		/// <param name="comparer">The comparer to use to determine equality.</param>
		public SerializableDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
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
		public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{
		}

		/// <summary>Constant for the value xml tag.</summary>
        private const string _valueTag = "Value";

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
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            // if was not empty element, read each key
            if (!wasEmpty)
            {
                // loop through all elements
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    // read the entry
                    reader.ReadStartElement(_entryTag);

                    // read the key
                    reader.ReadStartElement(_keyTag);
                    TKey key = (TKey)keySerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    // read the value
                    reader.ReadStartElement(_valueTag);
                    TValue value = (TValue)valueSerializer.Deserialize(reader);
                    reader.ReadEndElement();

                    // add to our dictionary, overwrite existing key
                    // if it's a duplicate, otherwise inserts.
                    this[key] = value;

                    // end the entry
                    reader.ReadEndElement();
                    reader.MoveToContent();
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
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            // write each entry
            foreach (TKey key in Keys)
            {
                // write the tag for the entry 
                writer.WriteStartElement(_entryTag);

                // write the key tag
                writer.WriteStartElement(_keyTag);
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                // write the tag for the value
                writer.WriteStartElement(_valueTag);
                valueSerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                // end the entry tag
                writer.WriteEndElement();
            }
        }
    }
}