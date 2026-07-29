// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS universal selector (asterisk, *).
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#universal-selector
    /// </remarks>
    internal class CssUniversalSelector : CssSimpleSelector
    {
        internal CssUniversalSelector(CssNamespace ns)
        {
            Debug.Assert(ns != null);

            mNamespace = ns;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            return mNamespace.Matches(element.ElementNamespace);
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        internal override string ToCss()
        {
            return mNamespace.GetPrefixedName("*", CssNamespace.Any.Name);
        }

        protected override string MakePreferableStyleName()
        {
            return "any";
        }

        private readonly CssNamespace mNamespace;

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(0, 0, 0);
    }
}
