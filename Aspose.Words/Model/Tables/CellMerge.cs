// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Specifies how a cell in a table is merged with other cells.
    /// </summary>
    public enum CellMerge
    {
        /// <summary>
        /// The cell is not merged.
        /// </summary>
        None = 0,
        /// <summary>
        /// The cell is the first cell in a range of merged cells.
        /// </summary>
        First = 1,
        /// <summary>
        /// The cell is merged to the previous cell horizontally or vertically.
        /// </summary>
        Previous = 2
    }
}
