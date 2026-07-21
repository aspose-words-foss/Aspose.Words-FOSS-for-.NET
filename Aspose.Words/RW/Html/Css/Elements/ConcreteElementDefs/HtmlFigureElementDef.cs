// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlFigureElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(gDefaultStyle);
        }

        private static readonly CssValue gFortyPx = new CssLengthValue(40, CssUnit.Px);

        private static readonly CssDeclarationCollection gDefaultStyle = new CssDeclarationCollection(
            new CssSpecifiedDeclaration("display", CssValue.Block),
            new CssSpecifiedDeclaration("margin-left", gFortyPx),
            new CssSpecifiedDeclaration("margin-right", gFortyPx));

    }
}
