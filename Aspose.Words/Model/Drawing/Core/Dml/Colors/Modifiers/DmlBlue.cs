// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.4 blue (Blue)
    /// This element specifies the input color with the specific blue 
    /// component, but with the red and green color components unchanged.
    /// </summary>
    internal class DmlBlue : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Blue; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlBlue();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the blue component.
            return (compIndex == 2) ? Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("blue", prefix, Value, writer);
        }
    }
}
