// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/04/2021 by Alexander Zhiltsov

using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Represents an interface to implement for object that can be stroked.
    /// </summary>
    internal interface IStrokable
    {
        /// <summary>
        /// Gets or sets the foreground color of the stroke.
        /// </summary>
        DrColor StrokeForeColor { get; set; }

        /// <summary>
        /// Gets the base foreground color without modifiers of the stroke.
        /// </summary>
        DrColor StrokeBaseForeColor { get; }

        /// <summary>
        /// Gets or sets the background color of the stroke.
        /// </summary>
        DrColor StrokeBackColor { get; set; }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the stroke foreground color.
        /// </summary>
        [JavaThrows(true)]
        ThemeColor StrokeForeThemeColor { get; set; }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the stroke background color.
        /// </summary>
        [JavaThrows(true)]
        ThemeColor StrokeBackThemeColor { get; set; }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the stroke foreground color.
        /// </summary>
        double StrokeForeTintAndShade { get; set; }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the stroke background color.
        /// </summary>
        double StrokeBackTintAndShade { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the stroke is visible.
        /// </summary>
        bool StrokeVisible { get; set; }

        /// <summary>
        /// Gets or sets a value between 0.0 (opaque) and 1.0 (clear) representing the degree of transparency
        /// of the stroke.
        /// </summary>
        double StrokeTransparency { get; set; }

        /// <summary>
        /// Defines the brush thickness that strokes the path of a visual element in points.
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// Specifies the dot and dash pattern for a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.DashStyle.Solid"/>.</p>
        /// </remarks>
        DashStyle DashStyle { get; set; }

        /// <summary>
        /// Defines the join style of a polyline.
        /// </summary>
        JoinStyle JoinStyle { get; set; }

        /// <summary>
        /// Defines the cap style for the end of a stroke.
        /// </summary>
        EndCap EndCap { get; set; }

        /// <summary>
        /// Defines the line style of the stroke.
        /// </summary>
        ShapeLineStyle LineStyle { get; set; }

        /// <summary>
        /// Defines the arrowhead for the start of a stroke.
        /// </summary>
        ArrowType StartArrowType { get; set; }

        /// <summary>
        /// Defines the arrowhead for the end of a stroke.
        /// </summary>
        ArrowType EndArrowType { get; set; }

        /// <summary>
        /// Defines the arrowhead width for the start of a stroke.
        /// </summary>
        ArrowWidth StartArrowWidth { get; set; }

        /// <summary>
        /// Defines the arrowhead length for the start of a stroke.
        /// </summary>
        ArrowLength StartArrowLength { get; set; }

        /// <summary>
        /// Defines the arrowhead width for the end of a stroke.
        /// </summary>
        ArrowWidth EndArrowWidth { get; set; }

        /// <summary>
        /// Defines the arrowhead length for the end of a stroke.
        /// </summary>
        ArrowLength EndArrowLength { get; set; }

        /// <summary>
        /// Gets the image for a stroke image or pattern fill.
        /// </summary>
        [JavaThrows(true)]
        byte[] StrokeImageBytes { get; }

        /// <summary>
        /// Defines the type of fill used for the background of a stroke.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="Core.LineFillType.Solid"/>.
        /// </remarks>
        LineFillType LineFillType { get; set; }

        /// <summary>
        /// Gets a <see cref="IThemeProvider"/> object.
        /// </summary>
        IThemeProvider StrokeThemeProvider { get; }

        /// <summary>
        /// Gets or sets a <see cref="DmlFill"/> object of the stroke fill.
        /// </summary>
        DmlFill StrokeFill { get; set; }
    }
}
