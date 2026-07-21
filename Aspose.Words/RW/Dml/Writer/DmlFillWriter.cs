// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2014 by Andrey Noskov

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlFillWriter
    {
        internal static void Write(DmlFill fill, IDmlShapeWriterContext writer, bool isTextEffect)
        {
            Write(fill, writer, isTextEffect, false);
        }

        /// <summary>
        /// Writes the DML fill with the option to specify whether placeholder colors that occur in chart style parts
        /// can be written to the current document part. Such a color has no particular color definition, and colors
        /// without a definition are not written to the document by default.
        /// </summary>
        internal static void Write(DmlFill fill, IDmlShapeWriterContext writer, bool isTextEffect,
            bool writePlaceholderColor)
        {
            if (fill == null)
                return;

            switch (fill.DmlFillType)
            {
                case DmlFillType.BlipFill:
                    WriteDmlBlipFill("a", (DmlBlipFill)fill, writer);
                    break;
                case DmlFillType.GradientFill:
                    WriteDmlGradientFill((DmlGradientFill)fill, writer, isTextEffect);
                    break;
                case DmlFillType.GroupFill:
                    WriteGroupFill(writer);
                    break;
                case DmlFillType.NoFill:
                    WriteDmlNoFill(writer, isTextEffect);
                    break;
                case DmlFillType.PatternFill:
                    WriteDmlPatternFill((DmlPatternFill)fill, writer, writePlaceholderColor);
                    break;
                case DmlFillType.SolidFill:
                    WriteDmlSolidFill((DmlSolidFill)fill, writer, isTextEffect);
                    break;
                case DmlFillType.StyleFill:
                    // If fill isn't specified then use the fill provided by the shape's style.
                    // So we have nothing to write here.
                    break;
                default:
                    throw new ArgumentException("Unexpected Dml fill type.");
            }
        }

        internal static void WriteDmlBlipFill(string prefix, DmlBlipFill blipFill, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string tagName = string.Format("{0}:blipFill", prefix);

            builder.StartElement(tagName);

            // If not present or zero, the DPI in the blip is used.
            if (blipFill.DotsPerInch > 0)
                builder.WriteAttributeUInt("dpi", blipFill.DotsPerInch);

            const string rotAttrName = "rotWithShape";

            // WORDSNET-14807 Mimic MSW and write "rotWithShape" explicitly, when
            // shape has a tile fill. MSW uses "1" as a default value for this attribute.
            if ((blipFill.BlipFillMode is DmlBlipFillTile) || !blipFill.RotateWithShape)
                builder.WriteAttribute(rotAttrName, blipFill.RotateWithShape);

            WriteBlip(blipFill.Blip, writer, "a:blip");
            // For converted VML shapes currently skip writing empty "srcRect".
            // Although mimic MSW behavior does it.
            DmlWriterUtil.WritePercentageOffsetRectangle(blipFill.SourceRectangle, "a", "srcRect", false,
                builder, isIsoStrict);
            blipFill.BlipFillMode.Write(writer);

            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes Blip.
        /// </summary>
        internal static void WriteBlip(DmlBlip blip, IDmlShapeWriterContext writer, string name)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement(name);

            WriteBlipImage(blip, writer);

            if (blip.CompressionState != DmlCompressionState.Email)
                builder.WriteAttribute("cstate", DmlEnum.CompressionStateToDml(blip.CompressionState));

            builder.WriteAttribute("xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));

            DmlEffectsWriter.Write(blip.Effects, writer);
            DmlExtensionListWriter.Write(blip.Extensions, writer);

            builder.EndElement(name);
        }

        /// <summary>
        /// Writes Blip image data (image binary data or/and image link).
        /// </summary>
        internal static void WriteBlipImage(DmlBlip blip, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            // WORDSNET-17960 If the length of the embedded image is zero, then there is nothing to write.
            if (blip.HasEmbedImage)
            {
                string imageId = writer.WriteImageBinData(blip.EmbedImage);
                builder.WriteAttribute("r:embed", imageId);
            }
            if (StringUtil.HasChars(blip.ImageLink))
            {
                string imageLinkId = writer.WriteImageLink(blip.ImageLink);
                builder.WriteAttribute("r:link", imageLinkId);
            }
        }

        /// <summary>
        /// Writes patterned fill.
        /// </summary>
        private static void WriteDmlPatternFill(DmlPatternFill patternFill, IDmlShapeWriterContext writer,
            bool writePlaceholderColor)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement("a:pattFill");
            builder.WriteAttribute("prst", DmlEnum.PatternTypeToDml(patternFill.FillPresetPattern));

            // The document becomes corrupted if the color is not specified (except chart style part).
            DmlColor foreColor = patternFill.ForegroundColor;
            if (!foreColor.IsEmpty || (writePlaceholderColor && (foreColor.ColorType == DmlColorType.PlaceholderColor)))
            {
                builder.StartElement("a:fgClr");
                DmlColorWriter.Write(patternFill.ForegroundColor, writer);
                builder.EndElement("a:fgClr");
            }

            // The document becomes corrupted if the color is not specified (except chart style part).
            DmlColor backColor = patternFill.BackgroundColor;
            if (!backColor.IsEmpty || (writePlaceholderColor && (backColor.ColorType == DmlColorType.PlaceholderColor)))
            {
                builder.StartElement("a:bgClr");
                DmlColorWriter.Write(patternFill.BackgroundColor, writer);
                builder.EndElement("a:bgClr");
            }

            builder.EndElement("a:pattFill");
        }

        /// <summary>
        /// Writes gradient fill.
        /// </summary>
        private static void WriteDmlGradientFill(DmlGradientFill gradientFill, IDmlShapeWriterContext writer,
            bool isTextEffect)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);

            string prefix = isTextEffect ? "w14" : "a";
            string tagName = string.Format("{0}:gradFill", prefix);
            builder.StartElement(tagName);

            builder.WriteAttributeIfNotDefault("flip", DmlEnum.TileFlipModeToDml(gradientFill.TileFlipMode), "none");
            builder.WriteAttribute("rotWithShape", gradientFill.RotateWithShape);

            WriteDmlGradientStops(gradientFill, writer, isTextEffect);
            gradientFill.Gradient.Write(writer, isTextEffect);
            DmlWriterUtil.WritePercentageOffsetRectangle(gradientFill.TileRectangle, prefix, "tileRect", false,
                builder, isIsoStrict);

            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes gradient stops.
        /// </summary>
        private static void WriteDmlGradientStops(DmlGradientFill gradientFill, IDmlShapeWriterContext writer,
            bool isTextEffect)
        {
            NrxXmlBuilder builder = writer.Builder;

            string tagName = isTextEffect ? "w14:gsLst" : "a:gsLst";
            builder.StartElement(tagName);
            foreach (DmlGradientStop stop in gradientFill.GradientStops)
                WriteDmlGradientStop(stop, writer, isTextEffect);
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes gradient stop.
        /// </summary>
        private static void WriteDmlGradientStop(DmlGradientStop stop, IDmlShapeWriterContext writer, bool isTextEffect)
        {
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            NrxXmlBuilder builder = writer.Builder;
            string prefix = isTextEffect ? "w14" : "a";
            string tagName = string.Format("{0}:gs", prefix);
            builder.StartElement(tagName);

            if (isIsoStrict)
                stop.Position = DmlPercentageUtil.ToDmlPercentPrecision(stop.Position);

            builder.WriteAttribute((isTextEffect ? "w14:pos" : "pos"),
                DmlPercentageUtil.ToPercentOrDmlPercent(stop.Position, isIsoStrict));

            DmlColorWriter.Write(prefix, stop.Color, writer);
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes solid fill.
        /// </summary>
        private static void WriteDmlSolidFill(DmlSolidFill solidFill, IDmlShapeWriterContext writer, bool isTextEffect)
        {
            NrxXmlBuilder builder = writer.Builder;

            string prefix = (isTextEffect) ? "w14" : "a";
            string tagName = string.Format("{0}:solidFill", prefix);

            DmlColor color;
            // WORDSNET-12980 If color was not initialized yet, we should initialize it.
            // Otherwise we will write empty string for the color and
            // this causes MS Word to crash.
            // WORDSNET-13497 Except placeholder color.
            if (solidFill.IsColorEmpty && (solidFill.Color.ColorType != DmlColorType.PlaceholderColor))
            {
                color = DmlColor.CreateFromDrColor(DrColor.Empty);
                color.ColorModifiers = solidFill.Color.ColorModifiers;
            }
            else
            {
                color = solidFill.Color;
            }

            builder.StartElement(tagName);
            DmlColorWriter.Write(prefix, color, writer);
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes NoFill fill.
        /// </summary>
        private static void WriteDmlNoFill(IDmlShapeWriterContext writer, bool isTextEffect)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.WriteEmptyElement(isTextEffect ? "w14:noFill" : "a:noFill");
        }

        /// <summary>
        /// Writes group fill.
        /// </summary>
        private static void WriteGroupFill(IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            builder.WriteEmptyElement("a:grpFill");
        }



    }
}
