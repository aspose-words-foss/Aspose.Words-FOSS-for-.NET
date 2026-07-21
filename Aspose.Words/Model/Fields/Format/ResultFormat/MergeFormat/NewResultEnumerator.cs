// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2010 by Dmitry Vorobyev

using System.Collections.Generic;
using System.Text;
using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Enumerates a "new" field result when applying MERGEFORMAT. Collects tokens and formats them.
    /// </summary>
    internal class NewResultEnumerator : ResultEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private NewResultEnumerator(
            IEnumerable<Node> resultNodes,
            Paragraph startParagraph,
            Paragraph endParagraph,
            OldResultEnumerator oldResultEnumerator)
            : base(resultNodes, startParagraph)
        {
            Debug.Assert(!oldResultEnumerator.IsInitialized);

            mEndParagraph = endParagraph;
            mOldResultEnumerator = oldResultEnumerator;
        }

        /// <summary>
        /// Merges formattings of new result runs and paragraphs with the old result ones.
        /// </summary>
        /// <remarks>
        /// MS Word format merging algorithm splits text of runs into tokens e.g. words and separators in both
        /// result node sequences and then overrides new result token formattings by old result ones based
        /// nothing but token order. So word's formatting may be overridden by separator's one and vise versa.
        /// Paragraph breaks are treated as separators and become full participants of this overriding too.
        /// Note however that if old result token count inside given paragraph is less than new result one,
        /// then remaining new result tokens' formattings will not be overridden. Otherwise, if these counts
        /// are not equal, remainder of old result token formattings will be moved to next paragraph and so on.
        /// </remarks>
        internal static void MergeFormat(
            IEnumerable<Node> resultNodes,
            Paragraph startParagraph,
            Paragraph endParagraph,
            OldResultEnumerator oldResultEnumerator,
            Paragraph oldEndParagraph)
        {
            NewResultEnumerator newResultEnumerator = new NewResultEnumerator(
                resultNodes,
                startParagraph,
                endParagraph,
                oldResultEnumerator);

            newResultEnumerator.MergeFormat(oldEndParagraph);
        }

        private void MergeFormat(Paragraph oldEndParagraph)
        {
            // mOldResultEnumerator must be moved first we to have an opportunity to use its RunPr property
            // while moving current enumerator.
            while (MoveResultsNext())
            {
                if (IsCurrentTokenParagraphBreak)
                {
                    Paragraph oldResultParagraph = mOldResultEnumerator.IsCurrentTokenParagraphBreak
                        ? mOldResultEnumerator.LastParagraph
                        : mOldResultEnumerator.CurrentParagraph;

                    SetLastParagraphPr(oldResultParagraph);

                    // New result paragraph is reached. Unlock processing if it has been locked previously.
                    UnlockProcessing();
                    mOldResultEnumerator.Unsuspend();
                }
                else if (IsCurrentTokenTable)
                {
                    MergeTables(CurrentTable, mOldResultEnumerator.CurrentTable);
                }
                else if (!mIsProcessingLocked)
                {
                    InsertCurrentText(mOldResultEnumerator.RunPr, false);

                    if (mOldResultEnumerator.IsCurrentTokenParagraphBreak)
                    {
                        // Old result paragraph is reached while new is not.
                        // Lock processing until new result paragraph is reached too.
                        LockProcessing();
                        mOldResultEnumerator.Suspend();
                    }
                }
            }

            FinalizeProcessing();
            RemoveRuns();

            // It seems like new result end paragraph formatting is always equal to old result end paragraph's one.
            if (oldEndParagraph != null)
                SetParaPr(mEndParagraph, oldEndParagraph.ParaPr, oldEndParagraph.ParagraphBreakRunPr);

            // If old result has paragraphs less than new one then new result current paragraph properties must be set.
            if (CurrentParagraph != null && CurrentParagraph != mEndParagraph && mOldResultEnumerator.CurrentParagraph != null)
                SetParaPr(CurrentParagraph, mOldResultEnumerator.CurrentParagraph.ParaPr, null);
        }

        private void SetLastParagraphPr(Paragraph oldResultParagraph)
        {
            if (LastParagraph == null)
                return;

            SetParaPr(
                LastParagraph,
                oldResultParagraph != null ? oldResultParagraph.ParaPr : null,
                mOldResultEnumerator.RunPr);
        }

        private bool MoveResultsNext()
        {
            if (!mOldResultEnumerator.MoveNext())
                return false;

            if (!MoveNext())
                return false;

            if (!mOldResultEnumerator.IsCurrentTokenTable && !IsCurrentTokenTable)
                return true;

            Paragraph paragraph = mOldResultEnumerator.LastParagraph ?? mOldResultEnumerator.CurrentParagraph;
            SetLastParagraphPr(paragraph);

            LockProcessing();

            return MoveToTable(mOldResultEnumerator) && MoveToTable(this);
        }

        private static bool MoveToTable(ResultEnumerator enumerator)
        {
            // WORDSNET-19826 If the enumerator is suspended, there's no point in MoveNext() because it causes infinite loop.
            if (enumerator.IsSuspended)
                return enumerator.IsCurrentTokenTable;

            while (!enumerator.IsCurrentTokenTable)
                if (!enumerator.MoveNext())
                    return false;

            return true;
        }

        private static void MergeTables(Table newResultTable, Table oldResultTable)
        {
            newResultTable.TablePr.Clear();
            CloneAttrCollectionRevisionsAware(oldResultTable.TablePr).CopyTo(newResultTable.TablePr);

            IEnumerator<Row> oldResultRows = oldResultTable.Rows.GetNodeEnumerator<Row>();
            Row oldResultRow = null;

            foreach (Row newResultRow in newResultTable.Rows)
            {
                if (oldResultRows.MoveNext())
                    oldResultRow = oldResultRows.Current;
                else if (oldResultRow == null)
                    return;

                MergeRow(newResultRow, oldResultRow);
            }
        }

        private static void MergeRow(Row newResultRow, Row oldResultRow)
        {
            newResultRow.TablePr = CloneAttrCollectionRevisionsAware(oldResultRow.TablePr);

            IEnumerator<Cell> newResultCells = newResultRow.Cells.GetNodeEnumerator<Cell>();
            IEnumerator<Cell> oldResultCells = oldResultRow.Cells.GetNodeEnumerator<Cell>();

            while (newResultCells.MoveNext() && oldResultCells.MoveNext())
            {
                Cell newResultCell = newResultCells.Current;
                Cell oldResultCell = oldResultCells.Current;

                MergeCell(newResultCell, oldResultCell);
            }
        }

        private static void MergeCell(Cell newResultCell, Cell oldResultCell)
        {
            newResultCell.CellPr = CloneAttrCollectionRevisionsAware(oldResultCell.CellPr);

            NodeRange newResultNodes = new NodeRange(newResultCell.FirstParagraph, newResultCell.LastParagraph);

            OldResultEnumerator oldResultEnumerator = new OldResultEnumerator(
                new NodeRange(oldResultCell.FirstParagraph, oldResultCell.LastParagraph),
                oldResultCell.FirstParagraph);

            MergeFormat(
                newResultNodes,
                newResultCell.FirstParagraph,
                newResultCell.LastParagraph,
                oldResultEnumerator,
                newResultCell.FirstParagraph != newResultCell.LastParagraph ? oldResultCell.LastParagraph : null);
        }

        private void InsertCurrentText(RunPr runPr, bool forceFinalize)
        {
            if ((mLastInsertedRun == null) ||
                !mLastInsertedRun.RunPr.Equals(runPr, gRunPrKeysToIgnoreInComparison) ||
                AreNodesBelongToDifferentStdContainers(mLastInsertedRun, mRunToEdit))
            {
                // If current formatting is changed then last inserted run must be finalized.
                FinalizeLastInsertedRun();

                if (mCurrentTokenBuilder.Length == 0)
                {
                    mLastInsertedRun = null;
                    // Last inserted run is finalized and text to insert is empty, so there's nothing left to do.
                    return;
                }

                mLastInsertedRun = new Run(mRunToEdit.Document, string.Empty, MergeRunPr(runPr, mRunToEdit.RunPr));
                mRunToEdit.InsertPrevious(mLastInsertedRun);

                // WORDSNET-16372 Apply old formatting to STD container.
                if (mRunToEdit.ParentNode.NodeType == NodeType.StructuredDocumentTag)
                {
                    StructuredDocumentTag std = (StructuredDocumentTag)mRunToEdit.ParentNode;
                    std.ContentsRunPr = MergeRunPr(runPr, std.ContentsRunPr);
                }
            }

            if (mCurrentTokenBuilder.Length != 0)
            {
                mLastInsertedRunTextBuilder.Append(mCurrentTokenBuilder);
                ClearCurrentText();
            }

            if (forceFinalize || IsNextNodeParagraph)
            {
                // Force finalization of last inserted run in case when paragraph break is met,
                // as run can not cross parapraph borders or in case when it need to be forced.
                FinalizeLastInsertedRun();
                mLastInsertedRun = null;
            }
        }

        private static RunPr MergeRunPr(RunPr oldRunPr, RunPr newRunPr)
        {
            return MergeRunPr(oldRunPr, newRunPr, true);
        }

        private static RunPr MergeRunPr(RunPr oldRunPr, RunPr newRunPr, bool trimRevisions)
        {
            RunPr mergedRunPr = CloneAttrCollectionRevisionsAware(oldRunPr, trimRevisions);
            newRunPr.MirrorTo(mergedRunPr, FontAttr.Bidi);
            return mergedRunPr;
        }

        private static void SetParaPr(Paragraph paragraph, ParaPr paraPr, RunPr runPr)
        {
            if (paraPr != null)
                paragraph.ParaPr = CloneAttrCollectionRevisionsAware(paraPr, false);

            if (runPr != null)
                paragraph.ParagraphBreakRunPr = MergeRunPr(runPr, paragraph.ParagraphBreakRunPr, false);
        }

        [JavaGenericParameter("T extends WordAttrCollection")]
        private static T CloneAttrCollectionRevisionsAware<T>(T source)
            where T : WordAttrCollection
        {
            return CloneAttrCollectionRevisionsAware(source, true);
        }

        [JavaGenericParameter("T extends WordAttrCollection")]
        private static T CloneAttrCollectionRevisionsAware<T>(T source, bool trimRevisions)
            where T : WordAttrCollection
        {
            T result = source.Clone();

            if (trimRevisions)
            {
                // WORDSNET-18892 Trim result nodes revisions.
                result.Remove(RevisionAttr.DeleteRevision);
                result.Remove(RevisionAttr.InsertRevision);
                result.AcceptFormatRevision();
            }

            return result;
        }

        private void FinalizeProcessing()
        {
            // If we are in the middle of the run, cut its text, otherwise remove edited run.
            if (!ApplyNextRunToEdit(mNextRunToEdit))
                FinalizeRunToEdit();

            mRunToEdit = null;
            mNextRunToEdit = null;

            FinalizeLastInsertedRun();
            mLastInsertedRun = null;

            ClearCurrentText();
            mNextTokenBuilder.Length = 0;
        }

        private void FinalizeLastInsertedRun()
        {
            if (mLastInsertedRun == null)
                return;

            mLastInsertedRun.Text = mLastInsertedRunTextBuilder.ToString();
            mLastInsertedRunTextBuilder.Length = 0;
        }

        private void FinalizeRunToEdit()
        {
            if (mRunToEdit == null)
                return;

            mRunToEdit.Text = mRunToEdit.Text.Substring(NextTextPosition, mRunToEdit.Text.Length - NextTextPosition);
        }

        private void InitializeProcessing()
        {
            if (IsNextNodeRun)
                OnNextCharCore(NextRun.Text[NextTextPosition]);
        }

        private void ClearCurrentText()
        {
            mCurrentTokenBuilder.Length = 0;
        }

        /// <summary>
        /// Removes the collected "old" runs.
        /// </summary>
        private void RemoveRuns()
        {
            foreach (Run run in mRunsToRemove)
                run.Remove();
        }

        /// <summary>
        /// Locks additional processing of current and next token relative values.
        /// </summary>
        private void LockProcessing()
        {
            if (mIsProcessingLocked)
                return;

            FinalizeProcessing();
            mIsProcessingLocked = true;
        }

        /// <summary>
        /// Unlocks additional processing of current and next token relative values.
        /// </summary>
        private void UnlockProcessing()
        {
            if (!mIsProcessingLocked)
                return;

            InitializeProcessing();
            mIsProcessingLocked = false;
        }

        protected override void OnFieldResultBoundary()
        {
            if (mIsProcessingLocked)
                return;

            // Due to the certain circumstances sometimes a field result contains not only nested field results
            // but the whole fields instead. To avoid these fields' updating collisions, we need to separate
            // the results of every of those fields in the individual runs, even if the formatting before, inside
            // and after the corresponding field result is the same.
            InsertCurrentText(mOldResultEnumerator.RunPr, true);
        }

        protected override void OnChar(char c)
        {
            if (mIsProcessingLocked)
                return;

            ApplyNextRunToEdit(NextRun);
            // Character must be appended after next run to edit to be applied for the case of BIDI text direction change.
            mCurrentTokenBuilder.Append(c);
        }

        protected override void OnNextChar(char c)
        {
            if (mIsProcessingLocked)
                return;

            OnNextCharCore(c);
        }

        private void OnNextCharCore(char c)
        {
            mNextTokenBuilder.Append(c);
            mNextRunToEdit = NextRun;
        }

        protected override void ApplyNext()
        {
            if (mIsProcessingLocked)
                return;

            ApplyNextRunToEdit(mNextRunToEdit);
            mNextRunToEdit = null;

            ClearCurrentText();
            StringBuilder builder = mCurrentTokenBuilder;
            mCurrentTokenBuilder = mNextTokenBuilder;
            mNextTokenBuilder = builder;
        }

        /// <summary>
        /// Makes next run to edit to be current. If it is current already, returns false.
        /// Otherwise, collects old edited run to be removed and returns true.
        /// </summary>
        private bool ApplyNextRunToEdit(Run nextRunToEdit)
        {
            if (mRunToEdit == nextRunToEdit)
                return false;

            if (mRunToEdit != null)
                mRunsToRemove.Add(mRunToEdit);

            // Reference equality check is enough for RunPr.Bidi as AttrBoolEx is multiton.
            if ((mRunToEdit != null) && (nextRunToEdit != null) &&
                ((mRunToEdit.RunPr.Bidi != nextRunToEdit.RunPr.Bidi) || AreNodesBelongToDifferentStdContainers(mRunToEdit, nextRunToEdit)))
            {
                // Insert current text if its BIDI direction was changed.
                InsertCurrentText(mOldResultEnumerator.RunPr, true);
            }

            mRunToEdit = nextRunToEdit;
            return true;
        }

        private static bool AreNodesBelongToDifferentStdContainers(Node x, Node y)
        {
            return (x.ParentNode != y.ParentNode) &&
                   ((x.ParentNode.NodeType == NodeType.StructuredDocumentTag) ||
                    (y.ParentNode.NodeType == NodeType.StructuredDocumentTag));
        }

        internal override bool IsSuspended
        {
            get { return false; }
        }

        private StringBuilder mCurrentTokenBuilder = new StringBuilder();
        private StringBuilder mNextTokenBuilder = new StringBuilder();
        private readonly StringBuilder mLastInsertedRunTextBuilder = new StringBuilder();
        private readonly List<Run> mRunsToRemove = new List<Run>();
        private readonly Paragraph mEndParagraph;
        private readonly OldResultEnumerator mOldResultEnumerator;
        private Run mLastInsertedRun;
        private Run mRunToEdit;
        private Run mNextRunToEdit;
        private bool mIsProcessingLocked;

        // Keep sorted since we use binary search.
        private static readonly int[] gRunPrKeysToIgnoreInComparison = { FontAttr.RsidRPr, FontAttr.RsidR, FontAttr.Bidi };
    }
}
