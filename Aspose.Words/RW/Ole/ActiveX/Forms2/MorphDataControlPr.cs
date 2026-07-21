// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2020 by Ilya Navrotskiy

using Aspose.Words.Drawing.Ole;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Forms2 attributes collection for <see cref="MorphDataControl"/>.
    /// </summary>
    /// <remarks> This Forms 2.0 control has some default values different from the other Forms 2.0 controls.</remarks>
    internal class MorphDataControlPr : Forms2Pr
    {
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        static MorphDataControlPr()
        {
            gDefaults = new MorphDataControlPr();
            Forms2Pr baseDefaults = new Forms2Pr();
            baseDefaults.ExpandTo(gDefaults, true);

            gDefaults[Forms2Attr.BackgroundColor] = OleColor.FromRaw(0x80000005);
            gDefaults[Forms2Attr.ForegroundColor] = OleColor.FromRaw(0x80000008);
            gDefaults[Forms2Attr.SpecialEffect] = SpecialEffect.Sunken;
            gDefaults[Forms2Attr.VariousPropertyBits] =
                VariousPropertiesBits.Reserved1 |
                VariousPropertiesBits.Enabled |
                VariousPropertiesBits.BackStyle |
                VariousPropertiesBits.Reserved2 |
                VariousPropertiesBits.IntegralHeight |
                VariousPropertiesBits.WordWrap |
                VariousPropertiesBits.SelectionMargin |
                VariousPropertiesBits.AutoWordSelect |
                VariousPropertiesBits.HideSelection;
        }

        private static readonly MorphDataControlPr gDefaults;
    }
}
