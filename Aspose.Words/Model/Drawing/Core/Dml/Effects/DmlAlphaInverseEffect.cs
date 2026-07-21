// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.4 alphaInv (Alpha Inverse Effect)
    /// This element represents an alpha inverse effect.
    /// Alpha (opacity) values are inverted by subtracting from 100%.
    /// </summary>
    internal class DmlAlphaInverseEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.AlphaInverse; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlAlphaInverseEffect value = (DmlAlphaInverseEffect)obj;

            return object.Equals(value.Color, Color);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Color.GetHashCode();
            return hash;
        }

        internal override DmlEffect Clone()
        {
            DmlAlphaInverseEffect lhs = (DmlAlphaInverseEffect)MemberwiseClone();

            if (mColor != null)
                lhs.mColor = mColor.Clone();

            return lhs;
        }

        internal DmlColor Color
        {
            get { return mColor; }
            set { mColor = value; }
        }

        private DmlColor mColor;
    }
}
