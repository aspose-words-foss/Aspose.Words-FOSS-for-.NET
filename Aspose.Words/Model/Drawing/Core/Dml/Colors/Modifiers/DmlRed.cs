// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.23 red (Red)
    /// This element specifies the input color with the specified red component, 
    /// but with its green and blue color components unchanged.
    /// </summary>
    internal class DmlRed : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Red; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlRed();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the red component.
            return (compIndex == 0) ? Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("red", prefix, Value, writer);
        }
    }
}
