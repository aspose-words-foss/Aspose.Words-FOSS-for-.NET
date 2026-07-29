// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/09/2005 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Provides typed access to <see cref="FootnoteSeparator"/> nodes of a document.
    /// </summary>
    /// <dev>
    /// Map of special footnote/endnote separators defined in a document.
    /// Key is <see cref="FootnoteSeparatorType"/>, Value is <see cref="FootnoteSeparator"/>.
    /// </dev>
    public class FootnoteSeparatorCollection : IEnumerable<FootnoteSeparator>
    {
        /// <summary>
        /// Retrieves a <see cref="FootnoteSeparator"/> of the specified type.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if the footnote/endnote separator of the specified type is not found.
        /// </remarks>
        public FootnoteSeparator this[FootnoteSeparatorType separatorType]
        {
            get { return mItems.GetValueOrNull(separatorType); }
        }

        internal void Add(FootnoteSeparator footnoteSeparator)
        {
            mItems[footnoteSeparator.SeparatorType] = footnoteSeparator;
        }

        /// <summary>
        /// Makes a deep clone of the collection.
        /// Suitable only when copying a complete document.
        /// </summary>
        internal FootnoteSeparatorCollection Clone(DocumentBase dstDoc, INodeCloningListener nodeCloningListener)
        {
            FootnoteSeparatorCollection lhs = (FootnoteSeparatorCollection)this.MemberwiseClone();
            lhs.mItems = new Dictionary<FootnoteSeparatorType, FootnoteSeparator>();
            foreach (KeyValuePair<FootnoteSeparatorType, FootnoteSeparator> pair in mItems)
            {
                FootnoteSeparator separator = pair.Value;

                FootnoteSeparator dstSeparator = (FootnoteSeparator)separator.Clone(true, nodeCloningListener);
                dstSeparator.SetDocument(dstDoc);

                lhs.mItems.Add(pair.Key, dstSeparator);
            }

            return lhs;
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Indicates that all separators in collection have default formatting.
        /// </summary>
        internal bool IsDefault
        {
            get
            {
                foreach (FootnoteSeparator footnoteSeparator in this)
                    if (!footnoteSeparator.IsDefault)
                        return false;

                return true;
            }
        }

        public IEnumerator<FootnoteSeparator> GetEnumerator()
        {
            return mItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Retrieves a <see cref="FootnoteSeparator"/> of the specified type.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if the footnote/endnote separator of the specified type is not found.
        /// </remarks>
        public FootnoteSeparator GetByFootnoteSeparatorType(FootnoteSeparatorType separatorType)
        {
            return mItems.GetValueOrNull(separatorType);
        }
#endif
        private Dictionary<FootnoteSeparatorType, FootnoteSeparator> mItems = new Dictionary<FootnoteSeparatorType, FootnoteSeparator>();
    }
}
