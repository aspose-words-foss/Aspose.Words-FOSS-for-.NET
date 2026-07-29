// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/09/2016 by Alexey Morozov

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Utility class to store extended comment information. Used only during DOCX import.
    /// </summary>
    internal class CommentEx
    {
        internal CommentEx(int parentParaId, bool done)
        {
            ParentParaId = parentParaId;
            Done = done;
        }

        /// <summary>
        /// Paragraph Id of last paragraph of parent comment.
        /// </summary>
        internal int ParentParaId;

        /// <summary>
        /// Indicates that comment is marked as done.
        /// </summary>
        internal bool Done;
    }
}
