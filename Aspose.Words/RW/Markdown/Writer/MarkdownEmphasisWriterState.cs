// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// Enumerates all possible Markdown emphasis writer states.
    /// </summary>
    internal enum MarkdownEmphasisWriterState
    {
        /// <summary>
        /// The state is not changing.
        /// </summary>
        None,

        /// <summary>
        /// The state is changing to 'opened'.
        /// </summary>
        Opening,

        /// <summary>
        /// The state is changing to 'closing'.
        /// </summary>
        Closing
    }
}
