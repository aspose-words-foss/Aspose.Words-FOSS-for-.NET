// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2009 by Roman Korchagin
using System;
using System.IO;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// 2.3.3 EncryptionVerifier Structure
    /// The EncryptionVerifier structure is used by Office Binary Document RC4 CryptoAPI 
    /// Encryption and ECMA-376 Document Encryption.
    /// </summary>
    internal class EncryptionVerifier
    {
        internal EncryptionVerifier()
        {
        }

        internal EncryptionVerifier(BinaryReader reader, bool isAes)
        {
            SaltSize = reader.ReadInt32();
            if (SaltSize != 16)
                throw new InvalidOperationException("Unexpected salt size.");

            Salt = reader.ReadBytes(SaltSize);
            EncryptedVerifier = reader.ReadBytes(16);
            VerifierHashSize = reader.ReadInt32();
            EncryptedVerifierHash = reader.ReadBytes(isAes ? 32 : 20);
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(0x00000010);
            writer.Write(Salt);
            writer.Write(EncryptedVerifier);
            writer.Write(0x00000014);
            writer.Write(EncryptedVerifierHash);
        }

        /// <summary>
        /// An unsigned integer that specifies the size of the Salt field. MUST be 0x00000010.
        /// </summary>
        internal int SaltSize;

        /// <summary>
        /// An array of bytes that specifies the salt value used during password hash generation. 
        /// MUST NOT be the same data used for the verifier stored encrypted in the EncryptedVerifier field.
        /// </summary>
        internal byte[] Salt;

        /// <summary>
        /// MUST be the randomly generated Verifier value encrypted using the algorithm 
        /// chosen by the implementation.
        /// </summary>
        internal byte[] EncryptedVerifier;

        /// <summary>
        /// An unsigned integer that specifies the number of bytes needed to contain the hash 
        /// of the data used to generate the EncryptedVerifier field.
        /// </summary>
        internal int VerifierHashSize;

        /// <summary>
        /// An array of bytes that contains the encrypted form of the hash of the randomly 
        /// generated Verifier value. The length of the array MUST be the size of the encryption 
        /// block size multiplied by the number of blocks needed to encrypt the hash of the Verifier. 
        /// If the encryption algorithm is RC4, the length MUST be 20 bytes. 
        /// If the encryption algorithm is AES, the length MUST be 32 bytes.
        /// </summary>
        internal byte[] EncryptedVerifierHash;
    }
}
