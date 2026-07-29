// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2016 by Andrey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Reader for DrawingML click/mouse-over hyperlink.
    /// </summary>
    internal class DmlHlinkReader : DmlReaderBase
    {
        private DmlHlinkReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
            mXmlReader = reader.XmlReader;
        }

        /// <summary>
        /// Reads click/mouse-over Hyperlink.
        /// 5.1.5.3.5 hlinkClick (Click Hyperlink)
        /// 5.1.2.1.23 hlinkHover (Hyperlink for Hover)
        /// 21.1.2.3.5 hlinkClick (Click Hyperlink)
        /// 21.1.2.3.6 hlinkMouseOver (Mouse-Over Hyperlink)
        /// Specifies the on-click/mouse-over hyperlink information to be applied to a run of text.
        /// When the hyperlink text is clicked the link will be fetched.
        /// </summary>
        internal static DmlHlink Read(DocxDocumentReaderBase reader)
        {
            DmlHlinkReader hlinkReader = new DmlHlinkReader(reader);
            return hlinkReader.ReadCore();
        }

        private DmlHlink ReadCore()
        {
            string tagName = mXmlReader.LocalName;

            DmlHlink hlink = new DmlHlink();

            // Read attributes.
            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case "id":
                        hlink.Id = mDocumentReader.GetRelationshipTarget(mXmlReader.Value);
                        break;
                    case "tgtFrame":
                        hlink.TargetFrame = mXmlReader.Value;
                        break;
                    case "tooltip":
                        hlink.Tooltip = mXmlReader.Value;
                        break;
                    case "action":
                    case "endSnd":
                    case "highlightClick":
                    case "history":
                    case "invalidUrl":
                    default:
                        WarnUnexpected(mXmlReader);
                        break;
                }
            }

            // Read elements.
            while (mXmlReader.ReadChild(tagName))
            {
                switch (mXmlReader.LocalName)
                {
                    case "extLst":
                        hlink.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    case "snd":
                    default:
                        WarnNotSupportedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return hlink;
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
        private readonly NrxXmlReader mXmlReader;
    }
}
