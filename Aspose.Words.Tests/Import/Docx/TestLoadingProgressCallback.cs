// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2021 by Dmitry Sokolov

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Loading;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Loading progress callback for tests.
    /// </summary>
    public class TestLoadingProgressCallback : IDocumentLoadingCallback
    {
        /// <summary>
        /// The loading arguments collection. Such arguments accepted by a document loading callback.
        /// </summary>
        public List<DocumentLoadingArgs> AcceptedArguments { get { return mAcceptedArguments; } }

        /// <summary>
        /// Progress value after which document loading should be canceled.
        /// </summary>
        public double CancelAt { get; set; }

        /// <summary>
        /// Callback method which called during document loading.
        /// </summary>
        /// <param name="args">Loading arguments.</param>
        public void Notify(DocumentLoadingArgs args)
        {
            mAcceptedArguments.Add(args);
            double approximateProgress = args.EstimatedProgress;

            if ((CancelAt > 0.0d) && (CancelAt <= args.EstimatedProgress))
            {
                // FormatterPal is used to get the same strings on .Net and Java
                throw new OperationCanceledException("ApproximateProgress = " +
                                                     FormatterPal.DoubleToStr2Decimals(approximateProgress) +
                                                     "; CancelAt = " + FormatterPal.DoubleToStr2Decimals(CancelAt));
            }
        }

        private readonly List<DocumentLoadingArgs> mAcceptedArguments = new List<DocumentLoadingArgs>();
    }
}
