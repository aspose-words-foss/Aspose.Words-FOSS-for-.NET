// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.8 gamma (Gamma)
    /// This element specifies that the output color rendered by 
    /// the generating application should be the sRGB gamma shift of the input color.
    /// </summary>
    internal class DmlGamma : DmlColorModifier
    {
        public override IDmlColorModifier Clone()
        {
            return new DmlGamma();
        }

        public override DrColor Modify(DrColor color)
        {
            int r = Gamma(color.R);
            int g = Gamma(color.G);
            int b = Gamma(color.B);
            return new DrColor(color.A, r, g, b);
        }

        private static int Gamma(int comp)
        {
            double normComp = comp / 255.0;
            return MathUtil.DoubleToInt(ScRgbUtil.DeLinearize(normComp) * 255);
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            writer.Builder.WriteEmptyElement(string.Format("{0}:gamma", prefix));
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Gamma; }
        }
    }
}
