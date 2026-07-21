// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// Directional Override Statii used to determine current letter direction (Table 2 in the Unicode BiDi algorithm).
    /// </summary>
    public enum DirectionalOverrideStatus
    {
        /// <summary>
        /// No override is currently active.
        /// </summary>
        Neutral,
        /// <summary>
        /// Characters are to be reset to R.
        /// </summary>
        RTL,
        /// <summary>
        /// Characters are to be reset to L.
        /// </summary>
        LTR
    }
}
