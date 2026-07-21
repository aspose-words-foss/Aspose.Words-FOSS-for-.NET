// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.1 alpha (Alpha)
    /// This element specifies its input color with 
    /// the specific opacity, but with its color unchanged.
    /// </summary>
    internal class DmlAlpha : DmlPercentageBasedColorModifier, IDmlColorIvertableModifier
    {
        /// <summary>
        /// Apply the modifier to the color.
        /// </summary>
        /// <returns>The modified color.</returns>
        public override DrColor Modify(DrColor color)
        {
            return new DrColor(MathUtil.DoubleToInt(255.0*Value), color);
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Alpha; }
        }

        protected override DmlPercentageBasedColorModifier CreateEmptyObject()
        {
            return new DmlAlpha();
        }

        /// <summary>
        /// Inverts Alpha value.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IDmlColorIvertableModifier.Invert()
        {
            Value = 1 - Value;
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            double alphaVal = DmlPercentageUtil.ToDmlPercentPrecision(Value);
            WriteCore("alpha", prefix, alphaVal, writer);
        }
    }
}
