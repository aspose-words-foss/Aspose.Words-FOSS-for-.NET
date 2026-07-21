// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.2 alphaMod (Alpha Modulation)
    /// This element specifies a more or less opaque version of 
    /// its input color. An alpha modulate never increases the alpha 
    /// beyond 100%. A 200% alpha modulate makes a input color twice 
    /// as opaque as before. A 50% alpha modulate makes a input color 
    /// half as opaque as before.
    /// </summary>
    internal class DmlAlphaModulation : DmlPercentageBasedColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            return new DrColor(MathUtil.DoubleToInt(color.A*Value), color);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.AlphaModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlAlphaModulation();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("alphaMod", prefix, Value, writer);
        }
    }
}
