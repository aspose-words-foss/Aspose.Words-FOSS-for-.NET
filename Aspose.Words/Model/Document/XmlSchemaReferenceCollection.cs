// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2013 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of XMLSchemaReference objects that represent the unique namespaces that are attached to a document.
    /// </summary>
    internal class XmlSchemaReferenceCollection : IEnumerable<XmlSchemaReference>
    {
        /// <summary>
        /// Attaches schema with given Uri to the document.
        /// </summary>
        /// <remarks>
        /// <p>Aspose.Words exposes in <see cref="XmlNamespaceCollection" /> only schema applied to document. 
        /// This method is indented to attach schema installed on user's computer.</p>
        /// </remarks>
        internal void Add(string uri, string location)
        {
            ArgumentUtil.CheckHasChars(uri, "uri");

            if (mItems.ContainsKey(uri))
                throw new InvalidOperationException("Document already has attached schema with such Uri.");

            mItems.Add(uri, new XmlSchemaReference(uri, location));
        }

        /// <summary>
        /// Attaches schema to the document.
        /// </summary>
        internal void Add(XmlNamespace xmlNamespace)
        {
            Add(xmlNamespace.Uri, xmlNamespace.ManifestLocation);
        }

        /// <summary>
        /// Gets a document schema at the specified index.
        /// </summary>
        internal XmlSchemaReference this[int index]
        {
            // Fixed typo: mItems[index] -> mItems.GetByIndex(index).
            get { return mItems.Values[index]; }
        }

        /// <summary>
        /// Detaches schema reference with given Uri.
        /// </summary>
        internal void Remove(string uri)
        {
            mItems.Remove(uri);
        }

        /// <summary>
        /// Detaches schema reference at give index.
        /// </summary>
        internal void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        /// <summary>
        /// Detaches all schema references from document.
        /// </summary>
        internal void Clear()
        {
            mItems.Clear();
        }

        /// <summary>
        /// Gets the number of schema attached to document.
        /// </summary>
        internal int Count
        {
            get { return mItems.Count; }
        }

        public IEnumerator<XmlSchemaReference> GetEnumerator()
        {
            return mItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal XmlSchemaReferenceCollection Clone()
        {
            XmlSchemaReferenceCollection lhs = new XmlSchemaReferenceCollection();

            foreach (XmlSchemaReference xmlSchemaReference in this)
                lhs.Add(xmlSchemaReference.Uri, xmlSchemaReference.Location);

            return lhs;
        }

        private readonly SortedStringListGeneric<XmlSchemaReference> mItems = new SortedStringListGeneric<XmlSchemaReference>();
    }
}
