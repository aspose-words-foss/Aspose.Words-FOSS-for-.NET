// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :only-child pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#only-child-pseudo
    /// </remarks>
    internal class CssOnlyChildSelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            IElementProvider parent = element.GetParentElement();
            if (parent == null)
                return false;

            // If the element's parent has only one child, this child must be this element.
            IElementProvider firstChild = parent.GetFirstChildElement();
            return (firstChild != null) && (firstChild.GetNextSiblingElement() == null);
        }

        internal override string ToCss()
        {
            return ":only-child";
        }

        protected override string MakePreferableStyleName()
        {
            return "only-child";
        }
    }
}
