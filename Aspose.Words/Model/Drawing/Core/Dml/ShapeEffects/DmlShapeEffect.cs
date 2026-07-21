// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    internal abstract class DmlShapeEffect
    {
        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlShapeEffect value = (DmlShapeEffect)obj;

            return (value.EffectType == EffectType);
        }

        public override int GetHashCode()
        {
            return EffectType.GetHashCode();
        }

        /// <summary>
        /// Returns type of the effect.
        /// </summary>
        internal abstract DmlShapeEffectType EffectType { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get; }

        /// <summary>
        /// Returns true if effect's properties are empty, in this case effect must not be rendered.
        /// Used for text effects.
        /// </summary>
        internal virtual bool IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if effect is applied to text.
        /// </summary>
        internal bool IsTextEffect
        {
            get { return mIsTextEffect; }
            set { mIsTextEffect = value; }
        }

        /// <summary>
        /// Creates effect of a specified type.
        /// </summary>
        internal static DmlShapeEffect CreateEffect(DmlShapeEffectType type)
        {
            DmlShapeEffect effect;
            switch (type)
            {
                case DmlShapeEffectType.FillOverlay:
                    effect = new DmlShapeFillOverlayEffect();
                    break;

                case DmlShapeEffectType.InnerShadow:
                    effect = new DmlShapeInnerShadowEffect();
                    break;

                case DmlShapeEffectType.PresetShadow:
                    effect = new DmlShapePresetShadowEffect();
                    break;

                case DmlShapeEffectType.SoftEdges:
                    effect = new DmlShapeSoftEdgeEffect();
                    break;

                case DmlShapeEffectType.Blur:
                    effect = new DmlShapeBlurEffect();
                    break;

                case DmlShapeEffectType.Glow:
                    effect = new DmlShapeGlowEffect();
                    break;

                case DmlShapeEffectType.OuterShadow:
                    effect = new DmlShapeOuterShadowEffect();
                    break;

                case DmlShapeEffectType.Reflection:
                    effect = new DmlShapeReflectionEffect();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return effect;
        }

        /// <summary>
        /// Returns true, if specified shadow type has been set successfully.
        /// </summary>
        internal virtual bool TrySetShadowType(ShadowType shadowType)
        {
            return false;
        }

        /// <summary>
        /// Gets <see cref="Aspose.Words.Drawing.ShadowType"/> of the effect.
        /// </summary>
        internal virtual ShadowType ShadowType
        {
            get { return ShadowType.ShadowMixed; }
            set { /* No default setter. */ }
        }

        /// <summary>
        /// Gets or sets <see cref="DmlColor"/> object for effect.
        /// </summary>
        internal virtual DmlColor Color { [CodePorting.Translator.Cs2Cpp.CppConstMethod]get; set; }

        /// <summary>
        /// Returns true if effect is one of the shadow effects.
        /// </summary>
        internal virtual bool IsShadowEffect
        {
            get { return false; }
        }

        internal abstract DmlShapeEffect Clone();

        private bool mIsTextEffect;
    }
}
