// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/09/2021 by Dmitry Sokolov

using System;

namespace Aspose.Words.Progress
{
    /// <summary>
    /// Implements ability to wrap a client exception.
    /// I.e. it allows to re-throw client original exception in the case of cancellation from callback.
    /// </summary>
    internal class CancellationException : Exception
    {
        internal CancellationException(Exception innerException)
            : base("Processing was canceled from progress callback.", innerException)
        {

        }
    }
}
