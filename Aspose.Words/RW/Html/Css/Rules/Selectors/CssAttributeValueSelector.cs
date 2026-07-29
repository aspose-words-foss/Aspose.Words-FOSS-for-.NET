// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS 'attribute value' selector. For example, '[lang=en]'.
    /// </summary>
    /// <remarks>
    /// The selector '[att=val]' selects an element with the "att" attribute whose value is exactly "val".
    /// For details see http://www.w3.org/TR/css3-selectors/#attribute-representation
    /// </remarks>
    internal class CssAttributeValueSelector : CssAttributeSelector
    {
        internal CssAttributeValueSelector(CssNamespace attributeNamespace, string attributeLocalName, string value)
            : base(attributeNamespace, attributeLocalName, "=", value)
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

            string attributeValue = element.GetAttributeValue(AttributeLocalName);
            if (attributeValue == null)
            {
                return false;
            }
            else
            {
                if (AttributeValueIsCaseInsensitive(AttributeLocalName))
                {
                    return StringUtil.EqualsIgnoreCase(Value, attributeValue);
                }
                else
                {
                    // Case-sensitive ordinal comparison.
                    return Value == attributeValue;
                }
            }
        }
    }
}
