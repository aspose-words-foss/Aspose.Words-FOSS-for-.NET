// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2021 by Ilya Navrotskiy

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the variant for a gradient fill.
    /// </summary>
    /// <remarks>
    /// Corresponds to the four variants on the Gradient tab in the Fill Effects dialog box in Word.
    /// </remarks>
    public enum GradientVariant
    {
        /// <summary>
        /// Gradient variant 'None'.
        /// </summary>
        None,

        /// <summary>
        /// Gradient variant 1.
        /// </summary>
        Variant1,

        /// <summary>
        /// Gradient variant 2.
        /// </summary>
        Variant2,

        /// <summary>
        /// Gradient variant 3.
        /// </summary>
        /// <remarks>
        /// This variant is not applicable to gradient fill with the style <see cref="GradientStyle.FromCenter"/>,
        /// if object has markup language <see cref="ShapeMarkupLanguage.Vml"/>.
        /// </remarks>
        Variant3,

        /// <summary>
        /// Gradient variant 4.
        /// </summary>
        /// <remarks>
        /// This variant is not applicable to gradient fill with the style <see cref="GradientStyle.FromCenter"/>,
        /// if object has markup language <see cref="ShapeMarkupLanguage.Vml"/>.
        /// </remarks>
        Variant4
    }
}
