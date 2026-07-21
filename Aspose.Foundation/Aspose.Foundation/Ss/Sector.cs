// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.IO;

namespace Aspose.Ss
{
    /// <summary>
    /// Structured storage sector related constants and functions.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class Sector
    {
        /// <summary>
        /// Converts sector index into offset in the structured storage.
        /// </summary>
        internal static long SectorToOffset(uint sect, int sectorSize)
        {
            return SectorToOffset(sect, sectorSize == SectorSize);
        }

        /// <summary>
        /// Converts sector index into offset in the structured storage.
        /// </summary>
        internal static long SectorToOffset(uint sect, bool isNormalSector)
        {
            // Casting to uint added to resolve WORDSJAVA-2007 and WORDSJAVA-2014:
            // This method used to calculate Stream Position. But AWJ uses ms MemoryStream internally 
            // when a doc is loaded from Java's InputStream. And the position in the MemoryStream  
            // should be smaller than int.Max-1.
            if (isNormalSector)
                return (uint) ((sect + 1) * SectorSize);
            else
                return (uint) (sect * MiniSectorSize);
        }

        internal static uint OffsetToSector(long offset, bool isNormalSector)
        {
            if (isNormalSector)
                return (uint)((offset / SectorSize) - 1);
            else
                return (uint)(offset / MiniSectorSize);
        }

        /// <summary>
        /// Writes free sector fat entries to pad to sector size.
        /// </summary>
        internal static void PadFreeSect(BinaryWriter writer)
        {
            while (writer.BaseStream.Position % SectorSize != 0)
                writer.Write((UInt32)FatEntryType.FreeSect);
        }

        /// <summary>
        /// Normal sector size.
        /// </summary>
        internal const int SectorSize = 512;
        /// <summary>
        /// Mini stream sector size.
        /// </summary>
        internal const int MiniSectorSize = 64;
        /// <summary>
        /// Size of one entry (sect) in fat or in difat.
        /// </summary>
        internal const int EntrySize = 4;
        /// <summary>
        /// How many entries in a FAT sector.
        /// </summary>
        internal const int FatEntriesInSector = SectorSize / EntrySize;
        /// <summary>
        /// One entry at the end is used to chain to next DIFAT block.
        /// </summary>
        internal const int DifatEntriesInSector = FatEntriesInSector - 1;
    }
}
