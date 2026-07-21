// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlAbbrElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            if (element.GetAttributeValue("title") != null)
            {
                cssDeclarations.AddOrReplace(gDefaultStyle);
            }
        }

        private static readonly CssDeclarationCollection gDefaultStyle = new CssDeclarationCollection(
            new CssSpecifiedDeclaration("border-bottom-style", CssValue.Dotted),
            new CssSpecifiedDeclaration("border-bottom-width", CssValue.OnePx));
    }
}
