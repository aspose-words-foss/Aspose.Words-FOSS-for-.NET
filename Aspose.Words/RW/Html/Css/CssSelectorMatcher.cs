// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2017 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Matches a <see cref="CssSelector"/> against HTML elements during depth-first traversal of an HTML tree.
    /// </summary>
    internal abstract class CssSelectorMatcher
    {
        protected CssSelectorMatcher(CssSelector selector, CssDocumentMode documentMode)
        {
            Debug.Assert(selector != null);

            mSelector = selector;
            mDocumentMode = documentMode;
        }

        internal abstract bool Push(IElementProvider element);

        internal abstract void Pop();

        internal CssSelector Selector
        {
            get { return mSelector; }
        }

        internal CssDocumentMode DocumentMode
        {
            get { return mDocumentMode; }
        }

        private readonly CssSelector mSelector;
        private readonly CssDocumentMode mDocumentMode;
    }
}
