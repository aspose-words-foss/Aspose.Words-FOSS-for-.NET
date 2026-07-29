// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System.Drawing;
using System.Drawing.Drawing2D;
using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Transforms
{
    internal class DmlGroupTransform : DmlTransform
    {
        internal DrMatrix CreateChildrenFitMatrix(double scaleX, double scaleY)
        {
            DrMatrix scaleMatrix = new DrMatrix();
            PointF childCenter = ChildrenCenterPoint;
            PointF groupCenter = CenterPoint;

            // We should translate around center of child area.
            scaleMatrix.Translate(-childCenter.X, -childCenter.Y, MatrixOrder.Append);
            scaleMatrix.Scale((float)scaleX, (float)scaleY, MatrixOrder.Append);

            // Do offset to fill group by child area
            scaleMatrix.Translate(groupCenter.X, groupCenter.Y, MatrixOrder.Append);
            return scaleMatrix;
        }

        internal override DrMatrix CreateRenderTransform()
        {
            DrMatrix matrix = CreateFitTransformation();
            DrMatrix rotateFlipTransform = CreateRotateFlipTransformation();
            matrix.Multiply(rotateFlipTransform, MatrixOrder.Append);
            return matrix;
        }

        /// <summary>
        /// The translation and scaling required to transform the union of 
        /// the children's bounding boxes to a rectangle defined by the group's 
        /// offset and extent attributes.
        /// </summary>
        internal override DrMatrix CreateFitTransformation()
        {
            float lengthRatio = GetLengthRatio();
            float widthRatio = GetWidthRatio();

            float translateX = (float)(X - widthRatio * ChildX);
            float translateY = (float)(Y - lengthRatio * ChildY);

            return new DrMatrix(
                widthRatio, 0,
                0, lengthRatio,
                translateX, translateY);
        }

        internal void FitChildrenArea()
        {
            ChildWidth = Width;
            ChildHeight = Height;

            ChildX = X;
            ChildY = Y;
        }

        internal override void Scale(double xscale, double yscale)
        {
            if ((xscale == 1.0) && (yscale == 1.0))
                return;

            base.Scale(xscale, yscale);

            mChildWidth *= xscale;
            mChildHeight *= yscale;
        }

        internal override DmlTransform Clone()
        {
            DmlGroupTransform result = new DmlGroupTransform();
            CopyTo(result);
            return result;
        }

        internal override void Offset(float dx, float dy)
        {
            base.Offset(dx, dy);
            mChildX += dx;
            mChildY += dy;
        }

        protected void CopyTo(DmlGroupTransform target)
        {
            base.CopyTo(target);
            target.mChildHeight = mChildHeight;
            target.mChildWidth = mChildWidth;
            target.mChildX = mChildX;
            target.mChildY = mChildY;
        }

        /// <summary>
        /// Specifies the width of the extents rectangle in EMUs. This rectangle shall 
        /// dictate the size of the object as displayed (the result of any 
        /// scaling to the original object).
        /// In Office, this element additionally defines the group scaling factor which 
        /// is the ratio of the extents to child extents when the size of the child extents 
        /// is nonzero. If the size of the child extents is zero or omitted, there is no group scaling.
        /// </summary>
        internal double ChildWidth
        {
            get { return mChildWidth; }
            set { mChildWidth = value; }
        }

        /// <summary>
        /// Specifies the length of the extents rectangle in EMUs. This rectangle shall 
        /// dictate the size of the object as displayed (the result of any 
        /// scaling to the original object).
        /// In Office, this element additionally defines the group scaling factor which 
        /// is the ratio of the extents to child extents when the size of the child extents 
        /// is nonzero. If the size of the child extents is zero or omitted, there is no group scaling.
        /// </summary>
        internal double ChildHeight
        {
            get { return mChildHeight; }
            set { mChildHeight = value; }
        }

        internal PointF ChildTopLeft
        {
            get { return new PointF((float)ChildX, (float)ChildY); }
            set
            {
                ChildX = value.X;
                ChildY = value.Y;
            }
        }

        /// <summary>
        /// Specifies a coordinate on the x-axis. The origin point for this coordinate shall be specified 
        /// by the parent XML element. In Office, the child extents and offset are used to determine 
        /// the center for scaling, rotation and flipping.
        /// </summary>
        internal double ChildX
        {
            get { return mChildX; }
            set { mChildX = value; }
        }

        /// <summary>
        /// Specifies a coordinate on the y-axis. The origin point for this coordinate shall be specified 
        /// by the parent XML element.
        /// </summary>
        internal double ChildY
        {
            get { return mChildY; }
            set { mChildY = value; }
        }

        private float GetLengthRatio()
        {
            //If the starting shape has zero width (e.g., it is a vertical line), 
            // then the cx attribute of a:ext is ignored and the horizontal scaling is skipped. 
            // Similarly, if the starting shape has zero height, then the cy attribute 
            // of a:ext is ignored and the vertical scaling is skipped. 
            if (MathUtil.IsZero(Height) || MathUtil.IsZero(ChildHeight))
                return 1.0f;
            else
                return (float)(Height / ChildHeight);
        }

        private float GetWidthRatio()
        {
            // If the starting shape has zero width (e.g., it is a vertical line), 
            // then the cx attribute of a:ext is ignored and the horizontal scaling is skipped. 
            // Similarly, if the starting shape has zero height, then the cy attribute 
            // of a:ext is ignored and the vertical scaling is skipped. 
            if (MathUtil.IsZero(Width) || MathUtil.IsZero(ChildWidth))
                return 1.0f;
            else
                return (float)(Width / ChildWidth);
        }

        internal override double XOffset
        {
            get { return X - ChildX; }
        }

        internal override double YOffset
        {
            get { return Y - ChildY; }
        }

        internal override float XScale
        {
            get { return GetWidthRatio(); }
        }

        internal override float YScale
        {
            get { return GetLengthRatio(); }
        }

        internal PointF ChildrenCenterPoint
        {
            get
            {
                // WORDSNET-16025 If child width or length is zero, parent size should be used for calculation center point.
                double width = (ChildWidth == 0) ? Width : ChildWidth;
                double length = (ChildHeight == 0) ? Height : ChildHeight;

                return new PointF((float)(ChildX + width / 2.0), (float)(ChildY + length / 2.0));
            }
        }

        private double mChildHeight;
        private double mChildWidth;
        private double mChildX;
        private double mChildY;
    }
}
