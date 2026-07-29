// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2012 by Konstantin Kornilov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how Aspose.Words should render EMF+ Dual metafiles.
    /// </summary>
    public enum EmfPlusDualRenderingMode
    {
        /// <summary>
        /// Aspose.Words tries to render EMF+ part of EMF+ Dual metafile. If some of the EMF+ records are not supported
        /// then Aspose.Words renders EMF part of EMF+ Dual metafile.
        /// </summary>
        EmfPlusWithFallback = 0,
        /// <summary>
        /// Aspose.Words renders EMF+ part of EMF+ Dual metafile.
        /// </summary>
        EmfPlus = 1,
        /// <summary>
        /// Aspose.Words renders EMF part of EMF+ Dual metafile.
        /// </summary>
        Emf = 2
    }
}
