// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/09/2010 by Konstantin Sidorenko
/*
 * Based on:
 *
 * @(#)AffineTransform.java    1.77 06/03/09
 *
 * Copyright 2006 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Collections;

namespace Aspose.Drawing
{
    /// <summary>
    /// Replaces <see cref="System.Drawing.Drawing2D.Matrix"/>.
    /// </summary>
    /// <remarks>
    /// Most algorithms taken from Sun's AffineTransform.java.
    /// Java's names for matrix elements used internally. 
    /// Map of java names to .net ones to description:
    /// 
    /// m00     M11     Scale X
    /// m10     M12     Shear Y
    /// m01     M21     Shear X
    /// m11     M22     Scale Y
    /// m02     M31     Translate X
    /// m12     M32     Translate Y
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public class DrMatrix
    {
        /// <summary>
        /// Initializes a new instance of the DrMatrix class as the identity matrix.
        /// </summary>
        public DrMatrix()
        {
            Me00 = 1f;
            Me11 = 1f;
            // m01 = m10 = m02 = m12 = 0f;    /* Not needed. */
            // mState = ApplyIdentity;        /* Not needed. */
            // mType = TypeIdentity;        /* Not needed. */
        }

        /// <summary>
        /// m00     M11     Scale X
        /// m10     M12     Shear Y
        /// m01     M21     Shear X
        /// m11     M22     Scale Y
        /// m02     M31     Translate X
        /// m12     M32     Translate Y
        /// </summary>
        public DrMatrix(float M11, float M12, float M21, float M22, float M31, float M32)
        {
            Me00 = M11;
            Me10 = M12;
            Me01 = M21;
            Me11 = M22;
            Me02 = M31;
            Me12 = M32;
            UpdateState();
        }

        public bool IsIdentity
        {
            get { return (Me00 == 1f) && (Me10 == 0f) && (Me01 == 0f) && (Me11 == 1f) && (Me02 == 0f) && (Me12 == 0f); }
        }

        public bool HasShear
        {
            get { return (Me10 != 0) || (Me01 != 0); }
        }

        /// <summary>
        /// Returns copy of matrix elements.
        /// </summary>
        public float[] GetElements()
        {
            return new float[] { Me00, Me10, Me01, Me11, Me02, Me12 };
        }

        public static bool IsNullOrIdentity(DrMatrix matrix)
        {
            return matrix == null || matrix.IsIdentity;
        }

        public static DrMatrix CreateStretchMatrix(RectangleF from, RectangleF to)
        {
            DrMatrix matrix = new DrMatrix();
            float fromCenterX = from.X + (float)(from.Width / 2.0f);    // Casting for Java.
            float fromCenterY = from.Y + (float)(from.Height / 2.0f);   // Casting for Java.
            matrix.Translate(-fromCenterX, -fromCenterY, MatrixOrder.Prepend); // Select center point for scaling.
            float sx = to.Width / from.Width;
            float sy = to.Height / from.Height;
            matrix.Scale(sx, sy, MatrixOrder.Append); // Scale
            float toCenterX = to.X + (float)(to.Width / 2.0f);  // Casting for Java.
            float toCenterY = to.Y + (float)(to.Height / 2.0f);
            matrix.Translate(toCenterX, toCenterY, MatrixOrder.Append); //Move center to "to" center
            return matrix;
        }

        /// <summary>
        /// Creates stretch and shear matrix that translates <paramref name="srcRect"/> rectangle to 
        /// <paramref name="dstParallelogram"/> parallelogram.
        /// </summary>
        /// <param name="srcRect">Source rectangle.</param>
        /// <param name="dstParallelogram">
        /// Points array which specifies destination parallelogram.
        /// Should contain exactly 3 points:
        /// 0 - top-left point;
        /// 1 - top-right point;
        /// 2 - bottom-left point.
        /// </param>
        public static DrMatrix CreateStretchAndShearMatrix(RectangleF srcRect, PointF[] dstParallelogram)
        {
            if (dstParallelogram == null)
                throw new ArgumentNullException("dstParallelogram");
            if (dstParallelogram.Length != 3)
                throw new ArgumentException("Parallelogram points array should contain 3 points.");

            PointF dstTopLeft = dstParallelogram[0];
            PointF dstTopRight = dstParallelogram[1];
            PointF dstBottomLeft = dstParallelogram[2];

            float m00 = (float)(dstTopRight.X - dstTopLeft.X) / srcRect.Width;
            float m10 = (float)(dstTopRight.Y - dstTopLeft.Y) / srcRect.Width;
            float m01 = (float)(dstBottomLeft.X - dstTopLeft.X) / srcRect.Height;
            float m11 = (float)(dstBottomLeft.Y - dstTopLeft.Y) / srcRect.Height;
            float m02 = dstTopLeft.X - (float)((float)(m00 * srcRect.Left) + (float)(m01 * srcRect.Top));
            float m12 = dstTopLeft.Y - (float)((float)(m10 * srcRect.Left) + (float)(m11 * srcRect.Top));

            return new DrMatrix(m00, m10, m01, m11, m02, m12);
        }

        /// <summary>
        /// Applies the geometric transform represented by this Matrix to the specified point list.
        /// </summary>
        public void TransformPoints(PointFList points)
        {
            if (IsIdentity)
                return;

            for (int i = 0; i < points.Count; i++)
                points[i] = TransformPoint(points[i]);
        }

        /// <summary>
        /// Applies the geometric transform represented by this Matrix to 
        /// a specified part of array of points.
        /// </summary>
        public void TransformPoints(PointF[] points, int startIndex, int numberOfPoints)
        {
            if (IsIdentity)
                return;

            int end = startIndex + numberOfPoints;
            if (end > points.Length)
                end = points.Length;

            for (int index = startIndex; index < end; index++)
            {
                points[index] = TransformPoint(points[index]);
            }
        }

        /// <summary>
        /// Applies the geometric transform represented by this Matrix to 
        /// a specified point.
        /// </summary>
        public PointF TransformPoint(PointF point)
        {
            PointF result = new PointF(point.X, point.Y);
            int state = mState;

            //Used 'points[index]' for assignment since PointF is value type in .Net.
            float x = result.X;
            float y = result.Y;
            switch (state)
            {
                case (ApplyShear | ApplyScale | ApplyTranslate):
                    result = new PointF(((float)(x * Me00) + (float)(y * Me01)) + Me02, // Casting for Java.
                                        ((float)(x * Me10) + (float)(y * Me11)) + Me12); // Casting for Java.
                    break;
                case (ApplyShear | ApplyScale):
                    result = new PointF((float)(x * Me00) + (float)(y * Me01), // Casting for Java.
                                        (float)(x * Me10) + (float)(y * Me11)); // Casting for Java.
                    break;
                case (ApplyShear | ApplyTranslate):
                    result = new PointF((float)(y * Me01) + Me02, // Casting for Java.
                                        (float)(x * Me10) + Me12); // Casting for Java.
                    break;
                case (ApplyShear):
                    result = new PointF(y * Me01,
                                        x * Me10);
                    break;
                case (ApplyScale | ApplyTranslate):
                    result = new PointF((float)(x * Me00) + Me02, // Casting for Java.
                                        (float)(y * Me11) + Me12); // Casting for Java.
                    break;
                case (ApplyScale):
                    result = new PointF(x * Me00,
                                        y * Me11);
                    break;
                case (ApplyTranslate):
                    result = new PointF(x + Me02,
                                        y + Me12);
                    break;
                case (ApplyIdentity):
                    // do nothing
                    break;
                default:
                    /* NOTREACHED */
                    StateError();
                    break;
            }
            return result;
        }

        /// <summary>
        /// Applies the geometric transform represented by this Matrix to a specified array of points.
        /// </summary>
        public void TransformPoints(PointF[] points)
        {
            TransformPoints(points, 0, points.Length);
        }

        /// <summary>
        /// Decompose source matrix (M) to four matrices so
        /// M = S * Hx * R * T. Where:
        /// S  - scale matrix;
        /// Hx - shear along X-axis matrix;
        /// R  - rotation matrix;
        /// T  - translate matrix.
        /// Function returns array of coefficients in that order:
        /// [scaleX, scaleY, shearX, angle (in degrees), translateX, translateY].
        /// Decomposition is only possible if the determinant of the matrix is non-zero.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// In case the matrix has zero determinant and thus can't be decomposed.
        /// </exception>
        public float[] DecomposeMatrix()
        {
            float[] result = new float[6];
            DecomposeMatrix(result);
            return result;
        }

        /// <summary>
        /// Decompose source matrix (M) to four matrices so
        /// M = S * Hx * R * T. Where:
        /// S  - scale matrix;
        /// Hx - shear along X-axis matrix;
        /// R  - rotation matrix;
        /// T  - translate matrix.
        /// Function fills the array of coefficients in that order:
        /// [scaleX, scaleY, shearX, angle (in degrees), translateX, translateY].
        /// Decomposition is only possible if the determinant of the matrix is non-zero.
        /// </summary>
        /// <param name="coefficients">
        /// An array of at least six elements where coefficients will be written to.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// In case the matrix has zero determinant and thus can't be decomposed.
        /// </exception>
        public void DecomposeMatrix(float[] coefficients)
        {
            if (HasZeroDeterminant)
                throw new InvalidOperationException("Can't decompose the matrix, because its determinant is zero.");
            double det = Determinant;

            double r, sx, sy, hx;
            if (MathUtil.AreEqual(M22, 0))
            {
                r = 90;
                sy = -M21;
                sx = M12;
                hx = -M11 / M12;
            }
            else
            {
                double rRad = Math.Atan(-M21 / M22);
                sy = M22 / Math.Cos(rRad);
                r = MathUtil.RadiansToDegrees(rRad);
                sx = det / sy;
                hx = (M11 * M21 + M12 * M22) / det;
            }

            coefficients[0] = (float)sx;
            coefficients[1] = (float)sy;
            coefficients[2] = (float)hx;
            coefficients[3] = (float)r;
            coefficients[4] = M31;
            coefficients[5] = M32;
        }

        /// <summary>
        /// Decompose source matrix (M) to four matrices so
        /// M = T * S * Hx * R. Where:
        /// S  - scale matrix;
        /// Hx - shear along X-axis matrix;
        /// R  - rotation matrix.
        /// T  - translate matrix.
        /// Function returns array of coefficients in that order:
        /// [scaleX, scaleY, shearX, angle (in degrees), translateX, translateY].
        /// Decomposition is only possible if the determinant of the matrix is non-zero.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// In case the matrix has zero determinant and thus can't be decomposed.
        /// </exception>
        public float[] DecomposeMatrixTshr()
        {
            float[] components = DecomposeMatrix();

            double determinant = Determinant;
            components[4] = (float)((M22 * M31 - M21 * M32) / determinant);
            components[5] = (float)((M11 * M32 - M12 * M31) / determinant);
            return components;
        }

        /// <summary>
        /// Returns the determinant of this transform matrix. If the determinant is
        /// non-zero, the transform is invertible; otherwise operations which require
        /// an inversion throw a NoninvertibleTransformException. A result very near
        /// zero, due to rounding errors, may indicate that inversion results do not
        /// carry enough precision to be meaningful.
        /// If this is a uniform scale transformation, the determinant also
        /// represents the squared value of the scale. Otherwise, it carries little
        /// additional meaning. The determinant is calculated as:
        ///
        /// | m00 m01 m02 |
        /// | m10 m11 m12 | = m00 * m11 - m01 * m10
        /// |  0   0   1  |
        /// </summary>
        public double Determinant
        {
            get { return Me00 * Me11 - Me01 * Me10; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Determinant"/> value of this matrix is equal to zero.
        /// </summary>
        /// <remarks>
        /// A matrix that has zero determinant can't be inverted or decomposed.
        /// </remarks>
        public bool HasZeroDeterminant
        {
            get { return MathUtil.IsZero(Determinant); }
        }

        /// <summary>
        /// Returns a transform, which if concatenated to this one, will result in
        /// the identity transform. This is useful for undoing transformations, but
        /// is only possible if the original transform has an inverse (ie. does not
        /// map multiple points to the same line or point). A transform exists only
        /// if getDeterminant() has a non-zero value.
        /// </summary>
        public DrMatrix CreateInverseMatrix()
        {
            if (HasZeroDeterminant)
                throw new InvalidOperationException("Can't inverse the matrix, because its determinant is zero.");

            double det = Determinant;
            double im00 = Me11 / det;
            double im10 = -Me10 / det;
            double im01 = -Me01 / det;
            double im11 = Me00 / det;
            double im02 = (Me01 * Me12 - Me02 * Me11) / det;
            double im12 = (-Me00 * Me12 + Me10 * Me02) / det;
            return new DrMatrix((float)im00, (float)im10, (float)im01, (float)im11, (float)im02, (float)im12);
        }

        /// <summary>
        /// Applies the geometric transform represented by this Matrix to a specified rectangle.
        /// </summary>
        public RectangleF Transform(RectangleF rect)
        {
            PointF tl = TransformPoint(new PointF(rect.Left, rect.Top));
            PointF br = TransformPoint(new PointF(rect.Right, rect.Bottom));
            // WORDSNET-7174.
            // The top left point of a source rectangle is
            // not always top left for a transformed rectangle.
            // The same applies to bottom right point.
            // So create rectangle from two new points.
            return GeometryUtil.GetBounds(new PointF[] { tl, br });
        }

        /// <summary>
        /// Transforms specified rectangle to parallelogram.
        /// </summary>
        /// <param name="rect">Rectangle to transform.</param>
        /// <returns>
        /// Array of 4 paralelogram vertices.
        /// Paralelogram vertices corresponds to rectangle vertices if following order:
        /// - top left;
        /// - top right;
        /// - bottom right;
        /// - bottom left.
        /// </returns>
        /// <remarks>
        /// In general case rectangle is translated to parallelogram. Previous method works well only for translate and scale matrices.
        /// </remarks>
        public PointF[] TransformToParallelogram(RectangleF rect)
        {
            PointF[] points = new PointF[4];
            points[0] = new PointF(rect.Left, rect.Top);
            points[1] = new PointF(rect.Right, rect.Top);
            points[2] = new PointF(rect.Right, rect.Bottom);
            points[3] = new PointF(rect.Left, rect.Bottom);
            TransformPoints(points);
            return points;
        }

        /// <summary>
        /// Applies the specified scale vector (the same scaleX and scaleY) to this Matrix using (default) Prepend order.
        /// </summary>
        /// <remarks>Overload added to fix SQ warnings.</remarks>
        public void Scale(float scaleFactor, MatrixOrder order)
        {
            float scaleX = scaleFactor;
            float scaleY = scaleFactor;
            Scale(scaleX, scaleY, order);
        }

        /// <summary>
        /// Applies the specified scale vector (scaleX and scaleY) to this Matrix using the specified order.
        /// </summary>
        public void Scale(float scaleX, float scaleY, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
            {
                PrependScale(scaleX, scaleY);
                return;
            }

            //MatrixOrder.Append

            // Only these two existing states need a new state
            if ((mState == ApplyTranslate) || (mState == ApplyIdentity))
                mState |= ApplyScale;

            if ((mState & ApplyShear) != 0)
            {
                Me01 *= scaleX;
                Me10 *= scaleY;
                if ((mState & ApplyScale) != 0)
                {
                    Me00 *= scaleX;
                    Me11 *= scaleY;
                }
            }
            else
            {
                Me00 *= scaleX;
                Me11 *= scaleY;
            }

            if ((mState & ApplyTranslate) != 0)
            {
                Me02 *= scaleX;
                Me12 *= scaleY;
            }

            mType = TypeUnknown;
            return;
        }

        /// <summary>
        /// Applies the specified scale vector (scaleX and scaleY) to this Matrix using (default) Prepend order.
        /// </summary>
        private void PrependScale(float sx, float sy)
        {
            int state = mState;

            //added to delete fall-throw from switch
            if ((state & ApplyScale) != 0)
            {
                Me00 *= sx;
                Me11 *= sy;
            }

            switch (state)
            {
                case (ApplyShear | ApplyScale | ApplyTranslate):
                case (ApplyShear | ApplyScale):
                case (ApplyShear | ApplyTranslate):
                case (ApplyShear):
                    Me01 *= sy;
                    Me10 *= sx;
                    if (Me01 == 0f && Me10 == 0f)
                    {
                        state &= ApplyTranslate;
                        if (Me00 == 1f && Me11 == 1f)
                        {
                            mType = (state == ApplyIdentity
                                         ? TypeIdentity
                                         : TypeTranslation);
                        }
                        else
                        {
                            state |= ApplyScale;
                            mType = TypeUnknown;
                        }
                        mState = state;
                    }
                    return;
                case (ApplyScale | ApplyTranslate):
                case (ApplyScale):
                    if (Me00 == 1f && Me11 == 1f)
                    {
                        state &= ApplyTranslate;
                        mState = state;
                        mType = (state == ApplyIdentity
                                     ? TypeIdentity
                                     : TypeTranslation);
                    }
                    else
                    {
                        mType = TypeUnknown;
                    }
                    return;
                case (ApplyTranslate):
                case (ApplyIdentity):
                    Me00 = sx;
                    Me11 = sy;
                    if (sx != 1f || sy != 1f)
                    {
                        mState = state | ApplyScale;
                        mType = TypeUnknown;
                    }
                    return;
                default:
                    /* NOTREACHED */
                    StateError();
                    break;
            }
        }

        /// <summary>
        /// Applies the specified translation vector to this Matrix in the specified order.
        /// </summary>
        public void Translate(float offsetX, float offsetY, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
            {
                PrependTranslate(offsetX, offsetY);
                return;
            }

            //MatrixOrder.Append

            switch (mState)
            {
                case (ApplyIdentity):
                case (ApplyScale):
                case (ApplyShear):
                case (ApplyShear | ApplyScale):
                    //this has no TRANSLATE
                    Me02 = offsetX;
                    Me12 = offsetY;
                    mState |= ApplyTranslate;
                    mType |= TypeTranslation;
                    break;

                case (ApplyTranslate):
                case (ApplyScale | ApplyTranslate):
                case (ApplyShear | ApplyTranslate):
                case (ApplyShear | ApplyScale | ApplyTranslate):
                    //this has one TRANSLATE too
                    Me02 = Me02 + offsetX;
                    Me12 = Me12 + offsetY;
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        /// <summary>
        /// Applies the specified translation vector to this Matrix using (default) Prepend order.
        /// </summary>
        private void PrependTranslate(float tx, float ty)
        {
            switch (mState)
            {
                case (ApplyShear | ApplyScale | ApplyTranslate):
                    Me02 = (float)((float)(tx * Me00) + (float)(ty * Me01)) + Me02;
                    Me12 = (float)((float)(tx * Me10) + (float)(ty * Me11)) + Me12;
                    if (Me02 == 0.0 && Me12 == 0.0)
                    {
                        mState = ApplyShear | ApplyScale;
                        if (mType != TypeUnknown)
                        {
                            mType -= TypeTranslation;
                        }
                    }
                    return;
                case (ApplyShear | ApplyScale):
                    Me02 = (float)(tx * Me00) + (float)(ty * Me01);
                    Me12 = (float)(tx * Me10) + (float)(ty * Me11);
                    if (Me02 != 0.0 || Me12 != 0.0)
                    {
                        mState = ApplyShear | ApplyScale | ApplyTranslate;
                        mType |= TypeTranslation;
                    }
                    return;
                case (ApplyShear | ApplyTranslate):
                    Me02 = (float)(ty * Me01) + Me02;
                    Me12 = (float)(tx * Me10) + Me12;
                    if (Me02 == 0.0 && Me12 == 0.0)
                    {
                        mState = ApplyShear;
                        if (mType != TypeUnknown)
                        {
                            mType -= TypeTranslation;
                        }
                    }
                    return;
                case (ApplyShear):
                    Me02 = ty * Me01;
                    Me12 = tx * Me10;
                    if (Me02 != 0.0 || Me12 != 0.0)
                    {
                        mState = ApplyShear | ApplyTranslate;
                        mType |= TypeTranslation;
                    }
                    return;
                case (ApplyScale | ApplyTranslate):
                    Me02 = (float)(tx * Me00) + Me02;  // Casting for Java.
                    Me12 = (float)(ty * Me11) + Me12;  // Casting for Java.
                    if (Me02 == 0.0 && Me12 == 0.0)
                    {
                        mState = ApplyScale;
                        if (mType != TypeUnknown)
                        {
                            mType -= TypeTranslation;
                        }
                    }
                    return;
                case (ApplyScale):
                    Me02 = tx * Me00;
                    Me12 = ty * Me11;
                    if (Me02 != 0.0 || Me12 != 0.0)
                    {
                        mState = ApplyScale | ApplyTranslate;
                        mType |= TypeTranslation;
                    }
                    return;
                case (ApplyTranslate):
                    Me02 = tx + Me02;
                    Me12 = ty + Me12;
                    if (Me02 == 0f && Me12 == 0f)
                    {
                        mState = ApplyIdentity;
                        mType = TypeIdentity;
                    }
                    return;
                case (ApplyIdentity):
                    Me02 = tx;
                    Me12 = ty;
                    if (tx != 0f || ty != 0f)
                    {
                        mState = ApplyTranslate;
                        mType = TypeTranslation;
                    }
                    return;
                default:
                    /* NOTREACHED */
                    StateError();
                    break;
            }
        }

        /// <summary>
        /// Multiplies this Matrix by the matrix specified in the matrix parameter, and in the order specified in the order parameter.
        /// </summary>
        public void Multiply(DrMatrix Tx, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
            {
                PrependMultiply(Tx);
                return;
            }

            //MatrixOrder.Append

            float m0, m1;
            float t00, t01, t10, t11;
            float t02, t12;
            int mystate = mState;
            int txstate = Tx.mState;
            switch (txstate)
            {
                case ApplyIdentity:
                    return;
                case ApplyTranslate:
                    switch (mystate)
                    {
                        case (ApplyIdentity):
                        case (ApplyScale):
                        case (ApplyShear):
                        case (ApplyShear | ApplyScale):
                            // Tx is TRANSLATE, this has no TRANSLATE
                            Me02 = Tx.Me02;
                            Me12 = Tx.Me12;
                            mState = mystate | ApplyTranslate;
                            mType |= TypeTranslation;
                            break;
                        case (ApplyTranslate):
                        case (ApplyScale | ApplyTranslate):
                        case (ApplyShear | ApplyTranslate):
                        case (ApplyShear | ApplyScale | ApplyTranslate):
                            // Tx is TRANSLATE, this has one too
                            Me02 = Me02 + Tx.Me02;
                            Me12 = Me12 + Tx.Me12;
                            break;
                        default:
                            // do nothing
                            break;
                    }
                    return;
                case ApplyScale:
                    // Only these two existing states need a new state
                    if ((mystate == ApplyTranslate) || (mystate == ApplyIdentity))
                        mState = mystate | ApplyScale;

                    // Tx is SCALE, this is anything
                    t00 = Tx.Me00;
                    t11 = Tx.Me11;
                    if ((mystate & ApplyShear) != 0)
                    {
                        Me01 = Me01 * t00;
                        Me10 = Me10 * t11;
                        if ((mystate & ApplyScale) != 0)
                        {
                            Me00 = Me00 * t00;
                            Me11 = Me11 * t11;
                        }
                    }
                    else
                    {
                        Me00 = Me00 * t00;
                        Me11 = Me11 * t11;
                    }
                    if ((mystate & ApplyTranslate) != 0)
                    {
                        Me02 = Me02 * t00;
                        Me12 = Me12 * t11;
                    }
                    mType = TypeUnknown;
                    return;
                case ApplyShear:
                    if (mystate == (ApplyShear | ApplyTranslate) ||
                        mystate == ApplyShear)
                    {
                        mystate = mystate | ApplyScale;
                        mState = mystate ^ ApplyShear;
                    }

                    if (mystate == ApplyTranslate ||
                        mystate == ApplyIdentity ||
                        mystate == (ApplyScale | ApplyTranslate) ||
                        mystate == ApplyScale)
                    {
                        mState = mystate ^ ApplyShear;
                    }

                    t01 = Tx.Me01;
                    t10 = Tx.Me10;

                    m0 = Me00;
                    Me00 = Me10 * t01;
                    Me10 = m0 * t10;

                    m0 = Me01;
                    Me01 = Me11 * t01;
                    Me11 = m0 * t10;

                    m0 = Me02;
                    Me02 = Me12 * t01;
                    Me12 = m0 * t10;
                    mType = TypeUnknown;
                    return;
                default:
                    // do nothing
                    break;
            }

            // If Tx has more than one attribute, it is not worth optimizing
            // all of those cases...
            t00 = Tx.Me00;
            t01 = Tx.Me01;
            t02 = Tx.Me02;
            t10 = Tx.Me10;
            t11 = Tx.Me11;
            t12 = Tx.Me12;
            //extracted here to remove fall-throws
            if ((mystate & ApplyTranslate) != 0)
            {
                m0 = Me02;
                m1 = Me12;
                t02 = (float)(t02 + (float)(m0 * t00)) + (float)(m1 * t01); // Casting for Java.
                t12 = (float)(t12 + (float)(m0 * t10)) + (float)(m1 * t11); // Casting for Java.
            }
            switch (mystate)
            {
                case (ApplyShear | ApplyScale | ApplyTranslate):
                case (ApplyShear | ApplyScale):
                    Me02 = t02;
                    Me12 = t12;

                    m0 = Me00;
                    m1 = Me10;
                    Me00 = (float)(m0 * t00) + (float)(m1 * t01);
                    Me10 = (float)(m0 * t10) + (float)(m1 * t11);

                    m0 = Me01;
                    m1 = Me11;
                    Me01 = (float)(m0 * t00) + (float)(m1 * t01);
                    Me11 = (float)(m0 * t10) + (float)(m1 * t11);
                    break;

                case (ApplyShear | ApplyTranslate):
                case (ApplyShear):
                    Me02 = t02;
                    Me12 = t12;

                    m0 = Me10;
                    Me00 = m0 * t01;
                    Me10 = m0 * t11;

                    m0 = Me01;
                    Me01 = m0 * t00;
                    Me11 = m0 * t10;
                    break;

                case (ApplyScale | ApplyTranslate):
                case (ApplyScale):
                    Me02 = t02;
                    Me12 = t12;

                    m0 = Me00;
                    Me00 = m0 * t00;
                    Me10 = m0 * t10;

                    m0 = Me11;
                    Me01 = m0 * t01;
                    Me11 = m0 * t11;
                    break;

                case (ApplyTranslate):
                case (ApplyIdentity):
                    Me02 = t02;
                    Me12 = t12;

                    Me00 = t00;
                    Me10 = t10;

                    Me01 = t01;
                    Me11 = t11;

                    mState = mystate | txstate;
                    mType = TypeUnknown;
                    return;
                default:
                    /* NOTREACHED */
                    StateError();
                    break;
            }
            UpdateState();
        }

        /// <summary>
        /// Multiplies this Matrix by the matrix specified in the matrix parameter using (default) Prepend order.
        /// </summary>
        private void PrependMultiply(DrMatrix Tx)
        {
            //analog of java AffineTransform.concatenate()
            float M0, M1;
            float T00, T01, T10, T11;
            float T02, T12;
            int mystate = mState;
            int txstate = Tx.mState;

            if (mystate == ApplyIdentity)
            {
                Me00 = Tx.Me00;
                Me11 = Tx.Me11;
                Me01 = Tx.Me01;
                Me10 = Tx.Me10;
                Me02 = Tx.Me02;
                Me12 = Tx.Me12;

                mState = txstate;
                mType = Tx.mType;
                return;
            }

            switch (txstate)
            {
                case ApplyIdentity:
                    return;
                case ApplyTranslate:
                    Translate(Tx.Me02, Tx.Me12, MatrixOrder.Prepend);
                    return;
                case ApplyScale:
                    Scale(Tx.Me00, Tx.Me11, MatrixOrder.Prepend);
                    return;
                case ApplyShear:
                    switch (mystate)
                    {
                        case (ApplyShear | ApplyScale | ApplyTranslate):
                        case (ApplyShear | ApplyScale):
                            T01 = Tx.Me01;
                            T10 = Tx.Me10;
                            M0 = Me00;
                            Me00 = Me01 * T10;
                            Me01 = M0 * T01;
                            M0 = Me10;
                            Me10 = Me11 * T10;
                            Me11 = M0 * T01;
                            mType = TypeUnknown;
                            return;
                        case (ApplyShear | ApplyTranslate):
                        case (ApplyShear):
                            Me00 = Me01 * Tx.Me10;
                            Me01 = 0f;
                            Me11 = Me10 * Tx.Me01;
                            Me10 = 0f;
                            mState = mystate ^ (ApplyShear | ApplyScale);
                            mType = TypeUnknown;
                            return;
                        case (ApplyScale | ApplyTranslate):
                        case (ApplyScale):
                            Me01 = Me00 * Tx.Me01;
                            Me00 = 0f;
                            Me10 = Me11 * Tx.Me10;
                            Me11 = 0f;
                            mState = mystate ^ (ApplyShear | ApplyScale);
                            mType = TypeUnknown;
                            return;
                        case (ApplyTranslate):
                            Me00 = 0f;
                            Me01 = Tx.Me01;
                            Me10 = Tx.Me10;
                            Me11 = 0f;
                            mState = ApplyTranslate | ApplyShear;
                            mType = TypeUnknown;
                            return;
                        default:
                            // do nothing
                            break;
                    }
                    break;
                default:
                    // do nothing
                    break;
            }

            // If Tx has more than one attribute, it is not worth optimizing
            // all of those cases...
            T00 = Tx.Me00;
            T01 = Tx.Me01;
            T02 = Tx.Me02;
            T10 = Tx.Me10;
            T11 = Tx.Me11;
            T12 = Tx.Me12;
            switch (mystate)
            {
                case (ApplyShear | ApplyScale):
                case (ApplyShear | ApplyScale | ApplyTranslate):
                    //to remove fall-throw
                    if (mystate == (ApplyShear | ApplyScale))
                        mState = mystate | txstate;
                    M0 = Me00;
                    M1 = Me01;
                    Me00 = (float)(T00 * M0) + (float)(T10 * M1);                // Casting for Java.
                    Me01 = (float)(T01 * M0) + (float)(T11 * M1);
                    Me02 = (float)(Me02 + (float)(T02 * M0)) + (float)(T12 * M1);

                    M0 = Me10;
                    M1 = Me11;
                    Me10 = (float)(T00 * M0) + (float)(T10 * M1);
                    Me11 = (float)(T01 * M0) + (float)(T11 * M1);
                    Me12 = (float)(Me12 + (float)(T02 * M0)) + (float)(T12 * M1);
                    mType = TypeUnknown;
                    return;

                case (ApplyShear | ApplyTranslate):
                case (ApplyShear):
                    M0 = Me01;
                    Me00 = T10 * M0;
                    Me01 = T11 * M0;
                    Me02 = Me02 + (float)(T12 * M0);

                    M0 = Me10;
                    Me10 = T00 * M0;
                    Me11 = T01 * M0;
                    Me12 = Me12 + (float)(T02 * M0);
                    break;

                case (ApplyScale | ApplyTranslate):
                case (ApplyScale):
                    M0 = Me00;
                    Me00 = T00 * M0;
                    Me01 = T01 * M0;
                    Me02 = Me02 + (float)(T02 * M0);

                    M0 = Me11;
                    Me10 = T10 * M0;
                    Me11 = T11 * M0;
                    Me12 = Me12 + (float)(T12 * M0);
                    break;

                case (ApplyTranslate):
                    Me00 = T00;
                    Me01 = T01;
                    Me02 = Me02 + T02;

                    Me10 = T10;
                    Me11 = T11;
                    Me12 = Me12 + T12;
                    mState = txstate | ApplyTranslate;
                    mType = TypeUnknown;
                    return;
                default:
                    /* NOTREACHED */
                    StateError();
                    break;
            }
            UpdateState();
        }

        /// <summary>
        /// Applies a clockwise rotation of an amount specified in the angle parameter in degrees, around the origin
        ///  (zero x and y coordinates) for this Matrix in the specified order.
        /// </summary>
        public void Rotate(float angle, MatrixOrder order)
        {
            angle = (float)MathUtil.NormalizeAngle(angle);
            if (MathUtil.IsZero(angle))
                return;

            if (order == MatrixOrder.Prepend)
            {
                PrependRotate(angle);
                return;
            }

            //MatrixOrder.Append

            if (angle == 90f || angle == -270f)
            {
                AppendRotate90();
            }
            else if (angle == -90f || angle == 270f)
            {
                AppendRotate270();
            }
            else if (angle == 180f || angle == -180f)
            {
                AppendRotate180();
            }
            else
            {
                double radians = MathUtil.DegreesToRadians(angle);
                double sin = System.Math.Sin(radians);
                double cos = System.Math.Cos(radians);
                if (cos != 1.0)//cos(0) == 1 --> do nothing.
                {
                    float M0, M1;
                    M0 = Me00;
                    M1 = Me10;
                    Me00 = (float)(cos * M0 - sin * M1);
                    Me10 = (float)(sin * M0 + cos * M1);
                    M0 = Me01;
                    M1 = Me11;
                    Me01 = (float)(cos * M0 - sin * M1);
                    Me11 = (float)(sin * M0 + cos * M1);
                    M0 = Me02;
                    M1 = Me12;
                    Me02 = (float)(cos * M0 - sin * M1);
                    Me12 = (float)(sin * M0 + cos * M1);
                    UpdateState();
                }
            }
        }

        /// <summary>
        /// Applies a clockwise rotation of an amount specified in the angle parameter in degrees, around the origin
        ///  (zero x and y coordinates) for this Matrix in the default (Prepend) order.
        /// </summary>
        private void PrependRotate(float angle)
        {
            if (angle == 0f)
                return;

            if (angle == 90f || angle == -270f)
            {
                Rotate90();
            }
            else if (angle == -90f || angle == 270f)
            {
                Rotate270();
            }
            else if (angle == 180f || angle == -180f)
            {
                Rotate180();
            }
            else
            {
                double radians = MathUtil.DegreesToRadians(angle);
                double sin = System.Math.Sin(radians);
                double cos = System.Math.Cos(radians);
                if (cos != 1.0)//cos(0) == 1 --> do nothing.
                {
                    float M0, M1;
                    M0 = Me00;
                    M1 = Me01;
                    Me00 = (float)(cos * M0 + sin * M1);
                    Me01 = (float)(-sin * M0 + cos * M1);
                    M0 = Me10;
                    M1 = Me11;
                    Me10 = (float)(cos * M0 + sin * M1);
                    Me11 = (float)(-sin * M0 + cos * M1);
                    UpdateState();
                }
            }
        }

        /// <summary>
        /// Applies a clockwise rotation about the specified point to this Matrix in the specified order.
        /// </summary>
        public void RotateAt(float angle, PointF point, MatrixOrder order)
        {
            if (order == MatrixOrder.Prepend)
            {
                PrependRotateAt(angle, point);
                return;
            }

            //MatrixOrder.Append

            Translate(-point.X, -point.Y, MatrixOrder.Append);
            Rotate(angle, MatrixOrder.Append);
            Translate(point.X, point.Y, MatrixOrder.Append);
        }

        /// <summary>
        /// Applies a clockwise rotation about the specified point to this Matrix in the default (Prepend) order.
        /// </summary>
        private void PrependRotateAt(float angle, PointF point)
        {
            // REMIND: Simple for now - optimize later
            Translate(point.X, point.Y, MatrixOrder.Prepend);
            Rotate(angle, MatrixOrder.Prepend);
            Translate(-point.X, -point.Y, MatrixOrder.Prepend);
        }

        //>>>>>>>>>> Utility methods to optimize rotate methods.

        //sin == 1, cos == 0.
        private void Rotate90()
        {
            float m = Me00;
            Me00 = Me01;
            Me01 = -m;
            m = Me10;
            Me10 = Me11;
            Me11 = -m;
            int state = Rot90Conversion[mState];
            if ((state & (ApplyShear | ApplyScale)) == ApplyScale &&
                Me00 == 1f && Me11 == 1f)
            {
                state -= ApplyScale;
            }
            mState = state;
            mType = TypeUnknown;
        }

        //sin == 0, cos == -1.
        private void Rotate180()
        {
            Me00 = -Me00;
            Me11 = -Me11;
            int state = mState;
            if ((state & (ApplyShear)) != 0)
            {
                // If there was a shear, then this rotation has no
                // effect on the state.
                Me01 = -Me01;
                Me10 = -Me10;
            }
            else
            {
                // No shear means the SCALE state may toggle when
                // m00 and m11 are negated.
                if (Me00 == 1f && Me11 == 1f)
                {
                    mState = state & ~ApplyScale;
                }
                else
                {
                    mState = state | ApplyScale;
                }
            }
            mType = TypeUnknown;
        }

        //sin == -1, cos == 0.
        private void Rotate270()
        {
            float m = Me00;
            Me00 = -Me01;
            Me01 = m;
            m = Me10;
            Me10 = -Me11;
            Me11 = m;
            int state = Rot90Conversion[mState];
            if ((state & (ApplyShear | ApplyScale)) == ApplyScale &&
                Me00 == 1f && Me11 == 1f)
            {
                state -= ApplyScale;
            }
            mState = state;
            mType = TypeUnknown;
        }

        //sin == 1, cos == 0.
        private void AppendRotate90()
        {
            float m = Me00;
            Me00 = -Me10;
            Me10 = m;
            m = Me01;
            Me01 = -Me11;
            Me11 = m;
            m = Me02;
            Me02 = -Me12;
            Me12 = m;

            UpdateState();
        }

        //sin == 0, cos == -1.
        private void AppendRotate180()
        {
            Me00 = -Me00;
            Me01 = -Me01;
            Me10 = -Me10;
            Me11 = -Me11;
            Me02 = -Me02;
            Me12 = -Me12;

            UpdateState();
        }

        //sin == -1, cos == 0.
        private void AppendRotate270()
        {
            float m = Me00;
            Me00 = Me10;
            Me10 = -m;
            m = Me01;
            Me01 = Me11;
            Me11 = -m;
            m = Me02;
            Me02 = Me12;
            Me12 = -m;

            UpdateState();
        }

        //<<<<<<<<<< Utility methods to optimize rotate methods.

        /// <summary>
        /// Resets this Matrix to have the elements of the identity matrix.
        /// </summary>
        public void Reset()
        {
            Me00 = 1f;
            Me11 = 1f;
            Me10 = 0f;
            Me01 = 0f;
            Me02 = 0f;
            Me12 = 0f;
            mState = ApplyIdentity;
            mType = TypeIdentity;
        }

        /// <summary>
        /// Returns deep copy of this instance.
        /// </summary>
        public DrMatrix Clone()
        {
            DrMatrix clone = new DrMatrix();
            clone.Me00 = Me00;
            clone.Me10 = Me10;
            clone.Me01 = Me01;
            clone.Me11 = Me11;
            clone.Me02 = Me02;
            clone.Me12 = Me12;
            clone.mState = mState;
            clone.mType = mType;

            return clone;
        }

        /// <summary>
        /// Locks the matrix.
        /// </summary>
        /// <remarks>
        /// It will not be possible to transform the matrix after locking. Any transformations will throw an exception.
        /// If it is needed to transform the locked matrix, it is recommended to clone it and work with cloned matrix.
        /// </remarks>
        public void Lock()
        {
            Locked = true;
        }

        /// <summary>
        /// Indicates whether the matrix is locked.
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// Throws an exception if the matrix is locked.
        /// </summary>
        private void ThrowIfLocked()
        {
            if (Locked)
                throw new InvalidOperationException("Locked matrix cannot be changed.");
        }

        public override int GetHashCode()
        {
            //.Net float.GetHashCode() gets int bits of the float i.e. the same as java Float.getIntBits(float).
            long bits = Me00.GetHashCode();
            bits = bits * 31 + Me01.GetHashCode();
            bits = bits * 31 + Me02.GetHashCode();
            bits = bits * 31 + Me10.GetHashCode();
            bits = bits * 31 + Me11.GetHashCode();
            bits = bits * 31 + Me12.GetHashCode();
            return ((int)bits) ^ ((int)(bits >> 32));
        }

        public override bool Equals(object obj)
        {
            DrMatrix matrix = obj as DrMatrix;
            return (matrix != null) && (Equals(this, matrix));
        }

        /// <summary>
        /// The actual implementation.
        /// </summary>
        public static bool Equals(DrMatrix a, DrMatrix b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(null, a))
                return false;

            if (ReferenceEquals(null, b))
                return false;

            return
                (a.Me00 == b.Me00) &&
                (a.Me10 == b.Me10) &&
                (a.Me01 == b.Me01) &&
                (a.Me11 == b.Me11) &&
                (a.Me02 == b.Me02) &&
                (a.Me12 == b.Me12);
        }

        /// <summary>
        /// Returns true if all components of the specified matrices are pairwise equal, ignoring the translation values.
        /// </summary>
        public static bool EqualsIgnoreTranslation(DrMatrix a, DrMatrix b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(null, a))
                return false;

            if (ReferenceEquals(null, b))
                return false;

            return
                MathUtil.AreEqual(a.Me00, b.Me00) &&
                MathUtil.AreEqual(a.Me10, b.Me10) &&
                MathUtil.AreEqual(a.Me01, b.Me01) &&
                MathUtil.AreEqual(a.Me11, b.Me11);
        }

        public static bool operator ==(DrMatrix a, DrMatrix b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(DrMatrix a, DrMatrix b)
        {
            return !Equals(a, b);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}", Me00, Me10, Me01, Me11, Me02, Me12);
        }

        /**
         * Manually recalculates the state of the transform when the matrix
         * changes too much to predict the effects on the state.
         * The following table specifies what the various settings of the
         * state field say about the values of the corresponding matrix
         * element fields.
         * Note that the rules governing the SCALE fields are slightly
         * different depending on whether the SHEAR flag is also set.
         * <pre>
         *                     SCALE            SHEAR          TRANSLATE
         *                    m00/m11          m01/m10          m02/m12
         *
         * IDENTITY             1.0              0.0              0.0
         * TRANSLATE (TR)       1.0              0.0          not both 0.0
         * SCALE (SC)       not both 1.0         0.0              0.0
         * TR | SC          not both 1.0         0.0          not both 0.0
         * SHEAR (SH)           0.0          not both 0.0         0.0
         * TR | SH              0.0          not both 0.0     not both 0.0
         * SC | SH          not both 0.0     not both 0.0         0.0
         * TR | SC | SH     not both 0.0     not both 0.0     not both 0.0
         * </pre>
         */
        private void UpdateState()
        {
            if (Me01 == 0f && Me10 == 0f)
            {
                if (Me00 == 1f && Me11 == 1f)
                {
                    if (Me02 == 0f && Me12 == 0f)
                    {
                        mState = ApplyIdentity;
                        mType = TypeIdentity;
                    }
                    else
                    {
                        mState = ApplyTranslate;
                        mType = TypeTranslation;
                    }
                }
                else
                {
                    if (Me02 == 0f && Me12 == 0f)
                    {
                        mState = ApplyScale;
                        mType = TypeUnknown;
                    }
                    else
                    {
                        mState = (ApplyScale | ApplyTranslate);
                        mType = TypeUnknown;
                    }
                }
            }
            else
            {
                if (Me00 == 0f && Me11 == 0f)
                {
                    if (Me02 == 0f && Me12 == 0f)
                    {
                        mState = ApplyShear;
                        mType = TypeUnknown;
                    }
                    else
                    {
                        mState = (ApplyShear | ApplyTranslate);
                        mType = TypeUnknown;
                    }
                }
                else
                {
                    if (Me02 == 0f && Me12 == 0f)
                    {
                        mState = (ApplyShear | ApplyScale);
                        mType = TypeUnknown;
                    }
                    else
                    {
                        mState = (ApplyShear | ApplyScale | ApplyTranslate);
                        mType = TypeUnknown;
                    }
                }
            }
        }

        /*
         * Convenience method used internally to throw exceptions when
         * a case was forgotten in a switch statement.
         */
        private static void StateError()
        {
            throw new InvalidOperationException("Missing case in transform state switch.");
        }

        //Getters for matrix elements added for MatrixPal (to initiate new native Matrix)
        //and for SwfBinaryWriter.
        // Java's names for matrix elements used internally. 
        // Map of java names to .net ones to description:
        // Me00     M11     Scale X
        // Me10     M12     Shear Y
        // Me01     M21     Shear X
        // Me11     M22     Scale Y
        // Me02     M31     Translate X
        // Me12     M32     Translate Y
        public float M11
        {
            get { return Me00; }
        }

        public float M12
        {
            get { return Me10; }
        }

        public float M21
        {

            get { return Me01; }
        }

        public float M22
        {

            get { return Me11; }
        }

        public float M31
        {

            get { return Me02; }
        }

        public float M32
        {

            get { return Me12; }
        }

        /// ScaleX      m11
        /**
         * The X coordinate scaling element of the 3x3
         * affine transformation matrix.
         */
        private float Me00
        {
            get { return mMatrixElem00; }
            set
            {
                ThrowIfLocked();
                mMatrixElem00 = value;
            }
        }

        /// ShearY      m12
        /**
         * The Y coordinate shearing element of the 3x3
         * affine transformation matrix.
         */
        private float Me10
        {
            get { return mMatrixElem10; }
            set
            {
                ThrowIfLocked();
                mMatrixElem10 = value;
            }
        }

        /// ShearX      m21
        /**
         * The X coordinate shearing element of the 3x3
         * affine transformation matrix.
         */
        private float Me01
        {
            get { return mMatrixElem01; }
            set
            {
                ThrowIfLocked();
                mMatrixElem01 = value;
            }
        }

        /// ScaleY      m22
        /**
         * The Y coordinate scaling element of the 3x3
         * affine transformation matrix.
         */
        private float Me11
        {
            get { return mMatrixElem11; }
            set
            {
                ThrowIfLocked();
                mMatrixElem11 = value;
            }
        }

        /// OffsetX     m31 (dx)
        /**
         * The X coordinate of the translation element of the
         * 3x3 affine transformation matrix.
         */
        private float Me02
        {
            get { return mMatrixElem02; }
            set
            {
                ThrowIfLocked();
                mMatrixElem02 = value;
            }
        }

        /// OffsetY     m32 (dy)
        /**
         * The Y coordinate of the translation element of the
         * 3x3 affine transformation matrix.
         */
        private float Me12
        {
            get { return mMatrixElem12; }
            set
            {
                ThrowIfLocked();
                mMatrixElem12 = value;
            }
        }

        /**
         * Backing fields for the properties Me00 ... Me12.
         * Do not use these fields directly to support immutability of the matrix in the Locked state.
         */
        private float mMatrixElem00;
        private float mMatrixElem10;
        private float mMatrixElem01;
        private float mMatrixElem11;
        private float mMatrixElem02;
        private float mMatrixElem12;

        /**
         * This field keeps track of which components of the matrix need to
         * be applied when performing a transformation.
         * @see #ApplyIdentity
         * @see #ApplyTranslate
         * @see #ApplyScale
         * @see #ApplyShear
         */
        private int mState;

        /**
         * This field caches the current transformation type of the matrix.
         * @see #TypeIdentity
         * @see #TypeTranslation
         * @see #TypeUniformScale
         * @see #TypeGeneralScale
         * @see #TypeFlip
         * @see #TypeQuadrantRotation
         * @see #TypeGeneralRotation
         * @see #TypeGeneralTransform
         * @see #TypeUnknown
         */
        private int mType;

        //>>>>>>>>>> Transformation type constants. Used for speed optimization.

        /*
         * This constant is only useful for the cached type field.
         * It indicates that the type has been decached and must be recalculated.
         */
        private const int TypeUnknown = -1;

        /**
         * An identity transform is one in which the output coordinates are
         * always the same as the input coordinates.
         * If this transform is anything other than the identity transform,
         * the type will either be the constant GENERAL_TRANSFORM or a
         * combination of the appropriate flag bits for the various coordinate
         * conversions that this transform performs.
         */
        public const int TypeIdentity = 0;

        /**
         * A translation moves the coordinates by a constant amount in x
         * and y without changing the length or angle of vectors.
         */
        public const int TypeTranslation = 1;

        /**
         * A uniform scale multiplies the length of vectors by the same amount
         * in both the x and y directions without changing the angle between
         * vectors.
         * This flag bit is mutually exclusive with the TypeGeneralScale flag.
         */
        public const int TypeUniformScale = 2;

        /**
         * A general scale multiplies the length of vectors by different
         * amounts in the x and y directions without changing the angle
         * between perpendicular vectors.
         * This flag bit is mutually exclusive with the TypeUniformScale flag.
         */
        public const int TypeGeneralScale = 4;

        /**
         * This constant is a bit mask for any of the scale flag bits.
         */
        public const int TypeMaskScale = (TypeUniformScale |
                               TypeGeneralScale);

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a mirror image flip about some axis which changes the
         * normally right handed coordinate system into a left handed
         * system in addition to the conversions indicated by other flag bits.
         * A right handed coordinate system is one where the positive X
         * axis rotates counterclockwise to overlay the positive Y axis
         * similar to the direction that the fingers on your right hand
         * curl when you stare end on at your thumb.
         * A left handed coordinate system is one where the positive X
         * axis rotates clockwise to overlay the positive Y axis similar
         * to the direction that the fingers on your left hand curl.
         * There is no mathematical way to determine the angle of the
         * original flipping or mirroring transformation since all angles
         * of flip are identical given an appropriate adjusting rotation.
         */
        public const int TypeFlip = 64;
        /* NOTE: TypeFlip was added after GENERAL_TRANSFORM was in public
         * circulation and the flag bits could no longer be conveniently
         * renumbered without introducing binary incompatibility in outside
         * code.
         */

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a quadrant rotation by some multiple of 90 degrees in
         * addition to the conversions indicated by other flag bits.
         * A rotation changes the angles of vectors by the same amount
         * regardless of the original direction of the vector and without
         * changing the length of the vector.
         * This flag bit is mutually exclusive with the TypeGeneralRotation flag.
         */
        public const int TypeQuadrantRotation = 8;

        /**
         * This flag bit indicates that the transform defined by this object
         * performs a rotation by an arbitrary angle in addition to the
         * conversions indicated by other flag bits.
         * A rotation changes the angles of vectors by the same amount
         * regardless of the original direction of the vector and without
         * changing the length of the vector.
         * This flag bit is mutually exclusive with the
         */
        public const int TypeGeneralRotation = 16;

        /**
         * This constant is a bit mask for any of the rotation flag bits.
         */
        public const int TypeMaskRotation = (TypeQuadrantRotation |
                              TypeGeneralRotation);

        /**
         * This constant indicates that the transform defined by this object
         * performs an arbitrary conversion of the input coordinates.
         * If this transform can be classified by any of the above constants,
         * the type will either be the constant TypeIdentity or a
         * combination of the appropriate flag bits for the various coordinate
         * conversions that this transform performs.
         */
        public const int TypeGeneralTransform = 32;

        /**
         * This constant is used for the internal state variable to indicate
         * that no calculations need to be performed and that the source
         * coordinates only need to be copied to their destinations to
         * complete the transformation equation of this transform.
         */
        private const int ApplyIdentity = 0;

        /**
         * This constant is used for the internal state variable to indicate
         * that the translation components of the matrix (m02 and m12) need
         * to be added to complete the transformation equation of this transform.
         */
        private const int ApplyTranslate = 1;

        /**
         * This constant is used for the internal state variable to indicate
         * that the scaling components of the matrix (m00 and m11) need
         * to be factored in to complete the transformation equation of
         * this transform.  If the ApplyShear bit is also set then it
         * indicates that the scaling components are not both 0.0.  If the
         * ApplyShear bit is not also set then it indicates that the
         * scaling components are not both 1.0.  If neither the ApplyShear
         * nor the ApplyScale bits are set then the scaling components
         * are both 1.0, which means that the x and y components contribute
         * to the transformed coordinate, but they are not multiplied by
         * any scaling factor.
         */
        private const int ApplyScale = 2;

        /**
         * This constant is used for the internal state variable to indicate
         * that the shearing components of the matrix (m01 and m10) need
         * to be factored in to complete the transformation equation of this
         * transform.  The presence of this bit in the state variable changes
         * the interpretation of the ApplyScale bit as indicated in its
         * documentation.
         */
        private const int ApplyShear = 4;

        // These tables translate the flags during predictable quadrant
        // rotations where the shear and scale values are swapped and negated.
        private static readonly int[] Rot90Conversion = new int[]
        {
            /* IDENTITY => */        ApplyShear,
            /* TRANSLATE (TR) => */  ApplyShear | ApplyTranslate,
            /* SCALE (SC) => */      ApplyShear,
            /* SC | TR => */         ApplyShear | ApplyTranslate,
            /* SHEAR (SH) => */      ApplyScale,
            /* SH | TR => */         ApplyScale | ApplyTranslate,
            /* SH | SC => */         ApplyShear | ApplyScale,
            /* SH | SC | TR => */    ApplyShear | ApplyScale | ApplyTranslate,
        };
    }
}
