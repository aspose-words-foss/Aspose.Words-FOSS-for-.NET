// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.10 green (Green)
    /// This elements specifies the input color with the specified 
    /// green component, but with its red and blue color components unchanged.
    /// </summary>
    internal class DmlGreen : DmlScRgbColorModifier
    {
        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Green; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlGreen();
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            // Change only the green component.
            return (compIndex == 1) ? Value : normComp;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("green", prefix, Value, writer);
        }
    }
}
