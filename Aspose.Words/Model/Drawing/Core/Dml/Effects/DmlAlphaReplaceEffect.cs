// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.8 alphaRepl (Alpha Replace Effect)
    /// This element specifies an alpha replace effect.
    /// Effect alpha (opacity) values are replaced by a fixed alpha.
    /// </summary>
    internal class DmlAlphaReplaceEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.AlphaReplace; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlAlphaReplaceEffect value = (DmlAlphaReplaceEffect)obj;

            return MathUtil.AreEqual(value.Alpha, Alpha);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Alpha.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the new opacity value.
        /// </summary>
        internal double Alpha
        {
            get { return mAlpha; }
            set { mAlpha = value; }
        }

        private double mAlpha;
    }
}