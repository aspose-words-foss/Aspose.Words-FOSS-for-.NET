// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.1 alphaBiLevel (Alpha Bi-Level Effect)
    /// This element represents an Alpha Bi-Level Effect.
    /// Alpha (Opacity) values less than the threshold are changed 
    /// to 0 (fully transparent) and alpha values greater than or equal 
    /// to the threshold are changed to 100% (fully opaque).
    /// </summary>
    internal class DmlAlphaBiLevelEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.AlphaBiLevel; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlAlphaBiLevelEffect value = (DmlAlphaBiLevelEffect)obj;

            return MathUtil.AreEqual(value.Threshold, Threshold);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Threshold.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the threshold value for the alpha bi-level effect.
        /// Value is in fraction representation, so 100% equals 1.0.
        /// </summary>
        internal double Threshold
        {
            get { return mThreshold; }
            set { mThreshold = value; }
        }

        private double mThreshold;
    }
}