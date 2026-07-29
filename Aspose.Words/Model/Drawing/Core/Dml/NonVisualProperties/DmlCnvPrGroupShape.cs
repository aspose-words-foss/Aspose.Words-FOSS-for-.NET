// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 21.3.2.5 cNvGrpSpPr (Non-Visual Group Shape Drawing Properties)
    /// This element specifies the non-visual drawing properties for a group shape.
    /// </summary>
    internal class DmlCnvPrGroupShape : DmlCnvPrBase
    {
        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.GroupShape; }
        }
    }
}
