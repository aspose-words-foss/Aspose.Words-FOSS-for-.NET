// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/09/2009 by Roman Korchagin
#if NETSTANDARD

using System;
using Aspose.Drawing;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Port this class to Java manually.
    /// </summary>
    public class MatrixPal
    {
        public MatrixPal(SKMatrix matrix)
        {
            mMatrix = matrix;
        }

        /// <summary>
        /// Returns a GDI+ matrix. Note that the caller must dispose the returned value.
        /// </summary>
        public static SKMatrix ToNativeMatrix(DrMatrix drMatrix)
        {
            SKMatrix matrix = new SKMatrix();
            matrix.Values = new float[]
            {
                drMatrix.M11, drMatrix.M21, drMatrix.M31,
                drMatrix.M12, drMatrix.M22, drMatrix.M32,
                0, 0, 1
            };
            return matrix;
        }

        public static DrMatrix FromNativeMatrix(SKMatrix matrix)
        {
            return new DrMatrix(matrix.ScaleX, matrix.SkewY, matrix.SkewX, matrix.ScaleY, matrix.TransX, matrix.TransY);
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
        public static string ToRawString(System.Drawing.PointF point)
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

        public static SKMatrix GetShearInstance(float sx, float sy)
        {
            return SKMatrix.CreateSkew(sx, sy);
        }

        public static SKMatrix Identity
        {
            get { return SKMatrix.CreateIdentity(); }
        }

        public static SKPath CreateTransformedShape(SKMatrix transform, SKPath glyphShape)
        {
            glyphShape.Transform(transform);
            return glyphShape;
        }

        public static void Translate(SKMatrix originTransform, float x, float y)
        {
            SKMatrix trans = SKMatrix.CreateTranslation(x, y);
            SKMatrix.Concat(ref originTransform, originTransform, trans);
        }

        public static SKMatrix CreateInverse(SKMatrix transform)
        {
            SKMatrix inverse;
            if (transform.TryInvert(out inverse))
                return inverse;

            return transform;
        }

        public static bool IsUniformScaleTransform(SKMatrix t, double[] refScale)
        {
            double sx = t.ScaleX;
            double sy = t.ScaleY;
            double shx = t.SkewX;
            double shy = t.SkewY;

            const double precision = 1e-6d;

            bool standard = (
                    (DoubleAreEqual(sx, sy, precision) || DoubleAreEqual(sx, -sy, precision)) &&
                            (DoubleAreEqual(shx, 0, precision)) &&
                            (DoubleAreEqual(shy, 0, precision)));

            if (standard)
            {
                refScale[0] = Math.Abs(sx);
                return true;
            }

            bool rotated = (
                    (DoubleAreEqual(sx, 0, precision)) &&
                            (DoubleAreEqual(sy, 0, precision)) &&
                            (DoubleAreEqual(shx, shy, precision) || DoubleAreEqual(shx, -shy, precision)));

            if (rotated)
            {
                refScale[0] = Math.Abs(shx);
                return true;
            }

            return false;
        }


        public static bool IsDefaultScaleTransform(SKMatrix at)
        {
            double sx = at.ScaleX;
            double sy = at.ScaleY;
            double shx = at.SkewX;
            double shy = at.SkewY;

            const double precision = 1e-6d;

            // Same value as the former GdiRenderer.DefaultScale (rendering engine is not part of the FOSS build).
            const float DefaultScale = ImageConstants.StandardResolution / 72f;

            // [ (+/-)G_DEFAULT_SCALE,     0,                tx ]
            // [      0,              (+/-)G_DEFAULT_SCALE,  ty ]
            // [      0,                   0,                1  ]
            bool standard = (
                    (DoubleAreEqual(sx, DefaultScale, precision) || DoubleAreEqual(sx, -DefaultScale, precision)) &&
                            (DoubleAreEqual(sy, DefaultScale, precision) || DoubleAreEqual(sy, -DefaultScale, precision)) &&
                            (DoubleAreEqual(shx, 0, precision)) &&
                            (DoubleAreEqual(shy, 0, precision)));

            // [      0,               (+/-)G_DEFAULT_SCALE,  tx ]
            // [ (+/-)G_DEFAULT_SCALE,      0,                ty ]
            // [      0,                    0,                1  ]
            bool rotated = (
                    (DoubleAreEqual(sx, 0, precision)) &&
                            (DoubleAreEqual(sy, 0, precision)) &&
                            (DoubleAreEqual(shx, DefaultScale, precision) || DoubleAreEqual(shx, -DefaultScale, precision)) &&
                            (DoubleAreEqual(shy, DefaultScale, precision) || DoubleAreEqual(shy, -DefaultScale, precision)));

            return standard || rotated;

        }

        public static bool DoubleAreEqual(double value1, double value2, double precision)
        {
            return (Math.Abs(value1 - value2) < precision);
        }

        public static SKMatrix GetScaleTransform(SKMatrix at)
        {
            float[] values = new float[9];
            at.GetValues(values);
            float sX = at.ScaleX;
            float sY = at.ScaleY;

            float m00 = sX;
            float m01 = 0;
            float m02 = 0;
            float m10 = 0;
            float m11 = sY;
            float m12 = 0;

            SKMatrix m = new SKMatrix();
            m.Values = new float[] {m00, m01, m02, m10, m11, m12, 0, 0, 1}; //{m00, m10, m01, m11, m02, m12, 0, 0, 1});
            return m;
        }

        /// <summary>
        /// Gets compensatory scale transform which can be applied to already scaled shape.
        /// Applying of compensatory scale transform to already scaled shape is equivalent to
        /// applying of full affine transform to non-scaled shape.
        /// </summary>
        /// <param name="at">The full affine transform</param>
        /// <returns></returns>
        public static SKMatrix GetCompensatoryScaleTransform(SKMatrix at)
        {
            float[] values = new float[9];
            at.GetValues(values);
            float sX = at.ScaleX;
            float sY = at.ScaleY;
            float shX = at.SkewX;
            float shY = at.SkewY;
            float tX = at.TransX;
            float tY = at.TransY;

            // We need to apply the following transformation to shape:
            //
            //     [ sX   shX  tX ]
            // A = [ shY  sY   tY ]
            //     [ 0    0    1  ]
            //
            // We have already performed scaling and our affine transformation looks like the following:
            //
            //     [ sX   0    0 ]
            // B = [ 0    sY   0 ]
            //     [ 0    0    1 ]
            //
            // So to perform scaling separately from shearing and translating we need to apply the following transformation
            // to already scaled shape:
            //
            //              [ 1/sX   0      0 ]
            // reverse(B) = [ 0      1/sY   0 ]
            //              [ 0      0      1 ]
            //
            //                  [ 1         shX/sY    tX ]
            // A * reverse(B) = [ shY/sX    1         tY ]
            //                  [ 0         0          1 ]

            float m00 = 1;
            float m01 = shX / sY;
            float m02 = tX;
            float m10 = shY / sX;
            float m11 = 1;
            float m12 = tY;

            SKMatrix m = new SKMatrix();
            m.Values = new float[] { m00, m01, m02, m10, m11, m12, 0, 0, 1 };
            return m;
        }

        public float ScaleX
        {
            get { return mMatrix.ScaleX; }
        }

        public float ScaleY
        {
            get { return mMatrix.ScaleY; }
        }

        public float ShearX
        {
            get { return mMatrix.SkewX; }
        }

        public float ShearY
        {
            get { return mMatrix.SkewY; }
        }

        public float TranslateX
        {
            get { return mMatrix.TransX; }
        }

        public float TranslateY
        {
            get { return mMatrix.TransY; }
        }

        public SKMatrix NativeMatrix
        {
            get { return mMatrix; }
        }

        private readonly SKMatrix mMatrix;
    }
}
#endif
