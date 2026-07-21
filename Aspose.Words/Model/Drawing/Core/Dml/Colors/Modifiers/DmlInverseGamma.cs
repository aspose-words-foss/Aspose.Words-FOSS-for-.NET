// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.18 invGamma (Inverse Gamma)
    /// This element specifies that the output color rendered by the 
    /// generating application should be the inverse sRGB gamma shift of the input color.
    /// </summary>
    internal class DmlInverseGamma : DmlColorModifier
    {
        public override IDmlColorModifier Clone()
        {
            return new DmlInverseGamma();
        }

        public override DrColor Modify(DrColor color)
        {
            int r = InverseGamma(color.R);
            int g = InverseGamma(color.G);
            int b = InverseGamma(color.B);
            return new DrColor(color.A, r, g, b);
        }

        private static int InverseGamma(int comp)
        {
            double normComp = comp / 255.0;
            return MathUtil.DoubleToInt(ScRgbUtil.Linearize(normComp) * 255);
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            writer.Builder.WriteEmptyElement(string.Format("{0}:invGamma", prefix));
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.InverseGamma; }
        }
    }
}
