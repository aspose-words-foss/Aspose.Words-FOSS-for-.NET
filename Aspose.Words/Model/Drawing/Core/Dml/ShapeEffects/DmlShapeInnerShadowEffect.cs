// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// 20.1.8.40 innerShdw (Inner Shadow Effect)
    /// This element specifies an inner shadow effect. A shadow is applied within the edges of the object according to
    /// the parameters given by the attributes.
    /// </summary>
    internal class DmlShapeInnerShadowEffect : DmlShapeEffect
    {
        internal DmlShapeInnerShadowEffect()
        {
            mColor.Alpha = new DmlAlpha();
            mColor.Alpha.Value = 0.5;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeInnerShadowEffect value = (DmlShapeInnerShadowEffect)obj;

            return object.Equals(value.Color, Color) &&
                   object.Equals(value.Direction, Direction) &&
                   MathUtil.AreEqual(value.BlurRadius, BlurRadius) &&
                   MathUtil.AreEqual(value.Distance, Distance);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (Color != null)
                hash ^= Color.GetHashCode();
            hash ^= BlurRadius.GetHashCode();
            hash ^= Direction.Value.GetHashCode();
            hash ^= Distance.GetHashCode();
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
            DmlShapeInnerShadowEffect lhs = (DmlShapeInnerShadowEffect)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            // The DmlAngle field is skipped since the class is immutable.

            return lhs;
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.InnerShadow; }
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
        /// Gets or sets <see cref="Aspose.Words.Drawing.ShadowType"/> corresponding to this inner shadow effect.
        /// </summary>
        internal override ShadowType ShadowType
        {
            get
            {
                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(13500000)))
                    return ShadowType.Shadow30;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(16200000)))
                    return ShadowType.Shadow31;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(18900000)))
                    return ShadowType.Shadow32;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(10800000)))
                    return ShadowType.Shadow33;

                if ((BlurRadius.Equals(114300)) &&
                    (Distance.Equals(0)) &&
                    (Direction.Value.Equals(0)))
                    return ShadowType.Shadow34;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(0)))
                    return ShadowType.Shadow35;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(8100000)))
                    return ShadowType.Shadow36;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(5400000)))
                    return ShadowType.Shadow37;

                if ((BlurRadius.Equals(63500)) &&
                    (Distance.Equals(50800)) &&
                    (Direction.Value.Equals(2700000)))
                    return ShadowType.Shadow38;

                return ShadowType.ShadowMixed;
            }
            set
            {
                TrySetShadowType(value);
            }
        }

        /// <summary>
        /// Returns true, if specified shadow type was set successfully.
        /// </summary>
        internal override bool TrySetShadowType(ShadowType shadowType)
        {
            switch (shadowType)
            {
                case ShadowType.Shadow30:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(13500000);
                    break;

                case ShadowType.Shadow31:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(16200000);
                    break;

                case ShadowType.Shadow32:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(18900000);
                    break;

                case ShadowType.Shadow33:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(10800000);
                    break;

                case ShadowType.Shadow34:
                    BlurRadius = 114300;
                    Color.Alpha = null;
                    break;

                case ShadowType.Shadow35:
                    BlurRadius = 63500;
                    Distance = 50800;
                    break;

                case ShadowType.Shadow36:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(8100000);
                    break;

                case ShadowType.Shadow37:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(5400000);
                    break;

                case ShadowType.Shadow38:
                    BlurRadius = 63500;
                    Distance = 50800;
                    Direction = new DmlAngle(2700000);
                    break;

                default:
                    return false;
            }

            return true;
        }

        private DmlColor mColor = new DmlPercentageRgbColor();
        private double mBlurRadius;
        private DmlAngle mDirection = new DmlAngle(0);
        private double mDistance;
    }
}
