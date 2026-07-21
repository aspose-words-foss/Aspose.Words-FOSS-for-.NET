// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2010 by Alexey Morozov
namespace Aspose.Words.Framesets
{
    /// <summary>
    /// Specifies border type of entire frameset.
    /// </summary>
    /// <remarks>
    /// Corresponds to "[MS-DOC] 2.9.60 DofrFsnSpbd" fNoBorder and f3DBorder flags.
    /// </remarks>
    internal enum FramesetBorderType
    {
        /// <summary>
        /// Entire frameset has no border.
        /// </summary>
        None,

        /// <summary>
        /// Entire frameset has flat border.
        /// </summary>
        Simple,

        /// <summary>
        /// Entire frameset has 3D border.
        /// </summary>
        Raised
    }
}
