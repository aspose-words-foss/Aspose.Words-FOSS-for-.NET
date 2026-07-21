// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/16/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// Type of Dml color representation.
    /// </summary>
    internal enum DmlColorType
    {
        /// <summary>
        /// Color is represented using the red, green, blue RGB color model. 
        /// Red, green, and blue is expressed as sequence of hex digits, RRGGB
        /// </summary>
        HexRgbColor,
        
        /// <summary>
        /// Color is represented using the HSL color model.
        /// </summary>
        HslColor,
        
        /// <summary>
        /// Color is represented using the red, green, blue RGB color model. 
        /// Each component, red, green, and blue is expressed as a percentage from 0% to 100%.
        /// </summary>
        PercentageRgbColor,
        
        /// <summary>
        /// This color can appear in themes. It is represented by scheme color with value "phClr".
        /// </summary>
        PlaceholderColor,
        
        /// <summary>
        /// Color which is bound to one of a predefined collection of colors.
        /// </summary>
        PresetColor,
        
        /// <summary>
        /// Color bound to a user's theme. 
        /// </summary>
        SchemeColor,
        
        /// <summary>
        /// Color bound to predefined operating system elements.
        /// </summary>
        SystemColor,

        /// <summary>
        /// Color which is retrieved from a color style of a chart.
        /// </summary>
        ChartStyleColor
    }
}
