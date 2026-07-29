// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2006 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a queue of execution items.
    /// </summary>
    internal class ExecutionQueue
    {
        /// <summary>
        /// Enqueues an execution item.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        internal void Enqueue(IExecutionItem item)
        {
            Debug.Assert(item != null);
            mQueue.Enqueue(item);
        }

        /// <summary>
        /// Dequeues an execution item.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        internal IExecutionItem Dequeue()
        {
            return mQueue.Dequeue();
        }

        /// <summary>
        /// Gets the number of execution items contained in this queue.
        /// </summary>
        internal int Count
        {
            get { return mQueue.Count; }
        }

        private readonly Queue<IExecutionItem> mQueue = new Queue<IExecutionItem>();
    }
}
