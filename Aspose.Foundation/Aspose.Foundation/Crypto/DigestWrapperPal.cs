// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2016 by Alexey Butalov

using Aspose.JavaAttributes;

namespace Aspose.Crypto
{
    /// <summary>
    /// This class wraps a BouncyCastle's IDigest object and exposes Aspose.Crypto IDigest inerface so we don't pass around a BouncyCastle object in AW code.
    /// </summary>
    [JavaManual("Platform abstraction for cryptography library. Manual porting by design.")]
    internal class DigestWrapperPal : Aspose.Crypto.IDigest
    {
        internal DigestWrapperPal(Org.BouncyCastle.Crypto.IDigest wrapped)
        {
            mWrapped = wrapped;
        }

        public void BlockUpdate(byte[] input, int inOff, int length)
        {
            mWrapped.BlockUpdate(input, inOff, length);
        }

        public int DoFinal(byte[] output, int outOff)
        {
            return mWrapped.DoFinal(output, outOff);
        }

        public int GetDigestSize()
        {
            return mWrapped.GetDigestSize();
        }

        public void Reset()
        {
            mWrapped.Reset();
        }

        private readonly Org.BouncyCastle.Crypto.IDigest mWrapped;
    }

}
