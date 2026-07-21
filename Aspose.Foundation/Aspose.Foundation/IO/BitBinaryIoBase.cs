// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2016 by Ilya Navrotskiy

namespace Aspose.IO
{
    /// <summary>
    /// Base class for binary reader and writer.
    /// </summary>
    internal abstract class BitBinaryIoBase
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="useMsbFirstBitOrdering">
        /// True for MSB first bit ordering.
        /// False for LSB first bit ordering.
        /// </param>
        protected BitBinaryIoBase(bool useMsbFirstBitOrdering)
        {
            UseMsbFirstBitOrdering = useMsbFirstBitOrdering;
            InitBuffer();
        }
        
        /// <summary>
        /// Initializes buffer.
        /// </summary>
        protected void InitBuffer()
        {
            Buffer = 0;
            CurrentBufferPosition = 0;
        }

        /// <summary>
        /// Returns bit mask for specified bit position.
        /// </summary>
        protected byte GetCurrentMask()
        {
            return UseMsbFirstBitOrdering ? (byte)(0x80 >> CurrentBufferPosition) : (byte)(0x01 << CurrentBufferPosition);
        }

        protected readonly bool UseMsbFirstBitOrdering;
        protected byte Buffer;
        protected int CurrentBufferPosition;
        protected const int BufferSize = 8;
    }
}
