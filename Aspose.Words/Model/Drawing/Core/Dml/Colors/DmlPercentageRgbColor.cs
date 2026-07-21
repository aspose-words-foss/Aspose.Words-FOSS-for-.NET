// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Colors
{
    /// <summary>
    /// This element specifies a color using the red, green, blue RGB color model. 
    /// Each component, red, green, and blue is expressed 
    /// as a percentage from 0% to 100%. A linear gamma of 1.0 is assumed.
    /// </summary>
    internal class DmlPercentageRgbColor : DmlColor
    {
        protected override DrColor CreateUnmodifiedColor(IThemeProvider themeProvider)
        {
            int r = MathUtil.DoubleToInt(R*255.0);
            int g = MathUtil.DoubleToInt(G*255.0);
            int b = MathUtil.DoubleToInt(B*255.0);
            return new DrColor(r, g, b);
        }

        public override DmlColor Clone()
        {
            DmlPercentageRgbColor result = new DmlPercentageRgbColor();
            result.R = R;
            result.G = G;
            result.B = B;
            CopyColorModifiersTo(result);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            DmlPercentageRgbColor value = (DmlPercentageRgbColor)obj;

            return MathUtil.AreEqual(value.R, R) &&
                   MathUtil.AreEqual(value.G, G) &&
                   MathUtil.AreEqual(value.B, B);
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            hash ^= R.GetHashCode();
            hash ^= G.GetHashCode();
            hash ^= B.GetHashCode();
            return hash;
        }

        public override DmlColorType ColorType
        {
            get { return DmlColorType.PercentageRgbColor; }
        }

        /// <summary>
        /// Specifies the percentage of red.
        /// </summary>
        internal double R
        {
            get { return mR; }
            set { mR = value; }
        }

        /// <summary>
        /// Specifies the percentage of green.
        /// </summary>
        internal double G
        {
            get { return mG; }
            set { mG = value; }
        }

        /// <summary>
        /// Specifies the percentage of blue.
        /// </summary>
        internal double B
        {
            get { return mB; }
            set { mB = value; }
        }

        private double mB;
        private double mG;
        private double mR;
    }
}
