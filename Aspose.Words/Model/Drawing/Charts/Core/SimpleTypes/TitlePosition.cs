// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// Possible title positions.
    /// Position of title is not stored in the chart XML, 
    /// value is calculated depending on the element the title applied to. 
    /// </summary>
    internal enum TitlePosition
    {
        // The following four positions defines default title position,
        
        /// <summary>
        /// Title is horizontally centered and positioned at top edge.
        /// </summary>
        Top,
        /// <summary>
        /// Title is horizontally centered and positioned at bottom edge.
        /// </summary>
        Bottom,
        /// <summary>
        /// Title is vertically centered and positioned at left edge.
        /// </summary>
        Left,
        /// <summary>
        /// Title is vertically centered and positioned at right edge.
        /// </summary>
        Right,

        // The following four positions are used to define default position of display units labels,
        // which behaves the same as title except of default positions.

        /// <summary>
        /// Title (Display unit label) is horizontal and positioned in top right corner.
        /// </summary>
        TopRight,
        /// <summary>
        /// Title (Display unit label) is horizontal and positioned in bottom right corner.
        /// </summary>
        BottomRight,
        /// <summary>
        /// Title (Display unit label) is vertical and positioned in left top corner.
        /// </summary>
        LeftTop,
        /// <summary>
        /// Title (Display unit label) is vertical and positioned in right top corner.
        /// </summary>
        RightTop
    }
}
