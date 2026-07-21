// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.17 inv (Inverse)
    /// This element specifies the inverse of its input color. 
    /// </summary>
    internal class DmlInverse : DmlColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            return new DrColor(color.A, 255 - color.R, 255 - color.G, 255 - color.B);
        }

        public override IDmlColorModifier Clone()
        {
            return new DmlInverse();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            writer.Builder.WriteEmptyElement(string.Format("{0}:inv", prefix));
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Inverse; }
        }
    }
}
