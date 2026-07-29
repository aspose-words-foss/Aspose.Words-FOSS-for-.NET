// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// The four different available letter presentation forms.
    /// </summary>
    public enum LetterForm
    {
        /// <summary>
        /// A presentation form of a letter that begins a sequence of connected letters.
        /// </summary>
        Initial,
        /// <summary>
        /// A presentation form of a letter that is connected to other letters on both sides.
        /// </summary>
        Medial,
        /// <summary>
        /// A presentation form of a letter that ends a sequence of connected letters.
        /// </summary>
        Final,
        /// <summary>
        /// A presentation form of a letter that is not connected to other letters on either sides.
        /// </summary>
        Isolated
    }
}
