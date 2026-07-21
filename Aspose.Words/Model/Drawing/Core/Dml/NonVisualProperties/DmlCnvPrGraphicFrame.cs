// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 21.3.2.4 cNvGraphicFramePr (Non-Visual Graphic Frame Drawing Properties)
    /// This element specifies the non-visual drawing properties for a graphic frame.
    /// </summary>
    internal class DmlCnvPrGraphicFrame : DmlCnvPrBase
    {
        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.GraphicFrame; }
        }
    }
}
