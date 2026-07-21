// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.11 greenMod (Green Modification)
    /// This element specifies the input color with its green 
    /// component modulated by the given percentage. A 50% green 
    /// modulate reduces the green component by half. A 200% 
    /// green modulate doubles the green component.
    /// </summary>
    internal class DmlGreenModulation : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.GreenModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlGreenModulation();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the green component.
            return (compIndex == 1) ? normComp * Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("greenMod", prefix, Value, writer);
        }
    }
}
