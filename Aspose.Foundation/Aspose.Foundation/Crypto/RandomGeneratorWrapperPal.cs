// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2016 by Alexey Butalov

using Aspose.JavaAttributes;

namespace Aspose.Crypto
{
    /// <summary>
    /// This class wraps a BouncyCastle's IRandomGenerator object and exposes Aspose.Crypto interface so we don't pass around a BouncyCastle object in AW code.
    /// </summary>
    [JavaManual("Platform abstraction for cryptography library. Manual porting by design.")]
    internal class RandomGeneratorWrapperPal : IRandomGenerator
    {
        internal RandomGeneratorWrapperPal(Org.BouncyCastle.Crypto.Prng.IRandomGenerator wrapped)
        {
            mWrapped = wrapped;
        }

        public void NextBytes(byte[] bytes)
        {
            mWrapped.NextBytes(bytes);
        }

        void IRandomGenerator.AddSeedMaterial(byte[] seed)
        {
            mWrapped.AddSeedMaterial(seed);
        }

        private readonly Org.BouncyCastle.Crypto.Prng.IRandomGenerator mWrapped;
    }
}
