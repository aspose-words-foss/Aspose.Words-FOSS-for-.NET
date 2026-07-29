// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/09/2011 by Alexey Kachalov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.11 biLevel (Bi-Level (Black/White) Effect)
    /// This element specifies a bi-level (black/white) effect. 
    /// Input colors whose luminance is less than the specified
    /// threshold value are changed to black. Input colors whose
    /// luminance are greater than or equal the specified value
    /// are set to white. The alpha effect values are unaffected
    /// by this effect.
    /// </summary>
    internal class DmlBiLevelEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.BiLevel; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlBiLevelEffect value = (DmlBiLevelEffect)obj;

            return MathUtil.AreEqual(value.Threshold, Threshold);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Threshold.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the threshold value for the bi-level effect.
        /// </summary>
        internal double Threshold
        {
            get { return mThreshold; }
            set { mThreshold = value; }
        }

        private double mThreshold;
    }
}
