// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2019 by Ilya Navrotskiy

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Allows to specify the direction to flow the text in a document.
    /// </summary>
    public enum DocumentDirection
    {
        /// <summary>
        /// Left to right direction.  
        /// </summary>
        LeftToRight = 0,

        /// <summary>
        /// Right to left direction.
        /// </summary>
        RightToLeft = 1,
        
        /// <summary>
        /// Auto-detect direction.
        /// </summary>
        /// <remarks>
        /// When this option is selected and text contains characters belonging to RTL scripts,
        /// the document direction will be set automatically to RTL.
        /// </remarks>
        Auto = 2
    }
}
