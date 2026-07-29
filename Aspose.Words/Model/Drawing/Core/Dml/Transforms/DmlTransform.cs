// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Transforms
{
    internal class DmlTransform
    {
        internal SizeF GetExtents()
        {
            return new SizeF((float)Width, (float)Height);
        }

        internal virtual DrMatrix CreateFitTransformation()
        {
            DrMatrix scaleTranslateTransform = new DrMatrix(
                1, 0,
                0, 1,
                (float)X, (float)Y);

            return scaleTranslateTransform;
        }

        internal virtual DrMatrix CreateRenderTransform()
        {
            DrMatrix scaleTranslateTransform = CreateFitTransformation();
            DrMatrix rotateFlipTransformation = CreateRotateFlipTransformation();
            scaleTranslateTransform.Multiply(rotateFlipTransformation, MatrixOrder.Append);
            return scaleTranslateTransform;
        }

        internal virtual DrMatrix CreateRotateFlipTransformation()
        {
            DrMatrix rot = new DrMatrix();
            PointF transformPoint = CenterPoint;
            rot.Translate(-transformPoint.X, -transformPoint.Y, MatrixOrder.Append);

            bool horisontalFlip = ((FlipOrientation == FlipOrientation.Horizontal) || (FlipOrientation == FlipOrientation.Both));
            bool verticalFlip = ((FlipOrientation == FlipOrientation.Vertical) || (FlipOrientation == FlipOrientation.Both));

            float flipH = horisontalFlip ? -1.0f : 1.0f;
            float flipV = verticalFlip ? -1.0f : 1.0f;

            rot.Scale(flipH, flipV, MatrixOrder.Append);
            rot.Rotate((float)Rotation.ValueInDegrees, MatrixOrder.Append);
            rot.Translate(transformPoint.X, transformPoint.Y, MatrixOrder.Append);
            return rot;
        }

        internal virtual void Scale(double xscale, double yscale)
        {
            mWidth *= xscale;
            mHeight *= yscale;
        }

        internal virtual void Offset(float dx, float dy)
        {
            mX += dx;
            mY += dy;
        }

        internal virtual DmlTransform Clone()
        {
            DmlTransform result = new DmlTransform();
            CopyTo(result);
            return result;
        }

        /// <summary>
        /// Returns true if the specified transformation equals this transformation.
        /// </summary>
        public bool Equals(DmlTransform transform)
        {
            // This condition exits the allowed number of conditional operators, 
            // but we have to check all the properties one by one here. So leave it as is.
            return (transform.mFlip == mFlip) &&
                   (transform.mVerticalFlip == mVerticalFlip) &&
                   MathUtil.AreEqual(transform.mHeight, mHeight) &&
                   MathUtil.AreEqual(transform.mRotation.Value, mRotation.Value) &&
                   MathUtil.AreEqual(transform.mWidth, mWidth) &&
                   MathUtil.AreEqual(transform.mX, mX) &&
                   MathUtil.AreEqual(transform.mY, mY);
        }

        /// <summary>
        /// Returns true if the transformation is empty.
        /// </summary>
        internal bool IsEmpty
        {
            get { return this.Equals(gEmpty); }
        }

        protected void CopyTo(DmlTransform target)
        {
            target.mFlip = mFlip;
            target.mHeight = mHeight;
            target.mRotation = mRotation;
            target.mVerticalFlip = mVerticalFlip;
            target.mWidth = mWidth;
            target.mX = mX;
            target.mY = mY;
        }

        /// <summary>
        /// Orientation of a shape.
        /// </summary>
        internal FlipOrientation FlipOrientation
        {
            get { return mFlip; }
            set { mFlip = value; }
        }

        /// <summary>
        /// Specifies the clockwise rotation of a group in 1/60000 of a degree.
        /// </summary>
        internal DmlAngle Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        /// <summary>
        /// Specifies the width of the extents rectangle in EMUs. This rectangle shall 
        /// dictate the size of the object as displayed (the result of any 
        /// scaling to the original object).
        /// In Office, the extents also determine the size and center of the object for most 
        /// geometry definitions. If the geometry data is independent of these extents, they are 
        /// only used to determine the center for rotation and flipping.
        /// </summary>
        internal double Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Specifies the height of the extents rectangle in EMUs. This rectangle shall 
        /// dictate the size of the object as displayed (the result of any 
        /// scaling to the original object).
        /// In Office, the extents also determine the size and center of the object for most 
        /// geometry definitions. If the geometry data is independent of these extents, they are 
        /// only used to determine the center for rotation and flipping.
        /// </summary>
        internal double Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        /// <summary>
        /// Specifies a coordinate on the x-axis. The origin point for this coordinate 
        /// shall be specified by the parent XML element.
        /// In Office, the offset element specifies the location of a bounding box of 
        /// an object as defined by the ext element.
        /// </summary>
        internal double X
        {
            get { return mX; }
            set { mX = value; }
        }

        /// <summary>
        /// Specifies a coordinate on the y-axis. The origin point for this coordinate 
        /// shall be specified by the parent XML element.
        /// In Office, the offset element specifies the location of a bounding box of 
        /// an object as defined by the ext element.
        /// </summary>
        internal double Y
        {
            get { return mY; }
            set { mY = value; }
        }

        internal PointF TopLeft
        {
            get { return new PointF((float)X, (float)Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        internal RectangleF BoundingBox
        {
            get { return new RectangleF((float)X, (float)Y, (float)Width, (float)Height); }
        }

        /// <summary>
        /// Gets the center point without offset defined by X and Y.
        /// </summary>
        internal virtual PointF OriginalCenterPoint
        {
            get { return mOriginalCenterPoint.IsEmpty ? new PointF((float)(Width / 2.0), (float)(Height / 2.0)) : mOriginalCenterPoint; }
            set { mOriginalCenterPoint = value; }
        }

        internal virtual PointF CenterPoint
        {
            get { return new PointF((float)(X + Width / 2.0), (float)(Y + Height / 2.0)); }
        }

        internal virtual float XScale
        {
            get { return 1.0f; }
        }

        internal virtual float YScale
        {
            get { return 1.0f; }
        }

        internal virtual double XOffset
        {
            get { return X; }
        }

        internal virtual double YOffset
        {
            get { return Y; }
        }

        private PointF mOriginalCenterPoint = PointF.Empty;
        private FlipOrientation mFlip = FlipOrientation.None;
        private DmlAngle mRotation = new DmlAngle(); // For Java
        private bool mVerticalFlip;
        private double mWidth;
        private double mHeight;
        private double mX;
        private double mY;

        private static readonly DmlTransform gEmpty = new DmlGroupTransform();
    }
}
