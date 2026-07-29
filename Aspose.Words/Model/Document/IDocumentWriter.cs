// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/12/2003 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Saving;

namespace Aspose.Words
{
    /// <summary>
    /// Defines an interface for writers that can write a model document into a specific format.
    /// </summary>
    internal interface IDocumentWriter
    {
        /// <summary>
        /// Writes the document in the format of the writer.
        /// </summary>
        /// <param name="saveInfo">The parameters that describe what to save.</param>
        [JavaThrows(true)]
        SaveOutputParameters SaveToStream(SaveInfo saveInfo);
    }
}
