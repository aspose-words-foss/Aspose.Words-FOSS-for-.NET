// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.24 redMod (Red Modulation)
    /// This element specifies the input color with its red component 
    /// modulated by the given percentage. A 50% red modulate reduces the 
    /// red component by half. A 200% red modulate doubles the red component.
    /// </summary>
    internal class DmlRedModulation : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.RedModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlRedModulation();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the red component.
            return (compIndex == 0) ? normComp * Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("redMod", prefix, Value, writer);
        }
    }
}
