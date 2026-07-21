// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/06/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlFormElementDef : HtmlElementDef
    {
        internal HtmlFormElementDef(CssDocumentMode documentMode)
        {
            mDocumentMode = documentMode;
        }

        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.
            cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("display", CssValue.Block));

            // Presentational hints.
            if (mDocumentMode == CssDocumentMode.Quirks)
            {
                cssDeclarations.AddOrReplace(new CssSpecifiedDeclaration("margin-bottom", new CssLengthValue(1, CssUnit.Em)));
            }
        }

        private readonly CssDocumentMode mDocumentMode;
    }
}