// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2016 by Dmitry Matveenko

using System;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Indicates the type of cell preferred width a grid column matches.
    /// </summary>
    [Flags]
    internal enum GridCellMatchType
    {
        /// <summary>
        /// Column matches no cells.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Column matches a cell with preferred width auto.
        /// </summary>
        Auto = 0x1,

        /// <summary>
        /// Column matches a cell with preferred width in twips.
        /// </summary>
        Twip = 0x2,

        /// <summary>
        /// Column matches a cell with preferred width in percent units.
        /// </summary>
        Percent = 0x4,
    }
}
