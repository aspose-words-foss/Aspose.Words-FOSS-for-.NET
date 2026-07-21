// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/09/2021 by Dmitry Sokolov

using System;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Words.Progress;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Implements processor for document reading stage and uses stream to calculate the estimate progress.
    /// </summary>
    internal class LoadingProgressProcessor :
        ProgressProcessorBase<IDocumentLoadingCallback, DocumentLoadingArgs>
    {
        internal LoadingProgressProcessor(
            DocumentBase docBase,
            LoadOptions loadOptions)
            : base(docBase, loadOptions.ProgressCallback, (int)LoadingStageType.Reading)
        {
        }

        /// <summary>
        /// Notifies a client about overall progress.
        /// </summary>
        internal void Execute(Stream stream)
        {
            Execute(stream, false);
        }

        /// <summary>
        /// Notifies a client about overall progress.
        /// </summary>
        internal void Execute(Stream stream, bool complete)
        {
            if (!IsProgressSupported() || !HasCallback || (stream == null))
                return;

            LoadingProgressCalculator calc = new LoadingProgressCalculator();
            StageData stageData = GetActualStageData(stream, calc, complete);

            // Create arguments and notify a client.
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
    }
}
