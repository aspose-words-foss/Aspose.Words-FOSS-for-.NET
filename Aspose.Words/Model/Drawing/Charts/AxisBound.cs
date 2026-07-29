// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2018 by Alexander Zhiltsov

using System;
using Aspose.Common;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents minimum or maximum bound of axis values.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Bound can be specified as a numeric, datetime or a special "auto" value.</para>
    /// <para>The instances of this class are immutable.</para>
    /// <seealso cref="AxisScaling.Minimum"/>
    /// <seealso cref="AxisScaling.Maximum"/>
    /// </remarks>
    public sealed class AxisBound
    {
        /// <summary>
        /// Creates a new instance indicating that axis bound should be determined automatically by a word-processing
        /// application.
        /// </summary>
        public AxisBound()
        {
            mIsAuto = true;
        }

        /// <summary>
        /// Creates an axis bound represented as a number.
        /// </summary>
        public AxisBound(double value)
        {
            mValue = value;
        }

        /// <summary>
        /// Creates an axis bound represented as datetime value.
        /// </summary>
        public AxisBound(DateTime datetime)
        {
            if ((datetime <= MinExclusiveDate) || (datetime > MaxDate))
                throw new ArgumentOutOfRangeException("datetime");

            mValue = datetime.ToOADate();
        }

        /// <summary>
        /// Determines whether the specified object is equal in value to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(AxisBound))
                return false;

            AxisBound other = (AxisBound)obj;
            return (mIsAuto == other.mIsAuto) && (mIsAuto || (mValue == other.mValue));
        }

        /// <summary>
        /// Serves as a hash function for this type. 
        /// </summary>
        /// <javaName>int hashCode()</javaName>
        public override int GetHashCode()
        {
            unchecked
            {
                return (mIsAuto.GetHashCode() * 397) ^ mValue.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a user-friendly string that displays the value of this object.
        /// </summary>
        public override string ToString()
        {
            return mIsAuto
                ? "Auto"
                : (mValue > MinExclusiveDateValue) && (mValue < MaxExclusiveDateValue)
                    ? mValue + " (" + DateTime.FromOADate(mValue) + ")"
                    : FormatterPal.DoubleToStr(mValue);
        }

        /// <summary>
        /// Returns a flag indicating that axis bound should be determined automatically.
        /// </summary>
        public bool IsAuto
        {
            get { return mIsAuto; }
        }

        /// <summary>
        /// Returns numeric value of axis bound.
        /// </summary>
        public double Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Returns value of axis bound represented as datetime.
        /// </summary>
        public DateTime ValueAsDate
        {
            get
            {
                try
                {
                    return DateTime.FromOADate(mValue);
                }
                catch (ArgumentException)
                {
                    // The instance contains value that cannot be represented as date: return predetermined value.
                    return DateTime.MinValue;
                }
            }
        }

        private readonly double mValue;
        private readonly bool mIsAuto;

        private static readonly DateTime MinExclusiveDate = new DateTime(100, 1, 1);
        private static readonly DateTime MaxDate = DateTime.MaxValue;
        private const double MinExclusiveDateValue = -657435d;
        private const double MaxExclusiveDateValue = 2958466d;

        /// <summary>
        /// Returns a static instance indicating that axis bound should be determined automatically by a word-processing
        /// application.
        /// </summary>
        internal static readonly AxisBound Auto = new AxisBound();
    }
}
