// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/27/2025 by Ilya Navrotskiy

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how Aspose.Words exports OfficeMath to <see cref="SaveFormat.Text"/>.
    /// </summary>
    ///
    /// <dev>
    /// The enumeration values are preserved to be similar to <see cref="MarkdownOfficeMathExportMode"/>.
    /// But just only two enumerations for a while.
    /// </dev>
    public enum TxtOfficeMathExportMode
    {
        /// <summary>
        /// Export OfficeMath as plain text.
        /// </summary>
        Text = 0,

        /// <summary>
        /// Export OfficeMath as LaTeX.
        /// </summary>
        Latex = 3
    }
}
