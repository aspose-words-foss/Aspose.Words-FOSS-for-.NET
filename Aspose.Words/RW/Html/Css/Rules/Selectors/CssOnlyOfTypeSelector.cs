// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :only-of-type pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#only-of-type-pseudo
    /// </remarks>
    internal class CssOnlyOfTypeSelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            IElementProvider parent = element.GetParentElement();
            if (parent == null)
            {
                return false;
            }


            IElementProvider sibling = element.GetPreviousSiblingElement();
            while (sibling != null)
            {
                if ((sibling.ElementName == element.ElementName) && (sibling.ElementNamespace == element.ElementNamespace))
                    return false;
                sibling = sibling.GetPreviousSiblingElement();
            }

            sibling = element.GetNextSiblingElement();
            while (sibling != null)
            {
                if ((sibling.ElementName == element.ElementName) && (sibling.ElementNamespace == element.ElementNamespace))
                    return false;
                sibling = sibling.GetNextSiblingElement();
            }

            // The parent contains the current element, and there are no other child elements with the same name and namespace.
            return true;
        }

        internal override string ToCss()
        {
            return ":only-of-type";
        }

        protected override string MakePreferableStyleName()
        {
            return "only-of-type";
        }
    }
}
