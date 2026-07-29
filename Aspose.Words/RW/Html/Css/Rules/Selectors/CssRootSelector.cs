// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS :root pseudo-class selector.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#root-pseudo
    /// </remarks>
    internal class CssRootSelector : CssPseudoClassSelector
    {
        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // In HTML documents, the root is always the <html> element.
            return element.ElementName == "html";
        }

        internal override string ToCss()
        {
            return ":root";
        }

        protected override string MakePreferableStyleName()
        {
            return "root";
        }
    }
}
