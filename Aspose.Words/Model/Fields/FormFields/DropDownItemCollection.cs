// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2005 by Roman Korchagin
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;


namespace Aspose.Words.Fields
{
    /// <summary>
    /// A collection of strings that represent all the items in a drop-down form field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="FormField"/>
    /// <seealso cref="FormField.DropDownItems"/>
    public class DropDownItemCollection : IEnumerable<string>, IComplexAttr
    {
        /// <summary>
        /// Don't need public ctor.
        /// </summary>
        internal DropDownItemCollection()
        {
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public string this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<string> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a string to the end of the collection.
        /// </summary>
        /// <param name="value">The string to add to the end of the collection.</param>
        /// <returns>
        /// <ms>The zero-based index at which the new element is inserted.</ms>
        /// <cpp>The zero-based index at which the new element is inserted.</cpp>
        /// <java><tt>true</tt> (as per the general contract of Collection.add).</java>
        /// </returns>
        /// <javaName>boolean add(java.lang.String s)</javaName>
        public int Add(string value)
        {
            int index = Count;
            Insert(index, value);
            return index;
        }

        /// <summary>
        /// Determines whether the collection contains the specified value.
        /// </summary>
        /// <param name="value">Case-sensitive value to locate.</param>
        /// <returns><c>true</c> if the item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(string value)
        {
            return mItems.Contains(value);
        }

        /// <summary>
        /// Returns the zero-based index of the specified value in the collection.
        /// </summary>
        /// <param name="value">The case-sensitive value to locate.</param>
        /// <returns>The zero based index. Negative value if not found.</returns>
        public int IndexOf(string value)
        {
            return mItems.IndexOf(value);
        }

        /// <summary>
        /// Inserts a string into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value is inserted.</param>
        /// <param name="value">The string to insert.</param>
        public void Insert(int index, string value)
        {
            ArgumentUtil.CheckNotNull(value, "value");

            if (Count >= MaxItemsCount)
                throw new InvalidOperationException(string.Format(
                    "There can be a maximum of {0} items in a dropdown form field.", MaxItemsCount));

            mItems.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value from the collection.
        /// </summary>
        /// <param name="name">The case-sensitive value to remove.</param>
        public void Remove(string name)
        {
            mItems.Remove(name);
        }

        /// <summary>
        /// Removes a value at the specified index.
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

        internal DropDownItemCollection Clone()
        {
            // We don't have derived classes therefore we simply create a new object, don't memberwise clone.
            DropDownItemCollection lhs = new DropDownItemCollection();

            foreach (string item in this)
                lhs.Add(item);

            return lhs;
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IComplexAttr.IsInheritedComplexAttr
        {
            get { return false; }
        }

        /// <summary>
        /// Reserved for system use. IComplexAttr.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IComplexAttr IComplexAttr.DeepCloneComplexAttr()
        {
            return Clone();
        }

        /// <summary>
        /// Maximum number of drop down items MS Word allows.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxItemsCount = 25;

        private readonly List<string> mItems = new List<string>();
    }
}
