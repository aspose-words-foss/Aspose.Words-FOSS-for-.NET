// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.3 alphaOff (Alpha Offset)
    /// This element specifies a more or less opaque version of its input 
    /// color. Increases or decreases the input alpha percentage by the 
    /// specified percentage offset. A 10% alpha offset increases a 50% 
    /// opacity to 60%. A -10% alpha offset decreases a 50% opacity to 40%. 
    /// The transformed alpha values are limited to a range of 0 to 100%. 
    /// A 10% alpha offset increase to a 100% opaque object still results 
    /// in 100% opacity.
    /// </summary>
    internal class DmlAlphaOffset : DmlPercentageBasedColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            return new DrColor(color.A + MathUtil.DoubleToInt(255.0*Value), color);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.AlphaOffset; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlAlphaOffset();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("alphaOff", prefix, Value, writer);
        }
    }
}
