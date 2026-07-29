// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/21/2014 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Text.Bullets;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// 20.1.2.2.41 txSp (Text Shape)
    /// Represents class writing a text shape within a parent shape. This text shape is specifically used for
    /// displaying text as it has only text related child elements.
    /// </summary>
    internal static class DmlTextShapeWriter
    {
        internal static void Write(DmlShape dmlShape, IDmlShapeWriterContext writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            NrxXmlBuilder builder = writer.Builder;
            DmlTextShape txShape = dmlShape.TextShape;

            if (txShape == null)
                return;

            switch (dmlShape.DmlNodeType)
            {
                case DmlNodeType.Shape:
                {
                    builder.StartElement("a:txSp");
                    WriteDmlShapeTextBody("a:txBody", txShape.TextBody, writer);
                    if (txShape.UseShapeTextRectangle)
                        builder.WriteEmptyElement("a:useSpRect");

                    builder.EndElement("a:txSp");
                    break;
                }
                case DmlNodeType.WordprocessingShape:
                {
                    string tagName = isIsoStrict ? "wp:bodyPr" : "wps:bodyPr";
                    WriteDmlTextShapeBodyPr(tagName, txShape.TextBody.Properties, writer);
                    break;
                }
                default:
                    throw new ArgumentException("Unexpected Dml node type.");
            }
        }

        internal static void WriteDmlShapeTextBody(
            string rootTagName, 
            DmlTextBody textBody, 
            IDmlShapeWriterContext writer)
        {
            if (textBody == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(rootTagName);

            WriteDmlTextShapeBodyPr("a:bodyPr", textBody.Properties, writer);
            WriteDmlTextListStyles(textBody.TextListStyles, writer);

            foreach (DmlParagraph paragraph in textBody.Paragraphs)
                WriteParagraph(paragraph, writer);

            builder.EndElement(rootTagName);
        }

        internal static void WriteParagraph(DmlParagraph paragraph, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement("a:p");

            WriteDmlParagraphProperties("a:pPr", paragraph.Properties, writer);
            WriteDmlParagraphTextElements("a", paragraph.Elements, writer);
            if (!paragraph.EndParagraphRunProperties.IsEmpty)
                WriteDmlRunProperties("a:endParaRPr", paragraph.EndParagraphRunProperties, writer);

            builder.EndElement("a:p");
        }

        private static void WriteDmlParagraphTextElements(string prefix, 
            IList<DmlParagraphTextElementBase> textElements, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            foreach (DmlParagraphTextElementBase textElement in textElements)
            {
                DmlTextBreakLine textBreak = textElement as DmlTextBreakLine;
                if (textBreak != null)
                {
                    builder.StartElement("a:br");
                    WriteDmlRunProperties("a:rPr", textBreak.RunProperties, writer);
                    builder.EndElement("a:br");
                }

                DmlTextField textField = textElement as DmlTextField;
                if (textField != null)
                {
                    builder.StartElement("a:fld");
                    builder.WriteAttribute("id", textField.Id);
                    if (StringUtil.HasChars(textField.Type))
                        builder.WriteAttribute("type", textField.Type);
                    // rPr must come before pPr to take precedence over it.
                    WriteDmlRunProperties("a:rPr", textField.RunProperties, writer);
                    WriteDmlParagraphProperties("a:pPr", textField.ParagraphProperties, writer);
                    WriteText("a", textField.Text, builder);

                    builder.EndElement("a:fld");
                }

                DmlRun textRun = textElement as DmlRun;

                if (textRun != null)
                {
                    string tagName = string.Format("{0}:r", prefix);

                    builder.StartElement(tagName);

                    WriteDmlRunProperties("a:rPr", textRun.RunProperties, writer);
                    WriteText(prefix, textRun.Text, builder);

                    builder.EndElement(tagName);
                }

                DmlTextMath textMath = textElement as DmlTextMath;
                if (textMath != null)
                {
                    if (textMath.IsRootElement)
                    {
                        builder.StartElement("a14:m");
                        builder.WriteAttribute("xmlns:a14", DmlExtensionsNamespace.DrawingMain);
                        DocxNamespaces namespaces = 
                            new DocxNamespaces(writer.Compliance == OoxmlComplianceCore.IsoStrict);
                        builder.WriteAttribute("xmlns:m", namespaces.Math);
                    }

                    DocxMathWriter.WriteStart(textMath.MathObject, textMath.RunProperties, (IMathWriterContext)writer);

                    WriteDmlParagraphTextElements("m", textMath.Elements, writer);

                    DocxMathWriter.WriteEnd(builder);

                    if (textMath.IsRootElement)
                        builder.EndElement("a14:m");
                }
            }
        }

        private static void WriteText(string prefix, string text, NrxXmlBuilder builder)
        {
            string tagName = string.Format("{0}:t", prefix);
            builder.StartElement(tagName);
            builder.WriteString(text);
            builder.EndElement(tagName);
        }

        internal static void WriteDmlTextListStyles(DmlTextListStyles textListStyles, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement("a:lstStyle");

            WriteDmlParagraphProperties("a:defPPr", textListStyles.DefaultParagraphProperties, writer);
            WriteDmlParagraphProperties("a:lvl1pPr", textListStyles.ListLevel1Style, writer);
            WriteDmlParagraphProperties("a:lvl2pPr", textListStyles.ListLevel2Style, writer);
            WriteDmlParagraphProperties("a:lvl3pPr", textListStyles.ListLevel3Style, writer);
            WriteDmlParagraphProperties("a:lvl4pPr", textListStyles.ListLevel4Style, writer);
            WriteDmlParagraphProperties("a:lvl5pPr", textListStyles.ListLevel5Style, writer);
            WriteDmlParagraphProperties("a:lvl6pPr", textListStyles.ListLevel6Style, writer);
            WriteDmlParagraphProperties("a:lvl7pPr", textListStyles.ListLevel7Style, writer);
            WriteDmlParagraphProperties("a:lvl8pPr", textListStyles.ListLevel8Style, writer);
            WriteDmlParagraphProperties("a:lvl9pPr", textListStyles.ListLevel9Style, writer);

            //Currently we do not read these elements.
            //extLst (Extension List) §20.1.2.2.15

            builder.EndElement("a:lstStyle");
        }

        private static void WriteDmlParagraphProperties(
            string elementName, 
            DmlParagraphProperties props, 
            IDmlShapeWriterContext writer)
        {
            if ((props == null) || props.IsEmpty)
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(elementName);

            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.LeftMargin, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.RightMargin, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.Level, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.TextIdentation, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.Alignment, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.DefaultTabSize, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.RightToLeftFlowDirection, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.IsEastAsianLineBreakAllowed, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.FontAlignment, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.IsLatinLineBreakAllowed, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.IsHangingPunctuationAllowed, props, writer);

            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.LineSpacing, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.SpaceBefore, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.SpaceAfter, props, writer);

            // Write bullet properties.
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.BulletColor, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.BulletSize, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.BulletFont, props, writer);
            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.Bullet, props, writer);

            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.TabList, props, writer);

            if (props.HasDefaultRunProperties)
                WriteDmlRunProperties("a:defRPr", props.DefaultRunProperties, writer);

            WriteDmlParagraphPropertiesIfNotNull(DmlParagraphPropertiesIds.Extensions, props, writer);

            //Currently we do not read these elements.
            //buAutoNum (Auto-Numbered Bullet) §21.1.2.4.1
            //buBlip (Picture Bullet)
            //buChar (Character Bullet) §21.1.2.4.3
            //buClr (Color Specified) §21.1.2.4.4
            //buClrTx (Follow Text) §21.1.2.4.5
            //buFont (Specified) §21.1.2.4.6
            //buFontTx (Follow text) §21.1.2.4.7
            //buNone (No Bullet) §21.1.2.4.8
            //buSzPct (Bullet Size Percentage) §21.1.2.4.9
            //buSzPts (Bullet Size Points) §21.1.2.4.10
            //buSzTx (Bullet Size Follows Text) §21.1.2.4.11

            builder.EndElement(elementName);
        }

        private static void WriteDmlTextSpacing(string elementName, DmlTextSpacing spacing, IDmlShapeWriterContext writer)
        {
            if (spacing == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);

            builder.StartElement(elementName);

            DmlPercentageTextSpacing percentageSpacing = spacing as DmlPercentageTextSpacing;
            if (percentageSpacing != null)
                builder.WriteElementWithAttributes("a:spcPct", "val", 
                    DmlPercentageUtil.ToPercentOrDmlPercent(percentageSpacing.Value, isIsoStrict));

            DmlPointsTextSpacing pointSpacing = spacing as DmlPointsTextSpacing;
            if (pointSpacing != null)
                builder.WriteElementWithAttributes("a:spcPts", "val", pointSpacing.Value.Value);

            builder.EndElement(elementName);
        }

        internal static void WriteDmlRunProperties(string elementName, DmlRunProperties props,
            IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement(elementName);

            // Order of attributes is the same as in scheme.
            builder.WriteAttribute("kumimoji", props.GetDirectProperty(DmlRunPropertiesIds.Kumimoji));

            WriteLanguageAttributeIfNotNull("lang", DmlRunPropertiesIds.Language, props, builder);
            WriteLanguageAttributeIfNotNull("altLang", DmlRunPropertiesIds.AlternativeLanguage, props, builder);

            if (props.IsPropertySpecified(DmlRunPropertiesIds.FontSize))
                builder.WriteAttribute("sz", props.FontSize.Value);
            builder.WriteAttribute("b", props.GetDirectProperty(DmlRunPropertiesIds.Bold));
            builder.WriteAttribute("i", props.GetDirectProperty(DmlRunPropertiesIds.Italics));
            if (props.IsPropertySpecified(DmlRunPropertiesIds.Underline))
                builder.WriteAttribute("u", DmlEnum.TextUnderlineTypeToDml(props.Underline));
            if (props.IsPropertySpecified(DmlRunPropertiesIds.Strikethrough))
                builder.WriteAttribute("strike", DmlEnum.TextStrikeToDml(props.Strikethrough));
            if (props.IsPropertySpecified(DmlRunPropertiesIds.Kerning))
                builder.WriteAttribute("kern", props.Kerning.Value);
            if (props.IsPropertySpecified(DmlRunPropertiesIds.Capitalization))
                builder.WriteAttribute("cap", DmlEnum.CapitalizationToDml(props.Capitalization));
            if (props.IsPropertySpecified(DmlRunPropertiesIds.Spacing))
                builder.WriteAttribute("spc", props.Spacing.Value);
            builder.WriteAttribute("normalizeH", props.GetDirectProperty(DmlRunPropertiesIds.NormalizeHeights));
            if (props.IsPropertySpecified(DmlRunPropertiesIds.Baseline))
                builder.WriteAttribute("baseline", DmlPercentageUtil.ToPercentOrDmlPercent(props.Baseline, isIsoStrict));
            builder.WriteAttribute("noProof", props.GetDirectProperty(DmlRunPropertiesIds.NoProofing));
            builder.WriteAttribute("dirty", props.GetDirectProperty(DmlRunPropertiesIds.IsDirty));
            builder.WriteAttribute("err", props.GetDirectProperty(DmlRunPropertiesIds.HasSpellingError));
            builder.WriteAttribute("smtClean", props.GetDirectProperty(DmlRunPropertiesIds.SmartTagsClean));
            builder.WriteAttributeUInt("smtId", props.GetDirectProperty(DmlRunPropertiesIds.SmartTagID));
            builder.WriteAttribute("bmk", props.GetDirectProperty(DmlRunPropertiesIds.BookmarkLinkTarget));

            // The following objects are written as child elements.
            DmlOutlineWriter.Write("a:ln", (DmlOutline)props.GetDirectProperty(DmlRunPropertiesIds.Outline), writer);
            DmlFillWriter.Write((DmlFill)props.GetDirectProperty(DmlRunPropertiesIds.Fill), writer, false);
            DmlShapeEffectsWriter.Write(props.Effects, writer, false);
            WriteHighlightColor(props, writer);
           
            if (props.IsPropertySpecified(DmlRunPropertiesIds.UnderlineStrokeTx))
                builder.WriteEmptyElement("a:uLnTx");
            else
                DmlOutlineWriter.Write("a:uLn", (DmlOutline)props.GetDirectProperty(DmlRunPropertiesIds.UnderlineStroke), writer);

            WriteUnderlineFillTx(props, writer);
            WriteDmlFont("a:latin", (DmlFont)props.GetDirectProperty(DmlRunPropertiesIds.LatinFont), builder);
            WriteDmlFont("a:ea", (DmlFont)props.GetDirectProperty(DmlRunPropertiesIds.EastAsianFont), builder);
            WriteDmlFont("a:cs", (DmlFont)props.GetDirectProperty(DmlRunPropertiesIds.ComplexScriptFont), builder);
            WriteDmlFont("a:sym", (DmlFont)props.GetDirectProperty(DmlRunPropertiesIds.SymbolFont), builder);

            DmlHlink hlinkClick = (DmlHlink)props.GetDirectProperty(DmlRunPropertiesIds.HlinkClick);
            if (hlinkClick != null)
                DmlHlinkWriter.WriteHlink("a:hlinkClick", hlinkClick.Id, hlinkClick.TargetFrame, hlinkClick.Tooltip,
                    hlinkClick.Extensions, writer);

            DmlHlink hlinkHover = (DmlHlink)props.GetDirectProperty(DmlRunPropertiesIds.HlinkHover);
            if (hlinkHover != null)
                DmlHlinkWriter.WriteHlink("a:hlinkHover", hlinkHover.Id, hlinkHover.TargetFrame, hlinkHover.Tooltip,
                    hlinkHover.Extensions, writer);

            if (props.IsPropertySpecified(DmlRunPropertiesIds.RightToLeftFlowDirection))
                builder.WriteEmptyElement("a:rtl");

            StringToObjDictionary<DmlExtension> extLst = (StringToObjDictionary<DmlExtension>)props.GetDirectProperty(DmlRunPropertiesIds.Extensions);
            if (extLst != null)
                DmlExtensionListWriter.Write(extLst, writer);

            // Currently do not read these elements.
            // effectDag (Effect Container) §20.1.8.25

            builder.EndElement(elementName);
        }

        /// <summary>
        /// Writes "UnderlineFillTx"
        /// </summary>
        private static void WriteUnderlineFillTx(DmlRunProperties props, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            if (props.IsPropertySpecified(DmlRunPropertiesIds.UnderlineFillTx))
            {
                builder.WriteEmptyElement("a:uFillTx");
            }
            else
            {
                DmlFill underlineFill = (DmlFill)props.GetDirectProperty(DmlRunPropertiesIds.UnderlineFill);
                if (underlineFill != null)
                {
                    builder.StartElement("a:uFill");
                    DmlFillWriter.Write(underlineFill, writer, false);
                    builder.EndElement("a:uFill");
                }
            }
        }

        /// <summary>
        /// Writes the highlight color property if specified.
        /// </summary>
        /// <param name="props">The specified <see cref="DmlRunProperties"/></param>
        /// <param name="writer">The specified <see cref="IDmlShapeWriterContext"/></param>
        private static void WriteHighlightColor(DmlRunProperties props, IDmlShapeWriterContext writer)
        {
            if (!props.IsPropertySpecified(DmlRunPropertiesIds.HighlightColor))
                return;

            string highlightElementName = "a:highlight";
            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(highlightElementName);
            DmlColorWriter.Write((DmlColor)props.GetDirectProperty(DmlRunPropertiesIds.HighlightColor), writer);
            builder.EndElement(highlightElementName);
        }

        private static void WriteDmlFont(string elementName, DmlFont font, NrxXmlBuilder builder)
        {
            if (font == null)
                return;

            builder.StartElement(elementName);
            builder.WriteAttributeString("typeface", font.TextTypeface);
            builder.WriteAttribute("panose", font.PanoseSetting);
            builder.WriteAttributeIfNotZero("pitchFamily", font.SimilarFontFamily);
            if (font.SimilarCharacterSet != 1)
                builder.WriteAttribute("charset", font.SimilarCharacterSet);
            builder.EndElement(elementName);
        }

        internal static void WriteDmlTextShapeBodyPr(string tagName, DmlTextBodyProperties props,
            IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement(tagName);

            // The following code writes bodyPr attributes in order specified in Dml scheme.
            // Ms Word uses the same order.
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.Rotation, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.AreFirstAndLastParagraphsUseSpacing, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.TextVerticalOverflow, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.TextHorizontalOverflow, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.TextOrientation, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.TextWrappingType, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.LeftInset, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.TopInset, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.RightInset, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.BottomInset, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.ColumnNumber, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.SpaceBetweenColumns, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.ColumnOrder, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.FromWordArt, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.Anchor, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.AnchorCenter, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.ForceAntiAlias, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.IsTextUpright, props, builder);
            WriteBodyPrAttributeIfNotNull(DmlTextBodyPropertiesDefaultsIds.UseCompatibleLineSpacing, props, builder);

            if (props.PresetTextWrap.TextShapeType != DmlTextShapeType.None)
            {
                builder.StartElement("a:prstTxWarp");
                builder.WriteAttribute("prst", DmlEnum.TextShapeTypeToDml(props.PresetTextWrap.TextShapeType));
                DmlGeomertyWriter.WriteGuides(props.PresetTextWrap.Guides.AdjustableValues, builder, "a:avLst");
                builder.EndElement("a:prstTxWarp");
            }

            // Also "a:noAutofit" might persist, but it is default value and usually is omitted.
            if (props.AutoFitMode is DmlShapeAutoFitMode)
                builder.WriteEmptyElement("a:spAutoFit");

            DmlNormalAutoFitMode normalAutoFit = props.AutoFitMode as DmlNormalAutoFitMode;
            if (normalAutoFit != null)
            {
                builder.WriteElementWithAttributes("a:normAutofit",
                    "fontScale",
                    DmlPercentageUtil.ToPercentOrDmlPercent(normalAutoFit.FontScale, isIsoStrict),
                    "lnSpcReduction",
                    DmlPercentageUtil.ToPercentOrDmlPercent(normalAutoFit.LineSpaceReduction, isIsoStrict));
            }

            Dml3DPropertiesWriter.WriteScene3D(props.Scene3DProperties, writer, false);
            Dml3DPropertiesWriter.WriteShape3D(props.Shape3DProperties, writer, false);

            // Write extLst (Extension List) §20.1.2.2.15
            DmlExtensionListWriter.Write(props.Extensions, writer);

            // We do not read these child nodes.
            //flatTx (No text in 3D scene) §20.1.5.8
            //prstTxWarp (Preset Text Warp) §20.1.9.19

            builder.EndElement(tagName);
        }

        private static void WriteDmlParagraphPropertiesIfNotNull(
            DmlParagraphPropertiesIds id,
            DmlParagraphProperties props,
            IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            object value = props.GetDirectProperty(id);
            if (value == null)
                return;

            switch (id)
            {
                case DmlParagraphPropertiesIds.Alignment:
                {
                    if (DmlEnum.IsNotSupportedValue(props.Alignment))
                    {
                        //Explicit Enum.ToString() added for Java.
                        writer.Warn(WarningType.MinorFormattingLoss, WarningSource.DrawingML,
                            string.Format(WarningStrings.UnsupportedDmlTextAlignment, props.Alignment.ToString(),
                                DmlEnum.FixUnsupportedValue(props.Alignment).ToString()));
                    }

                    builder.WriteAttribute("algn", DmlEnum.TextAlignmentToDml(props.Alignment));
                    break;
                }
                case DmlParagraphPropertiesIds.DefaultTabSize:
                    builder.WriteAttribute("defTabSz", value);
                    break;
                case DmlParagraphPropertiesIds.IsEastAsianLineBreakAllowed:
                    builder.WriteAttribute("eaLnBrk", value);
                    break;
                case DmlParagraphPropertiesIds.FontAlignment:
                    builder.WriteAttribute("fontAlgn", DmlEnum.FontAlignmentToDml(props.FontAlignment));
                    break;
                case DmlParagraphPropertiesIds.IsHangingPunctuationAllowed:
                    builder.WriteAttribute("hangingPunct", value);
                    break;
                case DmlParagraphPropertiesIds.TextIdentation:
                    builder.WriteAttribute("indent", value);
                    break;
                case DmlParagraphPropertiesIds.IsLatinLineBreakAllowed:
                    builder.WriteAttribute("latinLnBrk", value);
                    break;
                case DmlParagraphPropertiesIds.LeftMargin:
                    builder.WriteAttribute("marL", value);
                    break;
                case DmlParagraphPropertiesIds.RightMargin:
                    builder.WriteAttribute("marR", value);
                    break;
                case DmlParagraphPropertiesIds.RightToLeftFlowDirection:
                    builder.WriteAttribute("rtl", value);
                    break;
                case DmlParagraphPropertiesIds.Level:
                    builder.WriteAttribute("lvl", value);
                    break;
                case DmlParagraphPropertiesIds.LineSpacing:
                    WriteDmlTextSpacing("a:lnSpc", (DmlTextSpacing)value, writer);
                    break;
                case DmlParagraphPropertiesIds.SpaceAfter:
                    WriteDmlTextSpacing("a:spcAft", (DmlTextSpacing)value, writer);
                    break;
                case DmlParagraphPropertiesIds.SpaceBefore:
                    WriteDmlTextSpacing("a:spcBef", (DmlTextSpacing)value, writer);
                    break;
                case DmlParagraphPropertiesIds.BulletColor:
                    WriteBulletColor((DmlTextBulletColor)value, writer);
                    break;
                case DmlParagraphPropertiesIds.BulletSize:
                    WriteBulletSize((DmlTextBulletSize)value, writer);
                    break;
                case DmlParagraphPropertiesIds.BulletFont:
                    WriteBulletFont((DmlTextBulletFont)value, builder);
                    break;
                case DmlParagraphPropertiesIds.Bullet:
                    WriteBullet((DmlTextBullet)value, writer);
                    break;
                case DmlParagraphPropertiesIds.TabList:
                    WriteTabList((TabStopCollection)value, writer);
                    break;
                case DmlParagraphPropertiesIds.Extensions:
                    DmlExtensionListWriter.Write((StringToObjDictionary<DmlExtension>)value, writer);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Writes a:tabLst element using specified TabStops.
        /// </summary>
        private static void WriteTabList(TabStopCollection tabs, IDmlShapeWriterContext writer)
        {
            if ((tabs == null) || (tabs.Count == 0))
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement("a:tabLst");

            for (int i = 0; i < tabs.Count; i++)
            {
                TabStop tabStop = tabs[i];
                string alignment = DmlEnum.TabAlignmentToDml(tabStop.Alignment);
                builder.WriteElementWithAttributes("a:tab", "pos", tabStop.PositionTwips, "algn", alignment);
            }

            builder.EndElement(); //a:tabLst
        }

        private static void WriteBullet(DmlTextBullet value, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            if (!value.HasBullet)
            {
                builder.WriteEmptyElement("a:buNone");
            }
            else if (value.IsCharBullet)
            {
                builder.WriteElementWithAttributes("a:buChar", "char", value.BulletChar);
            }
            else if (value.IsAutoNumBullet)
            {
                builder.WriteElementWithAttributesStart("a:buAutoNum", "type", DmlEnum.TextBulletTypeToDml(value.BulletType));
                if (value.StartAt != 1)
                    builder.WriteAttribute("startAt", value.StartAt);

                builder.EndElement("a:buAutoNum");
            }
            else if(value.IsPictureBullet)
            {
                builder.StartElement("a:buBlip");
                DmlFillWriter.WriteBlip(value.PictureBullet, writer, "a:blip");
                builder.EndElement("a:buBlip");
            }
        }

        private static void WriteBulletFont(DmlTextBulletFont value, NrxXmlBuilder builder)
        {
            if (value.FollowText)
                builder.WriteEmptyElement("a:buFontTx");
            else
                WriteDmlFont("a:buFont", value.Font, builder);
        }

        private static void WriteBulletSize(DmlTextBulletSize value, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);

            switch (value.SizeType)
            {
                case DmlTextBulletSizeType.FolowText:
                    builder.WriteEmptyElement("a:buSzTx");
                    break;
                case DmlTextBulletSizeType.Percentage:
                    double asFraction = DmlPercentageUtil.FromDmlPercent(value.Value);
                    builder.WriteElementWithAttributes("a:buSzPct", "val", 
                        DmlPercentageUtil.ToPercentOrDmlPercent(asFraction, isIsoStrict));
                    break;
                case DmlTextBulletSizeType.Points:
                    builder.WriteElementWithAttributes("a:buSzPts", "val", value.Value);
                    break;
                default:
                    break;
            }
        }

        private static void WriteBulletColor(DmlTextBulletColor value, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;
            if (value.FollowText)
            {
                builder.WriteEmptyElement("a:buClrTx");
            }
            else
            {
                builder.StartElement("a:buClr");
                DmlColorWriter.Write(value.Color, writer);
                builder.EndElement("a:buClr");
            }
        }

        private static void WriteBodyPrAttributeIfNotNull(
            DmlTextBodyPropertiesDefaultsIds id,
            DmlTextBodyProperties props, 
            NrxXmlBuilder builder)
        {
            object value = props.GetDirectProperty(id);

            // MS Word writes the value -60000000 indicating that the default rotation (0) is used.
            // The value can only be used with unspecified or horizontal orientation.
            if ((value == null) && (id == DmlTextBodyPropertiesDefaultsIds.Rotation) && props.HasDefaultRotation)
                value = new DmlAngle((props.TextOrientation == ShapeTextOrientation.Horizontal) ? -60000000 : 0);

            if (value == null)
                return;

            switch (id)
            {
                case DmlTextBodyPropertiesDefaultsIds.Anchor:
                    builder.WriteAttribute("anchor", DmlEnum.TextAnchoringTypeToDml(props.Anchor));
                    break;
                case DmlTextBodyPropertiesDefaultsIds.AnchorCenter:
                    builder.WriteAttribute("anchorCtr", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.AreFirstAndLastParagraphsUseSpacing:
                    builder.WriteAttribute("spcFirstLastPara", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.ColumnNumber:
                    builder.WriteAttribute("numCol", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.LeftInset:
                    builder.WriteAttribute("lIns", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.TopInset:
                    builder.WriteAttribute("tIns", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.RightInset:
                    builder.WriteAttribute("rIns", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.BottomInset:
                    builder.WriteAttribute("bIns", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.ColumnOrder:
                    builder.WriteAttribute("rtlCol", (props.ColumnOrder == DmlTextColumnOrder.RightToLeft));
                    break;
                case DmlTextBodyPropertiesDefaultsIds.ForceAntiAlias:
                    builder.WriteAttribute("forceAA", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.FromWordArt:
                    builder.WriteAttribute("fromWordArt", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.IsTextUpright:
                    builder.WriteAttributeIfTrue("upright", props.IsTextUpright);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.Rotation:
                    builder.WriteAttribute("rot", ((DmlAngle)value).Value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.SpaceBetweenColumns:
                    builder.WriteAttribute("spcCol", value);
                    break;
                case DmlTextBodyPropertiesDefaultsIds.TextHorizontalOverflow:
                    builder.WriteAttribute("horzOverflow", DmlEnum.TextHorizontalOverflowTypeToDml(props.TextHorizontalOverflow));
                    break;
                case DmlTextBodyPropertiesDefaultsIds.TextVerticalOverflow:
                    builder.WriteAttribute("vertOverflow", DmlEnum.TextVerticalOverflowTypeToDml(props.TextVerticalOverflow));
                    break;
                case DmlTextBodyPropertiesDefaultsIds.TextOrientation:
                    builder.WriteAttribute("vert", DmlEnum.ShapeTextOrientationToDml(props.TextOrientation));
                    break;
                case DmlTextBodyPropertiesDefaultsIds.TextWrappingType:
                    builder.WriteAttribute("wrap", DmlEnum.TextWrappingTypeToDml(props.TextWrappingType));
                    break;
                case DmlTextBodyPropertiesDefaultsIds.UseCompatibleLineSpacing:
                    builder.WriteAttribute("compatLnSpc", value);
                    break;
                default:
                    break;
            }
        }

        private static void WriteLanguageAttributeIfNotNull(string attr, DmlRunPropertiesIds id,
            DmlRunProperties props,  NrxXmlBuilder builder)
        {
            object value = props.GetDirectProperty(id);
            if (value != null)
                builder.WriteAttribute(attr, LocaleConverter.LocaleToDocxTag((int)value));
        }
    }
}
