// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2007 by Vladimir Averkin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.OpcPackaging
{
    public sealed class OpcPackagePartCollection : IEnumerable<OpcPackagePart>
    {
        public OpcPackagePartCollection()
        {
            // Have to use sorted string list to ensure the same order of items in .NET and Java so the golds are the same.
            mItems = new SortedStringListGeneric<OpcPackagePart>(false);
        }

        /// <summary>
        /// Creates the opc package part collection using a specific comparator.
        /// </summary>
        public OpcPackagePartCollection(IComparer<string> comparer)
        {
            // Have to use sorted string list to ensure the same order of items in .NET and Java so the golds are the same.
            mItems = new SortedStringListGeneric<OpcPackagePart>(comparer);
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public void Add(OpcPackagePart part) 
        {
            mItems.Add(part.Name, part);
        }

        public bool Contains(string name)
        {
            return mItems.ContainsKey(name);
        }

        public OpcPackagePart this[string name]
        {
            get { return mItems.GetValueOrNull(name); }
        }

        public void Remove(string name)
        {
            mItems.Remove(name);
        }

        public IEnumerator<OpcPackagePart> GetEnumerator()
        {
            return mItems.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly SortedStringListGeneric<OpcPackagePart> mItems;
    }
}
