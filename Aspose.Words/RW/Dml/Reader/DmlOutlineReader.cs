// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2011 by Alexey Titov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Builds objects of DmlOutline class using xml.
    /// </summary>
    internal class DmlOutlineReader : DmlReaderBase
    {
        private DmlOutlineReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Builds an object of type DmlOutline using the provided reader.
        /// </summary>
        internal static DmlOutline Read(DocxDocumentReaderBase reader)
        {
            Debug.Assert(reader != null);

            DmlOutlineReader outlineReader = new DmlOutlineReader(reader);
            string tagName = reader.XmlReader.LocalName;

            if (tagName != "ln" && tagName != "textOutline" && tagName != "uLn" && tagName != "hiddenLine")
                return null;

            DmlOutline outline = new DmlOutline();
            outlineReader.ReadAttributes(outline);
            outlineReader.ReadChildTags(outline);
            return outline;
        }

        private void ReadChildTags(DmlOutline outline)
        {
            bool isText = (XmlReader.LocalName == "textOutline");

            string elementName = XmlReader.LocalName;
            while (XmlReader.ReadChild(elementName))
            {
                switch (XmlReader.LocalName)
                {
                    case "gradFill":
                    case "pattFill":
                    case "noFill":
                    case "solidFill":
                    {
                        outline.Fill = (isText)
                            ? DmlFillReader.ReadTextOutlineFill(mDocumentReader)
                            : DmlFillReader.Read(mDocumentReader);
                        break;
                    }
                    case "prstDash":
                        outline.Dash = ReadPresetDash();
                        break;
                    case "custDash":
                        outline.Dash = ReadCustomDash();
                        break;
                    case "headEnd":
                        outline.HeadLineEndStyle = new DmlHeadLineEndStyle();
                        ReadLineEndStyle(outline.HeadLineEndStyle);
                        break;
                    case "tailEnd":
                        outline.TailLineEndStyle = new DmlTailLineEndStyle();
                        ReadLineEndStyle(outline.TailLineEndStyle);
                        break;
                    case "bevel":
                        outline.LineJoinStyle = JoinStyle.Bevel;
                        break;
                    case "miter":
                        ReadMiter(outline);
                        break;
                    case "round":
                        outline.LineJoinStyle = JoinStyle.Round;
                        break;
                    case "extLst":
                        outline.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 5.1.10.43 miter (Miter Line Join).
        /// This element specifies that a line join shall be mitered.
        /// </summary>
        private void ReadMiter(DmlOutline outline)
        {
            outline.LineJoinStyle = JoinStyle.Miter;
            outline.LineMiterLimit =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("lim", ""), ComplianceInfo);
        }

        /// <summary>
        /// Reads 20.1.8.38 headEnd (Line Head/End Style)
        /// or 20.1.8.57 tailEnd (Tail line end style)
        /// </summary>
        private void ReadLineEndStyle(DmlLineEndStyle endStyle)
        {
            endStyle.Length = ReadLineEndLength("len");
            endStyle.Type = ReadLineEndType();
            endStyle.Width = ReadLineEndWidth("w");
        }

        /// <summary>
        /// Reads values of the ST_LineEndLength simple type
        /// </summary>
        private ArrowLength ReadLineEndLength(string attribureName)
        {
            string attrValue = XmlReader.ReadAttribute(attribureName, String.Empty);
            return DmlEnum.DmlToArrowLength(attrValue);
        }

        /// <summary>
        /// Reads values of the ST_LineEndWidth simple type
        /// </summary>
        private ArrowWidth ReadLineEndWidth(string attribureName)
        {
            string attrValue = XmlReader.ReadAttribute(attribureName, String.Empty);
            return DmlEnum.DmlToArrowWidth(attrValue);
        }

        /// <summary>
        /// Reads a value of the ST_LineEndType simple type.
        /// </summary>
        private ArrowType ReadLineEndType()
        {
            string attrValue = XmlReader.ReadAttribute("type", String.Empty);
            return DmlEnum.DmlToArrowType(attrValue);
        }

        /// <summary>
        /// Reads 20.1.8.48 prstDash tag.
        /// </summary>
        private DmlDash ReadPresetDash()
        {
            DmlPresetDash dash = new DmlPresetDash();
            string val = XmlReader.ReadAttribute("solid");
            dash.Preset = DmlEnum.DmlToPresetDashType(val);
            return dash;
        }

        /// <summary>
        /// Reads 20.1.8.21 custDash (Custom Dash) tag.
        /// </summary>
        private DmlDash ReadCustomDash()
        {
            DmlCustomDash dash = new DmlCustomDash();
            while (XmlReader.ReadChild("custDash"))
            {
                switch (XmlReader.LocalName)
                {
                    case "ds":
                        dash.DashStops.Add(ReadDashStop());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return dash;
        }

        /// <summary>
        /// Reads 20.1.8.22 ds (Dash Stop) tag.
        /// </summary>
        private DmlDashStop ReadDashStop()
        {
            DmlDashStop dashStop = new DmlDashStop();
            dashStop.DashLength =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("d", ""), ComplianceInfo);
            dashStop.SpaceLength =
                DmlPercentageUtil.FromPercentOrDmlPercent(XmlReader.ReadAttribute("sp", ""), ComplianceInfo);
            return dashStop;
        }


        private void ReadAttributes(DmlOutline outline)
        {
            ReadWidth(outline);
            ReadCompoundLineType(outline);
            ReadLineEndingCap(outline);
            ReadStrokeAlignment(outline);
        }

        /// <summary>
        /// Read values of the ST_PenAlignment simple type.
        /// </summary>
        private void ReadStrokeAlignment(DmlOutline outline)
        {
            string attrValue = XmlReader.ReadAttribute("algn", String.Empty);

            if (StringUtil.HasChars(attrValue))
                outline.StrokeAlignment = (attrValue == "in");
        }

        /// <summary>
        /// Read values of the ST_LineCap simple type.
        /// </summary>
        private void ReadLineEndingCap(DmlOutline outline)
        {
            string attrValue = XmlReader.ReadAttribute("cap", String.Empty);

            if (StringUtil.HasChars(attrValue))
                outline.EndCap = DmlEnum.DmlToLineEndingCapType(attrValue);
        }

        /// <summary>
        /// Read values of the ST_CompoundLine simple type.
        /// </summary>
        private void ReadCompoundLineType(DmlOutline outline)
        {
            string attrValue = XmlReader.ReadAttribute("cmpd", String.Empty);

            if (StringUtil.HasChars(attrValue))
                outline.CompoundLineType =  DmlEnum.DmlToCompoundLineType(attrValue);
        }

        private void ReadWidth(DmlOutline outline)
        {
            // If this attribute is omitted, then write nothing (then width from outline style will be used).
            double attrValue = XmlReader.ReadDoubleAttribute("w", double.NaN);

            if (!double.IsNaN(attrValue))
                outline.WidthInEmus = attrValue;
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private OoxmlComplianceInfo ComplianceInfo
        {
            get { return mDocumentReader.ComplianceInfo; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
