// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2018 by Victor Chebotok

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Indicates the format in which metafiles are saved to HTML documents.
    /// </summary>
    public enum HtmlMetafileFormat
    {
        /// <summary>
        /// Metafiles are rendered to raster PNG images.
        /// </summary>
        Png = 0,
        /// <summary>
        /// Metafiles are converted to vector SVG images.
        /// </summary>
        Svg = 1,
        /// <summary>
        /// Metafiles are saved as is, without conversion.
        /// </summary>
        EmfOrWmf = 2
    }
}
