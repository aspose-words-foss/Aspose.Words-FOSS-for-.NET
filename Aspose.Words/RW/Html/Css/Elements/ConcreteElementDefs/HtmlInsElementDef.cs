// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class HtmlInsElementDef : HtmlElementDef
    {
        protected override void ApplyStyles(IHtmlElementProvider element, CssDeclarationCollectionBuilder cssDeclarations)
        {
            // User agent (default) style.

            // WORDSNET-13034 Browsers apply 'text-decoration: underline' to <ins> elements but MS Word ignores this
            // default style. Insertion revisions appear underlined in MS Word without additional formatting. We mimic
            // MS Word's behavior and don't apply the default 'text-decoration' on <ins> elements.
        }
    }
}
