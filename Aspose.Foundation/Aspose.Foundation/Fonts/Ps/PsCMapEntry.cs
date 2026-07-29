// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2015 by Alexey Noskov

using System;

namespace Aspose.Fonts.Ps
{
    internal class PsCMapEntry : IComparable<PsCMapEntry>
    {
        internal PsCMapEntry(int charCode, int index)
        {
            mCharCode = charCode;
            mIndex = index;
        }

        public int CompareTo(PsCMapEntry other)
        {
            return Index.CompareTo(other.Index);
        }

        [JavaAttributes.JavaDelete]
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            // Entries must be ordered by index, so return result in indices comparison.
            PsCMapEntry entry = (PsCMapEntry)obj;
            return Index.CompareTo(entry.Index);
        }

        internal int CharCode
        {
            get { return mCharCode; }
        }

        internal int Index
        {
            get { return mIndex; }
        }

        private readonly int mCharCode;
        private readonly int mIndex;
    }
}
