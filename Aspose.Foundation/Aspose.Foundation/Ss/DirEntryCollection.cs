// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.Collections.Generic;
using System.IO;
using System.Text;
using Aspose.Collections.Generic;

namespace Aspose.Ss
{
    /// <summary>
    /// Typed collection of structured storage directory entries.
    /// </summary>
    /// <remarks>
    /// There is no load method here because reading directory is done sector by sector.
    /// </remarks>
    internal class DirEntryCollection
    {
        internal DirEntry this[uint index]
        {
            get { return (DirEntry)mItems[(int)index]; }
            set { mItems[(int)index] = value; }
        }

        internal int Count
        {
            get { return mItems.Count; }
        }

        internal void Add(DirEntry dirEntry)
        {
            mItems.Add(dirEntry);
        }

        /// <summary>
        /// Finds an entry with the specified name among the direct children of the root storage.
        /// </summary>
        internal DirEntry GetEntryInRoot(string name)
        {
            DirEntry child = GetSafe(this[0], this[0].Child);
            return FindEntryInSiblings(child, name, new SortedIntegerListGeneric<bool>());
        }

        private DirEntry FindEntryInSiblings(DirEntry entry, string name, SortedIntegerListGeneric<bool> visitedEntries)
        {
            if (entry.Name == name)
                return entry;

            DirEntry left = GetSafe(entry, entry.LeftSib);
            if ((left != null) && !visitedEntries.ContainsKey((int)entry.LeftSib))
            {
                visitedEntries.Add((int)entry.LeftSib, false);
                DirEntry result = FindEntryInSiblings(left, name, visitedEntries);
                if (result != null)
                    return result;
            }

            DirEntry right = GetSafe(entry, entry.RightSib);
            if ((right != null) && !visitedEntries.ContainsKey((int)entry.RightSib))
            {
                visitedEntries.Add((int)entry.RightSib, false);
                DirEntry result = FindEntryInSiblings(right, name, visitedEntries);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Gets a dir entry if index is not a NoStream value and if index is within the range.
        /// Otherwise returns null.
        /// WORDSNET-15829 added checking that item's child is not item itself, so preventing endless looping.
        /// </summary>
        internal DirEntry GetSafe(DirEntry parentEntry, uint childIndex)
        {
            if ((childIndex != DirEntry.NoStream) && (childIndex < mItems.Count) && 
                (parentEntry != this[childIndex]))
                return this[childIndex];
            else
                return null;
        }

        /// <summary>
        /// Saves directory into a stream, pads it with empty directory entries to a sector boundary.
        /// </summary>
        /// <returns></returns>
        internal MemoryStream ToMemoryStream()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);

            for (uint i = 0; i < mItems.Count; i++)
                this[i].Write(writer);

            //Pad with empty dir entries to the sector boundary.
            DirEntry emptyDirEntry = new DirEntry();
            while (stream.Length % Sector.SectorSize != 0)
                emptyDirEntry.Write(writer);

            return stream;
        }

        private readonly List<DirEntry> mItems = new List<DirEntry>();
    }
}
