// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// 20.1.8.45 outerShdw (Outer Shadow Effect)
    /// This element specifies an Outer Shadow Effect.
    /// </summary>
    internal class DmlShapeOuterShadowEffect : DmlShapeEffect
    {
        internal DmlShapeOuterShadowEffect()
        {
            mColor.Alpha = new DmlAlpha();
            mColor.Alpha.Value = 0.4;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeOuterShadowEffect value = (DmlShapeOuterShadowEffect)obj;

            return (value.RotateWithShape == RotateWithShape) &&
                (value.Alignment == Alignment) &&
                MathUtil.AreEqual(value.BlurRadius, BlurRadius) &&
                MathUtil.AreEqual(value.Distance, Distance) &&
                MathUtil.AreEqual(value.HorizontalScale, HorizontalScale) &&
                MathUtil.AreEqual(value.VerticalScale, VerticalScale) &&
                object.Equals(value.Color, Color) &&
                object.Equals(value.Direction, Direction) &&
                object.Equals(value.HorizontalSkew, HorizontalSkew) &&
                object.Equals(value.VerticalSkew, VerticalSkew);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (Color != null)
                hash ^= Color.GetHashCode();
            hash ^= BlurRadius.GetHashCode();
            hash ^= Direction.Value.GetHashCode();
            hash ^= Distance.GetHashCode();
            hash ^= Alignment.GetHashCode();
            hash ^= HorizontalSkew.Value.GetHashCode();
            hash ^= VerticalSkew.Value.GetHashCode();
            hash ^= HorizontalScale.GetHashCode();
            hash ^= VerticalScale.GetHashCode();
            hash ^= RotateWithShape.GetHashCode();

            return hash;
        }

        /// <summary>
        /// Returns true if effect is one of the shadow effects.
        /// </summary>
        internal override bool IsShadowEffect
        {
            get { return true; }
        }

        internal override DmlShapeEffect Clone()
        {
            DmlShapeOuterShadowEffect lhs = (DmlShapeOuterShadowEffect)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            // The DmlAngle fields are skipped since the class is immutable.

            return lhs;
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.OuterShadow; }
        }

        internal override bool IsEmpty
        {
            get
            {
                return MathUtil.IsZero(mBlurRadius, mDirection.ValueInDegrees, mDistance, mKx.ValueInDegrees, mKy.ValueInDegrees,
                    mSx, mSy);
            }
        }

        internal bool IsZeroSize
        {
            get
            {
                // If vertical or horizontal scale is zero we get zero width.
                // To avoid problems with this consider shadow with zero scale as empty.
                return MathUtil.IsZero(mSx) && MathUtil.IsZero(mSy);
            }
        }

        /// <summary>
        /// Specifies the color of the shadow.
        /// </summary>
        internal override DmlColor Color
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mColor; }
            set { mColor = value; }
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
        /// Specifies the direction to offset the shadow.
        /// </summary>
        internal DmlAngle Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

        /// <summary>
        /// Specifies how far to offset the shadow.
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
        /// Gets or sets <see cref="Aspose.Words.Drawing.ShadowType"/> corresponding to outer shadow effect.
        /// </summary>
        internal override ShadowType ShadowType
        {
            get
            {
                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(2700000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.TopLeft))
                    return ShadowType.Shadow21;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(5400000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.Top))
                    return ShadowType.Shadow22;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(8100000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.TopRight))
                    return ShadowType.Shadow23;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(0)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.Left))
                    return ShadowType.Shadow24;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(0)) &&
                    (Direction.Value.Equals(0)) &&
                    (VerticalScale.Equals(1.02)) &&
                    (HorizontalScale.Equals(1.02)) &&
                    (Alignment == DmlRectangleAlignment.Center))
                    return ShadowType.Shadow25;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(10800000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.Right))
                    return ShadowType.Shadow26;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(18900000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.BottomLeft))
                    return ShadowType.Shadow27;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(16200000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.Bottom))
                    return ShadowType.Shadow28;

                if ((BlurRadius.Equals(50800)) &&
                    (Distance.Equals(38100)) &&
                    (Direction.Value.Equals(13500000)) &&
                    (VerticalScale.Equals(1.0)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.BottomRight))
                    return ShadowType.Shadow29;

                if ((BlurRadius.Equals(76200)) &&
                    (Distance.Equals(0)) &&
                    (Direction.Value.Equals(13500000)) &&
                    (VerticalScale.Equals(0.23)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.BottomRight))
                    return ShadowType.Shadow39;

                if ((BlurRadius.Equals(76200)) &&
                    (Distance.Equals(0)) &&
                    (Direction.Value.Equals(18900000)) &&
                    (VerticalScale.Equals(0.23)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.BottomLeft))
                    return ShadowType.Shadow40;

                if ((BlurRadius.Equals(152400)) &&
                    (Distance.Equals(317500)) &&
                    (Direction.Value.Equals(5400000)) &&
                    (VerticalScale.Equals(-0.19)) &&
                    (HorizontalScale.Equals(0.9)) &&
                    (Alignment == DmlRectangleAlignment.Bottom))
                    return ShadowType.Shadow41;

                if ((BlurRadius.Equals(76200)) &&
                    (Distance.Equals(12700)) &&
                    (Direction.Value.Equals(8100000)) &&
                    (VerticalScale.Equals(-0.23)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.BottomRight))
                    return ShadowType.Shadow42;

                if ((BlurRadius.Equals(76200)) &&
                    (Distance.Equals(12700)) &&
                    (Direction.Value.Equals(2700000)) &&
                    (VerticalScale.Equals(-0.23)) &&
                    (HorizontalScale.Equals(1.0)) &&
                    (Alignment == DmlRectangleAlignment.BottomLeft))
                    return ShadowType.Shadow43;

                return ShadowType.ShadowMixed;
            }
            set
            {
                TrySetShadowType(value);
            }
        }

        /// <summary>
        /// Returns true, if specified shadow type is set successfully.
        /// </summary>
        internal override bool TrySetShadowType(ShadowType shadowType)
        {
            switch (shadowType)
            {
                case ShadowType.Shadow21:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(2700000);
                    Alignment = DmlRectangleAlignment.TopLeft;
                    break;

                case ShadowType.Shadow22:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(5400000);
                    Alignment = DmlRectangleAlignment.Top;
                    break;

                case ShadowType.Shadow23:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(8100000);
                    Alignment = DmlRectangleAlignment.TopRight;
                    break;

                case ShadowType.Shadow24:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Alignment = DmlRectangleAlignment.Left;
                    break;

                case ShadowType.Shadow25:
                    BlurRadius = 63500;
                    HorizontalScale = 1.02;
                    VerticalScale = 1.02;
                    Alignment = DmlRectangleAlignment.Center;
                    break;

                case ShadowType.Shadow26:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(10800000);
                    Alignment = DmlRectangleAlignment.Right;
                    break;

                case ShadowType.Shadow27:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(18900000);
                    Alignment = DmlRectangleAlignment.BottomLeft;
                    break;

                case ShadowType.Shadow28:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(16200000);
                    break;

                case ShadowType.Shadow29:
                    BlurRadius = 50800;
                    Distance = 38100;
                    Direction = new DmlAngle(13500000);
                    Alignment = DmlRectangleAlignment.BottomRight;
                    break;

                case ShadowType.Shadow39:
                    BlurRadius = 76200;
                    Direction = new DmlAngle(13500000);
                    VerticalScale = 0.23;
                    HorizontalSkew = new DmlAngle(1200000);
                    Alignment = DmlRectangleAlignment.BottomRight;
                    Color.Alpha.Value = 0.2;
                    break;

                case ShadowType.Shadow40:
                    BlurRadius = 76200;
                    Direction = new DmlAngle(18900000);
                    VerticalScale = 0.23;
                    HorizontalSkew = new DmlAngle(-1200000);
                    Alignment = DmlRectangleAlignment.BottomLeft;
                    Color.Alpha.Value = 0.2;
                    break;

                case ShadowType.Shadow41:
                    BlurRadius = 152400;
                    Distance = 317500;
                    Direction = new DmlAngle(5400000);
                    VerticalScale = -0.19;
                    HorizontalScale = 0.9;
                    Color.Alpha.Value = 0.15;
                    break;

                case ShadowType.Shadow42:
                    BlurRadius = 76200;
                    Distance = 12700;
                    Direction = new DmlAngle(8100000);
                    VerticalScale = -0.23;
                    HorizontalSkew = new DmlAngle(800400);
                    Alignment = DmlRectangleAlignment.BottomRight;
                    Color.Alpha.Value = 0.2;
                    break;

                case ShadowType.Shadow43:
                    BlurRadius = 76200;
                    Distance = 12700;
                    Direction = new DmlAngle(2700000);
                    VerticalScale = -0.23;
                    HorizontalSkew = new DmlAngle(-800400);
                    Alignment = DmlRectangleAlignment.BottomLeft;
                    Color.Alpha.Value = 0.2;
                    break;

                default:
                    return false;
            }

            return true;
        }

        private DmlColor mColor = new DmlPresetColor("black");
        private double mBlurRadius;
        private DmlAngle mDirection = new DmlAngle(0);
        private double mDistance;
        private DmlRectangleAlignment mAlignment = DmlRectangleAlignment.Bottom;
        private DmlAngle mKx = new DmlAngle(0);
        private DmlAngle mKy = new DmlAngle(0);
        private double mSx = 1.0;
        private double mSy = 1.0;
        private bool mRotateWithShape = true;

    }
}
