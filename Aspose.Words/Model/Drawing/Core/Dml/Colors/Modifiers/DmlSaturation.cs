// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.26 sat (Saturation)
    /// This element specifies the input color with the specified 
    /// saturation, but with its hue and luminance unchanged. Typically 
    /// saturation values fall in the range [0%, 100%].
    /// </summary>
    internal class DmlSaturation : DmlPercentageBasedColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            hsl.Sat = Value;
            return new DrColor(color.A, hsl.ToDrColor());
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Saturation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlSaturation();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("sat", prefix, Value, writer);
        }
    }
}
