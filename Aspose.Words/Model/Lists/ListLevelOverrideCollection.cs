// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/01/2009 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words.Lists
{
    internal class ListLevelOverrideCollection : IEnumerable<ListLevelOverride>
    {
        internal void Add(ListLevelOverride item)
        {
            mItems.Add(item);
        }

        internal void Remove(ListLevelOverride item)
        {
            mItems.Remove(item);
        }

        internal void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
        }

        public IEnumerator<ListLevelOverride> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Compares with the specified list level overrides.
        /// </summary>
        public bool Equals(ListLevelOverrideCollection listLevelOverrides)
        {
            return EqualsCore(listLevelOverrides, new HashSetGeneric<Pair>());
        }

        /// <summary>
        /// Core method for comparing with the specified list level overrides.
        /// </summary>
        /// <param name="listLevelOverrides">List level overrides that will be compared with this list level overrides.</param>
        /// <param name="alreadyComparedLinkedStyles">HashSet for collecting already compared styles to avoid dead loops.</param>
        internal bool EqualsCore(ListLevelOverrideCollection listLevelOverrides, HashSetGeneric<Pair> alreadyComparedLinkedStyles)
        {
            if (mItems.Count != listLevelOverrides.Count)
                return false;

            for (int i = 0; i < mItems.Count; i++)
            {
                if (!mItems[i].Equals(listLevelOverrides[i], alreadyComparedLinkedStyles))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Calculates hash code for this object.
        /// </summary>
        /// <dev>
        /// To be compatible with the <see cref="Equals(ListLevelOverrideCollection)"/> method, only properties that
        /// affect visual representation of the list can be included into the calculation. Properties like object name
        /// or ID should be ignored.
        /// </dev>
        public override int GetHashCode()
        {
            int hashCode = 0;

            foreach (ListLevelOverride item in mItems)
                hashCode = (hashCode * 397) ^ item.GetHashCode();

            return hashCode;
        }

        internal ListLevelOverride this[int index]
        {
            get { return mItems[index]; }
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        internal void Clear()
        {
            mItems.Clear();
        }

        private readonly List<ListLevelOverride> mItems = new List<ListLevelOverride>();
    }
}
