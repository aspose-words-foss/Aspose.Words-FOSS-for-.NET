// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/08/2024 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :visited pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/CSS2/selector.html#link-pseudo-classes
    /// </remarks>
    internal class CssVisitedSelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // Our HTML model don't support visited hyperlinks.
            return false;
        }

        internal override string ToCss()
        {
            return ":visited";
        }

        protected override string MakePreferableStyleName()
        {
            return "visited";
        }
    }
}
