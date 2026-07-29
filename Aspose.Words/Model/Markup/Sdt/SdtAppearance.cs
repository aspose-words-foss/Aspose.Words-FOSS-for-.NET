// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2016 by Alexander Zhiltsov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies the appearance of a structured document tag.
    /// </summary>
    /// <dev>
    /// Represents items of the 2.5.4.2 ST_SdtAppearance simple type.
    /// </dev>
    public enum SdtAppearance
    {
        /// <summary>
        /// Represents a structured document tag shown as a shaded rectangle or bounding box.
        /// </summary>
        BoundingBox,

        /// <summary>
        /// Represents a structured document tag shown as start and end markers.
        /// </summary>
        Tags,

        /// <summary>
        /// Represents a structured document tag that is not shown.
        /// </summary>
        Hidden,

        /// <summary>
        /// Defaults to <see cref="BoundingBox"/>.
        /// </summary>
        Default = BoundingBox
    }
}
