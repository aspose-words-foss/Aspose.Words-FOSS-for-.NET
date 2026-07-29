// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/02/2017 by Alexey Butalov

using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Nrx.Reader
{
    internal interface INrxVmlReader
    {
        /// <summary>
        /// Reads a shape container element such as w:pict or w:object.
        /// Reader should be positioned to element start.
        /// </summary>
        [JavaThrows(true)]
        IList<ShapeBase> Read(IVmlShapeReaderContext context);
    }
}
