// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

using System;
using System.IO;

namespace Aspose.IO
{
    /// <summary>
    /// Binary reader which allows to read data from stream by bits.
    /// </summary>
    /// <remarks>
    /// This class uses buffering. Therefore altering of stream position during reading 
    /// may lead to incorrect results.
    /// </remarks>
    internal sealed class BitBinaryReader : BitBinaryIoBase, IDisposable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="useMsbFirstBitOrdering">
        /// True for MSB first bit ordering.
        /// False for LSB first bit ordering.
        /// </param>
        public BitBinaryReader(Stream stream, bool useMsbFirstBitOrdering) : base (useMsbFirstBitOrdering)
        {
            mReader = new BinaryReader(stream);
        }

        /// <summary>
        /// Reads single bit from stream.
        /// </summary>
        public bool ReadBit()
        {
            if (CurrentBufferPosition == 0)
                Buffer = (byte)mReader.ReadByte();//JAVA's ReadByte() returns short.

            bool result = GetCurrentBitFromBuffer();

            CurrentBufferPosition++;
            if (CurrentBufferPosition == BufferSize)
                CurrentBufferPosition = 0;

            return result;
        }

        /// <summary>
        /// Reads <paramref name="length"/> bits from stream and returns as an unsigned value.
        /// </summary>
        public long ReadUnsignedValue(int length)
        {
            // kvk: I'm not quite sure how to implement reading of values with LSB ordering.
            // While it is not required just throw NotSupportedException.
            if(!UseMsbFirstBitOrdering)
                throw new NotSupportedException("Reading LSB ordered values is not supported.");

            if (length < 0 || length > 63)
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return 0;

            long value = 0;
            long mask = 1 << (length - 1);
            for (int i = 0; i < length; i++)
            {
                value += ReadBit() ? mask : 0;
                mask = mask >> 1;
            }

            return value;
        }

        public void Dispose()
        {
            mReader.Close();
        }

        /// <summary>
        /// Returns true if bit in buffer at the current position is set to '1'.
        /// </summary>
        private bool GetCurrentBitFromBuffer()
        {
            return BitUtil.IsSetByte(Buffer, GetCurrentMask());
        }

        private readonly BinaryReader mReader;
    }
}
