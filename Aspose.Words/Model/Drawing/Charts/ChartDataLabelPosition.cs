// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Specifies the position for a chart data label.
    /// </summary>
    /// <remarks>
    /// Not all series types allow you to specify label positions. And those that do, do not support all values.
    /// </remarks>
    /// <dev>
    /// Corresponds ST_DLblPos simple type (21.2.3.11 of ISO/IEC 29500).
    /// </dev>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum ChartDataLabelPosition
    {
        /// <summary>
        /// Specifies that a data label should be displayed centered on a data marker.
        /// </summary>
        Center,
        /// <summary>
        /// Specifies that a data label should be displayed to the left of a data marker.
        /// </summary>
        Left,
        /// <summary>
        /// Specifies that a data label should be displayed to the right of a data marker.
        /// </summary>
        Right,
        /// <summary>
        /// Specifies that a data label should be displayed above a data marker.
        /// </summary>
        Above,
        /// <summary>
        /// Specifies that a data label should be displayed below a data marker.
        /// </summary>
        Below,
        /// <summary>
        /// Specifies that a data label should be displayed inside the base of a data marker.
        /// </summary>
        InsideBase,
        /// <summary>
        /// Specifies that a data label should be displayed inside the end of a data marker.
        /// </summary>
        InsideEnd,
        /// <summary>
        /// Specifies that a data label should be displayed outside the end of a data marker.
        /// </summary>
        OutsideEnd,
        /// <summary>
        /// Specifies that a data label should be displayed in the most appropriate position.
        /// </summary>
        BestFit
    }
}
