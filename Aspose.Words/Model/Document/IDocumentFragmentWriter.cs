// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2006 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Defines an interface for writers that can write a fragment of a document to a string.
    /// </summary>
    internal interface IDocumentFragmentWriter
    {
        /// <summary>
        /// Saves just an individual node (document fragment) into a string.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        string SaveToString(Node node);
    }
}
