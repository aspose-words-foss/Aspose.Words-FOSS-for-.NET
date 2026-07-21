// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2022 by Dmitry Sokolov

using System;
using Aspose.JavaAttributes;
using Aspose.Words.Progress;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Implements processor for document reading stage and uses overall
    /// and current numeric values to calculate the estimate progress.
    /// </summary>
    internal class IncrementalLoadingProgressProcessor : ProgressProcessorBase<IDocumentLoadingCallback, DocumentLoadingArgs>
    {
        internal IncrementalLoadingProgressProcessor(
            DocumentBase docBase,
            IDocumentLoadingCallback progressCallback,
            LoadingStageType stageType,
            int overallCount)
                : base(docBase, progressCallback, (int)stageType)
        {
            OverallCount = overallCount;
        }

        /// <summary>
        /// Notifies a client about overall progress.
        /// </summary>
        /// <param name="complete">When true then current stage treated as completed.</param>
        internal void Execute(bool complete)
        {
            if (!IsProgressSupported() || !HasCallback)
                return;

            LoadingProgressCalculator calc = new LoadingProgressCalculator();
            mCurrentCount = complete ? OverallCount : ++mCurrentCount;
            StageData stageData = GetActualStageData(mCurrentCount, OverallCount, calc);

            // Create arguments and notify for a client.
            ExecuteCallback((stageData != null) ? new DocumentLoadingArgs(stageData, calc) : null);
        }

        [JavaThrows(true)]
        protected override void ExecuteCallback(DocumentLoadingArgs args)
        {
            if (args == null)
                return;

            try
            {
                Callback.Notify(args);
            }
            catch (Exception ex)
            {
                // Exception treated as processing cancellation.
                throw new CancellationException(ex);
            }
        }

        protected override bool IsProgressSupported()
        {
            return base.IsProgressSupported() && ProgressUtils.IsLoadingProgressSupported(Doc.OriginalLoadFormat);
        }

        /// <summary>
        /// Counter of progress value.
        /// </summary>
        private int mCurrentCount = 0;

        /// <summary>
        /// Overall amount value which means that current stage is complete.
        /// </summary>
        protected int OverallCount;
    }
}
