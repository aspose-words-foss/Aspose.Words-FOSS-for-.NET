// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/01/2014 by Alexey Butalov

using System;
using System.IO;
using System.Runtime.InteropServices;
using Aspose.JavaAttributes;

namespace Aspose.Xml
{
    [ComVisible(false)]
    public interface IAnyXmlBuilder
    {
        /// <summary>
        /// Starts the XML document and the root element.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void StartDocument(string rootElementName);

        /// <summary>
        /// Ends the root element and the XML document itself.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void EndDocument();

        /// <summary>
        /// Writes start of the element with the specified name.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void StartElement(string elementName);

        /// <summary>
        /// Writes end of the current element.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void EndElement();

        /// <summary>
        /// Writes end of the current element.
        /// In debug mode checks the closing element name.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void EndElement(string elementName);

        /// <summary>
        /// Writes the full end tag of the current element.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void FullEndElement();

        /// <summary>
        /// Writes the full end tag of the current element.
        /// In debug mode checks the closing element name.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void FullEndElement(string elementName);

        /// <summary>
        /// Writes text as is without entitizing or filtering invalid chars.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteRaw(string text);

        /// <summary>
        /// Writes text as is without entitizing or filtering invalid chars. 
        /// Optionally indents lines of the text to match parent element indentation.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteRaw(string text, bool indentLinesIfFormattingEnabled);

        /// <summary>
        /// Writes empty element with the specified name.
        /// </summary>
        void WriteEmptyElement(string elementName);

        /// <summary>
        /// Writes element with the specified name and value.
        /// </summary>
        void WriteElement(string elementName, string value);

        /// <summary>
        /// Writes element with the specified name and a string value if the string is not null or empty.
        /// </summary>
        void WriteOptionalElement(string elementName, string value);

        /// <summary>
        /// Writes element with the specified name and a DateTime value.
        /// DateTime is validated (year value should be greater than 1).
        /// If DateTime is invalid the element is not written.
        /// </summary>
        void WriteOptionalElement(string elementName, DateTime value);

        /// <summary>
        /// Writes element with the specified name and an integer value.
        /// </summary>
        void WriteElement(string elementName, int value);

        /// <summary>
        /// Writes the given text content. Filters out invalid characters.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteString(string value);

        /// <summary>
        /// Writes an attribute string if value is not default. Always. Filters out invalid characters.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteAttributeStringIfNotDefault(string name, string value, string defaultValue);

        /// <summary>
        /// Writes an attribute string. Always. Filters out invalid characters.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteAttributeString(string name, string value);

        /// <summary>
        /// Writes MIME-compliant Base64 data. Compliance means that Base64 string is broken into lines, containing
        /// exactly 76 characters each. Only the last line of base64 data can contain less than 76 characters.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteBase64(Stream stream);

        /// <summary>
        /// Writes MIME-compliant Base64 data. Compliance means that Base64 string is broken into lines, containing
        /// exactly 76 characters each. Only the last line of base64 data can contain less than 76 characters.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        void WriteBase64(byte[] buffer, int index, int count);
    }
}
