// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies the types of connection points.
    /// </summary>
    internal enum ConnectionSiteType
    {
        /// <summary>
        /// No connection points.
        /// </summary>
        None = 0,
        /// <summary>
        /// The edit points of the shape are used as connection points.
        /// </summary>
        Segments = 1,
        /// <summary>
        /// A custom array of connection points.
        /// </summary>
        Custom = 2,
        /// <summary>
        /// Standard four connection points at midpoints of top, bottom, left and right sides.
        /// </summary>
        Rect = 3
    }
}
