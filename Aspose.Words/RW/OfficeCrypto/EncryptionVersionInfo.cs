// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2012 by Alexey Morozov

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Enumerates values used in Office encryption algorithms.
    /// </summary>
    /// <remarks>
    /// Applications in the 2007 Office system earlier than Service Pack 2 set a Version.vMajor value of 0x0003. 
    /// Versions with Service Pack 2 and Office 2010 set a Version.vMajor value of 0x004. 
    /// Office 2003 applications set a Version.vMajor version value of 0x0002.
    /// </remarks>
    internal enum EncryptionVersionInfo
    {
        /// <summary>
        /// Office Binary RC4 encryption.
        /// </summary>
        RC4 = 0x00010001,

        /// <summary>
        /// Office Binary RC4 CryptoAPI encryption in Office 2003.
        /// </summary>
        CryptoApi = 0x00020002,

        /// <summary>
        /// Office Binary RC4 CryptoAPI encryption in Office 2007.
        /// </summary>
        CryptoApi2007 = 0x00020003,

        /// <summary>
        /// Office Binary RC4 CryptoAPI encryption in Office 2010.
        /// </summary>
        CryptoApi2010 = 0x00020004,

        /// <summary>
        /// ECMA-376 Standard encryption in Office 2007 earlier than Service Pack 2.
        /// </summary>
        Standard = 0x00020003,

        /// <summary>
        /// ECMA-376 Standard encryption in Office 2007 with Service Pack 2 and Office 2010.
        /// </summary>
        Standard2010 = 0x00020004,

        /// <summary>
        /// ECMA-376 Extensible encryption in Office 2007 earlier than Service Pack 2.
        /// </summary>
        Extensible = 0x00030003,

        /// <summary>
        /// ECMA-376 Extensible encryption in Office 2007 with Service Pack 2 and Office 2010.
        /// </summary>
        Extensible2010 = 0x00030004,

        /// <summary>
        /// ECMA-376 Agile encryption.
        /// </summary>
        Agile = 0x00040004
    }
}
