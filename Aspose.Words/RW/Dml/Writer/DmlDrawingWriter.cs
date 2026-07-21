// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/11/2008 by Roman Korchagin

using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Writes DrawingML shapes to DOCX.
    /// </summary>
    internal static class DmlDrawingWriter
    {
        internal static void WriteStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            if (dml.IsTopLevel)
                WriteDrawingStart(dml, writer);

            DmlGraphicWriter.WriteStart(dml, writer);
        }

        internal static void WriteEnd(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlGraphicWriter.WriteEnd(dml, writer);

            if (dml.IsTopLevel)
                WriteDrawingEnd(writer, dml);
        }

        private static void WriteDrawingStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            //<complexType name="CT_Drawing">
            //  <choice minOccurs="1" maxOccurs="unbounded">
            //      <element ref="wp:anchor" minOccurs="0"/>
            //      <element ref="wp:inline" minOccurs="0"/>
            //  </choice>
            //</complexType>
            NrxXmlBuilder builder = writer.CurrentBuilder;
            builder.StartElement("w:drawing");

            // SPEED We convert attribute collection into a flat property bag for speed to avoid
            // lookup of every defined attribute.
            ShapePrBag shapePr = new ShapePrBag();

            shapePr.Fill(dml.ShapePr);
            if (dml.DmlNode.DocPrExtensions != null)
                shapePr.DocPrExtensions = dml.DmlNode.DocPrExtensions;

            //<complexType name="CT_Anchor">
            //  <sequence>
            //      <element name="simplePos" type="a:CT_Point2D"/>
            //      <element name="positionH" type="CT_PosH"/>
            //      <element name="positionV" type="CT_PosV"/>
            //      <element name="extent" type="a:CT_PositiveSize2D"/>
            //      <element name="effectExtent" type="CT_EffectExtent" minOccurs="0"/>
            //      <group ref="EG_WrapType"/>
            //      <element name="docPr" type="a:CT_NonVisualDrawingProps" minOccurs="1" maxOccurs="1"/>
            //      <element name="cNvGraphicFramePr" type="a:CT_NonVisualGraphicFrameProperties" minOccurs="0" maxOccurs="1"/>
            //      <element ref="a:graphic" minOccurs="1" maxOccurs="1"/>
            //  </sequence>
            //  <attribute name="distT" type="ST_WrapDistance" use="optional"/>
            //  <attribute name="distB" type="ST_WrapDistance" use="optional"/>
            //  <attribute name="distL" type="ST_WrapDistance" use="optional"/>
            //  <attribute name="distR" type="ST_WrapDistance" use="optional"/>
            //  <attribute name="simplePos" type="xsd:boolean"/>
            //  <attribute name="relativeHeight" type="xsd:unsignedInt" use="required"/>
            //  <attribute name="behindDoc" type="xsd:boolean" use="required"/>
            //  <attribute name="locked" type="xsd:boolean" use="required"/>
            //  <attribute name="layoutInCell" type="xsd:boolean" use="required"/>
            //  <attribute name="hidden" type="xsd:boolean" use="optional"/>
            //  <attribute name="allowOverlap" type="xsd:boolean" use="required"/>
            //</complexType>
            builder.StartElement(dml.ShapePr.IsInline ? "wp:inline" : "wp:anchor");

            builder.WriteAttribute("distT", shapePr.DistanceTop);
            builder.WriteAttribute("distB", shapePr.DistanceBottom);
            builder.WriteAttribute("distL", shapePr.DistanceLeft);
            builder.WriteAttribute("distR", shapePr.DistanceRight);

            if (!dml.ShapePr.IsInline)
            {
                // The 'hidden' attribute is not read/written because MS Word ignores it, see the chapter 2.1.1345 of
                // [MS-OI29500].

                builder.WriteAttribute("simplePos", false);
                builder.WriteAttribute("relativeHeight", writer.SaveInfo.GetShapeZIndex(dml));
                builder.WriteAttribute("behindDoc", shapePr.BehindText, ShapePr.BehindTextDefault);
                builder.WriteAttribute("locked", shapePr.AnchorLocked, ShapePr.AnchorLockedDefault);
                builder.WriteAttribute("layoutInCell", shapePr.AllowInCell, ShapePr.AllowInCellDefault);
                builder.WriteAttribute("allowOverlap", shapePr.AllowOverlap, ShapePr.AllowOverlapDefault);

                DmlBasicsWriter.WriteXY("wp:simplePos", 0, 0, builder);
                WritePositionH(shapePr, writer);
                WritePositionV(shapePr, writer);
            }

            WriteExtent(shapePr, writer);
            WriteEffectExtent(shapePr, writer);
            WriteWrap(shapePr, writer);
            WriteDocPr(shapePr, writer);
            WriteCNvGraphicFramePr(shapePr, writer);
        }

        private static void WriteDrawingEnd(DocxDocumentWriterBase writer, ShapeBase dml)
        {
            WriteSizeRelH(dml.ShapePr, writer);
            WriteSizeRelV(dml.ShapePr, writer);

            writer.CurrentBuilder.EndElement();   // wp:inline or anchor
            writer.CurrentBuilder.EndElement("w:drawing");
        }

        private static void WriteSizeRelH(ShapePr shapePr, DocxDocumentWriterBase writer)
        {
            // DD: Todo this can be a good candidate for a complex attr: when two attrs are used together often we can treat them as a complex attr and 
            // incapsulate logic (WORDSNET-13478).
            if (shapePr.Contains(ShapeAttr.RelativeWidth) &&
                shapePr.Contains(ShapeAttr.WidthPercent)) // MS Word 2013 raises error if sizeRelH has no pctWidth
            {
                NrxXmlBuilder builder = writer.CurrentBuilder;
                builder.StartElement("wp14:sizeRelH");
                builder.WriteAttribute("relativeFrom", DmlEnum.RelativeWidthToDml(shapePr[ShapeAttr.RelativeWidth]));

                WritePercentage("wp14:pctWidth", (int)shapePr[ShapeAttr.WidthPercent], builder,
                    writer.Compliance == OoxmlComplianceCore.IsoStrict);

                builder.EndElement(); // wp14:sizeRelH
            }
        }

        /// <summary>
        /// Writes element of percentage type.
        /// </summary>
        private static void WritePercentage(string element, int vmlPercent, NrxXmlBuilder builder, bool isIsoStrict)
        {
            double asFraction = DmlPercentageUtil.FromDmlPercent(DmlPercentageUtil.VmlToDmlPercent(vmlPercent));
            builder.WriteElement(element, DmlPercentageUtil.ToPercentOrDmlPercent(asFraction, isIsoStrict));
        }

        private static void WriteSizeRelV(ShapePr shapePr, DocxDocumentWriterBase writer)
        {
            // DD: Todo this can be a good candidate for a complex attr: when two attrs are used together often we can treat them as a complex attr and 
            // incapsulate logic (WORDSNET-13478).
            if (shapePr.Contains(ShapeAttr.RelativeHeight) &&
                shapePr.Contains(ShapeAttr.HeightPercent)) // MS Word 2013 raises error if sizeRelV has no pctHeight
            {
                NrxXmlBuilder builder = writer.CurrentBuilder;
                builder.StartElement("wp14:sizeRelV");
                builder.WriteAttribute("relativeFrom", DmlEnum.RelativeHeightToDml(shapePr[ShapeAttr.RelativeHeight]));

                WritePercentage("wp14:pctHeight", (int)shapePr[ShapeAttr.HeightPercent], builder,
                    writer.Compliance == OoxmlComplianceCore.IsoStrict);

                builder.EndElement(); // wp14:sizeRelV
            }
        }
        
        private static void WritePositionH(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            // LeftPercent is not null, it means Choice was read, so we have to write AlternateContent.  
            if (shapePr.LeftPercent != null)
                WritePositionAlternateContent(shapePr, builder, true, isIsoStrict);
            else
                WritePositionHCore(shapePr, builder, true, isIsoStrict);
        }

        private static void WritePositionV(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            // TopPercent is not null, it means Choice was read, so we have to write AlternateContent.  
            if (shapePr.TopPercent != null)
                WritePositionAlternateContent(shapePr, builder, false, isIsoStrict);
            else
                WritePositionVCore(shapePr, builder, true, isIsoStrict);
        }

        /// <summary>
        /// Writes positionV/positionH elements covered in AlternateContent.
        /// </summary>
        private static void WritePositionAlternateContent(ShapePrBag shapePr, NrxXmlBuilder builder, bool isPositionH,
            bool isIsoStrict)
        {
            builder.StartElement("mc:AlternateContent");
            builder.WriteAttribute("xmlns:mc", 
                DocxNamespaces.GetNamespace(DocxNamespace.MarkupCompatibility, isIsoStrict));

            builder.StartElement("mc:Choice");
            builder.WriteAttribute("Requires", "c14");
            builder.WriteAttribute("xmlns:c14", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChart2007, isIsoStrict));
            if (isPositionH)
                WritePositionHCore(shapePr, builder, false, isIsoStrict);
            else
                WritePositionVCore(shapePr, builder, false, isIsoStrict);
            builder.EndElement("mc:Choice");

            builder.StartElement("mc:Fallback");
            if (isPositionH)
                WritePositionHCore(shapePr, builder, true, isIsoStrict);
            else
                WritePositionVCore(shapePr, builder, true, isIsoStrict);
            builder.EndElement("mc:Fallback");

            builder.EndElement("mc:AlternateContent");
        }

        private static void WritePositionVCore(ShapePrBag shapePr, NrxXmlBuilder builder, 
            bool isFallback, bool isIsoStrict)
        {
            builder.StartElement("wp:positionV");
            builder.WriteAttribute("relativeFrom", DmlEnum.RelativeVerticalPositionToDml(shapePr.RelativeVerticalPosition));

            if ((shapePr.VerticalAlignment != null) &&
                ((VerticalAlignment)shapePr.VerticalAlignment != VerticalAlignment.None) &&
                ((VerticalAlignment)shapePr.VerticalAlignment != VerticalAlignment.Inline))
            {
                builder.WriteElement("wp:align", DmlEnum.VerticalAlignmentToDml(shapePr.VerticalAlignment));
            }
            else
            {
                if (isFallback)
                {
                    int top = (shapePr.Top != null) ? ConvertUtilCore.PointToEmu((double)shapePr.Top) : 0;
                    builder.WriteElement("wp:posOffset", FormatterPal.IntToXml(top));
                }
                else
                {
                    WritePercentage("wp14:pctPosVOffset", (int)shapePr.TopPercent, builder, isIsoStrict);
                }
            }

            builder.EndElement("wp:positionV");
        }

        private static void WritePositionHCore(ShapePrBag shapePr, NrxXmlBuilder builder,
            bool isFallback, bool isIsoStrict)
        {
            builder.StartElement("wp:positionH");
            builder.WriteAttribute("relativeFrom", DmlEnum.RelativeHorizontalPositionToDml(shapePr.RelativeHorizontalPosition));

            if ((shapePr.HorizontalAlignment != null) &&
                ((HorizontalAlignment)shapePr.HorizontalAlignment != HorizontalAlignment.None))
            {
                builder.WriteElement("wp:align", DmlEnum.HorizontalAlignmentToDml(shapePr.HorizontalAlignment));
            }
            else
            {
                if (isFallback)
                {
                    int left = (shapePr.Left != null) ? ConvertUtilCore.PointToEmu((double)shapePr.Left) : 0;
                    builder.WriteElement("wp:posOffset", FormatterPal.IntToXml(left));
                }
                else
                {
                    WritePercentage("wp14:pctPosHOffset", (int)shapePr.LeftPercent, builder, isIsoStrict);
                }
            }

            builder.EndElement("wp:positionH");
        }

        private static void WriteWrap(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            WrapType wrapType = (WrapType)((shapePr.WrapType != null) ? shapePr.WrapType : ShapePr.WrapTypeDefault);
            switch (wrapType)
            {
                case WrapType.Inline:
                    // Do nothing.
                    break;
                case WrapType.None:
                    builder.WriteEmptyElement("wp:wrapNone");
                    break;
                case WrapType.Square:
                    builder.StartElement("wp:wrapSquare");
                    // Have not yet seen distL, distT, distR, distB.
                    WriteWrapText(shapePr, writer);
                    builder.EndElement();
                    break;
                case WrapType.Through:
                    builder.StartElement("wp:wrapThrough");
                    // Have not yet seen distL, distR.
                    WriteWrapText(shapePr, writer);
                    WriteWrapPolygon(shapePr, writer);
                    builder.EndElement();
                    break;
                case WrapType.Tight:
                    builder.StartElement("wp:wrapTight");
                    // Have not yet seen distL, distR.
                    WriteWrapText(shapePr, writer);
                    WriteWrapPolygon(shapePr, writer);
                    builder.EndElement();
                    break;
                case WrapType.TopBottom:
                    builder.StartElement("wp:wrapTopAndBottom");
                    // Have not yet seen distT, distB.
                    builder.EndElement();
                    break;
                default:
                    Debug.Fail("Write wrap type.");
                    break;
            }
        }

        private static void WriteWrapText(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            WrapSide wrapSide = (shapePr.WrapSide != null) ? (WrapSide)shapePr.WrapSide : WrapSide.Default;
            builder.WriteAttribute("wrapText", DmlEnum.WrapSideToDml(wrapSide));
        }

        private static void WriteWrapPolygon(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            builder.StartElement("wp:wrapPolygon");
            builder.WriteAttribute("edited", shapePr.EditedWrap);

            if (shapePr.WrapPolygonVertices != null)
            {
                PathPoint[] wrapPolygon = (PathPoint[])shapePr.WrapPolygonVertices;
                WritePolygonStart(wrapPolygon[0], builder);
                for (int i = 1; i < wrapPolygon.Length; i++)
                    WritePolygonLineTo(wrapPolygon[i], builder);
                WritePolygonLineTo(wrapPolygon[0], builder);
            }
            else
            {
                // Wrap polygon does not exist, but we actually have to write it to OOXML, 
                // it is required by the schema. Let's just write default.
                DmlBasicsWriter.WriteXY("wp:start", -389, 0, builder);
                DmlBasicsWriter.WriteXY("wp:lineTo", -389, 21340, builder);
                DmlBasicsWriter.WriteXY("wp:lineTo", 21795, 21340, builder);
                DmlBasicsWriter.WriteXY("wp:lineTo", 21795, 0, builder);
                DmlBasicsWriter.WriteXY("wp:lineTo", -389, 0, builder);
            }

            builder.EndElement();
        }

        private static void WritePolygonStart(PathPoint pt, NrxXmlBuilder builder)
        {
            DmlBasicsWriter.WriteXY("wp:start", pt.X.Value, pt.Y.Value, builder);
        }

        private static void WritePolygonLineTo(PathPoint pt, NrxXmlBuilder builder)
        {
            DmlBasicsWriter.WriteXY("wp:lineTo", pt.X.Value, pt.Y.Value, builder);
        }

        private static void WriteDocPr(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            //<complexType name="CT_NonVisualDrawingProps">
            //  <sequence>
            //      <element name="hlinkClick" type="CT_Hyperlink" minOccurs="0" maxOccurs="1"/>
            //      <element name="hlinkHover" type="CT_Hyperlink" minOccurs="0" maxOccurs="1"/>
            //      <element name="extLst" type="CT_OfficeArtExtensionList" minOccurs="0" maxOccurs="1"/>
            //  </sequence>
            //  <attribute name="id" type="ST_DrawingElementId" use="required"/>
            //  <attribute name="name" type="xsd:string" use="required"/>
            //  <attribute name="descr" type="xsd:string" use="optional" default=""/>
            //  <attribute name="hidden" type="xsd:boolean" use="optional" default="false"/>
            //</complexType>
            NrxXmlBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement("wp:docPr");
            builder.WriteAttribute("id", shapePr.ShapeId, ShapePr.ShapeIdDefault);
            // WORDSNET-21316 We must always write the name attribute. This is the method that always writes a string attribute.
            builder.WriteAttributeString("name", (shapePr.ShapeName != null) ? (string)shapePr.ShapeName : ShapePr.ShapeNameDefault);
            builder.WriteAttribute("descr", shapePr.ShapeDescription);
            builder.WriteAttribute("title", shapePr.ShapeTitle);

            if (shapePr.Hidden != null)
                builder.WriteAttributeIfTrue("hidden", (bool)shapePr.Hidden);

            DmlHlinkWriter.WriteHlink("a:hlinkClick", (string)shapePr.HyperlinkAddress, (string)shapePr.HyperlinkTarget,
                (string)shapePr.ScreenTip, null, writer);

            if (shapePr.DocPrExtensions != null)
                DmlExtensionListWriter.Write("a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict),
                    shapePr.DocPrExtensions, writer);

            builder.EndElement();
        }

        private static void WriteExtent(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            DmlBasicsWriter.WriteCxCy(
                "wp:extent",
                ConvertUtilCore.PointToEmu((double)shapePr.Width),
                ConvertUtilCore.PointToEmu((double)shapePr.Height),
                builder);
        }

        private static void WriteEffectExtent(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            if ((shapePr.DmlEffectExtentLeft == null) &&
                (shapePr.DmlEffectExtentTop == null) &&
                (shapePr.DmlEffectExtentRight == null) &&
                (shapePr.DmlEffectExtentBottom == null))
                return;

            NrxXmlBuilder builder = writer.CurrentBuilder;
            builder.StartElement("wp:effectExtent");
            builder.WriteAttribute("l", shapePr.DmlEffectExtentLeft, ShapePr.DmlEffectExtentLeftDefault);
            builder.WriteAttribute("t", shapePr.DmlEffectExtentTop, ShapePr.DmlEffectExtentTopDefault);
            builder.WriteAttribute("r", shapePr.DmlEffectExtentRight, ShapePr.DmlEffectExtentRightDefault);
            builder.WriteAttribute("b", shapePr.DmlEffectExtentBottom, ShapePr.DmlEffectExtentBottomDefault);
            builder.EndElement();
        }

        private static void WriteCNvGraphicFramePr(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;
            builder.StartElement("wp:cNvGraphicFramePr");
            WriteGraphicFrameLocks(shapePr, writer);
            builder.EndElement();
        }

        private static void WriteGraphicFrameLocks(ShapePrBag shapePr, DocxDocumentWriterBase writer)
        {
            if ((shapePr.LockAspectRatio == null) &&
                (shapePr.LockAgainstGrouping == null) &&
                (shapePr.LockPosition == null) &&
                (shapePr.LockAgainstSelect == null) &&
                (shapePr.DmlLockDrillDown == null) &&
                (shapePr.DmlLockResize == null))
                return;

            NrxXmlBuilder builder = writer.CurrentBuilder;
            builder.StartElement("a:graphicFrameLocks");
            builder.WriteAttribute("xmlns:a", writer.DocxNamespaces.DrawingMLMain);
            builder.WriteAttribute("noChangeAspect", shapePr.LockAspectRatio);
            builder.WriteAttribute("noGrp", shapePr.LockAgainstGrouping);
            builder.WriteAttribute("noMove", shapePr.LockPosition);
            builder.WriteAttribute("noSelect", shapePr.LockAgainstSelect);
            builder.WriteAttribute("noDrilldown", shapePr.DmlLockDrillDown);
            builder.WriteAttribute("noResize", shapePr.DmlLockResize);
            builder.EndElement();
        }
    }
}
