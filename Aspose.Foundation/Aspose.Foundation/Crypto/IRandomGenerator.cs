// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/04/2016 by Roman Korchagin

namespace Aspose.Crypto
{
    /// <summary>
    /// This is modelled after Org.BouncyCastle.Crypto.IRandomGenerator.
    /// We need it to make code auto portable to C++.
    /// 
    /// Note we require implementation of the methods that we really need in AW.
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Fills byte array with random values.
        /// </summary>
        void NextBytes(byte[] bytes);

        /// <summary>
        /// Adds seed material to the generator.
        /// </summary>
        void AddSeedMaterial(byte[] seed);
    }
}
