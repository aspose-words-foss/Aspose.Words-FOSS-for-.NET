// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.23 duotone (Duotone Effect)
    /// This element specifies a duotone effect.
    /// For each pixel, combines clr1 and clr2 through a linear 
    /// interpolation to determine the new color for that pixel.
    /// </summary>
    internal class DmlDuotoneEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.Duotone; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlDuotoneEffect value = (DmlDuotoneEffect)obj;

            return object.Equals(value.Color1, Color1) && object.Equals(value.Color2, Color2);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            if (Color1 != null)
                hash ^= Color1.GetHashCode();
            if (Color2 != null)
                hash ^= Color2.GetHashCode();
            return hash;
        }

        internal override DmlEffect Clone()
        {
            DmlDuotoneEffect lhs = (DmlDuotoneEffect)MemberwiseClone();

            if (mColor1 != null)
                lhs.mColor1 = mColor1.Clone();

            if (mColor2 != null)
                lhs.mColor2 = mColor2.Clone();

            return lhs;
        }

        /// <summary>
        /// Gets or sets a value of color which is used as the beginning of linear interpolation of the duotone effect.
        /// </summary>
        internal DmlColor Color1
        {
            get { return mColor1; }
            set { mColor1 = value; }
        }

        /// <summary>
        /// Gets or sets a value of color which is used as the end of linear interpolation of the duotone effect.
        /// </summary>
        internal DmlColor Color2
        {
            get { return mColor2; }
            set { mColor2 = value; }
        }

        private DmlColor mColor1;
        private DmlColor mColor2;
    }
}
