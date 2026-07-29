// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for all CSS pseudo-element selectors.
    /// </summary>
    internal class CssPseudoElementSelector : CssSimpleSelector
    {
        internal CssPseudoElementSelector(HtmlElementPart part, string name)
        {
            Debug.Assert(part != HtmlElementPart.Element);
            Debug.Assert(StringUtil.HasChars(name));
            Debug.Assert((name == "before") ||
                         (name == "after") ||
                         (name == "first-line") ||
                         (name == "first-letter"));

            mPart = part;
            mName = name;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // Pseudo-elements always exist. If an element itself is selected, all its pseudo-elements are selected as well.
            return true;
        }

        internal override HtmlElementPart SelectedPart
        {
            get { return mPart; }
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        internal override string ToCss()
        {
            bool pseudoElementIsDefinedInCss21 =
                (mPart == HtmlElementPart.Before) ||
                (mPart == HtmlElementPart.After) ||
                (mPart == HtmlElementPart.FirstLetter) ||
                (mPart == HtmlElementPart.FirstLine);

            // In CSS 2.1, the 'one-colon' syntax was used for pseudo-elements. For example, ':before' instead of '::before'.
            string prefix = pseudoElementIsDefinedInCss21
                ? ":"
                : "::";

            return prefix + mName;
        }

        internal override bool SelectsPseudoElement()
        {
            return true;
        }

        protected override string MakePreferableStyleName()
        {
            return mName;
        }

        private readonly HtmlElementPart mPart;

        private readonly string mName;

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(0, 0, 1);
    }
}
