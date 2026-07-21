// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2005 by Roman Korchagin
using System;
using System.IO;
using System.Text;
using Aspose.Collections.Generic;
using Aspose.IO;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Ss
{
    /// <summary>
    /// Structured storage file system without using Win32.
    /// Note this is not a full implementation, but a simplistic and cut down version.
    /// Supports only 512 byte sectors and file size will be limited to 2Gb because of casting ints.
    /// Keeps all structured storage file in memory.
    /// </summary>
    [CppDeclareFriendClass("Aspose.Tests.Ss.TestFileSystem")]
    public class FileSystem
    {
        /// <summary>
        /// A quick check for a signature of a structured storage document.
        /// Preserves the stream position.
        /// </summary>
        public static bool IsStructuredStorage(Stream stream)
        {
            long savePos = stream.Position;
            byte[] data = new byte[4];
            int bytesRead = stream.Read(data, 0, 4);
            stream.Position = savePos;
            return IsStructuredStorage(data, bytesRead);
        }

        /// <summary>
        /// A quick check for a signature of a structured storage document.
        /// </summary>
        public static bool IsStructuredStorage(byte[] data, int bytesRead)
        {
            return (bytesRead >= gStructuredStorageSignature.Length) &&
                   (data[0] == gStructuredStorageSignature[0]) &&
                   (data[1] == gStructuredStorageSignature[1]) &&
                   (data[2] == gStructuredStorageSignature[2]) &&
                   (data[3] == gStructuredStorageSignature[3]);
        }

        /// <summary>
        /// Ctor. Creates a new structured storage with the specified root storage.
        /// </summary>
        public FileSystem(MemoryStorage root)
        {
            mRoot = root;
        }

        /// <summary>
        /// Ctor. Creates a new structured storage with an empty Root storage.
        /// </summary>
        public FileSystem(Guid clsid) : this(new MemoryStorage(clsid))
        {
        }

        /// <summary>
        /// Ctor. Reads structured storage file into memory completely. Closes the file on return.
        /// </summary>
        public FileSystem(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
            {
                Load(stream);
                ReadData();
            }
        }

        /// <summary>
        /// Ctor. Reads structured storage header, FAT and directory into memory, but does not read the data.
        /// Data is loaded automatically when you first access the <see cref="Root"/> property.
        /// Does not close the stream.
        /// </summary>
        public FileSystem(Stream stream)
        {
            Load(stream);
        }

        /// <summary>
        /// Loads structured storage header, FAT and directory from a stream.
        /// Note this always reads from the stream start.
        /// </summary>
        private void Load(Stream stream)
        {
            Debug.Assert(stream != null);
            mFileSystemStream = stream;
            mFileSystemStream.Position = 0;

            mHeader = new Header(new BinaryReader(mFileSystemStream, Encoding.Unicode));
            mFat = new Fat(mFileSystemStream, mHeader);
            mFat.Read();
            CheckZeroFat();

            mMiniFat = new SectCollection(ReadStream(mHeader.SectMiniFatStart, mHeader.MiniFatLength, mHeader.MiniFatLength, true));
            ReadDirectory();
        }

        /// <summary>
        /// WORDSNET-9530 Resiliency against FAT that consists entirely of zeroes.
        /// </summary>
        private void CheckZeroFat()
        {
            const int EntriesToCheck = 10;
            int maxI = System.Math.Min(EntriesToCheck, mFat.Count);
            for (uint i = 0; i < maxI; i++)
            {
                if (mFat[i] != 0)
                    return;
            }
            throw new InvalidOperationException("The FAT in the structured storage document seems to be corrupted.");
        }

        /// <summary>
        /// Saves structured storage to a file.
        /// </summary>
        public void Save(string fileName)
        {
            using (Stream stream = File.Create(fileName))
                Save(stream);
        }

        /// <summary>
        /// Saves structured storage to a stream. Note this always writes at the stream start.
        /// </summary>
        public void Save(Stream stream)
        {
#if !PYNET
            if ((stream != null) && (!stream.CanSeek))
                SaveToNonSeekStream(stream);
            else
                SaveCore(stream);
#endif

#if PYNET
             // WORDSNET-26550. For Python version we have some restrictions when working with streams.
             // So we will use a temporary memory stream for saving.
             SaveToNonSeekStream(stream);
#endif
        }

        /// <summary>
        /// Saves the document to a temporary stream and then copies it to a non-seekable stream.
        /// </summary>
        private void SaveToNonSeekStream(Stream stream)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                // WORDSNET-21179 Use a temporary stream to be able to write to the NonSeekStream.
                SaveCore(memStream);
                memStream.Position = 0;
                StreamUtil.CopyStream(memStream, stream);
            }
        }

        /// <summary>
        /// Saves structured storage to a stream. Note this always writes at the stream start.
        /// </summary>
        private void SaveCore(Stream stream)
        {
            Debug.Assert(stream != null);
            mFileSystemStream = stream;
            //Reserve space for the header
            mFileSystemStream.Position = Sector.SectorSize;

            //Create control structures.
            mHeader = new Header();
            mFat = new Fat(mFileSystemStream, mHeader);
            mMiniFat = new SectCollection();
            mMiniStream = new MemoryStream();

            //Build flat dirEntries from the storage tree and at the same time write streams and update fats.
            mDirEntries = new DirEntryCollection();
            WriteStorage(mRoot, null);

            //Write mini stream and minifat. Its start is recorded in the root directory entry.
            if (mMiniStream.Length > 0)
            {
                mDirEntries[0].SectStart = WriteStream(mMiniStream, true);
                mDirEntries[0].Size = mMiniStream.Length;

                // RK This has to be in a local variable to allow autoporting to Java.
                int csectMiniFat;
                mHeader.SectMiniFatStart = WriteStream(mMiniFat.ToMemoryStream(), true, out csectMiniFat);
                mHeader.CsectMiniFat = csectMiniFat;
            }
            else
                mHeader.SectMiniFatStart = FatEntryType.EndOfChain;

            //Write directory.
            mHeader.SectDirStart = WriteStream(mDirEntries.ToMemoryStream(), true);

            mFat.Write();

            //Finally write the header
            mFileSystemStream.Position = 0;
            mHeader.Write(new BinaryWriter(mFileSystemStream, Encoding.Unicode));

            mFileSystemStream.Position = mFileSystemStream.Length;
        }

        /// <summary>
        /// Recursively writes storage and its children storages and streams into the structured storage.
        /// Creates directory entries and updates fat.
        /// </summary>
        /// <param name="storage">Storage to write.</param>
        /// <param name="storageEntry">Directory entry that corresponds to this storage.
        /// Pass null to indicate root storage.</param>
        private void WriteStorage(MemoryStorage storage, DirEntry storageEntry)
        {
            //Create root storage when given a null directory entry.
            if (storageEntry == null)
            {
                storageEntry = new DirEntry("Root Entry", true, storage.Clsid);
                mDirEntries.Add(storageEntry);
            }

            //Set the pointer to the first child if there is at least one child.
            if (storage.Count > 0)
                storageEntry.Child = (uint)mDirEntries.Count;

            //Write the children.
            for (int i = 0; i < storage.Count; i++)
            {
                DirEntry childEntry;
                string itemName = storage.GetKey(i);
                object item = storage.GetByIndex(i);
                MemoryStorage itemMemoryStorage = item as MemoryStorage;
                Stream itemStream = item as Stream;

                if (itemMemoryStorage != null)
                {
                    //Create directory entry and write a child storage.
                    childEntry = new DirEntry(itemName, false, itemMemoryStorage.Clsid);
                    mDirEntries.Add(childEntry);
                    WriteStorage(itemMemoryStorage, childEntry);
                }
                else if (itemStream != null)
                {
                    //Create a directory entry and write a child stream.
                    childEntry = new DirEntry(itemName, DirEntryType.Stream, itemStream.Length);
                    mDirEntries.Add(childEntry);
                    childEntry.SectStart = WriteStream(itemStream, false);
                }
                else
                    throw new InvalidOperationException("Unknown object in memory storage.");

                //If this is not the last child, set its right sibling to next item.
                if (i < storage.Count - 1)
                    childEntry.RightSib = (uint)mDirEntries.Count;
            }
        }

        /// <summary>
        /// Writes a stream into the structured storage and updates fat or mini fat structures.
        /// Returns the start sector of the written data.
        /// WORDSNET-15430 Allow FileStream to be written.
        /// </summary>
        private uint WriteStream(Stream srcStream, bool isForceFat, out int sectorCount)
        {
            sectorCount = 0;
            if (srcStream.Length == 0)
                return FatEntryType.EndOfChain;

            //Select fat or mini fat structures to use.
            bool isFat = (mHeader.IsBigEnoughForFat(srcStream.Length) || isForceFat);
            SectCollection fat = (isFat) ? mFat : mMiniFat;
            Stream dstStream = (isFat) ? mFileSystemStream : mMiniStream;
            int sectorSize = (isFat) ? Sector.SectorSize : Sector.MiniSectorSize;

            //Remember start sector and write the stream.
            uint sectStart = Sector.OffsetToSector(dstStream.Position, isFat);
            srcStream.Position = 0;
            StreamUtil.CopyStream(srcStream, dstStream);

            //Pad to sector size.
            StreamUtil.SeekToNextPage(dstStream, sectorSize);

            //Update fat to include all sectors used by the stream.
            sectorCount = MathUtil.DivUp(srcStream.Length, sectorSize);
            //Start from 1 because first entry in the chain is known from the directory entry.
            for (uint i = 1; i < sectorCount; i++)
                fat.Add(sectStart + i);
            fat.Add(FatEntryType.EndOfChain);

            return sectStart;
        }

        /// <summary>
        /// Same as above, just does not return number of sectors written.
        /// WORDSNET-15430 Allow FileStream to be written.
        /// </summary>
        private uint WriteStream(Stream srcStream, bool isForceFat)
        {
            int dummy;
            uint startSector = WriteStream(srcStream, isForceFat, out dummy);
            // RK Split the return statement so autoporting can work.
            return startSector;
        }

        /// <summary>
        /// Reads all directory entries into a flat list.
        /// </summary>
        /// <remarks>
        /// This is stored as a stream and it would be nice to read it using ReadStream,
        /// but I cannot since the length of this stream is not known.
        /// </remarks>
        private void ReadDirectory()
        {
            Debug.Assert(mHeader != null, "Header must be read before.");
            Debug.Assert(mFat != null, "Fat must be read before.");

            mDirEntries = new DirEntryCollection();
            BinaryReader reader = new BinaryReader(mFileSystemStream, Encoding.Unicode);
            const int DirEntriesPerSector = Sector.SectorSize / DirEntry.StructureSize;

            //Walk the FAT to go through all directory entries.
            uint sect = mHeader.SectDirStart;

            HashSetGeneric<uint> sectRead = new HashSetGeneric<uint>();
            while (sect != FatEntryType.EndOfChain)
            {
                mFileSystemStream.Position = Sector.SectorToOffset(sect, true);

                for (int i = 0; i < DirEntriesPerSector; i++)
                    mDirEntries.Add(new DirEntry(reader));

                sectRead.Add(sect);

                //Get the next sector of the directory entry.
                sect = mFat[sect];

                // Verify FAT has no cycles.
                if(sectRead.Contains(sect))
                {
                    // WARN.
                    throw new InvalidOperationException("The structured storage file seems to be corrupt. FAT contains cycles.");
                }
            }
        }

        /// <summary>
        /// Reads the specified number of bytes from the beginning of the specified stream.
        /// </summary>
        /// <param name="sectStart"></param>
        /// <param name="streamLength">The full length of the stream as specified in the dir entry.
        /// Needed to decide whether to read from FAT or MiniFAT.</param>
        /// <param name="lengthToRead">The length of the stream to read.</param>
        /// <param name="isForceFat">True to force reading from FAT. False to let the
        /// code decide whether to read from FAT or MiniFAT.</param>
        private MemoryStream ReadStream(
            uint sectStart,
            int streamLength,
            int lengthToRead,
            bool isForceFat)
        {
            lengthToRead = System.Math.Min(streamLength, lengthToRead);

            // SPEED Specify length to reduce the number of memory allocations.
            MemoryStream dstStream = new MemoryStream(lengthToRead);
            dstStream.SetLength(lengthToRead);

            // Select main or mini fat structures.
            bool isFat = (isForceFat || mHeader.IsBigEnoughForFat(streamLength));

            if (!isFat)
            {
                // Read mini stream if present and not loaded.
                DirEntry rootEntry = mDirEntries[0];
                if ((rootEntry.SectStart != DirEntry.NoStream) && (mMiniStream == null))
                    mMiniStream = ReadStream(rootEntry.SectStart, (int)rootEntry.Size, (int)rootEntry.Size, true);

                // RK Resiliency. Reading from mini stream is requested, but mini fat or mini stream is
                // missing, therefore return without reading.
                if ((mHeader.MiniFatLength == 0) || (mMiniStream == null))
                {
                    dstStream.SetLength(0);
                    return dstStream;
                }
            }

            SectCollection fat = (isFat) ? mFat : mMiniFat;
            Stream srcStream = (isFat) ? mFileSystemStream : mMiniStream;
            int sectorSize = (isFat) ? Sector.SectorSize : Sector.MiniSectorSize;

            //Maintain dstPos to know where to write in the destination memory stream.
            int dstPos = 0;

            //Maintain srcPos to avoid unnecessary seeks when sectors are continuous to speed up reading.
            long srcPos = srcStream.Position;

            //Walk the fat and read all sectors of the stream.
            uint sect = sectStart;

            // WORDSNET-5326 Missed EndOfChain marker in FAT (FreeSect occurred instead) but it seems OK for Word so we should do the same.
            while ((sect != FatEntryType.EndOfChain) && (sect != FatEntryType.FreeSect))
            {
                long newSrcPos = Sector.SectorToOffset(sect, isFat);
                if (srcPos != newSrcPos)
                {
                    // WORDSNET-20574 Offset beyond the stream.
                    if (newSrcPos > srcStream.Length)
                        break;

                    srcStream.Position = newSrcPos;
                    srcPos = newSrcPos;
                }

                int bytesRemaining = lengthToRead - dstPos;

                //Normally this is not needed because loop terminated at the end of chain marker.
                //However I've seen an invalid document that has no end marker.
                if (bytesRemaining == 0)
                    break;

                int bytesToRead = System.Math.Min(sectorSize, bytesRemaining);
                int bytesRead = srcStream.Read(dstStream.GetBuffer(), dstPos, bytesToRead);

                // This should not happen, but this does happen.
                // For example in defect 1049 there are 511 bytes instead of 512 at the end of the file.
                if (bytesRead != bytesToRead)
                    Debug.WriteLine("The structured file is probably corrupt. Cannot read all data that was expected, but okay to continue.");

                // WORDSNET-1049 penning document in Aspose.Words fails with 'Object reference not set to an instance of an object.' exception.
                // Used to add number of bytes read, but should add number of bytes that we intended to read.
                // The file was shorter by 1 byte than expected.
                dstPos += bytesToRead;
                srcPos += bytesToRead;

                // WORDSNET-16071 attached document with corrupted stream throws exception in AW.
                // Word reads file well so we should stop reading failed stream
                // and fail (or not - in case the stream is not used) somewhere else
                if (sect >= fat.Count)
                    break;

                sect = fat[sect];
            }

            return dstStream;
        }

        /// <summary>
        /// Reads just the first sector of the first stream that matches this name.
        /// Use this for things like file format detection without loading all storage into memory.
        /// </summary>
        public MemoryStream ReadOneSectorFromStreamInRootStorage(string streamName)
        {
            DirEntry dirEntry = mDirEntries.GetEntryInRoot(streamName);
            if (dirEntry == null)
                throw new InvalidOperationException(string.Format("Cannot find directory entry {0}.", streamName));

            return ReadStream(dirEntry.SectStart, (int)dirEntry.Size, Sector.SectorSize, false);
        }

        /// <summary>
        /// Loads content of all streams and storages into memory.
        /// </summary>
        private void ReadData()
        {
            ReadData(mDirEntries[0], null);
        }

        /// <summary>
        /// Recursively reads content of all streams and storages into memory starting from the specified
        /// directory entry.
        /// </summary>
        private void ReadData(DirEntry dirEntry, MemoryStorage parent)
        {
            // WORDSNET-1045 Document hangs up Aspose.Words when opened.
            // Lets allow reading structured storage files with circular references in the directory
            // because this is what MS Word allows to do.
            if (dirEntry.IsRead)
            {
                Debug.WriteLine("The structured storage file seems to be corrupt. This directory entry was already read, ignoring.");
                return;
            }
            else
            {
                dirEntry.IsRead = true;
            }

            switch (dirEntry.EntryType)
            {
                case DirEntryType.Root:
                case DirEntryType.Storage:
                {
                    MemoryStorage storage = new MemoryStorage(dirEntry.Clsid);
                    // WORDSNET-11915 Prevent reading Root Entry more than once while reading corrupted files.
                    if ((dirEntry.EntryType == DirEntryType.Root) && (mRoot == null))
                        mRoot = storage;
                    else
                        parent.Add(dirEntry.Name, storage);

                    // Recursion. Read all children if they are present.
                    DirEntry child = mDirEntries.GetSafe(dirEntry, dirEntry.Child);
                    if (child != null)
                        ReadData(child, storage);

                    break;
                }
                case DirEntryType.Stream:
                {
                    // WORDSNET-22534 Resilience for invalid entry.
                    if((int)dirEntry.Size >= 0)
                        parent.Add(dirEntry.Name, ReadStream(dirEntry.SectStart, (int)dirEntry.Size, (int)dirEntry.Size, false));
                    break;
                }
                case DirEntryType.Invalid:
                    // This seems to be all zeroes, skip such entries.
                    return;
                default:
                    throw new InvalidOperationException("Unknown type of directory entry in the structured storage.");
            }

            // Read all siblings from left and right completely and add them to the current parent storage.
            DirEntry left = mDirEntries.GetSafe(dirEntry, dirEntry.LeftSib);
            if (left != null)
                ReadData(left, parent);

            DirEntry right = mDirEntries.GetSafe(dirEntry, dirEntry.RightSib);
            if (right != null)
                ReadData(right, parent);
        }

        /// <summary>
        /// Returns true if the structured storage contains the specified storage or stream. Case sensitive compare.
        /// </summary>
        public bool ContainsInRootStorage(string name)
        {
            return (mDirEntries.GetEntryInRoot(name) != null);
        }

        /// <summary>
        /// Access to the in-memory version of the structured storage.
        /// </summary>
        public MemoryStorage Root
        {
            get
            {
                // RK This is delay reading introduced for detecting DOC file format without loading all data.
                if (mRoot == null)
                    ReadData();

                return mRoot;
            }
        }

        public static byte[] StructuredStorageSignature
        {
            get { return gStructuredStorageSignature; }
        }

        /// <summary>
        /// Exposed for unit testing only.
        /// </summary>
        internal Fat Fat
        {
            get { return mFat; }
        }

        /// <summary>
        /// The stream that contains the file in the structured storage format.
        /// </summary>
        private Stream mFileSystemStream;
        private Header mHeader;
        private Fat mFat;
        private SectCollection mMiniFat;
        private DirEntryCollection mDirEntries;
        private MemoryStream mMiniStream;
        private MemoryStorage mRoot;
        private static readonly byte[] gStructuredStorageSignature = new byte[] {0xd0, 0xcf, 0x11, 0xe0};
    }
}
