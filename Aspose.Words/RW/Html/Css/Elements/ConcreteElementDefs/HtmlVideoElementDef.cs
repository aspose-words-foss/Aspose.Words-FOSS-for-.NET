// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlVideoElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // Presentational hints.

            TranslateHeightAttributeToCss(element, cssDeclarations);
            TranslateWidthAttributeToCss(element, cssDeclarations);
        }
    }
}
