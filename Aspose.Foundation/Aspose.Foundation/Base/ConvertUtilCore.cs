// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2005 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.Drawing;

namespace Aspose
{
    /// <summary>
    /// Provides helper functions to convert between various measurement units.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class ConvertUtilCore
    {
        /// <summary>
        /// Converts points to pixels at 96 dpi.
        /// </summary>
        /// <param name="points">The value to convert.</param>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static double PointToPixel(double points)
        {
            return PointToPixel(points, ImageConstants.StandardResolution);
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution.
        /// </summary>
        /// <param name="points">The value to convert.</param>
        /// <param name="resolution">The dpi (dots per inch) resolution.</param>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static double PointToPixel(double points, double resolution)
        {
            return (points / PointsPerInch) * resolution;
        }

        /// <summary>
        /// Converts points to pixels at 96 dpi and rounds to int.
        /// </summary>
        public static int PointToPixelInt(double points)
        {
            return MathUtil.DoubleToInt(PointToPixel(points));
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and rounds to int.
        /// </summary>
        public static int PointToPixelInt(double points, double resolution)
        {
            return MathUtil.DoubleToInt(PointToPixel(points, resolution));
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and casts the result to int.
        /// </summary>
        public static int PointToPixelIntTruncate(double points, double resolution)
        {
            return MathUtil.Truncate(PointToPixel(points, resolution));
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and rounds to int.
        /// Makes the value not less than 1.
        /// </summary>
        public static int PointToPixelIntCeiling(double points, double resolution)
        {
            return Math.Max((int)Math.Ceiling(PointToPixel(points, resolution)), 1);
        }

        /// <summary>
        /// Converts points to pixels at the standard pixel resolution and rounds to int.
        /// Makes the value not less than 1.
        /// </summary>
        public static int PointToPixelIntCeiling(double points)
        {
            return PointToPixelIntCeiling(points, ImageConstants.StandardResolution);
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and rounds to int.
        /// Makes the value not less than 1.
        /// </summary>
        public static Size PointToPixelInt(SizeF sizePoints, float scale, double horizontalDpi, double verticalDpi)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale");
            if (horizontalDpi <= 0)
                throw new ArgumentOutOfRangeException("dpi");
            if (verticalDpi <= 0)
                throw new ArgumentOutOfRangeException("dpi");

            return new Size(
                Math.Max(PointToPixelInt(sizePoints.Width * scale, horizontalDpi), 1),
                Math.Max(PointToPixelInt(sizePoints.Height * scale, verticalDpi), 1));
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and rounds up to int.
        /// Makes the value not less than 1.
        /// </summary>
        public static Size PointToPixelIntCeiling(SizeF sizePoints, float scale, double horizontalDpi, double verticalDpi)
        {
            return new Size(PointToPixelIntCeiling(sizePoints.Width, scale, horizontalDpi),
                            PointToPixelIntCeiling(sizePoints.Height, scale, verticalDpi));
        }

        /// <summary>
        /// Converts rectangle in points to rectangle in pixels at the specified pixel resolution and rounds up to int.
        /// Makes the values not less than 0.
        /// </summary>
        public static Rectangle PointToPixelIntCeiling(RectangleF rectanglePoints, float scale, double horizontalDpi, double verticalDpi)
        {
            return Rectangle.FromLTRB(
                PointToPixelIntCeiling(rectanglePoints.Left, scale, horizontalDpi, 0),
                PointToPixelIntCeiling(rectanglePoints.Top, scale, verticalDpi, 0),
                PointToPixelIntCeiling(rectanglePoints.Right, scale, horizontalDpi, 0),
                PointToPixelIntCeiling(rectanglePoints.Bottom, scale, verticalDpi, 0));
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and rounds up to int.
        /// Makes the value not less than 1.
        /// </summary>
        public static int PointToPixelIntCeiling(float points, float scale, double dpi)
        {
            return PointToPixelIntCeiling(points, scale, dpi, 1);
        }

        /// <summary>
        /// Converts points to pixels at the specified pixel resolution and rounds up to int.
        /// Makes the value not less than parameter minValue.
        /// </summary>
        public static int PointToPixelIntCeiling(float points, float scale, double dpi, int minValue)
        {
            if (scale <= 0)
                throw new ArgumentOutOfRangeException("scale");
            if (dpi <= 0)
                throw new ArgumentOutOfRangeException("dpi");

            return Math.Max((int)Math.Ceiling(PointToPixel(points * scale, dpi)), minValue);
        }

        /// <summary>
        /// Converts points to microns (1 thousandth of mm).
        /// </summary>
        public static double PointToMicron(double points)
        {
            return points / PointsPerThousandthMm;
        }

        /// <summary>
        /// Converts points to microns and rounds to int (1 thousandth of mm).
        /// </summary>
        public static int PointToMicronInt(double points)
        {
            return MathUtil.DoubleToInt(PointToMicron(points));
        }

        /// <summary>
        /// Converts points to HIMETRIC (1 hundreds of mm).
        /// </summary>
        public static double PointToHimetric(double points)
        {
            return points / PointsPerHundredthMm;
        }

        /// <summary>
        /// Converts points to HIMETRIC (1 hundreds of mm) and rounds to int.
        /// </summary>
        public static int PointToHimetricInt(double points)
        {
            return MathUtil.DoubleToInt(PointToHimetric(points));
        }

        /// <summary>
        /// Converts HIMETRIC (1 hundreds of mm) to points.
        /// </summary>
        public static double HimetricToPoint(double himetrics)
        {
            return himetrics * PointsPerHundredthMm;
        }

        /// <overloads>Converts pixels to points.</overloads>
        /// <summary>
        /// Converts pixels to points at 96 dpi.
        /// </summary>
        /// <param name="pixels">The value to convert.</param>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static double PixelToPoint(double pixels)
        {
            return PixelToPoint(pixels, ImageConstants.StandardResolution);
        }

        /// <summary>
        /// Converts pixels to points at 96 dpi.
        /// </summary>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static RectangleF PixelToPoint(RectangleF rectInPixel)
        {
            return RectangleF.FromLTRB((float)PixelToPoint(rectInPixel.Left), (float)PixelToPoint(rectInPixel.Top),
                                       (float)PixelToPoint(rectInPixel.Right), (float)PixelToPoint(rectInPixel.Bottom));
        }

        /// <summary>
        /// Converts pixels to points at the specified pixel resolution.
        /// </summary>
        /// <param name="pixels">The value to convert.</param>
        /// <param name="resolution">The dpi (dots per inch) resolution.</param>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static double PixelToPoint(double pixels, double resolution)
        {
            return ((pixels / resolution) * PointsPerInch);
        }

        public static int PixelToTwip(double pixels, double resolution)
        {
            return MathUtil.DoubleToInt((pixels / resolution) * TwipsPerInch);
        }

        public static int PixelToLi(double pixels, double resolution)
        {
            return MathUtil.DoubleToInt((pixels / resolution) * LisPerInch);
        }

        /// <summary>
        /// Converts pixels from one resolution to another.
        /// </summary>
        /// <param name="pixels">The value to convert.</param>
        /// <param name="oldDpi">The current dpi (dots per inch) resolution.</param>
        /// <param name="newDpi">The new dpi (dots per inch) resolution.</param>
        public static int PixelToNewDpi(double pixels, double oldDpi, double newDpi)
        {
            return MathUtil.DoubleToInt(pixels * newDpi / oldDpi);
        }

        /// <summary>
        /// Converts inches to points.
        /// </summary>
        /// <param name="inches">The value to convert.</param>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static double InchToPoint(double inches)
        {
            return inches * PointsPerInch;
        }

        /// <summary>
        /// Converts points to inches.
        /// </summary>
        /// <param name="points">The value to convert.</param>
        /// <remarks>
        /// 1 inch equals 72 points.
        /// </remarks>
        public static double PointToInch(double points)
        {
            return points / PointsPerInch;
        }

        /// <summary>
        /// Converts millimeters to points.
        /// </summary>
        /// <param name="mm">The value to convert.</param>
        /// <remarks>
        /// 1 inch equals 25.4 millimeters. 1 inch equals 72 points.
        /// </remarks>
        public static double MmToPoint(double mm)
        {
            return mm * PointsPerMm;
        }

        public static int MmToTwip(double mm)
        {
            return MathUtil.DoubleToInt(mm * TwipsPerMm);
        }

        public static Size MmToTwip(double x, double y)
        {
            return new Size(MmToTwip(x), MmToTwip(y));
        }

        /// <summary>
        /// Converts mm to pixels at 96 dpi and rounds to int.
        /// </summary>
        public static int MmToPixelInt(double mm)
        {
            return MathUtil.DoubleToInt(MmToPixel(mm));
        }

        public static double MmToPixel(double mm)
        {
            return MmToPixel(mm, ImageConstants.StandardResolution);
        }

        public static double MmToPixel(double mm, double resolution)
        {
            return mm * resolution / MmPerInch;
        }

        public static double InchToPixel(double inches)
        {
            return InchToPixel(inches, ImageConstants.StandardResolution);
        }

        public static double InchToPixel(double inches, double resolution)
        {
            return inches * resolution;
        }

        /// <summary>
        /// Converts centimeters to points.
        /// </summary>
        public static double CmToPoint(double cm)
        {
            return cm * PointsPerCm;
        }

        /// <summary>
        /// Converts centimetres to twips.
        /// </summary>
        public static int CmToTwip(double cm)
        {
            return MathUtil.DoubleToInt(cm * TwipsPerCm);
        }

        public static int CmToEmu(double cmValue)
        {
            return TwipToEmu(CmToTwip(cmValue));
        }

        public static double CmToInch(double cm)
        {
            return cm / CmPerInch;
        }

        /// <summary>
        /// 1 line is 12 points.
        /// </summary>
        public static double LineToPoint(double lines)
        {
            return lines * PointsPerLine;
        }

        /// <summary>
        /// 1 line is 12 points.
        /// </summary>
        public static int LineHundredthToTwip(int lineHundredths)
        {
            return MathUtil.DoubleToInt(lineHundredths * TwipsPerLine / 100.0);
        }

        public static double PointToLine(double points)
        {
            return points / PointsPerLine;
        }

        public static int PointToHalfPoint(double points)
        {
            // This function is used to convert points to half points for storing font's size.
            // Previously we used DoubleToInt function here (with rounding to nearest even int) but
            // in order to copy MS Word behavior we should use DoublePal.RoundToIntUp() function instead.
            // This was requested in WORDSNET-8388 And as no existing tests failed i decided to change
            // this function's behavior and not to create one with similar behavior but with rounding
            // to nearest int as i have done for DoubleToInt.
            return DoublePal.RoundToIntUp(points * 2.0);
        }

        public static double HalfPointToPoint(int halfPoints)
        {
            return halfPoints / 2.0;
        }

        public static double HalfPointToTwip(int halfPoints)
        {
            return halfPoints * TwipsPerHalfPoint;
        }

        public static int PointToEightsPoint(double points)
        {
            return MathUtil.DoubleToInt(points * 8.0);
        }

        public static double EightsPointToPoint(int eightsPoints)
        {
            return eightsPoints / 8.0;
        }

        public static int EightsPointToTwip(int eightsPoint)
        {
            return MathUtil.DoubleToInt(eightsPoint * 2.5);
        }

        public static int PointToTwip(double points)
        {
            return MathUtil.DoubleToInt(points * TwipsPerPoint);
        }

        public static double TwipToPoint(int twips)
        {
            return twips / TwipsPerPoint;
        }

        public static double TwipToInch(int twips)
        {
            return twips / TwipsPerInch;
        }

        public static double TwipToPoint(double twips)
        {
            return twips / TwipsPerPoint;
        }

        public static double TwipToPicas(double twips)
        {
            return twips / TwipsPerPicas;
        }

        public static double TwipToMm(int twips)
        {
            return twips / TwipsPerMm;
        }

        public static double TwipToCm(int twips)
        {
            return twips / TwipsPerCm;
        }

        /// <summary>
        /// 1 line is 12 points.
        /// </summary>
        public static double TwipToLine(int twips)
        {
            return twips / 240.0;
        }

        /// <summary>
        /// Converts twips to pixels at 96 dpi.
        /// </summary>
        public static int TwipToPixel(int twips)
        {
            return PointToPixelInt(TwipToPoint(twips));
        }

        public static int InchToTwip(double inches)
        {
            return MathUtil.DoubleToInt(inches * TwipsPerInch);
        }

        public static Size InchToTwip(double x, double y)
        {
            return new Size(InchToTwip(x), InchToTwip(y));
        }

        public static int EmuToTwip(int emus)
        {
            return MathUtil.DoubleToInt(emus / EmusPerTwip);
        }

        public static int TwipToEmu(int twips)
        {
            return MathUtil.DoubleToInt(twips * EmusPerTwip);
        }

        /// <summary>
        /// Additional overloaded method is needed in Java because <see cref="T:SizeF PointToEmu(SizeF sizeInPoints)"/>
        /// is converted to /*SizeF*/long pointToEmu(/*SizeF*/long sizeInPoints) and this method is invoked instead of
        /// <see cref="T:int PointToEmu(double points)"/> in Java.
        /// </summary>
        public static int PointToEmu(int points)
        {
            return MathUtil.DoubleToInt(points * EmusPerPoint);
        }

        public static int PointToEmu(double points)
        {
            return MathUtil.DoubleToInt(points * EmusPerPoint);
        }

        public static RectangleF PointToEmu(RectangleF rectInPoints)
        {
            return RectangleF.FromLTRB(PointToEmu(rectInPoints.Left), PointToEmu(rectInPoints.Top),
                PointToEmu(rectInPoints.Right), PointToEmu(rectInPoints.Bottom));
        }

        public static SizeF PointToEmu(SizeF sizeInPoints)
        {
            return new SizeF(PointToEmu(sizeInPoints.Width), PointToEmu(sizeInPoints.Height));
        }

        public static double EmuToPoint(int emus)
        {
            return emus / EmusPerPoint;
        }

        public static double EmuToPoint(double emus)
        {
            return emus / EmusPerPoint;
        }

        public static RectangleF EmuToPoint(RectangleF rectInEmu)
        {
            return new RectangleF(
                (float)EmuToPoint(rectInEmu.Left),
                (float)EmuToPoint(rectInEmu.Top),
                (float)EmuToPoint(rectInEmu.Width),
                (float)EmuToPoint(rectInEmu.Height));
        }

        public static double PixelToEmu(int pixels)
        {
            return PointToEmu(PixelToPoint(pixels));
        }

        public static double PixelToEmu(int pixels, double resolution)
        {
            return PointToEmu(PixelToPoint(pixels, resolution));
        }
        public static RectangleF PixelToEmu(RectangleF rectInPixels, double resolution)
        {
            return RectangleF.FromLTRB(
                (float)PixelToEmu((int)rectInPixels.Left, resolution),
                (float)PixelToEmu((int)rectInPixels.Top, resolution),
                (float)PixelToEmu((int)rectInPixels.Right, resolution),
                (float)PixelToEmu((int)rectInPixels.Bottom, resolution));
        }

        public static RectangleF PixelToEmu(RectangleF rectInPixels)
        {
            return new RectangleF(
                (float)PixelToEmu((int)rectInPixels.Left),
                (float)PixelToEmu((int)rectInPixels.Top),
                (float)PixelToEmu((int)rectInPixels.Width),
                (float)PixelToEmu((int)rectInPixels.Height));
        }

        public static double EmuToInch(int emus)
        {
            return emus / EmusPerInch;
        }

        public static float EmuToPixel(int emu)
        {
            return (float)PointToPixel(emu / EmusPerPoint);
        }

        public static RectangleF EmuToPixel(RectangleF rectInEmu)
        {
            return RectangleF.FromLTRB(EmuToPixel((int)rectInEmu.Left), EmuToPixel((int)rectInEmu.Top),
                EmuToPixel((int)rectInEmu.Right), EmuToPixel((int)rectInEmu.Bottom));
        }

        public static float EmuToMillimeter(float emus)
        {
            return emus / (float)EmusPerMm;
        }

        public static double EmuToMillimeter(int emus)
        {
            return emus / EmusPerMm;
        }

        public static int MmToEmu(double mm)
        {
            return MathUtil.DoubleToInt(mm * EmusPerMm);
        }

        public static RectangleF MmToEmu(RectangleF mm)
        {
            return RectangleF.FromLTRB(MmToEmu(mm.Left), MmToEmu(mm.Top), MmToEmu(mm.Right), MmToEmu(mm.Bottom));
        }

        public static int LiToPixel(double lis, double resolution)
        {
            // Casting to float in the code below may lead to inexact calculation for small li values.
            // This inexactness allows to get results close to Word's while laying out documents with
            // the printer metrics compatibility option. Use this method with caution for
            // other than printer metrics-related calculations.
            return MathUtil.DoubleToInt((float)(lis / LisPerPoint) / PointsPerInch * resolution);
        }

        /// <summary>
        /// Converts lis to pixels at 96 dpi.
        /// </summary>
        public static int LiToPixel(int width)
        {
            return LiToPixel(width, 96);
        }

        /// <summary>
        /// <para>Converts points to layout integers (custom scale value).</para>
        /// <para>
        /// This method accepts a double because it is mostly used for conversion
        /// from model public values into layout and in the model such values are doubles.
        /// </para>
        /// </summary>
        public static int PointToLi(double points)
        {
            return MathUtil.DoubleToInt(points * LisPerPoint);
        }

        /// <summary>
        /// The same as above. Needed only to autoport .Net strong autoboxing to java.
        /// </summary>
        public static int PointToLi(int points)
        {
            return MathUtil.DoubleToInt(points * LisPerPoint);
        }

        /// <summary>
        /// Converts layout integers (custom scale value) to points.
        ///
        /// The result is float because the result is used for the APS model where values are floats.
        /// </summary>
        public static float LiToPoint(int lis)
        {
            return (float)lis / LisPerPoint;
        }

        /// <summary>
        /// Converts twips to layout integers (custom scale value).
        /// </summary>
        public static int TwipToLi(int twips)
        {
            return twips * LisPerTwip;
        }

        /// <summary>
        /// Converts layout integers (custom scale value) to twips.
        /// </summary>
        public static int LiToTwip(int lis)
        {
            return lis / LisPerTwip;
        }

        /// <summary>
        /// Converts list to twips, rounding the result up.
        /// </summary>
        /// <remarks>
        /// So it answers the question "how many twips do you need to accommodate the given number of lis".
        /// It is used for table cell contents width calculation, because MS Word works with cell widths in twips.
        /// </remarks>
        public static int LisToTwipRoundingUp(int lis)
        {
            int twips = lis / LisPerTwip;
            int remainder = lis % LisPerTwip;
            int rounding = (remainder == 0) ? 0 : 1;
            return twips + rounding;
        }

        /// <summary>
        /// Converts half points to layout integers (custom scale value).
        /// </summary>
        public static int HalfPointToLi(double halfPoints)
        {
            return MathUtil.DoubleToInt(halfPoints * LisPerHalfPoint);
        }

        /// <summary>
        /// Converts layout integers (custom scale value) to half points.
        /// </summary>
        public static float LiToHalfPoint(int lis)
        {
            return (float)lis / LisPerHalfPoint;
        }

        public static RectangleF LiToPoint(Rectangle li)
        {
            return new RectangleF(LiToPoint(li.X), LiToPoint(li.Y), LiToPoint(li.Width), LiToPoint(li.Height));
        }

        public static PointF LiToPoint(Point li)
        {
            return new PointF(LiToPoint(li.X), LiToPoint(li.Y));
        }

        public static SizeF LiToPointFromSize(Size li)
        {
            return new SizeF(LiToPoint(li.Width), LiToPoint(li.Height));
        }

        public static Rectangle PointToLi(RectangleF pt)
        {
            return new Rectangle(PointToLi(pt.X),
            PointToLi(pt.Y), PointToLi(pt.Width),
            PointToLi(pt.Height));
        }

        /// <summary>
        /// </summary>
        public static Point PointToLi(PointF pt)
        {
            return new Point(PointToLi(pt.X), PointToLi(pt.Y));
        }

        /// <summary>
        /// Converts layout integers (custom scale value) to lines,
        /// each line is 12 points.
        /// </summary>
        public static float LiToLine(int lis)
        {
            return (float)((float)lis / LisPerPoint) / 12.0f; // Casting for Java.
        }

        /// <summary>
        /// Converts 16.16 fixed point into floating point number.
        /// </summary>
        public static double FixedToDouble(int value)
        {
            return value / 65536.0;
        }

        /// <summary>
        /// Converts a floating point number into 16.16 fixed point number.
        /// </summary>
        public static int DoubleToFixed(double value)
        {
            // TODO 1 Maybe I should throw instead?
            if (value > MaxFixedValue)
                value = MaxFixedValue;
            else if (value < MinFixedValue)
                value = MinFixedValue;

            return MathUtil.DoubleToInt(value * 65536.0);
        }

        public static double DmlAnglesToRadians(double dmlAngle)
        {
            return MathUtil.DegreesToRadians(dmlAngle / DmlAnglesPerDegree);
        }

        public static double RadiansToDmlAngles(double value)
        {
            return MathUtil.RadiansToDegrees(value) * DmlAnglesPerDegree;
        }

        public static double DegreesToDmlAngles(double value)
        {
            return value * DmlAnglesPerDegree;
        }

        public static double DmlAnglesToDegrees(double value)
        {
            return value / DmlAnglesPerDegree;
        }

        /// <summary>
        /// Converts lis to pixels, rounds to int and converts back to lis.
        /// </summary>
        public static int LiToLiRoundedByPixels(int value, float resolution)
        {
            int pixels = LiToPixel(value, resolution);
            return PixelToLi(pixels, resolution);
        }

        /// <summary>
        /// Converts inches to centimeters.
        /// </summary>
        /// <param name="inches">The value to convert.</param>
        /// <remarks>
        /// 1 inch equals 2.54 centimeters.
        /// </remarks>
        public static double InchToCm(double inches)
        {
            return inches * CmPerInch;
        }

        public static double PixelToMm(double pixels, double resolution)
        {
            return PixelToPoint(pixels, resolution) / PointsPerMm;
        }

        public static double PixelToMicroMeter(double pixels, double resolution)
        {
            return PixelToPoint(pixels, resolution) / PointsPerThousandthMm;
        }

        public static double PointToMm(double points)
        {
            return points / PointsPerMm;
        }

        public static double PointToCm(double points)
        {
            return (points * CmPerInch) / PointsPerInch;
        }

        /// <summary>
        /// Converts the specified value to twips, gets the smallest integral value that is greater than or equal
        /// to the converted value, and converts it to emus.
        /// </summary>
        /// <param name="emus">The specified values</param>
        /// <returns>The calculated value</returns>
        public static double CeilingTwipsToEmus(double emus)
        {
            return Math.Ceiling(emus / EmusPerTwip) * EmusPerTwip;
        }

        /// <summary>
        /// http://trac.bookofhook.com/bookofhook/trac.cgi/wiki/IntroductionToFixedPointMath
        /// </summary>
        public const double MaxFixedValue = 32767.0 + 65535.0 / 65536.0;  // 0x7fffffff
        public const double MinFixedValue = -32768.0 - 65535.0 / 65536.0; // 0x8000ffff

        public const double MmPerInch = 25.4;
        public const double CmPerInch = 2.54;

        public const double PointsPerInch = 72.0;
        public const float PointsPerHundredthInch = (float)PointsPerInch * 0.01f;
        public const float PointsPerThousandthInch = (float)PointsPerInch * 0.001f;

        public const float PixelsPerPointStandardResolution = (float)(ImageConstants.StandardResolution / PointsPerInch);
        public const float PointsPerPixelStandardResolution = (float)(1.0 / PixelsPerPointStandardResolution);

        public const double PointsPerMm = PointsPerInch / MmPerInch;
        public const float PointsPerTenthMm = (float)PointsPerMm * 0.1f;
        public const float PointsPerHundredthMm = (float)PointsPerMm * 0.01f;
        public const float PointsPerThousandthMm = (float)PointsPerMm * 0.001f;
        public const double PointsPerCm = PointsPerMm * 10;

        private const double PointsPerLine = 12.0;

        public const double TwipsPerPoint = 20.0;
        public const double TwipsPerHalfPoint = TwipsPerPoint / 2.0;
        public const double TwipsPerInch = TwipsPerPoint * PointsPerInch;
        public const double TwipsPerMm = TwipsPerPoint * PointsPerMm;
        public const double TwipsPerCm = TwipsPerMm * 10;
        public const double TwipsPerPicas = TwipsPerPoint * 12;
        private const double TwipsPerLine = TwipsPerPoint * PointsPerLine;

        public const double PointsPerTwip = 1.0 / TwipsPerPoint;

        public const double EmusPerPoint = 12700.0;
        private const double EmusPerInch = EmusPerPoint * PointsPerInch;
        private const double EmusPerMm = EmusPerPoint * PointsPerMm;
        public const double EmusPerTwip = EmusPerPoint * PointsPerTwip;

        public const int LisPerPoint = 1000;
        private const int LisPerInch = (int)(LisPerPoint * PointsPerInch);
        public const int LisPerTwip = (int)(LisPerPoint / TwipsPerPoint);
        private const int LisPerHalfPoint = LisPerPoint / 2;

        /// <summary>
        /// WordLi is a measure similar to AW's Li.
        /// </summary>
        /// <remarks>
        /// Eventually, Lis should be replaced by WordLis to achieve higher fidelity and fix rounding issues, such as WORDSNET-19657
        /// </remarks>
        public const int WordLisPerPoint = 0x1000;

        /// <summary>
        /// This is widely used in Word as maximum size in points for an object.
        /// </summary>
        public const double MaxSizePoint = 1584;

        /// <summary>
        /// This is widely used in Word as maximum size in emus for an object.
        /// </summary>
        public const double MaxSizeEmus = 1584 * EmusPerPoint;

        /// <summary>
        /// This is widely used in Word as minimum size in points for an object.
        /// </summary>
        public const double MinSizePoint = 0.75d;

        public const int MaxSizeTwip = (int)(MaxSizePoint * TwipsPerPoint);

        public const int MinSizeTwip = (int)(MinSizePoint * TwipsPerPoint);

        public const int MaxSizeLis = 1584 * LisPerPoint;

        public const double DmlAnglesPerDegree = 60000.0;
    }
}
