// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2018 by Ilya Navrotskiy

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies the way headers and footers are exported to plain text format.
    /// </summary>
    public enum TxtExportHeadersFootersMode
    {
        /// <summary>
        /// No headers and footers are exported.
        /// </summary>
        None = 0,
        /// <summary>
        /// Only primary headers and footers are exported at the beginning and end of each section.
        /// </summary>
        /// <remarks>
        /// <para>It is hard to meaningfully output headers and footers to plain text because it is not paginated.</para> 
        /// <para>When this mode is used, only primary headers and footers are exported at the beginning and end of each section.</para>
        /// </remarks>
        PrimaryOnly = 1,
        /// <summary>
        /// All headers and footers are placed after all section bodies at the very end of a document.
        /// </summary>
        /// <remarks>
        /// This mode is similar to Word.
        /// </remarks>
        AllAtEnd = 2
    }
}
