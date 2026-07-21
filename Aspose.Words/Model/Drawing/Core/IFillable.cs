// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/12/2019 by Ilya Navrotskiy

using System.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Implements interface for object that can be filled.
    /// </summary>
    internal interface IFillable
    {
        /// <summary>
        /// Changes type of the fill to Solid.
        /// </summary>
        void Solid();

        /// <summary>
        /// Changes type of the fill to preset texture.
        /// </summary>
        void PresetTextured(PresetTexture presetTexture);

        /// <summary>
        /// Changes type of the fill to a pattern.
        /// </summary>
        void Patterned(PatternType patternType);

        /// <summary>
        /// Returns a PresetTexture for the fill.
        /// </summary>
        [JavaThrows(true)]
        PresetTexture GetPresetTexture();

        /// <summary>
        /// Returns a PatternType for the fill.
        /// </summary>
        [JavaThrows(true)]
        PatternType GetPatternType();

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void TwoColorGradient(GradientStyle style, GradientVariant variant);

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void OneColorGradient(GradientStyle style, GradientVariant variant, double degree);

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        void SetImage(byte[] imageBytes);

        /// <summary>
        /// Sets specified fill to this object.
        /// </summary>
        void SetFill(IFill fill);

        /// <summary>
        /// Returns a double value representing transparency of a specified color.
        /// </summary>
        double GetTransparency(DmlColor color);

        /// <summary>
        /// Sets a specified value to a transparency of specified color .
        /// </summary>
        void SetTransparency(DmlColor color, double value);

        #region The public properties of the old Fill object for compatibility.

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        Color FilledColor { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool OldOn { get; set; }

        /// <summary>
        /// Gets or sets a double value between 0.0 (clear) and 1.0 (opaque) representing the degree
        /// of opacity of the specified fill.
        /// </summary>
        double OldOpacity { get; set; }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        [JavaThrows(true)]
        byte[] FillableImageBytes { get; }

        #endregion

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        Color FillableForeColor { get; set; }

        /// <summary>
        /// Gets a Color object that represents the base foreground color for the fill without modifiers.
        /// </summary>
        Color FillableBaseForeColor { get; }

        /// <summary>
        /// Gets or sets a Color object that represents the background color for the fill.
        /// </summary>
        Color FillableBackColor { get; set; }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the fill.
        /// </summary>
        [JavaThrows(true)]
        ThemeColor FillableForeThemeColor { get; set; }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the fill.
        /// </summary>
        [JavaThrows(true)]
        ThemeColor FillableBackThemeColor { get; set; }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        double FillableForeTintAndShade { get; set; }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        double FillableBackTintAndShade { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool FillableVisible { get; set; }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency of the specified fill.
        /// </summary>
        double FillableTransparency { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the fill rotates with the specified object.
        /// </summary>
        bool RotateWithObject { get; set; }

        /// <summary>
        /// Gets fill type.
        /// </summary>
        FillTypeCore FillType { get; }

        /// <summary>
        /// Gets or sets the alignment for tile texture fill.
        /// </summary>
        TextureAlignment TextureAlignment { get; set; }

        /// <summary>
        /// Gets or sets the angle of the gradient fill.
        /// </summary>
        double GradientAngle { get; set; }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills.
        /// </summary>
        GradientVariant GradientVariant { get; }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// </summary>
        GradientStyle GradientStyle { get; }

        /// <summary>
        /// Gets a collection of <see cref="GradientStop"/> objects for the fill.
        /// </summary>
        /// <remarks>This is facade for <see cref="DmlGradientFill.GradientStops"/> collection.</remarks>
        GradientStopCollection GradientStops { get; }

        /// <summary>
        /// Gets IThemeProvider object.
        /// </summary>
        IThemeProvider FillableThemeProvider { get; }
    }
}
