// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/01/2017 by Victor Chebotok

using Aspose.Words.Fonts;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to characters. Uses formatting rules that are different from what MS Word uses but produce
    /// better looking results and are more confomant to the CSS and HTML specifications.
    /// </summary>
    internal class FontFormatterHtmlRules : FontFormatter
    {
        internal FontFormatterHtmlRules(
            bool applyUserAgentStyles,
            CssFontFaceProvider cssFontFaceProvider,
            DocumentFontProvider fontProvider)
            : base(applyUserAgentStyles)
        {
            mApplyUserAgentStyles = applyUserAgentStyles;
            mCssFontFaceProvider = cssFontFaceProvider;
            mFontProvider = fontProvider;
        }

        protected override void ApplySpecialFormatting(
            Font font,
            CssDeclarationCollection declarations,
            CssTextDecoration textDecoration,
            bool isInsideHtmlParagraph)
        {
            // Default HTML font size is different from default AW font size, so we need to set it explicitly.
            // Note that we don't apply this to styles.
            if (mApplyUserAgentStyles)
            {
                font.Size = 12;
                font.SizeBi = 12;
            }

            ApplyFontSize(font, declarations);
            ApplyFontFamily(font, declarations);

            ApplyTextDecoration(font, textDecoration);
        }

        protected override void ApplySpecialFormatting(Font font, StyleType styleType, CssDeclarationCollection declarations)
        {
            ApplyFontSize(font, declarations);
            ApplyFontFamily(font, declarations);

            // WORDSNET-9025 'text-decoration' is not translated to style formatting. 'text-decoration' corresponds
            // to font Underline and StrikeThrough properties in AW document model. However, 'text-decoration' always specifies
            // the full set of decorations applied to text. For example, 'text-decoration:underline' means not only that text
            // is to be underlined, but also that the text has no line through it (has no decoration other than 'underline').
            // As a result, it is impossible in CSS to combine text decorations that come from CSS classes and from inline
            // style but it is possible in AW model. To match CSS rules we have to prevent combination of text decorations
            // in AW model, that is why we do not translate 'text-decoration' to AW styles. Text decorations are always imported
            // as direct formatting, and it is the only source of them in the result AW model.
            //
            // We have to clear corresponding properties out of the style's font formatting, because we might be applying
            // formatting to a built-in style that has text decoration applied by default (for example, the 'Hyperlink' style).
            ClearTextDecoration(font);
        }

        private void ApplyFontSize(Font font, CssDeclarationCollection declarations)
        {
            bool applyFontSize = false;

            CssDeclaration fontSizeDeclaration = declarations["font-size"];
            if (fontSizeDeclaration != null)
            {
                applyFontSize = true;
            }

            double fontSize = (fontSizeDeclaration != null)
                ? CssUtil.LengthToPoint(fontSizeDeclaration.Value)
                : CssUtil.DefaultFontSize;
            if (MathUtil.IsMinValue(fontSize))
            {
                return;
            }

            // The 'vertical-align' CSS property is translated to Font.Subscript or Font.Superscript.
            // In MS Word, text with Font.Subscript or Font.Superscript property set is rendered in smaller font
            // than specified, but in HTML the 'vertical-align' property does not affect the font size.
            // To compensate the font size reduction in MS Word we increase the font size specified in HTML.
            // This behavior is optional, however. When <sub> or <sup> elements are processed with formatting specified
            // in document builder, the font size of text imported from these elements is not changed.
            if (mApplyUserAgentStyles)
            {
                CssDeclaration verticalAlignDeclaration = declarations["vertical-align"];
                if ((verticalAlignDeclaration != null) &&
                    (verticalAlignDeclaration.Value.Equals(CssValue.Sub) || verticalAlignDeclaration.Value.Equals(CssValue.Super)))
                {
                    // In MS Word, text with Font.Subscript or Font.Superscript property set is rendered in 2/3
                    // of its specified size. To compensate the font size reduction we increase the font size
                    // specified in HTML by 3/2 = 1.5
                    const double subSupFontScale = 1.5;
                    fontSize *= subSupFontScale;
                    applyFontSize = true;
                }
            }

            // We apply font size either if it is declared explicitly (there is a 'font-size' declaration in CSS style)
            // or if font is sub/superscript and should be scaled.
            if (applyFontSize)
            {
                font.Size = fontSize;
                font.SizeBi = fontSize;
            }
        }

        private void ApplyFontFamily(Font font, CssDeclarationCollection declarations)
        {
            string originalFontName = declarations.GetString(HtmlConstants.OriginalFontFamily);
            if (StringUtil.HasChars(originalFontName))
            {
                // The original font name might have been changed during export to HTML. Here we restore it.
                font.Name = originalFontName;
                return;
            }

            string fontName = declarations.GetFontName(mCssFontFaceProvider, mFontProvider);
            if (StringUtil.HasChars(fontName))
            {
                font.Name = fontName;
            }
        }

        private static void ApplyTextDecoration(Font font, CssTextDecoration textDecoration)
        {
            if (textDecoration.StrikeThrough == NullableBool.True)
            {
                font.StrikeThrough = true;
            }
            if (textDecoration.Underline == NullableBool.True)
            {
                font.Underline = Underline.Single;
                if (textDecoration.UnderlineColor != null)
                {
                    font.UnderlineColorInternal = textDecoration.UnderlineColor;
                }
            }
        }

        private static void ClearTextDecoration(Font font)
        {
            font.Parent.RemoveRunAttr(FontAttr.Underline);
            font.Parent.RemoveRunAttr(FontAttr.UnderlineColor);
            font.Parent.RemoveRunAttr(FontAttr.StrikeThrough);
        }

        /// <summary>
        /// Controls whether certain default (user-agent) formatting is applied to characters.
        /// </summary>
        private readonly bool mApplyUserAgentStyles;

        private readonly CssFontFaceProvider mCssFontFaceProvider;
        private readonly DocumentFontProvider mFontProvider;
    }
}
