// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/03/2013 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Text wrapping options for a text box when tight wrapping is set for the text box.
    /// </summary>
    /// <remarks>
    /// See [ECMA-376] Part 4, Section 2.18.99 ST_TextboxTightWrap.
    /// </remarks>
    internal enum TextboxTightWrap
    {
        /// <summary>
        /// No lines of the paragraph allow the surrounding text to tightly wrap around their edges.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// All lines of the paragraph allow the surrounding text to tightly wrap to their edges.
        /// </summary>
        AllLines = 0x01,

        /// <summary>
        /// Only the first and last lines of the paragraph allow the surrounding text to tightly wrap around their edges.
        /// </summary>
        FirstAndLastLine = 0x02,

        /// <summary>
        /// Only the first line of the paragraph allows the surrounding text to tightly wrap around its edges.
        /// </summary>
        FirstLineOnly = 0x03,

        /// <summary>
        /// Only the last line of the paragraph allows the surrounding text to tightly wrap around its edges.
        /// </summary>
        LastLineOnly,

        Default = None
    }
}
