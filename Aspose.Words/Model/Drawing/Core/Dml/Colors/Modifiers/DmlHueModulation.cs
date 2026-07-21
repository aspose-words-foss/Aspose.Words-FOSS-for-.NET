// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.15 hueMod (Hue Modulate)
    /// This element specifies the input color with its hue modulated 
    /// by the given percentage. A 50% hue modulate decreases the angular 
    /// hue value by half. A 200% hue modulate doubles the angular hue value.
    /// </summary>
    internal class DmlHueModulation : DmlPercentageBasedColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            hsl.Hue *= Value;
            return new DrColor(color.A, hsl.ToDrColor());
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.HueModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlHueModulation();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("hueMod", prefix, Value, writer);
        }
    }
}
