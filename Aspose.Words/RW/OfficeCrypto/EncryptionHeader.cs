// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2009 by Roman Korchagin

using System.IO;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// 2.3.2 EncryptionHeader Structure
    /// [ECMA-376] document encryption and Office binary document RC4 CryptoAPI 
    /// encryption use the EncryptionHeader structure to specify encryption properties 
    /// for an encrypted stream.
    /// </summary>
    internal class EncryptionHeader
    {
        internal EncryptionHeader(BinaryReader reader)
        {
            int headerSize = reader.ReadInt32();
            Flags = (EncryptionHeaderFlags)reader.ReadInt32();
            SizeExtra = reader.ReadInt32();
            AlgId = (AlgorithmId)reader.ReadInt32();
            AlgIdHash = (AlgorithmIdHash)reader.ReadInt32();
            KeySize = reader.ReadInt32();
            ProviderType = reader.ReadInt32();
            // Skip unused reserved.
            reader.ReadInt32();
            // Skip unused reserved.
            reader.ReadInt32();

            const int fixedHeaderSize = 32;
            CspName = OfficeCryptoUtil.ReadWChar(reader, headerSize - fixedHeaderSize);
        }

        /// <summary>
        /// An EncryptionHeaderFlags structure that specifies properties of the encryption 
        /// algorithm used as specified in section 2.3.1.
        /// </summary>
        internal EncryptionHeaderFlags Flags;

        /// <summary>
        /// Reserved, MUST be 0x00000000.
        /// </summary>
        internal int SizeExtra;

        /// <summary>
        /// A signed integer that specifies the encryption algorithm.
        /// </summary>
        internal AlgorithmId AlgId;

        /// <summary>
        /// A signed integer that specifies the hashing algorithm in concert with the Flags.fExternal bit
        /// </summary>
        internal AlgorithmIdHash AlgIdHash;

        /// <summary>
        /// An unsigned integer that specifies the number of bits in the encryption key. MUST be a multiple of 8.
        /// </summary>
        internal int KeySize;

        /// <summary>
        /// An implementation specified value which corresponds to constants accepted by the specified CSP.
        /// </summary>
        internal int ProviderType;

        /// <summary>
        /// A null-terminated Unicode string that specifies the CSP name.
        /// </summary>
        internal string CspName;
    }
}
