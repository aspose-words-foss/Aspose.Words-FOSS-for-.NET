// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :nth-of-type() pseudo-class selector. For example, ':nth-of-type(3n+1)'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#nth-of-type-pseudo
    /// </remarks>
    internal class CssNthOfTypeSelector : CssPseudoClassSelector
    {
        internal CssNthOfTypeSelector(CssIndexArgument argument)
        {
            Debug.Assert(argument != null);
            mArgument = argument;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            IElementProvider parent = element.GetParentElement();
            if (parent == null)
                return false;

            // Determine and check the index of this element among the children that have the same name and namespace.
            int index = 0;
            IElementProvider[] childElements = parent.GetChildElements();
            for (int i = 0; i < childElements.Length; i++)
            {
                IElementProvider child = childElements[i];
                if (child == element)
                    return mArgument.Matches(index);

                if ((child.ElementName == element.ElementName) && (child.ElementNamespace == element.ElementNamespace))
                    ++index;
            }

            // We should not get here. If the element has a parent, the parent's list of children must contain
            // the element.
            Debug.Assert(false);
            return false;
        }

        internal override string ToCss()
        {
            return ":nth-of-type(" + mArgument.GetText() + ")";
        }

        protected override string MakePreferableStyleName()
        {
            return "nth-of-type(" + mArgument.GetText() + ")";
        }

        private readonly CssIndexArgument mArgument;
    }
}
