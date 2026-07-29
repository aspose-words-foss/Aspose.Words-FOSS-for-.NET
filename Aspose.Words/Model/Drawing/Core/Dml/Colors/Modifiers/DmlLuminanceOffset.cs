// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.21 lumOff (Luminance Offset)
    /// This element specifies the input color with its luminance shifted, 
    /// but with its hue and saturation unchanged.
    /// </summary>
    internal class DmlLuminanceOffset : DmlPercentageBasedColorModifier
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        internal DmlLuminanceOffset()
        {
        }

        /// <summary>
        /// Ctor with specifying a luminance offset.
        /// </summary>
        internal DmlLuminanceOffset(double value)
        {
            Value = value;
        }

        public override DrColor Modify(DrColor color)
        {
            HSLColor hsl = new HSLColor(color);
            hsl.Lum += Value;
            return new DrColor(color.A, hsl.ToDrColor());
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.LuminanceOffset; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlLuminanceOffset();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("lumOff", prefix, Value, writer);
        }
    }
}
