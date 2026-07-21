// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS 'attribute value suffix' selector. For example, '[lang$=en]'.
    /// </summary>
    /// <remarks>
    /// The selector '[att$=val]' selects an element with the "att" attribute whose value ends with the suffix "val".
    /// If "val" is the empty string then the selector does not represent anything.
    /// For details see http://www.w3.org/TR/css3-selectors/#attribute-substrings
    /// </remarks>
    internal class CssAttributeSuffixSelector : CssAttributeSelector
    {
        internal CssAttributeSuffixSelector(CssNamespace attributeNamespace, string attributeLocalName, string value)
            : base(attributeNamespace, attributeLocalName, "$=", value)
        {
            // Empty constructor. Everything is set up by the base class.
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // HTML has no means to specify namespaces of attributes, and all of HTML attribute have empty namespace.
            if (AttributeNamespace.IsSpecific && !AttributeNamespace.IsEmpty)
            {
                return false;
            }

            if (Value.Length == 0)
            {
                return false;
            }

            string actualValue = element.GetAttributeValue(AttributeLocalName);
            if ((actualValue == null) || (actualValue.Length < Value.Length))
            {
                return false;
            }

            string attributeSuffix = actualValue.Substring(actualValue.Length - Value.Length, Value.Length);
            if (AttributeValueIsCaseInsensitive(AttributeLocalName))
            {
                return StringUtil.EqualsIgnoreCase(attributeSuffix, Value);
            }
            else
            {
                // Ordinal (case-sensitive) comparison.
                return attributeSuffix == Value;
            }
        }
    }
}
