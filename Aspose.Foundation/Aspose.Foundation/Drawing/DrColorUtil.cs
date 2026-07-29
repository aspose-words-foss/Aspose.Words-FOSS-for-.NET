// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2017 by Sergey Merkulov

using System;

namespace Aspose.Drawing
{
    /// <summary>
    /// Provides utility methods to work with <see cref="DrColor"/> class.
    /// </summary>
    public static class DrColorUtil
    {
        /// <summary>
        /// Converts RGB color to CMYK using simple formula.
        /// Returns components in order: Cyan, Magenta, Yellow, Black.
        /// </summary>
        public static byte[] ConvertToCmyk(byte redComponent, byte greenComponent, byte blueComponent)
        {
            byte[] cmykBytes = new byte[4];
            const float maxColorValue = 255f;

            float red = redComponent / maxColorValue;
            float green = greenComponent / maxColorValue;
            float blue = blueComponent / maxColorValue;

            float black = MathUtil.Min(1 - red, 1 - green, 1 - blue);
            float cyan, magenta, yellow;
            if (black == 1)
            {
                cyan = magenta = yellow = 0;
            }
            else
            {
                cyan = (1 - red - black) / (1 - black);
                magenta = (1 - green - black) / (1 - black);
                yellow = (1 - blue - black) / (1 - black);
            }

            cmykBytes[0] = (byte)(cyan * maxColorValue);
            cmykBytes[1] = (byte)(magenta * maxColorValue);
            cmykBytes[2] = (byte)(yellow * maxColorValue);
            cmykBytes[3] = (byte)(black * maxColorValue);

            return cmykBytes;
        }

        /// <summary>
        /// Returns palette index of specified for indexed image.
        /// </summary>
        public static int GetPaletteIndex(byte pixelByte, int colorIndex, int bitsPerComponent)
        {
            int startBitInByte = (colorIndex * bitsPerComponent) % 8;
            int mask = (1 << bitsPerComponent) - 1;
            int paletteIndex = (pixelByte & (mask << startBitInByte)) >> startBitInByte;

            return paletteIndex;
        }



        /// <summary>
        /// Checks whether the specified color is a shade of gray.
        /// </summary>
        public static bool IsShadeOfGray(int r, int g, int b)
        {
            return
                (Math.Abs(r - g) < ColorDifferenceThreshold) &&
                (Math.Abs(g - b) < ColorDifferenceThreshold);
        }

        internal const int ColorDifferenceThreshold = 10;
    }
}
