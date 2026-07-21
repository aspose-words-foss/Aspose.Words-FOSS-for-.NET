// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a collection of execution items.
    /// </summary>
    internal class ExecutionItemCollection : IEnumerable<IExecutionItem>
    {
        /// <summary>
        /// Adds an execution item to the collection.
        /// </summary>
        /// <param name="executionItem">The execution item to add.</param>
        internal void Add(IExecutionItem executionItem)
        {
            mItems.Add(executionItem);
        }

        public IEnumerator<IExecutionItem> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an execution item at the specified index.
        /// </summary>
        internal IExecutionItem this[int index]
        {
            get
            {
                Debug.Assert((index >= 0) && (index < mItems.Count));
                return mItems[index];
            }
        }

        /// <summary>
        /// Return the total number of execution items in this collection.
        /// </summary>
        internal int Count
        {
            get { return mItems.Count; }
        }

        private readonly List<IExecutionItem> mItems = new List<IExecutionItem>();
    }
}
