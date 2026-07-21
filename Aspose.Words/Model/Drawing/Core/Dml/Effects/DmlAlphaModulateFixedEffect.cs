// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.6 alphaModFix (Alpha Modulate Fixed Effect)
    /// This element represents an alpha modulate fixed effect.
    /// Effect alpha (opacity) values are multiplied by a fixed percentage.
    /// </summary>
    internal class DmlAlphaModulateFixedEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.AlphaModulateFixed; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlAlphaModulateFixedEffect value = (DmlAlphaModulateFixedEffect)obj;

            return MathUtil.AreEqual(value.Amount, Amount);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Amount.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the percentage amount to scale the alpha.
        /// Value is in fraction representation, so 100% equals 1.0.
        /// </summary>
        internal double Amount
        {
            get { return mAmount; }
            set { mAmount = value; }
        }

        private double mAmount;
    }
}