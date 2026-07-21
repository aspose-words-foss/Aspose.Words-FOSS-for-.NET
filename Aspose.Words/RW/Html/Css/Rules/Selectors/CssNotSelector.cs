// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :not() pseudo-class selector. For example, ':not(div)'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#negation
    /// </remarks>
    internal class CssNotSelector : CssPseudoClassSelector
    {
        internal CssNotSelector(CssSimpleSelector argument)
        {
            Debug.Assert(argument != null);

            mArgument = argument;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            return !mArgument.Selects(element, documentMode);
        }

        internal override CssSelectorSpecificity Specificity
        {
            get
            {
                // The :not() pseudo-class itself is not counted.
                return mArgument.Specificity;
            }
        }

        internal override string ToCss()
        {
            return ":not(" + mArgument.ToCss() + ")";
        }

        protected override string MakePreferableStyleName()
        {
            return "not(" + mArgument.ToCss() + ")";
        }

        private readonly CssSimpleSelector mArgument;
    }
}
