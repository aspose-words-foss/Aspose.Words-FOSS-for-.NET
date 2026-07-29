// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2013 by Konstantin Kornilov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// A collection of individual bookmarks outline level.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-bookmarks/">Working with Bookmarks</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Key is a case-insensitive string bookmark name. Value is a int bookmark outline level.</para>
    /// <para>Bookmark outline level may be a value from 0 to 9. Specify 0 and Word bookmark will not be displayed in the document outline.
    /// Specify 1 and Word bookmark will be displayed in the document outline at level 1; 2 for level 2 and so on.</para>
    /// </remarks>
    public class BookmarksOutlineLevelCollection : IEnumerable<KeyValuePair<string, int>>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <overloads>Provides access to the collection items.</overloads>
        /// <summary>
        /// Gets or a sets a bookmark outline level by the bookmark name.
        /// </summary>
        /// <param name="name">Case-insensitive name of the bookmark.</param>
        /// <returns>The outline level of the bookmark. Valid range is 0 to 9.</returns>
        public int this[string name]
        {
            get { return mItems[name]; }
            set
            {
                if ((value < 0) || (value > 9))
                    throw new ArgumentOutOfRangeException("value");

                mItems[name] = value;
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Gets or a sets a bookmark outline level by the bookmark name.
        /// </summary>
        public int GetByName(string name)
        {
            return this[name];
        }
#endif

        /// <summary>
        /// Gets or sets a bookmark outline level at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the bookmark.</param>
        /// <returns>The outline level of the bookmark. Valid range is 0 to 9.</returns>
        public int this[int index]
        {
            get { return mItems.Values[index]; }
            set
            {
                if ((value < 0) || (value > 9))
                    throw new ArgumentOutOfRangeException("value");

                string key = mItems.Keys[index];
                mItems[key] = value;
            }
        }

        /// <summary>
        /// Adds a bookmark to the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the bookmark to add.</param>
        /// <param name="outlineLevel">The outline level of the bookmark. Valid range is 0 to 9.</param>
        public void Add(string name, int outlineLevel)
        {
            if ((outlineLevel < 0) || (outlineLevel > 9))
                throw new ArgumentOutOfRangeException("outlineLevel");

            mItems.Add(name, outlineLevel);
        }

        /// <summary>
        /// Determines whether the collection contains a bookmark with the given name.
        /// </summary>
        /// <param name="name">Case-insensitive name of the bookmark to locate.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            return mItems.ContainsKey(name);
        }

        /// <summary>
        /// Returns the zero-based index of the specified bookmark in the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the bookmark.</param>
        /// <returns>The zero based index. Negative value if not found.</returns>
        public int IndexOfKey(string name)
        {
            return mItems.IndexOfKey(name);
        }

        /// <summary>
        /// Removes a bookmark with the specified name from the collection.
        /// </summary>
        /// <param name="name">The case-insensitive name of the bookmark.</param>
        public void Remove(string name)
        {
            mItems.Remove(name);
        }

        /// <summary>
        /// Removes a bookmark at the specified index.
        /// </summary>
        /// <param name="index">The zero based index.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly SortedStringListGeneric<int> mItems = new SortedStringListGeneric<int>(false);
    }
}
