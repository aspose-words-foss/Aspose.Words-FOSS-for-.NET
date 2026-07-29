// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Base class for <see cref="TaskPaneCollection"/>, <see cref="WebExtensionBindingCollection"/>,
    /// <see cref="WebExtensionPropertyCollection"/> and <see cref="WebExtensionReferenceCollection"/> collections.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-office-add-ins/">Work with Office Add-ins</a> documentation article.</para>
    /// </summary>
    /// <typeparam name="T">Type of a collection item.</typeparam>
    public abstract class BaseWebExtensionCollection<T> : IEnumerable<T>
        where T: class
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets or sets an item at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the item.</param>
        public T this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                mItems[index] = value;
            }
        }

        /// <summary>
        /// Adds specified item to the collection.
        /// </summary>
        /// <param name="item">Item for adding.</param>
        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            mItems.Add(item);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Removes the item at the specified index from the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the collection item.</param>
        public void Remove(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Gets first element from the collection or null when it is not exist.
        /// </summary>
        internal T First
        {
            get { return (mItems.Count > 0) ? mItems[0] : null; }
        }

        /// <summary>
        /// Gets last element from the collection or null when it is not exist.
        /// </summary>
        internal T Last
        {
            get { return (mItems.Count > 0) ? mItems[mItems.Count - 1] : null; }
        }

        #region  IEnumerable<TaskPane>

        /// <summary>
        /// Returns an enumerator that can iterate through a collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion  IEnumerable<TaskPane>

        /// <summary>
        /// Container for collection items.
        /// </summary>
        private readonly List<T> mItems = new List<T>();
    }
}
