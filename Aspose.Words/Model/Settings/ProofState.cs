// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Represents the proofing tools state in this document.
    /// </summary>
    internal enum ProofState
    {
        /// <summary>
        /// No syntax check performed.
        /// </summary>
        None,
        /// <summary>
        /// No errors found.
        /// </summary>
        Clean,
        /// <summary>
        /// Errors are present in the document.
        /// </summary>
        Dirty
    }
}
