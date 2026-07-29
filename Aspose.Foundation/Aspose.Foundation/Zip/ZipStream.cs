// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/03/2020 by Alexander Sevidov

using System;
using System.IO;
using Aspose.Common;
using Aspose.JavaAttributes;

namespace Aspose.Zip
{
    /// <summary>
    /// <see cref="Stream"/> implementation used to directly read ZIP entries from DOCX.
    /// </summary>
    [JavaDelete("For ZIP on Java we use a completely different implementation.")]
    public class ZipStream : MemoryStream
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public ZipStream(MemoryStream compressedData, long length, bool isCompressed)
        {
            try
            {
                mLength = length;
                mIsCompressed = isCompressed;

                // WORDSNET-13730 Reduce buffer size to avoid the impossibility to allocate memory for the buffer.
                // This problem may occur when memory is fragmented.
                long defaultBufferSize = 1 << 17; // 128 Kb.
                long optimizedBufferSize = 1 << 15; // 32 Kb.
                long dataBoundarySize = 600 * 1024; // 600 Kb.
                long maxBufferSize = length > dataBoundarySize ? defaultBufferSize : optimizedBufferSize;

                mBufferSize = (int)Math.Min(mLength, maxBufferSize);
                mCompressedStream = compressedData;
                
                InitializeBuffer();
            }
            catch (Exception ex1)
            {
                if (!(ex1 is ZipException))
                    // wrap the original exception and throw
                    throw new ZipException("Cannot extract", ex1);
                throw;
            }
        }

        /// <summary>
        /// Initializes inner buffer - a portion of compressed data, which will be decompressed on a fly.
        /// </summary>
        private void InitializeBuffer()
        {
            mPosition = 0;
            mBufferPosition = 0;
            mCompressedStream.Position = 0;
            if (mBuffer == null)
                mBuffer = new byte[mBufferSize];

            Stream deflateStream = (mIsCompressed)
                                ? new DeflateStream(mCompressedStream, CompressionMode.Decompress, true)
                                : mCompressedStream;

            mDecompressedStream = new CrcCalculatorStream(deflateStream);

            mChunkSize = mDecompressedStream.Read(mBuffer, 0, mBufferSize);
        }

        /// <summary>
        /// The length of uncompressed data.
        /// </summary>
        public override long Length
        {
            get { return mLength; }
        }

        /// <summary>
        /// Current position in uncompressed stream.
        /// </summary>
        public override long Position
        {
            get { return mPosition; }
            set { SetPosition(value); }
        }

        /// <summary>
        /// Sets the position of a stream. It starts reading of uncompressed data from the beginning, if necessary.
        /// </summary>        
        private void SetPosition(long value)
        {
            if (PositionIsInChunk(value))
            {
                mPosition = value;
            }
            else
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative ZipStream position.");
                
                if (value < mBufferPosition)
                    InitializeBuffer();

                while (!PositionIsInChunk(value) && (mChunkSize > 0))
                    ReadNextChunk();

                mPosition = value;
            }
        }

        /// <summary>
        /// Determines, if the <paramref name="value"/> position is in the current chunk.
        /// </summary>
        private bool PositionIsInChunk(long value)
        {
            return (mBufferPosition <= value && value < mBufferPosition + mChunkSize);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }


        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Close()
        {
            mCompressedStream.Close();
            mDecompressedStream.Close();
        }

        protected override void Dispose(bool disposing)
        {
            mCompressedStream.Dispose();
            mDecompressedStream.Dispose();
            
            base.Dispose(disposing);
        }

        /// <summary>
        /// Reads a sequence of uncompressed bytes from the compressed stream and advances the position 
        /// by the number of bytes read.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (mChunkSize == 0)
                return 0;

            int bytesToRead = count;
            long actualRead = 0;

            int dstOffset = offset;

            while (bytesToRead > actualRead) 
            {
                int localPos = (int)mPosition % mBufferSize;
                long localBytesToRead = Math.Min(mChunkSize - localPos, (count - actualRead));

                Array.Copy(mBuffer, localPos, buffer, dstOffset, localBytesToRead);

                mPosition += localBytesToRead;
                actualRead += localBytesToRead;
                dstOffset += (int)localBytesToRead;

                if (!PositionIsInChunk(mPosition))
                    ReadNextChunk();

                if (mChunkSize == 0)
                    return (int)actualRead;
            }

            return (int)actualRead;
        }

        /// <summary>
        /// Reads next chunk.
        /// </summary>        
        private int ReadNextChunk()
        {
            int bytesRead = mDecompressedStream.Read(mBuffer, 0, mBufferSize);
            mBufferPosition += mChunkSize;
            mChunkSize = bytesRead;
            return bytesRead;
        }

        public override int ReadByte()
        {
            if (mChunkSize == 0)
                return -1;

            int result = mBuffer[mPosition % mBufferSize];
            mPosition++;
            if (!PositionIsInChunk(mPosition))
                ReadNextChunk();
            
            return result;
        }

        public override byte[] GetBuffer()
        {
            return ToArray();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    SetPosition(offset);
                    break;

                case SeekOrigin.Current:
                    SetPosition(mPosition + offset);
                    break;

                case SeekOrigin.End:
                    SetPosition(mLength - offset);
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            return mPosition;
        }

        public override byte[] ToArray()
        {
            long old_position = mPosition;

            SetPosition(0);
            byte[] allData = new byte[mLength];

            if (mLength > Int32.MaxValue)
            {
                ReadLargeData(allData, mLength);
            }
            else
            {
                int bytesRead = 0;
                int totalBytesRead = 0;
                int toRead = (int)mLength;

                // Keep reading until we've read all requested bytes or can't read any more
                while (totalBytesRead < toRead &&
                       (bytesRead = Read(allData, totalBytesRead, toRead - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                }

                // Optional: Check if we read all expected bytes
                if (totalBytesRead < toRead)
                {
                    // Handle the case where we couldn't read all the data
                    // This could throw an exception or log a warning depending on your requirements
                    throw new EndOfStreamException($"Requested {toRead} bytes but could only read {totalBytesRead}");
                }
            }

            SetPosition(old_position);
            return allData;
        }

        /// <summary>
        /// Reads data that exceeds Int32.MaxValue length by parts.
        /// </summary>
        private void ReadLargeData(byte[] allData, long length)
        {            
            long actualRead = 0;
            byte[] buffer = new byte[mBufferSize];
            
            int chunkSize = Read(buffer, 0, mBufferSize);

            while ((actualRead < length) && (chunkSize > 0))
            {
                Array.Copy(buffer, 0, allData, actualRead, chunkSize);

                chunkSize = Read(buffer, 0, mBufferSize);
                actualRead += chunkSize;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException(" Zip Stream Write");
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException(" Zip Stream WriteByte");
        }

        public override void WriteTo(Stream stream)
        {
            throw new NotImplementedException(" Zip Stream WriteTo");
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException(" Zip Stream SetLength");
        }

        private readonly Stream mCompressedStream;
        private Stream mDecompressedStream;


        private long mPosition;
        private readonly int mBufferSize;
        private int mChunkSize;
        private long mBufferPosition;
        private readonly long mLength;
        private byte[] mBuffer;

        private readonly bool mIsCompressed;
    }
}
