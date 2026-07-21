// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS descendant combinator. For example, the ' p' part of 'div p'.
    /// </summary>
    /// <remarks>
    /// The selector 'a b' selects a 'b' element that is a descendant of an 'a' element in the document tree.
    /// See http://www.w3.org/TR/css3-selectors/#descendant-combinators
    /// </remarks>
    internal class CssDescendantCombinator : CssCombinator
    {
        internal CssDescendantCombinator(CssCombinableSelector selector)
            : base(selector)
        {
            // Empty constructor.
        }

        internal override string ToCss()
        {
            return " " + Selector.ToCss();
        }

        internal override string GetPreferableStyleName()
        {
            return "_" + Selector.GetPreferableStyleName();
        }

        internal override bool IsSupportedByMsWord
        {
            get { return true; }
        }
    }
}
