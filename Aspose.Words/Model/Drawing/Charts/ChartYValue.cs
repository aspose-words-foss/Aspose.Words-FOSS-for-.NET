// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2022 by Alexander Zhiltsov

using System;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents an Y value for a chart series.
    /// </summary>
    /// <remarks>
    /// <p>This class contains a number of static methods for creating an Y value of a particular type. The
    /// <see cref="ValueType"/> property allows you to determine the type of an existing Y value.</p>
    /// <p>All non-null Y values of a chart series must be of the same <see cref="ChartYValueType"/> type.</p>
    /// </remarks>
    public class ChartYValue
    {
        /// <summary>
        /// The static methods are used to create instances of this class.
        /// </summary>
        private ChartYValue()
        {
        }

        /// <summary>
        /// Creates a <see cref="ChartYValue"/> instance of the <see cref="ChartYValueType.Double"/> type.
        /// </summary>
        public static ChartYValue FromDouble(double value)
        {
            ChartYValue yValue = new ChartYValue();
            yValue.mDoubleValue = value;
            yValue.mValueType = ChartYValueType.Double;
            return yValue;
        }

        /// <summary>
        /// Creates a <see cref="ChartYValue"/> instance of the <see cref="ChartYValueType.DateTime"/> type.
        /// </summary>
        public static ChartYValue FromDateTime(DateTime value)
        {
            ChartYValue yValue = new ChartYValue();
            yValue.mDateTimeValue = value;
            yValue.mValueType = ChartYValueType.DateTime;
            return yValue;
        }

        /// <summary>
        /// Creates a <see cref="ChartYValue"/> instance of the <see cref="ChartYValueType.Time"/> type.
        /// </summary>
        public static ChartYValue FromTimeSpan(TimeSpan value)
        {
            ChartYValue yValue = new ChartYValue();
            yValue.mTimeValue = value;
            yValue.mValueType = ChartYValueType.Time;
            return yValue;
        }

        /// <summary>
        /// Gets a hash code for the current Y value object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 31 * mValueType.GetHashCode();
                switch (mValueType)
                {
                    case ChartYValueType.Double:
                        return hash + mDoubleValue.GetHashCode();
                    case ChartYValueType.DateTime:
                        return hash + mDateTimeValue.GetHashCode();
                    case ChartYValueType.Time:
                        return hash + mTimeValue.GetHashCode();
                    default:
                        return hash;
                }
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified object is equal to the current Y value object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            else if (ReferenceEquals(this, obj))
                return true;
            else if (obj.GetType() != GetType())
                return false;

            ChartYValue yValue = (ChartYValue)obj;
            if (ValueType != yValue.ValueType)
                return false;

            switch (ValueType)
            {
                case ChartYValueType.Double:
                    return mDoubleValue.Equals(yValue.mDoubleValue);
                case ChartYValueType.DateTime:
                    return mDateTimeValue.Equals(yValue.mDateTimeValue);
                case ChartYValueType.Time:
                    return mTimeValue.Equals(yValue.mTimeValue);
                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        /// <summary>
        /// Gets the type of the Y value stored in the object.
        /// </summary>
        public ChartYValueType ValueType
        {
            get { return mValueType; }
        }

        /// <summary>
        /// Gets the stored numeric value.
        /// </summary>
        /// <dev>
        /// Returns <see cref="double.NaN"/> if the instance is not of the <see cref="ChartYValueType.Double"/> type.
        /// </dev>
        public double DoubleValue
        {
            get { return mDoubleValue; }
        }

        /// <summary>
        /// Gets the stored datetime value.
        /// </summary>
        /// <dev>
        /// Returns <see cref="DateTime.MinValue"/> if the instance is not of the
        /// <see cref="ChartYValueType.DateTime"/> type.
        /// </dev>
        public DateTime DateTimeValue
        {
            get { return mDateTimeValue; }
        }

        /// <summary>
        /// Gets the stored time value.
        /// </summary>
        /// <dev>
        /// Returns <see cref="TimeSpan.Zero"/> if the instance is not of the <see cref="ChartYValueType.Time"/> type.
        /// </dev>
        public TimeSpan TimeValue
        {
            get { return mTimeValue; }
        }

        private ChartYValueType mValueType;
        private double mDoubleValue = double.NaN;
        private DateTime mDateTimeValue = DateTime.MinValue;
        private TimeSpan mTimeValue = TimeSpan.Zero;
    }
}
