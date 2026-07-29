// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2021 by Dmitry Sokolov

using System;
using System.Collections.Generic;
using Aspose.Words.Saving;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Saving progress callback for tests.
    /// </summary>
    public class TestSavingProgressCallback : IDocumentSavingCallback
    {
        /// <summary>
        /// The saving arguments collection. Such arguments accepted by a document saving callback.
        /// </summary>
        public List<DocumentSavingArgs> AcceptedArguments { get { return mAcceptedArguments; } }

        /// <summary>
        /// Progress value after which document saving should be canceled.
        /// </summary>
        public double CancelAt { get; set; }

        /// <summary>
        /// Callback method which called during document saving.
        /// </summary>
        /// <param name="args">Saving arguments.</param>
        public void Notify(DocumentSavingArgs args)
        {
            mAcceptedArguments.Add(args);
            double approximateProgress = args.EstimatedProgress;

            if ((CancelAt > 0.0d) && (CancelAt <= args.EstimatedProgress))
                throw new OperationCanceledException("ApproximateProgress = " + approximateProgress + "; CancelAt = " + CancelAt);
        }

        private readonly List<DocumentSavingArgs> mAcceptedArguments = new List<DocumentSavingArgs>();
    }
}
