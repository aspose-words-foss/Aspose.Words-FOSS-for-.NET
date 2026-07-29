// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2011 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// A collection of strings that represent XML schemas that are associated with a custom XML part.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class. You access the collection of XML schemas of a custom XML part
    /// via the <see cref="CustomXmlPart.Schemas"/> property.</para>
    /// 
    /// <seealso cref="CustomXmlPart"/>
    /// <seealso cref="CustomXmlPart.Schemas"/>
    /// </remarks>
    public class CustomXmlSchemaCollection : IEnumerable<string>
    {
        /// <summary>
        /// Public ctor not needed.
        /// </summary>
        internal CustomXmlSchemaCollection()
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

        private readonly List<string> mItems = new List<string>();

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="value">The item to add.</param>
        public void Add(string value)
        {
            ArgumentUtil.CheckNotNull(value, "value");
            mItems.Add(value);
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

        /// <summary>
        /// Makes a deep clone of this object.
        /// </summary>
        public CustomXmlSchemaCollection Clone()
        {
            // We don't have derived classes therefore we simply create a new object, don't memberwise clone.
            CustomXmlSchemaCollection lhs = new CustomXmlSchemaCollection();

            foreach (string item in this)
                lhs.Add(item);

            return lhs;
        }
    }
}
