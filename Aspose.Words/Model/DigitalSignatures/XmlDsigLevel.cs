// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/08/2024 by Ilya Navrotskiy

namespace Aspose.Words.DigitalSignatures
{
    /// <summary>
    /// Specifies the level of a digital signature based on XML-DSig standard.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum XmlDsigLevel
    {
        /// <summary>
        /// Specifies XML-DSig signature level.
        /// </summary>
        /// <remarks>
        /// A simple digital signature that should not be trusted after its signing certificate expires.
        /// </remarks>
        XmlDSig = 0,
        /// <summary>
        /// Specifies XAdES-EPES signature level.
        /// </summary>
        /// <remarks>
        /// Adds information about the signing certificate to the XML-DSig signature.
        /// A malicious user cannot switch the signing certificate for another certificate with the same public/private key.
        /// </remarks>
        XAdEsEpes = 1
    }
}
