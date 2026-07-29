// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.31 shade (Shade)
    /// This element specifies a darker version of its input color. 
    /// A 10% shade is 10% of the input color combined with 90% black.
    /// Value is in fraction representation.
    /// </summary>
    internal class DmlShade : DmlScRgbColorModifier
    {
        internal DmlShade()
        {
        }

        internal DmlShade(double value) : base(value, false)
        {
        }

        internal DmlShade(double value, bool gammaShift) : base(value, gammaShift)
        {
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            return MathUtil.FitToRange(normComp * Value, 0.0, 1.0);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Shade; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlShade();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("shade", prefix, Value, writer);
        }
    }
}
