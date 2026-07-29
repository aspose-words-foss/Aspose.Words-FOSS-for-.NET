// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2010 by Alexey Morozov

namespace Aspose.Words.DigitalSignatures
{
    /// <summary>
    /// Specifies the type of a digital signature.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum DigitalSignatureType
    {
        /// <summary>
        /// Indicates an error, unknown digital signature type.
        /// </summary>
        Unknown, 
        /// <summary>
        /// The Crypto API signature method used in Microsoft Word 97-2003 .DOC binary documents.
        /// </summary>
        CryptoApi,
        /// <summary>
        /// The XmlDsig signature method used in OOXML and OpenDocument documents.
        /// </summary>
        XmlDsig
    }
}
