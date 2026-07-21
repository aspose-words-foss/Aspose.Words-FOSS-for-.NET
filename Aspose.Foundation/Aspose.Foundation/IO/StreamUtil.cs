// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2016 by Dmitry Burov

using System;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.IO
{
    /// <summary>
    /// Contains utility methods for working with streams.
    /// </summary>
    public static class StreamUtil
    {
        /// <summary>
        /// Moves position in the stream to the next boundary of the page of the specified size.
        /// Grows the stream if necessary.
        /// </summary>
        public static void SeekToNextPage(Stream stream, int pageSize)
        {
            int newPos = MathUtil.RoundUp(stream.Position, pageSize);

            if (stream.Length < newPos)
                stream.SetLength(newPos);

            stream.Position = newPos;
        }

        /// <summary>
        /// Reads an exact number of bytes from the specified stream into the buffer starting at the given offset.
        /// Throws <see cref="EndOfStreamException"/> if the stream ends before the requested number of bytes is read.
        /// </summary>
        /// <param name="stream">The input stream to read from.</param>
        /// <param name="buffer">The buffer to store the read bytes.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing data.</param>
        /// <param name="count">The exact number of bytes to read.</param>
        /// <exception cref="EndOfStreamException">
        /// Thrown if the stream ends before <paramref name="count"/> bytes could be read.
        /// </exception>
        public static void Read(Stream stream, byte[] buffer, int offset, int count)
        {
            int totalBytesRead = TryRead(stream, buffer, offset, count);

            if (totalBytesRead != count)
                throw new EndOfStreamException("End of stream reached with " + (count - totalBytesRead) + " bytes left to read");
        }

        /// <summary>
        /// Attempts to read the specified number of bytes from the stream into the buffer starting at the given offset.
        /// Returns the total number of bytes actually read, which may be less than requested if the end of the stream is reached.
        /// </summary>
        /// <param name="stream">The input stream to read from.</param>
        /// <param name="buffer">The buffer to store the read bytes.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing data.</param>
        /// <param name="count">The number of bytes to attempt to read.</param>
        /// <returns>The total number of bytes read into the buffer, which may be less than <paramref name="count"/> if the end of the stream is reached.</returns>
        public static int TryRead(Stream stream, byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = stream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                totalBytesRead += bytesRead;

                // We have reached the end of the stream.
                if (bytesRead == 0)
                    break;
            }

            return totalBytesRead;
        }

        /// <summary>
        /// Reads up to the specified length from the stream and returns the stream position to original.
        /// </summary>
        public static byte[] ReadAndReturn(Stream stream, int length)
        {
            long savePos = stream.Position;

            if (stream.Length - stream.Position < length)
                length = (int)(stream.Length - stream.Position); // read as much as we have

            byte[] data = new byte[length];
            Read(stream, data, 0, data.Length);
            stream.Position = savePos;
            return data;
        }

        /// <summary>
        /// Copies from the current position in src stream till the end.
        /// Copies into the current position in dst stream.
        /// </summary>
        public static void CopyStream(Stream srcStream, Stream dstStream)
        {
            if (srcStream == null)
                throw new ArgumentNullException("srcStream");
            if (dstStream == null)
                throw new ArgumentNullException("dstStream");

            byte[] buf = new byte[65536];
            while (true)
            {
                int bytesRead = srcStream.Read(buf, 0, buf.Length);
                // Read returns 0 when reached end of stream. Checking for negative too to make it conceptually close to Java.
                if (bytesRead <= 0)
                    break;
                else
                    dstStream.Write(buf, 0, bytesRead);
            }
        }

        /// <summary>
        /// Reads first part of the Stream.
        /// </summary>
        public static byte[] ReadStream(Stream srcStream, int numberOfBytesToRead)
        {
            using (MemoryStream dstStream = new MemoryStream())
            {
                byte[] buf = new byte[numberOfBytesToRead];
                int totalBytes = 0;
                while (totalBytes < numberOfBytesToRead)
                {
                    int bytesRead = srcStream.Read(buf, 0, buf.Length);
                    totalBytes += bytesRead;
                    if (bytesRead <= 0)
                        break;

                    dstStream.Write(buf, 0, bytesRead);
                }

                return dstStream.ToArray();
            }
        }

        /// <summary>
        /// If the source stream is a memory stream, then returns it.
        /// Otherwise copies the source into a new memory stream and returns it.
        /// </summary>
        public static MemoryStream CopyStreamIfNotMemory(Stream srcStream)
        {
            MemoryStream srcMemoryStream = srcStream as MemoryStream;
            if (srcMemoryStream != null)
                return srcMemoryStream;

            MemoryStream result = new MemoryStream((int)srcStream.Length);
            CopyStream(srcStream, result);
            return result;
        }

        /// <summary>
        /// Copies a complete stream into a new byte array.
        /// </summary>
        [JavaConvertCheckedExceptions]
        public static byte[] CopyStreamToByteArray(Stream srcStream)
        {
            if (srcStream == null)
                throw new ArgumentNullException("srcStream");

#if JAVA
            // WORDSJAVA-2279 Proved that Java's System.arraycopy is faster on small arrays copying.
            // But we haven't tested it on .NET so we have to make this solution Java-only at the moment.
            if (srcStream is MemoryStream && srcStream.Length < 2e+6)
                return ((MemoryStream)srcStream).ToArray();
#endif

            // RK Casting length to integer to make code autoportable to Java.
            byte[] result = new byte[(int)srcStream.Length];
            using (MemoryStream dstStream = new MemoryStream(result))
            {
                srcStream.Position = 0;
                CopyStream(srcStream, dstStream);
                return result;
            }
        }

        /// <summary>
        /// Opens a file, reads the contents of the file into a byte array, and then closes the file.
        /// </summary>
        /// <param name="fileName">The file to open for reading</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        public static byte[] CopyFileToByteArray(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
                return CopyStreamToByteArray(stream);
        }

        /// <summary>
        /// Checks if reader has enough bytes to read.
        /// </summary>
        public static bool HasEnoughBytesToRead(BinaryReader reader, int bytesCount)
        {
            return bytesCount <= (reader.BaseStream.Length - reader.BaseStream.Position);
        }

        /// <summary>
        /// Writes string in ANSI encoding to stream at the specified offset.
        /// </summary>
        /// <param name="value">The string value to write in ANSI encoding to stream.</param>
        /// <param name="stream">The stream to write to.</param>
        public static void WriteAnsiStringToStream(string value, Stream stream)
        {
            foreach (char c in value)
                stream.WriteByte((byte)c);
        }

        /// <summary>
        /// Returns true, if a specified stream ends with a specified bytes.
        /// </summary>
        public static bool IsEndsWith(Stream stream, byte[] bytes, int defaultBufferLength = 1024)
        {
            if ((bytes.Length == 0) || (stream.Length < bytes.Length))
                return false;

            // Allocate buffer for reading the stream.
            byte[] buffer = new byte[Math.Min(defaultBufferLength, bytes.Length)];

            // Save current stream position.
            long position = stream.Position;

            // Move to position from which we should start to read the stream.
            stream.Position = stream.Length - bytes.Length;

            // The current position within bytes array.
            int bytesPos = 0;

            // Read first portion of stream bytes.
            int count = stream.Read(buffer, 0, buffer.Length);
            while (count > 0)
            {
                int i = 0;
                while (i < count)
                {
                    if (buffer[i] != bytes[bytesPos])
                    {
                        // Restore original stream position before exit.
                        stream.Position = position;
                        return false;
                    }

                    i++;
                    bytesPos++;
                }

                // Read next portion of stream bytes.
                count = stream.Read(buffer, 0, buffer.Length);
            }

            // Restore original stream position before exit.
            stream.Position = position;
            return true;
        }
    }
}
