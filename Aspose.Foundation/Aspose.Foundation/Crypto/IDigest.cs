// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/04/2016 by Roman Korchagin

namespace Aspose.Crypto
{
    /// <summary>
    /// This is modelled after Org.BouncyCastle.Crypte.IDigest.
    /// We need it to make code auto portable to C++.
    /// </summary>
    public interface IDigest
    {
        void BlockUpdate(byte[] input, int inOff, int length);
        int DoFinal(byte[] output, int outOff);
        int GetDigestSize();
        void Reset();
    }
}
