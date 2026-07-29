// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using Aspose.IO;

namespace Aspose.Ss
{
    /// <summary>
    /// Directory entry in a structured storage.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class DirEntry
    {
        /// <summary>
        /// Creates an empty directory entry.
        /// </summary>
        internal DirEntry()
        {
            EntryType = DirEntryType.Invalid;
            EntryColor = DirEntryColor.Black;
            LeftSib = NoStream;
            RightSib = NoStream;
            Child = NoStream;
            Clsid = Guid.Empty;
        }

        /// <summary>
        /// Creates a new storage directory entry.
        /// </summary>
        internal DirEntry(string name, bool isRoot, Guid clsid) : this()
        {
            Name = name;
            EntryType = (isRoot) ? DirEntryType.Root : DirEntryType.Storage;
            Clsid = clsid;
            // As per the clearswift mimesweeper issue and according to the spec,
            // non-root storages should have zero sect start.
            SectStart = (isRoot) ? NoStream : 0;
        }

        /// <summary>
        /// Creates a new stream directory entry.
        /// </summary>
        internal DirEntry(string name, DirEntryType entryType, long size) : this()
        {
            Name = name;
            EntryType = entryType;
            Size = size;
            SectStart = NoStream;
        }

        /// <summary>
        /// Reads a directory entry.
        /// </summary>
        internal DirEntry(BinaryReader reader)
        {
            int startPos = (int)reader.BaseStream.Position;

            byte[] nameBuf = reader.ReadBytes(NameBufLength);

            // WORDSNET-19404 The problem occurred because there is a corrupted directory entry.
            // Its length is only 46 bytes, so we cannot continue reading.
            if (nameBuf.Length < NameBufLength)
                return;

            int nameByteCount = reader.ReadUInt16();

            if(nameByteCount > NameBufLength)
            {
                // WORDSNET-6361 Resilience. Sector is filled with 0xff so nameByteCount is 0xffff.
                // WARN. "The structured storage seems to be corrupt."
                // WORDSNET-28023 Do safe exit on 0xffff entry.
                reader.BaseStream.Position = startPos + StructureSize;
                return;
            }

            if (nameByteCount > 0)
            {
                if (nameByteCount > nameBuf.Length)
                    throw new InvalidOperationException("The structured storage seems to be corrupt.");

                Name = Encoding.Unicode.GetString(nameBuf, 0, nameByteCount - 2); //Exclude zero terminator.
            }
            else
            {
                Name = null;
            }

            EntryType = (DirEntryType)reader.ReadByte();

            // WORDSNET-1048 Opening document in Aspose.Words fails with 'Unable to read beyond the end of the stream.' exception.
            // The directory's last entry is set to be Invalid (not used) and the file is 1 byte short.
            // If I read that directory entry to the end, an exception is thrown.
            // So I'd rather read nothing for invalid (unused) directory entries.
            if (EntryType == DirEntryType.Invalid)
            {
                reader.BaseStream.Position = startPos + StructureSize;
                return;
            }

            EntryColor = (DirEntryColor)reader.ReadByte();
            LeftSib = reader.ReadUInt32();
            RightSib = reader.ReadUInt32();
            Child = reader.ReadUInt32();
            Clsid = new Guid(reader.ReadBytes(ClsidLength));
            UserFlags = reader.ReadUInt32();
            CreateTime = reader.ReadInt64();
            // WORDSNET-4012 Resilience. DirEntry truncated but WordXP still able to open defect file.
            if (StreamUtil.HasEnoughBytesToRead(reader, Jira4012ChunkSize))
            {
                ModifyTime = reader.ReadInt64();
                SectStart = reader.ReadUInt32();
                Size = reader.ReadUInt32();
                reader.ReadInt32(); //Padding
            }
        }

        internal void Write(BinaryWriter writer)
        {
            // Name is a fixed array buffer that contains a Unicode string with zero terminator and the rest are zeroes.
            byte[] nameBuf = new byte[NameBufLength];
            int nameByteCount;
            if (StringUtil.HasChars(Name))
            {
                // Convert full name to Unicode.
                byte[] allNameBuf = Encoding.Unicode.GetBytes(Name);

                // We must have space for the name plus zero terminator in the fixed-size buffer.
                if (allNameBuf.Length > NameBufLength - 2)
                    throw new InvalidOperationException(string.Format("The name '{0}' is too long for writing to structured storage.", Name));

                Array.Copy(allNameBuf, 0, nameBuf, 0, allNameBuf.Length);

                // The number of bytes must include 16 bit zero terminator.
                nameByteCount = allNameBuf.Length + 2;
            }
            else
            {
                nameByteCount = 0;
            }

            writer.Write(nameBuf);
            writer.Write((UInt16)nameByteCount);
            writer.Write((byte)EntryType);
            writer.Write((byte)EntryColor);
            writer.Write((UInt32)LeftSib);
            writer.Write((UInt32)RightSib);
            writer.Write((UInt32)Child);
            writer.Write(Clsid.ToByteArray());
            writer.Write((UInt32)UserFlags);
            writer.Write((Int64)CreateTime);
            writer.Write((Int64)ModifyTime);
            writer.Write((UInt32)SectStart);
            writer.Write((UInt32)Size);
            writer.Write((Int32)0);
        }

        /// <summary>
        /// The element name in Unicode, padded with zeros to fill this byte array.
        /// Terminating Unicode NULL is required.
        /// </summary>
        internal string Name;

        /// <summary>
        /// Type of object. Value taken from the STGTY enumeration
        /// </summary>
        internal DirEntryType EntryType;

        /// <summary>
        /// Value taken from DECOLOR enumeration
        /// </summary>
        internal DirEntryColor EntryColor;

        /// <summary>
        /// SID of the left-sibling of this entryin the directory tree
        /// </summary>
        internal uint LeftSib;

        /// <summary>
        /// SID of the right-sibling of this entry in the directory tree
        /// </summary>
        internal uint RightSib;

        /// <summary>
        /// SID of the child acting as the root of all the children of this element (if _mse=STGTY_STORAGE or STGTY_ROOT)
        /// </summary>
        internal uint Child;

        /// <summary>
        /// CLSID of this storage (if _mse=STGTY_STORAGE or STGTY_ROOT)
        /// </summary>
        internal Guid Clsid;

        /// <summary>
        /// 04] User flags of this storage (if _mse=STGTY_STORAGE or STGTY_ROOT)
        /// </summary>
        internal uint UserFlags;

        /// <summary>
        /// Create time-stamp (if _mse=STGTY_STORAGE)
        /// </summary>
        internal long CreateTime;

        /// <summary>
        /// Modify time-stamp (if _mse=STGTY_STORAGE)
        /// </summary>
        internal long ModifyTime;

        /// <summary>
        /// starting SECT of the stream (if _mse=STGTY_STREAM)
        /// </summary>
        internal uint SectStart;

        /// <summary>
        /// size of stream in bytes (if _mse=STGTY_STREAM)
        /// </summary>
        internal long Size;

        /// <summary>
        /// Not part of structured storage.
        /// This flag is used to prevent infinite loop when reading an invalid directory with circular reference.
        /// We set this after the data of the directory entry was read.
        /// </summary>
        internal bool IsRead;

        /// <summary>
        /// Length of this structure.
        /// </summary>
        internal const int StructureSize = 128;
        /// <summary>
        /// Indicated unallocated directory entry.
        /// </summary>
        internal const uint NoStream = 0xFFFFFFFF;

        private const int NameBufLength = 64;
        private const int ClsidLength = 16;

        /// <summary>
        /// This size defines for ugly resilience Jira4012.
        /// There is certainly invalid file with truncated directory entry,
        /// unfortunately WordXP opens this file without complains and customer wants AW to do the same.
        /// </summary>
        private const int Jira4012ChunkSize = (8 + 4 + 4 + 4);
    }
}
