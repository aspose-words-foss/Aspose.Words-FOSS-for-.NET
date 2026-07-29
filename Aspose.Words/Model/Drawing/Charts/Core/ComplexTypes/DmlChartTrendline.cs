// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2012 by Alexey Noskov

using System;
using System.Drawing;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.212 trendline (Trendlines) element.
    /// This element specifies a trendline.
    /// </summary>
    internal class DmlChartTrendline : DmlExtensionListSource
    {
       
        internal DmlChartTrendline Clone()
        {
            // It is not required to clone x and y values and points, they all are filled on demand using series values.
            DmlChartTrendline lhs = (DmlChartTrendline)MemberwiseClone();

            lhs.mTrendlinePr = mTrendlinePr.Clone();

            lhs.mXValues = null;
            lhs.mYValues = null;
            lhs.mPoints = null;

            return lhs;
        }

        /// <summary>
        /// Sets the series the trendline belongs to.
        /// </summary>
        internal void SetSeries(ChartSeries series)
        {
            mSeries = series;
        }

        /// <summary>
        /// Method calculates <see cref="XValues"/> and <see cref="YValues"/>.
        /// </summary>
        private void Prepare()
        {
            PrepareXValues();

            switch (TrendlineType)
            {
                case TrendlineType.Exponential:
                    PrepareExponential();
                    break;
                case TrendlineType.Logarithmic:
                    PrepareLogarithmic();
                    break;
                case TrendlineType.MovingAverage:
                    PrepareMovingAverage();
                    break;
                case TrendlineType.Linear:
                case TrendlineType.Polynomial:
                    PreparePolynomial();
                    break;
                case TrendlineType.Power:
                    PreparePower();
                    break;
                default:
                    throw new ArgumentException(UnexpectedTrendlineTypeMessage);
            }
        }

        /// <summary>
        /// Checks if x-values are the same.
        /// </summary>
        /// <remarks>
        /// WORDSNET-19634 It makes no sense to create a trend line if all X-values are the same.
        /// </remarks>
        /// <returns>"True" if float value of all X-values is the same, otherwise "false"</returns>
        private bool AreXValuesSame()
        {
            DmlChartValue prev = null;

            foreach (DmlChartValue value in mSeries.OrderedXValuesArray)
            {
                if ((prev != null) && !MathUtil.AreEqual(value.Value, prev.Value))
                    return false;

                prev = value;
            }

            return true;
        }

        /// <summary>
        /// Method fills <see cref="XValues"/> with x values taking forecast in account.
        /// </summary>
        private void PrepareXValues()
        {
            if (mXValues != null)
                return;

            mXValues = new DmlChartDimensionData(new DmlChartDataSource());
            mXValues.Values = new DmlChartValueCollection(XValueType);

            if (XValueType == DmlChartValueType.String || XValueType == DmlChartValueType.MultiLvlString)
                PrepareStringXValues();
            else if (mSeries.X.IsDate)
                PrepareDateXValues();
            else
                PrepareNumXValues();
        }

        /// <summary>
        /// Prepare x-values for numeric and date trendlines and returns the index of the last value.
        /// </summary>
        private int PrepareXValuesGetLastIndex(int startIndex)
        {
            int index = startIndex;
            string formatCode = mSeries.X.Data.FormatCode;

            for (int i = 0; i < OrderedSourceXValues.Length; i++)
            {
                DmlChartValue x = OrderedSourceXValues[i];

                if (DmlChartValue.IsNullOrNaN(x))
                    continue;

                DmlChartNumValue srcNum = OrderedSourceXValues[i] as DmlChartNumValue;
                string actualFormatCode = (srcNum == null) ? formatCode : srcNum.FormatCode;
                mXValues.Data.AddValue(new DmlChartNumValue(index, x.Value, actualFormatCode));
                index++;
            }

            return index;
        }

        /// <summary>
        /// Method fills <see cref="XValues"/> with numeric values.
        /// </summary>
        private void PrepareNumXValues()
        {
            // First calculate minimum and maximum series x values. 
            SetXValuesExtremums();

            // Calculate approximate step between x values.
            double step = (mMaxValueX - mMinValueX) / mSeries.ValueCount;

            // Subtract backward from actual minimum and add forward to actual maximum.
            // This way we get forecast minimum and maximum values.
            double forecastMin = mMinValueX - Backward;
            double forecastMax = mMaxValueX + Forward;

            // If trendline is Power or Logarithmic, minimum value cannot be less than zero.
            // If such situation occurs, add minimum absolute value to maximum value and reset minimum to zero.
            // It seems MS Word does the same.
            if ((TrendlineType == TrendlineType.Power || TrendlineType == TrendlineType.Logarithmic) && (forecastMin < 0))
            {
                forecastMax += System.Math.Abs(forecastMin);
                forecastMin = 0;
            }

            // Set start index to zero and current value to forecast minimum.
            int index = 0;
            double current = forecastMin;

            // Add backward forecast values if required.
            // WORDSNET-13836 If X step is zero (vertical line in scatter chart), 
            // it is impossible to process Forward and Backward. So simply ignore them.
            if (!MathUtil.IsZero(step))
            {
                while (current < mMinValueX)
                {
                    mXValues.Data.AddValue(new DmlChartDummyValue(index, (float)current));
                    index++;
                    current += step;
                }
            }

            index = PrepareXValuesGetLastIndex(index);

            // Add forward forecast values if required.
            // WORDSNET-13836 If X step is zero (vertical line in scatter chart), 
            // it is impossible to process Forward and Backward. So simply ignore them.
            if (!MathUtil.IsZero(step))
            {
                current = mMaxValueX;
                while ((current < forecastMax) || (!MathUtil.IsZero(Forward) && MathUtil.AreEqual(current, forecastMax)))
                {
                    mXValues.Data.AddValue(new DmlChartDummyValue(index, (float)current));
                    index++;
                    current += step;
                }
            }

            mXValues.Data.ValueCount = index;
        }

        /// <summary>
        /// Sets the maximum and minimum of the X-values.
        /// </summary>
        private void SetXValuesExtremums()
        {
            for (int i = 0; i < mSeries.ValueCount; i++)
            {
                DmlChartValue x = mSeries.X.GetValue(i);

                if (DmlChartValue.IsNullOrNaN(x))
                    continue;

                mMinValueX = System.Math.Min(mMinValueX, x.Value);
                mMaxValueX = System.Math.Max(mMaxValueX, x.Value);
            }
        }

        /// <summary>
        /// Method fills <see cref="XValues"/> with numeric values, which represent dates.
        /// </summary>
        private void PrepareDateXValues()
        {
            if (!mSeries.X.IsDate)
                throw new ArgumentException("Date values are expected.");

            SetXValuesExtremums();
            // WORDSNET-21968 Use the number of non-empty points.
            int index = PrepareXValuesGetLastIndex(0);
            string formatCode = mSeries.X.Data.FormatCode;

            // Only forward is taken in account, MS Word does not allow to specify backward for dates.
            mXValues.Data.ValueCount = (index + MathUtil.DoubleToInt(Forward));

            double step = (mMaxValueX - mMinValueX) / index;

            for (int i = index; i < mXValues.Data.ValueCount; i++)
            {
                mMaxValueX += step;
                mXValues.Data.AddValue(new DmlChartNumValue(i, mMaxValueX, formatCode));
            }
        }

        /// <summary>
        /// Method fills <see cref="XValues"/> with string values.
        /// </summary>
        private void PrepareStringXValues()
        {
            if(XValueType != DmlChartValueType.String && XValueType != DmlChartValueType.MultiLvlString)
                throw new ArgumentException("String value type is expected.");

            // Copy original values into x values collection.
            for (int i = 0; i < mSeries.ValueCount; i++)
                mXValues.Data.AddValue(mSeries.X.GetValue(i).Clone());

            // Increase number of points if forecast is specified.
            // In this case if there is no value for specified index, data source will return dummy value.
            // Backward is not taken in account here (Ms Word does not allow to specify backward for string values axis).
            mXValues.Data.ValueCount = mSeries.ValueCount + MathUtil.DoubleToInt(Forward);
        }
        
        /// <summary>
        /// Gets points that are used in trendline calculation.
        /// </summary>
        private void PreparePoints()
        {
            PointFList points = new PointFList();
            float prevX = float.NaN;
            float prevY = float.NaN;
            int countIdenticalX = 1;

            for (int i = 0; i < OrderedSourceXValues.Length; i++)
            {
                // In case of date X, we use index as a value. It seems MS Word does the same.
                // This will allow us to get correct equation of trendline.
                DmlChartValue x = OrderedSourceXValues[i];

                if (DmlChartValue.IsNullOrNaN(x))
                    continue;

                DmlChartValue y = mSeries.Y.GetValue(x.Index);

                if (DmlChartValue.IsNullOrNaN(y))
                    continue;

                float xValue = XValues.IsDate ? (i + 1) : x.FloatValue;
                float yValue = y.FloatValue;
                PointF point;

                switch (TrendlineType)
                {
                    case TrendlineType.Exponential:
                        // We linearize y=a*e^(b*x) equation and get ln(y)=ln(a) + b*x equation.
                        // Then find ln(a) and b coefficients the same way as we do for linear trendline.
                        point = new PointF(xValue, (float)System.Math.Log(yValue));
                        break;
                    case TrendlineType.Logarithmic:
                        // We can consider equation y=a*ln(x)+b as linear function y=f(ln(x)).
                        point = new PointF((float)System.Math.Log(xValue), yValue);
                        break;
                    case TrendlineType.Linear:
                    case TrendlineType.Polynomial:
                    case TrendlineType.MovingAverage:
                        if (prevX == xValue)
                        {
                            prevY += yValue;
                            countIdenticalX++;
                        }
                        else
                        {
                            if (countIdenticalX > 1)
                                SetAverageYForIdenticalXLinear(points, prevX, (prevY / countIdenticalX));

                            countIdenticalX = 1;
                            prevY = yValue;
                        }

                        point = new PointF(xValue, yValue);
                        break;
                    case TrendlineType.Power:
                        // Power trendline is represented by y=a*x^b equation. It can be linearized as ln(y)=ln(a)+b*ln(x)
                        point = new PointF((float)System.Math.Log(xValue), (float)System.Math.Log(yValue));
                        break;
                    default:
                        throw new ArgumentException(UnexpectedTrendlineTypeMessage);
                }

                prevX = xValue;
                points.Add(point);
            }

            mPreparedPoints = points;
        }

        /// <summary>
        /// Sets average y-values for identical x-values.
        /// </summary>
        /// <remarks>
        /// WORDSNET-24626 If series contains several identical x-values MS Word uses average y-value for all such x.
        /// </remarks>
        private static void SetAverageYForIdenticalXLinear(PointFList points, float x, float averageY)
        {
            for (int i = 0; i < points.Count; i++)
                if (MathUtil.AreEqual(points[i].X, x))
                     points[i] = new PointF(x, averageY);
        }

        /// <summary>
        /// Calculates trendline's control points and <see cref="YValues"/> of Polynomial or Linear trendline.
        /// </summary>
        private void PreparePolynomial()
        {
            int order = Order < PreparedPoints.Count ? Order : System.Math.Max(2, PreparedPoints.Count - 1);

            // Order of linear trendline is 1.
            if (TrendlineType == TrendlineType.Linear)
                order = 1;

            mCoefficients = CalculateCoeff(PreparedPoints, order + 1, Intercept);
            PrepareTrendlineCore(order);
        }

        /// <summary>
        /// Calculates trendline's control points and <see cref="YValues"/> of Power trendline.
        /// </summary>
        private void PreparePower()
        {
            PrepareLogarithmic();
        }

        /// <summary>
        /// Calculates trendline's control points and <see cref="YValues"/> of Logarithmic trendline.
        /// </summary>
        private void PrepareLogarithmic()
        {
            mCoefficients = CalculateCoeff(PreparedPoints, 2);
            PrepareTrendlineCore(0);
        }

        /// <summary>
        /// Calculates trendline's control points and <see cref="YValues"/> of Exponential trendline.
        /// </summary>
        private void PrepareExponential()
        {
            mCoefficients = EmptyMatrix(2, 1);

            // If intercept is set to zero, we do not calculate coefficients, because they should be set to zero.
            // We process this situation later in PrepareTrendlineCore method.
            if (!MathUtil.IsZero(Intercept))
            {
                // Use logarithm of intercept for further calculation.
                double intercept = double.IsNaN(Intercept) ? double.NaN : System.Math.Log(Intercept);
                mCoefficients = CalculateCoeff(PreparedPoints, 2, intercept);
            }

            PrepareTrendlineCore(0);
        }

        /// <summary>
        /// Prepares Moving Average trendline. Since this type of trendline does not support forecast and
        /// its values cannot exit real series value limits, we do nothing here except setting actual series values as 
        /// trendline's X and Y values. 
        /// </summary>
        private void PrepareMovingAverage()
        {
            // WORDSNET-13140 Do not reset calculated X values with values from series.
            // Values from series might be empty, that causes the exception.
            mYValues = mSeries.Y;
            mPoints = PreparedPoints;
        }

        /// <summary>
        /// Core method for calculation trendline's control points and <see cref="YValues"/>.
        /// </summary>
        private void PrepareTrendlineCore(int order)
        {
            // Lets take one more X point between known X points.
            int pointCount = mXValues.Data.ValueCount * 2 - 1;

            // Init y values source. These values will be used for adjusting axis.
            mYValues = new DmlChartDimensionData(new DmlChartDataSource());
            mYValues.Values = new DmlChartValueCollection(DmlChartValueType.Numeric);
            mYValues.Values.ValueCount = pointCount;

            bool isDate = mXValues.IsDate;
            mPoints = new PointFList(pointCount);

            // WORDSNET-17004 The values of the trend line can be unsorted. But for rendering of the trend line, the values 
            // should be sorted in ascending order.
            DmlChartValue[] sortedXvalues = mXValues.Data.ToArray();
            Array.Sort(sortedXvalues);

            for (int i = 0; i < sortedXvalues.Length; i++)
            {
                DmlChartValue x1 = sortedXvalues[i];
   
                if (DmlChartValue.IsNullOrNaN(x1))
                    continue;
                
                // If value is date, we use index instead of actual float value.
                float x = isDate ? (i + 1) : x1.FloatValue;
                AddPoint(x, mCoefficients, order);

                if (i < sortedXvalues.Length - 1)
                {
                    DmlChartValue x2 = sortedXvalues[i + 1];
                    // If value is date, we use index instead of actual float value.
                    x = isDate ? (i + 3.0f / 2.0f) : (x1.FloatValue + x2.FloatValue) / 2;
                    AddPoint(x, mCoefficients, order);
                }
            }
        }

        private void AddPoint(float x, double[][] coeffs, int order)
        {
            float y = CalculateYValue(x, coeffs, order);
            mYValues.Data.AddValue(new DmlChartNumValue(mPoints.Count, y));
            mPoints.Add(new PointF(x, y));
        }

        /// <summary>
        /// Method calculates Y value by given X, coefficients and order values.
        /// </summary>
        private float CalculateYValue(float x, double[][] coeffs, int order)
        {
            switch (TrendlineType)
            {
                case TrendlineType.Linear:
                case TrendlineType.Polynomial:
                {
                    float y = 0;
                    for (int degree = 0; degree < order + 1; degree++)
                        y += (float)(coeffs[degree][0] * System.Math.Pow(x, degree));
                    return y;
                }
                case TrendlineType.Logarithmic:
                {
                    double a = coeffs[1][0];
                    double b = coeffs[0][0];
                    // If X is zero Log(0)->-Infinity, we cannot use this value for further calculation.
                    // 0.9b is an experimental value, which gives the result close to MS.
                    return MathUtil.IsZero(x) ? (float)b * 0.9f : (float)(a * System.Math.Log(x) + b);
                }
                case TrendlineType.Exponential:
                {
                    // If intercept is set to zero, set both coefficients to zero,
                    // because we cannot get logarithm of such intercept for further calculation. MS Word does the same.
                    double a = MathUtil.IsZero(Intercept) ? 0.0f : System.Math.Exp(coeffs[0][0]);
                    double b = MathUtil.IsZero(Intercept) ? 0.0f : coeffs[1][0];

                    return (float)(a * System.Math.Exp(b * x));
                }
                case TrendlineType.Power:
                {
                    double a = System.Math.Exp(coeffs[0][0]);
                    double b = coeffs[1][0];
                    // If X is zero, x^b = 0, but MS seems to use 1.2a value. so do the same.
                    return MathUtil.IsZero(x) ? (float)a * 1.2f : (float)(a * System.Math.Pow(x, b));
                }
                default:
                    throw new ArgumentException(UnexpectedTrendlineTypeMessage);
            }
        }

        /// <summary>
        /// Calculates coefficients for polynomial trendline of the specified degree taking intercept value into account.
        /// Intercept value should be taken into account only upon rendering Linear, Polynomial or Exponential trendlines.
        /// For other trendlines use <see cref="CalculateCoeff(PointFList, int)"/>
        /// </summary>
        private static double[][] CalculateCoeff(PointFList points, int degree, double intercept)
        {
            double[][] coeffs = CalculateCoeff(points, degree);

            if (double.IsNaN(intercept))
                return coeffs;

            double b = coeffs[0][0];

            // We cannot calculate coefficients with 100% precision. Use 0.001% of y values range as tolerance.
            double range = CalculateRangeY(points);
            double precision = 1e+5d;
            double tolerance = CalculateRangeY(points) / precision;

            // WORDSNET-24353 Provide the epsilon as minimum precision (if series is a horizontal line - delta y is zero).
            tolerance = System.Math.Max(double.Epsilon, tolerance);

            // Add intercept point.
            points.Add(new PointF(0, (float)intercept));
            int iterationsCount = 0;

            // WORDSNET-26954 In some cases the coefficients do not change. To avoid hanging set the maximum number of
            // iterations.
            while (!MathUtil.AreEqual(b, intercept, tolerance) && (iterationsCount <= precision))
            {
                // Do not include intercept point.
                for (int i = 0; i < (points.Count - 1); i++)
                {
                    float x = points[i].X;
                    float y = 0;

                    for (int d = 0; d < degree; d++)
                        y += (float)(coeffs[d][0] * System.Math.Pow(x, d));

                    points[i] = new PointF(x, y);
                }

                // Use trendline points plus (0, intercept) point for the next iteration.
                coeffs = CalculateCoeff(points, degree);
                b = coeffs[0][0];
                iterationsCount++;
            }

            // Reset first coefficient to intercept value. This is not required actually, 
            // but this will make trendline's formula closer to MS.
            coeffs[0][0] = intercept;

            return coeffs;
        }

        /// <summary>
        /// Calculates y range.
        /// </summary>
        private static double CalculateRangeY(PointFList points)
        {
            double maxY = 0;
            double minY = 0;
            for (int i = 0; i < points.Count; i++)
            {
                PointF point = points[i];
                maxY = System.Math.Max(point.Y, maxY);
                minY = System.Math.Min(point.Y, minY);
            }

            double deltaY = maxY - minY;
            return deltaY;
        }

        /// <summary>
        /// Calculates coefficients for polynomial trendline of the specified degree.
        /// Uses algorithm described here http://charlway.blogspot.com/2012/08/polynomial-regression-in-python.html.
        /// </summary>
        private static double[][] CalculateCoeff(PointFList points, int degree)
        {
            double[][] x = MakeXmatrix(points, degree);
            double[][] y = MakeYmatrix(points);
            double[][] xt = TransposeMatrix(x); //Transpose X Matrix
            double[][] xtx = MultiplyMatrices(xt, x); //Multiply X' by X
            double[][] xtInvert = GausJ(xtx); //Invert
            double[][] xty = MultiplyMatrices(xt, y); //Times together to get X' x Y
            double[][] coeffMatrix = MultiplyMatrices(xtInvert, xty); //Here we get the coefficients of the polynomial
            return coeffMatrix;
        }

        /// <summary>
        /// Fill up the Xmatrix
        /// </summary>
        private static double[][] MakeXmatrix(PointFList points, int degree)
        {
            double[][] xMatrix = EmptyMatrix(points.Count, degree);
            for (int row = 0; row < points.Count; row++)
            {
                // The new Xmatrix elements are the X point to the power of the column value.
                // powers = 0,1,2, ... ,amount of x points - 1
                for (int col = 0; col < degree; col++)
                    xMatrix[row][col] = System.Math.Pow(points[row].X, col);
            }

            return xMatrix;
        }

        /// <summary>
        /// Making the Y column matrix
        /// </summary>
        private static double[][] MakeYmatrix(PointFList points)
        {
            double[][] yMatrix = EmptyMatrix(points.Count, 1);
            for (int row = 0; row < points.Count; row++)
                yMatrix[row][0] = points[row].Y;

            return yMatrix;
        }

        /// <summary>
        /// Create a matrix of zeros This is Rows by Columns
        /// </summary>
        private static double[][] EmptyMatrix(int m, int n)
        {
            double[][] array = new double[m][];
            for (int i = 0; i < m; i++)
                array[i] = new double[n];

            return array;
        }

        /// <summary>
        /// Creates a square identity matrix of size m x m
        /// </summary>
        private static double[][] IdentityMatrix(int m)
        {
            int n = m;
            double[][] result = EmptyMatrix(m, n);

            for (int i = 0; i < m; i++)
                result[i][i] = 1;

            return result;
        }

        /// <summary>
        /// Transposes source matrix.
        /// </summary>
        private static double[][] TransposeMatrix(double[][] sourceMatrix)
        {
            double[][] result = EmptyMatrix(sourceMatrix[0].Length, sourceMatrix.Length);

            for (int i = 0; i < sourceMatrix.Length; i++)
            {
                for (int j = 0; j < sourceMatrix[0].Length; j++)
                    result[j][i] = sourceMatrix[i][j];
            }

            return result;
        }

        /// <summary>
        /// Multiplies matrices.
        /// </summary>
        private static double[][] MultiplyMatrices(double[][] a, double[][] b)
        {
            double[][] result = EmptyMatrix(a.Length, b[0].Length);

            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[0].Length; j++)
                    for (int k = 0; k < b.Length; k++)
                        result[i][j] += a[i][k] * b[k][j];

            return result;
        }

        private static double[][] GausJ(double[][] matrix)
        {
            // Build Identity matrix to do row operations on, this will become inverted matrix 
            double[][] result = IdentityMatrix(matrix.Length);

            for (int r = 0; r < matrix.Length; r++)
            {
                // Divide each element on the diagonal by itself, 
                // making the new diagonal elements all = 1, so we never divide by 0.
                double divider = 0;

                // While our divider is 0, cant divide by 0.
                while (MathUtil.IsZero(divider))
                {
                    // Divider is now equals to element r,r in original matrix 
                    divider = matrix[r][r];
                    // In case the element r is 0, so we don't divide by 0
                    if (MathUtil.IsZero(divider))
                    {
                        for (int t = 0; t < matrix.Length; t++)
                        {
                            if (MathUtil.IsZero(matrix[r][r]))
                            {
                                // We add rows till we don't have 0 elements
                                for (int elem = 0; elem < matrix.Length; elem++)
                                    matrix[r][elem] = matrix[r][elem] + matrix[t][elem];
                            }
                        }
                    }
                }

                for (int n = 0; n < matrix.Length; n++)
                {
                    // Divide element by divider
                    matrix[r][n] = (matrix[r][n]) / divider;
                    // Do the same for the identity matrix that's getting transformed, output matrix
                    result[r][n] = (result[r][n]) / divider;
                }

                for (int row = 0; row < matrix.Length; row++)
                {
                    if (row != r)
                    {
                        double multi = matrix[row][r] / matrix[r][r];
                        for (int col = 0; col < matrix[0].Length; col++)
                        {
                            // Subtract columns
                            matrix[row][col] = matrix[row][col] - (multi * matrix[r][col]);
                            //Subtract columns for new matrix as well
                            result[row][col] = result[row][col] - (multi * result[r][col]);
                        }
                    }
                }
            }

            return result;
        }

        public override StringToObjDictionary<DmlExtension> Extensions
        {
            get { return (StringToObjDictionary<DmlExtension>)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Extensions); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Extensions, value); }
        }

        internal double Backward
        {
            get { return (double)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Backward); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Backward, value); }
        }

        internal bool DispEq
        {
            get { return (bool)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.DispEq); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.DispEq, value); }
        }

        internal bool DispRSqr
        {
            get { return (bool)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.DispRSqr); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.DispRSqr, value); }
        }

        internal double Forward
        {
            get { return (double)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Forward); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Forward, value); }
        }

        /// <summary>
        /// Returns NaN if intercept is not set.
        /// </summary>
        internal double Intercept
        {
            get { return (double)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Intercept); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Intercept, value); }
        }

        internal string Name
        {
            get { return (string)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Name); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Name, value); }
        }

        /// <summary>
        /// Allowed values are [2;6], zero means that property is not set.
        /// </summary>
        internal int Order
        {
            get { return (int)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Order); }
            set
            {
                if (value >= 2 && value <= 6)
                    mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Order, value);
            }
        }

        /// <summary>
        /// Allowed values are [2;255], zero means that property is not set.
        /// </summary>
        internal int Period
        {

            get { return (int)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.Period); }
            set
            {
                if (value >= 2 && value <= 255)
                    mTrendlinePr.SetProperty(DmlChartTrendlineAttr.Period, value);
            }
        }

        internal DmlChartSpPr SpPr
        {
            get
            {
                if (mTrendlinePr.GetProperty(DmlChartTrendlineAttr.SpPr) == null)
                    mTrendlinePr.SetProperty(DmlChartTrendlineAttr.SpPr, new DmlChartSpPr());

                return (DmlChartSpPr)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.SpPr);
            }
        }

        internal DmlChartTrendlineLabel TrendlineLbl
        {
            get { return (DmlChartTrendlineLabel)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.TrendlineLbl); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.TrendlineLbl, value); }
        }

        internal TrendlineType TrendlineType
        {
            get { return (TrendlineType)mTrendlinePr.GetProperty(DmlChartTrendlineAttr.TrendlineType); }
            set { mTrendlinePr.SetProperty(DmlChartTrendlineAttr.TrendlineType, value); }
        }

        internal DmlChartTrendlinePr TrendlinePr
        {
            get { return mTrendlinePr; }
        }

        /// <summary>
        /// Returns prepared points based on the type of trendline.
        /// </summary>
        private PointFList PreparedPoints
        {
            get
            {
                if (mPreparedPoints == null)
                    PreparePoints();

                return mPreparedPoints;
            }
        }

        /// <summary>
        /// Returns X values of the trendline. Use these values to recalculate axis limits.
        /// </summary>
        internal DmlChartDimensionData XValues
        {
            get
            {
                // Init x values on demand.
                if (mXValues == null)
                    Prepare();

                return mXValues;
            }
        }

        /// <summary>
        /// Returns Y values of the trendline. Use these values to recalculate axis limits.
        /// </summary>
        internal DmlChartDimensionData YValues
        {
            get
            {
                // Init y values on demand.
                if (mYValues == null)
                    Prepare();

                return mYValues;
            }
        }

        /// <summary>
        /// Returns array of trendline control points in axis units.
        /// </summary>
        internal PointFList Points
        {
            get
            {
                if (mPoints == null)
                    Prepare();
                
                return mPoints;
            }
        }

        /// <summary>
        /// Returns true if X values are valid.
        /// </summary>
        internal bool IsValid
        {
            get
            {
                // MS Word does not render Power and Logarithmic trendlines for values less than or equals zero (invalid values.)
                // It makes no sense to create a trend line if all X-values are the same or the number of values is less than 2
                // or if Y-values are empty.
                return (OrderedSourceXValues.Length > 1) && (mSeries.Y.Data.LastNonEmptyValueIndex > 0) && 
                       !AreXValuesSame() && 
                       ((TrendlineType != TrendlineType.Power) && (TrendlineType != TrendlineType.Logarithmic) || 
                       (OrderedSourceXValues[0].FloatValue > 0));
            }
        }

        /// <summary>
        /// Returns an array of coefficients used to calculate trendline values.
        /// </summary>
        internal double[][] Coefficients
        {
            get { return mCoefficients; }
        }

        /// <summary>
        /// Returns <see cref="ChartSeries"/> for which the trendline is rendered.
        /// </summary>
        internal ChartSeries Series
        {
            get { return mSeries; }
        }

        /// <summary>
        /// Returns value type of series' x values.
        /// </summary>
        private DmlChartValueType XValueType
        {
            get { return (mSeries.X.Data == null) ? DmlChartValueType.String : mSeries.X.Data.ValueType; }
        }

        /// <summary>
        /// Returns sorted X values. 
        /// </summary>
        private DmlChartValue[] OrderedSourceXValues
        {
            get { return  mSeries.OrderedXValuesArray; }
        }

        private double[][] mCoefficients;
        private DmlChartTrendlinePr mTrendlinePr = new DmlChartTrendlinePr();
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private ChartSeries mSeries;
        private DmlChartDimensionData mXValues;
        private DmlChartDimensionData mYValues;
        private PointFList mPreparedPoints;
        private PointFList mPoints;
        private double mMinValueX = double.MaxValue;
        private double mMaxValueX = double.MinValue;
        private const string UnexpectedTrendlineTypeMessage = "Unexpected TrendlineType";
    }
}
