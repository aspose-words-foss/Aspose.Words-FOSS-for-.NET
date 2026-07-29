// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.20 lumMod (Luminance Modulation)
    /// This element specifies the input color with its luminance modulated 
    /// by the given percentage. A 50% luminance modulate reduces the luminance 
    /// by half. A 200% luminance modulate doubles the luminance.
    /// </summary>
    internal class DmlLuminanceModulation : DmlPercentageBasedColorModifier
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        internal DmlLuminanceModulation()
        {
        }

        /// <summary>
        /// Ctor with specifying a luminance modulation.
        /// </summary>
        internal DmlLuminanceModulation(double value)
        {
            Value = value;
        }

        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            hsl.Lum *= Value;
            return new DrColor(color.A, hsl.ToDrColor());
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.LuminanceModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlLuminanceModulation();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("lumMod", prefix, Value, writer);
        }
    }
}
