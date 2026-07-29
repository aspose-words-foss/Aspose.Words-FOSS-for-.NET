// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2006 by Dmitry Vorobyev

using System;
using System.Drawing;

namespace Aspose
{
    /// <summary>
    /// Provides various mathematical functions missed from the System.Math class.
    /// </summary>
    public static class MathUtil
    {
        /// <summary>
        /// Simple square function.
        /// </summary>
        public static float Sqr(float f)
        {
            return f * f;
        }

        /// <summary>
        /// Simple square function.
        /// </summary>
        public static double Sqr(double d)
        {
            return d * d;
        }

        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        public static double RadiansToDegrees(double radians)
        {
            return (180 / Math.PI) * radians;
        }

        /// <summary>
        /// Normalizes specified angle so it becomes greater or equal to zero and less then 360 degrees.
        /// </summary>
        /// <param name="angle">Source angle.</param>
        public static double NormalizeAngle(double angle)
        {
            if (Math.Abs(angle) >= 360)
                angle %= 360;

            if (angle < 0)
                angle += 360;

            return angle;
        }

        /// <summary>
        /// Normalizes specified angle so it becomes greater or equal to zero and less then 360 degrees.
        /// </summary>
        /// <param name="angle">Source angle.</param>
        public static double NormalizeAngleRadians(double angle)
        {
            return DegreesToRadians(NormalizeAngle(RadiansToDegrees(angle)));
        }

        /// <summary>
        /// Checks if the specified angle is zero.
        /// </summary>
        public static bool IsZeroAngle(double angle)
        {
            return MathUtil.IsZero(MathUtil.NormalizeAngle(angle));
        }


        /// <summary>
        /// Fits a value to a specified range.
        /// </summary>
        public static double FitToRange(double value, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        /// <summary>
        /// Fits a value to a specified range.
        /// </summary>
        public static float FitToRange(float value, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        /// <summary>
        /// Fits a value to a specified range.
        /// </summary>
        public static int FitToRange(int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        /// <summary>
        /// Returns true, if a specified value fits in a specified range.
        /// </summary>
        public static bool IsInRange(int value, int minValue, int maxValue)
        {
            if (minValue > maxValue)
                return ((value <= minValue) && (value >= maxValue));

            return ((value >= minValue) && (value <= maxValue));
        }

        /// <summary>
        /// Multiplies a square matrix and a vector.
        /// </summary>
        public static double[] MatrixMultiply(double[] squareMatrix, double[] vector)
        {
            int length = vector.Length;
            Debug.Assert(length * length == squareMatrix.Length);

            double[] result = new double[length];
            for (int i = 0; i < squareMatrix.Length; i++)
                result[i / length] += squareMatrix[i] * vector[i % length];
            return result;
        }

        /// <summary>
        /// Rounds a double value and casts it to int in a way that will produce the same result on .NET and Java.
        /// </summary>
        public static int DoubleToInt(double value)
        {
#if JAVA || NETSTANDARD
            // WORDSJAVA-2352, WORDSJAVA-2641- '(int)double.PositiveInfinity' returns int.MinValue on .Net.
            // '(int)double.MaxValue', '(int)double.NaN' - the same.
            if (double.IsInfinity(value) || double.IsNaN(value) || value == double.MaxValue)
                return int.MinValue;

#endif
            return (int)Math.Round(value);
        }

        /// <summary>
        /// Rounds a double value and casts it to short in a way that will produce the same result on .NET and Java.
        /// </summary>
        public static short DoubleToShort(double value)
        {
            return (short)((ushort)Math.Round(value));
        }

        /// <summary>
        /// Casts "Int32" numeric value to "Int16", possible overflows are ignored.
        /// </summary>
        /// <returns>
        /// "Int32" value, which was fit to the "Int16" values range.
        /// </returns>
        public static int CastIntToShort(int value)
        {
            const int shortCapacity = ushort.MaxValue + 1;

            return (value % shortCapacity > short.MaxValue) ?
               -(shortCapacity - value % shortCapacity) : value % shortCapacity;
        }

        /// <summary>
        /// Truncates a double value and casts it to int.
        /// </summary>
        public static int Truncate(double value)
        {
            return (int)(value);
        }

        /// <summary>
        /// Return true if Double value is zero.
        /// </summary>
        /// <param name="value">Double value</param>
        public static bool IsZero(double value)
        {
            return (Math.Abs(value) < Double.Epsilon);
        }

        /// <summary>
        /// Return true if float value is zero.
        /// </summary>
        /// <param name="value">Float value</param>
        public static bool IsZero(float value)
        {
            return (Math.Abs(value) < float.Epsilon);
        }

        /// <summary>
        /// Return true if integer value is zero.
        /// </summary>
        /// <dev>fix C++ ambiguity</dev>
        /// <param name="value">Float value</param>
        public static bool IsZero(int value)
        {
            return value == 0;
        }

        /// <summary>
        /// Return true if all values in the Double array are zeros.
        /// </summary>
        /// <param name="values">Double values array</param>
        public static bool IsZero(params double[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (!IsZero(values[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Normalizes a zero value by removing the minus sign from negative zero (i.e. -0.0).
        /// If the input value is zero, it returns positive zero (0.0). Otherwise, it returns the original value.
        /// </summary>
        /// <param name="value">The input double value to be normalized.</param>
        /// <returns>A double value with the minus sign removed if it is zero, otherwise the original value.</returns>
        public static double NormalizeZero(double value)
        {
            // According to IEEE 754, −0 and +0 compare as equal.
            return (value == 0.0) ? 0.0 : value;
        }

        /// <summary>
        /// Normalizes a zero value by removing the minus sign from negative zero (i.e. -0.0f).
        /// If the input value is zero, it returns positive zero (0.0f). Otherwise, it returns the original value.
        /// </summary>
        /// <param name="value">The input float value to be normalized.</param>
        /// <returns>A float value with the minus sign removed if it is zero, otherwise the original value.</returns>
        public static float NormalizeZero(float value)
        {
            // According to IEEE 754, −0 and +0 compare as equal.
            return (value == 0.0f) ? 0.0f : value;
        }

        /// <summary>
        /// Return true if Double value is <c>double.MinValue</c>.
        /// </summary>
        /// <param name="value">Double value</param>
        public static bool IsMinValue(double value)
        {
            return AreEqual(value, Double.MinValue);
        }

        /// <summary>
        /// Checks whether two double values are 'almost' equal.
        /// Values are considered equal if absolute difference between values is less than 1e-10.
        /// </summary>
        public static bool AreEqual(double value1, double value2)
        {
            const double tolerance = 1e-10;
            return AreEqual(value1, value2, tolerance);
        }

        /// <summary>
        /// Checks whether two double values are 'almost' equal.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <param name="tolerance">The maximum acceptable difference, when these values are considered as equal.</param>
        /// <returns>"True" if the difference between values is less than tolerance, otherwise - "false"</returns>
        public static bool AreEqual(double value1, double value2, double tolerance)
        {
            return (Math.Abs(value1 - value2) < Math.Abs(tolerance));
        }

        /// <summary>
        /// Checks whether two float values are 'almost' equal.
        /// Values are considered equal if absolute difference between values is less than 1e-10.
        /// </summary>
        public static bool AreEqual(float value1, float value2)
        {
            const float tolerance = 1e-10f;
            return AreEqual(value1, value2, tolerance);
        }

        /// <summary>
        /// Checks whether two float values are 'almost' equal.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <param name="tolerance">The maximum acceptable difference, when these values are considered as equal.</param>
        /// <returns>"True" if the difference between values is less than tolerance, otherwise - "false"</returns>
        public static bool AreEqual(float value1, float value2, float tolerance)
        {
            return (Math.Abs(value1 - value2) < Math.Abs(tolerance));
        }

        /// <summary>
        /// Checks whether two points are 'almost' equal.
        /// </summary>
        public static bool AreEqual(PointF point1, PointF point2)
        {
            return AreEqual(point1.X, point2.X) && AreEqual(point1.Y, point2.Y);
        }

        /// <summary>
        /// Checks whether two rectangles are 'almost' equal.
        /// </summary>
        public static bool AreEqual(RectangleF r1, RectangleF r2)
        {
            return AreEqual(r1.X, r2.X) && AreEqual(r1.Y, r2.Y) && AreEqual(r1.Width, r2.Width) && AreEqual(r1.Height, r2.Height);
        }

        /// <summary>
        /// Checks whether the left-hand operand is less than or equal to the right-hand operand.
        /// </summary>
        /// <remarks>
        /// A tolerance must be used when comparing two values, because the comparison of two numbers might change between
        /// versions of the .NET Framework (the precision of the numbers' internal representation might change) (see Test22038).
        /// </remarks>
        /// <param name="leftHandOperand">The float value representing the left-hand operand</param>
        /// <param name="rightHandOperand">The float value representing the right-hand operand</param>
        /// <returns>"True" if the left value is less than or equal to the right value, otherwise - "false"</returns>
        public static bool IsLessOrEqual(float leftHandOperand, float rightHandOperand)
        {
            return (leftHandOperand < rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand);
        }

        /// <summary>
        /// Checks whether the left-hand operand is less than or equal within the given tolerance to the right-hand operand.
        /// </summary>
        public static bool IsLessOrEqual(float leftHandOperand, float rightHandOperand, float tolerance)
        {
            return (leftHandOperand < rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand, tolerance);
        }

        /// <summary>
        /// Checks whether the left-hand operand is less than or equal to the right-hand operand.
        /// </summary>
        /// <param name="leftHandOperand">The double value representing the left-hand operand</param>
        /// <param name="rightHandOperand">The double value representing the right-hand operand</param>
        /// <returns>"True" if the left value is less than or equal to the right value, otherwise - "false"</returns>
        public static bool IsLessOrEqual(double leftHandOperand, double rightHandOperand)
        {
            return (leftHandOperand < rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand);
        }

        /// <summary>
        /// Checks whether the left-hand operand is less than or equal within the given tolerance to the right-hand operand.
        /// </summary>
        public static bool IsLessOrEqual(double leftHandOperand, double rightHandOperand, double tolerance)
        {
            return (leftHandOperand < rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand, tolerance);
        }

        /// <summary>
        /// Checks whether the left-hand operand is greater than or equal to the right-hand operand.
        /// </summary>
        /// <param name="leftHandOperand">The float value representing the left-hand operand</param>
        /// <param name="rightHandOperand">The float value representing the right-hand operand</param>
        /// <returns>"True" if the left value is greater than or equal to the right value, otherwise - "false"</returns>
        public static bool IsGreaterOrEqual(float leftHandOperand, float rightHandOperand)
        {
            return (leftHandOperand > rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand);
        }

        /// <summary>
        /// Checks whether the left-hand operand is greater than or equal to the right-hand operand.
        /// </summary>
        /// <param name="leftHandOperand">The double value representing the left-hand operand</param>
        /// <param name="rightHandOperand">The double value representing the right-hand operand</param>
        /// <returns>"True" if the left value is greater than or equal to the right value, otherwise - "false"</returns>
        public static bool IsGreaterOrEqual(double leftHandOperand, double rightHandOperand)
        {
            return (leftHandOperand > rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand);
        }

        /// <summary>
        /// Checks whether the left-hand operand is greater than or equal within the given tolerance to the right-hand operand.
        /// </summary>
        public static bool IsGreaterOrEqual(double leftHandOperand, double rightHandOperand, double tolerance)
        {
            return (leftHandOperand > rightHandOperand) || AreEqual(leftHandOperand, rightHandOperand, tolerance);
        }

        /// <summary>
        /// Returns a value indicating whether the specified value is a prime number.
        /// </summary>
        /// <dev>
        /// Based on ROTOR sources.
        /// </dev>
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }

                return true;
            }

            return (candidate == 2);
        }

        /// <summary>
        /// Returns the next prime number after the specified min value to be used as a hashtable capacity.
        /// If the calculated prime number is out of Int32 range, returns the specified min value.
        /// </summary>
        /// <dev>
        /// Based on ROTOR sources.
        /// </dev>
        public static int GetPrimeForHashtable(int min)
        {
            ArgumentUtil.CheckNonNegative(min, "min");

            for (int i = 0; i < gPrimesForHashtable.Length; i++)
            {
                int prime = gPrimesForHashtable[i];
                if (prime >= min)
                    return prime;
            }

            // Outside of our predefined table. Compute the hard way.
            for (int i = (min | 1); i < Int32.MaxValue; i += 2)
            {
                if (IsPrime(i))
                    return i;
            }

            return min;
        }

        /// <summary>
        /// Returns the maximum int value from a specified list.
        /// </summary>
        public static int Max(params int[] list)
        {
            Debug.Assert(list.Length > 0);

            int max = list[0];
            for (int i = 1; i < list.Length; i++)
                max = Math.Max(max, list[i]);

            return max;
        }

        /// <summary>
        /// Returns the minimum float value from the specified list.
        /// </summary>
        public static float Min(params float[] list)
        {
            float min = Single.MaxValue;
            for (int i = 0; i < list.Length; i++)
                min = Math.Min(min, list[i]);

            return min;
        }

        /// <summary>
        /// Returns the maximum float value from the specified list.
        /// </summary>
        public static float Max(params float[] list)
        {
            float max = Single.MinValue;
            for (int i = 0; i < list.Length; i++)
                max = Math.Max(max, list[i]);

            return max;
        }

        /// <summary>
        /// Calculates Y value for the polyline defined by specified array of <see cref="Point"/>.
        /// </summary>
        /// <remarks>
        /// An array must be sorted in ascending <see cref="Point.X"/> value and
        /// all <see cref="Point.X"/> values must be unique.
        /// </remarks>>
        public static int GetPolylineY(int x, Point[] function)
        {
            if (x < function[0].X)
                return 0;
            int length = function.Length;

            int rangeId = 1;
            // Find an appropriate range. May be changed to binary search.
            for (; rangeId < length; rangeId++)
            {
                if (function[rangeId - 1].X >= function[rangeId].X)
                    throw new ArgumentException("The array of points is given incorrectly.");

                if (x < function[rangeId].X)
                    break;
            }

            // Last polyline segment (last defined point, infinity) has the same line equation like previous segment.
            if (rangeId == length)
                rangeId--;

            int x0 = function[rangeId - 1].X;
            int x1 = function[rangeId].X;
            int y0 = function[rangeId - 1].Y;
            int y1 = function[rangeId].Y;

            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        /// <summary>
        /// Divides the given integers, rounding the quotient to the nearest integer.
        /// </summary>
        /// <remarks>
        /// Use instead of division to get a rounded integer result, eg 1 instead of 0 from 9/10.
        /// Division by zero is not handled.
        /// Rounding is standard, not banker's (5/2 rounds to 3, not 2).
        /// </remarks>
        public static int Divide(int dividend, int divisor)
        {
            return MulDiv(1, dividend, divisor);
        }

        /// <summary>
        /// Returns a rounded value of p1*p2/divisor, handling int32 arithmetic overflow.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="divisor"></param>
        /// <remarks>
        /// The result of p1*p2 is handled via int64, so no arithmetic overflow happens.
        /// This is needed to reproduce some MS Word calculations for table grid,
        /// so that rounding happens in the same way.
        /// </remarks>
        public static int MulDiv(int p1, int p2, int divisor)
        {
            long product = (long)p1 * p2;
            int result = (int)(product / divisor);

            int remainder = (int)(product % divisor);
            int minRoundUpRemainder = divisor / 2 + divisor % 2;
            if (remainder >= minRoundUpRemainder && remainder != 0)
                ++result;

            return result;
        }

        /// <summary>
        /// Returns true if the value is odd (not even).
        /// </summary>
        public static bool IsOdd(long value)
        {
            return ((value & 0x01) != 0);
        }

        /// <summary>
        /// Integer divide and round up if necessary so the remainder will be zero.
        /// </summary>
        public static int DivUp(long value, int divider)
        {
            long result = value / divider;
            if ((value % divider) != 0)
                result++;
            return (int)result;
        }

        /// <summary>
        /// Rounds up the value to be divisible by divider with zero remainder.
        /// </summary>
        public static int RoundUp(long value, int divider)
        {
            return DivUp(value, divider) * divider;
        }

        /// <summary>
        /// Casts a double to int32 in a way that uint32 are "correctly" casted too (they become negative numbers).
        /// </summary>
        public static int CastDoubleToInt(double value)
        {
            long temp = (long)value;
            return (int)temp;
        }

        /// <summary>
        /// Calculate the factorial of a number.
        /// </summary>
        public static int Factorial(int n)
        {
            int r = 1;
            while (n > 1)
                r *= n--;
            return r;
        }

        private static readonly int[] gPrimesForHashtable = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        /// <summary>
        /// Returns the floating-point value that corresponds to a specific bit pattern.
        /// Sources from https://msdn.microsoft.com/en-us/library/aa987800(v=vs.80).aspx
        /// </summary>
        public static float IntBitsToFloat(int bits)
        {
            int sign = ((bits & 0x80000000) == 0) ? 1 : -1;
            int exponent = ((bits & 0x7f800000) >> 23);
            int mantissa = (bits & 0x007fffff);

            mantissa |= 0x00800000;
            // Calculate the result:
            return (float)(sign * mantissa * Math.Pow(2, exponent - 150));
        }

        /// <summary>
        /// Normalizes rectangle by inverting negative width and/or height.
        /// </summary>
        public static RectangleF NormalizeRectangle(RectangleF rectangle)
        {
            RectangleF result = rectangle;
            if (result.Width < 0)
                result = new RectangleF(result.X + result.Width, result.Y, -result.Width, result.Height);
            if (result.Height < 0)
                result = new RectangleF(result.X, result.Y + result.Height, result.Width, -result.Height);
            return result;
        }

        /// <summary>
        /// Normalizes rectangle by inverting negative width and/or height.
        /// </summary>
        public static Rectangle NormalizeRectangle(Rectangle rectangle)
        {
            Rectangle result = rectangle;
            if (result.Width < 0)
                result = new Rectangle(result.X + result.Width, result.Y, -result.Width, result.Height);
            if (result.Height < 0)
                result = new Rectangle(result.X, result.Y + result.Height, result.Width, -result.Height);
            return result;
        }

        /// <summary>
        /// Gets the estimated precision that should be used while calculating axis values.
        /// </summary>
        /// <param name="value">The value used as the basis</param>
        /// <returns>The calculated precision</returns>
        public static int GetPrecision(double value)
        {
            // Remove integer part of value.
            value -= (int)value;

            if (IsZero(value))
                return 0;

            // Implicit casting for Java: it is difference for infinity double values.
            int power = DoubleToInt(Math.Round(Math.Log10(value)));
            value *= Math.Pow(10, (-power));
            value -= (int)value;

            // If values is so small, than use the reduced tolerance to correct comparison.
#if JAVA
            power -= java.math.BigDecimal.valueOf(value).scale();
#else
            power -= GetNumberAfterPoint(value) + 1;
#endif
            return power;
        }

        /// <summary>
        /// Gets the tolerance used when comparing values (the maximum value of precision is -10).
        /// </summary>
        public static double GetTolerance(double value)
        {
            int precision = Math.Min(-10, GetPrecision(value));
            return Math.Pow(10, precision);
        }

        /// <summary>
        /// Gets the mean (arithmetic average) of specified values.
        /// </summary>
        public static double GetMean(double[] values)
        {
            double result = 0;
            for (int i = 0; i < values.Length; i++)
                result += values[i];

            return result / values.Length;
        }

        /// <summary>
        /// Gets the standard deviation of specified values.
        /// </summary>
        public static double GetStandardDeviation(double[] values)
        {
            double mean = GetMean(values);
            double sumOfSquares = 0;

            for (int i = 0; i < values.Length; i++)
                sumOfSquares += Math.Pow(values[i] - mean, 2);

            double variance = sumOfSquares / (values.Length - 1);

            return Math.Sqrt(variance);
        }

        /// <summary>
        /// Gets the number of digits after point.
        /// </summary>
        [JavaAttributes.JavaDelete]
        private static int GetNumberAfterPoint(double value)
        {
            decimal argument = (decimal)value;
            return BitConverter.GetBytes(decimal.GetBits(argument)[3])[2];
        }
    }
}
