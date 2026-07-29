// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2008 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Txt.Writer
{
    /// <summary>
    /// Builds lines of text representing a table-like structure.
    /// </summary>
    internal class TxtTableBuilder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TxtTableBuilder(Table table, TxtTableBuilder parentBuilder, TxtSaveOptionsBase saveOptions)
        {
            // WORDSNET-22293 The character width depends on the font size of the text within the table.
            CharWidth = GetApproximateCharWidth(table);

            // WORDSNET-7125 Outputting a table with negative left indent to txt causes exception.
            // For negative left indent to work we need to move all txt output to the right,
            // but let's not bother and output the table from zero position instead.
            mTableLeftIndent = PointsToChars(System.Math.Max(table.LeftIndent, 0));

            mParentBuilder = parentBuilder;
            mSaveOptions = saveOptions;
            if (parentBuilder != null)
            {
                // We should start a nested table from the next line.
                mRowStartLineIndex = Lines.CurrentLineIndex;
                mRowEndLineIndex = Lines.CurrentLineIndex;
            }

            NestedLineIndex = 0;
        }

        internal void StartRow()
        {
            mCell = null;
            mCellPosition = mTableLeftIndent;
        }

        internal void EndRow()
        {
            mRowStartLineIndex = mRowEndLineIndex;

            if (mParentBuilder != null)
                mParentBuilder.NestedLineIndex = mRowEndLineIndex;
        }

        /// <summary>
        /// Start writing of cell.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>True if the cell should be processed, or false otherwise.</returns>
        internal bool StartCell(Cell cell)
        {
            mCell = cell;

            if (ShouldSkipCell())
                return false;

            Lines.CurrentLineIndex = mRowStartLineIndex;
            return true;
        }

        internal void EndCell()
        {
            if (ShouldSkipCell())
                return;

            // Shift cell position.
            mCellPosition += GetCellWidthInChars();
            // Register the maximal line number to be used by the next row.
            mRowEndLineIndex = System.Math.Max(mRowEndLineIndex, Lines.CurrentLineIndex);

            // WORDSNET-21385 If this cell contains a nested table, we must consider nested content as well.
            if (NestedLineIndex > 0)
                HandleNestedContent();
        }

        /// <summary>
        /// Consider nested content to determine the correct <see cref="mRowEndLineIndex"/>
        /// </summary>
        private void HandleNestedContent()
        {
            mRowEndLineIndex = System.Math.Max(mRowEndLineIndex, NestedLineIndex);

            // Reset the value once it was used.
            NestedLineIndex = 0;
        }

        /// <summary>
        /// Starts writing of a paragraph.
        /// </summary>
        internal void StartParagraph(Paragraph paragraph)
        {
            mCurParagraph = paragraph;
            mCurParagraphText.Length = 0;
        }

        /// <summary>
        /// Writes accumulated paragraph text to the content lines.
        /// </summary>
        internal void EndParagraph()
        {
            if (mCurParagraphText.Length <= CharsLeftInCurrentCell)
            {
                AppendTextToCurrentLine(mCurParagraphText.ToString());
            }
            else
            {
                AppendTextWithWordWrap(mCurParagraphText.ToString());
            }

            Lines.NewLine();
            mCurParagraphText.Length = 0;
        }

        /// <summary>
        /// Appends a specified text to the current paragraph string builder.
        /// </summary>
        internal void AppendText(string text)
        {
            // Do not add paragraph breaks directly as it will break the layout.
            if (text == mSaveOptions.ParagraphBreak)
            {
                EndParagraph();
            }
            else
            {
                mCurParagraphText.Append(text);
            }
        }

        /// <summary>
        /// Returns text that is aligned inside a current cell.
        /// </summary>
        private string ApplyAlignment(string text, ParagraphAlignment alignment)
        {
            switch (alignment)
            {
                case ParagraphAlignment.Right:
                {
                    return text.PadLeft(CharsLeftInCurrentCell);
                }
                case ParagraphAlignment.Center:
                {
                    int spaces = CharsLeftInCurrentCell - text.Length;
                    // Do not pad text from the right as we do it when appending text to a current line later. This avoids
                    // writing of extra spaces for a very last cells.
                    return text.PadLeft((spaces / 2) + text.Length);
                }
                default:
                    return text;
            }
        }

        private bool ShouldSkipCell()
        {
            // Do not process merged cells.
            return mCell.CellPr.IsMergedToPrevious;
        }

        /// <summary>
        /// Appends a specified text to the current line.
        /// </summary>
        private void AppendTextToCurrentLine(string text)
        {
            if (text.Length == 0)
                return;

            // Append space if needed.
            if (PositionInCurrentLine < 0)
                mLines.CurrentLine.Append(' ', -PositionInCurrentLine);

            // WORDSNET-19050 Apply paragraph alignment to the text.
            string alignedText = ApplyAlignment(text, mCurParagraph.ParaPr.Alignment);

            Lines.CurrentLine.Append(alignedText);
        }

        /// <summary>
        /// Appends a specified text to the current line with word wrapping.
        /// </summary>
        private void AppendTextWithWordWrap(string text)
        {
            int start = 0;

            while (start < text.Length)
            {
                int length = GetMaxSubstringLength(text, start);
                string textToAppend = text.Substring(start, length);
                AppendTextToCurrentLine(textToAppend);

                start += length;
                // Start a new line for each line except of a very last one.
                if (start < text.Length)
                    Lines.NewLine();
            }
        }

        /// <summary>
        /// Returns the maximum length of the substring that may be appended
        /// to the current line in the current cell so that words are wrapped correctly.
        /// </summary>
        private int GetMaxSubstringLength(string text, int startPosition)
        {
            Debug.Assert(StringUtil.HasChars(text));

            // WORDSNET-24250 If there is no room for new characters in this line of cell, then go to the next line.
            // Note, non-empty cell in our design has minimal length equal to one char (see GetCellWidthInChars()).
            while ((CharsLeftInCurrentCell < 1))
                Lines.CurrentLineIndex++;

            int lastBreakPosition = 0;
            int charsLeft = System.Math.Min(text.Length - startPosition, CharsLeftInCurrentCell);
            bool isInWord = !char.IsWhiteSpace(text[startPosition]);

            for (int i = 0; i < charsLeft; i++)
            {
                int position = startPosition + i;
                char c = text[position];

                if (Char.IsWhiteSpace(c))
                {
                    if (isInWord)
                    {
                        lastBreakPosition = position;
                        isInWord = false;
                    }
                }
                else
                {
                    isInWord = true;
                }
            }

            int result;
            if (lastBreakPosition == 0)
                result = charsLeft;   // Handle long words or spaces.
            else
                result = lastBreakPosition - startPosition;

            Debug.Assert(result >= 0);
            return result;
        }

        /// <summary>
        /// The index of line, which is the last line of the last row of the nested table.
        /// </summary>
        private int NestedLineIndex { get; set; }

        private int GetCellWidthInChars()
        {
            if (mCell == null)
                return 0;
            double cellWidthInPoints = ConvertUtilCore.TwipToPoint(GetCellWidthInTwips());
            return System.Math.Max(PointsToChars(cellWidthInPoints), 1);
        }

        private int GetCellWidthInTwips()
        {
            CellPr firstCellPr = mCell.CellPr;

            if (firstCellPr.HorizontalMerge == CellMerge.None)
                return firstCellPr.Width;

            int width = firstCellPr.Width;
            for (Cell cell = mCell.NextCell; cell != null; cell = cell.NextCell)
            {
                if (cell.CellPr.HorizontalMerge != CellMerge.Previous)
                    break;
                width += cell.CellPr.Width;
            }

            return width;
        }

        /// <summary>
        /// Converts a value in points into a value in characters.
        /// </summary>
        private int PointsToChars(double points)
        {
            Debug.Assert(points >= 0);
            return (int)(points / CharWidth);
        }

        /// <summary>
        /// Gets the approximate character width that depends on the font size of the text within the table.
        /// </summary>
        private static double GetApproximateCharWidth(Table table)
        {
            return GetApproximateCharWidth(GetTotalCharsPerFontSize(table));
        }

        /// <summary>
        /// Gets the dictionary with total characters per font size of the text within the table.
        /// </summary>
        private static Dictionary<double, long> GetTotalCharsPerFontSize(Table table)
        {
            Debug.Assert(table != null);

            Dictionary<double, long> totalCharsPerFontSize = new Dictionary<double, long>();

            foreach (Run run in table.GetChildNodes(NodeType.Run, true))
            {
                if (totalCharsPerFontSize.ContainsKey((double)run.RunPr.Size))
                    totalCharsPerFontSize[(double)run.RunPr.Size] += run.Text.Length;
                else
                    totalCharsPerFontSize.Add((double)run.RunPr.Size, (long)run.Text.Length);
            }
            return totalCharsPerFontSize;
        }

        /// <summary>
        /// Gets the approximate character width that depends on the specified dictionary with
        /// total characters per font size of the text within the table.
        /// </summary>
        private static double GetApproximateCharWidth(Dictionary<double, long> totalCharsPerFontSize)
        {
            const double widthDividedToHeight = 0.6;
            const double calibrationCoefficient = 3.5;

            Debug.Assert(totalCharsPerFontSize != null);

            if (totalCharsPerFontSize.Count == 0)
                return DefaultCharWidth;

            double numerator = 0;
            long totalCharsQuantity = 0;
            foreach (KeyValuePair<double, long> sizeAndQuantity in totalCharsPerFontSize)
            {
                totalCharsQuantity += sizeAndQuantity.Value;
                numerator += (sizeAndQuantity.Value * sizeAndQuantity.Key);
            }
            return (((numerator * widthDividedToHeight) / totalCharsQuantity) / calibrationCoefficient);
        }

        internal TxtContentLines Lines
        {
            get
            {
                // We share line collection between nested table builders so only create it for the first builder.
                if (mLines == null)
                    mLines = (mParentBuilder != null) ? mParentBuilder.Lines : new TxtContentLines();
                return mLines;
            }
        }

        /// <summary>
        /// Returns the number of characters we exceeded beyond the current cell position. May be negative.
        /// </summary>
        private int PositionInCurrentLine
        {
            get { return (Lines.CurrentLine.Length - AbsoluteCellPosition); }
        }

        /// <summary>
        /// Returns the number of characters we exceeded beyond the current cell position.
        /// </summary>
        private int PositionInCurrentCell
        {
            get { return System.Math.Max(PositionInCurrentLine, 0); }
        }

        /// <summary>
        /// Returns the number of characters left until the end of the current cell.
        /// </summary>
        private int CharsLeftInCurrentCell
        {
            get { return (GetCellWidthInChars() - PositionInCurrentCell); }
        }

        /// <summary>
        /// Returns the X position of the current cell relatively to the beginning of the current line.
        /// </summary>
        private int AbsoluteCellPosition
        {
            get { return (ParentCellPosition + mCellPosition); }
        }

        /// <summary>
        /// Returns the X position of the parent cell relatively to the beginning of the current line.
        /// </summary>
        private int ParentCellPosition
        {
            get { return (mParentBuilder != null) ? mParentBuilder.AbsoluteCellPosition : 0; }
        }

        /// <summary>
        /// A width of a character in points used to compute how to break cell contents into lines
        /// obtained from the first run of the cell.
        /// </summary>
        private double CharWidth { get; set; }

        /// <summary>
        /// Measures in "chars".
        /// </summary>
        private readonly int mTableLeftIndent;
        /// <summary>
        /// The previous table builder.
        /// </summary>
        private readonly TxtTableBuilder mParentBuilder;

        private readonly TxtSaveOptionsBase mSaveOptions;
        /// <summary>
        /// The lines of the table.
        /// </summary>
        private TxtContentLines mLines;
        /// <summary>
        /// The current cell.
        /// </summary>
        private Cell mCell;
        /// <summary>
        /// The current horizontal offset of the cell in chars.
        /// </summary>
        private int mCellPosition;
        /// <summary>
        /// The index of the current row's first line.
        /// </summary>
        private int mRowStartLineIndex;
        /// <summary>
        /// The index of the current row's last line.
        /// </summary>
        private int mRowEndLineIndex;
        /// <summary>
        /// A string builder to accumulate a text of the current paragraph.
        /// </summary>
        private readonly StringBuilder mCurParagraphText = new StringBuilder();
        /// <summary>
        /// The current paragraph.
        /// </summary>
        private Paragraph mCurParagraph;
        /// <summary>
        /// The default width of a character in points.
        /// </summary>
        private const double DefaultCharWidth = 8.0;
    }
}
