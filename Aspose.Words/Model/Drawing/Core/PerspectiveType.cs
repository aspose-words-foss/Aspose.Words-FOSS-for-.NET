// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// MS Word defines perspective attributes for shapes. I never actually seen them in MS Word files yet.
    /// </summary>
    internal enum PerspectiveType
    {
        /// <summary>
        /// Apply transform in absolute space centered on shape.
        /// </summary>
        Absolute = 0,   
        /// <summary>
        /// Apply transform to shape geometry.
        /// </summary>
        Shape = 1,      
        /// <summary>
        /// Apply transform to shape geometry.
        /// </summary>
        Drawing = 2   
    }
}
