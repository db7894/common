=========================================================================
Core Xml --- XML Serialization Utilities
=========================================================================
:Assembly: SharedAssemblies.Core.dll
:Namespace: SharedAssemblies.Core.Xml
:Author: Jim Hare (`jhare@bashwork.com <mailto:jhare@bashwork.com>`_)
:Date: |today|

.. highlight:: csharp

.. index:: 
    pair: XML; Serialization

.. module:: SharedAssemblies.Core.Xml
   :platform: Windows, .Net
   :synopsis: Xml - XML Serialization Utilities
   
Introduction
------------------------------------------------------------

The *Xml* namespace of the *Core* contains helper and utility classes to make XML serialization easier without having to
worry about creating the serializers, pretty printing, etc.

Usage
------------------------------------------------------------

Since each of the classes in this assembly are generic utility classes, they each 
have their own unique usages that are described in the class sections below.

Classes
--------------------------------------------------------------

.. class:: XmlUtility

    The **XmlUtility** is the utility class that contains the helper methods for serializing objects to a string or file and back.
    While the *XmlUtility* class and its methods can be used directly, it is also possible to use the extension methods provided
    in the *Xml* namespace of the *Core* that allow XML serialization as an extension method off of any object.  
    
    Even though *ToXml()* is provided as an extension method, *FromXml()* is not.  It seems natural to call 
    **value.ToXml()**, where the value is converted to a string and returned.  However, as a design decision 
    it was viewed that **value.FromXml(xmlString)** would seem awkward and unnatural since the *FromXml()*
    essentially creates the object and switches the reference.  Thus, it was omitted and viewed that *XmlUtility*
    should be called directly for de-serialization.
    
        .. method:: static string PrettyPrintXml(string xml)
            
            :param xml: XML string to pretty print with indentation and whitespace.
            :returns: the XML string after pretty-printing.
            
            **PrettyPrintXml** is simply a static method to take the typical one-line serialized XML and convert
            it to a more readable format with whitespace and indention as appropriate.
            
        .. method:: static string WebSafeXml(string xml)
        
            :param xml: an XML string to convert to web-safe XML for display purposes.
            :returns: the XML string after converted to web-safe encoding.
            
            **WebSafeXml** is not very useful in the programmatic scheme of things, but helps when needing
            to display XML on a web page, it prevents the XML from being considered markup and allows it to be
            interpreted as literal text.
            
        .. method:: static T TypeFromXmlFile<T>(string fileName)
        
            :param T: The type to de-serialize from the XML contained in the file.
            :param fileName: the path to the file to de-serialize.
            :returns: the object created from the XML in the file.
            
            **TypeFromXmlFile** opens the file, reads the entire contents as a string, and uses an
            XmlSerializer to create an object from the XML and cast to type *T*.
            
        .. method:: static string XmlFromType(object input)
        
            :param object: the object to serialize to an XML string.
            :returns: the XML string that contains the serialized output.
            
            **XmlFromtype** converts the *input* object into an XML string.  
            
            .. note:: The **ToXml()** extension method in the *XmlExtensions* is a syntactical short-cut to this call.
            
        .. method:: static T TypeFromXml<T>(string xml)
        
            :param T: The type to de-serialize from the XML contained in the string.
            :param xml: The string containing the XML to de-serialize.
            :returns: the object de-serialized from the XML in the string.
            
            **TypeFromXml** reads the contents of the string and uses an XmlSerializer to create an object and
            cast to type *T*.

        .. method:: static string PrettyPrintXmlFromType(object input)
        
            :param object: the object to serialize to an pretty-printed XML string.
            :returns: the XML string that contains the pretty-printed, serialized output.
            
            **PrettyPrintXmlFromtype** converts the *input* object into a pretty-printed XML string.  This is really
            just a convenience method that calls *XmlFromType* and passes the result to *PrettyPrintXml* and returns
            the result.
            
For more information, see the `API Reference <../../../../../Api/index.html>`_.            
            
