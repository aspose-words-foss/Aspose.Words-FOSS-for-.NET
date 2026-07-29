// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2021 by Alexander Zhiltsov

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents chart series data to render.
    /// </summary>
    internal class DmlChartRenderingData : IEnumerable<DmlChartValue>
    {
        internal DmlChartRenderingData(DmlChartValueCollection values)
        {
            mOriginalValues = values;
            mValues = null;
        }

        internal void AddValue(DmlChartValue value)
        {
            PrepareForModification();
            Values.Add(value);
        }

        internal DmlChartRenderingData Clone()
        {
            return Clone(mOriginalValues.Clone());
        }

        internal DmlChartRenderingData Clone(DmlChartValueCollection originalData)
        {
            DmlChartRenderingData lhs = (DmlChartRenderingData)MemberwiseClone();

            lhs.mOriginalValues = originalData;

            if (mValues != null)
                lhs.mValues = mValues.Clone();

            return lhs;
        }

        /// <summary>
        /// If data is empty or incomplete, fills it with dummy data.
        /// </summary>
        internal void AddDummyData(int valueCount)
        {
            if (IsDate || HasNonEmptyValues)
                return;

            ValueCount = System.Math.Max(valueCount, 1);
            FillWithDummyValues();
        }

        /// <summary>
        /// Gets a value at the specified index.
        /// </summary>
        internal DmlChartValue GetValue(int index)
        {
            return GetValue(Values, index);
        }

        /// <summary>
        /// Gets a value at the specified index.
        /// </summary>
        internal static DmlChartValue GetValue(DmlChartValueCollection data, int index)
        {
            // If there is no data, we should generate data.
            // No data seems to be allowed only for Category axis, that is why return str value.
            if (data == null)
                return new DmlChartStrValue(index, (index + 1).ToString());

            DmlChartValue value = data[index];

            // If there is no entry with specified index, and this is str data, return a dummy value.
            if ((value == null) &&
                ((data.ValueType == DmlChartValueType.String) || (data.ValueType == DmlChartValueType.MultiLvlString)))
                return new DmlChartDummyValue(index, index + 1);

            return value;
        }

        /// <summary>
        /// Gets a value, originally loaded from the document, at the specified index.
        /// </summary>
        internal DmlChartValue GetOriginalValue(int index)
        {
            return GetValue(mOriginalValues, index);
        }

        /// <summary>
        /// Changes value type of the data collection. Method also converts values to the corresponding string values.
        /// Has effect only when original value type is numeric.
        /// </summary>
        internal void ResetTypeToString(string axisFormatCode, bool isSourceLinked)
        {
            if (ValueType != DmlChartValueType.Numeric && ValueType != DmlChartValueType.MultiLvlNumeric)
                return;

            ChangeCollectionValues(Values, DmlChartValueType.String, axisFormatCode, isSourceLinked);
        }

        /// <summary>
        /// Returns array of values that represents this data source.
        /// Used for rendering pie charts.
        /// </summary>
        internal DmlChartValue[] ToArray()
        {
            DmlChartValue[] values = new DmlChartValue[ValueCount];
            int notNullSize = 0;

            for (int i = 0; i < ValueCount; i++)
            {
                DmlChartValue val = GetValue(i);

                if (val == null)
                    continue;

                values[notNullSize++] = val;
            }

            // Strip out null values.
            DmlChartValue[] notNullValues = new DmlChartValue[notNullSize];
            Array.Copy(values, notNullValues, notNullSize);

            return notNullValues;
        }

        /// <summary>
        /// Returns sum of all values in the data source.
        /// </summary>
        internal double GetSum()
        {
            // Had to make the sum variable of double type, because different order of iterated values
            // led to different results because of float type's low precision.
            double sum = 0;

            // Sum is used for calculation of percentage values. It seems the sum should be calculated for absolute values.
            foreach (DmlChartValue val in Values)
                sum += System.Math.Abs(DmlChartValue.IsNullOrNaN(val) ? 0 : val.Value);

            return sum;
        }

        /// <summary>
        /// Returns first not null value from the collection, if there are not null values, returns null.
        /// </summary>
        internal DmlChartValue GetFirstNotNullValue()
        {
            int firstIndex = GetFirstNotNullIndex();
            return (firstIndex < 0) ? null : Values[firstIndex];
        }

        /// <summary>
        /// Returns dummy value for date data. Value of dummy value is calculated using other not null dates.
        /// </summary>
        internal DmlChartValue GetDummyDate(int index)
        {
            int firstIndex = GetFirstNotNullIndex();
            int lastIndex = GetLastNotNullIndex();

            // If we have no not null values, we cannot generate dummy value, return null.
            if ((firstIndex < 0) || (lastIndex < 0) || (firstIndex == lastIndex))
                return null;

            DmlChartValue firstValue = Values[firstIndex];
            DmlChartValue lastValue = Values[lastIndex];

            // Otherwise try to calculate value using approximate step.
            double step = (lastValue.Value - firstValue.Value) / (lastIndex - firstIndex);
            double val = firstValue.Value + (index - firstIndex) * step;

            return new DmlChartDummyValue(index, val);
        }

        /// <summary>
        /// Returns percentage value for data label.
        /// </summary>
        internal double GetPercentValue(int index, string format)
        {
            if (mPercentages == null)
                InitPercentages(format);

            return (index >= mPercentages.Length) ? 0.0d : mPercentages[index];
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<DmlChartValue> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Fills the specified <see cref="DmlChartRenderingData"/> with "dummy" values. The collection is expected
        /// to be empty with defined <see cref="ValueCount"/> property.
        /// </summary>
        private void FillWithDummyValues()
        {
            Debug.Assert(!HasNonEmptyValues);
            PrepareForModification();
            Values.FillWithDummyValues();
        }

        /// <summary>
        /// Returns index of the first not null entry. Return negative value if there is no not null values.
        /// </summary>
        private int GetFirstNotNullIndex()
        {
            for (int i = 0; i <= LastNonEmptyValueIndex; i++)
            {
                if (!DmlChartValue.IsNullOrNaN(Values[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns index of the last not null entry. Return negative value if there is no not null values.
        /// </summary>
        private int GetLastNotNullIndex()
        {
            for (int i = LastNonEmptyValueIndex; i >= 0; i--)
            {
                if (!DmlChartValue.IsNullOrNaN(Values[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Changes values into current collection.
        /// </summary>
        /// <param name="sourceValues">The values collection that is used as sample</param>
        /// <param name="valueType"><see cref="DmlChartValueType"/> of the result collection</param>
        /// <param name="formatCode">The format code is used to convert values of the source collection</param>
        /// <param name="isSourceLinked">Indicates whether the source collection is linked with source</param>
        private void ChangeCollectionValues(
            DmlChartValueCollection sourceValues,
            DmlChartValueType valueType,
            string formatCode,
            bool isSourceLinked)
        {
            DmlChartValueCollection newData = new DmlChartValueCollection(valueType);

            foreach (DmlChartValue value in sourceValues)
            {
                DmlChartNumValue numValue = value as DmlChartNumValue;

                if (numValue == null)
                    continue;

                newData.Add((valueType == DmlChartValueType.String)
                    ? (DmlChartValue)GetStringValue(numValue, formatCode, isSourceLinked)
                    : (DmlChartValue)new DmlChartNumValue(numValue.Index, numValue.Value, numValue.FormatCode));
            }

            mValues = newData;
        }

        /// <summary>
        /// Gets <see cref="DmlChartStrValue"/> based on input <see cref="DmlChartNumValue"/> and format code.
        /// </summary>
        /// <param name="value">The input <see cref="DmlChartNumValue"/> from source collection</param>
        /// <param name="formatCode">Format that is used to convert value to string</param>
        /// <param name="isSourceLinked">Indicates whether the source collection is linked with source</param>
        /// <returns><see cref="DmlChartStrValue"/></returns>
        private DmlChartStrValue GetStringValue(DmlChartNumValue value, string formatCode, bool isSourceLinked)
        {
            string strValue;
            string actualFormat = DmlChartFormatCodeValidator.GetFormatCode(formatCode, isSourceLinked,
                value.FormatCode, mFormatCode);

            // WORDSNET-17258 The date format can be used for category values. In this case, MS Word renders dates.
            // WORDSNET-23818 The value type should be checked.
            // WORDSNET-25379 The value must not exceed the .NET maximum date value.
            bool isDateFormat =
                IsCorrectMsWordDate(value.Value) &&
                (value.IsDate || DmlChartFormatCodeValidator.IsDateFormat(actualFormat)) &&
                !DmlChartFormatCodeValidator.IsGeneralFormatCode(actualFormat);

            if (isDateFormat && (value.ValueType != DmlChartValueType.None) && !value.IsNaN)
                strValue = GetStringDate(value.Value, actualFormat);
            // WORDSNET-22826 If a format code is specified, it should be used when converting the value to a string.
            // WORDSNET-28787 Used DoubleToString from DmlChartRenderingUtil when converting the value to a string.
            else if (!DmlChartFormatCodeValidator.IsGeneralFormatCode(actualFormat) && !double.IsNaN(value.Value))
                strValue = DmlChartRenderingUtil.DoubleToString(value.Value, actualFormat);
            else
                strValue = value.StringValue;

            return new DmlChartStrValue(value.Index, strValue);
        }


        /// <summary>
        /// Gets a flag indicating whether the specified numeric MS Word date value is correct.
        /// </summary>
        private static bool IsCorrectMsWordDate(double value)
        {
            const double minDateValue = 0; // 00.01.1900, the minimum date in MS Word.
            const double maxDateValue = 2958466; // Corresponds to DateTime.MaxDate.
            return (value >= minDateValue) && (value < maxDateValue);
        }

        /// <summary>
        /// Gets a string representation of a date based on the specified format code.
        /// </summary>
        /// <param name="value">The specified value</param>
        /// <param name="formatCode">The specified format code</param>
        /// <returns>The string representation of the date</returns>
        private string GetStringDate(double value, string formatCode)
        {
            // The axis label is not rendered if value is less than 0 and the axis is "DateAxis".
            if (value < 0)
                return string.Empty;

            bool isTime = DmlChartFormatCodeValidator.IsTimeFormat(formatCode);
            DateTime date = DmlChartUtil.GetDateFromDouble(value);

            // If values are dates MS Word rounds them, do the same.
            // WORDSNET-21384 MS Word does not round values if they are "time".
            long roundDateStep = isTime ? 0 : GetRoundDateStep();

            DateTime roundDate = (roundDateStep != 0)
                ? new DateTime(MidPointRound(date.Ticks, roundDateStep) * roundDateStep)
                : date;

            return DmlChartRenderingUtil.GetStringDate(roundDate, formatCode);
        }

        private long GetRoundDateStep()
        {
            long step = GetTicksStep();
            if (MidPointRound(step, TimeSpan.TicksPerDay) > 0)
                return TimeSpan.TicksPerDay;
            if (MidPointRound(step, TimeSpan.TicksPerHour) > 0)
                return TimeSpan.TicksPerHour;
            if (MidPointRound(step, TimeSpan.TicksPerMinute) > 0)
                return TimeSpan.TicksPerMinute;
            if (MidPointRound(step, TimeSpan.TicksPerSecond) > 0)
                return TimeSpan.TicksPerSecond;
            return step;
        }

        private long GetTicksStep()
        {
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            foreach (DmlChartValue value in Values)
            {
                if (value.IsNaN)
                    continue;

                minValue = System.Math.Min(minValue, value.Value);
                maxValue = System.Math.Max(maxValue, value.Value);
            }

            double step = (maxValue - minValue) / (LastNonEmptyValueIndex + 1);

            DateTime start = DateTime.FromOADate(0);
            DateTime end = DateTime.FromOADate(step);

            TimeSpan span = (end - start);
            return span.Ticks;
        }

        private static long MidPointRound(long src, long round)
        {
            return (src + (round / 2) + 1) / round;
        }

        private void InitPercentages(string format)
        {
            mPercentages = new double[ValueCount];
            DmlChartDummyValue[] positiveDiffs = new DmlChartDummyValue[ValueCount];
            DmlChartDummyValue[] negativeDiffs = new DmlChartDummyValue[ValueCount];
            double sum = GetSum();
            double percentageSum = 0;
            int precision = GetPrecision(format);

            // Initialize percentages with initial values. MS word rounds percentage values to whole numbers
            // if format is not specified and sometimes it is required to adjust values to get 100% as the sum.
            for (int i = 0; i < ValueCount; i++)
            {
                DmlChartValue val = GetValue(i);
                // If sum is 1 we have no need to divide the original value.
                double factor = MathUtil.AreEqual(1.0d, sum) ? 1 : (1 / sum);
                double fractionValue = DmlChartValue.IsNullOrNaN(val) ? 0.0d : (val.Value * factor);
                mPercentages[i] = fractionValue;

                // Mid point value must be rounded to greater value.
                double percentValue = System.Math.Round(fractionValue, precision, MidpointRounding.AwayFromZero);
                double diff = percentValue - fractionValue;
                percentageSum += System.Math.Abs(percentValue);

                if (MathUtil.IsZero(diff))
                    continue;

                // Ms Word double rounding seems to work in different way than .NET. It seems simply trimming value to 15
                // digits after the dot do the trick.
                DmlChartDummyValue[] usedDiffs = (diff > 0) ? positiveDiffs : negativeDiffs;
                usedDiffs[i] = new DmlChartDummyValue(i, DoublePal.Trim(System.Math.Abs(diff), 15));
            }

            // The desired value of percentage sum is 100, calculate difference between desired and actual values.
            double delta = 1 - percentageSum;

            // Absolute value of delta is the number of values to be adjusted.
            double deltaAbs = System.Math.Abs(delta);

            // If delta is zero, percentage values should not be adjusted.
            if (MathUtil.IsZero(delta))
                return;

            // If deltaAbs * 100 < 1 or the percent symbol is not used, percentage values should not be adjusted.
            if (deltaAbs * 100 < 1 || !DmlChartFormatCodeValidator.IsPercentFormat(format))
                return;

            // MS Word in most of case adds 1%. There are cases when it adds 2% maybe there are other cases,
            // but I cannot figure out how the addition is calculated. Though it seems to be rare case, so use 1% as an
            // addition.
            double addition = (1 / System.Math.Pow(10, precision)) * (delta / deltaAbs);

            // Sort differences array, it seems MS Word adjusts values that have the biggest difference between
            // rounded value and actual value. After sorting values that should be adjusted become at the end of array.
            // WORDSNET-22693 If addition is negative, MS Words corrects values with a maximum positive difference.
            DmlChartDummyValue[] diffs = (addition > 0) ? negativeDiffs : positiveDiffs;
            int numberOfNulls = MoveNullElements(diffs);
            Array.Sort(diffs, numberOfNulls, diffs.Length - numberOfNulls);
            int interations = DoublePal.RoundToIntUp(deltaAbs * 100);
            for (int i = 0; i < interations; i++)
            {
                int index = diffs.Length - (i + 1);
                DmlChartDummyValue diffVal = diffs[index];
                mPercentages[diffVal.Index] += addition;
            }
        }

        private static int MoveNullElements(DmlChartDummyValue[] array)
        {
            int nullCount = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    array[i] = array[nullCount];
                    array[nullCount] = null;
                    nullCount++;
                }
            }
            return nullCount;
        }

        /// <summary>
        /// Gets the number of digits after decimal separator from the specified format.
        /// </summary>
        /// <param name="format">The specified format</param>
        /// <returns>The precision</returns>
        private static int GetPrecision(string format)
        {
            int countPercentSymbol = 0;
            char separator = FormatterPal.GetDecimalSeparatorCurrent();
            // Take a number with the maximum number of decimal places.
            double baseValueForPecisionCalculation = 1d / 3;
            string formattedValue = FormatterPal.DoubleToStr(baseValueForPecisionCalculation, format);
            StringBuilder doubleValue = new StringBuilder();

            foreach (char chr in formattedValue)
            {
                // In the case of a "custom" format the percent symbol can be used several times.
                // The precision must be increased by 2 each time when the percent symbol is used.
                if (chr == '%')
                    countPercentSymbol++;

                // In the case of a "custom" format a non-digit chars can be used. Remove this chars.
                if (Char.IsDigit(chr) || chr == separator)
                    doubleValue.Append(chr);
            }

            formattedValue = doubleValue.ToString();
            int precision = countPercentSymbol * 2;
            int separatorIndex = formattedValue.IndexOf(separator);

            if (separatorIndex > 0)
                precision += formattedValue.Length - (separatorIndex + 1);

            // 15 is the maximum value for Math.Round method if a double type is used.
            return System.Math.Min(precision, 15);
        }

        /// <summary>
        /// Creates a copy of the original chart data value collection, if one has not already been created, so that the
        /// original data loaded from the document or defined by a customer will not be affected during a modification.
        /// </summary>
        private void PrepareForModification()
        {
            if (mValues == null)
                mValues = mOriginalValues.Clone();
        }

        /// <summary>
        /// Returns a data value by an index.
        /// </summary>
        internal DmlChartValue this[int index]
        {
            get { return Values[index]; }
        }

        /// <summary>
        /// Returns number of data values of a chart dimension including empty values.
        /// </summary>
        internal int ValueCount
        {
            get { return Values.ValueCount; }
            set
            {
                PrepareForModification();
                Values.ValueCount = value;
            }
        }

        /// <summary>
        /// Returns number of data values of a chart dimension including empty values.
        /// </summary>
        /// <remarks>
        /// WORDSNET-21034 The series of radar chart use the number of non-empty values. The original document contains
        /// 11 non-empty x-values and the index of the last non-empty value is 20. So, <see cref="LastNonEmptyValueIndex"/> can
        /// not be used.
        /// </remarks>
        internal int NonEmptyValueCount
        {
            get { return Values.NonEmptyValueCount; }
        }

        /// <summary>
        /// Gets index of the last non-empty value in this collection.
        /// </summary>
        internal int LastNonEmptyValueIndex
        {
            get { return Values.LastNonEmptyValueIndex; }
        }

        /// <summary>
        /// Indicates whether the collection contains non-empty values.
        /// </summary>
        internal bool HasNonEmptyValues
        {
            get { return Values.HasNonEmptyValues; }
        }

        /// <summary>
        /// Gets a format code used to convert data values to their string representation.
        /// </summary>
        internal string FormatCode
        {
            get
            {
                if (!StringUtil.HasChars(mFormatCode))
                    mFormatCode = DmlChartFormatCodeValidator.ValidateFormatCode(mOriginalValues.FormatCode);

                return mFormatCode;
            }
        }

        /// <summary>
        /// Gets value type of items of this collection.
        /// </summary>
        internal DmlChartValueType ValueType
        {
            get { return Values.ValueType; }
        }

        /// <summary>
        /// Returns <c>true</c> if values in the collection are dates.
        /// </summary>
        internal bool IsDate
        {
            get
            {
                DmlChartValue val = GetFirstNotNullValue();
                return (val != null) && val.IsDate;
            }
        }

        /// <summary>
        /// Indicates, whether the values were replaced by date.
        /// </summary>
        /// <remarks>
        /// When values were replaced by dates the data labels should use values with original indexes.
        /// </remarks>
        internal bool IsReplacedByDate { get; set; }

        /// <summary>
        /// Gets actual data of this instance.
        /// </summary>
        private DmlChartValueCollection Values
        {
            get { return (mValues != null) ? mValues : mOriginalValues; }
        }

        private string mFormatCode;
        private DmlChartValueCollection mOriginalValues;
        private DmlChartValueCollection mValues;
        private double[] mPercentages;
    }
}
