// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/05/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Images;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    internal interface IDmlBlipFillMode
    {
        IDmlBlipFillMode Clone();

        /// <summary>
        /// The corresponded VML fill type.
        /// </summary>
        FillTypeCore FillType { get; }

        /// <summary>
        /// Indicates whether the blip has offsets from the bounding box.
        /// </summary>
        bool HasOffsets { get; }

        void Write(IDmlShapeWriterContext writer);
    }
}
