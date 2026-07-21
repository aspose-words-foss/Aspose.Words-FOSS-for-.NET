// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/11/2020 by Ilya Navrotskiy

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Forms2 attributes collection for <see cref="ImageControl"/>.
    /// </summary>
    /// <remarks> This Forms 2.0 control has some default values different from the other Forms 2.0 controls.</remarks>
    internal class ImageControlPr : Forms2Pr
    {
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        static ImageControlPr()
        {
            gDefaults = new ImageControlPr();
            Forms2Pr baseDefaults = new Forms2Pr();
            baseDefaults.ExpandTo(gDefaults, true);

            gDefaults[Forms2Attr.BorderStyle] = BorderStyle.Single;
        }

        private static readonly ImageControlPr gDefaults;
    }
}
