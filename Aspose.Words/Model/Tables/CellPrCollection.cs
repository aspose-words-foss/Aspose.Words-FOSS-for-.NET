// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2005 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Typed collection of CellPr.
    /// </summary>
    internal class CellPrCollection : IComplexAttr, IEnumerable<AttrCollection>
    {
        internal void Add(AttrCollection item)
        {
            mItems.Add(item);
        }

        internal void Insert(int index, AttrCollection item)
        {
            mItems.Insert(index, item);
        }


        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (AttrCollection collection in mItems)
            {
                if (collection != null)
                    hashCode = (hashCode * 397) ^ collection.GetHashCode();
            }

            return hashCode;
        }

        public IEnumerator<AttrCollection> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal CellPr this[int index] 
        {
            get { return (CellPr)mItems[index]; }
            set { mItems[index] = value; }
        }

        public bool IsInheritedComplexAttr
        {
            get { return false; }
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        public IComplexAttr DeepCloneComplexAttr()
        {
            CellPrCollection lhs = new CellPrCollection();
            for (int index = 0; index < mItems.Count; index++)
                lhs.Add(this[index].Clone());
            return lhs;
        }

        private readonly List<AttrCollection> mItems = new List<AttrCollection>();
    }
}
