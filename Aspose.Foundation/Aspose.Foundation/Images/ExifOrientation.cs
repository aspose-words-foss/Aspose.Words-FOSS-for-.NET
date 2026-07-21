// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2021 by Dmitry Sokolov

namespace Aspose.Images
{
    /// <summary>
    /// Orientation defined in the EXIF (exchangeable image file format) meta information.
    /// </summary>
    /// <remarks>
    /// This type of information is formatted according to the TIFF specification, and may be found in JPG, TIFF, PNG, JP2,
    /// PGF, MIFF, HDP, PSP and other images.
    /// </remarks>
    public enum ExifOrientation
    {
        /// <summary>
        /// Normal. Rotation none, flip none,
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Rotation none, flip X.
        /// </summary>
        MirrorHorizontal = 2,

        /// <summary>
        /// Rotation 180, flip none.
        /// </summary>
        Rotate = 3,

        /// <summary>
        /// Rotation none, flip Y.
        /// </summary>
        Mirror = 4,

        /// <summary>
        /// Rotation 270, flip X.
        /// </summary>
        MirrorHorizontalAndRotate270 = 5,

        /// <summary>
        /// Rotation 90, flip none.
        /// </summary>
        Rotate90 = 6,

        /// <summary>
        /// Rotation 90, flip X.
        /// </summary>
        MirrorHorizontalAndRotate90 = 7,

        /// <summary>
        /// Rotation 270, flip none.
        /// </summary>
        Rotate270 = 8
    }
}
