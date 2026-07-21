// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.55 errBars (Error Bars) element that specifies error bars.
    /// </summary>
    internal class DmlChartErrorBars : DmlExtensionListSource
    {
        /// <summary>
        /// Sets the parent series of this error bars. 
        /// Series is required to calculate error values in units of series values.
        /// </summary>
        internal void SetSeries(ChartSeries series)
        {
            mSeries = series;
        }

        internal DmlChartErrorBars Clone()
        {
            DmlChartErrorBars lhs = (DmlChartErrorBars)MemberwiseClone();
            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mPlusValues != null)
                lhs.mPlusValues = mPlusValues.Clone();

            if (mMinusValues != null)
                lhs.mMinusValues = mMinusValues.Clone();

            if (mAverageValue != null)
                lhs.mAverageValue = mAverageValue.Clone();

            lhs.Extensions = CloneExtensions();

            if (mMinus != null)
                lhs.mMinus = mMinus.Clone();

            if (mPlus != null)
                lhs.mPlus = mPlus.Clone();

            return lhs;
        }

        private void CalculateErrorValues()
        {
            // Depending on the error bar direction, use collection of X or Y values as a source collection
            // for calculating absolute error values in units of the corresponding series values.
            DmlChartDimensionData sourceCollection = GetChartDimensionData();

            // Stacked maximums are used to calculate errors for percent staked charts.
            double[] stackedMaximums = DmlChartRenderingUtil.GetStackedMaximums(mSeries.Chart);

            mPlusValues = CreateErrorChartDimensionData(sourceCollection);
            mMinusValues = CreateErrorChartDimensionData(sourceCollection);

            double error = GetFixedError(sourceCollection);

            // Calculate values of plus and minus errors.
            // WORDSNET-22213 Error bars should be rendered only for non-empty points.
            for (int i = 0; i <= mSeries.LastNonEmptyValueIndex; i++)
            {
                DmlChartValue sourceValue = sourceCollection.GetValue(i);

                // No error bar should be applied to null value, so simply do nothing.
                if (sourceValue == null)
                    continue;

                double plusAbsoluteValue = sourceValue.Value;
                double minusAbsoluteValue = sourceValue.Value;

                switch (ErrValType)
                {
                    case ErrorValueType.Percentage:
                    {
                        // It seems, MS Word always uses original value (not staked) for calculating percent error.
                        // So do the same.
                        error = CalculatePercentageError(sourceCollection.Data.GetOriginalValue(i), (float)Val);
                        double stackedError = GetStackedErrorValue(error, stackedMaximums[i]);
                        plusAbsoluteValue += stackedError;
                        minusAbsoluteValue -= stackedError;
                        break;
                    }
                    case ErrorValueType.CustomErrorBars:
                    {
                        DmlChartValue plusError = Plus.GetValue(sourceValue.Index);
                        DmlChartValue minusError = Minus.GetValue(sourceValue.Index);

                        double plusErrorValue = (plusError == null) ? 0.0d : plusError.Value;
                        double minusErrorValue = (minusError == null) ? 0.0d : minusError.Value;
                        plusAbsoluteValue += GetStackedErrorValue(plusErrorValue, stackedMaximums[i]);
                        minusAbsoluteValue -= GetStackedErrorValue(minusErrorValue, stackedMaximums[i]);
                        break;
                    }
                    case ErrorValueType.StandardDeviation:
                    {
                        plusAbsoluteValue = AverageValue.Value + error;
                        minusAbsoluteValue = AverageValue.Value - error;
                        break;
                    }
                    case ErrorValueType.FixedValue:
                    {
                        double stackedError = GetStackedErrorValue(error, stackedMaximums[i]);
                        plusAbsoluteValue += stackedError;
                        minusAbsoluteValue -= stackedError;
                        break;
                    }
                    case ErrorValueType.StandardError:
                    default:
                    {
                        plusAbsoluteValue += error;
                        minusAbsoluteValue -= error;
                        break;
                    }
                }

                mPlusValues.Data.AddValue(new DmlChartNumValue(sourceValue.Index, plusAbsoluteValue));
                mMinusValues.Data.AddValue(new DmlChartNumValue(sourceValue.Index, minusAbsoluteValue));
            }
        }

        /// <summary>
        /// Gets the <see cref="DmlChartDimensionData"/> based on error bar direction.
        /// </summary>
        /// <returns><see cref="DmlChartDimensionData"/></returns>
        private DmlChartDimensionData GetChartDimensionData()
        {
            return (ErrDir == ErrorBarDirection.X)
                ? GetXValuesSource()
                : mSeries.Y;
        }

        /// <summary>
        /// Gets the <see cref="DmlChartDimensionData"/> if value of the error bar direction is "X".
        /// </summary>
        /// <returns><see cref="DmlChartDimensionData"/></returns>
        private DmlChartDimensionData GetXValuesSource()
        {
            return mSeries.X.IsEmpty
                // WORDSNET-21470 If XValues is empty, MS Words uses the source with "dummy" values.
                ? DmlChartDimensionData.GetDummyDimensionData(mSeries.LastNonEmptyValueIndex + 1)
                : mSeries.X;
        }

        /// <summary>
        /// Gets the error value.
        /// </summary>
        /// <param name="error">The specified error</param>
        /// <param name="stackedMaximum">The specified stacked maximum</param>
        /// <returns>The calculated error</returns>
        private double GetStackedErrorValue(double error, double stackedMaximum)
        {
            return IsPercentStacked  ? (error / stackedMaximum) : error;
        }

        /// <summary>
        /// Calculates fixed error value, i.e. common error value for all data points.
        /// If error value depends on data point value, returns zero.
        /// </summary>
        private double GetFixedError(DmlChartDimensionData sourceCollection)
        {
            double error = 0;
            switch (ErrValType)
            {
                case ErrorValueType.StandardDeviation:
                    error = CalculateStandardDeviation(sourceCollection, AverageValue, Val);
                    break;
                case ErrorValueType.StandardError:
                    error = CalculateStandardError(sourceCollection, AverageValue, Val);
                    break;
                case ErrorValueType.FixedValue:
                    error = (float)Val;
                    break;
                case ErrorValueType.Percentage:
                case ErrorValueType.CustomErrorBars:
                default:
                    // Percentage and custom errors are specific for each concrete point, so they will be calculated later.
                    break;
            }
            return error;
        }

        /// <summary>
        /// Calculates average value of the given data source.
        /// </summary>
        private DmlChartValue CalculateAverageValue(DmlChartDimensionData sourceCollection)
        {
            double sum = 0;
            int notNullAndNotNanValuesCount = 0;

            for (int i = 0; i < sourceCollection.Data.ValueCount; i++)
            {
                DmlChartValue value = sourceCollection.GetValue(i);
                DmlChartValue x = mSeries.X.GetValue(i);
                DmlChartValue y = mSeries.Y.GetValue(i);
                
                //Calculate number of valid values.
                if (!DmlChartValue.IsNullOrNaN(x) && !DmlChartValue.IsNullOrNaN(y))
                    notNullAndNotNanValuesCount++;

                if (!DmlChartValue.IsNullOrNaN(value))
                    sum += value.Value;
            }

            return new DmlChartNumValue(0, (notNullAndNotNanValuesCount == 0) ? 0 : (sum / notNullAndNotNanValuesCount), 
                sourceCollection.Data.FormatCode);
        }

        /// <summary>
        /// Calculates standard deviation.
        /// http://en.wikipedia.org/wiki/Standard_deviation
        /// </summary>
        private static double CalculateStandardDeviation(
            DmlChartDimensionData sourceCollection, DmlChartValue avgValue, double value)
        {
            // Calculate sum of the population standard deviation, 
            // first compute the difference of each data point from the mean (avgPoint), 
            // and square the result of each, then sum.
            int n = sourceCollection.Data.ValueCount;
            double population = 0;
            for (int i = 0; i < n; i++)
            {
                DmlChartValue currentValue = sourceCollection.GetValue(i);
                if (!DmlChartValue.IsNullOrNaN(currentValue))
                    population += (float)System.Math.Pow((currentValue.Value - avgValue.Value), 2);
            }

            // Calculate a standard deviation as a sqrt of avg value of population.
            double deviation = System.Math.Sqrt(population / n);
            deviation *= (MathUtil.IsZero(value)) ? 1 : value;
            return deviation;
        }

        /// <summary>
        /// Calculates standard error.
        /// http://en.wikipedia.org/wiki/Standard_error
        /// </summary>
        private static double CalculateStandardError(DmlChartDimensionData sourceCollection, DmlChartValue avgValue,
            double value)
        {
            int n = sourceCollection.Data.ValueCount;
            double deviation = CalculateStandardDeviation(sourceCollection, avgValue, value);
            return deviation / System.Math.Sqrt(n);
        }

        /// <summary>
        /// Calculates percentage error.
        /// </summary>
        private static double CalculatePercentageError(DmlChartValue value, float errValue)
        {
            return (DmlChartValue.IsNullOrNaN(value)) ? 0.0d : (value.Value * errValue / 100);
        }

        /// <summary>
        /// Creates an empty copy of source.
        /// </summary>
        private static DmlChartDimensionData CreateErrorChartDimensionData(DmlChartDimensionData source)
        {
            DmlChartDataSource errorSource = new DmlChartDataSource();
            errorSource.Values = new DmlChartValueCollection(DmlChartValueType.Numeric);
            errorSource.Values.ValueCount = source.Data.ValueCount;
            errorSource.Values.FormatCode = source.Data.FormatCode;

            return new DmlChartDimensionData(errorSource);
        }

        internal ErrorBarType ErrBarType
        {
            get { return mErrBarType; }
            set { mErrBarType = value; }
        }

        /// <summary>
        /// By default returns Y.
        /// </summary>
        internal ErrorBarDirection ErrDir
        {
            get { return mErrDir; }
            set { mErrDir = value; }
        }

        internal ErrorValueType ErrValType
        {
            get { return mErrValType; }
            set { mErrValType = value; }
        }

        internal DmlChartDimensionData Minus
        {
            get
            {
                if (mMinus == null)
                    mMinus = new DmlChartDimensionData(new DmlChartDataSource());

                return mMinus;
            }
        }

        internal DmlChartDimensionData Plus
        {
            get
            {
                if (mPlus == null)
                    mPlus = new DmlChartDimensionData(new DmlChartDataSource());
             
                return mPlus;
            }
        }

        internal bool NoEndCap
        {
            get { return mNoEndCap; }
            set { mNoEndCap = value; }
        }

        internal DmlChartSpPr SpPr
        {
            get
            {
                if (mSpPr == null)
                    mSpPr = new DmlChartSpPr();

                return mSpPr;
            }
        }

        internal double Val
        {
            get { return mVal; }
            set { mVal = value; }
        }

        /// <summary>
        /// Returns absolute values of positive errors.
        /// Values are calculated.
        /// </summary>
        internal DmlChartDimensionData PlusValues
        {
            get
            {
                if (mPlusValues == null)
                    CalculateErrorValues();

                return mPlusValues;
            }
        }

        /// <summary>
        /// Returns absolute values of negative errors.
        /// Values are calculated.
        /// </summary>
        internal DmlChartDimensionData MinusValues
        {
            get
            {
                if (mMinusValues == null)
                    CalculateErrorValues();

                return mMinusValues;
            }
        }

        /// <summary>
        /// Returns average value of the source values.
        /// </summary>
        internal DmlChartValue AverageValue
        {
            get
            {
                if (mAverageValue == null)
                    mAverageValue = CalculateAverageValue((ErrDir == ErrorBarDirection.X) ? mSeries.X : mSeries.Y);

                return mAverageValue;
            }
        }

        /// <summary>
        /// Returns true if plus error bar should be rendered.
        /// </summary>
        internal bool RenderPlus
        {
            get { return ((ErrBarType == ErrorBarType.Both) || (ErrBarType == ErrorBarType.Plus)); }
        }

        /// <summary>
        /// Returns true if minus error bar should be rendered.
        /// </summary>
        internal bool RenderMinus
        {
            get { return ((ErrBarType == ErrorBarType.Both) || (ErrBarType == ErrorBarType.Minus)); }
        }

        /// <summary>
        /// Returns true if error bars value type is standard deviation. 
        /// In this case error bar must start at the average point.
        /// </summary>
        internal bool IsStandardDeviation
        {
            get { return (ErrValType == ErrorValueType.StandardDeviation); }
        }

        /// <summary>
        /// Gets the series to which these bars belongs.
        /// </summary>
        /// <remarks>Property exposed for test purposes.</remarks>
        internal ChartSeries Series
        {
            get { return mSeries; }
        }

        /// <summary>
        /// Returns true if parent chart is percent stacked.
        /// </summary>
        private bool IsPercentStacked
        {
            get { return (mSeries.Chart.ChartPr.Grouping == Grouping.PercentStacked); }
        }

        private ErrorBarType mErrBarType;
        private ErrorBarDirection mErrDir = ErrorBarDirection.Y;
        private ErrorValueType mErrValType;
        private DmlChartDimensionData mMinus;
        private DmlChartDimensionData mPlus;
        private bool mNoEndCap;
        private DmlChartSpPr mSpPr;
        private double mVal;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartSeries mSeries;
        private DmlChartDimensionData mMinusValues;
        private DmlChartDimensionData mPlusValues;
        private DmlChartValue mAverageValue;
    }
}
