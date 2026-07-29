// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// The base class for selectors that can be a part of a selector combination (of a complex selector). This includes
    /// simple selectors and sequences of simple selectors.
    /// </summary>
    internal abstract class CssCombinableSelector : CssSelector
    {
        /// <summary>
        /// Creates a new complex selector that matches elements that are matched by this selector and are children of the
        /// specified ancestor. In other words, this method creates a child combinator of the ancestor and this selectors.
        /// </summary>
        internal CssComplexSelector ChildOf(CssCombinableSelector ancestor)
        {
            // Only the last selector in a combination is allowed to select a pseudo-element.
            // For example, 'div:before > p:after' is an invalid CSS selector.
            Debug.Assert(!ancestor.SelectsPseudoElement());

            List<CssCombinator> combinators = new List<CssCombinator>(1);
            combinators.Add(new CssChildCombinator(this));
            return new CssComplexSelector(ancestor, combinators);
        }

        /// <summary>
        /// Indicates whether the selector selects the specified HTML element.
        /// </summary>
        internal abstract bool Selects(IElementProvider element, CssDocumentMode documentMode);

        /// <summary>
        /// Indicates whether the selector references a pseudo-element.
        /// </summary>
        /// <returns><c>true</c> if the selector contains a pseudo-element name (for example, ::first-letter),
        /// otherwise <c>false</c>.</returns>
        internal virtual bool SelectsPseudoElement()
        {
            // Most descendant classes do not reference a pseudo-element.
            return false;
        }

        /// <summary>
        /// Creates an instance of <see cref="CssCombinableSelectorMatcher"/> to match this selector against HTML elements
        /// in the specified mode.
        /// </summary>
        internal override CssSelectorMatcher CreateMatcher(CssDocumentMode documentMode)
        {
            return new CssCombinableSelectorMatcher(this, documentMode);
        }
    }
}
