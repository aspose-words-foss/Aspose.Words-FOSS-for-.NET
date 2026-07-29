// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2010 by Alexey Morozov
namespace Aspose.Words.Framesets
{
    /// <summary>
    /// Specifies layout for child frames.
    /// </summary>
    /// <remarks>Values correspond to [MS-DOC] 2.9.56 DofrFsn tCols type.</remarks>
    internal enum FrameLayoutType
    {
        /// <summary>
        /// Indicates that frame has no child frames.
        /// </summary>
        None = -1,

        /// <summary>
        /// Child frames should be arranged in rows.
        /// </summary>  
        Vertical = 0x00,
        
        /// <summary>
        /// Child frames should be arranged in columns.
        /// </summary>
        Horizontal = 0x01
    }
}
