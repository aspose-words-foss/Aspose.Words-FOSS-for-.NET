// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/22/2020 by Artem Ptitsin.

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the watermark type.
    /// </summary>
    public enum WatermarkType
    {
        /// <summary>
        /// Indicates that the text will be used as a watermark.
        /// <p>Such a watermark corresponds to a WordArt object.</p>
        /// </summary>
        Text,

        /// <summary>
        /// Indicates that the image will be used as a watermark.
        /// <p>Such a watermark corresponds to a shape with image.</p>
        /// </summary>
        Image,

        /// <summary>
        /// Indicates watermark is no set.
        /// </summary>
        None
    }
}
