// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2010 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a collection of <see cref="CustomPart"/> objects.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not normally need to create instances of this class. You access custom parts 
    /// related to the OOXML package via the <see cref="Document.PackageCustomParts"/> property.</para>
    /// 
    /// <seealso cref="CustomPart"/>
    /// <seealso cref="Document.PackageCustomParts"/>
    /// </remarks>
    public class CustomPartCollection : IEnumerable<CustomPart>
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
        public CustomPart this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<CustomPart> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="part">The item to add.</param>
        public void Add(CustomPart part)
        {
            mItems.Add(part);
        }

        /// <summary>
        /// Removes an item at the specified index.
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
        /// Makes a deep copy of this collection and its items.
        /// </summary>
        public CustomPartCollection Clone()
        {
            // We don't have derived classes therefore we simply create a new object, don't memberwise clone.
            CustomPartCollection lhs = new CustomPartCollection();

            foreach (CustomPart item in mItems)
                lhs.Add(item.Clone());

            return lhs;
        }

        private readonly List<CustomPart> mItems = new List<CustomPart>();
    }
}
