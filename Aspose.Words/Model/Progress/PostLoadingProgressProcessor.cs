// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/09/2021 by Dmitry Sokolov

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Progress processor for the post-loading stage.
    /// Uses numeric values to calculate estimated progress.
    /// </summary>
    internal class PostLoadingProgressProcessor : IncrementalLoadingProgressProcessor
    {
        internal PostLoadingProgressProcessor(
            DocumentBase docBase,
            LoadOptions loadOptions)
            : base(docBase, loadOptions.ProgressCallback, LoadingStageType.Processing, 0)
        {
            if (HasCallback && IsProgressSupported())
                OverallCount = Doc.GetChildNodes(NodeType.Paragraph, true).Count;
        }
    }
}
