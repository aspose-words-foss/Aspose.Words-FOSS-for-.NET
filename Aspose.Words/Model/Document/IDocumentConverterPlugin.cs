// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2023 by Andrey Noskov

using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Loading;
using Aspose.Words.Saving;

namespace Aspose.Words
{
    /// <summary>
    /// Defines an interface for external converter plugin.
    /// </summary>
    [JavaUsePublicApiMapOnly]
    public interface IDocumentConverterPlugin
    {
        /// <summary>
        /// Converts document using specified input output streams and save options.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="loadOptions">The document load options.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="saveOptions">The save options.</param>
        [JavaUseSecondApiChangeMap("outputStream")]
        void Convert(Stream inputStream, LoadOptions loadOptions, Stream outputStream, SaveOptions saveOptions);

        /// <summary>
        /// Converts pages from document from input stream to array of images.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="loadOptions">The document load options.</param>
        /// <param name="saveOptions">The save options.</param>
        /// <returns>Array of page images streams.</returns>
        [JavaUseSecondApiChangeMap("return")]
        Stream[] ConvertToImages(Stream inputStream, LoadOptions loadOptions, SaveOptions saveOptions);
    }
}
