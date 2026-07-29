// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a record for a element in the user agent style sheet.
    /// Derived classes created for concrete elements should be registered in <see cref="DefaultElementStyleResolver" />.
    /// </summary>
    internal abstract class ElementDef
    {
        /// <summary>
        /// Adds default CSS style of the element to the collection of CSS declarations.
        /// </summary>
        /// <param name="element">
        /// The element whose default style is to be returned.
        /// </param>
        /// <param name="cssDeclarations">
        /// The collection of CSS declarations into which element's default CSS style is to be added.
        /// </param>
        /// <param name="omittedFormatting">
        /// Groups of user agent (default) CSS styles that are omitted from style resolution, because they are overridden
        /// by targed document formatting.
        /// </param>
        internal void ApplyDefaultCss(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations,
            CssUserAgentFormatting omittedFormatting)
        {
            // The 'dir' attribute affects 'unicode-bidi' but this style has lower priority than the default 'unicode-bidi'
            // style of the element, so it is applied before default styles.
            TranslateDirAttributeToCss(element, cssDeclarations);

            // Default styles of the element that cannot be overridden by target document formatting.
            ApplyStyles(element, cssDeclarations);

            // Default styles of the element that can be overridden by target document formatting.
            if ((omittedFormatting & CssUserAgentFormatting.Font) == 0)
            {
                ApplyOverridableFontStyles(element, cssDeclarations);
            }

            // Value of the 'hidden' attribute has higher priority than default 'display' style of the element,
            // so it is applied after default styles.
            if (CanBeHidden())
            {
                TranslateHiddenAttributeToCss(element, cssDeclarations);
            }
        }

        /// <summary>
        /// Indicates whether the element can be hidden completely by setting "display:none" when the "hidden" attribute 
        /// is applied to it.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the "hidden" attribute sets "display:none" on element. Otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanBeHidden()
        {
            // For most HTML elements the "hidden" attribute is translated to "display:none".
            return true;
        }

        /// <summary>
        /// Adds normal (mandatory) default styles of the element to the collection of CSS declarations.
        /// </summary>
        /// <param name="element">
        ///     The HTML element whose default styles are to be returned.
        /// </param>
        /// <param name="cssDeclarations">
        ///     The collection of CSS declarations into which element's default styles are to be added.
        /// </param>
        /// <remarks>
        /// Override in derived classes to add HTML element's default styles into the collection.
        /// </remarks>
        protected abstract void ApplyStyles(IElementProvider element, CssDeclarationCollectionBuilder cssDeclarations);

        /// <summary>
        /// Adds overridable default font styles of the element to the collection of CSS declarations.
        /// </summary>
        /// <param name="element">
        ///     The element whose default font styles are to be returned.
        /// </param>
        /// <param name="cssDeclarations">
        ///     The collection of CSS declarations into which element's default font styles are to be added.
        /// </param>
        /// <remarks>
        /// Override in derived classes to add element's overridable default font styles into the collection.
        /// </remarks>
        protected virtual void ApplyOverridableFontStyles(IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Empty method. Expected to be implemented in descendant classes.
        }

        /// <summary>
        /// Translate 'bgcolor' attribute to corresponding CSS rule.
        /// </summary>
        protected static void TranslateBGColorAttributeToCss(IElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // "bgcolor" attribute
            CssValue bgcolorValue = CssValue.ParseLegacyColor(element.GetAttributeValue("bgcolor"));
            if (bgcolorValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("background-color", bgcolorValue));
            }
        }

        /// <summary>
        /// Translate 'valign' attribute to corresponding CSS rule.
        /// </summary>
        protected static void TranslateVAlignAttributeToCss(IElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // "valign" attribute
            switch (element.GetAttributeValue("valign", string.Empty).ToLowerInvariant())
            {
                case "top":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Top));
                    break;
                case "middle":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Middle));
                    break;
                case "bottom":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Bottom));
                    break;
                case "baseline":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("vertical-align", CssValue.Baseline));
                    break;
                default:
                    // Other 'valign' attribute values are ignored.
                    break;
            }
        }

        /// <summary>
        /// Translate 'dir' common attribute to corresponding CSS rule.
        /// </summary>
        private static void TranslateDirAttributeToCss(IElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            switch (element.GetAttributeValue("dir", string.Empty).ToLowerInvariant())
            {
                case "ltr":
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("direction", CssValue.Ltr));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("unicode-bidi", CssValue.Embed));
                    break;
                }
                case "rtl":
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("direction", CssValue.Rtl));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("unicode-bidi", CssValue.Embed));
                    break;
                }
                default:
                {
                    // Other 'align' attribute values are ignored.
                    break;
                }
            }
        }

        /// <summary>
        /// Translate 'hidded' common attribute to corresponding CSS rule.
        /// </summary>
        private static void TranslateHiddenAttributeToCss(IElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            if (element.GetAttributeValue("hidden") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.None));
            }
        }
    }
}
