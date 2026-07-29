// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2014 by Alexey Morozov

namespace Aspose.Words.RW.Ole
{
    /// <summary>
    /// Specifies the embedding types of OLE objects.
    /// </summary>
    internal enum OleEmbeddingType
    {
        /// <summary>
        /// Data embedded as OLE package object.
        /// </summary>
        Ole1Package,

        /// <summary>
        /// Data embedded as "Ole1Native" stream.
        /// </summary>
        Ole1NativeStream,

        /// <summary>
        /// Data embedded as structured storage.
        /// </summary>
        Ole2Storage,

        /// <summary>
        /// Data embedded as "Contents" stream.
        /// </summary>
        Ole2StreamContents,

        /// <summary>
        /// Data embedded as "Package" stream.
        /// </summary>
        Ole2StreamPackage,

        /// <summary>
        /// Data embedded as "EmbeddedOdf" stream.
        /// </summary>
        Ole2StreamEmbeddedOdf,

        /// <summary>
        /// Data embedded as "Equation0x0020Native" stream.
        /// </summary>
        Ole2StreamEquationNative,
    }
}
