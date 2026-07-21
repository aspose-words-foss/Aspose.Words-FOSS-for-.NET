// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlLIElementDef : HtmlElementDef
    {
        internal HtmlLIElementDef(bool applyFormattingAsMsWord)
        {
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.ListItem));
            // This style is applied only by Chrome. Firefox and Internet Explorer do not specify any special text alignment
            // on <li> elements. Although this style is uncommon, we still apply it for the following reasons:
            //  - during export this style helps to style <li> elements so that they look identical in all browsers;
            //  - Chrome is the most popular browser, so it is reasonable to use this browser's styles as a reference
            //    during import; moreover, Chrome formatting looks more consistent when a list contains mixed LTR and RTL items.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("text-align", CssValue.AwMatchParent));

            if (mApplyFormattingAsMsWord)
            {
                return;
            }

            // Presentational hints.

            // "type" attribute
            string type = element.GetAttributeValue("type", string.Empty);

            switch (type)
            {
                case "1":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Decimal));
                    break;
                case "a":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.LowerAlpha));
                    break;
                case "A":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.UpperAlpha));
                    break;
                case "i":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.LowerRoman));
                    break;
                case "I":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.UpperRoman));
                    break;
                default:
                    // SQ fix: nothing to do.
                    break;
            }

            switch (type.ToLowerInvariant())
            {
                case "disc":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Disc));
                    break;
                case "circle":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Circle));
                    break;
                case "square":
                    cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("list-style-type", CssValue.Square));
                    break;
                default:
                    // SQ fix: nothing to do.
                    break;
            }
        }

        private readonly bool mApplyFormattingAsMsWord;
    }
}
