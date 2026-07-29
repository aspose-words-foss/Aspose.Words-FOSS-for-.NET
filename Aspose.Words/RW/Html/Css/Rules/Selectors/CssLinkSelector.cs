// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :link pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/CSS2/selector.html#link-pseudo-classes
    /// </remarks>
    internal class CssLinkSelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // In HTML4, the link pseudo-classes applies to A elements with an "href" attribute. 
            return (element.ElementName == "a") && (element.GetAttributeValue("href") != null);
        }

        internal override string ToCss()
        {
            return ":link";
        }

        protected override string MakePreferableStyleName()
        {
            return "link";
        }
    }
}
