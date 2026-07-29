// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/09/2009 by Roman Korchagin
#if !NETSTANDARD
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Port this class to Java manually.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public static class MatrixPal
    {
        /// <summary>
        /// Returns a GDI+ matrix. Note that the caller must dispose the returned value.
        /// </summary>
        public static Matrix ToNativeMatrix(DrMatrix drMatrix)
        {
            return new Matrix(drMatrix.M11, drMatrix.M12, drMatrix.M21, drMatrix.M22, drMatrix.M31, drMatrix.M32);
        }

        public static DrMatrix FromNativeMatrix(Matrix matrix)
        {
            float[] elements = matrix.Elements;
            return new DrMatrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
        }

        /// <summary>
        /// Returns raw hex representation of the float values of the matrix - use this when comparing small differences with Java.
        /// </summary>
        public static string ToRawString(DrMatrix drMatrix)
        {
            return string.Format(
                "{0}, {1}, {2}, {3}, {4}, {5}",
                StringUtil.BytesToHex(BitConverter.GetBytes(drMatrix.M11), 0, 4, true),
                StringUtil.BytesToHex(BitConverter.GetBytes(drMatrix.M12), 0, 4, true),
                StringUtil.BytesToHex(BitConverter.GetBytes(drMatrix.M21), 0, 4, true),
                StringUtil.BytesToHex(BitConverter.GetBytes(drMatrix.M22), 0, 4, true),
                StringUtil.BytesToHex(BitConverter.GetBytes(drMatrix.M31), 0, 4, true),
                StringUtil.BytesToHex(BitConverter.GetBytes(drMatrix.M32), 0, 4, true));
        }

        /// <summary>
        /// Returns raw hex representation of the float values of the point - use this when comparing small differences with Java.
        /// </summary>
        public static string ToRawString(PointF point)
        {
            return string.Format(
                "{0}, {1}",
                StringUtil.BytesToHex(BitConverter.GetBytes(point.X), 0, 4, true),
                StringUtil.BytesToHex(BitConverter.GetBytes(point.Y), 0, 4, true));
        }

        /// <summary>
        /// Returns raw hex representation of the float value - use this when comparing small differences with Java.
        /// </summary>
        public static string ToRawString(float value)
        {
            return StringUtil.BytesToHex(BitConverter.GetBytes(value), 0, 4, true);
        }

        /// <summary>
        /// Returns raw hex representation of the double value - use this when comparing small differences with Java.
        /// </summary>
        public static string ToRawString(double value)
        {
            return StringUtil.BytesToHex(BitConverter.GetBytes(value), 0, 8, true);
        }
    }
}
#endif
