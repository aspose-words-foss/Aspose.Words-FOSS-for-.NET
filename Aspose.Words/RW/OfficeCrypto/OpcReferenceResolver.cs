// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2010 by Alexey Morozov

using System.IO;
using Aspose.OpcPackaging;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements interface for resolving OpcPackage parts. Used by the digital signature verification.
    /// </summary>
    internal class OpcReferenceResolver : IReferenceResolver
    {
        internal OpcReferenceResolver(OpcPackageBase package)
        {
            mPackage = package;
        }

        /// <summary>
        /// Resolve needed document part from OPC package.
        /// </summary>
        public MemoryStream Resolve(Reference reference)
        {
            return mPackage.Parts[reference.Name].GetAsMemoryStream();
        }

        private readonly OpcPackageBase mPackage;
    }
}