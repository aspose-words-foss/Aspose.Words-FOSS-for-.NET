// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a sequence of simple CSS selectors.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#sequence-of-simple-selectors
    /// </remarks>
    internal class CssCompoundSelector : CssCombinableSelector
    {
        internal CssCompoundSelector(CssSimpleSelector head, CssCombinableSelector tail)
        {
            Debug.Assert(head != null);
            Debug.Assert(tail != null);

            // A pseudo-element must be the last component of a compound selector,
            // so the head cannot be a pseudo-element.
            Debug.Assert(!(head is CssPseudoElementSelector));

            mHead = head;
            mTail = tail;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // SPEED. We use the short-circuit logical and operator (&&) to skip tail selector matching if the head selector
            // doesn't match the element. This improves performance slightly, since selector matching is a very frequent
            // operation.
            return mHead.Selects(element, documentMode) && mTail.Selects(element, documentMode);
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return mHead.Specificity.Add(mTail.Specificity); }
        }

        internal override string ToCss()
        {
            return mHead.ToCss() + mTail.ToCss();
        }

        internal override bool SelectsPseudoElement()
        {
            // A pseudo-element must be the last component of a compound selector,
            // so the head cannot be a pseudo-element.
            return mTail.SelectsPseudoElement();
        }

        internal override HtmlElementPart SelectedPart
        {
            get
            {
                // A pseudo-element must be the last component of a compound selector, so we should look at the tail to tell
                // whether this selector selects a pseudo-element. The head always selects the real element.
                return mTail.SelectedPart;
            }
        }

        internal CssSimpleSelector Head
        {
            get { return mHead; }
        }

        internal CssCombinableSelector Tail
        {
            get { return mTail; }
        }

        protected override string MakePreferableStyleName()
        {
            return mHead.GetPreferableStyleName() + "_" + mTail.GetPreferableStyleName();
        }

        private readonly CssSimpleSelector mHead;

        private readonly CssCombinableSelector mTail;
    }
}
