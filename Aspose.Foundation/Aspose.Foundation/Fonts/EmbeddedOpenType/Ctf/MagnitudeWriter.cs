// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2016 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Allows to write magnitude dependent encoded data to stream.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#Magnitude for more info.
    /// Stream position should not be altered while writing because BitBinaryWriter is used.
    /// </remarks>
    internal sealed class MagnitudeWriter : IDisposable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MagnitudeWriter(MemoryStream stream)
        {
            mBitWriter = new BitBinaryWriter(stream, false);
        }

        public void Dispose()
        {
            if (!mDisposed)
            {
                mBitWriter.Dispose();
                mDisposed = true;
            }
        }

        /// <summary>
        /// Writes next value to stream.
        /// </summary>
        /// <returns>The number of the written bits.</returns>
        internal int WriteValue(short value)
        {
            if (value == 0)
            {
                mBitWriter.WriteBit(false);
                return 1;
            }
            
            bool isNegative = (value < 0);
            
            // The value will be encoded as a number of '1' bits before '0' bit.
            int bitsCount = (isNegative) ? -value : value;
            
            for (int i = 0; i < bitsCount; i++)
                mBitWriter.WriteBit(true);
            
            // Write '0' bit after value bits are written.
            mBitWriter.WriteBit(false);
            
            // If value is negative, then after '0' bit we should write '1' bit, otherwise '0' bit.
            mBitWriter.WriteBit(isNegative);

            return bitsCount + 2;
        }

        /// <summary>
        /// Flushes current buffer of underlying bit binary writer to a stream.
        /// </summary>
        internal void Flush()
        {
            mBitWriter.Flush();
        }

        private readonly BitBinaryWriter mBitWriter;
        private bool mDisposed;
    }
}
