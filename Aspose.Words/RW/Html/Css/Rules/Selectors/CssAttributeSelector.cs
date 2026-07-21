// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/05/2013 by Victor Chebotok

using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for all selectors that select attribute name or value.
    /// </summary>
    internal abstract class CssAttributeSelector : CssSimpleSelector
    {
        protected CssAttributeSelector(
            CssNamespace attributeNamespace,
            string attributeLocalName,
            string selectorOperator,
            string value)
        {
            Debug.Assert(attributeNamespace != null);
            Debug.Assert(StringUtil.HasChars(attributeLocalName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(attributeLocalName));
            // If the "operator" parameter is specified, the "value" parameter must must be non-null but it may be an empty
            // string. If "operator" is empty, "value" must be null.
            Debug.Assert(StringUtil.HasChars(selectorOperator) == (value != null));

            AttributeNamespace = attributeNamespace;
            AttributeLocalName = attributeLocalName;
            mSelectorOperator = selectorOperator;
            Value = value;
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        internal override string ToCss()
        {
            string escapedName = CssEscape.EscapeIdentifier(AttributeLocalName);
            string prefixedName = AttributeNamespace.GetPrefixedName(escapedName, CssNamespace.Empty.Name);

            if (StringUtil.HasChars(mSelectorOperator))
            {
                string escapedValue = CssEscape.EscapeDoubleQuotedString(Value);
                return "[" + prefixedName + mSelectorOperator + "\"" + escapedValue + "\"]";
            }
            else
            {
                return "[" + prefixedName + "]";
            }
        }

        protected override string MakePreferableStyleName()
        {
            return StringUtil.HasChars(mSelectorOperator)
                ? AttributeLocalName + mSelectorOperator + Value
                : AttributeLocalName;
        }

        protected CssNamespace AttributeNamespace { get; private set; }

        protected string AttributeLocalName { get; private set; }

        protected string Value { get; private set; }

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(0, 1, 0);

        private readonly string mSelectorOperator;
    }
}
