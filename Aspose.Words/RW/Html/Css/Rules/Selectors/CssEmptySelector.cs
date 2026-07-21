// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :empty pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#empty-pseudo
    /// </remarks>
    internal class CssEmptySelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // Check that the element contains neither other elements nor text nodes.
            return (element.GetFirstChildElement() == null) && (element.GetInnerText().Length == 0);
        }

        internal override string ToCss()
        {
            return ":empty";
        }

        protected override string MakePreferableStyleName()
        {
            return "empty";
        }
    }
}
