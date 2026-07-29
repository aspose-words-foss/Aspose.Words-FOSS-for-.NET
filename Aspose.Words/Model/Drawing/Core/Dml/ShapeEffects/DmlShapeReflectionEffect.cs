// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// 20.1.8.50 reflection (Reflection Effect)
    /// This element specifies a reflection effect.
    /// </summary>
    internal class DmlShapeReflectionEffect : DmlShapeEffect
    {
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeReflectionEffect value = (DmlShapeReflectionEffect)obj;

            return (value.RotateWithShape == RotateWithShape) &&
                   (value.Alignment == Alignment) &&
                   MathUtil.AreEqual(value.BlurRadius, BlurRadius) &&
                   MathUtil.AreEqual(value.Distance, Distance) &&
                   MathUtil.AreEqual(value.HorizontalScale, HorizontalScale) &&
                   MathUtil.AreEqual(value.VerticalScale, VerticalScale) &&
                   MathUtil.AreEqual(value.EndAlpha, EndAlpha) &&
                   MathUtil.AreEqual(value.StartAlpha, StartAlpha) &&
                   MathUtil.AreEqual(value.EndPosition, EndPosition) &&
                   MathUtil.AreEqual(value.StartPosition, StartPosition) &&
                   object.Equals(value.Direction, Direction) &&
                   object.Equals(value.FadeDirection, FadeDirection) &&
                   object.Equals(value.HorizontalSkew, HorizontalSkew) &&
                   object.Equals(value.VerticalSkew, VerticalSkew);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= BlurRadius.GetHashCode();
            hash ^= Direction.Value.GetHashCode();
            hash ^= Distance.GetHashCode();
            hash ^= HorizontalSkew.Value.GetHashCode();
            hash ^= VerticalSkew.Value.GetHashCode();
            hash ^= HorizontalScale.GetHashCode();
            hash ^= VerticalScale.GetHashCode();
            hash ^= EndAlpha.GetHashCode();
            hash ^= StartAlpha.GetHashCode();
            hash ^= EndPosition.GetHashCode();
            hash ^= StartPosition.GetHashCode();
            hash ^= FadeDirection.Value.GetHashCode();
            return hash;
        }

        internal override DmlShapeEffect Clone()
        {
            // The DmlAngle fields are not cloned since the class is immutable.
            return (DmlShapeReflectionEffect)MemberwiseClone();
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.Reflection; }
        }

        internal override bool IsEmpty
        {
            get
            {
                return MathUtil.IsZero(mBlurRadius, 
                    mDirection.ValueInDegrees,
                    mDistance,
                    mKx.ValueInDegrees,
                    mKy.ValueInDegrees,
                    mSx,
                    mSy,
                    mEndAlpha,
                    mStartAlpha,
                    mEndPosition,
                    mStartPosition,
                    mFadeDirection.ValueInDegrees);
            }
        }
        
        /// <summary>
        /// Specifies shadow alignment; alignment happens first, effectively setting the origin for scale, skew, and offset.
        /// </summary>
        internal DmlRectangleAlignment Alignment
        {
            get { return mAlignment; }
            set { mAlignment = value; }
        }

        /// <summary>
        /// Specifies the blur radius.
        /// </summary>
        internal double BlurRadius
        {
            get { return mBlurRadius; }
            set { mBlurRadius = value; }
        }

        /// <summary>
        /// Specifies the direction of the alpha gradient ramp relative to the shape itself.
        /// </summary>
        internal DmlAngle Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

        /// <summary>
        /// Specifies how far to distance the shadow.
        /// </summary>
        internal double Distance
        {
            get { return mDistance; }
            set { mDistance = value; }
        }

        /// <summary>
        /// Specifies the horizontal skew angle.
        /// </summary>
        internal DmlAngle HorizontalSkew
        {
            get { return mKx; }
            set { mKx = value; }
        }

        /// <summary>
        /// Specifies the vertical skew angle.
        /// </summary>
        internal DmlAngle VerticalSkew
        {
            get { return mKy; }
            set { mKy = value; }
        }

        /// <summary>
        /// Specifies the horizontal scaling factor; negative scaling causes a flip.
        /// </summary>
        internal double HorizontalScale
        {
            get { return mSx; }
            set { mSx = value; }
        }

        /// <summary>
        /// Specifies the vertical scaling factor; negative scaling causes a flip.
        /// </summary>
        internal double VerticalScale
        {
            get { return mSy; }
            set { mSy = value; }
        }

        /// <summary>
        /// Specifies whether the shadow rotates with the shape if the shape is rotated.
        /// </summary>
        internal bool RotateWithShape
        {
            get { return mRotateWithShape; }
            set { mRotateWithShape = value; }
        }


        /// <summary>
        /// Specifies the ending reflection opacity.
        /// </summary>
        internal double EndAlpha
        {
            get { return mEndAlpha; }
            set { mEndAlpha = value; }
        }

        /// <summary>
        /// Specifies the starting reflection opacity.
        /// </summary>
        internal double StartAlpha
        {
            get { return mStartAlpha; }
            set { mStartAlpha = value; }
        }

        /// <summary>
        /// Specifies the end position (along the alpha gradient ramp) of the end alpha value.
        /// </summary>
        internal double EndPosition
        {
            get { return mEndPosition; }
            set { mEndPosition = value; }
        }

        /// <summary>
        /// Specifies the start position (along the alpha gradient ramp) of the start alpha value.
        /// </summary>
        internal double StartPosition
        {
            get { return mStartPosition; }
            set { mStartPosition = value; }
        }

        /// <summary>
        /// Specifies the direction to offset the reflection.
        /// </summary>
        internal DmlAngle FadeDirection
        {
            get { return mFadeDirection; }
            set { mFadeDirection = value; }
        }

        private double mBlurRadius;
        private DmlAngle mDirection = new DmlAngle();
        private double mDistance;
        private DmlRectangleAlignment mAlignment = DmlRectangleAlignment.Bottom;
        private DmlAngle mKx = new DmlAngle();
        private DmlAngle mKy = new DmlAngle();
        private double mSx = 1.0;
        private double mSy = 1.0;
        private bool mRotateWithShape = true;
        private double mEndAlpha;
        private double mStartAlpha = 1.0;
        private double mEndPosition = 1.0;
        private double mStartPosition;
        private DmlAngle mFadeDirection = new DmlAngle(5400000.0);
    }
}
