// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/11/2012 by Alexey Noskov

using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class for reading 5.7.2.221 userShapes (User Shapes).
    /// </summary>
    internal class DmlChartShapesReader : DmlReaderBase
    {
        private DmlChartShapesReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        internal static void Read(DocxDocumentReaderBase reader, string userShapesId)
        {
            reader.SwitchToPartReaderByRelId(userShapesId);
            Read(reader);
            reader.RestorePartReader();
        }

        private static void Read(DocxDocumentReaderBase reader)
        {
            DmlChartShapesReader chartShapesReader = new DmlChartShapesReader(reader);

            while (reader.XmlReader.ReadChild("userShapes"))
                chartShapesReader.ReadUserShape();
        }

        private void ReadUserShape()
        {
            string elementName = XmlReader.LocalName;
            ShapeBase shape = null;

            PointF from = PointF.Empty;
            PointF to = PointF.Empty;

            while (XmlReader.ReadChild(elementName))
            {
                switch (XmlReader.LocalName)
                {
                    case "from":
                        from = ReadAnchorPoint();
                        break;
                    case "to":
                    case "ext":
                        to = ReadAnchorPoint();
                        break;
                    case "cxnSp":
                    case "sp":
                        shape = DmlShapeReader.Read(mDocumentReader);
                        break;
                    case "grpSp":
                        shape = DmlCompositeNodeReader.ReadGroup(mDocumentReader);
                        break;
                    case "pic":
                        shape = DmlPictureReader.Read(mDocumentReader);
                        break;
                    case "graphicFrame":
                        // Attempts to create document with graphic frame in user shapes
                        // failed while resolving WORDSNET-14542, so currently skip this item.
                        WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            if (shape != null)
            {
                // WORDSNET-14006 Mimic MS Word behavior - remove empty group shape.
                if (shape.IsGroup && !shape.HasChildNodes)
                {
                    shape.Remove();
                }
                else
                {
                    shape.From = from;
                    shape.To = to;
                    shape.AnchorType = GetAnchorType(elementName);
                }
            }
        }

        private PointF ReadAnchorPoint()
        {
            string elementName = XmlReader.LocalName;

            // WORDSNET-15569 cx cy might be attributes.
            float x = (float)XmlReader.ReadDoubleAttribute("cx", 0);
            float y = (float)XmlReader.ReadDoubleAttribute("cy", 0);

            while (XmlReader.ReadChild(elementName))
            {
                switch (XmlReader.LocalName)
                {
                    case "x":
                    case "cx":
                        x = (float)FormatterPal.ParseDouble(XmlReader.ReadString());
                        break;
                    case "y":
                    case "cy":
                        y = (float)FormatterPal.ParseDouble(XmlReader.ReadString());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return new PointF(x, y);
        }

        private static DmlChartUserShapeAnchorType GetAnchorType(string elementName)
        {
            switch (elementName)
            {
                case "absSizeAnchor":
                    return DmlChartUserShapeAnchorType.Absolute;
                case "relSizeAnchor":
                default:
                    return DmlChartUserShapeAnchorType.Relative;
            }
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
