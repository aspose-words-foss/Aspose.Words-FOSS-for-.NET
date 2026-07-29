// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.34 tint (Tint)
    /// This element specifies a lighter version of its input color. 
    /// A 10% tint is 10% of the input color combined with 90% white.
    /// Value is in fraction representation.
    /// </summary>
    internal class DmlTint : DmlScRgbColorModifier
    {
        public DmlTint()
        {
        }

        internal DmlTint(double value) : base(value, false)
        {
        }

        internal DmlTint(double value, bool gammaShift) : base(value, gammaShift)
        {
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Tint; }
        }

        protected override double ModifyComponent(double normComp, int compIndex)
        {
            return (Value >= 0.0)
                ? normComp * Value + (1 - Value)
                : normComp * (1 + Value);
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlTint();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            WriteCore("tint", prefix, Value, writer);
        }
    }
}
