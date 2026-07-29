// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2024 by Victor Chebotok

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Enumerates entries of a package.
    /// </summary>
    public class ComparisonPackage
    {
        public ComparisonPackage(PackageEntryList entries)
        {
            mEntries = entries;
            mEntryIndex = -1;
        }

        public bool MoveToNextEntry()
        {
            int nextIndex = mEntryIndex + 1;
            if (nextIndex >= mEntries.Count)
            {
                return false;
            }
            mEntryIndex = nextIndex;
            return true;
        }

        public byte[] LoadEntryToByteArray()
        {
            return mEntries.GetEntryData(mEntryIndex);
        }

        public string EntryFileName
        {
            get { return mEntries.GetEntryFileName(mEntryIndex); }
        }

        private readonly PackageEntryList mEntries;
        private int mEntryIndex;
    }
}
