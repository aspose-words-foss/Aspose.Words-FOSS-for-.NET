// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.27 satMod (Saturation Modulation)
    /// This element specifies the input color with its saturation 
    /// modulated by the given percentage. A 50% saturation modulate 
    /// reduces the saturation by half. A 200% saturation modulate 
    /// doubles the saturation.
    /// </summary>
    internal class DmlSaturationModulation : DmlPercentageBasedColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);

            // WORDSNET-20638 MS Word does not limit the product of the saturation modulation value and the saturation.
            double satWithSatMod = hsl.Sat * Value;

            return new DrColor(color.A, HSLColor.ToDrColor(hsl.Hue, satWithSatMod, hsl.Lum));
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.SaturationModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlSaturationModulation();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("satMod", prefix, Value, writer);
        }
    }
}
