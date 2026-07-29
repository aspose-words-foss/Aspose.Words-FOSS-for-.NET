// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2009 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// A typed collection of <see cref="OdsoRecipientData"/>
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/mail-merge-and-reporting/">Mail Merge and Reporting</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// 
    /// <seealso cref="OdsoRecipientData"/>
    /// <seealso cref="Odso.RecipientDatas"/>
    /// </remarks>
    public class OdsoRecipientDataCollection : IEnumerable<OdsoRecipientData>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Gets or sets an item in this collection.
        /// </summary>
        public OdsoRecipientData this[int index]
        {
            get { return mItems[index]; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mItems[index] = value;
            }
        }

        /// <summary>
        /// Returns an <ms>enumerator</ms><java>iterator</java><cpp>enumerator</cpp> object that can be used to iterate over all items in the collection.
        /// </summary>
        public IEnumerator<OdsoRecipientData> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an object to the end of this collection.
        /// </summary>
        /// <param name="value">The object to add. Cannot be <c>null</c>.</param>
        public int Add(OdsoRecipientData value)
        {
            ArgumentUtil.CheckNotNull(value, "value");
            mItems.Add(value);
            return mItems.Count - 1;
        }

        /// <summary>
        /// Removes all elements from this collection.
        /// </summary>
        public void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Removes the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element.</param>
        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        private readonly List<OdsoRecipientData> mItems = new List<OdsoRecipientData>();
    }
}
