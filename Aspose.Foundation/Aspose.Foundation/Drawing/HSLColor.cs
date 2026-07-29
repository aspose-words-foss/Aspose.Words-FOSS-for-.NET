// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/03/2010 by Roman Korchagin

using System;

namespace Aspose.Drawing
{
    /// <summary>
    /// HSL values are in the range of 0 to 1.
    /// Code taken from http://axonflux.com/handy-rgb-to-hsl-and-rgb-to-hsv-color-model-c
    /// </summary>
    public class HSLColor
    {
        public HSLColor(double hue, double sat, double lum)
        {
            Hue = hue;
            Sat = sat;
            Lum = lum;
        }

        /// <summary>
        /// Given an RGB color in range of 0-255 constructs H,S,L in range of 0-1
        /// </summary>
        public HSLColor(DrColor rgb)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;

            double max = System.Math.Max(System.Math.Max(r, g), b);
            double min = System.Math.Min(System.Math.Min(r, g), b);

            double mid = (max + min) / 2.0;
            mLum = mid;
            mHue = mid;
            mSat = mid;

            if (max == min)
            {
                mHue = 0; // achromatic
                mSat = 0;
            }
            else
            {
                double d = max - min;
                mSat = (mLum > 0.5) ? d / (2.0 - max - min) : d / (max + min);
                
                if (r == max)
                    mHue = (g - b)/d + (g < b ? 6 : 0);
                else if (g == max)
                    mHue = (b - r)/d + 2;
                else if (b == max)
                    mHue = (r - g)/d + 4;
            }
            mHue /= 6.0;
        }

        /// <summary>
        /// Converts RGB color to HSL color.
        /// </summary>
        /// <remarks>
        /// AM. I found this algorithm at http://support.microsoft.com/kb/29240 and it seems Office uses it. 
        /// It gives very close result with minor difference caused by fraction loss.
        /// For a while I don't want this algorithm to be common.
        /// </remarks>
        public static HSLColor OfficeFromDrColor(DrColor rgbColor)
        {
            int h, s, l;

            // intermediate value: % of spread from max
            int r = rgbColor.R;
            int g = rgbColor.G;
            int b = rgbColor.B;

            // calculate lightness
            // max and min RGB values
            int cMax = Math.Max(Math.Max(r, g), b);
            int cMin = Math.Min(Math.Min(r, g), b);

            l = ( ((cMax+cMin)*HslMax) + RgbMax )/(2*RgbMax);

            if (cMax == cMin) 
            {           
                // r=g=b --> achromatic case

                s = 0;                     // saturation
                h = UndefinedHue;          // hue
            }
            else 
            {   
                // chromatic case
                
                // saturation
                if (l <= (HslMax/2))
                    s = (((cMax-cMin)*HslMax) + ((cMax+cMin)/2)) / (cMax+cMin);
                else
                    s = (((cMax-cMin)*HslMax) + ((2*RgbMax-cMax-cMin)/2)) / (2*RgbMax-cMax-cMin);

                // hue
                int deltaR = (((cMax-r)*(HslMax/6)) + ((cMax-cMin)/2)) / (cMax-cMin);
                int deltaG = (((cMax-g)*(HslMax/6)) + ((cMax-cMin)/2)) / (cMax-cMin);
                int deltaB = (((cMax-b)*(HslMax/6)) + ((cMax-cMin)/2)) / (cMax-cMin);

                if (r == cMax)
                    h = deltaB - deltaG;
                else if (g == cMax)
                    h = (HslMax/3) + deltaR - deltaB;
                else // B == cMax
                    h = ((2*HslMax)/3) + deltaG - deltaR;

                if (h < 0)
                    h += HslMax;

                if (h > HslMax)
                    h -= HslMax;
            }

            return new HSLColor(h / (double)HslMax, s / (double)HslMax, l / (double)HslMax);
        }

        public DrColor ToDrColor()
        {
            return ToDrColor(Hue,Sat,Lum);
        }

        /// <summary>
        /// Creates <see cref="DrColor"/> based on specified hue, saturation and lightness.
        /// </summary>
        /// <param name="hue">The specified hue</param>
        /// <param name="sat">The specified saturation</param>
        /// <param name="lum">The specified lightness</param>
        /// <returns><see cref="DrColor"/></returns>
        public static DrColor ToDrColor(double hue, double sat, double lum)
        {
            double r;
            double g;
            double b;

            if (lum == 0)
            {
                r = 0;
                g = 0;
                b = 0;
            }
            else
            {
                if (sat == 0)
                {
                    r = lum;
                    g = lum;
                    b = lum;
                }
                else
                {
                    double q = (lum < 0.5) ? lum * (1.0 + sat) : lum + sat - lum * sat;
                    double p = 2.0 * lum - q;
                    r = HueToRgb(p, q, hue + 1 / 3.0);
                    g = HueToRgb(p, q, hue);
                    b = HueToRgb(p, q, hue - 1 / 3.0);
                }
            }

            return DrColor.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        /// <summary>
        /// Converts HSL color to RGB color.
        /// </summary>
        /// <remarks>
        /// AM. I found this algorithm at http://support.microsoft.com/kb/29240 and it seems Office uses it. 
        /// It gives very close result with minor difference caused by fraction loss.
        /// For a while I don't want this algorithm to be common.
        /// </remarks>
        public DrColor OfficeToDrColor()
        {
            int hue = (int)(Hue * HslMax);
            int lum = (int)(Lum * HslMax);
            int sat = (int)(Sat * HslMax);

            int r, g, b;

            if (sat == 0)
            {
                // achromatic case
                r = g = b = (lum * RgbMax) / HslMax;
                if (hue != UndefinedHue)
                {
                    // ERROR
                }
            }
            else
            {
                // chromatic case

                // set up magic numbers
                int magic2;
                if (lum <= (HslMax / 2))
                    magic2 = (lum * (HslMax + sat) + (HslMax / 2)) / HslMax;
                else
                    magic2 = lum + sat - ((lum * sat) + (HslMax / 2)) / HslMax;
                int magic1 = 2 * lum - magic2;

                // get RGB, change units from HLSMAX to RGBMAX
                r = (OfficeHueToRgb(magic1, magic2, hue + (HslMax / 3)) * RgbMax + (HslMax / 2)) / HslMax;
                g = (OfficeHueToRgb(magic1, magic2, hue) * RgbMax + (HslMax / 2)) / HslMax;
                b = (OfficeHueToRgb(magic1, magic2, hue - (HslMax / 3)) * RgbMax + (HslMax / 2)) / HslMax;
            }

            return DrColor.FromArgb(r, g, b);
        }

        private static int OfficeHueToRgb(int n1, int n2, int hue)
        {
            // Range check: note values passed add/subtract thirds of range
            if (hue < 0)
                hue += HslMax;

            if (hue > HslMax)
                hue -= HslMax;

            // Return r,g, or b value from this tridrant
            if (hue < (HslMax / 6))
                return (n1 + (((n2 - n1) * hue + (HslMax / 12)) / (HslMax / 6)));
            if (hue < (HslMax / 2))
                return (n2);
            if (hue < ((HslMax * 2) / 3))
                return (n1 + (((n2 - n1) * (((HslMax * 2) / 3) - hue) + (HslMax / 12)) / (HslMax / 6))
          );
            else
                return (n1);
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0.0)
                t += 1.0;

            if (t > 1.0)
                t -= 1.0;

            if (t < 1.0 / 6.0)
                return p + (q - p) * 6.0 * t;

            if (t < 1.0 / 2.0)
                return q;

            if (t < 2.0 / 3.0)
                return p + (q - p) * (2 / 3.0 - t) * 6.0;

            return p;
        }

        public double Hue
        {
            get { return mHue; }
            set { mHue = MathUtil.FitToRange(value, 0, 1.0); }
        }

        public double Sat
        {
            get { return mSat; }
            set { mSat = MathUtil.FitToRange(value, 0, 1.0); }
        }

        public double Lum
        {
            get { return mLum; }
            set { mLum = MathUtil.FitToRange(value, 0, 1.0); }
        }

        private double mHue;
        private double mSat;
        private double mLum;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int HslMax = 240;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int RgbMax = 255;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int UndefinedHue = (HslMax * 2 / 3);
    }
}
