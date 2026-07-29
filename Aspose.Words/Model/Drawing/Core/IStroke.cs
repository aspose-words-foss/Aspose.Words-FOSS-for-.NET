// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2014 by Andrey Noskov

using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Interface to define a common public data for <see cref="DmlOutline"/> and <see cref="Stroke"/>.
    /// </summary>
    internal interface IStroke
    {
        /// <summary>
        /// Defines whether the path will be stroked.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        bool On { get; set; }

        /// <summary>
        /// Defines the brush thickness that strokes the path of a shape in points.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 0.75.</p>
        /// </remarks>
        double Weight { get; set; }

        /// <summary>
        /// Defines the color of a stroke.
        /// </summary>
        DrColor ColorInternal { get; set; }

        /// <summary>
        /// Represents an unmodified base color of a stroke.
        /// </summary>
        DrColor ColorInternalUnmodified { get; }

        /// <summary>
        /// Defines the color of a stroke.
        /// </summary>
        /// <remarks>
        /// It seems in MS Word there is no way to set PatternFill for outline
        /// if we work with Dml shape (it is possible for VML Shapes only).
        /// </remarks>
        DrColor Color2Internal { get; set; }

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
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.JoinStyle.Round"/>.</p>
        /// </remarks>
        JoinStyle JoinStyle { get; set; }

        /// <summary>
        /// Defines the cap style for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Aspose.Words.Drawing.EndCap.Flat"/>.</p>
        /// </remarks>
        EndCap EndCap { get; set; }

        /// <summary>
        /// Defines the line style of the stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ShapeLineStyle.Single"/>.</p>
        /// </remarks>
        ShapeLineStyle LineStyle { get; set; }

        /// <summary>
        /// Defines the arrowhead for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowType.None"/>.</p>
        /// </remarks>
        ArrowType StartArrowType { get; set; }

        /// <summary>
        /// Defines the arrowhead for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowType.None"/>.</p>
        /// </remarks>
        ArrowType EndArrowType { get; set; }

        /// <summary>
        /// Defines the arrowhead width for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowWidth.Medium"/>.</p>
        /// </remarks>
        ArrowWidth StartArrowWidth { get; set; }

        /// <summary>
        /// Defines the arrowhead length for the start of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowLength.Medium"/>.</p>
        /// </remarks>
        ArrowLength StartArrowLength { get; set; }

        /// <summary>
        /// Defines the arrowhead width for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowWidth.Medium"/>.</p>
        /// </remarks>
        ArrowWidth EndArrowWidth { get; set; }

        /// <summary>
        /// Defines the arrowhead length for the end of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ArrowLength.Medium"/>.</p>
        /// </remarks>
        ArrowLength EndArrowLength { get; set; }

        /// <summary>
        /// Defines the amount of transparency of a stroke. Valid range is from 0 to 1.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1.</p>
        /// </remarks>
        double Opacity { get; set; }

        /// <summary>
        /// Defines the image for a stroke image or pattern fill.
        /// </summary>
        [JavaThrows(true)]
        byte[] ImageBytes { get; }

        /// <summary>
        /// Defines the type of fill used for the background of a stroke.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="Core.LineFillType.Solid"/>.</p>
        /// </remarks>
        LineFillType LineFillType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DmlFill"/> object of the stroke fill.
        /// </summary>
        DmlFill StrokeFill { get; set; }
    }
}
