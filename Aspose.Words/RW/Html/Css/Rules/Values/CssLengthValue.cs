// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an identifier CSS length value.
    /// </summary>
    internal class CssLengthValue : CssValue
    {
        internal CssLengthValue(double value, CssUnit unit)
            : base(CssValueType.Length, value)
        {
            mValue = value;
            Unit = unit;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ (int)Unit;
                return result;
            }
        }

        /// <summary>
        /// Gets a length value converted to the specified units. Otherwise returns zero.
        /// If we attempt to get a length value as number, we treat it as pixels.
        /// Note: relative values aren't supported.
        /// </summary>
        internal double GetLength(CssUnit unit)
        {
            double lenght = 0;
            switch (unit)
            {
                case CssUnit.None:
                case CssUnit.In:
                case CssUnit.Cm:
                case CssUnit.Mm:
                case CssUnit.Pt:
                case CssUnit.Pc:
                case CssUnit.Px:
                    lenght = PointsToLength(unit);
                    break;
                case CssUnit.Em:
                case CssUnit.Ex:
                case CssUnit.Rem:
                    Debug.Assert(false, "Relative length is not allowed.");
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return lenght;
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append(FormatterPal.DoubleToStr2Decimals(mValue));
            sb.Append(CssUnitToString(Unit));
        }

        protected override bool DoEquals(CssValue other)
        {
            if (!(other is CssLengthValue))
                return false;

            const double tolerance = 0.005; // This accuracy is enough in HTML.
            CssLengthValue length = (CssLengthValue)other;
            return MathUtil.AreEqual(mValue, length.mValue, tolerance) && (Unit == length.Unit);
        }

        private static string CssUnitToString(CssUnit unit)
        {
            switch (unit)
            {
                case CssUnit.In:
                    return "in";
                case CssUnit.Cm:
                    return "cm";
                case CssUnit.Mm:
                    return "mm";
                case CssUnit.Pt:
                    return "pt";
                case CssUnit.Pc:
                    return "pc";
                case CssUnit.Px:
                    return "px";
                case CssUnit.Em:
                    return "em";
                case CssUnit.Ex:
                    return "ex";
                case CssUnit.Rem:
                    return "rem";
                default:
                    Debug.Fail("Unknown css unit type.");
                    return "";
            }
        }

        private double GetLengthInPoints()
        {
            double lengthInPt;
            switch (Unit)
            {
                case CssUnit.None:
                    lengthInPt = mValue;
                    break;
                case CssUnit.In:
                    lengthInPt = ConvertUtilCore.InchToPoint(mValue);
                    break;
                case CssUnit.Cm:
                    lengthInPt = ConvertUtilCore.CmToPoint(mValue);
                    break;
                case CssUnit.Mm:
                    lengthInPt = ConvertUtilCore.MmToPoint(mValue);
                    break;
                case CssUnit.Pt:
                    lengthInPt = mValue;
                    break;
                case CssUnit.Pc:
                    lengthInPt = mValue * 12; // 1pc = 12pt.
                    break;
                case CssUnit.Px:
                    lengthInPt = ConvertUtilCore.PixelToPoint(mValue);
                    break;
                case CssUnit.Em:
                case CssUnit.Ex:
                case CssUnit.Rem:
                    Debug.Assert(false, "Relative length is not allowed.");
                    lengthInPt = mValue;
                    break;
                default:
                    Debug.Assert(false);
                    lengthInPt = mValue;
                    break;
            }

            return lengthInPt;
        }

        private double PointsToLength(CssUnit unit)
        {
            double lengthInPoints = GetLengthInPoints();
            double length;
            switch (unit)
            {
                case CssUnit.None:
                    length = lengthInPoints;
                    break;
                case CssUnit.In:
                    length = ConvertUtilCore.PointToInch(lengthInPoints);
                    break;
                case CssUnit.Cm:
                    length = lengthInPoints / ConvertUtilCore.PointsPerCm;
                    break;
                case CssUnit.Mm:
                    length = lengthInPoints / ConvertUtilCore.PointsPerMm;
                    break;
                case CssUnit.Pt:
                    length = lengthInPoints;
                    break;
                case CssUnit.Pc:
                    length = lengthInPoints / 12; // 1pc = 12pt.
                    break;
                case CssUnit.Px:
                    length = ConvertUtilCore.PointToPixel(lengthInPoints);
                    break;
                case CssUnit.Em:
                case CssUnit.Ex:
                case CssUnit.Rem:
                    Debug.Assert(false, "Relative length is not allowed.");
                    length = lengthInPoints;
                    break;
                default:
                    Debug.Assert(false);
                    length = lengthInPoints;
                    break;
            }

            return length;
        }

        internal override double DoubleValue
        {
            get { return mValue; }
        }

        internal CssUnit Unit { get; }

        internal bool IsRelative
        {
            get { return (Unit == CssUnit.Em) || (Unit == CssUnit.Ex) || (Unit == CssUnit.Rem); }
        }

        private readonly double mValue;
    }
}
