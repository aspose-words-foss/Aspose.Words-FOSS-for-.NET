// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlMarkElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("background-color", gYellowColorValue));
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("color", CssValue.Black));
        }

        private static readonly CssHashValue gYellowColorValue = CssHashValue.FromColor(DrColor.Yellow);
    }
}
