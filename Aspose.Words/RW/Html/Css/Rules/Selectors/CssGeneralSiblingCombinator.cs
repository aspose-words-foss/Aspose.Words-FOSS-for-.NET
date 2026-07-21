// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS general sibling combinator. For example, the '~ p' part of 'div ~ p'.
    /// </summary>
    /// <remarks>
    /// The 'a ~ b' selector selects a 'b' element if both 'a' and 'b' elements share the same parent and the 'a' element
    /// precedes the 'b' element.
    /// See http://www.w3.org/TR/css3-selectors/#general-sibling-combinators
    /// </remarks>
    internal class CssGeneralSiblingCombinator : CssCombinator
    {
        public CssGeneralSiblingCombinator(CssCombinableSelector selector)
            : base(selector)
        {
            // Empty constructor.
        }

        internal override string ToCss()
        {
            return " ~ " + Selector.ToCss();
        }

        internal override string GetPreferableStyleName()
        {
            return " ~ " + Selector.GetPreferableStyleName();
        }
    }
}
