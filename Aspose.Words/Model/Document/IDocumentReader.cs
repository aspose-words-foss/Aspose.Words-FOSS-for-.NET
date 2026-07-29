// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2017 by Alexey Butalov

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Defines an interface for readers that can read a file into a document.
    /// Helps to break dependency Aspose.Words from RW.*.
    /// </summary>
    internal interface IDocumentReader
    {
        /// <summary>
        /// Reads the file into the document.
        /// </summary>
        [JavaThrows(true)]
        void Read();

        bool IsEncrypted { get; }

        [JavaThrows(true)]
        Stream Decrypt();
    }
}
