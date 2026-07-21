// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2017 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Matches a <see cref="CssCombinableSelector"/> against HTML elements during depth-first traversal of an HTML tree.
    /// </summary>
    internal class CssCombinableSelectorMatcher : CssSelectorMatcher
    {
        internal CssCombinableSelectorMatcher(CssCombinableSelector selector, CssDocumentMode documentMode)
            : base(selector, documentMode)
        {
            // Empty constructor.
        }

        internal override bool Push(IElementProvider element)
        {
            // Implicit (anonymous) elements are not in the HTML tree and they cannot be selected by selectors.
            return (!element.IsImplicit) &&
                ((CssCombinableSelector)Selector).Selects(element, DocumentMode);
        }

        internal override void Pop()
        {
            // Empty method. Nothing to do.
        }
    }
}
