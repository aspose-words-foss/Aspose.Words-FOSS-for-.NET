// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/14/2014 by Alexey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class reads 'graphics' element of DrawingML and returns the corresponding <see cref="DmlNode"/>
    /// </summary>
    internal class DmlGraphicsReader
    {
        private DmlGraphicsReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        internal static ShapeBase Read(DocxDocumentReaderBase reader)
        {
            DmlGraphicsReader graphicsReader = new DmlGraphicsReader(reader);
            return graphicsReader.ReadGraphic();
        }

        /// <summary>
        /// Reads 5.1.2.1.16 graphic (Graphic Object).
        /// </summary>
        private ShapeBase ReadGraphic()
        {
            // Has elements only.
            while (XmlReader.ReadChild("graphic"))
            {
                switch (XmlReader.LocalName)
                {
                    case "graphicData":
                        return ReadGraphicData();
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads 5.1.2.1.17 graphicData (Graphic Object Data).
        /// This element specifies the reference to a graphic object within the document.
        /// This graphic object is provided entirely by the document authors who choose to
        /// persist this data within the document.
        /// </summary>
        private ShapeBase ReadGraphicData()
        {
            // Read attributes.
            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "uri":
                        // This attribute tells the consumer how to interpret the graphicData.
                        // Office supports a set of specific uri values:
                        // http://schemas.openxmlformats.org/drawingml/2006/chart
                        // http://schemas.openxmlformats.org/drawingml/2006/compatibility
                        // http://schemas.openxmlformats.org/drawingml/2006/diagram
                        // http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas
                        // http://schemas.openxmlformats.org/drawingml/2006/picture
                        // http://schemas.openxmlformats.org/drawingml/2006/table
                        // http://schemas.openxmlformats.org/drawingml/2006/ole
                        // http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas
                        // http://schemas.microsoft.com/office/word/2010/wordprocessingGroup
                        // http://schemas.microsoft.com/office/word/2010/wordprocessingShape
                        break;
                    default:
                        XmlReader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, XmlReader.LocalName));
                        break;
                }
            }

            // Read elements.
            // The spec says vaguely that any element from any namespace can occur here. We only support some of course.
            while (XmlReader.ReadChild("graphicData"))
            {
                switch (XmlReader.LocalName)
                {
                    case "lockedCanvas":
                    case "wpc": // wpc seems to be analog of locked canvas in Word 2013, the only difference it is editable.
                        return DmlCompositeNodeReader.ReadLockedCanvas(mDocumentReader);
                    case "pic":
                        return DmlPictureReader.Read(mDocumentReader);
                    case "chart":
                        return DmlChartReader.Read(mDocumentReader);
                    case "relIds":
                        return DmlDiagramReader.Read(mDocumentReader);
                    case "wsp":   //ISO 29500 extension Wordprocessing shape.
                    case "sp":    // I never saw sp as direct child of graphicData, but if wsp can be its child sp can too.
                        return DmlShapeReader.Read(mDocumentReader);
                    case "wgp":   // ISO 29500 extension Wordprocessing group shape.
                    case "grpSp": // I never saw grpSp as direct child of graphicData, but if wpg can be its child grpSp can too.
                        return DmlCompositeNodeReader.ReadGroup(mDocumentReader);
                    case "contentPart": // ISO 29500 17.3.3.2 contentPart (Content Part)
                        return DmlContentPartReader.Read(mDocumentReader);
                    case "AlternateContent": // WORDSNET-11918 Read Choose from this AlternateContent.
                        return ReadAlternateContent();
                    default:
                        XmlReader.IgnoreElement();
                        break;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads M.7.3.4.2 AlternateContent (Alternate Content block).
        /// An alternate content block allows for an alternative representation of information.
        /// </summary>
        private Shape ReadAlternateContent()
        {
            Shape shape = null;
            while (XmlReader.ReadChild("AlternateContent"))
            {
                switch (XmlReader.LocalName)
                {
                    case "Choice":
                        shape = ReadAlternateContentCore();
                        break;
                    case "Fallback":
                        XmlReader.IgnoreElement();
                        break;
                    default:
                        Debug.Fail(XmlReader.LocalName);
                        XmlReader.IgnoreElement();
                        break;
                }
            }

            return shape;
        }

        /// <summary>
        /// Reads M.7.3.4.2.1 AlternateContent Syntax
        /// </summary>
        private Shape ReadAlternateContentCore()
        {
            string tagName = XmlReader.LocalName;

            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "contentPart":
                        return DmlContentPartReader.Read(mDocumentReader);
                    default:
                        Debug.Fail(XmlReader.LocalName);
                        XmlReader.IgnoreElement();
                        break;
                }
            }

            return null;
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
