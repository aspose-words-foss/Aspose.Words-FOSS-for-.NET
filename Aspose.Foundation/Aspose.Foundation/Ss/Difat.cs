// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2005 by Roman Korchagin
using System;
using System.IO;
using System.Text;
using Aspose.IO;

namespace Aspose.Ss
{
    /// <summary>
    /// Double-indirection FAT - basically FAT of FAT in a structured storage.
    /// </summary>
    internal static class Difat
    {
        /// <summary>
        /// Reads and returns DIFAT entries from the header and from extra DIFAT sectors (if present).
        /// </summary>
        /// <param name="stream">Structured storage we are reading.</param>
        /// <param name="difatEntries">Number of DIFAT entries (number of FAT sectors).</param>
        /// <param name="sectDifStart">First DIFAT sector (will be end of chain if DIFAT fits into the header).</param>
        /// <param name="difatSectors">Number of difat sectors.</param>
        internal static SectCollection Read(Stream stream, int difatEntries, uint sectDifStart, int difatSectors)
        {
            //Create a stream to read difat into.
            MemoryStream difatStream = new MemoryStream();

            //Used to copy between source and difat streams.
            byte[] buf = new byte[Sector.SectorSize];

            //Read DIFAT from the header
            stream.Position = Header.DifatInHeaderOffset;
            int lengthInHeader = System.Math.Min(difatEntries, Header.MaxDifatEntriesInHeader) * Sector.EntrySize;
            StreamUtil.Read(stream, buf, 0, lengthInHeader);

            difatStream.Write(buf, 0, lengthInHeader);

            //Read additional DIFAT sectors (if present).
            uint difatSect = sectDifStart;
            for (int i = 0; i < difatSectors; i++)
            {
                stream.Position = Sector.SectorToOffset(difatSect, true);
                StreamUtil.Read(stream, buf, 0, Sector.SectorSize);

                //Last entry in the DIFAT sector is used to find the next DIFAT sector so exclude it.
                int chainPos = Sector.DifatEntriesInSector * Sector.EntrySize;
                difatStream.Write(buf, 0, chainPos);
                
                //Retrieve next DIFAT sect from the last entry in the buffer.
                difatSect = (uint)(buf[chainPos] | (buf[chainPos + 1] << 8) | (buf[chainPos + 2] << 16) | (buf[chainPos + 3] << 24));
            }

            return new SectCollection(difatStream);
        }

        /// <summary>
        /// Writes DIFAT to the header and to extra DIFAT sectors (if needed) at the current position in the stream.
        /// </summary>
        /// <param name="stream">Where to write difat to.</param>
        /// <param name="sectFatStart">First FAT sector. Note that FAT must be in consequtive sectors.</param>
        /// <param name="difatEntries">Number of DIFAT entries (FAT sectors).</param>
        /// <param name="header">Updated <see cref="Header.SectDifStart"/> and <see cref="Header.CsectDif"/> fields in this structure.</param>
        internal static void Write(
            Stream stream, 
            uint sectFatStart,
            int difatEntries,
            Header header)
        {
            uint sectFat = sectFatStart;

            BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);

            //Write DIFAT sects into the header
            long savePos = stream.Position;
            stream.Position = Header.DifatInHeaderOffset;
            int difatEntriesInHeader = System.Math.Min(difatEntries, Header.MaxDifatEntriesInHeader);
            for (int i = 0; i < difatEntriesInHeader; i++)
            {
                writer.Write((UInt32)sectFat);
                sectFat++;
            }
            Sector.PadFreeSect(writer);
            stream.Position = savePos;

            //Write remaining DIFAT into sectors.
            int remainingDifatEntries = difatEntries - difatEntriesInHeader;
            if (remainingDifatEntries > 0)
            {
                header.SectDifStart = Sector.OffsetToSector(stream.Position, true);
                header.CsectDif = 0;

                //Loop to create as many DIFAT sectors as needed.
                while (remainingDifatEntries > 0)
                {
                    int difatEntriesInSector = System.Math.Min(remainingDifatEntries, Sector.DifatEntriesInSector);
                    for (int i = 0; i < difatEntriesInSector; i++)
                    {
                        writer.Write((UInt32)sectFat);
                        sectFat++;
                    }
                    Sector.PadFreeSect(writer);
                
                    remainingDifatEntries -= difatEntriesInSector;
                    header.CsectDif++;

                    //Rewrite the last entry in the DIFAT sector to chain it to the next DIFAT sector.
                    stream.Position -= Sector.EntrySize;
                    if (remainingDifatEntries > 0)
                    {
                        uint nextDifatSect = Sector.OffsetToSector(stream.Position, true) + 1;
                        writer.Write((UInt32)nextDifatSect);
                    }
                    else
                        writer.Write(FatEntryType.EndOfChain);    
                }
            }
            else
            {
                header.SectDifStart = FatEntryType.EndOfChain;
                header.CsectDif = 0;
            }
        }
    }
}
