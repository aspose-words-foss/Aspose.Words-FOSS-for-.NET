// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2011 by Alexey Morozov

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Provides common interface for ECMA-376 files encryption.
    /// </summary>
    internal interface IEcma376Encryption
    {
        /// <summary>
        /// Decrypts structured storage and writes decrypted content into given stream.
        /// </summary>
        [JavaThrows(true)]
        void Decrypt(Stream encryptionInfo, Stream encryptedStream, Stream stream, string password);

        /// <summary>
        /// Encrypts stream and writes encrypted content into given structured storage.
        /// </summary>
        [JavaThrows(true)]
        void Encrypt(Stream encryptionInfo, Stream stream, Stream encryptedStream, string password);
    }
}
