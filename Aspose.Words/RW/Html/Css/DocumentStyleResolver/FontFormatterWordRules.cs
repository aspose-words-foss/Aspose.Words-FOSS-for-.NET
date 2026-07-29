// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/01/2017 by Victor Chebotok

using System.Drawing;
using Aspose.Common;
using Aspose.Fonts.TrueType;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fonts;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to characters. Uses same formatting rules as MS Word.
    /// </summary>
    internal class FontFormatterWordRules : FontFormatter
    {
        internal FontFormatterWordRules(
            DocumentFontProvider fontProvider,
            bool isLoadingHtmlAltChunk)
            : base(false)
        {
            mFontProvider = fontProvider;
            mIsLoadingHtmlAltChunk = isLoadingHtmlAltChunk;
        }

        protected override void ApplySpecialFormatting(
            Font font,
            CssDeclarationCollection declarations,
            CssTextDecoration textDecoration,
            bool isInsideHtmlParagraph)
        {
            ApplyDeclarations(font, declarations, isInsideHtmlParagraph, false, true);
            ApplyPropagatedTextDecoration(font, textDecoration);
        }

        protected override void ApplySpecialFormatting(
            Font font,
            StyleType styleType,
            CssDeclarationCollection declarations)
        {
            // In MS Word, 'text-decoration: none' is applied to characters styles only.
            bool applyNoneTextDecoration = styleType == StyleType.Character;

            // For some reason, MS Word applies character category hint to styles. We mimic this behavior when loading altChunks.
            ApplyDeclarations(font, declarations, true, mIsLoadingHtmlAltChunk, applyNoneTextDecoration);
        }

        private void ApplyDeclarations(
            Font font,
            CssDeclarationCollection declarations,
            bool isInsideHtmlParagraph,
            bool applyCharacterCategoryHint,
            bool applyNoneTextDecoration)
        {
            ApplyFontSize(font, declarations);
            if (mIsLoadingHtmlAltChunk)
            {
                ApplyFontFamily(font, declarations, isInsideHtmlParagraph, applyCharacterCategoryHint);
            }
            else
            {
                ApplyMsoFontFamily(font, declarations, applyCharacterCategoryHint);
                ApplyMsoLanguage(font, declarations);
            }
            ApplyBorder(font, declarations);
            ApplyBackgroundColor(font, declarations);
            ApplyTextDecoration(font, declarations, applyNoneTextDecoration);
        }

        private static void ApplyTextDecoration(
            Font font,
            CssDeclarationCollection declarations,
            bool applyNoneTextDecoration)
        {
            Debug.Assert(declarations != null);

            CssDeclaration textDecorationDeclaration = declarations["text-decoration"];
            if (textDecorationDeclaration == null)
            {
                return;
            }

            CssTextDecorationPropertyValue textDeclarationValue =
                textDecorationDeclaration.Value as CssTextDecorationPropertyValue;
            if (textDeclarationValue == null)
            {
                return;
            }

            if (textDeclarationValue.IsNone)
            {
                if (applyNoneTextDecoration)
                {
                    font.StrikeThrough = false;
                    font.Underline = Underline.None;
                    font.DoubleStrikeThrough = false;
                    font.TextEffect = TextEffect.None;
                }
            }
            else
            {
                if (textDeclarationValue.IsLineThrough)
                {
                    font.StrikeThrough = true;
                }
                if (textDeclarationValue.IsUnderline)
                {
                    font.Underline = Underline.Single;
                }
            }
        }

        private static void ApplyPropagatedTextDecoration(Font font, CssTextDecoration textDecoration)
        {
            if (textDecoration.Underline != NullableBool.NotDefined)
            {
                font.Underline = (textDecoration.Underline == NullableBool.True)
                    ? Underline.Single
                    : Underline.None;
            }

            if (textDecoration.StrikeThrough != NullableBool.NotDefined)
            {
                font.StrikeThrough = textDecoration.StrikeThrough == NullableBool.True;
            }
        }

        private static void ApplyFontSize(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration fontSizeDeclaration = declarations["font-size"];
            if (fontSizeDeclaration == null)
            {
                return;
            }

            double fontSize = CssUtil.LengthToPoint(fontSizeDeclaration.Value);
            if (MathUtil.IsMinValue(fontSize))
            {
                return;
            }

            font.Size = fontSize;
            font.SizeBi = fontSize;
        }

        private void ApplyFontFamily(
            Font font,
            CssDeclarationCollection declarations,
            bool isInsideHtmlParagraph,
            bool applyCharacterCategoryHint)
        {
            // Mimics observable behavior of MS Word for text imported from elements other than <p>.
            if (!isInsideHtmlParagraph)
            {
                font.NameFarEast = "Times New Roman";
            }

            // We don't support @font-face rules when loading HTML with MS Word rules.
            string fontName = declarations.GetFontName(null, mFontProvider);
            if (StringUtil.HasChars(fontName))
            {
                font.NameAscii = fontName;
                font.NameOther = fontName;

                // WORDSNET-21707 MS Word sets complex script language if the corresponding font family supports it.
                TTFont ttFont = mFontProvider.GetTTFont(fontName, FontStyle.Regular);
                if ((ttFont != null) && ttFont.UnicodeRanges.IsComplexScriptSupported())
                {
                    font.NameBi = fontName;
                }

                if (applyCharacterCategoryHint)
                {
                    font.Parent.SetRunAttr(FontAttr.CharacterCategoryHint, CharacterCategory.Ascii);
                }
            }
        }

        private void ApplyMsoFontFamily(
            Font font,
            CssDeclarationCollection declarations,
            bool applyCharacterCategoryHint)
        {
            string msoAsciiFontFamily = declarations.GetIdentifier("mso-ascii-font-family");
            if (StringUtil.HasChars(msoAsciiFontFamily))
            {
                font.NameAscii = msoAsciiFontFamily;
            }

            string msoFarEastFontFamily = declarations.GetIdentifier("mso-fareast-font-family");
            if (StringUtil.HasChars(msoFarEastFontFamily))
            {
                font.NameFarEast = msoFarEastFontFamily;
            }

            string msoHAnsiFontFamily = declarations.GetIdentifier("mso-hansi-font-family");
            if (StringUtil.HasChars(msoHAnsiFontFamily))
            {
                font.NameOther = msoHAnsiFontFamily;
            }

            string msoBidiFontFamily = declarations.GetIdentifier("mso-bidi-font-family");
            if (StringUtil.HasChars(msoBidiFontFamily))
            {
                font.NameBi = msoBidiFontFamily;
            }

            string fontName = declarations.GetFontName(null, mFontProvider);
            if (StringUtil.HasChars(fontName))
            {
                if (!StringUtil.HasChars(msoAsciiFontFamily))
                    font.NameAscii = fontName;

                if (!StringUtil.HasChars(msoHAnsiFontFamily))
                    font.NameOther = fontName;

                // WORDSNET-21707 MS Word sets complex script language if the corresponding font family supports it.
                TTFont ttFont = mFontProvider.GetTTFont(fontName, FontStyle.Regular);
                if ((ttFont != null) &&
                    ttFont.UnicodeRanges.IsComplexScriptSupported() &&
                    !StringUtil.HasChars(msoBidiFontFamily))
                {
                    font.NameBi = fontName;
                }
                if (applyCharacterCategoryHint)
                {
                    font.Parent.SetRunAttr(FontAttr.CharacterCategoryHint, CharacterCategory.Ascii);
                }
            }

            // Applying theme fonts. Theme fonts may override the font families previously set.
            ApplyThemeFont(font, declarations, "mso-ascii-theme-font", FontAttr.NameAscii);
            ApplyThemeFont(font, declarations, "mso-hansi-theme-font", FontAttr.NameOther);
            ApplyThemeFont(font, declarations, "mso-fareast-theme-font", FontAttr.NameFarEast);
            ApplyThemeFont(font, declarations, "mso-bidi-theme-font", FontAttr.NameBi);
        }

        private static void ApplyThemeFont(
            Font font,
            CssDeclarationCollection declarations,
            string propertyName,
            int fontAttr)
        {
            string themeFontIdentifier = declarations.GetIdentifier(propertyName);
            if (!StringUtil.HasChars(themeFontIdentifier))
                return;

            ThemeFontCore themeFontCore = ParseMsoThemeFontIdentifier(themeFontIdentifier);
            if (themeFontCore == ThemeFontCore.None)
                return;

            font.Parent.SetRunAttr(fontAttr, ComplexFontName.FromTheme(themeFontCore));
        }

        private static void ApplyBorder(Font font, CssDeclarationCollection declarations)
        {
            // Only top border seems to be taken into account by MS Word.
            CssBorder topBorder = CssBorder.CreateBorder(declarations, BorderType.Top, false);
            if (topBorder.IsUndefined)
            {
                return;
            }

            topBorder.ToModelBorder(font.Border);
        }

        private static void ApplyBackgroundColor(Font font, CssDeclarationCollection declarations)
        {
            CssBackgroundColor backgroundColor = new CssBackgroundColor(declarations);
            backgroundColor.ToShading(font.Parent);
        }

        /// <summary>
        /// Applies locales according to the MSO properties.
        /// </summary>
        private static void ApplyMsoLanguage(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration ansiLanguageDeclaration = declarations["mso-ansi-language"];
            if ((ansiLanguageDeclaration != null) && (ansiLanguageDeclaration.Value.Count == 1))
                font.LocaleId = ParseMsoLocaleId(ansiLanguageDeclaration.Value.FirstValue);

            CssDeclaration fareastLanguageDeclaration = declarations["mso-fareast-language"];
            if ((fareastLanguageDeclaration != null) && (fareastLanguageDeclaration.Value.Count == 1))
                font.LocaleIdFarEast = ParseMsoLocaleId(fareastLanguageDeclaration.Value.FirstValue);

            CssDeclaration bidiLanguageDeclaration = declarations["mso-bidi-language"];
            if ((bidiLanguageDeclaration != null) && (bidiLanguageDeclaration.Value.Count == 1))
                font.LocaleIdBi = ParseMsoLocaleId(bidiLanguageDeclaration.Value.FirstValue);
        }

        /// <summary>
        /// Resolves the language tag or hexadecimal value to a locale ID.
        /// </summary>
        /// <returns>
        /// If the language tag is unknown, this method returns <see cref="Language.InvariantCulture"/>.
        /// </returns>
        internal static int ParseMsoLocaleId(CssValue cssValue)
        {
            // The language tag may be in a hexadecimal form (for example `mso-ansi-language:#0471`).
            if (cssValue.ValueType == CssValueType.Hash)
            {
                string hash = ((CssHashValue)cssValue).Text;
                if (hash.Length == 4)
                {
                    int localeId = FormatterPal.TryParseHex(hash);
                    // Note that the condition also covers the case where parsing fails and returns int.MinValue.
                    if ((localeId >= 0) && (localeId <= 0xFFFF))
                    {
                        return localeId;
                    }
                }
            }
            else if (cssValue.ValueType == CssValueType.Identifier)
            {
                // MS Word has some limitations and slightly different behavior during resolving locale ID.
                // Currently, we use the old dictionary from the built-in locale converter.
                return LocaleConverter.WmlTagToLocale(((CssIdentifierValue)cssValue).Value);
            }

            return (int)Language.InvariantCulture;
        }

        private static ThemeFontCore ParseMsoThemeFontIdentifier(string themeFont)
        {
            // For values 'major-latin' and 'minor-latin' MS Word sets w:asciiTheme="majorHAnsi" and w:asciiTheme="minorHAnsi"
            // respectively. We follow this behavior.
            string themeFontLower = themeFont.ToLowerInvariant();
            switch (themeFontLower)
            {
                case "minor-latin":
                    return ThemeFontCore.MinorHAnsi;
                case "minor-bidi":
                    return ThemeFontCore.MinorBidi;
                case "minor-fareast":
                    return ThemeFontCore.MinorEastAsia;
                case "major-latin":
                    return ThemeFontCore.MajorHAnsi;
                case "major-bidi":
                    return ThemeFontCore.MajorBidi;
                case "major-fareast":
                    return ThemeFontCore.MajorEastAsia;
                default:
                    return ThemeFontCore.None;
            }
        }

        private readonly DocumentFontProvider mFontProvider;

        /// <summary>
        /// Indicates whether this class is used to format content loaded from a HTML altChunk.
        /// Note that MS Word's behavior is a little different in this case.
        /// </summary>
        private readonly bool mIsLoadingHtmlAltChunk;
    }
}
