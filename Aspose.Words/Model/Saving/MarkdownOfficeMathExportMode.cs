// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/12/2024 by Ilya Navrotskiy

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how Aspose.Words exports OfficeMath to Markdown.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum MarkdownOfficeMathExportMode
    {
        /// <summary>
        /// Export OfficeMath as plain text.
        /// </summary>
        Text = 0,

        /// <summary>
        /// Export OfficeMath as image.
        /// </summary>
        Image = 1,

        /// <summary>
        /// Export OfficeMath as MathML.
        /// </summary>
        MathML = 2,

        /// <summary>
        /// Export OfficeMath as LaTeX.
        /// </summary>
        Latex = 3,

        /// <summary>
        /// Export OfficeMath as LaTeX that is compatible with MarkItDown.
        /// </summary>
        /// <remarks>Please see https://github.com/microsoft/markitdown for details on MarkItDown.</remarks>
        MarkItDown = 4
    }
}
