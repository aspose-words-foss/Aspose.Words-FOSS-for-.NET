// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/08/2008 by Roman Korchagin

using Aspose.Common;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// In DocxImport we have a number of occasions where we need to parse a string which contains
    /// some numeric value (in various forms from integer to floating double) and some units ('pt', 'in', 'fd', etc.).
    /// I have contemplated for some time how to parse such strings in a most expedient way and finally
    /// decided to write a special class for handling them.
    /// </summary>
    internal class VmlQuantity
    {
        internal VmlQuantity(string value)
            : this(value, "")
        {
        }

        internal VmlQuantity(string value, string defaultUnits)
        {
            mSource = value;

            if (value != string.Empty)
            {
                // The index of a character where number part ends and units part starts.
                int splitIndex;

                for (splitIndex = value.Length - 1; splitIndex >= 0; splitIndex--)
                {
                    if (value[splitIndex] >= '0' && value[splitIndex] <= '9')
                        break;
                }

                string number = value.Substring(0, splitIndex + 1);
                mNumber = FormatterPal.TryParseDoubleInvariant(number);
                mIsValid = !double.IsNaN(mNumber);

                mUnits = value.Substring(splitIndex + 1, value.Length - splitIndex - 1).Trim();

                if (!StringUtil.HasChars(mUnits))
                    mUnits = defaultUnits;
            }
        }

        /// <summary>
        /// Returns true if the source string can be interpreted as degrees value.
        /// </summary>
        internal bool IsDegrees
        {
            get { return mIsValid && ((mUnits == "fd") || (mUnits == "f") || IsWithoutUnits); }
        }

        /// <summary>
        /// Returns true if the source string can be interpreted as distance value.
        /// </summary>
        internal bool IsDistance
        {
            get
            {
                return mIsValid && (IsDistanceUnits || IsWithoutUnits || IsZero);
            }
        }

        /// <summary>
        /// Returns true if the source string can be interpreted as fixed value.
        /// </summary>
        internal bool IsFixed
        {
            get { return mIsValid && ((mUnits == "f") || IsWithoutUnits); }
        }

        /// <summary>
        /// Returns true if the source string can be interpreted as percent value.
        /// </summary>
        internal bool IsPercent
        {
            get { return mIsValid && ((mUnits == "%") || IsZero); }
        }

        /// <summary>
        /// Returns true if the source string has no units defined.
        /// </summary>
        internal bool IsWithoutUnits
        {
            get { return mUnits == string.Empty; }
        }

        /// <summary>
        /// Converts quantity value from distance to EMUs.  Make sure that IsDistance is true before calling.
        /// </summary>
        internal int DistanceToEmus()
        {
            return MathUtil.DoubleToInt(DistanceToPoints() * 12700);
        }

        /// <summary>
        /// Returns true if the source string has numeric value of '0' with no units defined.
        /// </summary>
        private bool IsZero
        {
            get { return (mNumber == 0) && IsWithoutUnits; }
        }

        /// <summary>
        /// Returns true if the source string is a numeric value with no units defined.
        /// </summary>
        private bool IsDimensionless
        {
            get { return mIsValid && IsWithoutUnits; }
        }

        /// <summary>
        /// Returns true if the source string has distance units (in, px, pt, mm, cm).
        /// </summary>
        private bool IsDistanceUnits
        {
            get { return (mUnits == "in") || (mUnits == "px") || (mUnits == "pt") || (mUnits == "mm") || (mUnits == "cm"); }
        }

        /// <summary>
        /// Converts quantity value from distance to points. Make sure that IsDistance is true before calling.
        /// </summary>
        internal double DistanceToPoints()
        {
            Debug.Assert(IsDistance, string.Format("'{0}' is not a valid distance value.", mSource));

            switch (mUnits)
            {
                case "":
                    return mNumber;
                case "px":
                    // andrnosk: WORDSNET-7525 According to specification ECMA 376 possible units designators are
                    // cm, mm, in, pt, pc, or px. That is why px is added.
                    return ConvertUtilCore.PixelToPoint(mNumber);
                case "pt":
                    return mNumber;
                case "in":
                    return ConvertUtilCore.InchToPoint(mNumber);
                case "mm":
                    return ConvertUtilCore.MmToPoint(mNumber);
                case "cm":
                    // WORDSNET-11683 The problem occurred because size of shape in the document is specified in cm units.
                    return ConvertUtilCore.CmToPoint(mNumber);
                default:
                    // If the unit is not known then just return numeric part.
                    // Other options are to return 0 or throw, but I do not feel inclined to them.
                    // It is up to the caller to check if this instance IsDistance before calling ToPoints.
                    return 0;
            }
        }

        /// <summary>
        /// Converts quantity value from distance to PathValue.
        /// </summary>
        internal PathValue DistanceToPathValue()
        {
            // andrnosk: WORDSNET-4576 PathValue should be in Twips.
            return new PathValue(ConvertUtilCore.PointToTwip(DistanceToPoints()), false);
        }

        /// <summary>
        /// Converts quantity value from degrees to fixed. Make sure that IsDegrees is true before calling.
        /// </summary>
        internal int DegreesToFixed()
        {
            Debug.Assert(IsDegrees, string.Format("'{0}' is not a valid degrees value.", mSource));

            // WORDSNET-19749 Support the postfix "f".
            switch (mUnits)
            {
                case "fd":
                case "f":
                    return MathUtil.DoubleToInt(mNumber);
                case "":
                    return MathUtil.DoubleToInt(mNumber * 0x10000);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts quantity value from percent to integer. Make sure that IsPercent is true before calling.
        /// </summary>
        internal int PercentToInt()
        {
            Debug.Assert(IsPercent, string.Format("'{0}' is not a valid percent value.", mSource));

            if (IsPercent)
                return MathUtil.DoubleToInt(mNumber);
            else
                return 0;
        }

        /// <summary>
        /// Returns quantity value as fixed integer. Make sure that IsFixed is true before calling.
        /// </summary>
        internal int ToFixed()
        {
            Debug.Assert(IsFixed, string.Format("'{0}' is not a valid fixed value.", mSource));

            if (IsFixed)
            {
                if (IsDimensionless)
                    return MathUtil.DoubleToInt(mNumber * 0x10000);
                else
                    return MathUtil.DoubleToInt(mNumber);
            }
            else
                return 0;
        }

        private readonly string mSource;
        private readonly double mNumber;
        private readonly string mUnits = "";
        private readonly bool mIsValid;

        /// <summary>
        /// Pixels (px) units string.
        /// </summary>
        internal const string PixelUnits = "px";

        /// <summary>
        /// Points (pt) unit string.
        /// </summary>
        internal const string PointUnits = "pt";
    }
}
