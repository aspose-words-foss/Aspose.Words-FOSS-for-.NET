// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2012 by Alexey Noskov

using System;
using Aspose.Common;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents pt (Numeric Point) (5.7.2.151) element.
    /// This element specifies data for a particular data point.
    /// </summary>
    internal class DmlChartNumValue : DmlChartValue
    {
        internal DmlChartNumValue(int index, double value)
            : this(index, value, null)
        {
        }

        internal DmlChartNumValue(int index, double value, string formatCode)
            : this(index, value, formatCode, 1)
        {
        }

        internal DmlChartNumValue(int index, double value, string formatCode, double displayUnit)
            : base(index, DmlChartValueType.Numeric)
        {
            mValue = value;
            SourceFormatCode = formatCode;
            mDisplayUnit = displayUnit;
        }

        internal void SetDisplayUnit(double displayUnit)
        {
            mDisplayUnit = displayUnit;
        }

        internal static string ValueToString(double value, double displayUnit, string formatCode)
        {
            if (double.IsNaN(value))
                return NanStringValue;

            double displayValue = value / displayUnit;

            // FormatterPal needed for java since ordinal double format has too big precision in java.
            // Special processing for "general" format is required.
            if (DmlChartFormatCodeValidator.IsGeneralFormatCode(formatCode))
                return DmlChartRenderingUtil.GetGeneralFormatStringValue(displayValue);
            else
                return DmlChartFormatCodeValidator.IsDateFormat(formatCode)
                    ? ToDateString(value, formatCode)
                    : DmlChartRenderingUtil.DoubleToString(displayValue, formatCode);
        }
      
        /// <summary>
        /// Converts date value to string.
        /// </summary>
        private static string ToDateString(double value, string formatCode)
        {
            // If the DmlChartNumValue is a date and less than 0, MS Word do not display the string value.
            if (value < 0)
                return string.Empty;

            DateTime date = DmlChartUtil.GetDateFromDouble(value);

            // If format code is not specified lets use standard one.
            if (DmlChartFormatCodeValidator.IsGeneralFormatCode(formatCode))
                formatCode = FormatterPal.GetShortDatePatternCurrent();

            return DmlChartRenderingUtil.GetStringDate(date, formatCode);
        }

        internal override string StringValue
        {
            get
            {
                if (string.IsNullOrEmpty(mStringValue))
                    mStringValue = ValueToString(mValue, mDisplayUnit, mFormatCode);

                return mStringValue;
            }
        }

        /// <summary>
        /// Returns true if format string is date format.
        /// </summary>
        internal override bool IsDate
        {
            get
            {
                if (mIsDate == NullableBool.NotDefined)
                {
                    string formatCode = FormatCode;
                    if (!StringUtil.HasChars(formatCode) && (Collection != null))
                        formatCode = Collection.FormatCode;

                    mIsDate = DmlChartFormatCodeValidator.IsDateFormat(formatCode)
                        ? NullableBool.True
                        : NullableBool.False;
                }

                return (mIsDate == NullableBool.True);
            }
        }

        internal override double Value
        {
            get { return mValue; }
        }

        internal string FormatCode
        {
            get { return mFormatCode; }
            set
            {
                mFormatCode = DmlChartFormatCodeValidator.ValidateFormatCode(value);
                mIsDate = NullableBool.NotDefined;
            }
        }

        /// <summary>
        /// Returns format code set in the document.
        /// Used for roundtrip.
        /// </summary>
        internal string SourceFormatCode
        {
            get { return mSourceFormatCode; }
            private set
            {
                mSourceFormatCode = value;
                FormatCode = mSourceFormatCode;
            }
        }

        /// <summary>
        /// Returns the double value based on the specified display unit value (default "1").
        /// </summary>
        internal double DisplayUnitValue
        {
            get { return mValue / mDisplayUnit ; }
        }

        private readonly double mValue;
        private string mFormatCode;
        private string mSourceFormatCode;
        private double mDisplayUnit;
        private string mStringValue = string.Empty;
        private NullableBool mIsDate = NullableBool.NotDefined;
    }
}
