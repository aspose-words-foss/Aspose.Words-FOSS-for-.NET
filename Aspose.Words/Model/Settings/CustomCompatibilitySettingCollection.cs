// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2010 by Denis Darkin (copy/pasted and adopted from OdsoRecipientDataCollection)

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// A typed collection of <see cref="CustomCompatibilitySetting"/>
    /// </summary>
    /// <seealso cref="CustomCompatibilitySetting"/>
    internal class CustomCompatibilitySettingCollection : IEnumerable<CustomCompatibilitySetting>
    {
                
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        internal int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets or sets an item in this collection.
        /// </summary>
        internal CustomCompatibilitySetting this[int index]
        {
            get { return mItems[index]; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mItems[index] = value;
            }
        }

        /// <summary>
        /// Returns item by name or null if item with given name is not presented in collection.
        /// </summary>
        /// <remarks>
        /// Seems there should be not too much items so hash table is not necessary.
        /// </remarks>
        internal CustomCompatibilitySetting this[string name]
        {
            get
            {
                foreach(CustomCompatibilitySetting item in mItems)
                    if (item.Name == name)
                        return item;

                return null;
            }
        }

        public IEnumerator<CustomCompatibilitySetting> GetEnumerator()
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

        /// <summary>
        /// Adds an object to the end of this collection.
        /// </summary>
        /// <param name="value">The object to add. Cannot be null.</param>
        internal int Add(CustomCompatibilitySetting value)
        {
            ArgumentUtil.CheckNotNull(value, "value");
            mItems.Add(value);
            return mItems.Count - 1;
        }

        /// <summary>
        /// Removes all elements from this collection.
        /// </summary>
        internal void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        internal void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Makes a copy of the object.
        /// </summary>
        internal CustomCompatibilitySettingCollection Clone()
        {
            CustomCompatibilitySettingCollection lhs = new CustomCompatibilitySettingCollection();

            foreach (CustomCompatibilitySetting item in mItems)
                lhs.mItems.Add(item.Clone());

            return lhs;
        }

        private readonly List<CustomCompatibilitySetting> mItems = new List<CustomCompatibilitySetting>();
    }
}
