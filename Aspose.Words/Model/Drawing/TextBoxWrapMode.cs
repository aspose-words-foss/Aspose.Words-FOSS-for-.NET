// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/07/2006 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies how text wraps inside a shape.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags",
        Justification = "Not used as a flag.")]
    public enum TextBoxWrapMode
    {
        /// <summary>
        /// Text wraps inside a shape.
        /// </summary>
        Square = 0,
// In MS Word I've only seen Square and None.
//        ByPoints = 1,
        /// <summary>
        /// Text does not wrap inside a shape.
        /// </summary>
        None = 2,
//        TopBottom = 3,
//        Through = 4
    }
}
