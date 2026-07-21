// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a combinator part of a CSS selector (the combinator operation and the right-hand selector).
    /// Base class for concrete CSS combinators.
    /// </summary>
    internal abstract class CssCombinator
    {
        protected CssCombinator(CssCombinableSelector selector)
        {
            Debug.Assert(selector != null);

            mSelector = selector;
        }

        /// <summary>
        /// Gets the CSS declaration of the selector.
        /// </summary>
        internal abstract string ToCss();

        internal abstract string GetPreferableStyleName();

        internal CssCombinableSelector Selector
        {
            get { return mSelector; }
        }

        /// <summary>
        /// Returns a value indicating whether this combinator is supported by MS Word in HTML documents.
        /// </summary>
        internal virtual bool IsSupportedByMsWord
        {
            // Most combinators are not supported by MS Word.
            get { return false; }
        }

        private readonly CssCombinableSelector mSelector;
    }
}
