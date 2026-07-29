// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/12/2013 by Ivan Pnsk

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a node in <see cref="LinkedList"/>
    /// </summary>
    internal interface ILinkedListNode
    {
        /// <summary>
        /// Gets or sets the next node in the list, or null if the current node is the last one in the list.
        /// </summary>
        ILinkedListNode NextNode { get; set; }

        /// <summary>
        /// Gets the previous node in the list, or null if the current node is the first one in the list.
        /// </summary>
        ILinkedListNode PrevNode { get; set; }

        /// <summary>
        /// Gets or sets a main order index inside linked list.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Gets or sets a secondary order index inside linked list.
        /// This index is used when a node is inserted in the middle of a list.
        /// The values sequence is ascending, but IS NOT consistent, so sequence can be 1,12,13,1000,1000001,2000000... 
        /// </summary>
        long SecondaryIndex { get; set; }

        /// <summary>
        /// Gets True if a node is marked as removed from a list or never was added to a list.
        /// </summary>
        bool IsNotIncluded { get; }

        /// <summary>
        /// Mark node as removed from a list.
        /// </summary>
        void MarkAsRemoved();
    }
}
