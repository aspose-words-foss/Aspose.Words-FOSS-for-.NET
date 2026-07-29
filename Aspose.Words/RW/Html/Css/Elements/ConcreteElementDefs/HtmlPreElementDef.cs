// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlPreElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-family",
                                                                     new CssFontFamilyPropertyValue(CssValue.Monospace)));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.Pre));

            // Presentational hints.

            if (element.GetAttributeValue("wrap") != null)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("white-space", CssValue.PreWrap));
            }
        }
    }
}
