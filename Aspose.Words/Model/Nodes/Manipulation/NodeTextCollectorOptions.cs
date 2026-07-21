// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2016 by Edward Voronov

namespace Aspose.Words
{
    internal class NodeTextCollectorOptions
    {
        public NodeTextCollectorOptions()
        {
            IsFieldResultMode = false;
            AllowHiddenText = true;
            AllowDeletedText = true;
            AllowInsertedText = true;
            SkipCommentText = true;
            SkipFootnoteText = true;
        }

        /// <summary>
        /// True to include field result only.
        /// False to include whole field content: field code, field result and field control characters.
        /// </summary>
        internal bool IsFieldResultMode { get; set; }

        /// <summary>
        /// True to include hidden inline nodes. False to exclude.
        /// </summary>
        internal bool AllowHiddenText { get; set; }

        /// <summary>
        /// True to include deleted inline nodes. False to exclude.
        /// </summary>
        internal bool AllowDeletedText { get; set; }

        /// <summary>
        /// True to include inserted inline nodes. False to exclude.
        /// </summary>
        internal bool AllowInsertedText { get; set; }

        /// <summary>
        /// Set true to skip comment nodes content. Set false to include it.
        /// </summary>
        internal bool SkipCommentText { get; set; }

        /// <summary>
        /// Set true to skip footnote nodes content. Set false to include it.
        /// </summary>
        internal bool SkipFootnoteText { get; set; }
    }
}
