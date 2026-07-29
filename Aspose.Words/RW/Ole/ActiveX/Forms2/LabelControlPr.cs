// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2020 by Ilya Navrotskiy

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Forms2 attributes collection for <see cref="LabelControl"/>.
    /// </summary>
    /// <remarks> This Forms 2.0 control has some default values different from the other Forms 2.0 controls.</remarks>
    internal class LabelControlPr : Forms2Pr
    {
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        static LabelControlPr()
        {
            gDefaults = new LabelControlPr();
            Forms2Pr baseDefaults = new Forms2Pr();
            baseDefaults.ExpandTo(gDefaults, true);

            gDefaults[Forms2Attr.VariousPropertyBits] =
                VariousPropertiesBits.Reserved1 |
                VariousPropertiesBits.Enabled |
                VariousPropertiesBits.BackStyle |
                VariousPropertiesBits.Reserved2 |
                VariousPropertiesBits.WordWrap;
        }

        private static readonly LabelControlPr gDefaults;
    }
}
