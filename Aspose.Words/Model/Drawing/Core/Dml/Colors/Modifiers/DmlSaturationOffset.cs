// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.28 satOff (Saturation Offset)
    /// This element specifies the input color with its saturation 
    /// shifted, but with its hue and luminance unchanged. A 10% 
    /// offset to 20% saturation yields 30% saturation.
    /// </summary>
    internal class DmlSaturationOffset : DmlPercentageBasedColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            DrColor result = new DrColor(color.A, HSLColor.ToDrColor(hsl.Hue, hsl.Sat + Value, hsl.Lum));

            if (!MathUtil.IsZero(hsl.Sat))
                return result;

            // WORDSNET-20719 Strange behavior of MS Word. If saturation is zero MS Word uses the saturation 
            // offset value multiplied by 5 when calculating the blue component. Received by an experimental way.
            const double blueComponentFactor = 5.0;
            DrColor colorZeroSat = HSLColor.ToDrColor(hsl.Hue, (Value * blueComponentFactor), hsl.Lum);

            return new DrColor(result.A, result.R, result.G, colorZeroSat.B);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.SaturationOffset; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlSaturationOffset();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("satOff", prefix, Value, writer);
        }
    }
}
