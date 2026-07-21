// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Dmitry Burov
// 14/05/2011 by Alexey Titov

// Ignore Spelling: bezier

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Collections;

namespace Aspose.Drawing
{
    public static class GeometryUtil
    {
        /// <summary>
        /// Calculates the center point of the rectangle.
        /// </summary>
        public static PointF GetCenterPoint(RectangleF rect)
        {
            return new PointF(rect.X + (float)(rect.Width / 2.0f), rect.Y + (float)(rect.Height / 2.0f)); // Casting for Java.
        }

        /// <summary>
        /// Gets the specified rectangle points in array.
        /// </summary>
        public static PointF[] GetRectPoints(RectangleF rect)
        {
            return new PointF[] {
                new PointF(rect.Left, rect.Top),
                new PointF(rect.Right, rect.Top),
                new PointF(rect.Right, rect.Bottom),
                new PointF(rect.Left, rect.Bottom)
            };
        }

        /// <summary>
        /// Calculates the bounding rectangle for the specified rectangle rotated by specified angle in degrees.
        /// </summary>
        public static RectangleF GetRotatedRectangleBounds(RectangleF rect, float angle)
        {
            return GetBounds(GetRotatedRectanglePoints(rect, angle));
        }

        /// <summary>
        /// Gets the bounding rectangle for specified array of points.
        /// </summary>
        public static RectangleF GetBounds(PointF[] rectPoints)
        {
            return GetBounds(null, rectPoints);
        }

        private static RectangleF GetBounds(PointFList list, PointF[] array)
        {
            // WORDSNET-7424.
            // Return RectangleF.Empty as bound for empty rectangle.
            if ((list == null) && (array == null))
                return RectangleF.Empty;

            int pointCount = (list != null) ? list.Count : array.Length;
            if (pointCount == 0)
                return RectangleF.Empty;

            PointF point0 = (list != null) ? list[0] : array[0];
            float maxX = point0.X;
            float minX = maxX;
            float maxY = point0.Y;
            float minY = maxY;

            for (int i = 1; i < pointCount; i++)
            {
                PointF point = (list != null) ? list[i] : array[i];

                if (point.X > maxX)
                    maxX = point.X;
                else if (point.X < minX)
                    minX = point.X;

                if (point.Y > maxY)
                    maxY = point.Y;
                else if (point.Y < minY)
                    minY = point.Y;
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Gets the array of points of the specified rectangle rotated by the specified angle in degrees.
        /// </summary>
        public static PointF[] GetRotatedRectanglePoints(RectangleF rect, float angle)
        {
            // Prepare the transformation matrix.
            DrMatrix transformer = new DrMatrix();
            transformer.RotateAt(angle, GetCenterPoint(rect), MatrixOrder.Prepend);

            // Convert the rectangle into points array.
            PointF[] rectPoints = GetRectPoints(rect);

            // Rotate the rectangle.
            transformer.TransformPoints(rectPoints);

            return rectPoints;
        }

        /// <summary>
        /// Returns distance between points.
        /// </summary>
        /// <param name="point1">Point 1.</param>
        /// <param name="point2">Point 2.</param>
        public static float GetDistanceBetweenPoints(PointF point1, PointF point2)
        {
            float dX = point1.X - point2.X;
            float dY = point1.Y - point2.Y;
            return (float)Math.Sqrt((float)(dX * dX) + (float)(dY * dY)); // Casting for Java.
        }

        /// <summary>
        /// Value of tolerance that is used in geometric computations.
        /// </summary>
        public const float Tolerance = 0.001f;

        /// <summary>
        /// Returns true if the two sizes are equal within the given tolerance.
        /// </summary>
        public static bool EqualsPaperSize(SizeF size1, SizeF size2, bool isLandscape, double tolerance)
        {
            if (isLandscape)
            {
                if ((Math.Abs(size1.Width - size2.Height) <= tolerance) &&
                    (Math.Abs(size1.Height - size2.Width) <= tolerance))
                    return true;
            }
            else
            {
                if ((Math.Abs(size1.Width - size2.Width) <= tolerance) &&
                    (Math.Abs(size1.Height - size2.Height) <= tolerance))
                    return true;
            }

            return false;
        }
    }
}
