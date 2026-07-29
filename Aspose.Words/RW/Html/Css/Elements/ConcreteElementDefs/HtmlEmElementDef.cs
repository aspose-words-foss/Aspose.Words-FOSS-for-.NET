// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlEmElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            // WORDSNET-27579 This declaration may or may not be overridable depending on the element's position
            // in the HTML tree. It may be removed at a later stage where the element's position is known.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("font-style", CssValue.Italic));
        }
    }
}
