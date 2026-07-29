// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2008 by Roman Korchagin

using Aspose.Drawing;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Helper methods to deal with colors in MS Word documents.
    /// </summary>
    internal static class WordColorUtil
    {
        internal static Word97Color ColorToWord97Color(DrColor color)
        {
            return WordXPColorToWord97Color(ColorToWordXPColor(color));
        }

        internal static DrColor Word97ColorToColor(Word97Color word97Color)
        {
            return WordXPColorToColor(Word97ColorToWordXPColor(word97Color));
        }

        /// <summary>
        /// Converts WordXP 32bit color into Word97 color.
        /// Returns closest WordColor for the specified System.Drawing.Color.
        /// Not very correct, might need to replace later.
        /// </summary>
        internal static Word97Color WordXPColorToWord97Color(int wordXPColor)
        {
            if (wordXPColor == WordXPColor.Auto)
            {
                //Special case for the Auto color.
                return Word97Color.Auto;
            }
            else
            {
                //Perform color matching.
                double bestDelta = double.MaxValue;
                Word97Color bestWord97Color = Word97Color.Black;
                for (int i = 0; i < gWord97ColorMap.Length; i++)
                {
                    double curDelta = ColorDiff(gWord97ColorMap[i], wordXPColor);
                    if (curDelta <= bestDelta)
                    {
                        bestWord97Color = (Word97Color)i;
                        bestDelta = curDelta;
                    }
                }
                return bestWord97Color;
            }
        }

        /// <summary>
        /// HSL based color difference. 
        /// </summary>
        /// <remarks>
        /// AM. After some experiments I found that HSL model gives more exactly color matching results. 
        /// </remarks>
        private static double ColorDiff(int colorA, int colorB)
        {
            int ar = (colorA & 0x000000FF);
            int ag = ((colorA & 0x0000FF00) >> 8);
            int ab = ((colorA & 0x00FF0000) >> 16);

            int br = (colorB & 0x000000FF);
            int bg = ((colorB & 0x0000FF00) >> 8);
            int bb = ((colorB & 0x00FF0000) >> 16);

            HSLColor hsla = new HSLColor(DrColor.FromArgb(ar, ag, ab));
            HSLColor hslb = new HSLColor(DrColor.FromArgb(br, bg, bb));

            double diff = System.Math.Abs(hsla.Sat - hslb.Sat) + System.Math.Abs(hsla.Lum - hslb.Lum);

            // Hue is undefined for achromatic color so skip component from calculation.
            if (!(IsAchromatic(ar, ag, ab) || IsAchromatic(br, bg, bb)))
                diff += System.Math.Abs(hsla.Hue - hslb.Hue);

            return diff;
        }

        private static bool IsAchromatic(int r, int g, int b)
        {
            return (r == g) && (g == b);
        }

        /// <summary>
        /// Converts Word97 color into WordXP 32bit color.
        /// </summary>
        internal static int Word97ColorToWordXPColor(Word97Color word97Color)
        {
            //Sometimes in documents I encounter colors which are out of Word97Color range,
            //I convert these to Automatic.
            if ((int)word97Color < gWord97ColorMap.Length)
                return gWord97ColorMap[(int)word97Color];
            else
                return WordXPColor.Auto;
        }

        /// <summary>
        /// Converts .NET Color into Word XP 32bit color (into Word raw color).
        /// </summary>
        internal static int ColorToWordXPColor(DrColor color)
        {
            int result = 0;
            result |= color.R;
            result |= (color.G << 8);
            result |= (color.B << 16);
            //Convert alpha into transparency.
            result |= (~color.A << 24);
            return result;
        }

        /// <summary>
        /// Converts .NET Color into COLORREF structure.
        /// </summary>
        /// <remarks>
        /// See [MS-DOC] 2.9.43 COLORREF.
        /// </remarks>
        internal static uint ColorToColorRef(DrColor color)
        {
            if (color.IsEmpty)
                return 0xff000000;

            // WORDSNET-13087 fAuto MUST be 0 at least for shading. Otherwise Word shows error message and unable to load complete document.
            return (uint)(color.R | (color.G << 8) | (color.B << 16));
        }

        /// <summary>
        /// Converts Word 32bit color (Word raw color) into .NET Color.
        /// I call Word 32bit color "raw color". Word32 bit color layout is different from .Net color.
        /// It has R, G, B and T. R is the low bits, T is the high bits. 
        /// T is transparency.
        /// </summary>
        internal static DrColor WordXPColorToColor(int officeXPColor)
        {
            int r = (officeXPColor & 0x000000FF);
            int g = ((officeXPColor >> 8) & 0x000000FF);
            int b = ((officeXPColor >> 16) & 0x000000FF);
            int t = ((officeXPColor >> 24) & 0x000000FF);
            //It looks like in Word file it is transparency, 
            //but in windows it is alpha, so I need to invert it.
            int a = ~t & 0x000000FF;
            return new DrColor(a, r, g, b);
        }

        /// <summary>
        /// Returns the extended color modifier from MSOTINTSHADE OfficeArt record.
        /// </summary>
        /// <dev>
        /// SPEC: [MS-ODRAW].
        /// 2.2.3 MSOSHADE record.
        /// (16 bits): 0x01F4 MSOSHADE flag.
        /// (8 bits):  0xXX shade value.
        /// (8 bits):  0x10 MSOTINTSHADE flag.
        /// 2.2.4 MSOTINT record.
        /// (16 bits): 0x02F4 MSOTINT flag.
        /// (8 bits):  0xXX tint value.
        /// (8 bits):  0x10 MSOTINTSHADE flag.
        /// </dev>
        internal static int MsoRecordToExtColorModifier(int value)
        {
            // Checks MSOTINTSHADE flags.
            if (((value >> 24) == 0x10) && ((byte)value == 0xf4))
                // Returns the modifier as a positive value if MSOTINT flag is set, and as a negative one otherwise.
                return (ushort)value == 0x02f4
                    ? (value >> 16 & 0xff)
                    : -(value >> 16 & 0xff);

            return 0;
        }

        /// <summary>
        /// Convert extended color modifiers to MSOTINTSHADE OfficeArt record.
        /// </summary>
        /// <dev>
        /// SPEC: Similar with <see cref="MsoRecordToExtColorModifier"/>.
        /// </dev>
        internal static int ExtColorModifierToMsoRecord(int modifier)
        {
            // Checks whether the value is within the allowed tint range.
            if ((modifier >= 0) && (modifier < 256))
                // Returns the tint modifier value with the required MSOTINT and MSOTINTSHADE flags.
                return ((modifier | 0x1000) << 16) | 0x02f4;

            // Checks whether the value is within the allowed shade range.
            if ((modifier < 0) && (modifier > -256))
                // Returns the shade modifier value with the required MSOSHADE and MSOTINTSHADE flags.
                return ((-modifier | 0x1000) << 16) | 0x01f4;

            return 0;
        }

        static WordColorUtil()
        {
            gWord97ColorMap = new int[] 
            {
                WordXPColor.Auto,
                WordXPColor.Black,
                WordXPColor.Blue,
                WordXPColor.Cyan,
                WordXPColor.Green,
                WordXPColor.Magenta,
                WordXPColor.Red,
                WordXPColor.Yellow,
                WordXPColor.White,
                WordXPColor.DarkBlue,
                WordXPColor.DarkCyan,
                WordXPColor.DarkGreen,
                WordXPColor.DarkMagenta,
                WordXPColor.DarkRed,
                WordXPColor.DarkYellow,
                WordXPColor.DarkGray,
                WordXPColor.LightGray
            };
        }

        private static readonly int[] gWord97ColorMap;
    }
}
