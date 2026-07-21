// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2005 by Roman Korchagin

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Celler
{
    /// <summary>
    /// Represents a table row in the celler table model, see CellerTable for more info.
    /// </summary>
    internal class CellerRow
    {
        internal CellerRow(int columnCount, Row rowNode)
        {
            Cells = new CellerCell[columnCount];
            RowNode = rowNode;
        }

        internal Row RowNode { get; }

        internal CellerCell[] Cells { get; }

        /// <summary>
        /// Indicates whether all cells of this row are vertically merged with cells of the previous row.
        /// </summary>
        internal bool IsMerged { get; set; }

        /// <summary>
        /// Indicates how many merged rows are located above this row (including this row itself).
        /// </summary>
        internal int MergedRowsAbove { get; set; }

        /// <summary>
        /// The total height of this row and all adjacent merged rows right below it.
        /// </summary>
        internal double MergedRowsHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this row belongs to the table header.
        /// </summary>
        internal bool IsHeading { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this row is the first row in its row group (header or body).
        /// </summary>
        internal bool IsFirstInGroup { get; set; }
    }
}
