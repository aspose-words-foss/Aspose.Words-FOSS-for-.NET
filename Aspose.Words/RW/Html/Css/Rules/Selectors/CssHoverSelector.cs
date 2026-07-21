// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :hover pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/CSS2/selector.html#dynamic-pseudo-classes
    /// </remarks>
    internal class CssHoverSelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // Our HTML model don't support dynamic states.
            return false;
        }

        internal override string ToCss()
        {
            return ":hover";
        }

        protected override string MakePreferableStyleName()
        {
            return "hover";
        }
    }
}
