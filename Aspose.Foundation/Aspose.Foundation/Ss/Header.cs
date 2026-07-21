// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.IO;

namespace Aspose.Ss
{
    /// <summary>
    /// Header of a structured storage.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class Header
    {
        internal Header()
        {
            MinorVersion = 33;
            MajorVersion = SupportedMajorVersion;
            SectorShift = 9;        //512 byte sectors
            MiniSectorShift = 6;    //64 byte sectors
            MiniSectorCutoff = 4096;
        }

        internal Header(BinaryReader reader)
        {
            long sig = reader.ReadInt64();
            if (sig != Signature)
                throw new InvalidOperationException("This is not a structured storage file.");

            reader.ReadBytes(ClsidLength);    //Skip CLSID because it must be zero.
            MinorVersion = reader.ReadUInt16();
            MajorVersion = reader.ReadUInt16();
            if (MajorVersion > SupportedMajorVersion)
                throw new NotSupportedException("This structured storage version is not supported.");

            reader.ReadUInt16();    //byte order
            SectorShift = reader.ReadUInt16();
            MiniSectorShift = reader.ReadUInt16();
            reader.ReadUInt16();    //reserved
            reader.ReadUInt32();    //reserved
            CsectDir = reader.ReadInt32();
            CsectFat = reader.ReadInt32();
            SectDirStart = reader.ReadUInt32();
            reader.ReadUInt32();    //signature
            MiniSectorCutoff = reader.ReadUInt32();
            SectMiniFatStart = reader.ReadUInt32();
            CsectMiniFat = reader.ReadInt32();
            SectDifStart = reader.ReadUInt32();
            CsectDif = reader.ReadInt32();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write((Int64)Signature);
            writer.Write(new byte[ClsidLength]);
            writer.Write((UInt16)MinorVersion);
            writer.Write((UInt16)MajorVersion);
            writer.Write((UInt16)0xfffe);    //byte order
            writer.Write((UInt16)SectorShift);
            writer.Write((UInt16)MiniSectorShift);
            writer.Write((Int16)0);        //reserved
            writer.Write((Int32)0);        //reserved
            writer.Write((Int32)CsectDir);
            writer.Write((Int32)CsectFat);
            writer.Write((UInt32)SectDirStart);
            writer.Write((Int32)0);        //signature
            writer.Write((UInt32)MiniSectorCutoff);
            writer.Write((UInt32)SectMiniFatStart);
            writer.Write((Int32)CsectMiniFat);
            writer.Write((UInt32)SectDifStart);
            writer.Write((Int32)CsectDif);
        }

        /// <summary>
        /// Returns true if the specified stream size is to be written into fat or into mini fat.
        /// </summary>
        internal bool IsBigEnoughForFat(long streamLength)
        {
            return (streamLength >= MiniSectorCutoff);
        }

        internal int MiniFatLength
        {
            get { return CsectMiniFat * Sector.SectorSize; }
        }

        /// <summary>
        /// minor version of the format: 33 is written by reference implementation
        /// </summary>
        internal ushort MinorVersion;

        /// <summary>
        /// major version of the dll/format: 3 for 512-byte sectors, 4 for 4 KB sectors
        /// </summary>
        internal ushort MajorVersion;

        /// <summary>
        /// size of sectors in power-of-two; typically 9 indicating 512-byte sectors
        /// </summary>
        internal ushort SectorShift;

        /// <summary>
        /// size of mini-sectors in power-of-two; typically 6 indicating 64-byte mini-sectors
        /// </summary>
        internal ushort MiniSectorShift;

        /// <summary>
        /// must be zero for 512-byte sectors, number of SECTs in directory chain for 4 KB sectors
        /// </summary>
        internal int CsectDir;

        /// <summary>
        /// number of SECTs in the FAT chain
        /// </summary>
        internal int CsectFat;

        /// <summary>
        /// first SECT in the directory chain
        /// </summary>
        internal uint SectDirStart;

        /// <summary>
        /// maximum size for a mini stream; typically 4096 bytes
        /// </summary>
        internal uint MiniSectorCutoff;

        /// <summary>
        /// first SECT in the MiniFAT chain
        /// </summary>
        internal uint SectMiniFatStart;

        /// <summary>
        /// number of SECTs in the MiniFAT chain
        /// </summary>
        internal int CsectMiniFat;

        /// <summary>
        /// first SECT in the DIFAT chain
        /// </summary>
        internal uint SectDifStart;

        /// <summary>
        /// number of SECTs in the DIFAT chain
        /// </summary>
        internal int CsectDif;

        /// <summary>
        /// Max number of DIFAT records in the header sector.
        /// </summary>
        internal const int MaxDifatEntriesInHeader = 109;
        /// <summary>
        /// Fat sectors start in the header at this offset.
        /// </summary>
        internal const int DifatInHeaderOffset = 0x4c;

        /// <summary>
        /// Version 3 uses 512 byte sectors. Version 4 was introduced in Windows 2000 and uses 4KB sectors.
        /// MS Word uses Version 3 so we can throw everything else.
        /// </summary>
        private const int SupportedMajorVersion = 3;
        private const long Signature = unchecked((long)0xe11ab1a1e011cfd0);
        private const int ClsidLength = 16;
    }
}
