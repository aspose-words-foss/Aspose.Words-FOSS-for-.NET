// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    internal class DmlShapePresetShadowEffect : DmlShapeEffect
    {
        internal DmlShapePresetShadowEffect()
        {
            mColor.Alpha = new DmlAlpha();
            mColor.Alpha.Value = 0.5;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapePresetShadowEffect value = (DmlShapePresetShadowEffect)obj;

            return (value.PresetShadow == PresetShadow) &&
                   MathUtil.AreEqual(value.Distance, Distance) &&
                   object.Equals(value.Color, Color) &&
                   object.Equals(value.Direction, Direction);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (Color != null)
                hash ^= Color.GetHashCode();
            hash ^= Direction.Value.GetHashCode();
            hash ^= Distance.GetHashCode();
            hash ^= PresetShadow.GetHashCode();
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
            DmlShapePresetShadowEffect lhs = (DmlShapePresetShadowEffect)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            // The DmlAngle field is skipped since the class is immutable.

            return lhs;
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.PresetShadow; }
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
        /// Specifies which preset shadow to use.
        /// </summary>
        internal DmlPresetShadow PresetShadow
        {
            get { return mPresetShadow; }
            set { mPresetShadow = value; }
        }

        /// <summary>
        /// Gets or sets <see cref="Aspose.Words.Drawing.ShadowType"/> corresponding to this preset shadow effect.
        /// </summary>
        internal override ShadowType ShadowType
        {
            get
            {
                switch (mPresetShadow)
                {
                    case DmlPresetShadow.TopLeftDropShadow:
                        return ShadowType.Shadow1;
                    case DmlPresetShadow.TopLeftLargeDropShadow:
                        return ShadowType.Shadow10;
                    case DmlPresetShadow.BackLeftLongPerspectiveShadow:
                        return ShadowType.Shadow11;
                    case DmlPresetShadow.BackRightLongPerspectiveShadow:
                        return ShadowType.Shadow12;
                    case DmlPresetShadow.TopLeftDoubleDropShadow:
                        return ShadowType.Shadow13;
                    case DmlPresetShadow.BottomRightSmallDropShadow:
                        return ShadowType.Shadow14;
                    case DmlPresetShadow.FrontLeftLongPerspectiveShadow:
                        return ShadowType.Shadow15;
                    case DmlPresetShadow.FrontRightLongPerspectiveShadow:
                        return ShadowType.Shadow16;
                    case DmlPresetShadow.OuterBoxShadow3D:
                        return ShadowType.Shadow17;
                    case DmlPresetShadow.InnerBoxShadow3D:
                        return ShadowType.Shadow18;
                    case DmlPresetShadow.BackCenterPerspectiveShadow:
                        return ShadowType.Shadow19;
                    case DmlPresetShadow.TopRightDropShadow:
                        return ShadowType.Shadow2;
                    case DmlPresetShadow.FrontBottomShadow:
                        return ShadowType.Shadow20;
                    case DmlPresetShadow.BackLeftPerspectiveShadow:
                        return ShadowType.Shadow3;
                    case DmlPresetShadow.BackRightPerspectiveShadow:
                        return ShadowType.Shadow4;
                    case DmlPresetShadow.BottomLeftDropShadow:
                        return ShadowType.Shadow5;
                    case DmlPresetShadow.BottomRightDropShadow:
                        return ShadowType.Shadow6;
                    case DmlPresetShadow.FrontLeftPerspectiveShadow:
                        return ShadowType.Shadow7;
                    case DmlPresetShadow.FrontRightPerspectiveShadow:
                        return ShadowType.Shadow8;
                    case DmlPresetShadow.TopLeftSmallDropShadow:
                        return ShadowType.Shadow9;
                    default:
                        return ShadowType.ShadowMixed;
                }
            }
            set
            {
                TrySetShadowType(value);
            }
        }

        /// <summary>
        /// Returns true, if specified shadow type has been set successfully.
        /// </summary>
        internal override bool TrySetShadowType(ShadowType shadowType)
        {

            switch (shadowType)
            {
                case ShadowType.Shadow1:
                    PresetShadow = DmlPresetShadow.TopLeftDropShadow;
                    Direction = new DmlAngle(13500000);
                    Distance = 107763;
                    break;

                case ShadowType.Shadow2:
                    PresetShadow = DmlPresetShadow.TopRightDropShadow;
                    Direction = new DmlAngle(18900000);
                    Distance = 107763;
                    break;

                case ShadowType.Shadow3:
                    PresetShadow = DmlPresetShadow.BackLeftPerspectiveShadow;
                    break;

                case ShadowType.Shadow4:
                    PresetShadow = DmlPresetShadow.BackRightPerspectiveShadow;
                    break;

                case ShadowType.Shadow5:
                    PresetShadow = DmlPresetShadow.BottomLeftDropShadow;
                    Direction = new DmlAngle(8100000);
                    Distance = 107763;
                    break;

                case ShadowType.Shadow6:
                    PresetShadow = DmlPresetShadow.BottomRightDropShadow;
                    Direction = new DmlAngle(2700000);
                    Distance = 107763;
                    break;

                case ShadowType.Shadow7:
                    PresetShadow = DmlPresetShadow.FrontLeftPerspectiveShadow;
                    break;

                case ShadowType.Shadow8:
                    PresetShadow = DmlPresetShadow.FrontRightPerspectiveShadow;
                    break;

                case ShadowType.Shadow9:
                    PresetShadow = DmlPresetShadow.TopLeftSmallDropShadow;
                    Direction = new DmlAngle(13500000);
                    Distance = 107763;
                    break;

                case ShadowType.Shadow10:
                    PresetShadow = DmlPresetShadow.TopLeftLargeDropShadow;
                    Direction = new DmlAngle(13500000);
                    Distance = 107763;
                    break;

                case ShadowType.Shadow11:
                    PresetShadow = DmlPresetShadow.BackLeftLongPerspectiveShadow;
                    break;

                case ShadowType.Shadow12:
                    PresetShadow = DmlPresetShadow.BackRightLongPerspectiveShadow;
                    break;

                case ShadowType.Shadow13:
                    PresetShadow = DmlPresetShadow.TopLeftDoubleDropShadow;
                    Direction = new DmlAngle(13500000);
                    Distance = 53882;
                    break;

                case ShadowType.Shadow14:
                    PresetShadow = DmlPresetShadow.BottomRightSmallDropShadow;
                    Direction = new DmlAngle(2700000);
                    Distance = 35921;
                    break;

                case ShadowType.Shadow15:
                    PresetShadow = DmlPresetShadow.FrontLeftLongPerspectiveShadow;
                    break;

                case ShadowType.Shadow16:
                    PresetShadow = DmlPresetShadow.FrontRightLongPerspectiveShadow;
                    break;

                case ShadowType.Shadow17:
                    PresetShadow = DmlPresetShadow.OuterBoxShadow3D;
                    Direction = new DmlAngle(2700000);
                    Distance = 17961;
                    break;

                case ShadowType.Shadow18:
                    PresetShadow = DmlPresetShadow.InnerBoxShadow3D;
                    Direction = new DmlAngle(13500000);
                    Distance = 17961;
                    break;

                case ShadowType.Shadow19:
                    PresetShadow = DmlPresetShadow.BackCenterPerspectiveShadow;
                    break;

                case ShadowType.Shadow20:
                    PresetShadow = DmlPresetShadow.FrontBottomShadow;
                    break;

                default:
                    return false;
            }

            return true;
        }

        private DmlColor mColor = new DmlPercentageRgbColor();
        private double mDistance;
        private DmlPresetShadow mPresetShadow;

        // WORDSNET-13508 This property is optional and has default value according to spec,
        // so it is have to be initialized. Otherwise DmlShapeEffectsWriter can failed with NullReferenceException.
        private DmlAngle mDirection = new DmlAngle();
    }
}
