// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS 'attribute dash match' selector. For example, '[lang|=en]'.
    /// </summary>
    /// <remarks>
    /// The selector '[att|=val]' selects an element with the "att" attribute, its value either being exactly "val" 
    /// or beginning with "val" immediately followed by "-" (U+002D).
    /// For details see http://www.w3.org/TR/css3-selectors/#attribute-representation
    /// </remarks>
    internal class CssAttributeDashPrefixSelector : CssAttributeSelector
    {
        internal CssAttributeDashPrefixSelector(CssNamespace attributeNamespace, string attributeLocalName, string value)
            : base(attributeNamespace, attributeLocalName, "|=", value)
        {
            // Empty constructor. Everything is set up by the base class.
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // HTML has no means to specify namespaces of attributes, and none of HTML attribute has a namespace.
            if (AttributeNamespace.IsSpecific && !AttributeNamespace.IsEmpty)
            {
                return false;
            }

            string actualValue = element.GetAttributeValue(AttributeLocalName);
            if (actualValue == null)
            {
                return false;
            }

            if (AttributeValueIsCaseInsensitive(AttributeLocalName))
            {
                if (StringUtil.EqualsIgnoreCase(actualValue, Value))
                {
                    return true;
                }
            }
            else
            {
                // Ordinal (case-sensitive) comparison.
                if (actualValue == Value)
                {
                    return true;
                }
            }

            string prefix = Value + '-';
            if (actualValue.Length < prefix.Length)
            {
                return false;
            }

            string attributePrefix = actualValue.Substring(0, prefix.Length);
            if (AttributeValueIsCaseInsensitive(AttributeLocalName))
            {
                return StringUtil.EqualsIgnoreCase(attributePrefix, prefix);
            }
            else
            {
                // Ordinal (case-sensitive) comparison.
                return attributePrefix == prefix;
            }
        }
    }
}
