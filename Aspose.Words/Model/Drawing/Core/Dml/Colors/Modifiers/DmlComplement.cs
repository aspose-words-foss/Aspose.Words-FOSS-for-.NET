// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// 20.1.2.3.7 comp (Complement)
    /// This element specifies that the color rendered should be the complement 
    /// of its input color with the complement being defined as such. Two colors 
    /// are called complementary if, when mixed they produce a shade of grey. 
    /// For instance, the complement of red which is RGB (255, 0, 0) 
    /// is cyan which is RGB (0, 255, 255).
    /// Primary colors and secondary colors are typically paired in this way:
    /// red and cyan (where cyan is the mixture of green and blue) 
    /// green and magenta (where magenta is the mixture of red and blue) 
    /// blue and yellow (where yellow is the mixture of red and green)
    /// </summary>
    internal class DmlComplement : DmlColorModifier
    {
        public override DrColor Modify(DrColor color)
        {
            //Algorithm description http://serennu.com/colour/rgbtohsl.php
            HSLColor hsl = new HSLColor(color);
            double tmp = hsl.Hue + 0.5; // To opposite color on the color wheel.
            if (tmp > 1.0)
                tmp -= 1.0;
            hsl.Hue = tmp;
            return hsl.ToDrColor();
        }

        public override IDmlColorModifier Clone()
        {
            return new DmlComplement();
        }

        public override void Write(string prefix, IDmlShapeWriterContext writer)
        {
            writer.Builder.WriteEmptyElement(string.Format("{0}:comp", prefix));
        }

        public override DmlColorModifierType ModifierType
        {
            get { return DmlColorModifierType.Complement; }
        }
    }
}
