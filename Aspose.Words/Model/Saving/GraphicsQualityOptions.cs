// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2016 by Vyacheslav Durin

#if !NETSTANDARD // Added after porting additional investigation is required.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Aspose.JavaAttributes;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Allows to specify additional <ms><see cref="Graphics"/> quality options</ms><java><b>java.awt.RenderingHints</b></java>
    /// <cpp><see cref="Graphics"/> quality options</cpp>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/save-a-document/">Save a Document</a> documentation article.</para>
    /// </summary>
    /// <java>
    /// <javaMember type="method" name="java.awt.RenderingHints com.aspose.words.GraphicsQualityOptions.getRenderingHints()">
    /// <summary>
    /// Gets current <b>java.awt.RenderingHints</b> to view or to add new hints.
    /// </summary>
    /// </javaMember>
    /// <javaMember type="method" name="void com.aspose.words.GraphicsQualityOptions.setRenderingHints(java.awt.RenderingHints renderingHints)">
    /// <summary>
    /// Overwrites current <b>java.awt.RenderingHints</b>.
    /// </summary>
    /// </javaMember>
    /// </java>
    [JavaManual("Platform-specific implementation.")]
    public class GraphicsQualityOptions
    {
        /// <summary>
        /// Gets or sets text layout information (such as alignment, orientation and tab stops) display manipulations
        /// (such as ellipsis insertion and national digit substitution) and OpenType features.
        /// </summary>
        public StringFormat StringFormat { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies how composited images are drawn to this Graphics.
        /// </summary>
        public CompositingMode? CompositingMode { get; set; }

        /// <summary>
        /// Gets or sets the rendering quality of composited images drawn to this Graphics.
        /// </summary>
        public CompositingQuality? CompositingQuality { get; set; }

        /// <summary>
        /// Gets or sets the interpolation mode associated with this Graphics.
        /// </summary>
        public InterpolationMode? InterpolationMode { get; set; }

        /// <summary>
        /// Gets or sets the rendering quality for this Graphics.
        /// </summary>
        public SmoothingMode? SmoothingMode { get; set; }

        /// <summary>
        /// Gets or sets the rendering mode for text associated with this Graphics.
        /// </summary>
        public TextRenderingHint? TextRenderingHint { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether WrapMode is TileFlipXY.
        /// </summary>
        /// <remarks>
        /// <p> The <see cref="WrapMode"/> specifies how a texture or gradient is tiled when it is smaller
        /// than the area being filled.</p>
        /// <p>By default uses <see cref="WrapMode.Tile"/> (specifies tiling without flipping).
        /// This causes inaccurate rendering of the scaled image(with high resolution).</p>
        /// <p>This property allows to switch WrapMode to <see cref="WrapMode.TileFlipXY"/> (specifies that tiles are
        /// flipped horizontally as you move along a row and flipped vertically as you move along a column).</p>
        /// </remarks>
        public bool UseTileFlipMode { get; set; }
    }
}

#endif
