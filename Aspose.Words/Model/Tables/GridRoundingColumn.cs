// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/12/2021 by Dmitry Matveenko

namespace Aspose.Words.Tables
{
    internal enum GridRoundingColumn
    {
        /// <summary>
        /// Round twips or content maximums, depending on the column type.
        /// </summary>
        TwipsOrContentMax,

        /// <summary>
        /// Round minimum or content minimums, depending on the grid type.
        /// </summary>
        Minimum,

        /// <summary>
        /// Round the final width, it is used after resizing.
        /// </summary>
        Width,
    }
}
