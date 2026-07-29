// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/03/2013 by Victor Chebotok

using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlHRElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("color", CssValue.Gray));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-width", CssValue.OnePx));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-width", CssValue.OnePx));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-width", CssValue.OnePx));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Inset));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Inset));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Inset));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Inset));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", CssValue.Auto));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", CssValue.Auto));

            // Presentational hints.

            // "align" attribute
            switch (element.GetAttributeValue("align", string.Empty).ToLowerInvariant())
            {
                case "center":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", CssValue.Auto));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", CssValue.Auto));
                    break;
                case "left":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", CssValue.Zero));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", CssValue.Auto));
                    break;
                case "right":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-left", CssValue.Auto));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-right", CssValue.Zero));
                    break;
                default:
                    // Other 'align' attribute values are ignored.
                    break;
            }

            // Border style if both "color" and "noshade" attribute is not present.
            if ((element.GetAttributeValue("color") != null) || (element.GetAttributeValue("noshade") != null))
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-style", CssValue.Solid));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-style", CssValue.Solid));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-style", CssValue.Solid));
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-style", CssValue.Solid));

                // "color" attribute
                // The following code violates the HTML 5 specification, but browsers do the same.
                CssValue colorValue = CssValue.ParseLegacyColor(element.GetAttributeValue("color"));
                if (colorValue != null)
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", colorValue));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", colorValue));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", colorValue));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", colorValue));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("background-color", colorValue));
                }
                else if (element.GetAttributeValue("noshade") != null)
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-top-color", CssValue.Gray));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-right-color", CssValue.Gray));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-color", CssValue.Gray));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-left-color", CssValue.Gray));
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("background-color", CssValue.Gray));
                }
            }

            // "size" attribute
            // The algrorithm of parsing a "size" attribute value provided in HTML 5 is too complicated.
            // It states that "size" must affect "border-size" properties, but it is not the case in modern browsers.
            // The following code mimics Google Chrome behavior instead.
            // This code assumes that a UA style sheet sets HR's "border-width" properties to 1px.
            string sizeValueStr = element.GetAttributeValue("size");
            if (sizeValueStr == null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("height", CssValue.Zero));
            }
            else
            {
                int sizeValue = HtmlUtil.ParseNonNegativeInteger(sizeValueStr);
                if (sizeValue <= 1)
                {
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("border-bottom-width", CssValue.Zero));
                }
                CssValue heightValue = (sizeValue <= 2)
                    ? (CssValue)CssValue.Zero
                    : new CssLengthValue(sizeValue - 2, CssUnit.Px);
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("height", heightValue));
            }

            TranslateWidthAttributeToCss(element, cssDeclarations);
        }
    }
}
