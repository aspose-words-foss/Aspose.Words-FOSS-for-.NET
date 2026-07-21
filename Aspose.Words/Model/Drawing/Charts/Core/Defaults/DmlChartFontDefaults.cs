// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.Defaults
{
    /// <summary>
    /// The class represents font defaults used to render chart.
    /// Font defaults must be taken from theme.
    /// </summary>
    /// <remarks>
    /// The default font is the minor font as defined by the document's theme. The default font size for each element is
    /// the font size of the chart, except for the chart title which is always 120% the font size of the chart. If the chart
    /// does not have a font size set, then the default font size is 10 (and chart title font size is 18). Axis titles and chart
    /// title are bold by default, while all other chart elements are normal. The default font color is the same as the Axis
    /// and Major Gridlines Line Color.
    /// </remarks>
    internal static class DmlChartFontDefaults
    {
        /// <summary>
        /// Default font typeface (name) that is used if font typeface is not set explicitly.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const string DefaultFontName = "Calibri";

        /// <summary>
        /// Default font size that is used if font size if not set explicitly.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultFontSizeInPoints = 10;
    }
}
