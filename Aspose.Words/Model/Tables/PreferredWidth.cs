// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2011 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Common;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents a value and its unit of measure that is used to specify the preferred width of a table or a cell.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Preferred width can be specified as a percentage, number of points or a special "none/auto" value.</para>
    ///
    /// <para>The instances of this class are immutable.</para>
    ///
    /// <seealso cref="Table.PreferredWidth"/>
    /// <seealso cref="CellFormat.PreferredWidth"/>
    /// </remarks>
    public sealed class PreferredWidth
    {
        // RK The XXXSafe methods are supposed to silently bring the value into the correct range, but at the moment
        // they just do no checks at all. If we attempt to bring the value into the correct range then some tests
        // in the table layouter fail and I don't want to deal with them now.

        internal static PreferredWidth FromRawSafe(PreferredWidthType type, int rawValue)
        {
            return new PreferredWidth(type, rawValue);
        }

        internal static PreferredWidth FromTwipsSafe(int twips)
        {
            return twips == 0
                ? ZeroTwips
                : new PreferredWidth(PreferredWidthType.Points, twips);
        }

        internal static PreferredWidth FromPercentSafe(double percent)
        {
            return new PreferredWidth(PreferredWidthType.Percent, MathUtil.DoubleToInt(percent * PercentFactor));
        }

        internal static PreferredWidth FromPointsSafe(double points)
        {
            return new PreferredWidth(PreferredWidthType.Points, ConvertUtilCore.PointToTwip(points));
        }

        /// <summary>
        /// Limits an arbitrary percentage value to the range allowed by this class (safe range).
        /// </summary>
        internal static double LimitPercentsToSafeRange(double percent)
        {
            return MathUtil.FitToRange(percent, 0, MaxPercent);
        }

        /// <summary>
        /// Limits an arbitrary value in points to the range allowed by this class (safe range).
        /// </summary>
        internal static double LimitPointsToSafeRange(double points)
        {
            return MathUtil.FitToRange(points, 0, MaxPoints);
        }

        /// <summary>
        /// Gets a valid table preferred width with out-of-range values matching MS Word interpretation.
        /// </summary>
        /// <remarks>
        /// The main purpose for this method is to interpret values not fitting into int16.
        /// The method also handles negative and 0 value cases.
        /// The method is used for table preferred width only.
        /// For cell preferred width, int32 handling should be the same, but handling 0 values might be different.
        /// </remarks>
        internal PreferredWidth GetCorrespondingValidTblwValue()
        {
            // Cast the value to int16 (TestInvalidTblw()):
            short valueRaw16 = (short)ValueRaw;
            // Treat as auto if the value is negative *after the casting*.
            int validRaw = valueRaw16;
            validRaw = System.Math.Max(0, validRaw);
            // Use the maximum legal twip value if it is exceeded.
            if (Type != PreferredWidthType.Percent)
                validRaw = System.Math.Min(TableGridMetrics.MaxWidthTwips, validRaw);
            // For pct preferred, MaxWidthTwips limits the twip values calculated from percent preferred.

            // Disregard the value if the type is auto.
            if (Type == PreferredWidthType.Auto)
                validRaw = 0;
            // It appears that this did not work for TestJira9884 earlier, but currently it works correctly.

            PreferredWidth validPreferred;
            // Do not treat zero dxa as auto, it must be handled specially for nested table metrics. TestTcw0DxaNested.
            if ((validRaw == 0) && (Type == PreferredWidthType.Percent))
                validPreferred = Auto;
            else if (validRaw == ValueRaw)
                validPreferred = this;
            else
                validPreferred = FromRawSafe(Type, validRaw);

            return validPreferred;
        }

        /// <summary>
        /// A creation method that returns a new instance that represents a preferred width specified as a percentage.
        /// </summary>
        /// <param name="percent">The value must be from 0 to 100.</param>
        public static PreferredWidth FromPercent(double percent)
        {
            ArgumentUtil.ValidateRange(percent, 0, 0, MaxPercent, MaxPercent, true, "percent");
            return FromPercentSafe(percent);
        }

        /// <summary>
        /// A creation method that returns a new instance that represents a preferred width specified using a number of points.
        /// </summary>
        /// <param name="points">The value must be from 0 to 22 inches (22 * 72 points).</param>
        public static PreferredWidth FromPoints(double points)
        {
            ArgumentUtil.ValidateRange(points, 0, 0, MaxPoints, MaxPoints, true, "points");
            return FromPointsSafe(points);
        }

        /// <summary>
        /// Returns an instance that represents the "preferred width is not specified" value.
        /// </summary>
        public static readonly PreferredWidth Auto = new PreferredWidth(PreferredWidthType.Auto, 0);

        /// <summary>
        /// Private ctor, the raw value is twips or 1/50th of a percent.
        /// </summary>
        private PreferredWidth(PreferredWidthType type, int rawValue)
        {
            mTypeOrig = type;

            // WORDSNET-1311 MS Word has this Nil type, looks like it means Auto.
            if ((int)type == 0)
                type = PreferredWidthType.Auto;

            // RK In some documents I get a raw value non zero when the type is Auto and MS Word behaves as it is a fixed value,
            // but in some documents I get a small raw value e.g. 80 and MS Word behaves as it is an Auto value.
            // If I try to "correct" it by either setting the value to 0 or by setting the type to "fixed" it causes problems in some documents.
            // I don't have a good explanation for this and therefore leave as is. The layout algorithm seems to be smart enough
            // to handle such confusing values (Auto and Value > 0) correctly and it is enough for me.

            mType = type;
            mValueRaw = rawValue;
        }

        /// <summary>
        /// Gets the unit of measure used for this preferred width value.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public PreferredWidthType Type
        {
            get { return mType; }
        }

        /// <summary>
        /// Gets the original unit of measure used for this preferred width value (passed to constructor).
        /// </summary>
        internal PreferredWidthType TypeOrig
        {
            get { return mTypeOrig; }
        }

        /// <summary>
        /// Gets the preferred width value. The unit of measure is specified in the <see cref="Type"/> property.
        /// </summary>
        public double Value
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                switch (mType)
                {
                    case PreferredWidthType.Auto:
                        return 0;
                    case PreferredWidthType.Percent:
                        return (double)mValueRaw / PercentFactor;
                    case PreferredWidthType.Points:
                        return ConvertUtilCore.TwipToPoint(mValueRaw);
                    default:
                        throw new InvalidOperationException("Unknown preferred width type.");
                }
            }
        }

        /// <summary>
        /// This is either twips or 1/50th of percent.
        /// </summary>
        internal int ValueRaw
        {
            get { return mValueRaw; }
        }

        /// <summary>
        /// Gets the length value in twips.
        /// </summary>
        internal int ValueTwips
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                Debug.Assert(IsFixed, "Attempted to access non-twips as twips.");
                return mValueRaw;
            }
        }

        /// <summary>
        /// Returns the twips value for fixed value type, and zero otherwise.
        /// </summary>
        /// <remarks>
        /// It is used to preserve the original AW behavior when the older code is not aware
        /// of preferred widths in percent units for widthBefore and widthAfter.
        /// </remarks>
        internal int ValueTwipsSafe
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                return IsFixed
                    ? ValueRaw
                    : 0;
            }
        }

        internal bool IsPositive
        {
            get { return mValueRaw > 0; }
        }

        internal static bool HasPositiveValue(PreferredWidth width)
        {
            return (width != null) && width.IsPositive;
        }

        /// <summary>
        /// Returns True if the preferred width is none/auto.
        /// </summary>
        internal bool IsAuto
        {
            get { return (mType == PreferredWidthType.Auto) || (mValueRaw <= 0); }
        }

        /// <summary>
        /// True if the preferred width is expressed in absolute units.
        /// </summary>
        internal bool IsFixed
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return (mType == PreferredWidthType.Points); }
        }

        /// <summary>
        /// True if the preferred width is expressed in percent.
        /// </summary>
        internal bool IsPercent
        {
            get { return (mType == PreferredWidthType.Percent); }
        }

        /// <summary>
        /// Gets a boolean value indicating either the PreferredWidth is valid.
        /// </summary>
        internal bool IsValid
        {
            get
            {
                return (mType == PreferredWidthType.Auto) ||
                       (mType == PreferredWidthType.Percent) ||
                       (mType == PreferredWidthType.Points);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="PreferredWidth"/> is equal in value to the current <see cref="PreferredWidth"/>.
        /// </summary>
        public bool Equals(PreferredWidth other)
        {
            // Generated by ReSharper.
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return (other.mType == mType) && (other.mValueRaw == mValueRaw);
        }

        /// <summary>
        /// Determines whether the specified object is equal in value to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Generated by ReSharper.
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(PreferredWidth))
                return false;
            return Equals((PreferredWidth)obj);
        }

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            // Generated by ReSharper.
            //JAVA-added explicit conversion from enum to int to workaround Metaspec parser bug:
            //Enum.GetHashCode() reported as Object.GetHashCode().
            return (((int)mType).GetHashCode() * 397) ^ mValueRaw;
        }

        /// <summary>
        /// Returns a user-friendly string that displays the value of this object.
        /// </summary>
        public override string ToString()
        {
            switch (mType)
            {
                case PreferredWidthType.Auto:
                    return "Auto";
                case PreferredWidthType.Percent:
                    return FormatterPal.DoubleToStr2Decimals(Value) + '%';
                case PreferredWidthType.Points:
                    return FormatterPal.IntToStr(ValueTwips);
                default:
                    return base.ToString();
            }
        }

        private readonly PreferredWidthType mType;
        private readonly PreferredWidthType mTypeOrig;
        private readonly int mValueRaw;

        /// <summary>
        /// WORDSNET-5520 MS Word allows setting preferred width up to 600% so we do the same.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxPercent = 600;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxPoints = 22 * 72;

        /// <summary>
        /// All percent values in the spec as before Iso29500 strict came
        /// without "%" sign. That meant they are specified in 1/50th of percent.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int PercentFactor = 50;

        /// <summary>
        /// Static immutable zero twips used to avoid creating a new zero object each time it is needed.
        /// </summary>
        internal static readonly PreferredWidth ZeroTwips = FromRawSafe(PreferredWidthType.Points, 0);
        // A combination of 0 width type Points and zero value is actually a special case, not normally saved by MS Word.
        // For grid calculation it is only used to avoid getting auto width for v-merged cells.
    }
}
