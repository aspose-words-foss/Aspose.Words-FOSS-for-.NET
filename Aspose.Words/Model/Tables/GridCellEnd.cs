// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2022 by Dmitry Matveenko

using System;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Indicates if a grid column is the last column spanned by a table cell.
    /// </summary>
    [Flags]
    internal enum GridCellEnd
    {
        /// <summary>
        /// Grid column matches no cell ends.
        /// </summary>
        None = 0,

        /// <summary>
        /// Grid column matches the end columns of a multi-column cell (but not single cells).
        /// </summary>
        Wide = 1,

        /// <summary>
        /// Grid column matches a single column cell.
        /// </summary>
        Single = 2,
    }
}
