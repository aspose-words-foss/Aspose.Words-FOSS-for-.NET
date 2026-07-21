// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/01/2017 by Victor Chebotok

using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to characters.
    /// </summary>
    internal abstract class FontFormatter
    {
        protected FontFormatter(bool applyOverridableStyles)
        {
            mApplyOverridableStyles = applyOverridableStyles;
        }

        internal void Format(Font font, CssStyleTracker cssStyleTracker)
        {
            CssDeclarationCollection declarations = cssStyleTracker.ElementDeclarations;
            CssTextDecoration textDecoration = cssStyleTracker.TextDecoration;

            // WORDSNET-27579 If an element maps to a built-in character style and we're in the "use builder formatting" mode,
            // default user-agent HTML formatting must not be applied directly, because otherwise it will override formatting
            // applied by character styles.
            if (!mApplyOverridableStyles)
            {
                // Remove user-agent default property values from elements that map to built-in character styles.
                // This prevents this user-agent defaults from overwriting formatting specified by styles, which is what
                // we want in the "use builder formatting" HTML insertion mode.
                string[] overridableProperties = GetOverridableProperties(font.Style, cssStyleTracker);
                declarations = declarations.WithoutUserAgentDeclarations(overridableProperties);

                // This removes underline from hyperlinks.
                if (ArrayUtil.Contains(overridableProperties, "text-decoration"))
                {
                    textDecoration = textDecoration.WithoutUnderline();
                }
            }

            ApplyDeclarations(font, declarations);

            ApplyBorder(font, cssStyleTracker.InlineBorder);
            ApplyVerticalAlign(font, cssStyleTracker.SubSupScript);
            ApplyBackgroundColor(font, new CssBackgroundColor(cssStyleTracker.Background.GetFontBackgroundColor()));

            // Apply 'lang' attribute values.
            font.LocaleId = cssStyleTracker.LocaleId;
            font.LocaleIdBi = cssStyleTracker.LocaleIdBi;
            font.LocaleIdFarEast = cssStyleTracker.LocaleIdFarEast;

            HtmlElementInfo parentBlock = cssStyleTracker.CurrentElementInfo.ParentBlockElement;
            bool isInsideHtmlParagraph = (parentBlock != null) && (parentBlock.Element.ElementName == "p");

            ApplySpecialFormatting(font, declarations, textDecoration, isInsideHtmlParagraph);
        }

        /// <summary>
        /// Gets properties that map to default formatting of built-in character styles.
        /// </summary>
        private static string[] GetOverridableProperties(Style fontStyle, CssStyleTracker styleTracker)
        {
            // MS Word propagates character styles to inner inline elements (for example, "<em><span>Text</span><em>")
            // so we have to look up the HTML tree for the element that applies the character style.
            string elementName = styleTracker.FindNearestInlineOverridableParentElement();

            switch (elementName)
            {
                case "a":
                    if (fontStyle.StyleIdentifier == StyleIdentifier.Hyperlink)
                    {
                        // "Hyperlink" text is blue and underlined by default.
                        return new string[] { "color", "text-decoration" };
                    }
                    break;
                case "strong":
                    if (fontStyle.StyleIdentifier == StyleIdentifier.Strong)
                    {
                        // "Strong" text is bold by default.
                        return new string[] { "font-weight" };
                    }
                    break;
                case "em":
                    if (fontStyle.StyleIdentifier == StyleIdentifier.Emphasis)
                    {
                        // "Emphasis" text is italic by default.
                        return new string[] { "font-style" };
                    }
                    break;
                default:
                    break;
            }
            return ArrayUtil.EmptyStringArray;
        }

        internal void Format(Font font, StyleType styleType, CssDeclarationCollection declarations)
        {
            ApplyDeclarations(font, declarations);

            ApplyBorder(font, CssBorder.CreateBoxBorder(declarations));
            ApplyVerticalAlign(font, new CssVerticalAlign(declarations));
            ApplyBackgroundColor(font, new CssBackgroundColor(declarations));

            ApplySpecialFormatting(font, styleType, declarations);
        }

        [JavaAttributes.JavaThrows(true)]
        protected abstract void ApplySpecialFormatting(
            Font font,
            CssDeclarationCollection declarations,
            CssTextDecoration textDecoration,
            bool isInsideHtmlParagraph);

        [JavaAttributes.JavaThrows(true)]
        protected abstract void ApplySpecialFormatting(Font font, StyleType styleType, CssDeclarationCollection declarations);

        private static void ApplyDeclarations(Font font, CssDeclarationCollection declarations)
        {
            ApplyColor(font, declarations);
            ApplyDisplay(font, declarations);
            ApplyFontStyle(font, declarations);
            ApplyFontVariant(font, declarations);
            ApplyFontWeight(font, declarations);
            ApplyLetterSpacing(font, declarations);
            ApplyTextTransform(font, declarations);
            ApplyVisibility(font, declarations);
        }

        private static void ApplyLetterSpacing(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration declaration = declarations["letter-spacing"];
            if (declaration == null)
                return;

            Debug.Assert(declaration.Value.Count == 1);
            CssValue cssValue = declaration.Value.FirstValue;

            // Default is 0. Can be negative.
            double spacing = cssValue.Equals(CssValue.Normal) ? 0 : CssUtil.LengthToPoint(cssValue);
            if (!MathUtil.IsMinValue(spacing))
                font.Spacing = spacing;
        }

        private static void ApplyBorder(Font font, CssBorder border)
        {
            if (!border.IsUndefined)
            {
                border.ToModelBorder(font.Border);
            }
        }

        private static void ApplyColor(Font font, CssDeclarationCollection declarations)
        {
            DrColor color = declarations.GetColor("color");
            if (color != null)
            {
                font.ColorInternal = color;
            }
        }

        private static void ApplyDisplay(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration displayDeclaration = declarations["display"];
            if (displayDeclaration == null)
            {
                return;
            }

            Debug.Assert(displayDeclaration.Value.Count == 1);
            CssValue cssValue = displayDeclaration.Value.FirstValue;

            // According to the specification, if 'display:none' is declared for an element, all the child elements should be hidden,
            // no matter what 'display' property value they have. This behavior cannot be implemented using the IFontApplicable mechanism,
            // it's implemented in DocumentStyleResolver class (see ElementDisplayState property for details).
            // But this function is used when we create document styles based on CSS rules declared in a HTML document.
            if (cssValue.Equals(CssValue.None))
            {
                font.Hidden = true;
            }
        }

        private static void ApplyFontVariant(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration declaration = declarations["font-variant"];
            if (declaration == null)
            {
                return;
            }

            Debug.Assert(declaration.Value.Count == 1);
            CssValue cssValue = declaration.Value.FirstValue;

            font.SmallCaps = cssValue.Equals(CssValue.SmallCaps);
        }

        private static void ApplyFontStyle(Font font, CssDeclarationCollection declarations)
        {
            NullableBool isItalic = CssUtil.IsItalicFont(declarations, "font-style");
            if (NullableBoolUtil.HasValue(isItalic))
            {
                font.Italic = isItalic == NullableBool.True;
                font.ItalicBi = isItalic == NullableBool.True;
            }
        }

        private static void ApplyFontWeight(Font font, CssDeclarationCollection declarations)
        {
            NullableBool isBold = CssUtil.IsBoldFont(declarations, "font-weight");
            if (NullableBoolUtil.HasValue(isBold))
            {
                font.Bold = isBold == NullableBool.True;
                font.BoldBi = isBold == NullableBool.True;
            }
        }

        private static void ApplyTextTransform(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration declaration = declarations["text-transform"];
            if (declaration == null)
            {
                return;
            }

            Debug.Assert(declaration.Value.Count == 1);
            CssValue cssValue = declaration.Value.FirstValue;

            font.AllCaps = cssValue.Equals(CssValue.UpperCase);
        }

        private static void ApplyVerticalAlign(Font font, CssVerticalAlign verticalAlign)
        {
            if (verticalAlign.VerticalAlignValue == null)
            {
                return;
            }

            Debug.Assert(verticalAlign.VerticalAlignValue.Count == 1);
            CssValue cssValue = verticalAlign.VerticalAlignValue.FirstValue;
            switch (cssValue.ValueType)
            {
                case CssValueType.Identifier:
                {
                    if (cssValue.Equals(CssValue.Sub))
                        font.VerticalAlignment = RunVerticalAlignment.Subscript;
                    else if (cssValue.Equals(CssValue.Super))
                        font.VerticalAlignment = RunVerticalAlignment.Superscript;
                    else if (cssValue.Equals(CssValue.Baseline))
                        font.VerticalAlignment = RunVerticalAlignment.Baseline;
                    break;
                }
                default:
                {
                    double length = CssUtil.LengthToPoint(cssValue);
                    if (!MathUtil.IsMinValue(length))
                        font.Position = length;
                    break;
                }
            }
        }

        private static void ApplyVisibility(Font font, CssDeclarationCollection declarations)
        {
            CssDeclaration declaration = declarations["vertical-align"];
            if (declaration == null)
            {
                return;
            }

            Debug.Assert(declaration.Value.Count == 1);
            CssValue cssValue = declaration.Value.FirstValue;

            if (cssValue.Equals(CssValue.Hidden) || cssValue.Equals(CssValue.Collapse))
            {
                font.Hidden = true;
            }
        }

        private static void ApplyBackgroundColor(Font font, CssBackgroundColor backgroundColor)
        {
            backgroundColor.ToShading(font.Parent);
        }

        private readonly bool mApplyOverridableStyles;
    }
}
