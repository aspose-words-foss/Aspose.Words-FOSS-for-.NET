// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/08/2022 by Alexander Zhiltsov

using System;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents an X value for a chart series.
    /// </summary>
    /// <remarks>
    /// <p>This class contains a number of static methods for creating an X value of a particular type. The
    /// <see cref="ValueType"/> property allows you to determine the type of an existing X value.</p>
    /// <p>All non-null X values of a chart series must be of the same <see cref="ChartXValueType"/> type.</p>
    /// </remarks>
    public class ChartXValue
    {
        /// <summary>
        /// The static methods are used to create instances of this class.
        /// </summary>
        private ChartXValue()
        {
        }

        /// <summary>
        /// Creates a <see cref="ChartXValue"/> instance of the <see cref="ChartXValueType.String"/> type.
        /// </summary>
        public static ChartXValue FromString(string value)
        {
            ChartXValue xValue = new ChartXValue();
            xValue.mStringValue = value;
            xValue.mValueType = ChartXValueType.String;
            return xValue;
        }

        /// <summary>
        /// Creates a <see cref="ChartXValue"/> instance of the <see cref="ChartXValueType.Double"/> type.
        /// </summary>
        public static ChartXValue FromDouble(double value)
        {
            ChartXValue xValue = new ChartXValue();
            xValue.mDoubleValue = value;
            xValue.mValueType = ChartXValueType.Double;
            return xValue;
        }

        /// <summary>
        /// Creates a <see cref="ChartXValue"/> instance of the <see cref="ChartXValueType.DateTime"/> type.
        /// </summary>
        public static ChartXValue FromDateTime(DateTime value)
        {
            ChartXValue xValue = new ChartXValue();
            xValue.mDateTimeValue = value;
            xValue.mValueType = ChartXValueType.DateTime;
            return xValue;
        }

        /// <summary>
        /// Creates a <see cref="ChartXValue"/> instance of the <see cref="ChartXValueType.Time"/> type.
        /// </summary>
        public static ChartXValue FromTimeSpan(TimeSpan value)
        {
            ChartXValue xValue = new ChartXValue();
            xValue.mTimeValue = value;
            xValue.mValueType = ChartXValueType.Time;
            return xValue;
        }

        /// <summary>
        /// Creates a <see cref="ChartXValue"/> instance of the <see cref="ChartXValueType.Multilevel"/> type.
        /// </summary>
        public static ChartXValue FromMultilevelValue(ChartMultilevelValue value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            ChartXValue xValue = new ChartXValue();
            xValue.mMultilevelValue = value;
            xValue.mValueType = ChartXValueType.Multilevel;
            return xValue;
        }

        /// <summary>
        /// Gets a hash code for the current X value object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 31 * mValueType.GetHashCode();
                switch (mValueType)
                {
                    case ChartXValueType.String:
                        return hash + ((mStringValue != null) ? mStringValue.GetHashCode() : 0);
                    case ChartXValueType.Double:
                        return hash + mDoubleValue.GetHashCode();
                    case ChartXValueType.DateTime:
                        return hash + mDateTimeValue.GetHashCode();
                    case ChartXValueType.Time:
                        return hash + mTimeValue.GetHashCode();
                    case ChartXValueType.Multilevel:
                        Debug.Assert(mMultilevelValue != null);
                        return hash + mMultilevelValue.GetHashCode();
                    default:
                        return hash;
                }
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified object is equal to the current X value object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            else if (ReferenceEquals(this, obj))
                return true;
            else if (obj.GetType() != GetType())
                return false;

            ChartXValue xValue = (ChartXValue)obj;
            if (ValueType != xValue.ValueType)
                return false;

            switch (ValueType)
            {
                case ChartXValueType.String:
                    return object.Equals(mStringValue, xValue.StringValue);
                case ChartXValueType.Double:
                    return mDoubleValue.Equals(xValue.mDoubleValue);
                case ChartXValueType.DateTime:
                    return mDateTimeValue.Equals(xValue.mDateTimeValue);
                case ChartXValueType.Time:
                    return mTimeValue.Equals(xValue.mTimeValue);
                case ChartXValueType.Multilevel:
                    Debug.Assert(mMultilevelValue != null);
                    return mMultilevelValue.Equals(xValue.mMultilevelValue);
                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        /// <summary>
        /// Gets the type of the X value stored in the object.
        /// </summary>
        public ChartXValueType ValueType
        {
            get { return mValueType; }
        }

        /// <summary>
        /// Gets the stored string value.
        /// </summary>
        /// <dev>
        /// Returns <b>null</b> if the instance is not of the <see cref="ChartXValueType.String"/> type.
        /// </dev>
        public string StringValue
        {
            get { return mStringValue; }
        }

        /// <summary>
        /// Gets the stored numeric value.
        /// </summary>
        /// <dev>
        /// Returns <see cref="double.NaN"/> if the instance is not of the <see cref="ChartXValueType.Double"/> type.
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
        /// <see cref="ChartXValueType.DateTime"/> type.
        /// </dev>
        public DateTime DateTimeValue
        {
            get { return mDateTimeValue; }
        }

        /// <summary>
        /// Gets the stored time value.
        /// </summary>
        /// <dev>
        /// Returns <see cref="TimeSpan.Zero"/> if the instance is not of the <see cref="ChartXValueType.Time"/> type.
        /// </dev>
        public TimeSpan TimeValue
        {
            get { return mTimeValue; }
        }

        /// <summary>
        /// Gets the stored multilevel value.
        /// </summary>
        /// <dev>
        /// Returns <b>null</b> if the instance is not of the <see cref="ChartXValueType.Multilevel"/> type.
        /// </dev>
        public ChartMultilevelValue MultilevelValue
        {
            get { return mMultilevelValue; }
        }

        private ChartXValueType mValueType;
        private string mStringValue;
        private double mDoubleValue = double.NaN;
        private DateTime mDateTimeValue = DateTime.MinValue;
        private TimeSpan mTimeValue = TimeSpan.Zero;
        private ChartMultilevelValue mMultilevelValue;
    }
}
