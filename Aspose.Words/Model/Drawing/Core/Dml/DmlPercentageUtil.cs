// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2008 by Roman Korchagin

using System;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Utility methods for 5.1.12.22 ST_FixedPercentage (Fixed Percentage) amd ST_Percentage.
    /// </summary>
    internal static class DmlPercentageUtil
    {
        /// <summary>
        /// Converts from 0..100% to 0.0..1.0 fraction.
        /// </summary>
        private static double FromPercent(string value, double defaultValue)
        {
            if (!IsPercentWithSign(value))
                return defaultValue;

            // Trim % from the end.
            string num = value.Substring(0, value.Length - 1);

            return FormatterPal.ParseDouble(num)/100;
        }

        /// <summary>
        /// Converts from 0..100% to 0.0..1.0 fraction.
        /// </summary>
        internal static double FromPercent(string value)
        {
            return FromPercent(value, 0);
        }

        /// <summary>
        /// Checks if the specified value is a number with the percent sign.
        /// </summary>
        private static bool IsPercentWithSign(string value)
        {
            return StringUtil.HasChars(value) && gContentRegex.IsMatch(value);
        }

        /// <summary>
        /// Converts from 0..100000 (1000th percent) or 0.0..100.0% to 0.0..1.0 fraction.
        /// </summary>
        /// <param name="value">Input string to convert.</param>
        /// <param name="defaultValue">Default value to return, if input string is wrong percent value.
        /// Should be 0.0..1.0 fraction.</param>
        /// <param name="complianceInfo">Depending on input value this argument is used to specify which OOXML
        /// specification the document conforms.</param>
        /// <returns>Conversion result as 0.0..1.0 fraction.</returns>
        internal static double FromPercentOrDmlPercent(string value, double defaultValue, 
            OoxmlComplianceInfo complianceInfo)
        {
            if (IsPercentWithSign(value))
            {
                if (complianceInfo != null)
                    complianceInfo.MarkAsIsoTransitional();

                return FromPercent(value, defaultValue);
            }
            else
                return FromDmlPercent(value, defaultValue);
        }

        /// <summary>
        /// Converts from 0..100000 (1000th percent) or 0.0..100.0% to 0.0..1.0 fraction.
        /// </summary>
        internal static double FromPercentOrDmlPercent(string value, OoxmlComplianceInfo complianceInfo)
        {
            return FromPercentOrDmlPercent(value, 0, complianceInfo);
        }

        /// <summary>
        /// Converts from 0.0..1.0 to 0%..100%.
        /// </summary>
        private static string ToPercent(double fraction)
        {
            return FormatterPal.DoubleToStr9Decimals(fraction * 100.0) + "%";
        }

        /// <summary>
        /// Converts from 0.0..1.0 to 0..100000 (1000th percent) or to 0.00%..100.00% depending on isIsoStrict parameter.
        /// </summary>
        /// <param name="fraction">A fraction value to convert.</param>
        /// <param name="isIsoStrict">The format of ISO 29500 Strict specification should be used.</param>
        /// <returns>Conversion result.</returns>
        internal static string ToPercentOrDmlPercent(double fraction, bool isIsoStrict)
        {
            return isIsoStrict ?
                ToPercent(fraction) :
                FormatterPal.DoubleToStrNDecimals(ToDmlPercent(fraction), 0);
        }

        /// <summary>
        /// Converts from 0..100000 to 0.0..1.0 (1000th percent to fraction).
        /// </summary>
        internal static double FromDmlPercent(double value)
        {
            return value / Factor;
        }

        /// <summary>
        /// Converts from 0..100000 to 0.0..1.0 (1000th percent to fraction).
        /// </summary>
        internal static double FromDmlPercent(string value, double defaultValue)
        {
            double number = FormatterPal.TryParseDoubleInvariant(value);
            return !Double.IsNaN(number) ? FromDmlPercent(number) : defaultValue;
        }

        /// <summary>
        /// Converts from 0.0..1.0 to 0..100000 (fraction to 1000th percent).
        /// </summary>
        internal static double ToDmlPercent(double fraction)
        {
            return fraction * Factor;
        }

        /// <summary>
        /// Converts from int 0..100000 to 0.0 to 1.0 fixed 16.16.
        /// </summary>
        internal static int DmlToPercentFixed(int value)
        {
            return ConvertUtilCore.DoubleToFixed(FromDmlPercent(value));
        }

        /// <summary>
        /// Converts from 0.0..1.0 fixed 16.6 to int 0..100000.
        /// </summary>
        internal static int PercentFixedToDml(int value)
        {
            return MathUtil.DoubleToInt(ToDmlPercent(ConvertUtilCore.FixedToDouble(value)));
        }

        /// <summary>
        /// Converts from int -100000..100000 to fixed 16.16 brightness value.
        /// </summary>
        internal static int DmlToBrightnessFixed(int value)
        {
            return ConvertUtilCore.DoubleToFixed(value / DmlToBrightnessRatio);
        }

        /// <summary>
        /// Converts from fixed 16.16 brightness value to int -100000..100000.
        /// </summary>
        internal static int BrightnessFixedToDml(int value)
        {
            return MathUtil.DoubleToInt(ConvertUtilCore.FixedToDouble(value) * DmlToBrightnessRatio);
        }

        /// <summary>
        /// Converts from int -100000..100000 to fixed 16.16 contrast value.
        /// </summary>
        internal static int DmlToContrastFixed(int value)
        {
            // Transpose the value to make positive and then scale down to the 0.0 to 1.0 range.
            double percent = (value + (-DmlFixedPercentMin)) / DmlToContrastRatio;
            // Now convert to model contrast value.
            return ConvertUtilCore.DoubleToFixed(ImageData.PercentToContrast(percent));
        }

        /// <summary>
        /// Converts from fixed 16.16 contrast value to int -100000..100000.
        /// </summary>
        internal static int ContrastFixedToDml(int value)
        {
            // Convert model contrast value into percent 0.0 to 1.0.
            double percent = ImageData.ContrastToPercent(ConvertUtilCore.FixedToDouble(value));
            // Scale back to OOXML range and transpose.
            return MathUtil.DoubleToInt((percent * DmlToContrastRatio) - (-DmlFixedPercentMin));
        }

        /// <summary>
        /// Converts from DML percent to VML percent value.
        /// </summary>
        internal static int DmlToVmlPercent(double value)
        {
            // In VML value is integer 1/10th percent. In DML 1/1000th percent.
            return MathUtil.DoubleToInt(value*VmlPerPercent/DmlPerPercent);
        }

        /// <summary>
        /// Converts from VML percent to DML percent value.
        /// </summary>
        internal static double VmlToDmlPercent(double value)
        {
            return value * DmlPerPercent / VmlPerPercent;
        }

        /// <summary>
        /// Rounds specified value according to precision which used to store
        /// percent value in DrawingML.
        /// </summary>
        internal static double ToDmlPercentPrecision(double value)
        {
            // The definition of the percent type described at "22.9.2.11 ST_PositivePercentage"
            // has the following restriction: "This simple type's contents shall
            // match the following regular expression pattern: [0-9]+(\.[0-9]+)?%". Mimic MSW behavior
            // and take only first 3 digits from fraction part.
            return System.Math.Round(value, 5, MidpointRounding.AwayFromZero);
        }

        private const double Factor = 100000.0;

        private const double DmlPerPercent = 1000;
        private const double VmlPerPercent = 10;

        private const double ContrastRange = ImageData.ContrastMax - ImageData.ContrastMin;
        private const double BrightnessRange = ImageData.BrightnessMax - ImageData.BrightnessMin;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int DmlFixedPercentMax = 100000;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int DmlFixedPercentMin = -100000;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int DmlFixedPercenRange = DmlFixedPercentMax - DmlFixedPercentMin;

        private const double DmlToBrightnessRatio = DmlFixedPercenRange / BrightnessRange;
        private const double DmlToContrastRatio = DmlFixedPercenRange / ContrastRange;

        private static readonly Regex gContentRegex = new Regex(@"-?[0-9]+(\.[0-9]+)?%", RegexOptions.Compiled);
    }
}
