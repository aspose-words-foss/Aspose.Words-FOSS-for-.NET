// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2023 by Andrey Noskov

using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Loading;

namespace Aspose.Words
{
    /// <summary>
    /// Defines an interface for external merger plugin that can merge Pdf documents.
    /// </summary>
    [JavaUsePublicApiMapOnly]
    public interface IDocumentMergerPlugin
    {
        /// <summary>
        /// Merges the given input PDF documents into a single output PDF document using specified input and output streams.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="inputStreams">The input streams.</param>
        /// <param name="loadOptions">Load options for the input files.</param>
        [JavaUseSecondApiChangeMap("outputStream")]
        void Merge(Stream outputStream, Stream[] inputStreams, LoadOptions[] loadOptions);
    }
}
