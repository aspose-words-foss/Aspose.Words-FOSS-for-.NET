// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2016 by Alexey Morozov


namespace Aspose.Words.Loading
{

    /// <summary>
    /// Specifies modes for document loading.
    /// </summary>
    /// <remarks>
    /// We've decided to add this enum for DOC Raw-mode extraction to be supported along with doc SkipFormatting mode.
    /// GroupDocs mentioned they would want different kinds of fast reading with gradually degrading text fidelity.
    /// for <see cref="PlainTextDocument"/> construction.
    /// </remarks>
    internal enum LoadMode
    {
        /// <summary>
        /// Normal document loading. Slowest loading speed.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// No formatting but all text is read and parsed. Average loading speed.
        /// </summary>
        SkipFormatting = 1,

        /// <summary>
        /// Raw unparsed text chunks is read. Best loading speed.
        /// </summary>
        /// <remarks>Supported by DOC only.</remarks>
        RawText = 2
    }
}
