// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/01/2017 by Alexey Butalov

using System.Xml;
using Aspose.JavaAttributes;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Base class, helps to read a hyperlink DOCX or WML element.
    /// </summary>
    internal abstract class NrxHyperlinkReaderBase
    {
        /// <summary>
        /// Reads a hyperlink element.
        /// RK Hyperlink in DOCX is very different from hlink in WordML.
        /// </summary>
        [JavaThrows(true)]
        internal abstract void Read(NrxDocumentReaderBase reader);

        internal void SetInlineReader(NrxInlineReaderBase reader)
        {
            Debug.Assert(reader != null);
            inlineReader = reader;
        }

        protected void ReadChildren(NrxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();

            string tagName = xmlReader.LocalName;
            int parentDepth = xmlReader.UnderlyingReader.Depth;
            XmlTextReader underlyingReader = xmlReader.UnderlyingReader;

            while (true)
            {
                bool hasMoreChildren = xmlReader.ReadChild(tagName);

                // WORDSNET-22087 Do not stop reading children when occurs child with same name as parent.
                // Hyperlink may contain nested "hlink"/"hyperlink" elements for WML/DOCX format.
                if (!hasMoreChildren)
                {
                    if (underlyingReader.Depth == parentDepth)
                        break;

                    if (!MoveToNextElement(underlyingReader, tagName, parentDepth))
                        break;
                }

                inlineReader.ReadChild(reader);
            }
        }

        /// <summary>
        /// Moves to the start of the next element in the stream. 
        /// </summary>
        /// <param name="reader">Xml reader.</param>
        /// <param name="parentName">Tag name of the parent element.</param>
        /// <param name="parentDepth">Parent depth.</param>
        /// <returns>Returns true if the element was read. Returns false if reached the end of the specified element or the end of file.</returns>
        private bool MoveToNextElement(XmlTextReader reader, string parentName, int parentDepth)
        {
            reader.MoveToElement();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        // Found an element, return it to the caller.
                        return true; 
                    }
                    case XmlNodeType.EndElement:
                    {
                        // If reached the end of the parent element, signal the caller to stop.
                        if ((reader.LocalName == parentName) && (reader.Depth == parentDepth))
                            return false;
                        break;
                    }
                    default:
                        break;
                }
            }

            // End of file.
            return false;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        protected NrxInlineReaderBase inlineReader;
    }
}
