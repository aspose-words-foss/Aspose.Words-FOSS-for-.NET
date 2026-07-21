// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 21.3.2.6 cNvPicPr (Non-Visual Picture Drawing Properties)
    /// This element specifies the non-visual properties for the picture canvas.
    /// </summary>
    internal class DmlCnvPrPicture : DmlCnvPrBase
    {
        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.Picture; }
        }

        /// <summary>
        /// Specifies if the user interface should show the resizing of the picture
        /// based on the picture's current size or its original size.
        /// Default is true.
        /// </summary>
        internal bool PreferRelativeResize
        {
            get { return mPreferRelativeResize; }
            set { mPreferRelativeResize = value; }
        }

        private bool mPreferRelativeResize = true;
    }
}
