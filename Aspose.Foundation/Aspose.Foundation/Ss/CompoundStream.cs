// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2016 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.IO;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Ss
{
    /// <summary>
    /// <see cref="Stream"/> implementation used to read from structured storage file.
    /// </summary>
    public class CompoundStream : Stream
    {
        internal CompoundStream(Stream srcStream, SectCollection fat, uint sectStart, int length, int sectorSize)
        {
            mFat = fat;
            mLength = length;
            mSectStart = sectStart;

            mSrcStream = srcStream;
            mSectorSize = sectorSize;

            mPosition = 0;
        }

        public override void Flush()
        {
            // Read-only for a while.
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException("Close()");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    mPosition = offset;
                    break;

                case SeekOrigin.Current:
                    mPosition += offset;
                    break;

                case SeekOrigin.End:
                    mPosition = mLength - offset;
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            return mPosition;
        }

        public override void SetLength(long value)
        {
            // Read-only for a while.
            throw new NotImplementedException();
        }

        // Added for Java. .Net realy uses this code from Stream.ReadByte(). 
        // Needed in optimization.
        public override int ReadByte()
        {
            
            byte[] oneByteArray = new byte[1];
            int r = Read(oneByteArray, 0, 1);
            if (r == 0)
                return -1;
            return oneByteArray[0];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesToRead = Math.Min(count, (int)(mLength - mPosition));
            bytesToRead = Math.Max(bytesToRead, 0);
            long actualRead = 0;

            int dstOffset = offset;
            while (bytesToRead > actualRead)
            {
                byte[] sectBytes = GetSectBytes();

                int localPos = (int)mPosition%mSectorSize;
                long localBytesToRead = Math.Min(mSectorSize - localPos, (count - actualRead));

                Array.Copy(sectBytes, localPos, buffer, dstOffset, localBytesToRead);

                mPosition += localBytesToRead;
                actualRead += localBytesToRead;
                dstOffset += (int)localBytesToRead;
            }

            return (int)actualRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // Read-only for a while.
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value)
        {
            // Read-only for a while.
            throw new NotImplementedException("WriteByte(byte value)");
        }

        /// <summary>
        /// Gets sector data for given position.
        /// </summary>
        // TODO position never used.
        private byte[] GetSectBytes()
        {
            // Starting sector for this position.
            int sectIndex = (int)mPosition / mSectorSize;
            long sectStart = sectIndex * mSectorSize;

            // Try to get cached bytes.
            byte[] sectBytes = mCachedSectBytes.GetValueOrNull(sectStart);
            if (sectBytes != null)
            {
                // Cache hit.
                return sectBytes;
            }

            // Cache miss.
            uint sect;
            if ((sectIndex - 1) == mLastSectIndex)
            {
                // We have cached sect from previous search so use it.
                sect = mFat[mLastSect];
            }
            else
            {
                // Otherwise perform full FAT search.
                sect = mSectStart;
                for (int i = 0; i < sectIndex; i++)
                {
                    if (IsEndSect(sect))
                        throw new InvalidOperationException("Unable to read beyound stream.");

                    sect = mFat[sect];
                }
            }

            mLastSectIndex = sectIndex;
            mLastSect = sect;

            sectBytes = CacheSect(sectStart, sect);

            return sectBytes;
        }

        private static bool IsEndSect(uint sect)
        {
            return (sect == FatEntryType.EndOfChain) || (sect == FatEntryType.FreeSect);
        }

        private byte[] CacheSect(long sectStart, uint sect)
        {
            BinaryReader reader = new BinaryReader(mSrcStream);
            mSrcStream.Position = Sector.SectorToOffset(sect, mSectorSize);
            byte[] sectBytes = reader.ReadBytes(mSectorSize);

            // Remove some entry.
            if (mCachedSectBytes.Count > CacheSize)
            {
                foreach (long key in mCachedSectBytes.Keys)
                {
                    mCachedSectBytes.Remove(key);
                    break;
                }
            }

            mCachedSectBytes.Add(sectStart, sectBytes);

            return sectBytes;
        }

        public override bool CanRead
        {
            [CppConstMethod]
            get { return true; }
        }

        public override bool CanSeek
        {
            [CppConstMethod]
            get { return true; }
        }

        public override bool CanWrite
        {
            [CppConstMethod]
            get { return false; }
        }

        public override long Length
        {
            [CppConstMethod]
            get { return mLength; }
        }

        public override long Position
        {
            [CppConstMethod]
            get { return mPosition; }
            set { mPosition = value; }
        }

        private long mPosition;

        private readonly long mLength;
        private readonly uint mSectStart;

        private readonly SectCollection mFat;
        private readonly int mSectorSize;

        private readonly Stream mSrcStream;

        private readonly Dictionary<long, byte[]> mCachedSectBytes = new Dictionary<long, byte[]>(CacheSize);

        /// <summary>
        /// Sector index from last FAT search. Greatly reduces FAT search count needed.
        /// </summary>
        private int mLastSectIndex = int.MinValue;

        /// <summary>
        ///  Sector number from last FAT search.
        /// </summary>
        private uint mLastSect = 0;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int CacheSize = 10;
    }
}
