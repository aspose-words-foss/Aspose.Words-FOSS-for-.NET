// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :nth-child() pseudo-class selector. For example, ':nth-child(3n+1)'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#nth-child-pseudo
    /// </remarks>
    internal class CssNthChildSelector : CssPseudoClassSelector
    {
        internal CssNthChildSelector(CssIndexArgument argument)
        {
            Debug.Assert(argument != null);
            mArgument = argument;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            IElementProvider parent = element.GetParentElement();
            if (parent == null)
                return false;

            IElementProvider[] childElements = parent.GetChildElements();
            for (int i = 0; i < childElements.Length; i++)
            {
                if (childElements[i] == element)
                    return mArgument.Matches(i);
            }

            // We should not get here. If the element has a parent, the parent's list of children must contain the element.
            Debug.Assert(false);
            return false;
        }

        internal override string ToCss()
        {
            return ":nth-child(" + mArgument.GetText() + ")";
        }

        protected override string MakePreferableStyleName()
        {
            return "nth-child(" + mArgument.GetText() + ")";
        }

        private readonly CssIndexArgument mArgument;
    }
}
