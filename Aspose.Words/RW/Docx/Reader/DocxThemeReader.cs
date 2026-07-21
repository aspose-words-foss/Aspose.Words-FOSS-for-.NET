// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2009 by Roman Korchagin

using System;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fonts;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.Themes;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads 5.1.8.9 theme (Theme).
    ///
    /// At the moment stores the theme unparsed in the document so it preserved and can be saved.
    /// At the moment parses only some info from the scheme related to fonts.
    /// When later I implement complete theme parsing, then storing unparsed code will be removed.
    /// </summary>
    internal class DocxThemeReader : DmlReaderBase
    {
        private DocxThemeReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Reads 5.1.8.9 theme (Theme).
        /// </summary>
        internal static void Read(DocxDocumentReader reader)
        {
            if (reader.LoadOptions.SkipFormatting)
                return;

            // Find the theme part is present. The theme part seems to be only specified for the
            // main document (not for glossary document).

            DocxXmlReader themeXmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.Theme);
            if (themeXmlReader == null)
                return;

            // Create a theme and store in the document.
            Theme theme = new Theme();
            reader.Document.SetThemeInternal(theme);

            // The theme needs this information from the settings to work properly.
            theme.ThemeFontLanguages = reader.Document.DocPr.ThemeFontLanguages;

            // Store the unparsed theme part in the model because we do not do full parsing yet.
            theme.SetThemePart(themeXmlReader.Part, themeXmlReader.NamespaceURI);

            // Also store unparsed in the model all internal child parts that are related to the theme.
            foreach (OpcRelationship rel in themeXmlReader.Part.Rels)
                ReadThemeRel(rel, reader, theme);

            DocxThemeReader themeReader = new DocxThemeReader(reader);
            themeReader.ReadCore(theme);
            reader.RestorePartReader();
        }

        /// <summary>
        /// Reads 5.1.8.9 theme (Theme).
        /// </summary>
        internal static Theme ReadThemeXml(DocxDocumentReader reader)
        {
            // Create a theme and store in the document.
            Theme theme = new Theme();

            // The theme needs this information from the settings to work properly.
            theme.ThemeFontLanguages = reader.Document.DocPr.ThemeFontLanguages;

            DocxThemeReader themeReader = new DocxThemeReader(reader);
            themeReader.ReadCore(theme);
            reader.RestorePartReader();

            return theme;
        }

        /// <summary>
        /// Reads 5.1.8.9 theme (Theme).
        /// </summary>
        internal static Theme ReadThemeOverride(DocxDocumentReaderBase reader)
        {
            if (!reader.IsDocx)
                return null;

            string themeOverrideRelType = reader.RelTypes.ThemeOverride;
            DocxXmlReader docxReader = (DocxXmlReader)reader.XmlReader;
            OpcPackagePart themeOverridePart = reader.GetPartByRelationshipType(docxReader.Part, themeOverrideRelType);
            if (themeOverridePart == null)
                return null;

            // WORDSNET-21265 Clone theme override part to avoid duplicated references to the same part (i.e. stream).
            DocxXmlReader themeXmlReader = new DocxXmlReader(themeOverridePart.Clone(), reader.LoadOptions.WarningCallback, WarningSource.DrawingML, reader.ComplianceInfo);
            reader.PushPartReader(themeXmlReader);

            // Create a theme and store in the document.
            Theme theme = new Theme();

            // The theme needs this information from the settings to work properly.
            theme.ThemeFontLanguages = reader.Document.DocPr.ThemeFontLanguages;

            // Store the unparsed theme part in the model because we do not do full parsing yet.
            theme.SetThemePart(themeXmlReader.Part, themeXmlReader.NamespaceURI);

            // Also store unparsed in the model all internal child parts that are related to the theme.
            foreach (OpcRelationship rel in themeXmlReader.Part.Rels)
                ReadThemeRel(rel, reader, theme);

            DocxThemeReader themeReader = new DocxThemeReader(reader);
            // Theme override is actually themeElements.
            themeReader.ReadThemeElements(theme);

            // Restore reader.
            reader.RestorePartReader();

            return theme;
        }

        private static void ReadThemeRel(OpcRelationship rel, DocxDocumentReaderBase reader, Theme theme)
        {
            DocxXmlReader themeXmlReader = reader.XmlReader as DocxXmlReader;
            if (rel.IsExternal || (themeXmlReader == null))
                return;

            string partName = themeXmlReader.Part.GetRelatedPartName(rel);
            OpcPackagePart part = reader.GetPartByName(partName);

            if (part != null)
            {
                theme.RelatedParts[rel.Id] = part;
            }
            else
            {
                // WORDSNET-24536 Skip parts which has "NULL" target in relations.
                reader.Warn(WarningType.DataLoss,
                    WarningSource.Docx,
                    string.Format(WarningStrings.ThemePartMissing, partName));
            }

        }

        private void ReadCore(Theme theme)
        {
            // Now parse some of the theme XML that we understand.
            theme.Name = XmlReader.ReadAttribute("name", "");

            while (XmlReader.ReadChild("theme"))
            {
                switch (XmlReader.LocalName)
                {
                    case "themeElements":
                        ReadThemeElements(theme);
                        break;
                    case "extLst":
                        // Extensions are not parsed now. A theme extension can only contain the element 2.4.1.1
                        // themeFamily [MS-ODRAWXML]. See also 2.2.8 Themes [MS-ODRAWXML].
                        ((IDmlExtensionListSource)theme).Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    case "objectDefaults":
                        theme.ObjectDefaults = ReadObjectDefaults();
                        break;
                    case "custClrLst":
                    case "extraClrSchemeLst":
                        // A warning is not generated since we write this XML to output document
                        // in ThemeWriter.WriteUnparsedElements.
                        XmlReader.IgnoreElement();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the element 20.1.6.7 objectDefaults (Object Defaults, CT_ObjectStyleDefaults).
        /// </summary>
        private ThemeObjectDefaults ReadObjectDefaults()
        {
            if (XmlReader.IsEmptyElement)
                return null;

            ThemeObjectDefaults result = new ThemeObjectDefaults();

            string tagName = XmlReader.LocalName;
            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "spDef":
                        result.ShapeDefaults = ReadShapeDefaults();
                        break;
                    case "lnDef":
                        result.LineDefaults = ReadShapeDefaults();
                        break;
                    case "txDef":
                        result.TextDefaults = ReadShapeDefaults();
                        break;
                    case "extLst":
                        if (!XmlReader.IsEmptyElement)
                            WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads shape definition element (CT_DefaultShapeDefinition).
        /// </summary>
        private DefaultShapeDefinition ReadShapeDefaults()
        {
            DefaultShapeDefinition result = new DefaultShapeDefinition();

            string tagName = XmlReader.LocalName;
            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "spPr":
                        result.ShapePr = new DefaultShapeProperties();
                        DmlNodePropertiesReader.ReadShapeProperties(mDocumentReader, result.ShapePr);
                        break;
                    case "bodyPr":
                        result.TextBodyPr = new DmlTextBodyProperties();
                        DmlTextShapeReader.ReadTextBodyProperties(mDocumentReader, result.TextBodyPr,
                            mDocumentReader.ComplianceInfo);
                        break;
                    case "lstStyle":
                        result.ListStyles = new DmlTextListStyles();
                        DmlTextShapeReader.ReadListStyles(mDocumentReader, result.ListStyles);
                        break;
                    case "style":
                        result.Style = DmlShapeStyleReader.Read(XmlReader, mDocumentReader.ComplianceInfo);
                        break;
                    case "extLst":
                        if (!XmlReader.IsEmptyElement)
                            WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads 5.1.8.10 themeElements (Theme Elements).
        /// </summary>
        private void ReadThemeElements(Theme theme)
        {
            string tagName = XmlReader.LocalName;
            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "fontScheme":
                        ReadFontScheme(theme);
                        break;
                    case "clrScheme":
                        ReadColorScheme(theme);
                        break;
                    case "fmtScheme":
                        ReadFormatScheme(theme);
                        break;
                    case "extLst":
                        if (!XmlReader.IsEmptyElement)
                            WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private void ReadFormatScheme(Theme theme)
        {
            theme.FormatScheme.Name = XmlReader.ReadAttribute("name", String.Empty);
            while (XmlReader.ReadChild("fmtScheme"))
            {
                switch (XmlReader.LocalName)
                {
                    case "bgFillStyleLst":
                        ReadBackgroundFillStyleList(theme.FormatScheme);
                        break;
                    case "fillStyleLst":
                        ReadFillStyleList(theme.FormatScheme);
                        break;
                    case "lnStyleLst":
                        ReadLineStyleList(theme.FormatScheme);
                        break;
                    case "effectStyleLst":
                        ReadEffectStyleList(theme.FormatScheme);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private void ReadBackgroundFillStyleList(FormatScheme formatScheme)
        {
            while (XmlReader.ReadChild("bgFillStyleLst"))
            {
                DmlFill fill = DmlFillReader.Read(mDocumentReader);
                if (fill != null)
                    formatScheme.AddBackgroundFillStyle(fill);
            }
        }

        private void ReadFillStyleList(FormatScheme formatScheme)
        {
            while (XmlReader.ReadChild("fillStyleLst"))
            {
                DmlFill fill = DmlFillReader.Read(mDocumentReader);
                if (fill != null)
                    formatScheme.AddFillStyle(fill);
            }
        }

        private void ReadLineStyleList(FormatScheme formatScheme)
        {
            while (XmlReader.ReadChild("lnStyleLst"))
            {
                DmlOutline outline = DmlOutlineReader.Read(mDocumentReader);
                if (outline != null)
                    formatScheme.AddLineStyle(outline);
            }
        }

        private void ReadEffectStyleList(FormatScheme formatScheme)
        {
            while (XmlReader.ReadChild("effectStyleLst"))
            {
                EffectStyle effectStyle = new EffectStyle();
                ReadEffectStyle(effectStyle);
                formatScheme.AddEffectStyle(effectStyle);
            }
        }

        private void ReadEffectStyle(EffectStyle effectStyle)
        {
            while (XmlReader.ReadChild("effectStyle"))
            {
                switch (XmlReader.LocalName)
                {
                    case "effectLst":
                        effectStyle.Effects = DmlShapeEffectReader.ReadEffects(mDocumentReader, true, false);
                        break;
                    case "scene3d":
                        effectStyle.Scene3DProperties =
                            DmlScene3DReader.ReadScene3DProperties(mDocumentReader, true, ComplianceInfo);
                        break;
                    case "sp3d":
                        effectStyle.Shape3DProperties =
                            DmlScene3DReader.ReadShape3DProperties(mDocumentReader, true, ComplianceInfo);
                        break;
                    case "effectDag":
                        effectStyle.Effects = DmlShapeEffectReader.ReadEffects(mDocumentReader, true, true);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private void ReadColorScheme(Theme theme)
        {
            theme.ColorScheme.Name = XmlReader.ReadAttribute("name", String.Empty);

            while (XmlReader.ReadChild("clrScheme"))
            {
                DmlColor color = null;
                string colorName = XmlReader.LocalName;
                switch (XmlReader.LocalName)
                {
                    case "accent1":
                    case "accent2":
                    case "accent3":
                    case "accent4":
                    case "accent5":
                    case "accent6":
                    case "dk1":
                    case "dk2":
                    case "folHlink":
                    case "hlink":
                    case "lt1":
                    case "lt2":
                        color = ReadChildColor(XmlReader, ComplianceInfo);
                        break;
                    case "extLst":
                        if (!XmlReader.IsEmptyElement)
                            WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
                if (color != null)
                {
                    theme.ColorScheme.AddColor(color, ThemeColorConverter.FromString(colorName));
                }
            }
        }

        private static DmlColor ReadChildColor(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            if (!reader.ReadChild(reader.LocalName))
                return null;

            return DmlColorReader.Read(reader, complianceInfo);
        }

        /// <summary>
        /// Reads 5.1.4.1.18 fontScheme (Font Scheme).
        /// </summary>
        private void ReadFontScheme(Theme theme)
        {
            theme.FontSchemeName = XmlReader.ReadAttribute("name", "");

            while (XmlReader.ReadChild("fontScheme"))
            {
                switch (XmlReader.LocalName)
                {
                    case "majorFont":
                        theme.SetMajorFonts(ReadFonts());
                        break;
                    case "minorFont":
                        theme.SetMinorFonts(ReadFonts());
                        break;
                    case "extLst":
                        if (!XmlReader.IsEmptyElement)
                            WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 5.1.4.1.24 majorFont (Major Font) or 5.1.4.1.25 minorFont (Minor fonts).
        /// </summary>
        private ThemeFonts ReadFonts()
        {
            ThemeFonts fonts = new ThemeFonts();

            string elemName = XmlReader.LocalName;
            while (XmlReader.ReadChild(elemName))
            {
                switch (XmlReader.LocalName)
                {
                    case "cs":
                        fonts.ComplexScriptFontInfo = ReadFont();
                        break;
                    case "ea":
                        fonts.EastAsianFontInfo = ReadFont();
                        break;
                    case "latin":
                        fonts.LatinFontInfo = ReadFont();
                        break;
                    case "font":
                        ReadSupplementalFont(fonts);
                        break;
                    case "extLst":
                        if (!XmlReader.IsEmptyElement)
                            WarnNotSupportedAndIgnoreElement(XmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return fonts;
        }

        /// <summary>
        /// Reads a
        /// 5.1.5.3.1 cs (Complex Script Font) or
        /// 5.1.5.3.3 ea (East Asian Font) or
        /// 5.1.5.3.7 latin (Latin Font)
        /// </summary>
        private FontInfo ReadFont()
        {
            FontInfo font = new FontInfo();

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "charset":
                        font.Charset = XmlReader.ValueAsInt;
                        break;
                    case "panose":
                        font.Panose = NrxXmlUtil.HexToBytes(XmlReader.Value, FontInfo.PanoseLength);
                        break;
                    case "pitchFamily":
                        font.Pitch = (FontPitch)XmlReader.ValueAsInt;
                        break;
                    case "typeface":
                        font.SetName(XmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            return font;
        }

        private void ReadSupplementalFont(ThemeFonts themeFonts)
        {
            string script = null;
            string typeface = null;

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "script":
                        script = XmlReader.Value;
                        break;
                    case "typeface":
                        typeface = XmlReader.Value;
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            if (StringUtil.HasChars(script))
            {
                ThemeSupplementalFont font = new ThemeSupplementalFont(script, typeface);
                themeFonts.SupplementalFonts[font.Script] = font;
            }
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
