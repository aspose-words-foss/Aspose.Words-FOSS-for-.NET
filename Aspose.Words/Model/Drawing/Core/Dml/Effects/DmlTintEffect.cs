// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.60 tint (Tint Effect)
    /// This element specifies a tint effect. Shifts effect color values 
    /// towards/away from hue by the specified amount.
    /// </summary>
    internal class DmlTintEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.Tint; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlTintEffect value = (DmlTintEffect)obj;

            return object.Equals(value.Hue, Hue) &&
                   MathUtil.AreEqual(value.Amount, Amount);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Amount.GetHashCode();
            hash ^= Hue.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies by how much the color value is shifted.
        /// </summary>
        internal double Amount
        {
            get { return mAmount; }
            set { mAmount = value; }
        }

        /// <summary>
        /// Specifies the hue towards which to tint.
        /// </summary>
        internal double Hue
        {
            get { return mHue; }
            set { mHue = value; }
        }

        private double mAmount;
        private double mHue;
    }
}
