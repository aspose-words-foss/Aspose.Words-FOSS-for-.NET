// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlBodyElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

            // Presentational hints.

            // The "link", "alink", and "vlink" attributes are not supported.
            // WORDSNET-8615 "background" attribute is not supported in HTML5 but we support it.
            TranslateBackgroundAttributeToCss(element, cssDeclarations);

            #region Margin attributes

            CssValue topMarginValue = MapFirstMetAttributeToMarginValue(element, new string[] { "marginheight", "topmargin" });
            if (topMarginValue != null)
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-top", topMarginValue));

            CssValue rightMarginValue = MapFirstMetAttributeToMarginValue(element, new string[] { "marginwidth", "rightmargin" });
            if (rightMarginValue != null)
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", rightMarginValue));

            CssValue bottomMarginValue = MapFirstMetAttributeToMarginValue(element, new string[] { "marginheight", "bottommargin" });
            if (bottomMarginValue != null)
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-bottom", bottomMarginValue));

            CssValue leftMarginValue = MapFirstMetAttributeToMarginValue(element, new string[] { "marginwidth", "leftmargin" });
            if (leftMarginValue != null)
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", leftMarginValue));

            #endregion

            TranslateBGColorAttributeToCss(element, cssDeclarations);

            // "text" attribute
            CssValue textValue = CssValue.ParseLegacyColor(element.GetAttributeValue("text"));
            if (textValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("color", textValue));
            }
        }

        private static CssValue MapFirstMetAttributeToMarginValue(IHtmlElementProvider element, string[] attributeNames)
        {
            CssValue result = null;
            foreach (string attributeName in attributeNames)
            {
                string attributeValue = element.GetAttributeValue(attributeName);
                if (attributeValue != null)
                {
                    result = CssValue.ParseLegacyPixelLength(attributeValue);
                    break;
                }
            }

            // The HTML 5 specification says that <body> elements should have default 8px margins.
            // We do not apply the default margins (return null), because in Word documents we have page margins.
            return result;
        }

        /// <summary>
        /// Translate 'background' attribute to corresponding CSS rule.
        /// </summary>
        private static void TranslateBackgroundAttributeToCss(IHtmlElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            // "background" attribute
            string backgroundUri = element.GetAttributeValue("background");
            if (backgroundUri != null)
            {
                CssValue backgroundValue = new CssUriValue(backgroundUri);
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("background-image", backgroundValue));
            }
        }
    }
}
