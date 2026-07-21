// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/11/2005 by Roman Korchagin

using System;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// Provides utility methods to work with enum types. Has to be manually ported because reflection is different on different platforms.
    /// </summary>
    [JavaManual("EffectiveArrayLength is checked on .Net. Just default value is used on Java.")]
    public static class EnumUtilPal
    {
        /// <summary>
        /// Use just default value in Java.
        /// </summary>
        public static int GetEffectiveArrayLength(Type enumType, int defaultValue)
        {
            int calculated = GetEffectiveArrayLength(enumType);
            Debug.Assert(defaultValue == calculated);
            if (defaultValue != calculated)
            {
                throw new ArgumentException(
                    "Please update defaultValue of effective array length: it should be greater then max value of the enum." +
                    "\r\nEnum Type: " + enumType.FullName +
                    "\r\ndefaultValue: " + defaultValue +
                    "\r\ncalculated EffectiveArrayLength: " + calculated);
            }
            return defaultValue;
        }

        /// <summary>
        /// Returns the length of an array which can hold items corresponding to all possible values of the enum of
        /// the specified type. The enum should be non-flag and its min value should be greater or equal to zero.
        /// Note, that calls of this method are quite expensive, so the best place to call it is the initialization
        /// of static fields of interested classes.
        /// </summary>
        public static int GetEffectiveArrayLength(Type enumType)
        {
            // Ensure that the specified type is a non-flag enum type.
            Debug.Assert(enumType.IsEnum);
            Debug.Assert(enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0);

            int minValue = int.MaxValue;
            int maxValue = int.MinValue;
            int count = 0;

            foreach (int value in Enum.GetValues(enumType))
            {
                if (value > maxValue)
                    maxValue = value;
                if (value < minValue)
                    minValue = value;

                count++;
            }

            // Ensure that the enum has at least one value and its min value is greater or equal to zero.
            Debug.Assert((count != 0) && (minValue >= 0));

            // Ensure that the enum does not have too large "holes" between its sequential values.
            // Let's use 50% as a threshold for these holes' sum length.
            Debug.Assert(((double)count) / (maxValue + 1) > 0.5d);

            return (maxValue + 1);
        }

        /// <summary>
        /// WORDSNET-871 The object.Equals(object) always returns false if args are enum and/or int.
        /// Use casting to int to compare enums and ints.
        /// </summary>
        public static bool EnumOrIntEquals(object objA, object objB)
        {
            if ((objA is Enum || objA is int) &&
                (objB is Enum || objB is int))
            {
                return (int)objA == (int)objB;
            }

            return objA.Equals(objB);
        }
    }
}
