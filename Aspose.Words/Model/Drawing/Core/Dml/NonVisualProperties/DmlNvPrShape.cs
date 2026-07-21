// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml.NonVisualProperties
{
    /// <summary>
    /// 20.1.2.2.29 nvSpPr (Non-Visual Properties for a Shape)
    /// This element specifies all non-visual properties for a shape.
    /// </summary>
    internal class DmlNvPrShape : DmlNvPrBase
    {
        internal override DmlNvHolder Holder
        {
            get { return DmlNvHolder.Shape; }
        }
    }
}
