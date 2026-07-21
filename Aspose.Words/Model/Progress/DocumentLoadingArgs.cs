// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/09/2021 by Dmitry Sokolov

using Aspose.Words.Progress;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// An argument passed into <see cref="IDocumentLoadingCallback.Notify(DocumentLoadingArgs)"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    public sealed class DocumentLoadingArgs
    {
        // Hide default ctr.
        private DocumentLoadingArgs() { }

        /// <summary>
        /// Ctr.
        /// </summary>
        /// <param name="stageData">Stage data.</param>
        /// <param name="progressCalculator">Estimated progress calculator.</param>
        internal DocumentLoadingArgs(
            StageData stageData,
            LoadingProgressCalculator progressCalculator)
        {
            EstimatedProgress =
                DoublePal.Trim(progressCalculator.GetEstimatedProgress(stageData), LoadingProgressCalculator.Precision);
        }


        /// <summary>
        /// Ctr.
        /// </summary>
        /// <param name="estimatedProgress">Estimated progress.</param>
        /// <remarks>
        /// This constructor is called by Pdf2Word plugin.
        /// </remarks>
        internal DocumentLoadingArgs(double estimatedProgress)
        {
            EstimatedProgress = DoublePal.Trim(estimatedProgress, LoadingProgressCalculator.Precision);
        }

        /// <summary>
        /// Overall estimated percentage progress.
        /// </summary>
        public double EstimatedProgress { get; private set; }
    }
}
