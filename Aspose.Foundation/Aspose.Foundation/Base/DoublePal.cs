// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/09/2009 by Roman Korchagin

using System;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose
{
    /// <summary>
    /// This class is to be manually ported to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public static class DoublePal
    {
        /// <summary>
        /// Casting double to int differs when double value is Double.NaN:
        /// .Net returns Integer.MinValue,
        /// Java returns 0.
        /// 
        /// The method allows Java behave like .Net.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int ToInt(double value)
        {
            // not changed in .Net.
            return (int) value;
        }

        /// <summary>
        /// Rounds up a double value and casts it to the nearest int in a way that will produce the same result on .NET and Java.
        /// </summary>
        public static int RoundToIntUp(double value)
        {
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Trims double value to the specified number of digits after decimal point.
        /// </summary>
        public static double Trim(double value, int digits)
        {
            double p = Math.Pow(10, digits);
            return (Math.Truncate(value*p))/p;
        }

        public static int GetHashCode(double value)
        {
            return value.GetHashCode();
        }
    }
}
