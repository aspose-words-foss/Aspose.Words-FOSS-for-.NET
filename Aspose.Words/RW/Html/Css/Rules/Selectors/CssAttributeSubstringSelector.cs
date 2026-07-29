// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS 'attribute value substring' selector. For example, '[lang*=en]'.
    /// </summary>
    /// <remarks>
    /// The selector '[att*=val]' selects an element with the "att" attribute whose value contains at least one instance 
    /// of the substring "val". If "val" is the empty string then the selector does not represent anything.
    /// For details see http://www.w3.org/TR/css3-selectors/#attribute-substrings
    /// </remarks>
    internal class CssAttributeSubstringSelector : CssAttributeSelector
    {
        internal CssAttributeSubstringSelector(CssNamespace attributeNamespace, string attributeLocalName, string value)
            : base(attributeNamespace, attributeLocalName, "*=", value)
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

            if (Value.Length == 0)
            {
                return false;
            }

            string actualValue = element.GetAttributeValue(AttributeLocalName);
            if (actualValue == null)
            {
                return false;
            }

            return StringUtil.Contains(actualValue, Value, AttributeValueIsCaseInsensitive(AttributeLocalName));
        }
    }
}
