// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/05/2023 by Artem Shabarshin

using Aspose.Common;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Determines whether the input HTML document has fixed-page format with positioned elements.
    /// </summary>
    internal class HtmlFixedPageDetector
    {
        internal void ProcessHtmlElementNode(
            string name,
            string ns,
            CssDeclarationCollection declarations)
        {
            // If we're already sure that the document is fixed-page HTML, we skip all futher steps.
            if (IsFixedPageHtml)
            {
                return;
            }

            // Skip SVG.
            if (ns == W3CNamespaces.Svg)
            {
                return;
            }

            // Skip all insignificant elements.
            if ((name != "div") &&
                (name != "span") &&
                (name != "input") &&
                (name != "select"))
            {
                return;
            }

            // Each significant element in a fixed-page document must be absolutely positioned.
            if (declarations.ContainsIdentifier("position", "absolute"))
            {
                if ((declarations["top"] != null) ||
                    (declarations["right"] != null) ||
                    (declarations["bottom"] != null) ||
                    (declarations["left"] != null))
                {
                    IsFixedPageHtml = true;
                }
            }
        }

        internal bool IsFixedPageHtml { get; private set; }
    }
}
