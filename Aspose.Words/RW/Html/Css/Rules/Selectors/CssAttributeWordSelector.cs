// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS 'attribute value word' selector. For example, '[lang~=en]'.
    /// </summary>
    /// <remarks>
    /// The selector '[att~=val]' selects an element with the "att" attribute whose value is a whitespace-separated list 
    /// of words, one of which is exactly "val". If "val" contains whitespace, it will never represent anything (since the words
    /// are separated by spaces). Also if "val" is the empty string, it will never represent anything.
    /// For details see http://www.w3.org/TR/css3-selectors/#attribute-representation
    /// </remarks>
    internal class CssAttributeWordSelector : CssAttributeSelector
    {
        internal CssAttributeWordSelector(CssNamespace attributeNamespace, string attributeLocalName, string value)
            : base(attributeNamespace, attributeLocalName, "~=", value)
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

            string[] parts = CssUtil.SplitClassAttributeValue(attributeValue);
            for (int i = 0; i < parts.Length; i++)
            {
                if (AttributeValueIsCaseInsensitive(AttributeLocalName))
                {
                    if (StringUtil.EqualsIgnoreCase(Value, parts[i]))
                    {
                        return true;
                    }
                }
                else
                {
                    // Ordinal case-sensitive comparison.
                    if (Value == parts[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
