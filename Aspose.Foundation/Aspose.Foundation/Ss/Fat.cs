// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2005 by Roman Korchagin

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Aspose.IO;

namespace Aspose.Ss
{
    /// <summary>
    /// Represents the FAT in a structured storage file.
    /// </summary>
    internal class Fat : SectCollection
    {
        internal Fat(Stream stream, Header header)
        {
            mStream = stream;
            mHeader = header;
        }

        /// <summary>
        /// Reads all FAT entires from the structured storage.
        /// </summary>
        internal void Read()
        {
            SectCollection difat = Difat.Read(mStream, mHeader.CsectFat, mHeader.SectDifStart, mHeader.CsectDif);

            // Cache each FAT sector to read FAT entries from it, not from the stream. This is quicker.
            byte[] fatCache = new byte[Sector.SectorSize];
            BinaryReader fatReader = new BinaryReader(new MemoryStream(fatCache), Encoding.Unicode);

            for (uint difatIdx = 0; difatIdx < mHeader.CsectFat; difatIdx++)
            {
                mStream.Position = Sector.SectorToOffset(difat[difatIdx], true);
                StreamUtil.TryRead(mStream, fatCache, 0, Sector.SectorSize);

                fatReader.BaseStream.Position = 0;
                for (int fatIdx = 0; fatIdx < Sector.FatEntriesInSector; fatIdx++)
                    this.Add(fatReader.ReadUInt32());
            }
        }

        /// <summary>
        /// Writes FAT and DIFAT and updates the header.
        /// </summary>
        internal void Write()
        {
            MemoryStream srcStream = base.ToMemoryStream();
            BinaryWriter writer = new BinaryWriter(mStream, Encoding.Unicode);

            //Remember where the fat starts in the file.
            uint sectFatStart = Sector.OffsetToSector(mStream.Position, true);

            //The goal is to make sure that the fat contains an entry for every sector in the
            //file including data, fat and difat sectors.
            int fatEntriesForData = base.Count;
            int fatEntriesForFat = MathUtil.DivUp(fatEntriesForData, Sector.FatEntriesInSector);

            // WORDSNET-1016 sending document via Outlook resulting in corruption of the file.
            // There were 0x80 fat entries for data. This requires a complete 1 sector of fat.
            // But when we write 0x81 fat entries, it actually requires 2 sectors of fat.
            // So we have to recalculate the required number of sectors for the fat once again.
            fatEntriesForFat = MathUtil.DivUp(fatEntriesForData + fatEntriesForFat, Sector.FatEntriesInSector);

            //Write fat entries for data
            mStream.Write(srcStream.GetBuffer(), 0, (int)srcStream.Length);

            //Write fat entries for fat itself
            for (int i = 0; i < fatEntriesForFat; i++)
                writer.Write((UInt32)FatEntryType.FatSect);

            //Figure out whether need DIFAT sectors or not.
            int difatEntries = MathUtil.DivUp((fatEntriesForData + fatEntriesForFat), Sector.FatEntriesInSector);
            if (difatEntries > Header.MaxDifatEntriesInHeader)
            {
                //Need DIFAT.
                int fatEntriesForDifat = MathUtil.DivUp(difatEntries - Header.MaxDifatEntriesInHeader, Sector.DifatEntriesInSector);

                //I don't fully understand this myself, but seems this is needed (TestBigExcel).
                for (int i = 0; i < fatEntriesForDifat; i++)
                    writer.Write((UInt32)FatEntryType.FatSect);

                //Write fat entries for difat
                for (int i = 0; i < fatEntriesForDifat; i++)
                    writer.Write((UInt32)FatEntryType.DifSect);

                difatEntries = MathUtil.DivUp((fatEntriesForData + fatEntriesForFat + fatEntriesForDifat), Sector.FatEntriesInSector);
            }

            mHeader.CsectFat = difatEntries;
            Sector.PadFreeSect(writer);

            Difat.Write(mStream, sectFatStart, difatEntries, mHeader);
        }

        private readonly Stream mStream;
        private readonly Header mHeader;
    }
}
