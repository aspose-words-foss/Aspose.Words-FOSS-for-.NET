// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Identifies a part of an HTML element to which CSS formatting is applied.
    /// </summary>
    /// <remarks>
    /// CSS formatting can be applied not only to whole CSS elements but also to specific parts of an element -
    /// to pseudo-elements, identified by double colon (::) syntax.
    /// Examples of such parts are: first letter (::first-letter) and first line of text (::first-line), additional content
    /// before (::before) or after (::after) the element. Not all of them are supported by Aspose.Words.
    /// </remarks>
    internal enum HtmlElementPart
    {
        /// <summary>
        /// A real HTML element as a whole. By default, CSS formatting is applied to this part.
        /// </summary>
        Element,

        /// <summary>
        /// Additional content inserted before the real element (::before).
        /// </summary>
        Before,

        /// <summary>
        /// Additional content inserted after the real element (::after).
        /// </summary>
        After,

        /// <summary>
        /// First letter of text inside the real element (::first-letter).
        /// </summary>
        FirstLetter,

        /// <summary>
        /// First line of text inside the real element (::first-line).
        /// </summary>
        FirstLine
    }
}
