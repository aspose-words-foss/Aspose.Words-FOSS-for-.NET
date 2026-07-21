// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2013 by Alexey Morozov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="XmlNamespace"/> objects that represents the entire collection of schemas in the Schema Library.
    /// </summary>
    /// <remarks>
    /// <p>In Microsoft Office Word, the Schema Library represents schemas installed on a user's computer that a user has applied to a Word document or 
    /// that a user has explicitly added to the Schema Library by using the Schema Library dialog box.</p>
    /// <p>In Aspose.Words this collection represents only schemas that a user has applied to document.</p>
    /// </remarks>
    internal class XmlNamespaceCollection : IEnumerable<XmlNamespace>
    {
        /// <summary>
        /// Adds schema to Schema Library.
        /// </summary>
        internal void Add(XmlNamespace xmlNamespace)
        {
            mItems.Add(xmlNamespace);
        }

        /// <summary>
        /// Returns schema at given index.
        /// </summary>
        internal XmlNamespace this[int index]
        {
            get { return mItems[index]; }
        }

        public IEnumerator<XmlNamespace> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes schema at given index.
        /// </summary>
        internal void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Removes all schemas from the collection.
        /// </summary>
        internal void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Gets the number of schemas in the collection.
        /// </summary>
        internal int Count
        {
            get { return mItems.Count; }
        }

        internal XmlNamespaceCollection Clone()
        {
            XmlNamespaceCollection lhs = new XmlNamespaceCollection();

            foreach (XmlNamespace xmlNamespace in this)
                lhs.Add(xmlNamespace.Clone());

            return lhs;
        }

        private readonly List<XmlNamespace> mItems = new List<XmlNamespace>();
    }
}
