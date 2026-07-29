// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2009 by Roman Korchagin
using System;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// The EncryptionHeaderFlags structure specifies properties of the encryption algorithm used. 
    /// It is always contained within an EncryptionHeader structure.
    /// </summary>
    [Flags]
    internal enum EncryptionHeaderFlags
    {
        /// <summary>
        /// A flag that specifies whether CryptoAPI RC4 or [ECMA-376] encryption is used. 
        /// MUST be 1 unless fExternal is 1. If fExternal is 1, MUST be 0.
        /// </summary>
        CryptoApi = 0x0004,
        /// <summary>
        /// MUST be 0 if document properties are encrypted. Encryption of document properties 
        /// is specified in section 2.3.5.4.
        /// </summary>
        DocProps = 0x0008,
        /// <summary>
        /// If extensible encryption is used, MUST be 1. If this field is 1, all other fields 
        /// in this structure MUST be 0.
        /// </summary>
        External = 0x0010,
        /// <summary>
        /// If the protected content is an [ECMA-376] document, MUST be 1. 
        /// If the fAES bit is 1, the fCryptoAPI bit MUST also be 1.
        /// </summary>
        Aes = 0x0020
    }
}
