// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.5 blueMod (Blue Modification)
    /// This element specifies the input color with its blue 
    /// component modulated by the given percentage. A 50% blue 
    /// modulate reduces the blue component by half. A 200% blue 
    /// modulate doubles the blue component.
    /// </summary>
    internal class DmlBlueModulation : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.BlueModulation; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlBlueModulation();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the blue component.
            return (compIndex == 2) ? normComp * Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("blueMod", prefix, Value, writer);
        }
    }
}
