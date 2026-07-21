// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.6 blueOff (Blue Offset)
    /// This element specifies the input color with its blue component 
    /// shifted, but with its red and green color components unchanged.
    /// </summary>
    internal class DmlBlueOffset : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.BlueOffset; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlBlueOffset();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the blue component.
            return (compIndex == 2) ? normComp + Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("blueOff", prefix, Value, writer);
        }
    }
}
