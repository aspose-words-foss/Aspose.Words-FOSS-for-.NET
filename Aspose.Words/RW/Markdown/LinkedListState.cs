// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Ivan Pereshein

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents state of <see cref="LinkedList"/>
    /// </summary>
    internal class LinkedListState
    {
        /// <summary>
        /// Gets or sets head of linked list.
        /// </summary>
        internal ILinkedListNode Head
        {
            get { return mHead; }
            set { mHead = value; }
        }

        /// <summary>
        /// Gets or sets tail of linked list.
        /// </summary>
        internal ILinkedListNode Tail
        {
            get { return mTail; }
            set { mTail = value; }
        }

        internal int GetNextIndex()
        {
            return mLastIndex++;
        }

        private ILinkedListNode mHead;
        private ILinkedListNode mTail;
        private int mLastIndex;
    }
}
