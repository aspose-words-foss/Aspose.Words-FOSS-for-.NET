// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.12 greenOff (Green Offset)
    /// This element specifies the input color with its green component 
    /// shifted, but with its red and blue color components unchanged.
    /// </summary>
    internal class DmlGreenOffset : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.GreenOffset; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlGreenOffset();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the green component.
            return (compIndex == 1) ? normComp + Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("greenOff", prefix, Value, writer);
        }
    }
}
