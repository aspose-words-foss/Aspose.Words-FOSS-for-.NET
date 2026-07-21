// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2010 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Enumerates an "old" field result when applying MERGEFORMAT. Finds formatting sources for runs and paragraphs.
    /// </summary>
    internal class OldResultEnumerator : ResultEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="resultNodes"></param>
        /// <param name="startParagraph"></param>
        internal OldResultEnumerator(IEnumerable<Node> resultNodes, Paragraph startParagraph)
            : base(resultNodes, startParagraph)
        {
        }

        internal override bool MoveNext()
        {
            if (mIsSuspended)
            {
                RunPr = null;
                return true;
            }

            return base.MoveNext();
        }

        protected override void OnChar(char c)
        {
            if (RunPr == null)
                RunPr = NextRun.RunPr;
        }

        protected override void OnNextChar(char c)
        {
            mNextRunPr = NextRun.RunPr;
        }

        protected override void OnNextNode()
        {
            if (IsNextNodeParagraph || IsNextNodeTable)
            {
                mNextRunPr = CurrentParagraph != null
                    ? CurrentParagraph.ParagraphBreakRunPr
                    : null;
            }
        }

        protected override void ApplyNext()
        {
            RunPr = mNextRunPr;
            mNextRunPr = null;
        }

        internal override bool IsSuspended
        {
            get { return mIsSuspended; }
        }

        /// <summary>
        /// Suspends the enumeration.
        /// </summary>
        internal void Suspend()
        {
            mIsSuspended = true;
        }

        /// <summary>
        /// Unsuspends the enumeration.
        /// </summary>
        internal void Unsuspend()
        {
            mIsSuspended = false;
        }

        /// <summary>
        /// Gets the current formatting source for runs and paragraph breaks.
        /// </summary>
        internal RunPr RunPr { get; private set; }

        private RunPr mNextRunPr;
        private bool mIsSuspended;
    }
}
