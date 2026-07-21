// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2025 by Ilya Navrotskiy

using System;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Allows to specify the export mode of hard line breaks in Markdown.
    /// </summary>
    /// <dev>
    /// A line ending (not in a code span or HTML tag) that is preceded by two or more spaces
    /// and does not occur at the end of a block is parsed as a hard line break (rendered in
    /// HTML as a <br /> tag). For a more visible alternative, a backslash before the line
    /// ending may be used instead of two or more spaces.
    /// </dev>
    [Flags]
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum MarkdownLineBreakExportMode
    {
        /// <summary>
        /// Hard line break is represented by backslash character.
        /// </summary>
        Backslash = 0x0000,

        /// <summary>
        /// Hard line break is represented by two spaces.
        /// </summary>
        Spaces = 0x0001
    }
}
