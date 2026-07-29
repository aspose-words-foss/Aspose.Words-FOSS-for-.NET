// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2017 by Alexey Butalov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// This class is used together with <see cref="IVmlShapeTypeReader"/> interface to break the direct dependency 
    /// Aspose.Words.Model from RW.Vml.
    /// </summary>
    internal class VmlShapeTypeReader : IVmlShapeTypeReader
    {
        /// <summary>
        /// Reads a shapeType element and returns parsed shape properties.
        /// </summary>
        public ShapePr ReadShapeType(string shapeTypeXml)
        {
            return VmlShapeReader.ReadShapeType(new VmlShapeTypeReaderContext(shapeTypeXml));
        }
    }
}
