// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2015 by Denis Darkin

using System.Collections.Generic;
using Aspose.Words.Revisions;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides revisions stacks for each story.
    /// </summary>
    internal class StoryRevisionStack
    {
        /// <summary>
        /// Stores necessary information for item of revision stack.
        /// </summary>
        private class RevisionStackItem
        {
            internal RevisionStackItem(RevisionBase revision)
                : this (revision, false)
            {
            }

            internal RevisionStackItem(RevisionBase revision, bool isMoveRange)
            {
                mRevision = revision;
                mIsMoveRange = isMoveRange;
            }

            internal RevisionBase Revision
            {
                get { return mRevision; }
            }

            /// <summary>
            /// True indicates that <see cref="Revision"/> is not a real revision but only is created
            /// for a move range. Math nodes inside move range are intended to have a move revision.
            /// </summary>
            internal bool IsMoveRange
            {
                get { return mIsMoveRange; }
            }

            private readonly RevisionBase mRevision;
            private readonly bool mIsMoveRange;
        }

        /// <summary>
        /// Edit and move revisions are stored here to be added to related run, which is read later.
        /// Peek version of this stack is always safe to call, if the stack is empty it will return null.
        /// </summary>
        private class RevisionStack
        {
            internal void PushInsertRevision(EditRevision revision)
            {
                mPendingInsertRevisions.Push(new RevisionStackItem(revision));
            }

            internal RevisionStackItem PopInsertRevision()
            {
                return mPendingInsertRevisions.Pop();
            }

            internal RevisionStackItem PeekInsertRevision()
            {
                return mPendingInsertRevisions.Top();
            }

            internal void PushDeleteRevision(EditRevision revision)
            {
                mPendingDeleteRevisions.Push(new RevisionStackItem(revision));
            }

            internal RevisionStackItem PopDeleteRevision()
            {
                return mPendingDeleteRevisions.Pop();
            }

            internal RevisionStackItem PeekDeleteRevision()
            {
                return mPendingDeleteRevisions.Top();
            }

            internal void PushMoveToRevision(MoveRevision revision, bool isMoveRange)
            {
                mPendingMoveToRevisions.Push(new RevisionStackItem(revision, isMoveRange));
            }

            internal RevisionStackItem PopMoveToRevision()
            {
                if (mPendingMoveToRevisions.Count == 0)
                    return null;

                return mPendingMoveToRevisions.Pop();
            }

            internal RevisionStackItem PeekMoveToRevision()
            {
                return mPendingMoveToRevisions.Top();
            }

            internal void PushMoveFromRevision(MoveRevision revision, bool isMoveRange)
            {
                mPendingMoveFromRevisions.Push(new RevisionStackItem(revision, isMoveRange));
            }

            internal RevisionStackItem PopMoveFromRevision()
            {
                // WORDSNET-17529 Ignore moveFromRangeEnd, when moveFromRangeStart element is missing.
                if (mPendingMoveFromRevisions.Count == 0)
                    return null;

                return mPendingMoveFromRevisions.Pop();
            }

            internal RevisionStackItem PeekMoveFromRevision()
            {
                return mPendingMoveFromRevisions.Top();
            }

            // DD: I'll preallocate each stack to 1 el only as usually this is the case.
            private readonly Stack<RevisionStackItem> mPendingInsertRevisions = new Stack<RevisionStackItem>(1);
            private readonly Stack<RevisionStackItem> mPendingDeleteRevisions = new Stack<RevisionStackItem>(1);
            private readonly Stack<RevisionStackItem> mPendingMoveToRevisions = new Stack<RevisionStackItem>(1);
            private readonly Stack<RevisionStackItem> mPendingMoveFromRevisions = new Stack<RevisionStackItem>(1);
        }

        /// <summary>
        /// Applies read revisions to given property collection.
        /// </summary>
        internal void Apply(RunPr runPr, Node node)
        {
            // If stack is empty there is nothing to apply.
            if (mRevisionStacks.Count == 0)
                return;

            RevisionStack curRevisionStack = mRevisionStacks.Peek();

            RevisionStackItem item = curRevisionStack.PeekDeleteRevision();
            if (item != null)
                runPr.DeleteRevision = (EditRevision)item.Revision;

            item = curRevisionStack.PeekInsertRevision();
            if (item != null)
                runPr.InsertRevision = (EditRevision)item.Revision;

            item = curRevisionStack.PeekMoveFromRevision();
            if ((item != null) &&
                (!item.IsMoveRange || IsMoveRevisionByMoveRange(node)))
                runPr.MoveFromRevision = (MoveRevision)item.Revision;

            item = curRevisionStack.PeekMoveToRevision();
            if ((item != null) &&
                (!item.IsMoveRange || IsMoveRevisionByMoveRange(node)))
                runPr.MoveToRevision = (MoveRevision)item.Revision;
        }

        public StoryRevisionStack(IWarningCallback warningCallback, WarningSource warningSource)
        {
            mWarningCallback = warningCallback;
            mWarningSource = warningSource;
        }

        /// <summary>
        /// Returns <c>true</c> for node types, which nodes are expected to have a move revision on locating
        /// in a move range.
        /// </summary>
        private static bool IsMoveRevisionByMoveRange(Node node)
        {
            return node.GetAncestor(NodeType.OfficeMath) != null;
        }

        /// <summary>
        /// Starts new revisions story.
        /// </summary>
        internal void PushStory()
        {
            mRevisionStacks.Push(new RevisionStack());
        }

        /// <summary>
        /// Returns to previous revision story.
        /// </summary>
        internal void PopStory()
        {
            mRevisionStacks.Pop();
        }

        /// <summary>
        /// Pushes edit revision to stack.
        /// </summary>
        internal void Push(EditRevision editRevision)
        {
            if (mRevisionStacks.Count == 0)
                PushStory();

            RevisionStack curRevisionStack = mRevisionStacks.Peek();

            if (editRevision.Type == EditRevisionType.Deletion)
                curRevisionStack.PushDeleteRevision(editRevision);
            else
                curRevisionStack.PushInsertRevision(editRevision);
        }

        /// <summary>
        /// Pushes move revision to stack.
        /// </summary>
        internal void Push(MoveRevision moveRevision, bool isMoveRange)
        {
            if (mRevisionStacks.Count == 0)
                PushStory();

            RevisionStack curRevisionStack = mRevisionStacks.Peek();

            if (moveRevision.Type == MoveRevisionType.MoveFrom)
                curRevisionStack.PushMoveFromRevision(moveRevision, isMoveRange);
            else
                curRevisionStack.PushMoveToRevision(moveRevision, isMoveRange);
        }

        /// <summary>
        /// Pops revisions from stack.
        /// </summary>
        internal RevisionBase Pop(RevisionBase revision)
        {
            Debug.Assert(mRevisionStacks.Count > 0);

            if (mRevisionStacks.Count == 0)
                return null;

            RevisionStack curRevisionStack = mRevisionStacks.Peek();

            MoveRevision moveRevision = revision as MoveRevision;
            EditRevision editRevision = revision as EditRevision;

            if (moveRevision != null)
                return PopMoveRevision(moveRevision, curRevisionStack);

            if (editRevision != null)
                return (editRevision.Type == EditRevisionType.Deletion)
                    ? curRevisionStack.PopDeleteRevision().Revision
                    : curRevisionStack.PopInsertRevision().Revision;

            Debug.Assert(true, "Unexpected revision type.");

            return null;
        }

        /// <summary>
        /// Pops moveToRevision or moveFromRevision depending from move revision type.
        /// </summary>
        private RevisionBase PopMoveRevision(MoveRevision moveRevision, RevisionStack curRevisionStack)
        {
            RevisionStackItem item = (moveRevision.Type == MoveRevisionType.MoveFrom)
                ? curRevisionStack.PopMoveFromRevision()
                : curRevisionStack.PopMoveToRevision();
            MoveRevision popedRevision = (MoveRevision)((item != null) ? item.Revision : null);

            if ((popedRevision == null) && (mWarningCallback != null))
            {
                WarningInfo warnInfo =
                    new WarningInfo(WarningType.UnexpectedContent, mWarningSource, WarningStrings.MoveStartMissed);
                mWarningCallback.Warning(warnInfo);
            }

            return popedRevision;
        }

        private readonly Stack<RevisionStack> mRevisionStacks = new Stack<RevisionStack>();
        private readonly IWarningCallback mWarningCallback;
        private readonly WarningSource mWarningSource;
    }
}
