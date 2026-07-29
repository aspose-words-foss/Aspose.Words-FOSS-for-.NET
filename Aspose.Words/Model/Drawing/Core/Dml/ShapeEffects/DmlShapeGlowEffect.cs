// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/24/2013 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// 20.1.8.32 glow (Glow Effect)
    /// This element specifies a glow effect, in which a color blurred outline is added outside the edges of the object.
    /// </summary>
    internal class DmlShapeGlowEffect : DmlShapeEffect
    {
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlShapeGlowEffect value = (DmlShapeGlowEffect)obj;

            return object.Equals(value.Color, Color) && MathUtil.AreEqual(value.Radius, Radius);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (Color != null)
                hash ^= Color.GetHashCode();
            hash ^= Radius.GetHashCode();
            return hash;
        }

        internal override DmlShapeEffect Clone()
        {
            DmlShapeGlowEffect lhs = (DmlShapeGlowEffect)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            return lhs;
        }

        internal override DmlShapeEffectType EffectType
        {
            get { return DmlShapeEffectType.Glow; }
        }

        internal override bool IsEmpty
        {
            get { return MathUtil.IsZero(mRaduis); }
        }

        /// <summary>
        /// Specifies the color of the glow.
        /// </summary>
        internal override DmlColor Color
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mColor; }
            set { mColor = value; }
        }

        /// <summary>
        /// Specifies the radius of the glow.
        /// </summary>
        internal double Radius
        {
            get { return mRaduis; }
            set { mRaduis = value; }
        }

        private DmlColor mColor;
        private double mRaduis;
    }
}
