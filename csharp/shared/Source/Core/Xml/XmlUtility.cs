using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SharedAssemblies.Core.Xml
{
    /// <summary>
    /// A collection of XML Utility classes
    /// </summary>
    public static class XmlUtility
    {
        /// <summary>
        /// Pretty prints an XML string with indentation
        /// </summary>
        /// <param name="xml">Raw xml string</param>
        /// <returns>Pretty printed XML string</returns>
        public static string PrettyPrintXml(string xml)
        {
            using (var memStream = new MemoryStream())
            using (var xmlWriter = new XmlTextWriter(memStream, Encoding.Unicode))
            {
                // see if we need to trim out the <?xml ... ?> declaration
                int declarationIndex = xml.IndexOf("?>");

                string trimmedXml = (declarationIndex > 0)
                                        ? xml.Substring(declarationIndex + 2)
                                        : xml;

                var xmlDoc = new XmlDocument();

                // Load the XmlDocument with the XML.
                xmlDoc.LoadXml(trimmedXml);

                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 4;

                // Write the XML into a formatting XmlTextWriter
                xmlDoc.WriteContentTo(xmlWriter);
                xmlWriter.Flush();
                memStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                memStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                using (var streamReader = new StreamReader(memStream))
                {
                    // Extract the text from the StreamReader.
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Returns a web-safe version of the xml string so not interpretted as
        /// cross-side-scripting attack.
        /// </summary>
        /// <param name="xml">Xml string</param>
        /// <returns>Xml string encoded for web display</returns>
        public static string WebSafeXml(string xml)
        {
            var builder = new StringBuilder(xml);
            return builder.Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace(" ", "&nbsp;")
                .Replace("\r\n", "<BR>")
                .Replace("\n", "<BR>")
                .ToString();
        }

        /// <summary>
        /// Writes an xml file from a type
        /// </summary>
        /// <param name="fileName">File to serialize object to</param>
        /// <param name="valueToSerialize">Object to write</param>
        public static void XmlFileFromType(string fileName, object valueToSerialize)
        {
            var outFile = new FileInfo(fileName);

            using (var writer = outFile.CreateText())
            {
                writer.Write(PrettyPrintXmlFromType(valueToSerialize));
            }
        }

        /// <summary>
        /// Reads a type from an xml file
        /// </summary>
        /// <typeparam name="T">The type to read from the xml file.</typeparam>
        /// <param name="fileName">Name of file object serialized in</param>
        /// <returns>The populated type generated from the deserialized xml</returns>
        public static T TypeFromXmlFile<T>(string fileName)
        {
            var inFile = new FileInfo(fileName);

            using (var reader = inFile.OpenText())
            {
                return TypeFromXml<T>(reader.ReadToEnd());
            }
        }
        
        /// <summary>
        /// Serialize an object into an xml string
        /// </summary>
        /// <param name="input">The object to serialize.</param>
        /// <returns>The xml result in a string.</returns>
        public static string XmlFromType(object input)
        {
            var xs = new XmlSerializer(input.GetType());

            // use new UTF8Encoding here, not Encoding.UTF8.  The later includes
            // the BOM which screws up subsequent reads, the former does not.
            using (var memoryStream = new MemoryStream())
            using (var xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding()))
            {
                xs.Serialize(xmlTextWriter, input);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }


		/// <summary>
		/// Serializes an object into an XmlDataDocument.
		/// </summary>
		/// <param name="input">Object to serialize.</param>
		/// <returns>An XmlDataDocument containing the XML result.</returns>
		/// <remarks>This will be deprecated as an error in version 3.0</remarks>
		[Obsolete("System.Xml.XmlDataDocument is obsolete.  This will be removed in future version.", false)]
		public static XmlDataDocument XmlDataDocumentFromType(object input)
		{
			var document = new XmlDataDocument();

			LoadXmlDocumentFromType(input, document);
			return document;
		}


		/// <summary>
		/// Serializes an object into an XmlDocument.
		/// </summary>
		/// <param name="input">Object to serialize.</param>
		/// <returns>An XmlDocument containing the XML result.</returns>
		/// <remarks>Version 1.2</remarks>
		public static XmlDocument XmlDocumentFromType(object input)
		{
			var document = new XmlDocument();

			LoadXmlDocumentFromType(input, document);
			return document;
		}


		/// <summary>
        /// Deserialize an object from an xml string
        /// </summary>
        /// <typeparam name="T">The type to read from the XML.</typeparam>
        /// <param name="xml">XML of objet</param>
        /// <returns>The result</returns>
        public static T TypeFromXml<T>(string xml)
        {
            var xs = new XmlSerializer(typeof(T));

            using (var stringReader = new StringReader(xml))
            using (var xmlReader   = new XmlTextReader(stringReader))
            {
                return (T)xs.Deserialize(xmlReader);
            }
        }


		/// <summary>
		/// Deserialize an object from a stream.
		/// Note: The caller is responsible for closing their own stream.
		/// </summary>
		/// <typeparam name="T">The type to read from the XML.</typeparam>
		/// <param name="stream">Stream containing the object XML.</param>
		/// <returns>The resulting object of type T.</returns>
		/// <remarks>Version 1.2</remarks>
		public static T TypeFromXml<T>(Stream stream)
		{
			var xs = new XmlSerializer(typeof(T));

			return (T)xs.Deserialize(stream);
		}


		/// <summary>
		/// Deserialize an object from a text reader.
		/// Note: The caller is responsible for closing their own reader.
		/// </summary>
		/// <typeparam name="T">The type to read from the XML.</typeparam>
		/// <param name="reader">A reader containing the object XML.</param>
		/// <returns>The resulting object of type T.</returns>
		/// <remarks>Version 1.2</remarks>
		public static T TypeFromXml<T>(TextReader reader)
		{
			var xs = new XmlSerializer(typeof(T));

			return (T)xs.Deserialize(reader);
		}


		/// <summary>
		/// Deserialize an object from an xml text reader.  
		/// Note: The caller is responsible for closing their own reader.
		/// </summary>
		/// <typeparam name="T">The type to read from the XML.</typeparam>
		/// <param name="reader">A reader containing the object XML.</param>
		/// <returns>The resulting object of type T.</returns>
		/// <remarks>Version 1.2</remarks>
		public static T TypeFromXml<T>(XmlTextReader reader)
		{
			var xs = new XmlSerializer(typeof(T));

			return (T)xs.Deserialize(reader);
		}


		/// <summary>
        /// Serialize a result into an xml string
        /// </summary>
        /// <param name="valueToSerialize">Result object</param>
        /// <returns>The result</returns>
        public static string PrettyPrintXmlFromType(object valueToSerialize)
        {
            return PrettyPrintXml(XmlFromType(valueToSerialize));
        }


		/// <summary>
		/// Helper method to load an xml document from an object.
		/// </summary>
		/// <param name="input">The object to convert to a document.</param>
		/// <param name="document">The document to load.</param>
		private static void LoadXmlDocumentFromType(object input, XmlDocument document)
		{
			var xs = new XmlSerializer(input.GetType());

			// use new UTF8Encoding here, not Encoding.UTF8.  The later includes
			// the BOM which screws up subsequent reads, the former does not.
			using (var memoryStream = new MemoryStream())
			using (var xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding()))
			{
				xs.Serialize(xmlTextWriter, input);

				// reset position and now read in the stream
				memoryStream.Position = 0;
				document.Load(memoryStream);
			}
		}
	}
}