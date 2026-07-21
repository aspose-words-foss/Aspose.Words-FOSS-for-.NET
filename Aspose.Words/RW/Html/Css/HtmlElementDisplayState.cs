// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/10/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents HTML element's display state based on CSS 'display' and 'visibility' properties.
    /// </summary>
    /// <remarks>
    /// The support for visibility:collapse is missing or partially incorrect in some modern browsers. We ignore it too.
    /// </remarks>
    internal enum HtmlElementDisplayState
    {
        /// <summary>
        /// Element is visible.
        /// </summary>
        Visible,
        /// <summary>
        /// Element should be hidden (see 'visibility:hidden').
        /// </summary>
        Hidden,
        /// <summary>
        /// Element shouldn't be displayed (see 'display:none'). 
        /// </summary>
        None
    }
}