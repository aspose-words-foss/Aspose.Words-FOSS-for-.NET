// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/01/2010 by Dmitry Vorobyev

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.JavaAttributes;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Designates a rectangular area of table cells.
    /// </summary>
    internal class RectangularCellRange : ICellRange
    {
        private RectangularCellRange(CellPosition start, CellPosition end)
        {
            mStart = start;
            mEnd = end;
        }

        /// <summary>
        /// Attempts to parse the specified string to a <see cref="RectangularCellRange"/> object.
        /// Returns an erroneous constant if there was a problem during parse.
        /// </summary>
        internal static Constant TryParse(
            FieldContext context,
            string name,
            bool areMultipleCellsAllowed,
            out ICellRange range)
        {
            range = null;

            Match match = gCellRangeRegex.Match(name);
            if (!match.Success)
                return null;

            Table rangeTable = null;
            Table bookmarkedTable = null;

            Group tableNameGroup = match.Groups[TableGroup];
            if (tableNameGroup.Success)
            {
                Constant result = TryGetBookmarkedTable(context, tableNameGroup.Value, out bookmarkedTable);
                if (result != null)
                {
                    // Something wrong with finding the bookmark.
                    return result;
                }
            }

            if (bookmarkedTable != null)
            {
                rangeTable = bookmarkedTable;
            }
            else if (context.ParentTable != null)
            {
                rangeTable = context.ParentTable;
            }

            Group oneCellGroup = match.Groups[OneCellGroup];
            if (oneCellGroup.Success)
            {
                // One cell/one row/one column range.
                range = Parse(rangeTable, oneCellGroup.Value, oneCellGroup.Value);
            }
            else if (match.Groups[RangeGroup].Success)
            {
                // Rectangular range.
                range = Parse(
                    rangeTable,
                    match.Groups[StartGroup].Value,
                    match.Groups[EndGroup].Value);
            }

            if (range == null)
            {
                return bookmarkedTable != null
                    ? ErrorConstant.CreateSyntaxError()
                    : null;
            }

            if (rangeTable == null)
                return new ErrorConstant("!The Formula Not In Table");

            if (!areMultipleCellsAllowed && !range.IsOneCell)
                return ErrorConstant.CreateSyntaxError();

            return null;
        }

        private static Constant TryGetBookmarkedTable(FieldContext context, string bookmarkName, out Table table)
        {
            table = null;

            // SPEED Get a bookmark from a cache.
            Bookmark bookmark = FieldUtil.GetCachedBookmark(context.Field, bookmarkName);
            if (bookmark == null)
            {
                // No such bookmark in the document.
                return new ErrorConstant(string.Format("!Undefined Bookmark, {0}", bookmarkName.ToUpper()));
            }

            Table bookmarkStartTable = (Table)bookmark.BookmarkStart.GetAncestor(NodeType.Table);
            Table bookmarkEndTable = (Table)bookmark.BookmarkEnd.GetAncestor(NodeType.Table);

            bool isBookmarkStartValid = (bookmarkStartTable != null);
            // Word treats okay a situation when a bookmark end is right after a table.
            bool isBookmarkEndValid = (bookmarkEndTable != null) ||
                (bookmark.BookmarkEnd.PreviousNonAnnotationSibling == bookmarkStartTable) ||
                (bookmark.BookmarkEnd.FirstNonMarkupParentNode.PreviousNonAnnotationSibling == bookmarkStartTable);

            if (!isBookmarkEndValid)
                return new ErrorConstant(string.Format("!{0} Is Not In Table", bookmarkName));

            if (!isBookmarkStartValid)
                return new DoubleConstant(0d);

            table = bookmarkStartTable;
            return null;
        }

        private static ICellRange Parse(Table table, string startName, string endName)
        {
            CellPosition startCell = CellPosition.TryParse(table, startName, 0);
            CellPosition endCell = CellPosition.TryParse(table, endName, int.MaxValue);
            return startCell != null && endCell != null
                ? new RectangularCellRange(startCell, endCell)
                : null;
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return new RectangularCellRangeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICellRange.IsRectangular
        {
            get { return true; }
        }

        bool ICellRange.IsOneCell
        {
            get { return CellPosition.AreSame(mStart, mEnd); }
        }

        CellPosition ICellRange.Start
        {
            get { return mStart; }
        }

        CellPosition ICellRange.End
        {
            get { return mEnd; }
        }

        Constant ICellRange.EmptyCellValue
        {
            get { return new DoubleConstant(0d); }
        }

        bool ICellRange.AlwaysEvaluateCellText
        {
            get { return true; }
        }

        /// <summary>
        /// Enumerates cells in a range.
        /// </summary>
        private sealed class RectangularCellRangeEnumerator : IEnumerator<Cell>
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="range"></param>
            internal RectangularCellRangeEnumerator(ICellRange range)
            {
                mRange = range;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            [JavaConvertCheckedExceptions]
            public bool MoveNext()
            {
                const int oneCellForward = 1;

                bool isNewPositionValid;
                if (mIsStarted)
                {
                    isNewPositionValid = mCurrentPosition.MoveRight(oneCellForward);
                }
                else
                {
                    mIsStarted = true;
                    mCurrentPosition = mRange.Start.Clone();
                    isNewPositionValid = mCurrentPosition.IsValid;
                }

                // Check the range.
                if (isNewPositionValid && (mCurrentPosition.ColumnIndex <= mRange.End.ColumnIndex))
                    return true;

                // End of range row. Move down until the first leftmost cell of the range is valid.
                mCurrentPosition.ColumnIndex = 0;
                while (mCurrentPosition.MoveDown(oneCellForward) && (mCurrentPosition.RowIndex <= mRange.End.RowIndex))
                {
                    // Move to leftmost range column.
                    if (mCurrentPosition.MoveRight(mRange.Start.ColumnIndex))
                        return true;
                }

                // No more rows.
                return false;
            }

            public void Reset()
            {
                mIsStarted = false;
            }

            public Cell Current
            {
                get { return mIsStarted ? mCurrentPosition.Cell : null; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly ICellRange mRange;
            private CellPosition mCurrentPosition;
            private bool mIsStarted;
        }

        private readonly CellPosition mStart;
        private readonly CellPosition mEnd;

        // Group indexes instead of names are required for auto-porting to Java.
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int TableGroup = 2;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int OneCellGroup = 4;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int RangeGroup = 7;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int StartGroup = 8;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int EndGroup = 9;

        private static readonly Regex gCellRangeRegex = new Regex(
            @"^((\w+)\s+)?" + // Table bookmark name (optional).
            @"((([A-Z]+\d+)|(R\d+C\d+))|" + // One cell range (A2 or R2C3).
            @"(([A-Z]+|\d+|[A-Z]+\d+):([A-Z]+|\d+|[A-Z]+\d+)))$", // Multiple cells range (A2:D2, B:B, 3:4).
            RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
