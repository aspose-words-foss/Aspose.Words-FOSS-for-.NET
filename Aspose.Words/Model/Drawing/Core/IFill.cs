// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2014 by Andrey Noskov

using Aspose.Drawing;
using Aspose.JavaAttributes;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Interface to define a common data for <see cref="Aspose.Words.Drawing.Core.Dml.Fills.DmlFill"/> and <see cref="Aspose.Words.Drawing.Core.VmlFill"/>.
    /// </summary>
    internal interface IFill
    {
        void SetImageBytes(byte[] imageBytes);

        /// <summary>
        /// Gets or sets a color.
        /// </summary>
        DrColor ColorInternal { get; set; }

        /// <summary>
        /// Gets a base color without modifiers.
        /// </summary>
        DrColor ColorInternalUnmodified { get; }

        /// <summary>
        /// Gets or sets a background color.
        /// </summary>
        DrColor Color2Internal { get; set; }

        /// <summary>
        /// Gets the array of custom gradient colors.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        GradientColor[] GradientColors { get; }

        /// <summary>
        /// Defines the transparency of a fill. Valid range from 0 to 1, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1.</p>
        /// </remarks>
        double Opacity { get; set; }

        /// <summary>
        /// Defines the transparency of the second fill color. Valid range from 0 to 1, where 0 is fully transparent and 1 is fully opaque.
        /// </summary>
        /// <remarks>
        /// <p>The default value is 1.</p>
        /// </remarks>
        double Opacity2 { get; set; }

        /// <summary>
        /// Determines whether the shape will be filled.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>true</c>.</p>
        /// </remarks>
        bool On { get; set; }

        /// <summary>
        /// Determines whether the fill rotates with the specified shape.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>false</c>.</p>
        /// </remarks>
        /// <dev>
        /// IN. Seems this property has not very good name as fill can be defined for also non-Shape objects.
        /// The analogue in VBA has name RotateWithObject. However, in GUI the name is RotateWithShape regardless of
        /// parent object. Also, as I can see, the default value for this property is <c>true</c> in Word (at least Word2016).
        /// </dev>
        bool RotateWithShape { get; set; }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        [JavaThrows(true)]
        byte[] ImageBytes { get; }

        /// <summary>
        /// Defines the angle of a gradient fill in degrees.
        /// </summary>
        /// <remarks>
        /// <p>The vector of a gradient is perpendicular to the vector of the blend direction
        /// from one color to another. The default value is 0 (zero) degrees, which is a horizontal
        /// vector from left to right. Positive angles rotate the gradient in a counter-clockwise direction.</p>
        /// <p>The default value is 0.</p>
        /// </remarks>
        double Angle { get; set; }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientstyle
        /// </summary>
        GradientStyle GradientStyle { get; }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills or 0 if not defined.
        /// https://docs.microsoft.com/en-us/office/vba/api/word.fillformat.gradientvariant
        /// </summary>
        GradientVariant GradientVariant { get; }

        // /// <summary>
        // /// Gets or sets a parent object.
        // /// </summary>
        IFillable Parent { get; set; }

        /// <summary>
        /// Locks or unlocks the aspect ration of a fill picture if exists.
        /// </summary>
        bool LockAspectRatio { get; set; }

        /// <summary>
        /// Defines the left position of the center of a radial gradient.
        /// </summary>
        double FocusLeft { get; set; }

        /// <summary>
        /// Defines the top position of the center of a radial gradient.
        /// </summary>
        double FocusTop { get; set; }

        /// <summary>
        /// Defines the center of a linear gradient fill.
        /// </summary>
        int Focus { get; set; }

        /// <summary>
        /// Defines the type of fill.
        /// </summary>
        FillTypeCore FillType { get; set; }
    }
}
