// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2021 by Dmitry Sokolov

using Aspose.Words.Progress;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// An argument passed into <see cref="IDocumentSavingCallback.Notify(DocumentSavingArgs)"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    public sealed class DocumentSavingArgs
    {
        // Hide default ctr.
        private DocumentSavingArgs() { }

        /// <summary>
        /// Ctr.
        /// </summary>
        /// <param name="stageData">Stage data.</param>
        /// <param name="progressCalculator">Estimated progress calculator.</param>
        internal DocumentSavingArgs(
            StageData stageData,
            SavingProgressCalculator progressCalculator)
        {
            EstimatedProgress =
                DoublePal.Trim(progressCalculator.GetEstimatedProgress(stageData), SavingProgressCalculator.Precision);
        }

        /// <summary>
        /// Overall estimated percentage progress.
        /// </summary>
        public double EstimatedProgress { get; }
    }
}
