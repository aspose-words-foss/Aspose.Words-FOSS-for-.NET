// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2013 by Alexey Butalov

using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS type selector. For example, 'div'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#type-selectors
    /// </remarks>
    internal class CssTypeSelector : CssSimpleSelector
    {
        internal CssTypeSelector(CssNamespace elementNamespace, string elementLocalName)
        {
            Debug.Assert(elementNamespace != null);
            Debug.Assert(StringUtil.HasChars(elementLocalName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(elementLocalName));

            mNamespace = elementNamespace;
            mLocalName = elementLocalName;
        }

        /// <summary>
        /// Creates a selector that matches elements that are both matched by this selector and have the specified class.
        /// In other words, this method appends '.className' to this selector.
        /// </summary>
        internal CssCompoundSelector WithClass(string className)
        {
            CssClassSelector classSelector = ClassSelector(className);
            return new CssCompoundSelector(this, classSelector);
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // Element names are lowercase.  
            return (element.ElementName == mLocalName) && mNamespace.Matches(element.ElementNamespace);
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        internal override string ToCss()
        {
            string escapedElementName = CssEscape.EscapeIdentifier(mLocalName);
            return mNamespace.GetPrefixedName(escapedElementName, CssNamespace.Any.Name);
        }

        internal CssNamespace Namespace
        {
            get { return mNamespace; }
        }

        internal string ElementName
        {
            get { return mLocalName; }
        }

        protected override string MakePreferableStyleName()
        {
            return mLocalName;
        }

        private readonly CssNamespace mNamespace;

        private readonly string mLocalName;

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(0, 0, 1);
    }
}
