// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2008 by Roman Korchagin

using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class DocxSmartTagReader
    {
        /// <summary>
        /// Reads a 'w:smartTag' element from the specified reader.
        /// </summary>
        /// <param name="reader">DocxReader to read from. Should be positioned to element start.</param>
        internal void Read(NrxDocumentReaderBase reader)
        {
            SmartTag smartTag = new SmartTag(reader.Document);
            reader.AddAndPushContainer(smartTag);

            // Read attributes.
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "element":
                        smartTag.Element = xmlReader.Value;
                        break;
                    case "uri":
                        smartTag.Uri = xmlReader.Value;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // Read elements.
            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("smartTag"))
            {
                switch (xmlReader.LocalName)
                {
                    case "smartTagPr":
                        ReadSmartTagPr(xmlReader, smartTag);
                        break;
                    default:
                        mInlineReader.ReadChild(reader);
                        break;
                }
            }

            reader.PopContainer(NodeType.SmartTag);
        }

        /// <summary>
        /// Reads a w:smartTagPr collection.
        /// </summary>
        private static void ReadSmartTagPr(AnyXmlReader partReader, SmartTag smartTag)
        {
            while (partReader.ReadChild("smartTagPr"))
            {
                switch (partReader.LocalName)
                {
                    case "attr":
                        ReadAttr(partReader, smartTag);
                        break;
                    default:
                        partReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a w:attr element of w:smartTagPr collection and adds to the smart tag properties.
        /// </summary>
        private static void ReadAttr(AnyXmlReader partReader, SmartTag smartTag)
        {
            string name = "";
            string uri = "";
            string val = "";

            while (partReader.MoveToNextAttribute())
            {
                switch (partReader.LocalName)
                {
                    case "name":
                        name = partReader.Value;
                        break;
                    case "uri":
                        uri = partReader.Value;
                        break;
                    case "val":
                        val = partReader.Value;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            if (StringUtil.HasChars(name))
                smartTag.Properties.AddSafe(new CustomXmlProperty(name, uri, val));
        }

        internal void SetInlineReader(DocxInlineReader inlineReader)
        {
            Debug.Assert(inlineReader != null);
            mInlineReader = inlineReader;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DocxInlineReader mInlineReader;
    }
}
