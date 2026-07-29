// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading header/footer parts.
    /// </summary>
    internal static class DocxHeaderFooterReader
    {
        /// <summary>
        /// Reads header/footer document part.
        /// </summary>
        /// <param name="reader">DocxReader to read from. Should be positioned to
        /// headerReference or footerReference element start.</param>
        internal static void Read(NrxDocumentReaderBase reader)
        {
            Debug.Assert(reader is DocxDocumentReaderBase);
            DocxDocumentReaderBase docReader = (DocxDocumentReaderBase) reader;

            NrxXmlReader xmlReader = docReader.XmlReader;

            bool isHeader = (xmlReader.LocalName == "headerReference");

            // Read attributes.
            string type = null;
            string id = null;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "type": // w:type
                        type = xmlReader.Value;
                        break;
                    case "id": // r:id
                        id = xmlReader.Value;
                        break;
                    default:
                        // Ignore.
                        break;
                }
            }

            // Create the header object.
            HeaderFooterType headerFooterType = NrxSectEnum.XmlToHeaderFooterType(type, isHeader);
            HeaderFooter headerFooter = new HeaderFooter(docReader.Document, headerFooterType);

            // Read the header part.
            DocxXmlReader partReader = docReader.SwitchToPartReaderByRelId(id);
            if (partReader != null)
            {
                docReader.IsInHeaderFooter = true;
                DocxReaderFactory.StoryReader.Read(docReader, headerFooter);
                headerFooter.IsLinkedToPrevious = false;
                docReader.IsInHeaderFooter = false;
                docReader.RestorePartReader();

                // WORDSNET-28848 Look for parent section ignoring current container node type.
                Section section = (Section)docReader.CurContainer.GetAncestor(NodeType.Section);
                docReader.InsertHeaderFooter(section, headerFooter);
            }
            else
            {
                docReader.Warn(WarningType.UnexpectedContent, WarningSource.Docx, WarningStrings.HeaderFooterPartMissing);
            }
        }
    }
}
