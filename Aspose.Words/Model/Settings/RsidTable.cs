// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2006 by Dmitry Vorobyev

using Aspose.Collections.Generic;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Holds rsid (Revision state identifier) table.
    /// </summary>
    internal class RsidTable
    {
        /// <summary>
        /// Adds an RSID value to the RSID table unless this value is already in the table.
        /// </summary>
        internal void Add(object rsid)
        {
            if (rsid == null)
                return;

            if ((int)rsid == int.MinValue)
                return;

            if ((int)rsid == 0)
                return;

            if (!mRsidTable.ContainsKey((int)rsid))
                mRsidTable.Add((int)rsid, 0);
        }
        
        /// <summary>
        /// Returns an RSID value by the specified index.
        /// </summary>
        internal int this[int index]
        {
            get { return mRsidTable.GetKey(index); }
        }
        
        /// <summary>
        /// Returns the total number of RSID values in the table.
        /// </summary>
        internal int Count
        {
            get { return mRsidTable.Count; }
        }

        internal void Clear()
        {
            mRsidTable.Clear();
        }

        public RsidTable Clone()
        {
            RsidTable lhs = new RsidTable();

            for(int i = 0; i < mRsidTable.Count; i++)
                lhs.mRsidTable.Add(mRsidTable.GetKey(i), 0);

            return lhs;
        }

        private readonly SortedIntegerListGeneric<object> mRsidTable = new SortedIntegerListGeneric<object>();
    }
}
