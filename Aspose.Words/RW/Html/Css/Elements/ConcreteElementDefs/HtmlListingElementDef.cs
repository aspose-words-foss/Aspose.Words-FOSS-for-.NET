// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlListingElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(gStyles);
        }

        private static readonly CssDeclarationCollection gStyles = new CssDeclarationCollection(
            new CssSpecifiedDeclaration("display", CssValue.Block),
            new CssSpecifiedDeclaration("font-family", new CssFontFamilyPropertyValue(CssValue.Monospace)),
            new CssSpecifiedDeclaration("white-space", CssValue.Pre));
    }
}
