// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a record for a HTML element in the user agent style sheet.
    /// Derived classes created for concrete HTML elements should be registered
    /// in <see cref="DefaultElementStyleResolver" />.
    /// </summary>
    internal abstract class HtmlElementDef : ElementDef
    {
        protected override void ApplyStyles(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            ApplyStyles((IHtmlElementProvider)element, cssDeclarations);
        }

        protected override void ApplyOverridableFontStyles(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            ApplyOverridableFontStyles((IHtmlElementProvider)element, cssDeclarations);
        }

        protected abstract void ApplyStyles(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations);

        protected virtual void ApplyOverridableFontStyles(
            IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Most HTML element don't have default overridable CSS styles.
        }
        /// <summary>
        /// Translate 'align' attribute to a corresponding CSS rule.
        /// </summary>
        protected static void TranslateAlignAttributeToCss(IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations, bool allowAbsmiddleValue)
        {
            const string textAlign = "text-align";

            switch (element.GetAttributeValue("align", string.Empty).ToLowerInvariant())
            {
                case "middle":
                case "center":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration(textAlign, CssValue.AwCenter));
                    break;
                case "absmiddle":
                    if (allowAbsmiddleValue)
                    {
                        cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration(textAlign, CssValue.Center));
                    }
                    break;
                case "left":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration(textAlign, CssValue.AwLeft));
                    break;
                case "right":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration(textAlign, CssValue.AwRight));
                    break;
                case "justify":
                    // WORDSNET-7232 MS Word processes "justify" value of "align" attribute as "start" (the initial value
                    // of the "text-align" attribute). So do we.
                    cssDeclarations.Remove(textAlign);
                    break;
                default:
                    // Other 'align' attribute values are ignored.
                    break;
            }
        }

        /// <summary>
        /// Translate 'height' attribute to corresponding CSS rule.
        /// </summary>
        protected static void TranslateHeightAttributeToCss(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // "height" attribute
            CssValue heightValue = CssValue.ParseLegacyDimension(element.GetAttributeValue("height"));
            if (heightValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("height", heightValue));
            }
        }

        /// <summary>
        /// Translate 'width' attribute to corresponding CSS rule.
        /// </summary>
        protected static void TranslateWidthAttributeToCss(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // "width" attribute
            CssValue widthValue = CssValue.ParseLegacyDimension(element.GetAttributeValue("width"));
            if (widthValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("width", widthValue));
            }
        }

    }
}
