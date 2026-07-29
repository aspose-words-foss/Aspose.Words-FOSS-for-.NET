// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2010 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// This interface breaks a dependency from the Nrx document reader to the object that will actually read DrawingML.
    /// </summary>
    internal interface INrxDmlReader
    {
        /// <summary>
        /// The implementation needs to read a DrawingML node from the current position (beginning of a w:drawing element)
        /// and add it to the model appropriately. The implementation needs to assign the provided run properties to the new node.
        /// </summary>
        [JavaThrows(true)]  // IO Exceptions
        ShapeBase ReadDrawing(RunPr runPr);
    }
}
