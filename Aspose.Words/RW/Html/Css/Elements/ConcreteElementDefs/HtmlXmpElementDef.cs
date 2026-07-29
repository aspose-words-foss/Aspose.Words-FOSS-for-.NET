// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlXmpElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-family", new CssFontFamilyPropertyValue(CssValue.Monospace)));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Pre));
        }
    }
}
