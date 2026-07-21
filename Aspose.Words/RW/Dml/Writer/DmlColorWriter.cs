// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/16/2014 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlColorWriter
    {
        internal static void Write(DmlColor color, IDmlShapeWriterContext writer)
        {
            Write("a", color, writer);
        }

        internal static void Write(string prefix, DmlColor color, IDmlShapeWriterContext writer)
        {
            if (color == null)
                return;

            switch (color.ColorType)
            {
                case DmlColorType.HexRgbColor:
                    WriteHexRgbColor(prefix, (DmlHexRgbColor)color, writer);
                    break;
                case DmlColorType.HslColor:
                    WriteHslColor(prefix, (DmlHslColor)color, writer);
                    break;
                case DmlColorType.PercentageRgbColor:
                    WritePercentageRgbColor(prefix, (DmlPercentageRgbColor)color, writer);
                    break;
                case DmlColorType.PlaceholderColor:
                    WritePlaceholderColor(prefix, (DmlPlaceholderColor)color, writer);
                    break;
                case DmlColorType.PresetColor:
                    WritePresetColor(prefix, (DmlPresetColor)color, writer);
                    break;
                case DmlColorType.SchemeColor:
                    WriteSchemeColor(prefix, (DmlSchemeColor)color, writer);
                    break;
                case DmlColorType.SystemColor:
                    WriteSystemColor(prefix, (DmlSystemColor)color, writer);
                    break;
                default:
                    throw new ArgumentException("Unexpected Dml color type.");
            }
        }

        private static void WriteHexRgbColor(string prefix, DmlHexRgbColor hexRgbColor, IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:srgbClr", prefix);
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);

            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "val"), hexRgbColor.Value);
            WriteHexRgbColorIndex(hexRgbColor, writer);
            DmlColorModifiersWriter.Write(prefix, hexRgbColor.ColorModifiers, writer);

            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes color index and related attributes.
        /// </summary>
        /// <param name="hexRgbColor">Color.</param>
        /// <param name="writer">Document writer.</param>
        private static void WriteHexRgbColorIndex(DmlHexRgbColor hexRgbColor, IDmlShapeWriterContext writer)
        {
            Debug.Assert((hexRgbColor != null) && (writer != null));

            if (hexRgbColor.ColorIndex == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            const string clrIndexPrefix = "a14";
            string clrIndexVal = string.Format("{0}:legacySpreadsheetColorIndex", clrIndexPrefix);

            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string nameSpace = DocxNamespaces.GetNamespace(DocxNamespace.MarkupCompatibility, isIsoStrict);

            builder.WriteAttribute(clrIndexVal, hexRgbColor.ColorIndex);
            builder.WriteAttribute("mc:Ignorable", clrIndexPrefix);

            builder.WriteAttribute("xmlns:a14", DmlExtensionsNamespace.DrawingMain);

            builder.WriteAttribute("xmlns:mc", nameSpace);
        }

        private static void WriteHslColor(string prefix, DmlHslColor hslColor, IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:hslClr", prefix);
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "hue"), 
                hslColor.Hue.Value);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "sat"),
                DmlPercentageUtil.ToPercentOrDmlPercent(hslColor.Saturation, isIsoStrict));
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "lum"),
                DmlPercentageUtil.ToPercentOrDmlPercent(hslColor.Luminance, isIsoStrict));

            DmlColorModifiersWriter.Write(prefix, hslColor.ColorModifiers, writer);

            builder.EndElement(tagName);
        }

        private static void WritePercentageRgbColor(string prefix, DmlPercentageRgbColor percentageRgbColor,
            IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:scrgbClr", prefix);
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);

            WritePercentWithZeroCheck(writer, prefix, "r", percentageRgbColor.R);
            WritePercentWithZeroCheck(writer, prefix, "g", percentageRgbColor.G);
            WritePercentWithZeroCheck(writer, prefix, "b", percentageRgbColor.B);

            DmlColorModifiersWriter.Write(prefix, percentageRgbColor.ColorModifiers, writer);
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes percentage attribute to current element. Interprets values that are less than precision of
        /// a double value as zero.
        /// </summary>
        private static void WritePercentWithZeroCheck(IDmlShapeWriterContext writer, string prefix, 
            string attribute, double value)
        {
            string strValue = DmlPercentageUtil.ToPercentOrDmlPercent(!MathUtil.IsZero(value) ? value : 0d,
                writer.Compliance == OoxmlComplianceCore.IsoStrict);
            writer.Builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, attribute), strValue);
        }

        private static void WritePlaceholderColor(string prefix, DmlPlaceholderColor placeholderColor,
            IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:schemeClr", prefix);
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "val"), "phClr");

            DmlColorModifiersWriter.Write(prefix, placeholderColor.ColorModifiers, writer);

            builder.EndElement(tagName);
        }

        private static void WritePresetColor(string prefix, DmlPresetColor presetColor, IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:prstClr", prefix);
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "val"), presetColor.Value);

            DmlColorModifiersWriter.Write(prefix, presetColor.ColorModifiers, writer);

            builder.EndElement(tagName);
        }

        private static void WriteSchemeColor(string prefix, DmlSchemeColor schemeColor, IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:schemeClr", prefix);
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "val"), ThemeColorConverter.ToString(schemeColor.Value));

            DmlColorModifiersWriter.Write(prefix, schemeColor.ColorModifiers, writer);

            builder.EndElement(tagName);
        }

        private static void WriteSystemColor(string prefix, DmlSystemColor systemColor, IDmlShapeWriterContext writer)
        {
            string tagName = string.Format("{0}:sysClr", prefix);
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);
            builder.WriteAttribute(DmlNamespaceUtil.GetAttrName(prefix, "val"), systemColor.Value);
            builder.WriteAttributeIfNotDefault(DmlNamespaceUtil.GetAttrName(prefix, "lastClr"), systemColor.LastColor, "");

            DmlColorModifiersWriter.Write(prefix, systemColor.ColorModifiers, writer);

            builder.EndElement(tagName);
        }
    }
}
