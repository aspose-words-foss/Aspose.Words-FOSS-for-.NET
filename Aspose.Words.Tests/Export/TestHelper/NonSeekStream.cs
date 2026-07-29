// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2020 by Alexander Sevidov.

using System;
using System.IO;

namespace Aspose.Words.Tests
{
    /// <summary>
    /// A non-seekable stream.
    /// </summary>
    /// <remarks>
    /// It is used for test purposes. Please check Test21179() for the details.
    /// </remarks>
    internal class NonSeekStream : Stream
    {
        public NonSeekStream(Stream baseStream)
        {
            mBaseStream = baseStream;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return mBaseStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return mBaseStream.Length;
            }
        }

        public override long Position 
        {
            get { return mBaseStream.Position; }
            set { throw new NotSupportedException("Seeking is not supported"); } 
        }

        /// <summary>
        /// Sets a position. Used for a test purposes.
        /// </summary>
        public void SetPositionInternal(long value)
        {
            mBaseStream.Position = value;
        }

        /// <summary>
        /// Reads bytes from the stream. Used for a test purposes.
        /// </summary>
        public int ReadBytes(byte[] buffer, int offset, int count) 
        {
            return mBaseStream.Read(buffer, offset, count);
        }

        public override void Flush()
        {
            mBaseStream.Flush();
        }

        [JavaAttributes.JavaThrows("java.io.IOException")]
        public override void Close()
        {
            mBaseStream.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Reading is not supported");
        }
        
        public override int ReadByte()
        {
            throw new NotSupportedException("Reading is not supported");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seeking is not supported");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Seeking is not supported");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            mBaseStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            mBaseStream.WriteByte(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                mBaseStream.Dispose();
        }

        private Stream mBaseStream;
    }
}
