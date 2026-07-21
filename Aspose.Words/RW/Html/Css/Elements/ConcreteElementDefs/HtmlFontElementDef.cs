// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlFontElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            // "color" attribute
            CssValue colorValue = CssValue.ParseLegacyColor(element.GetAttributeValue("color"));
            if (colorValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("color", colorValue));
            }

            // "face" attribute
            string faceValue = element.GetAttributeValue("face");
            if ((faceValue != null) && (faceValue.Trim() != string.Empty))
            {
                // WORDSNET-4667 Multiple font families specified in HTML are not imported as expected. 
                // Attribute "face" may have list of fonts separated by comma.
                string[] fontValues = faceValue.Split(',');

                CssValueList cssValueList = new CssValueList();
                foreach (string value in fontValues)
                {
                    string fontValue = value.Trim();
                    if (!string.IsNullOrEmpty(fontValue))
                    {
                        cssValueList.Add(CssValue.CreateFontFamilyValue(fontValue));
                    }
                }

                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration(
                    "font-family",
                    new CssFontFamilyPropertyValue(cssValueList)));
            }

            // "size" attribute
            CssValue sizeValue = CssValue.ParseLegacyFontSize(element.GetAttributeValue("size"));
            if (sizeValue != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-size", sizeValue));
            }
        }
    }
}
