// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2024 by Victor Chebotok

using System.Collections.Generic;
using System.Text;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Stores entries (inner files) of a package.
    /// </summary>
    public class PackageEntryList
    {
        public void AddEntry(string fileName, byte[] data)
        {
            mEntries.Add(new Entry(fileName, data));
        }

        public string GetEntryFileName(int index)
        {
            return mEntries[index].FileName;
        }

        public byte[] GetEntryData(int index)
        {
            return mEntries[index].Data;
        }

        public int Count
        {
            get { return mEntries.Count; }
        }

        private class Entry
        {
            internal Entry(string fileName, byte[] data)
            {
                FileName = fileName;
                Data = data;
            }

            internal string FileName { get; }

            internal byte[] Data { get; }
        }

        private readonly List<Entry> mEntries = new List<Entry>();
    }
}
