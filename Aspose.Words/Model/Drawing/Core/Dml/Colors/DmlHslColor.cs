// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This element specifies a color using the HSL color model. 
    /// A perceptual gamma of 2.2 is assumed.
    /// Hue refers to the dominant wavelength of color, saturation 
    /// refers to the purity of its hue, and luminance refers to its lightness or darkness.
    /// </summary>
    internal class DmlHslColor : DmlColor
    {
        public override DmlColor Clone()
        {
            DmlHslColor result = new DmlHslColor();
            result.Saturation = Saturation;
            result.Luminance = Luminance;
            result.Hue = Hue;
            CopyColorModifiersTo(result);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlHslColor value = (DmlHslColor)obj;

            return object.Equals(value.Hue, Hue) &&
                   MathUtil.AreEqual(value.Luminance, Luminance) &&
                   MathUtil.AreEqual(value.Saturation, Saturation);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= Hue.Value.GetHashCode();
            hash ^= Luminance.GetHashCode();
            hash ^= Saturation.GetHashCode();
            return hash;
        }

        public override DmlColorType ColorType
        {
            get { return DmlColorType.HslColor; }
        }

        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            HSLColor hsl = new HSLColor(Hue.ValueInDegrees / 360.0, Saturation, Luminance);
            return hsl.ToDrColor();
        }

        /// <summary>
        /// Specifies the saturation referring to the purity of the hue. 
        /// Expressed as a percentage with 0% referring to grey, 100% referring to the purest form of the hue.
        /// Value is in fraction representation.
        /// </summary>
        internal double Saturation
        {
            get { return mSaturation; }
            set { mSaturation = value; }
        }

        /// <summary>
        /// Specifies the luminance referring to the lightness or darkness of the color. 
        /// Expressed as a percentage with 0% referring to maximal dark (black) 
        /// and 100% referring to maximal white.
        /// Value is in fraction representation.
        /// </summary>
        internal double Luminance
        {
            get { return mLuminance; }
            set { mLuminance = value; }
        }

        /// <summary>
        /// Specifies the angular value describing the wavelength. Expressed in 1/60000ths of a degree.
        /// </summary>
        internal DmlAngle Hue
        {
            get { return mHue; }
            set { mHue = value; }
        }

        private DmlAngle mHue = new DmlAngle(); // For java
        private double mLuminance;
        private double mSaturation;
    }
}
