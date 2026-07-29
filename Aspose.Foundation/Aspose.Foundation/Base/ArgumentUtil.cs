// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/02/2016 by Roman Korchagin

using System;

namespace Aspose
{
    /// <summary>
    /// Contains utility methods for argument checking.
    /// </summary>
    public static class ArgumentUtil
    {
        public static void CheckHasChars(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("The argument cannot be null or empty string.", paramName);
        }

        public static void CheckNotNull(object value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void CheckRangeInclusive(int value, int min, int max, string paramName)
        {
            if ((value < min) || (value > max))
                throw new ArgumentOutOfRangeException(paramName, String.Format("Expected a value between {0} and {1}.", min, max));
        }

        public static void CheckRangeInclusive(double value, double min, double max, string paramName)
        {
            if ((value < min) || (value > max) || double.IsNaN(value))
                throw new ArgumentOutOfRangeException(paramName, String.Format("Expected a value between {0} and {1}.", min, max));
        }

        public static void CheckPositive(int value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(paramName, "Expected a positive value.");
        }

        public static void CheckPositive(double value, string paramName)
        {
            if (value <= 0 || double.IsNaN(value))
                throw new ArgumentOutOfRangeException(paramName, "Expected a positive value.");
        }

        public static void CheckNonNegative(int value, string paramName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(paramName, "Expected a non-negative value.");
        }

        public static void CheckNonNegative(double value, string paramName)
        {
            if (value < 0 || double.IsNaN(value))
                throw new ArgumentOutOfRangeException(paramName, "Expected a non-negative value.");
        }

        /// <summary>
        /// Validates the value in the given range.
        /// Either throws an exception or assigns given saturation values if it exceeds.
        /// </summary>
        public static double ValidateRange(double value, double minMargin, double minSaturation, double maxMargin, double maxSaturation, bool isThrow, string paramName)
        {
            double result;

            if (value < minMargin)
            {
                if (isThrow)
                    throw new ArgumentOutOfRangeException(paramName);

                result = minSaturation;
            }
            else if (value > maxMargin)
            {
                if (isThrow)
                    throw new ArgumentOutOfRangeException(paramName);

                result = maxSaturation;
            }
            else
            {
                result = value;
            }

            return result;
        }

        // Returns True if either obj1 or obj2 is null but not both.
        public static bool OneIsNull(object obj1, object obj2)
        {
            return ((obj1 == null) ^ (obj2 == null));
        }

        public static bool BothAreNotNull(object obj1, object obj2)
        {
            return ((obj1 != null) && (obj2 != null));
        }

        public static bool BothAreNull(object obj1, object obj2)
        {
            return ((obj1 == null) && (obj2 == null));
        }

        /// <summary>
        /// Returns true if both objects are not null, has the same type and their hashcodes matches.
        /// </summary>
        public static bool TypeAndHashCodeMatches(object obj1, object obj2)
        {
            return BothAreNotNull(obj1, obj2) &&
                   obj1.GetType() == obj2.GetType() &&
                   obj1.GetHashCode() == obj2.GetHashCode();
        }
    }
}
