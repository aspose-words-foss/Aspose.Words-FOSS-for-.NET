// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.39 hsl (Hue Saturation Luminance Effect)
    /// This element specifies a hue/saturation/luminance effect. 
    /// The hue, saturation, and luminance can each be adjusted relative to its current value.
    /// </summary>
    internal class DmlHueSaturationLuminanceEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.HueSaturationLuminance; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlHueSaturationLuminanceEffect value = (DmlHueSaturationLuminanceEffect)obj;

            return object.Equals(value.Hue, Hue) &&
                   MathUtil.AreEqual(value.Luminance, Luminance) &&
                   MathUtil.AreEqual(value.Saturation, Saturation);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Hue.GetHashCode();
            hash ^= Luminance.GetHashCode();
            hash ^= Saturation.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the number of degrees by which the hue is adjusted.
        /// </summary>
        internal DmlAngle Hue
        {
            get { return mHue; }
            set { mHue = value; }
        }

        /// <summary>
        /// Specifies the percentage by which the luminance is adjusted.
        /// </summary>
        internal double Luminance
        {
            get { return mLuminance; }
            set { mLuminance = value; }
        }

        /// <summary>
        /// Specifies the percentage by which the saturation is adjusted.
        /// </summary>
        internal double Saturation
        {
            get { return mSaturation; }
            set { mSaturation = value; }
        }

        private DmlAngle mHue = new DmlAngle(); // For Java
        private double mLuminance;
        private double mSaturation;
    }
}
