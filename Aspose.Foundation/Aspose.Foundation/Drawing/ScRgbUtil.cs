// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2011 by Alexey Titov

using System;

namespace Aspose.Drawing
{
    /// <summary>
    /// Represents helper methods to scRGB conversions.
    /// </summary>
    public static class ScRgbUtil
    {
        /// <summary>
        /// Converts sRGB component value to scRGB component.
        /// Code from http://social.msdn.microsoft.com/Forums/en/oxmlsdk/thread/f6d26f2c-114f-4a0d-8bca-a27442aec4d0
        /// </summary>
        public static double RgbToScRgb(int component)
        {
            double normalizedComponent = component / 255.0;
            return Linearize(normalizedComponent);
        }

        /// <summary>
        /// Converts scRGB component value to sRGB component.
        /// Code from http://social.msdn.microsoft.com/Forums/en/oxmlsdk/thread/f6d26f2c-114f-4a0d-8bca-a27442aec4d0
        /// </summary>
        public static int ScRgbToRgb(double component)
        {
            double normalizedComponent = DeLinearize(component);
            return MathUtil.DoubleToInt(normalizedComponent * 255.0);
        }

        /// <summary>
        /// Linearizes the RGB color component.
        /// </summary>
        /// <param name="component">Normalized RGB color component.</param>
        /// <returns>
        /// The inverse sRGB gamma shift of the input color component.
        /// </returns>
        public static double Linearize(double component)
        {
            if (component < 0.0)
                return 0.0;
            if (component <= 0.04045)
                return component / 12.92;
            if (component <= 1.0)
                return Math.Pow((component + 0.055) / 1.055, 2.4);
            return 1.0;
        }

        /// <summary>
        /// De-linearizes the RGB color component.
        /// </summary>
        /// <param name="component">Normalized RGB color component.</param>
        /// <returns>
        /// The sRGB gamma shift of the input color component.
        /// </returns>
        public static double DeLinearize(double component)
        {
            if (component < 0.0)
                return 0.0;
            else if (component <= 0.0031308)
                return component * 12.92;
            else if (component < 1.0)
                return 1.055 * Math.Pow(component, (1.0 / 2.4)) - 0.055;
            else
                return 1.0;
        }
    }
}
