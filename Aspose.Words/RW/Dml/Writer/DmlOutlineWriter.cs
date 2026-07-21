// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2014 by Andrey Noskov

using System;
using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlOutlineWriter
    {
        internal static void Write(string tagName, DmlOutline outline, IDmlShapeWriterContext writer)
        {
            if ((outline != null) && (outline.DirectPropertiesCount > 0))
                WriteOutlineCore(tagName, outline, writer);

            // If outline DirectPropertiesCount equals zero, this means outline is taken from style and nothing should be written in XML.
        }

        private static void WriteOutlineCore(string tagName, DmlOutline outline, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isTextEffect = (tagName == "w14:textOutline");
       
            builder.StartElement(tagName);

            if (tagName == "a14:hiddenLine")                     
                builder.WriteAttribute("xmlns:a14", DmlExtensionsNamespace.DrawingMain);
            
            object widthObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.WidthInEmus);
            if (widthObj != null)
                builder.WriteAttribute(isTextEffect ? "w14:w" : "w", (double)widthObj);         

            object lineEndingCapTypeObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.LineEndingCapType);
            if (lineEndingCapTypeObj != null)
                builder.WriteAttributeIfNotDefault(isTextEffect ? "w14:cap" : "cap",
                    DmlEnum.LineEndingCapTypeToDml((EndCap)lineEndingCapTypeObj), "flat");

            object compoundLineTypeObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.CompoundLineType);
            if (compoundLineTypeObj != null)
                builder.WriteAttributeIfNotDefault(isTextEffect ? "w14:cmpd" : "cmpd",
                    DmlEnum.CompoundLineTypeToDml((ShapeLineStyle)compoundLineTypeObj), "sng");

            object strokeAlignmentObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.StrokeAlignment);
            if (strokeAlignmentObj != null)
                builder.WriteAttributeIfNotDefault(isTextEffect ? "w14:algn" : "algn",
                    ((bool)strokeAlignmentObj ? "in" : "ctr"), "ctr");

            object fillObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.Fill);
            if (fillObj != null)
                DmlFillWriter.Write((DmlFill)fillObj, writer, isTextEffect);

            WriteDmlDash(outline, builder, isTextEffect, writer.Compliance);
            WriteDmlLineJoinStyle(outline, builder, isTextEffect, writer.Compliance);

            object headLineEndStyleObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.HeadLineEndStyle);
            if (headLineEndStyleObj != null)
                WriteDmlLineEnd((DmlLineEndStyle)headLineEndStyleObj, builder, true, isTextEffect);

            object tailLineEndStyle = outline.GetDirectProperty(DmlOutlinePropertiesIds.TailLineEndStyle);
            if (tailLineEndStyle != null)
                WriteDmlLineEnd((DmlLineEndStyle)tailLineEndStyle, builder, false, isTextEffect);

            // Write extLst(Extension List) §5.1.2.1.15
            object extLst = outline.GetDirectProperty(DmlOutlinePropertiesIds.Extensions);
            if (extLst != null)
               DmlExtensionListWriter.Write((StringToObjDictionary<DmlExtension>)extLst, writer);

            builder.EndElement(tagName);
        }

        private static void WriteDmlLineEnd(DmlLineEndStyle lineEndStyle, NrxXmlBuilder builder, bool isHead, bool isTextEffect)
        {
            if (lineEndStyle == null)
                return;

            string prefix = isTextEffect ? "w14" : "a";
            string tagName = string.Format((isHead ? "{0}:headEnd" : "{0}:tailEnd"), prefix);
            builder.StartElement(tagName);
            builder.WriteAttributeIfNotDefault(isTextEffect ? "w14:type" : "type", DmlEnum.ArrowTypeToDml(lineEndStyle.Type), "none");
            builder.WriteAttributeIfNotDefault(isTextEffect ? "w14:w" : "w", DmlEnum.ArrowWidthToDml(lineEndStyle.Width), "med");
            builder.WriteAttributeIfNotDefault(isTextEffect ? "w14:len" : "len", DmlEnum.ArrowLengthToDml(lineEndStyle.Length), "med");
            builder.EndElement(tagName);
        }

        private static void WriteDmlDash(DmlOutline outline, NrxXmlBuilder builder, bool isTextEffect, 
            OoxmlComplianceCore compliance)
        {
            object dmlDashObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.Dash);
            if (dmlDashObj != null)
            {
                DmlDash dmlDash = (DmlDash)dmlDashObj;
                switch (dmlDash.DashType)
                {
                    case DmlDashType.PresetDash:
                        WriteDmlPresetDash((DmlPresetDash)dmlDash, builder, isTextEffect);
                        break;
                    case DmlDashType.CustomDash:
                        WriteDmlCustomDash((DmlCustomDash)dmlDash, builder, isTextEffect, compliance);
                        break;
                    default:
                        throw new ArgumentException("Unexpected Dml dash type.");
                }
            }
        }

        private static void WriteDmlPresetDash(DmlPresetDash dmlPresetDash, NrxXmlBuilder builder, bool isTextEffect)
        {
            string tagName = isTextEffect ? "w14:prstDash" : "a:prstDash";
            string attrName = isTextEffect ? "w14:val" : "val";
            builder.WriteElementWithAttributes(tagName, attrName, DmlEnum.PresetDashTypeToDml(dmlPresetDash.Preset));
        }

        private static void WriteDmlCustomDash(DmlCustomDash dmlCustomDash, NrxXmlBuilder builder, bool isTextEffect,
            OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = compliance == OoxmlComplianceCore.IsoStrict;
            string rootTagName = isTextEffect ? "w14:custDash" : "a:custDash";
            builder.StartElement(rootTagName);

            foreach (DmlDashStop dashStop in dmlCustomDash.DashStops)
            {
                builder.WriteElementWithAttributes((isTextEffect ? "w14:ds" : "a:ds"),
                    (isTextEffect ? "w14:d" : "d"), 
                    DmlPercentageUtil.ToPercentOrDmlPercent(dashStop.DashLength, isIsoStrict),
                    (isTextEffect ? "w14:sp" : "sp"), 
                    DmlPercentageUtil.ToPercentOrDmlPercent(dashStop.SpaceLength, isIsoStrict));
            }

            builder.EndElement(rootTagName);
        }

        private static void WriteDmlLineJoinStyle(DmlOutline outline, NrxXmlBuilder builder, bool isTextEffect, 
            OoxmlComplianceCore compliance)
        {
            bool isIsoStrict = compliance == OoxmlComplianceCore.IsoStrict;
            object dmlLineJoinStyleObj = outline.GetDirectProperty(DmlOutlinePropertiesIds.LineJoinStyle);
            if (dmlLineJoinStyleObj != null)
            {
                switch ((JoinStyle)dmlLineJoinStyleObj)
                {
                    case JoinStyle.Bevel:
                        builder.WriteEmptyElement(isTextEffect ? "w14:bevel" : "a:bevel");
                        break;
                    case JoinStyle.Round:
                        builder.WriteEmptyElement(isTextEffect ? "w14:round" : "a:round");
                        break;
                    case JoinStyle.Miter:
                        {
                            // WORDSNET-13705 3. MS Word 2013 raises document corruption error if the lim attribute of 
                            // the w14:miter element has percent sign.
                            if (isTextEffect)
                            {
                                builder.WriteElementWithAttributes("w14:miter", "w14:lim",
                                    DmlPercentageUtil.ToDmlPercent(outline.LineMiterLimit));
                            }
                            else
                            {
                                builder.WriteElementWithAttributes("a:miter", "lim",
                                    DmlPercentageUtil.ToPercentOrDmlPercent(outline.LineMiterLimit, isIsoStrict));
                            }
                            break;
                        }
                    default:
                        // Unexpected JoinStyle - do nothing.
                        break;
                }
            }
        }
    }
}
