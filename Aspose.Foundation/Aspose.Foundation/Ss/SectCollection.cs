// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.IO;
using System.Text;
using Aspose.Collections;

namespace Aspose.Ss
{
    /// <summary>
    /// This is a collection of SECTs.
    /// SECT is a UInt32 that is used as an index into a FAT and can 
    /// also be converted into offfset in the structured storage file.
    /// </summary>
    internal class SectCollection
    {
        internal SectCollection()
        {
        }

        internal SectCollection(Stream stream)
        {
            stream.Position = 0;
            Read(stream, (int)stream.Length / Sector.EntrySize);
        }

        internal void Read(Stream stream, int entries)
        {
            BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);

            int newCount = mItems.Count + entries;
            if (mItems.Capacity < newCount)
                mItems.Capacity = newCount;

            for (int i = 0; i < entries; i++)
                mItems.Add(reader.ReadUInt32());
        }

        internal MemoryStream ToMemoryStream()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);

            for (uint i = 0; i < mItems.Count; i++)
                writer.Write(this[i]);

            return stream;
        }

        internal void Add(uint value)
        {
            mItems.Add(value);
        }

        internal uint this[uint index]
        {
            get { return (uint)mItems[(int)index]; }
            set { mItems[(int)index] = value; }
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        private readonly LongList mItems = new LongList();
    }
}
