// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/06/2019 by Anton Savko

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Type of a list item marker imported from HTML.
    /// </summary>
    internal enum HtmlMarkerType
    {
        /// <summary>
        /// Marker specified by '-aw-*' CSS properties.
        /// </summary>
        Aw,

        /// <summary>
        /// Marker specified by a &lt;ul&gt; HTML element.
        /// </summary>
        Html,

        /// <summary>
        /// Marker specified by a "::before" or "::after" pseudo-element.
        /// </summary>
        PseudoElement
    }
}
