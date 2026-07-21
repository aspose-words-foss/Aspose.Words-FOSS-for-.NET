// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2021 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents an interface to implement for objects that provide a <see cref="ChartFormat"/>.
    /// </summary>
    internal interface IChartFormatSource
    {
        /// <summary>
        /// Materializes a <see cref="DmlChartSpPr"/> property that stores fill/outline data in preparation for updating
        /// it. The method is needed for a case when the property refers to a parent <see cref="DmlChartSpPr"/> instance.
        /// </summary>
        void MaterializeSpPr();

        /// <summary>
        /// Gets a flag indicating whether fill properties are supported by a chart element.
        /// </summary>
        bool IsFillSupported { get; }

        /// <summary>
        /// Gets or sets fill properties of a chart element.
        /// </summary>
        DmlFill Fill { get; set; }

        /// <summary>
        /// Gets or sets outline properties of a chart element.
        /// </summary>
        DmlOutline Outline { get; set; }

        /// <summary>
        /// Gets or sets the shape type of a chart element.
        /// </summary>
        ChartShapeType ShapeType { get; set; }

        /// <summary>
        /// A theme provider to be able to get the current theme properties.
        /// </summary>
        IThemeProvider ThemeProvider { get; }

        /// <summary>
        /// Gets a flag indicating whether any format is defined.
        /// </summary>
        bool IsFormatDefined { get; }
    }
}
