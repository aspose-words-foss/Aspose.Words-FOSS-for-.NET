// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2024 by Victor Chebotok

using System.IO;
using Aspose.Common;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Extracts entries (inner files) from ZIP archives.
    /// </summary>
    public static class ZipPackageReader
    {
        public static PackageEntryList ReadEntries(Stream stream)
        {
            PackageEntryList entries = new PackageEntryList();
            using (ZipReaderPal zipReader = new ZipReaderPal(stream))
            {
                while (zipReader.MoveToNextEntry())
                {
                    byte[] data = zipReader.LoadEntryToByteArray();
                    entries.AddEntry(zipReader.EntryFileName, data);
                }
            }
            return entries;
        }
    }
}
