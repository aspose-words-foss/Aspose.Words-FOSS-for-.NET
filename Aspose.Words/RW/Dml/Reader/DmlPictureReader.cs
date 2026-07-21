// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/06/2011 by Alexey Titov

using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Reads "pic" tag.
    /// </summary>
    internal class DmlPictureReader : DmlReaderBase
    {
        private DmlPictureReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        internal static Shape Read(DocxDocumentReaderBase reader)
        {
            DmlPictureReader pictureBuilder = new DmlPictureReader(reader);
            return pictureBuilder.ReadCore();
        }

        [JavaThrows(false)]
        private Shape ReadCore()
        {
            try
            {
                if (XmlReader.LocalName != "pic")
                    return null;
                ReadPicture();
                return mPicture;
            }
            catch
            {
                // We are failed to read shape from xml.
                // In the case null will be returned to a client.
                return null;
            }
        }

        /// <summary>
        /// Reads the picture.
        /// </summary>
        private void ReadPicture()
        {
            mPicture = new Shape(mDocumentReader.Document, ShapeMarkupLanguage.Dml);
            mDocumentReader.AddAndPushContainer(mPicture);
            DmlPicture dmlPicture = new DmlPicture();
            mPicture.DmlNode = dmlPicture;
            string shapeTag = XmlReader.LocalName;
            while (XmlReader.ReadChild(shapeTag))
            {
                switch (XmlReader.LocalName)
                {
                    case "blipFill":
                        dmlPicture.BlipFill = (DmlBlipFill)DmlFillReader.Read(mDocumentReader);
                        break;
                    case "spPr":
                        DmlNodePropertiesReader.ReadShapeProperties(mDocumentReader, dmlPicture);
                        break;
                    case "style":
                        dmlPicture.Style = DmlShapeStyleReader.Read(XmlReader, mDocumentReader.ComplianceInfo);
                        break;
                    case "nvPicPr":
                        dmlPicture.NonVisualPr = new DmlNvPrPicture();
                        DmlNodePropertiesReader.ReadNonVisualProperties(dmlPicture.NonVisualPr, mDocumentReader);
                        break;
                    case "extLst":
                        dmlPicture.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    case "AlternateContent":
                        ReadAlternateContent(dmlPicture);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            mDocumentReader.PopContainer(NodeType.Shape);
        }

        /// <summary>
        /// An alternate content block allows for an alternative representation of information.
        /// </summary>
        private void ReadAlternateContent(DmlShapeBase shape)
        {
            while (XmlReader.ReadChild("AlternateContent"))
            {
                switch (XmlReader.LocalName)
                {
                    case "Choice":
                        // andrnosk: - Ignore it for now.
                        // The Requires attribute specifies a set of space-delimited namespaces
                        // that must be understood in order to select that choice. If there is a match
                        // between required namespaces and what the consumer understands, the appropriate Choice is
                        // returned. If there are multiple possible matches, only the first match is returned.
                        WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    case "Fallback":
                        // andrnosk: WORDSNET-5024 (part 2)
                        // An optional Fallback element can be used, and is utilized as a default when no match with Choice occurs.
                        // Always take Fallback for rendering, because it is appropriate alternative representation of Choice.
                        ReadFallback(shape);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// An optional Fallback element can be used, and is utilized as a default when no match with Choice occurs.
        /// </summary>
        private void ReadFallback(DmlShapeBase shape)
        {
            while (XmlReader.ReadChild("Fallback"))
            {
                switch (XmlReader.LocalName)
                {
                    case "blipFill":
                        ((DmlPicture)shape).BlipFill = (DmlBlipFill)DmlFillReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private Shape mPicture;
        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
