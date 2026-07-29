// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2025 by Dmitry Burov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies internal layout modes for rendering multiple pages into a single output.
    /// </summary>
    internal enum MultiPageLayoutMode
    {
        /// <summary>
        /// A single page is rendered.
        /// If the specified page set contains multiple pages, only the first one is rendered.
        /// </summary>
        SinglePage,

        /// <summary>
        /// All pages from the specified page set are rendered vertically, one below another,
        /// within a single image or document.
        /// </summary>
        Vertical,

        /// <summary>
        /// All pages from the specified page set are rendered horizontally, one beside another,
        /// within a single image or document.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Pages are rendered in a grid layout with a fixed number of columns. When the column
        /// limit is reached, rendering continues in the next row.
        /// </summary>
        Grid,

        /// <summary>
        /// Each page is rendered as a separate frame in a multi-frame TIFF image.
        /// This mode is applicable only to TIFF image formats.
        /// </summary>
        TiffFrames
    }
}
