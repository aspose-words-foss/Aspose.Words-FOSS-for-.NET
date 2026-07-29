// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/12/2022 by Vadim Saltykov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how links are exported into Markdown.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum MarkdownLinkExportMode
    {
        /// <summary>
        /// Automatically detect export mode for each link.
        /// </summary>
        /// <dev>
        /// Export link as a reference block if it has roundtrip information or is mentioned more than once
        /// in a document. Otherwise, export link as an inline block.
        /// </dev>
        Auto = 0,

        /// <summary>
        /// Export all links as inline blocks.
        /// </summary>
        Inline = 1,

        /// <summary>
        /// Export all links as reference blocks.
        /// </summary>
        Reference = 2
    }
}
