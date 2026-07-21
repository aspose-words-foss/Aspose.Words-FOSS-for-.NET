// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

using System.Collections.Generic;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// When implemented, designates a one-dimension or rectangular area of table cells.
    /// </summary>
    internal interface ICellRange : IEnumerable<Cell>
    {
        /// <summary>
        /// Gets whether the range is rectangular.
        /// </summary>
        bool IsRectangular { get; }

        /// <summary>
        /// Gets whether the range contains one cell.
        /// </summary>
        bool IsOneCell { get; }

        /// <summary>
        /// Gets the upper left position of the range.
        /// </summary>
        CellPosition Start { get; }

        /// <summary>
        /// Gets the lower right position of the range.
        /// </summary>
        CellPosition End { get; }

        /// <summary>
        /// Gets the value for empty cell.
        /// </summary>
        Constant EmptyCellValue { get; }

        /// <summary>
        /// Gets whether the text in cells should be always evaluated or not.
        /// </summary>
        bool AlwaysEvaluateCellText { get; }
    }
}
