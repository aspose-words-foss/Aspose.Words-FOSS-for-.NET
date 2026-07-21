// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2011 by Alexey Titov

using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// Represents 20.1.8.57 tailEnd (Tail line end style)
    /// This element specifies decorations which can be added to the tail of a line.
    /// </summary>
    internal class DmlTailLineEndStyle : DmlLineEndStyle
    {
        public override DmlLineEndStyle Clone()
        {
            DmlTailLineEndStyle result = new DmlTailLineEndStyle();
            CopyTo(result);
            return result;
        }
    }
}
