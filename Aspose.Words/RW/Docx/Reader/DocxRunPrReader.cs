// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/08/2007 by Vladimir Averkin

using System;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides methods for reading run properties from different document parts.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxRunPrReader : NrxRunPrReaderBase
    {
        internal DocxRunPrReader()
        {
        }

        internal DocxRunPrReader(DocxFldCharReader fldCharReader)
        {
            mFldCharReader = fldCharReader;
        }

        /// <summary>
        /// Reads 'w:rPr' element from the specified reader.
        /// Reader should be positioned to element start.
        /// </summary>
        internal override void Read(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            if (reader.LoadOptions.SkipFormatting)
            {
                // WORDSNET-13301 Text that appears directly in an rPr element was not extracted.
                ReadTextElementsOnly(reader, runPr);
                return;
            }

            while (xmlReader.ReadChild("rPr"))
            {
                if (xmlReader.Prefix == "w14")
                {
                    reader.ComplianceInfo.IsDocxExtensions = true;
                    ReadW14Child((DocxDocumentReaderBase)reader, runPr);
                }

                if (xmlReader.IsEndElement("rPr"))
                    break;

                ReadChild(reader, runPr, false);
            }
        }

        /// <summary>
        /// Reads only text child elements of an w:rPr element. For read with formatting skipping.
        /// </summary>
        private void ReadTextElementsOnly(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("rPr"))
            {
                if ((xmlReader.LocalName == "t") || (xmlReader.LocalName == "r"))
                    ReadChild(reader, runPr, false);
                else
                    xmlReader.IgnoreElementNoWarn();
            }
        }

        private static void ReadW14Child(DocxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            switch (xmlReader.LocalName)
            {
                case "glow":
                    runPr.SetAttr(FontAttr.EffectGlow, DmlShapeEffectReader.Read(reader));
                    break;
                case "shadow":
                    runPr.SetAttr(FontAttr.EffectShadow, DmlShapeEffectReader.Read(reader));
                    break;
                case "reflection":
                    runPr.SetAttr(FontAttr.EffectReflection, DmlShapeEffectReader.Read(reader));
                    break;
                case "textOutline":
                    runPr.SetAttr(FontAttr.EffectOutline, DmlOutlineReader.Read(reader));
                    break;
                case "textFill":
                    runPr.SetAttr(FontAttr.EffectFill, DmlFillReader.ReadTextFill(reader));
                    break;
                case "scene3d":
                    runPr.SetAttr(
                        FontAttr.EffectScene3D,
                        DmlScene3DReader.ReadScene3DProperties(reader, complianceInfo));
                    break;
                case "props3d":
                    runPr.SetAttr(
                        FontAttr.EffectProps3D,
                        DmlScene3DReader.ReadShape3DProperties(reader, complianceInfo));
                    break;
                case "stylisticSets":
                    runPr.SetAttr(FontAttr.OpenTypeStylisticSets, ReadStylisticSets(reader));
                    break;
                case "ligatures":
                    runPr.SetAttr(FontAttr.OpenTypeLigature, DocxEnum.XmlToLigature(xmlReader.ReadVal()));
                    break;
                case "numForm":
                    runPr.SetAttr(FontAttr.OpenTypeNumForm, DocxEnum.XmlToNumForm(xmlReader.ReadVal()));
                    break;
                case "numSpacing":
                    runPr.SetAttr(FontAttr.OpenTypeNumSpacing, DocxEnum.XmlToNumSpacing(xmlReader.ReadVal()));
                    break;
                case "cntxtAlts":
                    runPr.SetAttr(FontAttr.OpenTypeContextualAlternates, xmlReader.ReadBoolVal());
                    break;
                default:
                    xmlReader.IgnoreElement();
                    break;
            }
        }

        /// <summary>
        /// Reads an element that can occur as a child of an w:rPr element or m:rPr element in case of Office Math.
        /// </summary>
        internal override void ReadChild(NrxDocumentReaderBase reader, RunPr runPr, bool resiliency)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            switch (xmlReader.LocalName)
            {
                case "rStyle":
                {
                    int istd = reader.ResolveStyleIdToIstd(xmlReader.ReadVal(), StyleIndex.DefaultParagraphFont);
                    runPr.SetAttr(FontAttr.Istd, istd);
                    break;
                }
                case "rFonts":
                    ReadFonts(xmlReader, runPr);
                    break;
                case "font":
                    if (!runPr.ContainsKey(FontAttr.NameAscii))
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameAscii, xmlReader.ReadVal(), false);
                    break;
                case "b":
                {
                    // andrnosk: RESILENCE WORDSNET-7513 Catching InvalidOperationException and write warning if there is incorrect on/off value.
                    try
                    {
                        AttrBoolEx exVal = xmlReader.ReadBoolExVal();
                        runPr.SetAttr(FontAttr.Bold, exVal);
                    }
                    catch (InvalidOperationException ex)
                    {
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Docx, ex.Message);
                    }

                    break;
                }
                case "bCs":
                    runPr.SetAttr(FontAttr.BoldBi, xmlReader.ReadBoolExVal());
                    break;
                case "i":
                    runPr.SetAttr(FontAttr.Italic, xmlReader.ReadBoolExVal());
                    break;
                case "iCs":
                    runPr.SetAttr(FontAttr.ItalicBi, xmlReader.ReadBoolExVal());
                    break;
                case "caps":
                    runPr.SetAttr(FontAttr.AllCaps, xmlReader.ReadBoolExVal());
                    break;
                case "smallCaps":
                    runPr.SetAttr(FontAttr.SmallCaps, xmlReader.ReadBoolExVal());
                    break;
                case "strike":
                    runPr.SetAttr(FontAttr.StrikeThrough, xmlReader.ReadBoolExVal());
                    break;
                case "dstrike":
                    runPr.SetAttr(FontAttr.DoubleStrikeThrough, xmlReader.ReadBoolExVal());
                    break;
                case "outline":
                    runPr.SetAttr(FontAttr.Outline, xmlReader.ReadBoolExVal());
                    break;
                case "shadow":
                    // WORDSNET-5431 Ignore w14 markup for shadow.
                    if (xmlReader.NamespaceURI != DocxNamespaces.GetNamespace(
                            DocxNamespace.W14Markup,
                            complianceInfo.Compliance == OoxmlComplianceCore.IsoStrict))
                        runPr.SetAttr(FontAttr.Shadow, xmlReader.ReadBoolExVal());
                    break;
                case "emboss":
                    runPr.SetAttr(FontAttr.Emboss, xmlReader.ReadBoolExVal());
                    break;
                case "imprint":
                    runPr.SetAttr(FontAttr.Engrave, xmlReader.ReadBoolExVal());
                    break;
                case "noProof":
                    runPr.SetAttr(FontAttr.NoProofing, xmlReader.ReadBoolExVal());
                    break;
                case "vanish":
                    runPr.SetAttr(FontAttr.Hidden, xmlReader.ReadBoolExVal());
                    break;
                case "snapToGrid":
                    runPr.SetAttr(FontAttr.SnapToGrid, xmlReader.ReadBoolExVal());
                    break;
                case "webHidden":
                    runPr.SetAttr(FontAttr.WebHidden, xmlReader.ReadBoolExVal());
                    break;
                case "color":
                    ReadRunPrColor(xmlReader, runPr);
                    break;
                case "spacing":
                    runPr.SetAttr(FontAttr.Spacing, xmlReader.ReadValAsTwips(complianceInfo));
                    break;
                case "w":
                {
                    // WORDSNET-18712 Empty tag should be ignored, not thrown upon.
                    string value = xmlReader.ReadVal();
                    if (StringUtil.HasChars(value))
                        runPr.SetAttr(FontAttr.Scaling, NrxXmlReader.XmlToPercent(value, complianceInfo));
                    break;
                }
                case "kern":
                    runPr.SetAttr(FontAttr.Kerning, xmlReader.ReadValAsHalfPoints(complianceInfo));
                    break;
                case "position":
                    runPr.SetAttr(FontAttr.Position, xmlReader.ReadValAsHalfPoints(complianceInfo));
                    break;
                case "sz":
                    if (!resiliency)
                        runPr.SetAttr(FontAttr.Size, xmlReader.ReadValAsHalfPoints(complianceInfo));
                    break;
                case "szCs":
                    if (!resiliency)
                        runPr.SetAttr(FontAttr.SizeBi, xmlReader.ReadValAsHalfPoints(complianceInfo));
                    break;
                case "highlight":
                    runPr.SetAttr(FontAttr.HighlightColor, NrxRunEnum.XmlToHighlight(xmlReader.ReadVal()));
                    break;
                case "u":
                    ReadUnderline(xmlReader, runPr);
                    break;
                case "effect":
                    runPr.SetAttr(FontAttr.TextEffect, NrxRunEnum.XmlToTextEffect(xmlReader.ReadVal()));
                    break;
                case "bdr":
                    runPr.SetAttr(FontAttr.Border, xmlReader.ReadBorder());
                    break;
                case "shd":
                    runPr.SetAttr(FontAttr.Shading, xmlReader.ReadShading());
                    break;
                case "fitText":
                {
                    int id = xmlReader.ReadIntAttribute("id", 0);
                    int val = xmlReader.ReadValAsTwips(complianceInfo);
                    runPr.SetAttr(FontAttr.FitText, new FitText(val, id));
                    break;
                }
                case "vertAlign":
                    runPr.SetAttr(
                        FontAttr.VerticalAlignment,
                        NrxRunEnum.XmlToRunVerticalAlignment(xmlReader.ReadVal()));
                    break;
                case "rtl":
                    runPr.SetAttr(FontAttr.Bidi, xmlReader.ReadBoolExVal());
                    break;
                case "cs":
                    runPr.SetAttr(FontAttr.ComplexScript, xmlReader.ReadBoolExVal());
                    break;
                case "em":
                    runPr.SetAttr(FontAttr.EmphasisMark, NrxRunEnum.XmlToEmphasisMark(xmlReader.ReadVal()));
                    break;
                case "lang":
                    ReadLang(xmlReader, runPr);
                    break;
                case "specVanish":
                    runPr.SetAttr(FontAttr.SpecialHidden, xmlReader.ReadBoolExVal());
                    break;
                case "rPrChange":
                    Debug.Assert(mAnnotationReader != null);
                    mAnnotationReader.ReadRunPrRevision(reader, runPr);
                    break;
                case "ins":
                    Debug.Assert(mAnnotationReader != null);
                    DocxAnnotationReader.ReadRunPrInsertion(xmlReader, runPr);
                    break;
                case "del":
                    Debug.Assert(mAnnotationReader != null);
                    DocxAnnotationReader.ReadRunPrDeletion(xmlReader, runPr);
                    break;
                case "moveFrom":
                    Debug.Assert(mAnnotationReader != null);
                    DocxAnnotationReader.ReadRunPrMoveFrom(xmlReader, runPr);
                    break;
                case "moveTo":
                    Debug.Assert(mAnnotationReader != null);
                    DocxAnnotationReader.ReadRunPrMoveTo(xmlReader, runPr);
                    break;
                case "lit":
                    runPr.MathIsLiteral = xmlReader.ReadBoolVal();
                    break;
                case "nor":
                    runPr.MathIsNormalText = xmlReader.ReadBoolVal();
                    break;
                case "scr":
                    runPr.MathScript = DocxEnum.DocxToMathScriptType(xmlReader.ReadVal());
                    break;
                case "sty":
                    runPr.MathStyle = DocxEnum.DocxToMathStyleType(xmlReader.ReadVal());
                    break;
                case "brk":
                    runPr.MathLineBreak = NrxMathReaderUtil.ReadMathLineBreak(xmlReader);
                    break;
                case "aln":
                    runPr.MathIsAlignmentPoint = xmlReader.ReadBoolVal();
                    break;
                case "oMath":
                    runPr.IsOMath = xmlReader.ReadBoolVal();
                    break;
                case "eastAsianLayout":
                    ReadFarEastLayout(runPr, xmlReader);
                    break;
                case "pStyle":
                case "textAlignment":
                    xmlReader.IgnoreElementUnexpected(true);
                    break;
                case "instrText":
                case "t":
                {
                    DocxReaderFactory.RunReader.ReadText(reader, runPr);
                    NrxRunReaderBase.FlushRun(reader, runPr);
                    break;
                }
                case "rPr":
                    Read(reader, runPr);
                    break;
                case "r":
                {
                    DocxReaderFactory.RunReader.ReadTextLevelElement(reader, runPr);
                    break;
                }
                case "fldChar":
                    Debug.Assert(mFldCharReader != null);
                    if (mFldCharReader != null)
                    {
                        // RESILIENCY 25828  - a "fldChar" tag is found inside "rPr" element.
                        NrxRunReaderBase.FlushRun(reader, runPr);
                        mFldCharReader.Read(reader, runPr);
                    }

                    break;
                case "AlternateContent":
                    // WORDSNET-27021 Handle AlternateContent for run properties.
                    ReadAlternateContent(reader, runPr);
                    break;

                default:
                    xmlReader.IgnoreElement();
                    break;
            }
        }

        private void ReadAlternateContent(NrxDocumentReaderBase reader, RunPr runPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("AlternateContent"))
            {
                switch (xmlReader.LocalName)
                {
                    case "Fallback":
                    {
                        while (xmlReader.ReadChild("Fallback"))
                        {
                            ReadChild(reader, runPr, false);
                        }

                        break;
                    }

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        internal override void ReadFonts(NrxXmlReader xmlReader, RunPr runPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "ascii":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameAscii, xmlReader.Value, false);
                        break;
                    case "hAnsi":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameOther, xmlReader.Value, false);
                        break;
                    case "eastAsia":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameFarEast, xmlReader.Value, false);
                        break;
                    case "cs":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameBi, xmlReader.Value, false);
                        break;
                    case "hint":
                        runPr.SetAttr(FontAttr.CharacterCategoryHint, NrxRunEnum.XmlToHint(xmlReader.Value));
                        break;
                    case "asciiTheme":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameAscii, xmlReader.Value, true);
                        break;
                    case "hAnsiTheme":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameOther, xmlReader.Value, true);
                        break;
                    case "eastAsiaTheme":
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameFarEast, xmlReader.Value, true);
                        break;
                    case "cstheme": // This is the correct one.
                    case "csTheme": // But let's read this too.
                        NrxXmlUtil.SetFontNameIfHasChars(runPr, FontAttr.NameBi, xmlReader.Value, true);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Reads and sets eastAsianLayout element.
        /// </summary>
        /// <param name="runPr"></param>
        /// <param name="reader"></param>
        private static void ReadFarEastLayout(RunPr runPr, NrxXmlReader reader)
        {
            FarEastLayout feLayout = new FarEastLayout();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "id":
                        feLayout.FarEastLayoutId = reader.ValueAsInt;
                        break;
                    case "vert":
                        feLayout.Vertical = reader.ValueAsBool;
                        break;
                    case "vertCompress":
                        feLayout.VerticalCompress = reader.ValueAsBool;
                        break;
                    case "combine":
                        feLayout.Combine = reader.ValueAsBool;
                        break;
                    case "combineBrackets":
                        feLayout.CombineBrackets = NrxRunEnum.XmlToCombineBrackets(reader.Value);
                        break;
                    default:
                        break;
                }
            }

            runPr.FarEastLayout = feLayout;
        }

        private static void ReadRunPrColor(NrxXmlReader xmlReader, RunPr runPr)
        {
            // WORDSNET-23566 Remove color-related attributes that may have already been read before to mimic Word.
            runPr.Remove(FontAttr.Color);
            runPr.Remove(FontAttr.ThemeColor);
            runPr.Remove(FontAttr.ThemeShade);
            runPr.Remove(FontAttr.ThemeTint);

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "val":
                        runPr.SetAttr(FontAttr.Color, NrxXmlUtil.XmlToColor(xmlReader.Value));
                        break;
                    case "themeColor":
                        runPr.SetAttr(FontAttr.ThemeColor, xmlReader.Value);
                        break;
                    case "themeShade":
                        runPr.SetAttr(FontAttr.ThemeShade, xmlReader.Value);
                        break;
                    case "themeTint":
                        runPr.SetAttr(FontAttr.ThemeTint, xmlReader.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ReadUnderline(NrxXmlReader xmlReader, RunPr runPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "val":
                        runPr.SetAttr(FontAttr.Underline, NrxRunEnum.XmlToUnderline(xmlReader.Value));
                        break;
                    case "color":
                        runPr.SetAttr(FontAttr.UnderlineColor, NrxXmlUtil.XmlToColor(xmlReader.Value));
                        break;
                    case "themeColor":
                        runPr.SetAttr(FontAttr.UnderlineThemeColor, xmlReader.Value);
                        break;
                    case "themeShade":
                        runPr.SetAttr(FontAttr.UnderlineThemeShade, xmlReader.Value);
                        break;
                    case "themeTint":
                        runPr.SetAttr(FontAttr.UnderlineThemeTint, xmlReader.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ReadLang(NrxXmlReader xmlReader, RunPr runPr)
        {
            bool isLocaleIdSpecified = false;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "val":
                        SetLocale(runPr, FontAttr.LocaleId, xmlReader.Value);
                        isLocaleIdSpecified = true;
                        break;
                    case "eastAsia":
                        SetLocale(runPr, FontAttr.LocaleIdFarEast, xmlReader.Value);
                        isLocaleIdSpecified = true;
                        break;
                    case "bidi":
                        SetLocale(runPr, FontAttr.LocaleIdBi, xmlReader.Value);
                        isLocaleIdSpecified = true;
                        break;
                    default:
                        break;
                }
            }

            // Set LocaleId to some invalid value to ensure correctness of the roundtrip.
            if (!isLocaleIdSpecified)
                runPr.SetAttr(FontAttr.LocaleId, (int)Language.LanguageNotSet);
        }

        /// <summary>
        /// Sets locale considering LanguageNotSet.
        /// </summary>
        /// <remarks>
        /// See TestJira15005() for details.
        /// </remarks>
        private static void SetLocale(RunPr runPr, int key, string value)
        {
            if (value == "x-none")
                runPr.SetAttr(key, Language.LanguageNotSet);
            else
                runPr.SetAttr(key, LocaleConverter.DocxTagToLocale(value));
        }

        internal void SetAnnotationReader(DocxAnnotationReader annotationReader)
        {
            Debug.Assert(annotationReader != null);
            mAnnotationReader = annotationReader;
        }

        /// <summary>
        /// Reads the 'w14:stylisticSets' element.
        /// </summary>
        private static StylisticSets ReadStylisticSets(DocxDocumentReaderBase reader)
        {
            Debug.Assert(reader != null);

            StylisticSets stylisticSets = StylisticSets.Default;
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                if ((xmlReader.LocalName == "styleSet") && xmlReader.ReadBoolVal())
                    stylisticSets |= DocxEnum.XmlToStylisticSets(xmlReader.ReadId());
            }

            return stylisticSets;
        }

        private DocxAnnotationReader mAnnotationReader;
        private readonly DocxFldCharReader mFldCharReader;
    }
}
