// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.25 redOff (Red Offset)
    /// This element specifies the input color with its red component 
    /// shifted, but with its green and blue color components unchanged.
    /// </summary>
    internal class DmlRedOffset : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.RedOffset; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlRedOffset();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the red component.
            return (compIndex == 0) ? normComp + Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("redOff", prefix, Value, writer);
        }
    }
}
