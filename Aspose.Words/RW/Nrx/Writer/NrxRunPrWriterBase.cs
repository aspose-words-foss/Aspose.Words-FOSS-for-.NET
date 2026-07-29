// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2010 by Roman Korchagin

using Aspose.Drawing;
using Aspose.Words.Math;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Themes;

namespace Aspose.Words.RW.Nrx.Writer
{
    internal abstract class NrxRunPrWriterBase
    {
        /// <summary>
        /// Writes a run formatting properties into the writer in DOCX and WML formats.
        /// RK I don't like writePPrElementStart here.
        /// NOTE: this class should be immutable (state cannot be modified after it is created).
        /// </summary>
        /// <param name="runPr">Run properties to write.</param>
        /// <param name="attrSource">The attribute source that is used to resolve full values of BoolEx attributes.</param>
        /// <param name="writePPrElementStart">Write w:pPr element start before writing w:rPr element. Required to avoid writing empty w:pPr declarations (cosmetic).</param>
        /// <param name="writer">The writer to write to.</param>
        /// <returns>True if run properties were written. Required to avoid writing empty w:pPr declarations (cosmetic).</returns>
        internal bool WriteForNodes(RunPr runPr, 
            IRunAttrSource attrSource, 
            bool writePPrElementStart,
            INrxWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            if (runPr.IsMathRunPr && IsWriteMathPr(writer))
                WriteMathPr(writer, runPr);

            if (runPr.FormatRevision != null)
            {
                // Calculate and write the AfterChanges attribute collection.
                RunPr afterChanges = runPr.Clone();
                afterChanges.AcceptFormatRevision();
                // Force writing w:rPr always here because we need it to contain w:rPrChange that we always write below.
                WriteStart(afterChanges, attrSource, true, writePPrElementStart, true, writer, false);

                // Write the BeforeChanges attribute collection.
                builder.WriteRevisionStart(runPr.FormatRevision, "w:rPrChange", writer.GetNextAnnotationId());
                if (WriteStart(runPr, attrSource, true, false, false, writer, true))
                    builder.EndElement("w:rPr"); 
                builder.WriteRevisionEnd(); //w:rPrChange

                builder.EndElement("w:rPr"); 
                return true;
            }
            else if (WriteStart(runPr, attrSource, true, writePPrElementStart, false, writer, false))
            {
                builder.EndElement("w:rPr"); 
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Writes a complete run properties element.
        /// Writes without a reference to the character style and without pPr element start.
        /// </summary>
        /// <param name="runPr">The attributes to write?</param>
        /// <param name="attrSource">The attribute source that is used to resolve full values of BoolEx attributes.</param>
        /// <param name="writer">The writer to write to.</param>
        internal static void WriteForStyles(RunPr runPr, IRunAttrSource attrSource, INrxWriterContext writer)
        {
            if (runPr == null)
                return;

            if (runPr.FormatRevision != null)
            {
                // Calculate and write the AfterChanges attribute collection.
                RunPr afterChanges = runPr.Clone();
                afterChanges.AcceptFormatRevision();
                // Force writing w:rPr always here because we need it to contain w:rPrChange that we always write below.
                WriteStart(afterChanges, attrSource, false, false, true, writer, false);

                // Write the BeforeChanges attribute collection.
                // It seems that Word uses 0 as Id for style revisions.
                writer.Builder.WriteRevisionStart(runPr.FormatRevision, "w:rPrChange", 0);
                if (WriteStart(runPr, attrSource, false, false, false, writer, true))
                    writer.Builder.EndElement("w:rPr");
                writer.Builder.WriteRevisionEnd(); //w:rPrChange

                writer.Builder.EndElement("w:rPr");
            }
            else if (WriteStart(runPr, attrSource, false, false, false, writer, false))
                writer.Builder.EndElement("w:rPr");
        }

        protected abstract bool IsWriteMathPr(INrxWriterContext writer);

        /// <summary>
        /// Write a number of math-specific runPr values into m:rPr
        /// </summary>
        private void WriteMathPr(INrxWriterContext writer, RunPr runPr)
        {
            NrxXmlBuilder builder = writer.Builder;
            if (runPr.Count > 0) // we have at least one non-default pr in m:rPr, so lets write it.
            {
                builder.StartElement("m:rPr");
                builder.WriteValIfTrue("m:lit", runPr.MathIsLiteral);
                builder.WriteValIfTrue("m:nor", runPr.MathIsNormalText);

                MathScript mathScript = runPr.MathScript;
                if (mathScript != MathScript.Default)
                    builder.WriteVal("m:scr", DocxEnum.MathScriptTypeToDocx(mathScript));

                MathStyle mathStyle = runPr.MathStyle;
                if (mathStyle != MathStyle.Default)
                    builder.WriteVal("m:sty", DocxEnum.MathStyleTypeToDocx(mathStyle));

                if (runPr.MathLineBreak != null)
                    WriteMathLineBreak(runPr.MathLineBreak, builder);

                builder.WriteValIfTrue("m:aln", runPr.MathIsAlignmentPoint);

                builder.EndElement("m:rPr");
            }
        }

        protected abstract void WriteMathLineBreak(MathLineBreak mathLineBreak, NrxXmlBuilder builder);

        private static bool WriteStart(
            RunPr attrs, 
            IRunAttrSource attrSource, 
            bool isWriteStyleRef,
            bool isWritePPrElementStart,
            bool isForceWriteRPrElementStart,
            INrxWriterContext writer,
            bool isFormattingRevision)
        {
            if (attrs == null)
                return false;

            bool isDocx = writer.IsDocx;
            bool isIsoStrict = isDocx && (writer.Compliance == OoxmlComplianceCore.IsoStrict);

            // Counter is used to determine if there are any properties that need to be written in w:rPr declaration.
            // That is used to avoid writing empty w:rPr declarations.
            // Checking runPr.Count does not help here because there can be attributes that should not be written in WordML.
            int writableAttributesCounter = 0;

            string styleId = null;
            ComplexFontName fontAscii = null;
            ComplexFontName fontHAnsi = null;
            ComplexFontName fontFarEast = null;
            ComplexFontName fontCs = null;
            string hint = null;

            object bold = null;
            object boldCs = null;
            object italic = null;
            object italicCs = null;
            object caps = null;
            object smallCaps = null;
            object strike = null;
            object doubleStrike = null;
            object outline = null;
            object shadow = null;
            object emboss = null;
            object imprint = null;
            object noProof = null;
            object snapToGrid = null;
            object vanish = null;
            object webHidden = null;
            object color = null;
            object themeColor = null;
            object themeShade = null;
            object themeTint = null;

            object spacing = null;
            object w = null;
            object kern = null;
            object position = null;
            object size = null;
            object sizeCs = null;
            object highlight = null;

            string underlineValue = null;
            string underlineColor = null;

            string effect = null;
            Border border = null;
            Shading shading = null;

            FitText fitText = null;

            string vertAlign = null;
            object rtl = null;
            object cs = null;
            string em = null;

            string hyphen = null;
            string hyphenRule = null;

            string lang = null;
            string langFarEast = null;
            string langBidi = null;

            FarEastLayout feLayout = null;

            object specVanish = null;

            object wxFont = null;
            object wxSymFont = null;
            object wxSymChar = null;

            bool oMath = false;

            Theme theme = writer.Document.GetThemeInternal();

            // Retrieves run attributes from the specified WordAttrCollection.

            // This is the main loop to collect the properties.
            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                // RK By default, lets increment the number of properties collected so we don't
                // have to do it in every case statement.
                writableAttributesCounter++;

                switch (key)
                {
                    case FontAttr.Istd:
                    {
                        int istd = (int)value;

                        if (isWriteStyleRef && WordUtil.IsValidIstd(istd))
                        {
                            styleId = writer.GetStyleId(istd);
                        }
                        else
                        {
                            // We need to set it explicitly here to merge revised RunProps correctly.
                            styleId = null;
                            writableAttributesCounter--;
                        }
                        break;
                    }

                    case FontAttr.NameAscii:
                        fontAscii = (ComplexFontName)value;
                        if (!isDocx && fontAscii.IsThemeFont)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.NameFarEast:
                        fontFarEast = (ComplexFontName)value;
                        if (!isDocx && fontFarEast.IsThemeFont)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.NameOther:
                        fontHAnsi = (ComplexFontName)value;
                        if (!isDocx && fontHAnsi.IsThemeFont)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.NameBi:
                        fontCs = (ComplexFontName)value;
                        if (!isDocx && fontCs.IsThemeFont)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.CharacterCategoryHint:
                        hint = NrxRunEnum.HintToXml((CharacterCategory)value, isDocx);
                        break;
                    case FontAttr.Underline:
                        underlineValue = NrxRunEnum.UnderlineToXml((Underline)value, isDocx);
                        break;
                    case FontAttr.UnderlineColor:
                        underlineColor = NrxXmlUtil.ColorToXml((DrColor)value);
                        break;
                    case FontAttr.UnderlineThemeColor:
                        if (!isDocx)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.UnderlineThemeShade:
                        if (!isDocx)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.UnderlineThemeTint:
                        if (!isDocx)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.HyphenChar:
                        // RK Do nothing. Seems not used in DOCX.
                        writableAttributesCounter--;
                        break;
                    case FontAttr.HyphenRule:
                        if (isDocx)
                            writableAttributesCounter--;    // RK Do nothing. Seems not used in DOCX.
                        else
                            hyphenRule = NrxRunEnum.HyphenRuleToXml((HyphenRule)value);
                        break;
                    case FontAttr.LocaleId:
                        lang = GetLang((int)value, isDocx);
                        if (!StringUtil.HasChars(lang) && ((int)value != (int)Language.InvariantCulture))
                        {
                            writableAttributesCounter--;
                            lang = null;
                        }
                        break;
                    case FontAttr.LocaleIdFarEast:
                        langFarEast = GetLang((int)value, isDocx);
                        if (!StringUtil.HasChars(langFarEast) && ((int)value != (int)Language.InvariantCulture))
                        {
                            writableAttributesCounter--;
                            langFarEast = null;
                        }
                        break;
                    case FontAttr.LocaleIdBi:
                        langBidi = GetLang((int)value, isDocx);
                        if (!StringUtil.HasChars(langBidi) && ((int)value != (int)Language.InvariantCulture))
                        {
                            writableAttributesCounter--;
                            langBidi = null;
                        }
                        break;
                    case FontAttr.Bold:
                        bold = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.BoldBi:
                        boldCs = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Italic:
                        italic = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.ItalicBi:
                        italicCs = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.AllCaps:
                        caps = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.SmallCaps:
                        smallCaps = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.StrikeThrough:
                        strike = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.DoubleStrikeThrough:
                        doubleStrike = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Shadow:
                        shadow = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Emboss:
                        emboss = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Engrave:
                        imprint = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.NoProofing:
                        noProof = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Outline:
                        outline = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Bidi:
                        rtl = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.ComplexScript:
                        cs = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Hidden:
                        vanish = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.SpecialHidden:
                        specVanish = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.SnapToGrid:
                        snapToGrid = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.WebHidden:
                        // TODO 2 Find where in DOC.
                        webHidden = BoolExToBool(value, attrSource, key);
                        break;
                    case FontAttr.Size:
                        size = value;
                        break;
                    case FontAttr.SizeBi:
                        sizeCs = value;
                        break;
                    case FontAttr.Kerning:
                        kern = value;
                        break;
                    case FontAttr.Spacing:
                        spacing = value;
                        break;
                    case FontAttr.Position:
                        position = value;
                        break;
                    case FontAttr.Scaling:
                        w = value;
                        break;
                    case FontAttr.Border:
                        border = (Border)value;
                        if (border.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.Shading:
                        shading = (Shading)value;
                        if (shading.IsInherited)
                            writableAttributesCounter--;
                        break;
                    case FontAttr.Color:
                        color = value;
                        break;
                    case FontAttr.ThemeColor:
                        if (isDocx)
                            themeColor = value;
                        else
                            writableAttributesCounter--;
                        break;
                    case FontAttr.ThemeShade:
                        if (isDocx)
                            themeShade = value;
                        else
                            writableAttributesCounter--;
                        break;
                    case FontAttr.ThemeTint:
                        if (isDocx)
                            themeTint = value;
                        else
                            writableAttributesCounter--;
                        break;
                    case FontAttr.HighlightColor:
                        highlight = NrxRunEnum.HighlightToXml((DrColor)value, isDocx);
                        break;
                    case FontAttr.TextEffect:
                        effect = NrxRunEnum.TextEffectToXml((TextEffect)value, isDocx);
                        break;
                    case FontAttr.VerticalAlignment:
                        vertAlign = NrxRunEnum.RunVerticalAlignmentToXml((RunVerticalAlignment)value);
                        break;
                    case RevisionAttr.InsertRevision:
                    case RevisionAttr.DeleteRevision:
                    case RevisionAttr.MoveFromRevision:
                    case RevisionAttr.MoveToRevision:
                    {
                        // Do not write insert/delete/move revision inside a format revision.
                        if (!isFormattingRevision)
                        {
                            if (!(attrSource is Paragraph))         // This code was not in WML, but okay for now.
                                writableAttributesCounter--;
                        }
                        break;
                    }
                    case RevisionAttr.FormatRevision:
                        break;
                    case FontAttr.LineBreakClear:
                        // not written in WordML
                        writableAttributesCounter--;
                        break;
                    case FontAttr.MathIsOMath:
                        oMath = true;
                        break;
                    case FontAttr.EmphasisMark:
                        em = NrxRunEnum.EmphasisMarkToXml((EmphasisMark)value, isDocx);
                        break;
                    case FontAttr.FarEastLayout:
                        feLayout = (FarEastLayout)value;
                        break;
                    case FontAttr.EffectGlow:
                    case FontAttr.EffectShadow:
                    case FontAttr.EffectReflection:
                    case FontAttr.EffectOutline:
                    case FontAttr.EffectFill:
                    case FontAttr.EffectScene3D:
                    case FontAttr.EffectProps3D:
                    case FontAttr.OpenTypeLigature:
                    case FontAttr.OpenTypeNumForm:
                    case FontAttr.OpenTypeNumSpacing:
                    case FontAttr.OpenTypeStylisticSets:
                    case FontAttr.OpenTypeContextualAlternates:
                        // Writable, written in WriteW14Attributes().
                        break;
                    case FontAttr.FitText:
                        fitText = (FitText)value;
                        break;
                    default:
                        writableAttributesCounter--;
                        break;
                }
            }

            // Write properties in proper order.
            // Although the order of elements is not important,
            // keeping the same order as in MS Word 2003 generated xml
            // makes comparison and analysis more convenient.

            // Attributes with null values are not written by DocxBuilder.WriteXXX methods.
            // Check is performed inside DocxBuilder methods.

            if ((writableAttributesCounter == 0) && !isForceWriteRPrElementStart)
                return false;

            NrxXmlBuilder builder = writer.Builder;

            if (isWritePPrElementStart)
                builder.StartElement("w:pPr");

            builder.StartElement("w:rPr");

            // Write insert and delete revisions only if not writing a format revision.
            if (!isFormattingRevision)
            {
                // This is nasty, is there a better way?
                if (attrSource is Paragraph)
                {
                    if (attrs.HasInsertRevision)
                        builder.WriteRevision(attrs.InsertRevision, writer.GetNextAnnotationId());
                    if (attrs.HasDeleteRevision)
                        builder.WriteRevision(attrs.DeleteRevision, writer.GetNextAnnotationId());
                    if (attrs.HasMoveFromRevision)
                        builder.WriteRevision(attrs.MoveFromRevision, writer.GetNextAnnotationId());
                    if (attrs.HasMoveToRevision)
                        builder.WriteRevision(attrs.MoveToRevision, writer.GetNextAnnotationId());
                }
            }
            
            // TODO 6139. See "[MS-OI29500] 2.1.243 Part 1 Section 17.7.5.4, rPr (Run Properties)".
            // The standard states that the cs, highlight, oMath, rPrChange, rStyle, and rtl elements are valid children of the rPr element. 
            // Word does not allow these elements to be children of the rPr element.
            builder.WriteVal("w:rStyle", styleId);
            if (isDocx)
            {
                builder.WriteValIfTrue("w:oMath", oMath);
                
                builder.WriteElementWithAttributes(
                    "w:rFonts",
                    "w:ascii", GetFontNameOrNull(fontAscii),
                    "w:eastAsia", GetFontNameOrNull(fontFarEast),
                    "w:hAnsi", GetFontNameOrNull(fontHAnsi),
                    "w:asciiTheme", GetThemeNameOrNull(fontAscii), 
                    "w:eastAsiaTheme", GetThemeNameOrNull(fontFarEast),
                    "w:hAnsiTheme", GetThemeNameOrNull(fontHAnsi),
                    "w:cs", GetFontNameOrNull(fontCs),
                    "w:cstheme", GetThemeNameOrNull(fontCs),  // Yes, it is "cstheme", not "csTheme"!
                    "w:hint", hint);
            }
            else
            {
                builder.WriteElementWithAttributes(
                    "w:rFonts",
                    "w:ascii", ComplexFontName.Resolve(fontAscii, theme),
                    "w:fareast", ComplexFontName.Resolve(fontFarEast, theme),
                    "w:h-ansi", ComplexFontName.Resolve(fontHAnsi, theme),
                    "w:cs", ComplexFontName.Resolve(fontCs, theme),
                    "w:hint", hint);
            }

            builder.WriteVal("w:b", bold);
            builder.WriteVal(isDocx ? "w:bCs" : "w:b-cs", boldCs);
            builder.WriteVal("w:i", italic);
            builder.WriteVal(isDocx ? "w:iCs" : "w:i-cs", italicCs);
            builder.WriteVal("w:caps", caps);
            builder.WriteVal("w:smallCaps", smallCaps);
            builder.WriteVal("w:strike", strike);
            builder.WriteVal("w:dstrike", doubleStrike);
            builder.WriteVal("w:outline", outline);
            builder.WriteVal("w:shadow", shadow);
            builder.WriteVal("w:emboss", emboss);
            builder.WriteVal("w:imprint", imprint);
            builder.WriteVal("w:noProof", noProof);
            builder.WriteVal("w:snapToGrid", snapToGrid);
            builder.WriteVal("w:vanish", vanish);
            builder.WriteVal("w:webHidden", webHidden);
            
            if (isDocx)
            {
                builder.WriteElementWithAttributes(
                    "w:color",
                    "w:val", color,
                    "w:themeColor", themeColor,
                    "w:themeShade", themeShade,
                    "w:themeTint", themeTint);
            }
            else
            {
                builder.WriteVal("w:color", color);
            }
            
            builder.WriteVal("w:spacing", spacing);
            if ((w != null) && isIsoStrict)
                builder.WriteVal("w:w", NrxXmlBuilder.PercentToXml((int)w));
            else
                builder.WriteVal("w:w", w);
            builder.WriteVal("w:kern", kern);
            builder.WriteVal("w:position", position);
            builder.WriteVal("w:sz", size);
            builder.WriteVal(isDocx ? "w:szCs" : "w:sz-cs", sizeCs);
            builder.WriteVal("w:highlight", highlight);

            builder.WriteElementWithAttributes(
                "w:u",
                "w:val", underlineValue,
                "w:color", underlineColor);

            builder.WriteVal("w:effect", effect);
            builder.WriteBorder("w:bdr", border);
            builder.WriteShd(shading);

            if (fitText != null)
            {
                builder.WriteElementWithAttributes(
                    "w:fitText",
                    "w:val", fitText.Value,
                    "w:id", fitText.Id);
            }

            builder.WriteVal("w:vertAlign", vertAlign);
            builder.WriteVal("w:rtl", rtl);
            builder.WriteVal("w:cs", cs);
           
            builder.WriteVal("w:em", em);

            if (!isDocx)
            {
                builder.WriteElementWithAttributes(
                    "w:hyphen",
                    "w:val", hyphen,
                    "w:rule", hyphenRule);
            }

            if ((lang != null) || (langFarEast != null) || (langBidi != null))
            {
                builder.StartElement("w:lang");

                if(lang != null)
                    builder.WriteAttributeString("w:val", lang);
                if(langFarEast != null)
                    builder.WriteAttributeString(isDocx ? "w:eastAsia" : "w:fareast", langFarEast);
                if(langBidi != null)
                    builder.WriteAttributeString("w:bidi", langBidi);

                builder.EndElement();
            }

            if (feLayout != null)
                WriteFarEastLayout(builder, feLayout, isDocx);

            builder.WriteVal("w:specVanish", specVanish);

            writer.WriteW14Attributes(writer, attrs);

            if (!isDocx)
            {
                //TODO 3 Find data for wx.

                builder.WriteVal("wx:font", wxFont);

                builder.WriteElementWithAttributes(
                    "wx:sym",
                    "wx:font", wxSymFont,
                    "wx:char", wxSymChar);
            }

            return true;
        }

        /// <summary>
        /// Returns lang value for given locale considering LanguageNotSet.
        /// </summary>
        /// <remarks>
        /// See TestJira15005() for details.
        /// </remarks>
        private static string GetLang(int locale, bool isDocx)
        {
            if (isDocx && (Language.LanguageNotSet == (Language)locale))
            {
                return "x-none";
            }

            return isDocx ? LocaleConverter.LocaleToDocxTag(locale) : LocaleConverter.LocaleToWmlTag(locale);
        }

        /// <summary>
        /// Returns font name if font is not null and font is not theme font. Otherwise returns null.
        /// </summary>
        private static string GetFontNameOrNull(ComplexFontName fontName)
        {
            return ((fontName != null) && !fontName.IsThemeFont) ? fontName.Name : null;
        }

        /// <summary>
        /// Returns theme name if font is not null and font is theme font. Otherwise returns null.
        /// </summary>
        private static string GetThemeNameOrNull(ComplexFontName fontName)
        {
            return ((fontName != null) && fontName.IsThemeFont) ? NrxRunEnum.ThemeFontToXml(fontName.ThemeFontCore) : null;
        }

        private static void WriteFarEastLayout(NrxXmlBuilder builder, FarEastLayout feLayout, bool isDocx)
        {
            builder.WriteElementWithAttributes(
                isDocx ? "w:eastAsianLayout" : "w:asianLayout", 
                "w:id", feLayout.FarEastLayoutId,
                "w:vert", feLayout.Vertical,
                isDocx ? "w:vertCompress" : "w:vert-compress", feLayout.VerticalCompress,
                "w:combine", feLayout.Combine,
                isDocx ? "w:combineBrackets" : "w:combine-brackets", NrxRunEnum.CombineBracketsToXml(feLayout.CombineBrackets)
            );
        }

        /// <summary>
        /// TODO neremin this code never used.
        /// All "boolean" attributes in a run attributes collection are stored as BoolEx values.
        /// BoolEx has to be converted to bool before writing to WordML.
        /// </summary>
        /// <param name="value">The BoolEx value.</param>
        /// <param name="attrSource">The attribute source that is used to resolve full values of BoolEx attributes.</param>
        /// <param name="key">The key of the attribute.</param>
        /// <returns>True, false or null.</returns>
        private static object BoolExToBool(object value, IRunAttrSource attrSource, int key)
        {
            return ((AttrBoolEx)value).ResolveFetchInheritedRunAttrWithNull(attrSource, key);
        }
    }
}
