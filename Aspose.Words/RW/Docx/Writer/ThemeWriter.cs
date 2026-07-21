// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/07/2015 by Alexey Morozov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Themes;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Writer
{
    internal class ThemeWriter
    {
        /// <summary>
        /// Implements writing of document theme.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer, Theme theme, bool isThemeOverride)
        {
            AnyXmlBuilder builder = writer.CurrentBuilder;
            string tagName = isThemeOverride ? "a:themeOverride" : "a:theme";

            builder.StartDocument(true);
            builder.StartElement(tagName);
            builder.WriteAttributeString("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain,
                writer.Compliance == OoxmlComplianceCore.IsoStrict));

            if (!isThemeOverride)
            {
                builder.WriteAttributeString("name", theme.Name);

                builder.StartElement("a:themeElements"); // no element in theme override
            }

            // Write color scheme
            WriteColorScheme(builder, theme.ColorScheme);

            // Write font scheme
            builder.StartElement("a:fontScheme");
            builder.WriteAttributeString("name", theme.FontSchemeName);

            builder.StartElement("a:majorFont");
            WriteThemeFonts(builder, theme.MajorFonts);
            builder.EndElement("a:majorFont");

            builder.StartElement("a:minorFont");
            WriteThemeFonts(builder, theme.MinorFonts);
            builder.EndElement("a:minorFont");

            builder.EndElement("a:fontScheme");

            // Write format scheme
            WriteFmtScheme(writer, theme.FormatScheme);

            if (!isThemeOverride)
            {
                builder.EndElement("a:themeElements");

                WriteObjectDefaults(writer, theme.ObjectDefaults);

                WriteUnparsedElements(writer, theme);

                DmlExtensionListWriter.Write(((IDmlExtensionListSource)theme).Extensions, writer);
            }

            builder.EndElement(tagName);
            builder.EndDocument();
            builder.Flush();
        }

        private static void WriteColorScheme(AnyXmlBuilder builder, ThemeColors colorScheme)
        {
            builder.StartElement("a:clrScheme");
            builder.WriteAttributeString("name", colorScheme.Name);

            foreach (ThemeColor themeColor in ThemeColors.AllThemeColors)
            {
                if (themeColor == ThemeColor.Background1 || themeColor == ThemeColor.Background2
                    || themeColor == ThemeColor.Text1 || themeColor == ThemeColor.Text2)
                {
                    continue;
                }

                DmlColor dmlColor = colorScheme.GetColor(themeColor);

                if (dmlColor == null)
                    continue;

                builder.StartElement("a:" + ThemeColorConverter.ToString(themeColor));
                switch (dmlColor.ColorType)
                {
                    case DmlColorType.HexRgbColor:
                        DmlHexRgbColor dmlHexRgbColor = (DmlHexRgbColor)dmlColor;
                        builder.StartElement("a:srgbClr");
                        builder.WriteAttributeString("val", dmlHexRgbColor.Value);
                        builder.EndElement();
                        break;
                    case DmlColorType.SystemColor:
                        DmlSystemColor dmlSystemColor = (DmlSystemColor)dmlColor;
                        builder.StartElement("a:sysClr");
                        builder.WriteAttributeString("val", dmlSystemColor.Value);
                        builder.WriteAttributeString("lastClr", dmlSystemColor.LastColor);
                        builder.EndElement("a:sysClr");
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected color type.");
                }
                builder.EndElement("a:" + ThemeColorConverter.ToString(themeColor));
            }

            builder.EndElement("a:clrScheme");
        }

        private static void WriteThemeFonts(AnyXmlBuilder builder, ThemeFonts themeFonts)
        {
            builder.StartElement("a:latin");
            builder.WriteAttributeString("typeface", themeFonts.Latin);
            builder.EndElement("a:latin");
            builder.StartElement("a:ea");
            builder.WriteAttributeString("typeface", themeFonts.EastAsian);
            builder.EndElement("a:ea");
            builder.StartElement("a:cs");
            builder.WriteAttributeString("typeface", themeFonts.ComplexScript);
            builder.EndElement("a:cs");

            // Write supplemental fonts.
            foreach (KeyValuePair<string, ThemeSupplementalFont> entry in themeFonts.SupplementalFonts)
            {
                string script = entry.Key;
                ThemeSupplementalFont supplementalFont = entry.Value;
                string typeface = supplementalFont.Typeface;

                builder.StartElement("a:font");
                builder.WriteAttributeString("script", script);
                builder.WriteAttributeString("typeface", typeface);
                builder.EndElement("a:font");
            }
        }

        private static void WriteFmtScheme(DocxDocumentWriterBase writer, FormatScheme formatScheme)
        {
            AnyXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:fmtScheme");
            if (StringUtil.HasChars(formatScheme.Name))
                builder.WriteAttributeString("name", formatScheme.Name);

            builder.StartElement("a:fillStyleLst");
            for (int i = 0; i < formatScheme.FillStyleCount; i++)
                DmlFillWriter.Write(formatScheme.GetFillStyle(i), writer, false);
            builder.EndElement("a:fillStyleLst");

            builder.StartElement("a:lnStyleLst");
            for (int i = 0; i < formatScheme.LineStyleCount; i++)
                DmlOutlineWriter.Write("a:ln", formatScheme.GetLineStyle(i), writer);
            builder.EndElement("a:lnStyleLst");

            builder.StartElement("a:effectStyleLst");
            for (int i = 0; i < formatScheme.EffectStyleCount; i++)
                WriteEffectStyle(writer, formatScheme.GetEffectStyle(i));
            builder.EndElement("a:effectStyleLst");

            builder.StartElement("a:bgFillStyleLst");
            for (int i = 0; i < formatScheme.BackgroundFillStyleCount; i++)
                DmlFillWriter.Write(formatScheme.GetBackgroundFillStyle(i), writer, false);
            builder.EndElement("a:bgFillStyleLst");

            builder.EndElement("a:fmtScheme");
        }

        /// <summary>
        /// Writes 20.1.4.1.11 effectStyle (Effect Style) element.
        /// This element defines a set of effects and 3D properties that can be applied to an object.
        /// </summary>
        private static void WriteEffectStyle(DocxDocumentWriterBase writer, EffectStyle effectStyle)
        {
            AnyXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:effectStyle");

            DmlShapeEffectsWriter.Write(effectStyle.Effects, writer, true);

            if (effectStyle.Scene3DProperties != null)
                Dml3DPropertiesWriter.WriteScene3D("a", effectStyle.Scene3DProperties, writer, true);

            if (effectStyle.Shape3DProperties != null)
                Dml3DPropertiesWriter.WriteShape3D("a", effectStyle.Shape3DProperties, writer, true);

            builder.EndElement("a:effectStyle");
        }

        /// <summary>
        /// Writes the element 20.1.6.7 objectDefaults (Object Defaults, CT_ObjectStyleDefaults).
        /// </summary>
        private static void WriteObjectDefaults(DocxDocumentWriterBase writer, ThemeObjectDefaults objectDefaults)
        {
            AnyXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:objectDefaults");

            if (objectDefaults != null)
            {
                WriteShapeDefaults(writer, objectDefaults.ShapeDefaults, "a:spDef");
                WriteShapeDefaults(writer, objectDefaults.LineDefaults, "a:lnDef");
                WriteShapeDefaults(writer, objectDefaults.TextDefaults, "a:txDef");
            }

            builder.EndElement("a:objectDefaults");
        }

        /// <summary>
        /// Writes shape definition element (CT_DefaultShapeDefinition).
        /// </summary>
        private static void WriteShapeDefaults(DocxDocumentWriterBase writer, DefaultShapeDefinition shapeDefault,
            string tagName)
        {
            if (shapeDefault == null)
                return;

            AnyXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement(tagName);

            if (shapeDefault.ShapePr != null)
                DmlShapePropertiesWriter.Write("a", shapeDefault.ShapePr, writer);
            if (shapeDefault.TextBodyPr != null)
                DmlTextShapeWriter.WriteDmlTextShapeBodyPr("a:bodyPr", shapeDefault.TextBodyPr, writer);
            if (shapeDefault.ListStyles != null)
                DmlTextShapeWriter.WriteDmlTextListStyles(shapeDefault.ListStyles, writer);
            if (shapeDefault.Style != null)
                DmlShapeStyleWriter.Write("a", shapeDefault.Style, writer);

            builder.EndElement(tagName);
        }

        private static void WriteUnparsedElements(DocxDocumentWriterBase writer, Theme theme)
        {
            AnyXmlBuilder builder = writer.CurrentBuilder;

            theme.ThemePart.Stream.Position = 0;
            AnyXmlReader xmlReader = new AnyXmlReader(theme.ThemePart.Stream);

            // We may need to change ISO Transitional namespaces to ISO Strict or backward.
            bool isSourceIsoStrict =
                xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, true);
            bool isDestinationIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            DmlIsoStrictXmlUpdater xmlUpdater = (isSourceIsoStrict != isDestinationIsoStrict)
                ? new DmlIsoStrictXmlUpdater(isDestinationIsoStrict)
                : null;

            string rootTag = xmlReader.LocalName;

            xmlReader.ReadChild(rootTag);
            while (!xmlReader.IsEndElement(rootTag))
            {
                string tagName = xmlReader.LocalName;
                string elementRaw = xmlReader.ReadOuterXml(xmlUpdater, null);

                if (tagName != "themeElements" && tagName != "objectDefaults" && tagName != "extLst")
                    builder.WriteRaw(elementRaw, true);
            }
        }

        ///// <summary>
        ///// Possible theme colors in order of appearance.
        ///// </summary>
        //private static readonly ThemeColor[] gAllThemeColors = new ThemeColor[]
        //        {
        //            ThemeColor.Dark1, ThemeColor.Light1, ThemeColor.Dark2, ThemeColor.Light2,
        //            ThemeColor.Accent1, ThemeColor.Accent2, ThemeColor.Accent3, ThemeColor.Accent4, ThemeColor.Accent5, ThemeColor.Accent6,
        //            ThemeColor.Background1, ThemeColor.Background2,
        //            ThemeColor.Hyperlink, ThemeColor.FollowedHyperlink,
        //            ThemeColor.Text1, ThemeColor.Text2
        //        };
    }
}
