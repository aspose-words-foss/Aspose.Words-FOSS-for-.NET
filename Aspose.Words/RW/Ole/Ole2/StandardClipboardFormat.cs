// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2012 by Alexey Morozov

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Defines Standard Clipboard formats. See [MS-OLEDS] 2.1.1 Clipboard Formats.
    /// </summary>
    internal enum StandardClipboardFormat
    {
        None = 0x0001,
        /// <summary>
        /// Bitmap16 Object structure.
        /// </summary>
        Bitmap = 0x0002,
        /// <summary>
        /// Windows Metafile.
        /// </summary>
        MetafilePict = 0x0003,
        /// <summary>
        /// DeviceIndependentBitmap Object structure.
        /// </summary>
        Dib = 0x0008,
        /// <summary>
        /// Enhanced Metafile.
        /// </summary>
        EnhMetafile = 0x000e
    }
}
