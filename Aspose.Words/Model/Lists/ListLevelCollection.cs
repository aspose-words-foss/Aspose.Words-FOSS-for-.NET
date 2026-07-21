// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2005 by Roman Korchagin
using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// A collection of list formatting for each level in a list.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-lists/">Working with Lists</a> documentation article.</para>
    /// </summary>
    public class ListLevelCollection : IEnumerable<ListLevel>
    {
        /// <summary>
        /// Ctor. Used when loading a document from RTF.
        /// </summary>
        internal ListLevelCollection()
        {
            mItems = new List<ListLevel>();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="document">List levels require an owner document.</param>
        /// <param name="listType">Specifies how many list levels to create.</param>
        internal ListLevelCollection(DocumentBase document, ListType listType)
        {
            int levelCount = GetExpectedLevelCount(listType);
            mItems = new List<ListLevel>(levelCount);

            for (int levelIndex = 0; levelIndex < levelCount; levelIndex++)
                mItems.Add(new ListLevel(document, levelIndex));
        }

        /// <summary>
        /// Adds a list level to the document. Used when loading a document from RTF.
        /// </summary>
        /// <param name="listLevel"></param>
        internal void Add(ListLevel listLevel)
        {
            mItems.Add(listLevel);
        }
        
        /// <summary>
        /// WORDSNET-14420 Corrects (adds or removes) the number of levels in this collection to match the specified list type.
        /// </summary>
        internal void FixUpLevelCount(ListType listType, DocumentBase doc)
        {
            int expectedLevelCount = GetExpectedLevelCount(listType);

            // Remove excess levels if we have any.
            while (mItems.Count > expectedLevelCount)
                mItems.RemoveAt(mItems.Count - 1);

            // Add more levels if required.
            while (mItems.Count < expectedLevelCount)
                Add(new ListLevel(doc, mItems.Count));
        }

        /// <summary>
        /// Gets the number of levels for the specified list type.
        /// </summary>
        private static int GetExpectedLevelCount(ListType listType)
        {
            switch (listType)
            {
                case ListType.SingleLevel:
                    return 1;
                case ListType.HybridMultiLevel:
                case ListType.MultiLevel:
                    return ListLevel.MaxLevels;
                default:
                    throw new InvalidOperationException("Unknown list type.");
            }
        }

        /// <summary>
        /// Gets the enumerator object that will enumerate levels in this list.
        /// </summary>
        public IEnumerator<ListLevel> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// In some probably corrupted documents index can be greater than the number of levels,
        /// I silently default to the last available level.
        /// </summary>
        internal ListLevel FetchListLevel(int level)
        {
            // WORDSNET-23784 Should check the collection is not empty for resilience. 
            return (Count == 0) ? null : this[CorrectLevel(level)];
        }

        /// <summary>
        /// In some probably corrupted documents index can be greater than the number of levels,
        /// I silently default to the last available level.
        /// </summary>
        internal int CorrectLevel(int level)
        {
            return System.Math.Min(level, Count - 1);
        }

        /// <summary>
        /// Makes a deep copy of the list levels.
        /// Note that the paragraph styles that might be linked to the levels are not copied.
        /// </summary>
        internal ListLevelCollection Clone(DocumentBase dstDocument)
        {
            ListLevelCollection lhs = (ListLevelCollection)MemberwiseClone();

            lhs.mItems = new List<ListLevel>(mItems.Count);
            foreach (ListLevel item in mItems)
                lhs.mItems.Add(item.Clone(dstDocument));

            return lhs;
        }

        /// <summary>
        /// Gets a list level by index.
        /// </summary>
        public ListLevel this[int index]
        {
            get { return mItems[index]; }
            set { mItems[index] = value; }
        }

        /// <summary>
        /// Gets the number of levels in this list.
        /// </summary>
        /// <remarks>
        /// <p>There could be 1 or 9 levels in a list.</p>
        /// </remarks>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Clears ListLevels collection.
        /// </summary>
        internal void Clear()
        {
            mItems.Clear();
        }

        private List<ListLevel> mItems;
    }
}
