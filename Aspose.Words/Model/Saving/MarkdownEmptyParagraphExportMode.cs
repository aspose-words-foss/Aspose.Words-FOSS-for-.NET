// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2025 by Ilya Navrotskiy

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how Aspose.Words exports empty paragraphs to Markdown.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum MarkdownEmptyParagraphExportMode
    {
        /// <summary>
        /// Export as empty lines.
        /// </summary>
        /// <remarks>
        /// Note, empty lines are not meaningful in Markdown and will be lost upon loading.
        /// </remarks>
        EmptyLine,

        /// <summary>
        /// Export as Markdown HardLineBreak character '\'.
        /// </summary>
        MarkdownHardLineBreak,

        /// <summary>
        /// Don't export empty paragraphs.
        /// </summary>
        None
    }
}
