// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2011 by Alexey Titov

using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// Represents a 20.1.8.38 headEnd (Line Head/End Style)
    /// This element specifies decorations which can be added to the head of a line.
    /// </summary>
    internal class DmlHeadLineEndStyle : DmlLineEndStyle
    {
        public override DmlLineEndStyle Clone()
        {
            DmlHeadLineEndStyle result = new DmlHeadLineEndStyle();
            CopyTo(result);
            return result;
        }
    }
}
