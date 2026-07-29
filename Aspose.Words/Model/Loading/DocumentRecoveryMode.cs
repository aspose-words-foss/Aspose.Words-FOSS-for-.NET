// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2025 by Vadim Saltykov

namespace Aspose.Words.Loading
{

    /// <summary>
    /// Specifies the available recovery options when a document encounters errors during loading.
    /// </summary>
    public enum DocumentRecoveryMode
    {
        /// <summary>
        /// No recovery is attempted.
        /// If the document is invalid, loading will fail with an error.
        /// </summary>
        None = 0,

        /// <summary>
        /// Attempts to recover the document while preserving as much data as possible.
        /// </summary>
        TryRecover = 1,
    }
}
