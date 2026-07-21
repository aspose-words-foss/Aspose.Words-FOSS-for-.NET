// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Allows to read magnitude dependent encoded data from stream.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#Magnitude for more info.
    /// Stream position should not be altered while reading because BitBinaryReader is used.
    /// </remarks>
    internal sealed class MagnitudeReader : IDisposable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MagnitudeReader(MemoryStream stream)
        {
            mBitReader = new BitBinaryReader(stream, false);
        }

        public void Dispose()
        {
            if (!mDisposed)
            {
                mBitReader.Dispose();
                mDisposed = true;
            }
        }

        /// <summary>
        /// Reads next value from stream.
        /// </summary>
        internal short ReadValue()
        {
            short value = 0;

            // Number of '1' bits before '0' bit equals absolute value.
            while (mBitReader.ReadBit())
            {
                value++;
            }

            // If absolute value > 0, next bit means value sign.
            if ((value > 0) && mBitReader.ReadBit())
                value *= -1;

            return value;
        }

        private readonly BitBinaryReader mBitReader;
        private bool mDisposed;
    }
}
