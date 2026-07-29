// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Text.Bullets;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Nrx;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// 20.1.2.2.41 txSp (Text Shape)
    /// Represent class reading a text shape within a parent shape. This text shape is specifically used for
    /// displaying text as it has only text related child elements.
    /// </summary>
    internal class DmlTextShapeReader : DmlReaderBase
    {
        private DmlTextShapeReader()
        {
        }

        internal static DmlTextShape Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            if (xmlReader.LocalName != "txSp")
                return null;
            DmlTextShape result = new DmlTextShape();

            while (xmlReader.ReadChild("txSp"))
            {
                switch (xmlReader.LocalName)
                {
                    case "txBody":
                        ReadTextBody(reader, result.TextBody, "txBody");
                        break;
                    case "useSpRect":
                        result.UseShapeTextRectangle = true;
                        break;
                    case "xfrm":
                        result.Transform = DmlTransformReader.ReadTransform(xmlReader, complianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return result;
        }

        internal static void ReadTextBody(DocxDocumentReaderBase reader, DmlTextBody textBody, string tagName)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "p":
                        textBody.AddParagraph(ReadParagraph(reader));
                        break;
                    case "lstStyle":
                        ReadListStyles(reader, textBody.TextListStyles);
                        break;
                    case "bodyPr":
                        ReadTextBodyProperties(reader, textBody.Properties, reader.ComplianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_TextBodyProperties complex type that defines the body properties for the text body within a shape.
        /// 5.1.5.1.1 bodyPr (Body Properties)
        /// </summary>
        internal static void ReadTextBodyProperties(DocxDocumentReaderBase reader, DmlTextBodyProperties props,
            OoxmlComplianceInfo complianceInfo)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "anchor":
                        props.Anchor = DmlEnum.DmlToTextAnchoringType(xmlReader.Value);
                        break;
                    case "anchorCtr":
                        props.AnchorCenter = xmlReader.ValueAsBool;
                        break;
                    case "bIns":
                        props.BottomInset = MathUtil.DoubleToInt(xmlReader.GetValueAsEmus(complianceInfo));
                        break;
                    case "lIns":
                        props.LeftInset = MathUtil.DoubleToInt(xmlReader.GetValueAsEmus(complianceInfo));
                        break;
                    case "tIns":
                        props.TopInset = MathUtil.DoubleToInt(xmlReader.GetValueAsEmus(complianceInfo));
                        break;
                    case "rIns":
                        props.RightInset = MathUtil.DoubleToInt(xmlReader.GetValueAsEmus(complianceInfo));
                        break;
                    case "forceAA":
                        props.ForceAntiAlias = xmlReader.ValueAsBool;
                        break;
                    case "fromWordArt":
                        props.FromWordArt = xmlReader.ValueAsBool;
                        break;
                    case "rot":
                        props.Rotation = new DmlAngle(xmlReader.ValueAsDouble);
                        break;
                    case "numCol":
                        props.ColumnNumber = xmlReader.ValueAsInt;
                        break;
                    case "rtlCol":
                        props.ColumnOrder = xmlReader.ValueAsBool ?
                            DmlTextColumnOrder.RightToLeft : DmlTextColumnOrder.LeftToRight;
                        break;
                    case "spcCol":
                        props.SpaceBetweenColumns = xmlReader.ValueAsInt;
                        break;
                    case "compatLnSpc":
                        props.UseCompatibleLineSpacing = xmlReader.ValueAsBool;
                        break;
                    case "spcFirstLastPara":
                        props.AreFirstAndLastParagraphsUseSpacing = xmlReader.ValueAsBool;
                        break;
                    case "horzOverflow":
                        props.TextHorizontalOverflow = DmlEnum.DmlToTextHorizontalOverflowType(xmlReader.Value);
                        break;
                    case "upright":
                        props.IsTextUpright = xmlReader.ValueAsBool;
                        break;
                    case "vert":
                        props.TextOrientation = DmlEnum.DmlToShapeTextOrientation(xmlReader.Value);
                        break;
                    case "vertOverflow":
                        props.TextVerticalOverflow = DmlEnum.DmlToTextVerticalOverflowType(xmlReader.Value);
                        break;
                    case "wrap":
                        props.TextWrappingType = DmlEnum.DmlToTextWrappingType(xmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            // Read children
            while (xmlReader.ReadChild("bodyPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "noAutofit":
                        props.AutoFitMode = new DmlNoAutoFitMode();
                        break;
                    case "normAutofit":
                    {
                        DmlNormalAutoFitMode normalAutoFit = new DmlNormalAutoFitMode();
                        normalAutoFit.FontScale =
                            DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("fontScale",
                                String.Empty), 1, complianceInfo);
                        normalAutoFit.LineSpaceReduction =
                            DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute("lnSpcReduction",
                                String.Empty), complianceInfo);
                        props.AutoFitMode = normalAutoFit;
                        break;
                    }
                    case "spAutoFit":
                        props.AutoFitMode = new DmlShapeAutoFitMode();
                        break;
                    case "prstTxWarp":
                        ReadPresetTextWrap(props, xmlReader, complianceInfo);
                        break;
                    case "scene3d":
                        props.Scene3DProperties = DmlScene3DReader.ReadScene3DProperties(reader, complianceInfo);
                        break;
                    case "sp3d":
                        props.Shape3DProperties = DmlScene3DReader.ReadShape3DProperties(reader, complianceInfo);
                        break;
                    case "extLst":
                        props.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    case "flatTx":
                        WarnNotSupportedAndIgnoreElement(xmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        private static void ReadPresetTextWrap(DmlTextBodyProperties props, NrxXmlReader reader,
            OoxmlComplianceInfo complianceInfo)
        {
            string value = reader.ReadAttribute("prst", String.Empty);
            DmlPresetTextWarp presetTextWarp = new DmlPresetTextWarp();
            presetTextWarp.TextShapeType = DmlEnum.DmlToTextShapeType(value);
            props.PresetTextWrap = presetTextWarp;
            string presetXml = PresetTextWarpXmlRepository.GetPresetTextWarpXml(value);

            if (StringUtil.HasChars(presetXml))
            {
                NrxXmlReader guidesReader = new NrxXmlReader(presetXml, null);
                ReadPresetTextWrapGuides(value, true, props, guidesReader, complianceInfo);
            }

            // Read children
            while (reader.ReadChild("prstTxWarp"))
            {
                switch (reader.LocalName)
                {
                    case "avLst":
                        DmlGeometryReader.ReadAdjustableValues(props.PresetTextWrap.Guides, false, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
        }

        private static void ReadPresetTextWrapGuides(string tag, bool isPreset, DmlTextBodyProperties props,
            NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            while (reader.ReadChild(tag))
            {
                switch (reader.LocalName)
                {
                    case "pathLst":
                        ReadPathList(props, reader, complianceInfo);
                        break;
                    case "avLst":
                        DmlGeometryReader.ReadAdjustableValues(props.PresetTextWrap.Guides, isPreset, reader);
                        break;
                    case "gdLst":
                        ReadGuides(isPreset, props, reader);
                        break;
                    case "ahLst":
                        WarnNotSupportedAndIgnoreElement(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
        }

        private static void ReadPathList(DmlTextBodyProperties props, NrxXmlReader reader,
            OoxmlComplianceInfo complianceInfo)
        {
            while (reader.ReadChild("pathLst"))
            {
                switch (reader.LocalName)
                {
                    case "path":
                        DmlPath path = DmlPathReader.Read(reader, complianceInfo);
                        props.PresetTextWrap.AddPath(path);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
        }

        private static void ReadGuides(bool isPreset, DmlTextBodyProperties props, NrxXmlReader reader)
        {
            while (reader.ReadChild("gdLst"))
            {
                switch (reader.LocalName)
                {
                    case "gd":
                        DmlGeometryReader.ReadGuide(false, props.PresetTextWrap.Guides, isPreset, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_TextListStyle complex type that specifies the list of styles associated with this body of text.
        /// 5.1.5.4.12 lstStyle (Text List Styles)
        /// </summary>
        internal static void ReadListStyles(DocxDocumentReaderBase reader, DmlTextListStyles styles)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("lstStyle"))
            {
                switch (xmlReader.LocalName)
                {
                    case "defPPr":
                        ReadParagraphProperties(styles.DefaultParagraphProperties, reader);
                        break;
                    case "lvl1pPr":
                        ReadParagraphProperties(styles.ListLevel1Style, reader);
                        break;
                    case "lvl2pPr":
                        ReadParagraphProperties(styles.ListLevel2Style, reader);
                        break;
                    case "lvl3pPr":
                        ReadParagraphProperties(styles.ListLevel3Style, reader);
                        break;
                    case "lvl4pPr":
                        ReadParagraphProperties(styles.ListLevel4Style, reader);
                        break;
                    case "lvl5pPr":
                        ReadParagraphProperties(styles.ListLevel5Style, reader);
                        break;
                    case "lvl6pPr":
                        ReadParagraphProperties(styles.ListLevel6Style, reader);
                        break;
                    case "lvl7pPr":
                        ReadParagraphProperties(styles.ListLevel7Style, reader);
                        break;
                    case "lvl8pPr":
                        ReadParagraphProperties(styles.ListLevel8Style, reader);
                        break;
                    case "lvl9pPr":
                        ReadParagraphProperties(styles.ListLevel9Style, reader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        internal static DmlParagraph ReadParagraph(DocxDocumentReaderBase reader)
        {
            DmlParagraph paragraph = new DmlParagraph();
            ReadParagraph(paragraph, reader);
            return paragraph;
        }

        internal static void ReadParagraph(DmlParagraph paragraph, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("p"))
            {
                switch (xmlReader.LocalName)
                {
                    case "br":
                        paragraph.AddElement(ReadInline(reader));
                        break;
                    case "fld":
                        paragraph.AddElement(ReadInline(reader));
                        break;
                    case "r":
                        paragraph.AddElement(ReadInline(reader));
                        break;
                    case "m":
                        paragraph.AddElement(ReadInline(reader));
                        break;
                    case "pPr":
                        ReadParagraphProperties(paragraph.Properties, reader);
                        break;
                    case "endParaRPr":
                        ReadRunProperties(paragraph.EndParagraphRunProperties, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        internal static DmlParagraphTextElementBase ReadInline(DocxDocumentReaderBase reader)
        {
            switch (reader.XmlReader.LocalName)
            {
                case "br":
                    return ReadBreakLine(reader);
                case "fld":
                    return ReadTextField(reader);
                case "r":
                    return ReadRun(reader);
                case "m":
                    return DmlTextMathReader.Read(reader);
                default:
                    WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                    return null;
            }
        }

        private static DmlRun ReadRun(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlRun run = new DmlRun();
            while (xmlReader.ReadChild("r"))
            {
                switch (xmlReader.LocalName)
                {
                    case "t":
                        run.Text += xmlReader.ReadString();
                        break;
                    case "rPr":
                        ReadRunProperties(run.RunProperties, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
            return run;
        }

        internal static void ReadRunProperties(DmlRunProperties properties, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string localName = xmlReader.LocalName;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "altLang":
                        properties.AlternativeLanguage = (Language)LocaleConverter.DocxTagToLocale(xmlReader.Value);
                        break;
                    case "b":
                        properties.Bold = xmlReader.ValueAsBool;
                        break;
                    case "baseline":
                        properties.Baseline =
                            DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.Value, complianceInfo);
                        break;
                    case "bmk":
                        properties.BookmarkLinkTarget = xmlReader.Value;
                        break;
                    case "cap":
                        properties.Capitalization = DmlEnum.DmlToCapitalization(xmlReader.Value);
                        break;
                    case "dirty":
                        properties.IsDirty = xmlReader.ValueAsBool;
                        break;
                    case "err":
                        properties.HasSpellingError = xmlReader.ValueAsBool;
                        break;
                    case "i":
                        properties.Italics = xmlReader.ValueAsBool;
                        break;
                    case "kern":
                        properties.Kerning = new DmlTextPoints(xmlReader.ValueAsInt);
                        break;
                    case "kumimoji":
                        properties.Kumimoji = xmlReader.ValueAsBool;
                        break;
                    case "lang":
                        properties.Language = (Language)LocaleConverter.DocxTagToLocale(xmlReader.Value);
                        break;
                    case "noProof":
                        properties.NoProofing = xmlReader.ValueAsBool;
                        break;
                    case "normalizeH":
                        properties.NormalizeHeights = xmlReader.ValueAsBool;
                        break;
                    case "smtClean":
                        properties.SmartTagsClean = xmlReader.ValueAsBool;
                        break;
                    case "smtId":
                        properties.SmartTagID = xmlReader.ValueAsUInt;
                        break;
                    case "spc":
                        properties.Spacing = new DmlTextPoints(xmlReader.GetValueAs100thsOfPoint(complianceInfo));
                        break;
                    case "strike":
                        properties.Strikethrough = DmlEnum.DmlToTextStrike(xmlReader.Value);
                        break;
                    case "sz":
                        properties.FontSize = new DmlTextPoints(xmlReader.ValueAsInt);
                        break;
                    case "u":
                        properties.Underline = DmlEnum.DmlToTextUnderlineType(xmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            while (xmlReader.ReadChild(localName))
            {
                switch (xmlReader.LocalName)
                {
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "noFill":
                    case "pattFill":
                    case "solidFill":
                        properties.Fill = DmlFillReader.Read(reader);
                        break;
                    case "ln":
                        properties.Outline = DmlOutlineReader.Read(reader);
                        break;
                    case "highlight":
                    {
                        while (xmlReader.ReadChild("highlight"))
                        {
                            properties.HighlightColor = DmlColorReader.Read(xmlReader, complianceInfo);
                        }
                        break;
                    }
                    case "cs":
                        properties.ComplexScriptFont = ReadFont(xmlReader);
                        break;
                    case "ea":
                        properties.EastAsianFont = ReadFont(xmlReader);
                        break;
                    case "sym":
                        properties.SymbolFont = ReadFont(xmlReader);
                        break;
                    case "latin":
                        properties.LatinFont = ReadFont(xmlReader);
                        break;
                    case "effectLst":
                        properties.Effects = DmlShapeEffectReader.ReadEffects(reader, false, false);
                        break;
                    case "effectDag":
                        properties.Effects = DmlShapeEffectReader.ReadEffects(reader, false, true);
                        break;
                    case "hlinkClick":
                        properties.HlinkClick = DmlHlinkReader.Read(reader);
                        break;
                    case "hlinkMouseOver":
                        properties.HlinkHover = DmlHlinkReader.Read(reader);
                        break;
                    case "uFill":
                        properties.UnderlineFill = ReadUnderlineFill(reader);
                        break;
                    case "uFillTx":
                        properties.UnderlineFillTx = true;
                        break;
                    case "uLn":
                        properties.UnderlineStroke = DmlOutlineReader.Read(reader);
                        break;
                    case "uLnTx":
                        properties.UnderlineStrokeTx = true;
                        break;
                    case "rtl":
                        properties.RightToLeftFlowDirection = true;
                        break;
                    case "extLst":
                        properties.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        private static DmlFill ReadUnderlineFill(DocxDocumentReaderBase reader)
        {
            DmlFill fill = null;
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("uFill"))
            {
                switch (xmlReader.LocalName)
                {
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "pattFill":
                    case "noFill":
                    case "solidFill":
                        fill = DmlFillReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
            return fill;
        }

        private static DmlFont ReadFont(NrxXmlReader reader)
        {
            DmlFont font = new DmlFont();

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "charset":
                        font.SimilarCharacterSet = reader.ValueAsInt;
                        break;
                    case "panose":
                        font.PanoseSetting = reader.Value;
                        break;
                    case "pitchFamily":
                        font.SimilarFontFamily = reader.ValueAsInt;
                        break;
                    case "typeface":
                        font.TextTypeface = reader.Value;
                        break;
                    default:
                        WarnUnexpected(reader);
                        break;
                }
            }

            return font;
        }

        private static DmlTextField ReadTextField(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlTextField field = new DmlTextField();
            field.Id = xmlReader.ReadGuidAttribute("id", Guid.Empty);
            field.Type = xmlReader.ReadAttribute("type", String.Empty);
            while (xmlReader.ReadChild("fld"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rPr":
                        ReadRunProperties(field.RunProperties, reader);
                        break;
                    case "pPr":
                        ReadParagraphProperties(field.ParagraphProperties, reader);
                        break;
                    case "t":
                        field.Text += xmlReader.ReadString();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
            return field;
        }

        private static void ReadParagraphProperties(DmlParagraphProperties properties, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string localName = xmlReader.LocalName;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "algn":
                        properties.Alignment = DmlEnum.DmlToTextAlignment(xmlReader.Value);
                        break;
                    case "defTabSz":
                        properties.DefaultTabSize = MathUtil.DoubleToInt(xmlReader.GetValueAsEmus(complianceInfo));
                        break;
                    case "eaLnBrk":
                        properties.IsEastAsianLineBreakAllowed = xmlReader.ValueAsBool;
                        break;
                    case "hangingPunct":
                        properties.IsHangingPunctuationAllowed = xmlReader.ValueAsBool;
                        break;
                    case "fontAlgn":
                        properties.FontAlignment = DmlEnum.DmlToFontAlignment(xmlReader.Value);
                        break;
                    case "indent":
                        properties.TextIdentation = xmlReader.ValueAsInt;
                        break;
                    case "latinLnBrk":
                        properties.IsLatinLineBreakAllowed = xmlReader.ValueAsBool;
                        break;
                    case "lvl":
                        properties.Level = xmlReader.ValueAsInt;
                        break;
                    case "marL":
                        properties.LeftMargin = xmlReader.ValueAsInt;
                        break;
                    case "marR":
                        properties.RightMargin = xmlReader.ValueAsInt;
                        break;
                    case "rtl":
                        properties.RightToLeftFlowDirection = xmlReader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            while (xmlReader.ReadChild(localName))
            {
                switch (xmlReader.LocalName)
                {
                    case "defRPr":
                        properties.HasDefaultRunProperties = true;
                        ReadRunProperties(properties.DefaultRunProperties, reader);
                        break;
                    case "lnSpc":
                        properties.LineSpacing = ReadTextSpacing(xmlReader, complianceInfo);
                        break;
                    case "spcAft":
                        properties.SpaceAfter = ReadTextSpacing(xmlReader, complianceInfo);
                        break;
                    case "spcBef":
                        properties.SpaceBefore = ReadTextSpacing(xmlReader, complianceInfo);
                        break;
                        // Currently not implemented.
                    case "buAutoNum":
                        properties.Bullet = ReadAutoNumBullet(xmlReader);
                        break;
                    case "buBlip":
                        properties.Bullet = ReadBlipBullet(reader);
                        break;
                    case "buChar":
                        properties.Bullet = ReadCharBullet(xmlReader);
                        break;
                    case "buNone": // Sent empty bullet its type is none by default.
                        properties.Bullet = new DmlTextBullet();
                        break;
                    case "buClr":
                    case "buClrTx":
                        properties.BulletColor = ReadBulletColor(xmlReader, complianceInfo);
                        break;
                    case "buFont":
                    case "buFontTx":
                        properties.BulletFont = ReadBulletFont(xmlReader);
                        break;
                    case "buSzPct":
                    case "buSzPts":
                    case "buSzTx":
                        properties.BulletSize = ReadBulletSize(xmlReader, complianceInfo);
                        break;
                    case "tabLst":
                        properties.TabList = ReadTabList(xmlReader, complianceInfo);
                        break;
                    case "extLst":
                        properties.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }
        }

        private static TabStopCollection ReadTabList(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            TabStopCollection tabStops = new TabStopCollection();

            while (xmlReader.ReadChild("tabLst"))
            {
                switch (xmlReader.LocalName)
                {
                    case "tab":
                    {
                        TabAlignment alignment = TabAlignment.Center;
                        int pos = 0;

                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "algn":
                                    alignment = NrxParaEnum.XmlToTabAlignment(xmlReader.Value, complianceInfo);
                                    break;
                                case "pos":
                                    pos = MathUtil.DoubleToInt(xmlReader.GetValueAsEmus(complianceInfo));
                                    break;
                                default:
                                    WarnUnexpected(xmlReader);
                                    break;
                            }
                        }

                        tabStops.Add(new TabStop(pos, alignment, TabLeader.None));
                        break;
                    }
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            return tabStops;
        }

        private static DmlTextBullet ReadAutoNumBullet(NrxXmlReader xmlReader)
        {
            DmlTextBullet bullet = new DmlTextBullet();
            bullet.StartAt = xmlReader.ReadIntAttribute("startAt", 1);
            bullet.BulletType = DmlEnum.DmlToTextBulletType(xmlReader.ReadAttribute("type", ""));
            return bullet;
        }

        private static DmlTextBullet ReadBlipBullet(DocxDocumentReaderBase reader)
        {
            DmlTextBullet bullet = new DmlTextBullet();

            while (reader.XmlReader.ReadChild("buBlip"))
            {
                switch (reader.XmlReader.LocalName)
                {
                    case "blip":
                        bullet.PictureBullet = DmlFillReader.ReadBlip(reader);
                        break;
                    default:
                        WarnUnexpected(reader.XmlReader);
                        break;
                }
            }

            return bullet;
        }

        private static DmlTextBullet ReadCharBullet(NrxXmlReader xmlReader)
        {
            DmlTextBullet bullet = new DmlTextBullet();
            bullet.BulletChar = xmlReader.ReadAttribute("char", "");
            return bullet;
        }

        private static DmlTextBulletColor ReadBulletColor(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            DmlTextBulletColor bulletColor = new DmlTextBulletColor();

            switch (xmlReader.LocalName)
            {
                case "buClr":
                    bulletColor.Color = DmlColorReader.ReadColor(xmlReader, complianceInfo);
                    break;
                case "buClrTx":
                default:
                    break;
            }

            return bulletColor;
        }

        private static DmlTextBulletFont ReadBulletFont(NrxXmlReader xmlReader)
        {
            DmlTextBulletFont bulletFont = new DmlTextBulletFont();

            switch (xmlReader.LocalName)
            {
                case "buFont":
                    bulletFont.Font = ReadFont(xmlReader);
                    break;
                case "buFontTx":
                default:
                    break;
            }

            return bulletFont;
        }

        private static DmlTextBulletSize ReadBulletSize(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            double value = 0;
            DmlTextBulletSizeType sizeType = DmlTextBulletSizeType.FolowText;

            switch (xmlReader.LocalName)
            {
                case "buSzPct":
                    double asFraction =
                        DmlPercentageUtil.FromPercentOrDmlPercent(xmlReader.ReadAttribute(""), 1, complianceInfo);
                    value = DmlPercentageUtil.ToDmlPercent(asFraction);
                    sizeType = DmlTextBulletSizeType.Percentage;
                    break;
                case "buSzPts":
                    value = xmlReader.ReadDoubleAttribute(1.0);
                    sizeType = DmlTextBulletSizeType.Points;
                    break;
                case "buSzTx":
                default:
                    break;
            }

            return new DmlTextBulletSize(value, sizeType);
        }

        private static DmlTextSpacing ReadTextSpacing(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            DmlTextSpacing result = null;
            string parentElement = reader.LocalName;
            while (reader.ReadChild(parentElement))
            {
                switch (reader.LocalName)
                {
                    case "spcPct":
                    {
                        double value =  DmlPercentageUtil.FromPercentOrDmlPercent(reader.ReadAttribute(""), 1, complianceInfo);
                        DmlPercentageTextSpacing spacing = new DmlPercentageTextSpacing(value);
                        result = spacing;
                        break;
                    }
                    case "spcPts":
                    {
                        int value = reader.ReadIntAttribute(-1);
                        DmlPointsTextSpacing spacing = new DmlPointsTextSpacing(new DmlTextPoints(value));
                        result = spacing;
                        break;
                    }
                    default:
                    WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
            return result;
        }

        private static DmlTextBreakLine ReadBreakLine(DocxDocumentReaderBase reader)
        {
            DmlTextBreakLine breakLine = new DmlTextBreakLine();
            while (reader.XmlReader.ReadChild("br"))
            {
                switch (reader.XmlReader.LocalName)
                {
                    case "rPr":
                        ReadRunProperties(breakLine.RunProperties, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                        break;
                }
            }
            return breakLine;
        }

        private static IPresetTextWarpXmlRepository PresetTextWarpXmlRepository
        {
            get
            {
                if (mPresetTextWarpXmlRepository == null)
                    mPresetTextWarpXmlRepository = new PresetTextWarpXmlRepository();
                return mPresetTextWarpXmlRepository;
            }
        }

        private static IPresetTextWarpXmlRepository mPresetTextWarpXmlRepository;
    }
}
