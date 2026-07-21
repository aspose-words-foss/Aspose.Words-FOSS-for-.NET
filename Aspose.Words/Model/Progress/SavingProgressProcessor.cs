// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2021 by Dmitry Sokolov

using Aspose.JavaAttributes;
using Aspose.Words.Progress;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Progress processor for the saving stages.
    /// Uses document paragraph count to calculate estimated progress.
    /// </summary>
    internal class SavingProgressProcessor :
        ProgressProcessorBase<IDocumentSavingCallback, DocumentSavingArgs>
    {
        internal SavingProgressProcessor(
            DocumentBase docBase,
            SaveOptions saveOptions,
            SavingStageType stageType)
            : base(docBase, saveOptions.ProgressCallback, (int)stageType)
        {
            mSaveFormat = saveOptions.SaveFormat;

            if (HasCallback && IsProgressSupported())
                mTotalParaCount = Doc.GetChildNodes(NodeType.Paragraph, true).Count;
        }

        /// <summary>
        /// Notifies a client about next step of overall progress.
        /// </summary>
        internal void Step()
        {
            Step(1);
        }

        /// <summary>
        /// Notifies a client about next step of overall progress.
        /// </summary>
        internal void Step(int stepCount)
        {
            mProgressNodesCounter += stepCount - 1; // It will be also incremented in Execute.
            Execute(false);
        }

        /// <summary>
        /// Notifies a client about overall progress completion.
        /// </summary>
        internal void Complete()
        {
            Execute(true);
        }

        /// <summary>
        /// Notify a client using specified callback.
        /// </summary>
        [JavaThrows(true)]
        protected override void ExecuteCallback(DocumentSavingArgs args)
        {
            if (args == null)
                return;

            Callback.Notify(args);
        }

        /// <summary>
        /// Determines that progress supported in current context.
        /// </summary>
        protected override bool IsProgressSupported()
        {
            return base.IsProgressSupported() && ProgressUtils.IsSavingProgressSupported(mSaveFormat);
        }

        /// <summary>
        /// Notifies a client about overall progress.
        /// </summary>
        /// <param name="complete">When true then current stage treated as completed.</param>
        private void Execute(bool complete)
        {
            if (!IsProgressSupported() || !HasCallback)
                return;

            SavingProgressCalculator calc = new SavingProgressCalculator();
            mProgressNodesCounter = complete ? mTotalParaCount : ++mProgressNodesCounter;
            StageData stageData = GetActualStageData(mProgressNodesCounter, mTotalParaCount, calc);

            // Create arguments and notify a client.
            ExecuteCallback((stageData != null) ? new DocumentSavingArgs(stageData, calc) : null);
        }

        /// <summary>
        /// Overall amount of paragraphs which were added to the model during reading.
        /// </summary>
        private readonly int mTotalParaCount;

        /// <summary>
        /// Counter of processed nodes.
        /// </summary>
        private int mProgressNodesCounter = 0;

        /// <summary>
        /// Destination format.
        /// </summary>
        private readonly SaveFormat mSaveFormat;
    }
}
