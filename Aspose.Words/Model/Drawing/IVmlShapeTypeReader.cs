// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2017 by Alexey Butalov

using Aspose.JavaAttributes;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// This interface is used to break the direct dependency Aspose.Words.Model from RW.Vml.
    /// We use a stub implementation of IVmlShapeTypeReader in C++ branches until RW.Vml isn't ported to C++. 
    /// </summary>
    internal interface IVmlShapeTypeReader
    {
        /// <summary>
        /// Reads a shapeType element and returns parsed shape properties.
        /// </summary>
        [JavaThrows(true)]
        ShapePr ReadShapeType(string shapeTypeXml);
    }
}
