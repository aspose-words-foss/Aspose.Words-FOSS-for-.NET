// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2010 by Alexey Morozov
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// This interface is intended to provide format-independed access to document parts.
    /// Used in XmlDsig verification.
    /// </summary>
    internal interface IReferenceResolver
    {
        /// <summary>
        /// Returns Stream object resolved by reference Uri.
        /// </summary>
        [JavaThrows(true)]
        MemoryStream Resolve(Reference reference);
    }
}
