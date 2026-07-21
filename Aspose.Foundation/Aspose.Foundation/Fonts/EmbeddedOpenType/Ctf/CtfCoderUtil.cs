// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2016 by Ilya Navrotskiy

using System;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Utility class for CTF coding.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#CTF for more info.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal static class CtfCoderUtil
    {
        /// <summary>
        /// Calculates prediction for the specified HDMX device record and horizontal metric.
        /// </summary>
        /// <remarks>The 'prediction' is 'rounded_TT_AW' in terms of spec.</remarks>
        internal static int CalculatePrediction(HdmxDeviceRecord record, HorizontalMetric metric, ushort unitsPerEm)
        {
            // See formula in spec MicroType® Express (MTX) Font Format - 5.4. The 'hdmx' Table Translation.
            return (((((64 * record.PixelSize * metric.AdvanceWidth) + (unitsPerEm / 2)) / unitsPerEm) + 32) / 64);
        }

        /// <summary>
        /// Returns cloned source array of data with first two bytes changed to specified 'newVersion'.
        /// </summary>
        internal static byte[] CloneWithVersion(byte[] srcData, ushort newVersion)
        {
            byte[] clonedData = new byte[srcData.Length];
            clonedData[0] = (byte)(newVersion >> 8);
            clonedData[1] = (byte)newVersion;
            Array.Copy(srcData, 2, clonedData, 2, srcData.Length - 2);

            return clonedData;
        }

        internal const short CvtPositive8 = 255;
        internal const short CvtPositive1 = 248;
        internal const short CvtNegative8 = 247;
        internal const short CvtNegative0 = 239;
        internal const short CvtLowestCode = 238;
    }
}
