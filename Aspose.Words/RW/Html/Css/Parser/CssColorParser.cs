// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2021 by Victor Chebotok

using System.Drawing;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// An auxiliary class that parses CSS colors.
    /// </summary>
    internal class CssColorParser
    {
        /// <summary>
        /// Parses a CSS color keyword.
        /// </summary>
        /// <returns>
        /// The parsed color value or <c>null</c> if the string is not a valid color keyword.
        /// </returns>
        internal static DrColor ParseColorName(string name)
        {
            string lowerCaseName = StringUtil.AsciiLowerCase(name);
            DrColor color = DrKnownColors.FromName(lowerCaseName);
            if (color.IsEmpty)
            {
                color = ParseSystemColor(lowerCaseName);
            }

            // If the color name is known, return the corresponding color.
            return (!color.IsEmpty)
                ? color
                : null;
        }

        /// <summary>
        /// Parses a hex color value: 3 or 6 case insensitive hex digits without the leading hash character ('#').
        /// </summary>
        /// <remarks>
        /// https://www.w3.org/TR/css-color-3/#rgb-color
        /// </remarks>
        /// <returns>
        /// The parsed color value or <c>null</c> if the string is not a valid hex color value.
        /// </returns>
        internal static DrColor ParseHexColor(string value)
        {
            if ((value.Length != 3) && (value.Length != 6))
            {
                return null;
            }

            // Normally, each hex digit encodes a half-byte of the RGB value. In short hex color values, however, each hex digit
            // encodes one byte. #38B means the same as #3388BB.
            int digitShift = 4;
            int digitScale = 1;
            if (value.Length == 3)
            {
                digitShift *= 2;
                digitScale = 0x11;
            }

            // Decode the value digit by digit.
            int rgbValue = 0;
            foreach (char c in value)
            {
                int digit;
                if ((c >= '0') && (c <= '9'))
                {
                    digit = c - '0';
                }
                else if ((c >= 'A') && (c <= 'F'))
                {
                    digit = c - 'A' + 10;
                }
                else if ((c >= 'a') && (c <= 'f'))
                {
                    digit = c - 'a' + 10;
                }
                else
                {
                    // Error. Not a hex digit.
                    return null;
                }

                rgbValue <<= digitShift;
                rgbValue |= digit * digitScale;
            }

            // The parsed color is fully opaque. Its alpha value is 1.
            return new DrColor((255 << 24) + rgbValue);
        }

        /// <summary>
        /// Parses "rgb()" function colors.
        /// </summary>
        /// <remarks>
        /// See https://www.w3.org/TR/css-color-3/#rgb-color
        /// </remarks>
        /// <returns>
        /// Parsed color value or <c>null</c> in case of an error.
        /// </returns>
        internal static DrColor ParseRgbFunctionColor(CssValueList arguments)
        {
            // Three values delimeted by two commas.
            if (arguments.Count != 5)
            {
                return null;
            }
            if ((arguments[1].ValueType != CssValueType.Comma) || (arguments[3].ValueType != CssValueType.Comma))
            {
                return null;
            }

            int rgbValue = 0;

            // We remember the first value to make sure all RGB components are of the same type: either numbers or
            // percents.
            CssValueType firstValueType = arguments[0].ValueType;

            // Parse the argument list.
            for (int i = 0; i < 5; i += 2)
            {
                CssValue argument = arguments[i];

                // Each argument can be either a number or a percent but types of all arguments must be the same.
                if (((argument.ValueType != CssValueType.Number) && (argument.ValueType != CssValueType.Percentage)) ||
                    (argument.ValueType != firstValueType))
                {
                    // Parsing error. Unexpected value type.
                    return null;
                }

                double rawComponentValue = argument.DoubleValue;
                if (argument.ValueType == CssValueType.Percentage)
                {
                    // Value of 100% means 255 gradations.
                    rawComponentValue *= 2.55;
                }
                // Clips out-of-range values as browsers do.
                int componentValue = MathUtil.DoubleToInt(MathUtil.FitToRange(rawComponentValue, 0, 255));

                // Each component encodes a byte of the RGB value.
                rgbValue <<= 8;
                rgbValue |= componentValue;
            }

            // The parsed color is fully opaque. Its alpha value is 1.
            return new DrColor((255 << 24) + rgbValue);
        }

        /// <summary>
        /// Parses "hsl()" function colors.
        /// </summary>
        /// <remarks>
        /// See https://www.w3.org/TR/css-color-3/#hsl-color
        /// </remarks>
        /// <returns>
        /// Parsed color value or <c>null</c> in case of an error.
        /// </returns>
        internal static DrColor ParseHslFunctionColor(CssValueList arguments)
        {
            // Three values delimeted by two commas.
            // The first parameter (hue) is a unitless number.
            // Other parameters (saturation and luminance) are specified in percents.
            if (arguments.Count != 5)
            {
                return null;
            }
            if ((arguments[0].ValueType != CssValueType.Number) ||
                (arguments[1].ValueType != CssValueType.Comma) ||
                (arguments[2].ValueType != CssValueType.Percentage) ||
                (arguments[3].ValueType != CssValueType.Comma) ||
                (arguments[4].ValueType != CssValueType.Percentage))
            {
                return null;
            }
            return HslToRgb(arguments[0].DoubleValue, arguments[2].DoubleValue, arguments[4].DoubleValue);
        }

        /// <summary>
        /// Converts a color from the HSL model to RGB.
        /// </summary>
        private static DrColor HslToRgb(double hue, double saturation, double luminance)
        {
            // The algorithm was taken from the CSS specification:
            // https://www.w3.org/TR/css-color-3/#hsl-color

            // The algorithm normalizes the hue value to the [0; 1) range. However, the calculations get simpler
            // if hue is normalized to the [0; 6) range.
            double h = MathUtil.NormalizeAngle(hue) / 60;
            double s = MathUtil.FitToRange(saturation, 0, 100) / 100;
            double l = MathUtil.FitToRange(luminance, 0, 100) / 100;

            double m2 = (l <= 0.5)
                ? l * (s + 1)
                : l + s - l * s;
            double m1 = l * 2 - m2;

            int r = HueToRgbByte(m1, m2, h + 2);
            int g = HueToRgbByte(m1, m2, h);
            int b = HueToRgbByte(m1, m2, h - 2);

            return new DrColor(r, g, b);
        }

        private static int HueToRgbByte(double m1, double m2, double h)
        {
            // Calculate a HSL component value and then scale it to the [0; 255] range.
            double value = HueToRgb(m1, m2, h) * 255;
            value = MathUtil.FitToRange(value, 0, 255);
            return DoublePal.RoundToIntUp(value);
        }

        private static double HueToRgb(double m1, double m2, double h)
        {
            if (h < 0)
                h += 6;
            if (h > 6)
                h -= 6;

            if (h < 1)
                return m1 + (m2 - m1) * h;
            if (h < 3)
                return m2;
            if (h < 4)
                return m1 + (m2 - m1) * (4 - h);
            return m1;
        }

#if NETSTANDARD
        /// <summary>
        /// For netsstandard harcoded values are returned, since there are no SystemColors.
        /// </summary>
        private static DrColor ParseSystemColor(string value)
        {
            Color color;
            switch (StringUtil.AsciiLowerCase(value))
            {
                case "activeborder":
                    color = Color.FromArgb(-4934476);
                    break;
                case "activecaption":
                case "captiontext":
                    color = Color.FromArgb(-6703919);
                    break;
                case "appworkspace":
                    color = Color.FromArgb(-5526613);
                    break;
                case "background":
                    color = Color.FromArgb(-986896);
                    break;
                case "buttonface":
                case "threedface":
                    color = Color.FromArgb(-986896);
                    break;
                case "buttonhighlight":
                    color = Color.FromArgb(-1);
                    break;
                case "buttonshadow":
                case "threeddarkshadow":
                case "threedhighlight":
                case "threedlightshadow":
                case "threedshadow":
                    color = Color.FromArgb(-6250336);
                    break;
                case "buttontext":
                    color = Color.FromArgb(-986896);
                    break;
                case "graytext":
                    color = Color.FromArgb(-9605779);
                    break;
                case "highlight":
                    color = Color.FromArgb(-13395457);
                    break;
                case "highlighttext":
                    color = Color.FromArgb(-1);
                    break;
                case "inactiveborder":
                    color = Color.FromArgb(-722948);
                    break;
                case "inactivecaption":
                    color = Color.FromArgb(-4207141);
                    break;
                case "inactivecaptiontext":
                    color = Color.FromArgb(-16777216);
                    break;
                case "infobackground":
                    color = Color.FromArgb(-1842205);
                    break;
                case "infotext":
                    color = Color.FromArgb(-16777216);
                    break;
                case "menu":
                    color = Color.FromArgb(-986896);
                    break;
                case "menutext":
                    color = Color.FromArgb(-16777216);
                    break;
                case "scrollbar":
                    color = Color.FromArgb(-3618616);
                    break;
                case "window":
                    color = Color.FromArgb(-1);
                    break;
                case "windowframe":
                    color = Color.FromArgb(-10197916);
                    break;
                case "windowtext":
                    color = Color.FromArgb(-16777216);
                    break;
                default:
                    return DrColor.Empty;
            }

            return DrColor.FromNativeColor(color);
        }
#else
        /// <summary>
        /// Supports CSS2 system colors. http://www.w3.org/TR/css3-color/#css2-system
        /// Returns see <see cref="DrColor.Empty"/> if cannot parse as color.
        /// </summary>
        private static DrColor ParseSystemColor(string value)
        {
            Color color;
            switch (StringUtil.AsciiLowerCase(value))
            {
                case "activeborder":
                    color = SystemColors.ActiveBorder;
                    break;
                case "activecaption":
                case "captiontext":
                    color = SystemColors.ActiveCaption;
                    break;
                case "appworkspace":
                    color = SystemColors.AppWorkspace;
                    break;
                case "background":
                    color = SystemColors.Control;
                    break;
                case "buttonface":
                case "threedface":
                    color = SystemColors.ButtonFace;
                    break;
                case "buttonhighlight":
                    color = SystemColors.ButtonHighlight;
                    break;
                case "buttonshadow":
                case "threeddarkshadow":
                case "threedhighlight":
                case "threedlightshadow":
                case "threedshadow":
                    color = SystemColors.ButtonShadow;
                    break;
                case "buttontext":
                    color = SystemColors.ButtonFace;
                    break;
                case "graytext":
                    color = SystemColors.GrayText;
                    break;
                case "highlight":
                    color = SystemColors.Highlight;
                    break;
                case "highlighttext":
                    color = SystemColors.HighlightText;
                    break;
                case "inactiveborder":
                    color = SystemColors.InactiveBorder;
                    break;
                case "inactivecaption":
                    color = SystemColors.InactiveCaption;
                    break;
                case "inactivecaptiontext":
                    color = SystemColors.InactiveCaptionText;
                    break;
                case "infobackground":
                    color = SystemColors.ControlLight;
                    break;
                case "infotext":
                    color = SystemColors.InfoText;
                    break;
                case "menu":
                    color = SystemColors.Menu;
                    break;
                case "menutext":
                    color = SystemColors.MenuText;
                    break;
                case "scrollbar":
                    color = SystemColors.ScrollBar;
                    break;
                case "window":
                    color = SystemColors.Window;
                    break;
                case "windowframe":
                    color = SystemColors.WindowFrame;
                    break;
                case "windowtext":
                    color = SystemColors.WindowText;
                    break;
                default:
                    return DrColor.Empty;
            }

            return DrColor.FromNativeColor(color);
        }
#endif
    }
}
