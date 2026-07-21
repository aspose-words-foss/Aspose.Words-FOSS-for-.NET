// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a complex CSS selector, which contains a combinable selector and one or more combinators.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#combinators
    /// </remarks>
    internal class CssComplexSelector : CssSelector
    {
        internal CssComplexSelector(CssCombinableSelector left, IList<CssCombinator> combinators)
        {
            Debug.Assert(left != null);
            Debug.Assert(combinators != null);
            Debug.Assert(combinators.Count > 0);

            mLeftSelector = left;
            mCombinators = combinators;

            mSpecificity = left.Specificity;
            foreach (CssCombinator combinator in combinators)
            {
                mSpecificity = mSpecificity.Add(combinator.Selector.Specificity);
            }
        }

        internal override HtmlElementPart SelectedPart
        {
            get
            {
                // Only the last selector in a combination is allowed to select a pseudo-element. All other selectors select
                // the real element itself and there is no need to check them.
                return mCombinators[mCombinators.Count - 1].Selector.SelectedPart;
            }
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return mSpecificity; }
        }

        internal override string ToCss()
        {
            StringBuilder result = new StringBuilder();
            result.Append(mLeftSelector.ToCss());
            foreach (CssCombinator combinator in mCombinators)
            {
                result.Append(combinator.ToCss());
            }
            return result.ToString();
        }

        /// <summary>
        /// Creates an instance of <see cref="CssComplexSelectorMatcher"/> to match this selector against HTML elements 
        /// in the specified mode.
        /// </summary>
        internal override CssSelectorMatcher CreateMatcher(CssDocumentMode documentMode)
        {
            return new CssComplexSelectorMatcher(this, documentMode);
        }

        /// <summary>
        /// Converts this complex selector to an array of combinators.
        /// The leftmost selector becomes <see cref="CssDescendantCombinator"/>. Other combinators are copied.
        /// </summary>
        internal CssCombinator[] ToCombinatorArray()
        {
            CssCombinator[] result = new CssCombinator[mCombinators.Count + 1];
            result[0] = new CssDescendantCombinator(mLeftSelector);
            for (int i = 0; i < mCombinators.Count; i++)
            {
                result[i + 1] = mCombinators[i];
            }
            return result;
        }

        protected override string MakePreferableStyleName()
        {
            StringBuilder result = new StringBuilder();
            result.Append(mLeftSelector.GetPreferableStyleName());
            foreach (CssCombinator combinator in mCombinators)
            {
                result.Append(combinator.GetPreferableStyleName());
            }
            return result.ToString();
        }

        private readonly CssCombinableSelector mLeftSelector;

        private readonly IList<CssCombinator> mCombinators;

        private readonly CssSelectorSpecificity mSpecificity;
    }
}
