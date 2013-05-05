using System;
using System.Xml;
using SharedAssemblies.Core.Xml;

namespace SharedAssemblies.Core.Extensions
{
    /// <summary>
    /// A collection of extensinos methods for easing translation of object to XML and back
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Gets the XML string representing an object
        /// </summary>
        /// <param name="value">The value to serialize to XML</param>
        /// <returns>The XML string representing the object</returns>
        public static string ToXml(this object value)
        {
            // call overload with default pretty printing to no
            return value.ToXml(false);
        }

        /// <summary>
        /// Gets the XML string representing an object
        /// </summary>
        /// <param name="value">The value to serialize to XML</param>
        /// <param name="shouldPrettyPrint">True if should use whitespacing and indentation</param>
        /// <returns>The XML string that represents the object</returns>
        public static string ToXml(this object value, bool shouldPrettyPrint)
        {
            return shouldPrettyPrint
                       ? XmlUtility.PrettyPrintXmlFromType(value)
                       : XmlUtility.XmlFromType(value);
        }

		/// <summary>
		/// Gets an XML document representing an object.
		/// </summary>
		/// <param name="value">The object to serialize to an XmlDocument.</param>
		/// <returns>The XmlDocument which contains the serialized object.</returns>
		/// <remarks>Version 1.2</remarks>
		public static XmlDocument ToXmlDocument(this object value)
		{
			return XmlUtility.XmlDocumentFromType(value);
		}

		/// <summary>
		/// Gets an XML document representing an object.
		/// </summary>
		/// <param name="value">The object to serialize to an XmlDataDocument.</param>
		/// <returns>The XmlDataDocument which contains the serialized object.</returns>
		/// <remarks>This will be deprecated as an error in version 3.0</remarks>
		/// <remarks>This is now completely deprecated as an error in version 2.0</remarks>
		[Obsolete("System.Xml.XmlDataDocument is obsolete.  This will be removed in future version.", false)]
		public static XmlDataDocument ToXmlDataDocument(this object value)
		{
			return XmlUtility.XmlDataDocumentFromType(value);
		}

		/// <summary>
        /// Saves the xml string representing an object to a filename specified
        /// </summary>
        /// <param name="value">The value to serialize to XML</param>
        /// <param name="fileName">The filename to store the XML string to</param>
        public static void ToXmlFile(this object value, string fileName)
        {
            XmlUtility.XmlFileFromType(fileName, value);
        }
    }
}