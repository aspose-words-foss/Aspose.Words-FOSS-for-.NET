// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2010 by Alexey Morozov

namespace Aspose.Words.Framesets
{
    /// <summary>
    /// Specifies the scrollbar behavior for a frame.
    /// </summary>
    /// <remarks>Values of members correspond to [MS-DOC] 2.9.122 IScrollType.</remarks>
    internal enum FrameScrollType
    {
        /// <summary>
        /// A scrollbar appears only if it is needed. 
        /// </summary>
        Auto = 0x00,
        
        /// <summary>
        /// A scrollbar always appears on the frame.
        /// </summary>
        Yes = 0x01,
        
        /// <summary>
        /// The frame never has a scrollbar.
        /// </summary>
        No = 0x02
    }
}
