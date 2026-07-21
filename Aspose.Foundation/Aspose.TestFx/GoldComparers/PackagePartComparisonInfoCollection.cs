// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/08/2007 by Vladimir Averkin

using System.Collections;
using System.Collections.Generic;

namespace Aspose.TestFx.GoldComparers
{
    public class PackagePartComparisonInfoCollection : IEnumerable<PackagePartComparisonInfo>
    {
        internal void Add(PackagePartComparisonInfo info)
        {
            mInfoTable.Add(info.Name, info);
        }

        internal PackagePartComparisonInfo this[string name]
        {
            get { return mInfoTable.GetValueOrNull(name); }
        }

        public IEnumerator<PackagePartComparisonInfo> GetEnumerator()
        {
            return mInfoTable.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get { return mInfoTable.Count; }
        }

        private readonly SortedList<string, PackagePartComparisonInfo> mInfoTable =
            new SortedList<string, PackagePartComparisonInfo>();
    }
}
