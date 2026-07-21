// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2016 by Ilya Navrotskiy

using System;
using System.IO;
#if CPLUSPLUS
using System.Text;
#endif

namespace Aspose.IO
{
    /// <summary>
    /// Binary writer that allows to write data to stream by bits.
    /// </summary>
    /// <remarks>
    /// This class uses buffering. Therefore altering of stream position during writing 
    /// may lead to incorrect results.
    /// </remarks>
    internal sealed class BitBinaryWriter : BitBinaryIoBase, IDisposable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <param name="useMsbFirstBitOrdering">
        /// True for MSB first bit ordering.
        /// False for LSB first bit ordering.
        /// </param>
        internal BitBinaryWriter(Stream stream, bool useMsbFirstBitOrdering) : base(useMsbFirstBitOrdering)
        {
#if CPLUSPLUS
            mWriter = new BinaryWriter(stream, Encoding.UTF8, true);
#else
            mWriter = new BinaryWriter(stream);
#endif
        }

        /// <summary>
        /// Writes single bit to a stream.
        /// </summary>
        public void WriteBit(bool value)
        {
            SetCurrentBitToBuffer(value);
            
            CurrentBufferPosition++;
            if (CurrentBufferPosition < BufferSize)
                return;

            WriteCurBuffer();
            InitBuffer();
        }
        
        /// <summary>
        /// Writes specified number of bits from the byte value.
        /// </summary>
        public void WriteValue(byte value, int numberOfBitsFromRight)
        {
            if (numberOfBitsFromRight == 0)
                return;

            if ((numberOfBitsFromRight < 0) || (numberOfBitsFromRight > 8))
                throw new ArgumentOutOfRangeException("numberOfBitsFromRight");

            int curBit = 1 << (numberOfBitsFromRight - 1);

            for (int i = 0; i < numberOfBitsFromRight; i++)
            {
                bool curBitVal = ((value & curBit) != 0);
                WriteBit(curBitVal);
                curBit = curBit >> 1;
            }
        }

        /// <summary>
        /// Writes specified number of bits from the int value.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)] // Manual implementation of perf critical code
        public void WriteValue(int value, int numberOfBitsFromRight)
        {
            if (numberOfBitsFromRight == 0)
                return;

            if ((numberOfBitsFromRight < 0) || (numberOfBitsFromRight > 31))
                throw new ArgumentOutOfRangeException("numberOfBitsFromRight");

            byte[] valueBytes = BitConverter.GetBytes(value);

            int startByte = (numberOfBitsFromRight - 1) / 8;
            int startBit = numberOfBitsFromRight - (startByte * 8);
            
            WriteValue(valueBytes[startByte--], startBit);

            while (startByte >= 0)
                WriteValue(valueBytes[startByte--], 8);
        }

        public void Dispose()
        {
            mWriter.Close();
        }

        /// <summary>
        /// Flushes current buffer to a stream.
        /// </summary>
        internal void Flush()
        {
            if (CurrentBufferPosition < 1)
                return;

            WriteCurBuffer();
        }

        /// <summary>
        /// Writes current buffer to a stream.
        /// </summary>
        private void WriteCurBuffer()
        {
            mWriter.Write(Buffer);
            mWriter.Flush();
        }

        /// <summary>
        /// Sets bit at the current position in buffer.
        /// </summary>
        private void SetCurrentBitToBuffer(bool value)
        {
            Buffer = (byte)BitUtil.SetBit(Buffer, GetCurrentMask(), value);
        }

        private readonly BinaryWriter mWriter;
    }
}
