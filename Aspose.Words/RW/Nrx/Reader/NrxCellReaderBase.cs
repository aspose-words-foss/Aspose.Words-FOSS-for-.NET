// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides method for parsing 'w:tc' element.
    /// NOTE: Derived classes should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxCellReaderBase
    {
        protected NrxCellReaderBase(NrxStoryReaderBase storyReader, 
            NrxParaPrReaderBase paraPrReader,
            NrxCellPrReaderBase cellPrReader)
        {
            Debug.Assert(storyReader != null);
            Debug.Assert(paraPrReader != null);
            Debug.Assert(cellPrReader != null);
            mStoryReader = storyReader;
            mParaPrReader = paraPrReader;
            mCellPrReader = cellPrReader;
        }

        /// <summary>
        /// Reads 'w:tc' element.
        /// </summary>
        internal void Read(NrxDocumentReaderBase reader)
        {
            Cell cell = new Cell(reader.Document);
            reader.AddAndPushContainer(cell);

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("tc"))
            {
                switch (xmlReader.LocalName)
                {
                    case "tcPr":
                        mCellPrReader.Read(reader, cell.CellPr);
                        break;
                    case "pPr":
                        mParaPrReader.Read(reader, reader.GlobalParaPr, reader.GlobalParagraphBreakRunPr);
                        break;
                    case "tcW":
                        // WORDSNET-4327 Resiliency. Normally tcW occurs inside tcPr, but we've got one here
                        // and MS Word seems to read it okay. So do we now.
                        cell.CellPr.SetPreferredWidthAndWidth(xmlReader.ReadLength(reader.ComplianceInfo));
                        break;
                    case "hMerge":
                        // WORDSNET-15721 "Horizontally merged cell" element occurred in unexpected place. 
                        // Implement resiliency and read "hMerge" element from "tc" level. 
                        NrxCellPrReaderBase.ReadHMerge(xmlReader, cell.CellPr);
                        break;
                    default:
                        // WORDSNET-13301 Text of second and later cells in a merged cell range was included in extracted
                        // text, although Fixed Grid Calculator replaces such cells with grid spans. So, do not read 
                        // children of that cells on fixed table layout (when Table.AllowAutoFit is true). 

                        if (!reader.IsDocx ||
                            !reader.LoadOptions.SkipFormatting ||
                            cell.ParentTable.AllowAutoFit ||
                            !IsContinueHMergingCell(cell))
                        {
                            mStoryReader.ReadChild(reader);
                        }
                        break;
                }
            }

            reader.PopContainer(NodeType.Cell);
        }

        /// <summary>
        /// Returns flag indicating that the specified cell is second or later cell in a horizontally merging range.
        /// </summary>
        private static bool IsContinueHMergingCell(Cell cell)
        {
            if (cell.CellPr.HorizontalMerge != CellMerge.Previous)
                return false;

            // MS Word treats CellMerge.Previous as None unless there is a preceding CellMerge.First.
            Cell firstCell = cell.PreviousCell;
            while ((firstCell != null) && (firstCell.CellPr.HorizontalMerge == CellMerge.Previous))
                firstCell = firstCell.PreviousCell;

            return (firstCell != null) && (firstCell.CellPr.HorizontalMerge == CellMerge.First);
        }

        private readonly NrxStoryReaderBase mStoryReader;
        private readonly NrxParaPrReaderBase mParaPrReader;
        private readonly NrxCellPrReaderBase mCellPrReader;
    }
}
