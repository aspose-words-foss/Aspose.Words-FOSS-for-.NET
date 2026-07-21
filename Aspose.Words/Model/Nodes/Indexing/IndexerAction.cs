// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2016 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies values to customize node indexing.
    /// </summary>
    internal enum IndexerAction
    {
        /// <summary>
        /// No additional processing required.
        /// </summary>
        None,

        /// <summary>
        /// Node and its child should be completely ignored.
        /// </summary>
        Ignore,

        /// <summary>
        /// Node should be ignored but child should be processed.
        /// </summary>
        Skip,

        /// <summary>
        /// Node and its child should be collapsed node into single char.
        /// </summary>
        Collapse
    }
}
