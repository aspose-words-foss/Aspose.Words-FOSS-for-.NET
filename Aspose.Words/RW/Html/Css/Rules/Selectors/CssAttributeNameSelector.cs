// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS 'attribute name' selector. For example, '[style]'.
    /// </summary>
    /// <remarks>
    /// The selector '[att]' selects an element with the "att" attribute, whatever the value of the attribute.
    /// For details see http://www.w3.org/TR/css3-selectors/#attribute-representation
    /// </remarks>
    internal class CssAttributeNameSelector : CssAttributeSelector
    {
        internal CssAttributeNameSelector(CssNamespace attributeNamespace, string attributeLocalName)
            : base(attributeNamespace, attributeLocalName, null, null)
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

            return element.GetAttributeValue(AttributeLocalName) != null;
        }
    }
}
