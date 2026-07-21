// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/05/2011 by Alexey Titov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Effects
{
    /// <summary>
    /// 20.1.8.42 lum (Luminance Effect)
    /// This element specifies a luminance effect. 
    /// Brightness linearly shifts all colors closer to white or black. 
    /// Contrast scales all colors to be either closer or further apart.
    /// </summary>
    internal class DmlLuminanceEffect : DmlEffect
    {
        /// <summary>
        /// Gets the type of effects.
        /// </summary>
        internal override DmlEffectType Type
        {
            get { return DmlEffectType.Luminance; }
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlLuminanceEffect value = (DmlLuminanceEffect)obj;

            return MathUtil.AreEqual(value.Brightness, Brightness) &&
                   MathUtil.AreEqual(value.Contrast, Contrast);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Brightness.GetHashCode();
            hash ^= Contrast.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the percent to change the brightness.
        /// </summary>
        /// <remarks>
        /// Despite the various specs and according to MS Word behavior 
        /// this value is specified in range [-100%; 100%]. This should be forced for public API later.
        /// Keep it in range [0; 1].
        /// </remarks>
        internal double Brightness
        {
            get { return mBrightness; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");

                mBrightness = value;
            }
        }

        /// <summary>
        /// Specifies the percent to change the contrast.
        /// </summary>
        /// <remarks>
        /// Despite the various specs and according to MS Word behavior 
        /// this value is specified in range [-100%; 100%]. This should be forced for public API later.
        /// Keep it in range [0; 1].
        /// </remarks>
        internal double Contrast
        {
            get { return mContrast; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                    throw new ArgumentOutOfRangeException("value");

                mContrast = value;
            }
        }

        private double mBrightness;
        private double mContrast;
    }
}