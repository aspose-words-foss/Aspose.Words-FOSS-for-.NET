// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2012 by Alexey Morozov

using System;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements 2.9.167 ODTPersist2. See http://msdn.microsoft.com/en-us/library/gg132373(v=office.12).aspx.
    /// </summary>
    [Flags]
    internal enum OdtPersist2
    {
        None = 0x0000,
        /// <summary>
        /// Specifies that the presentation of this OLE object in the document is in the Enhanced Metafile format. 
        /// This is different from StoredAsEMF in the case of an object being represented as an icon. 
        /// For icons, the icon can be an Enhanced Metafile even if the OLE object does not support the Enhanced Metafile format.
        /// </summary>
        Emf = 0x0001,
        Reserved1 = 0x0002,
        /// <summary>
        /// Specifies whether the application that saved this Word Binary file had queried this OLE object 
        /// to determine whether it supported the Enhanced Metafile format.
        /// </summary>
        QueriedEmf = 0x0004,
        /// <summary>
        /// Specifies that this OLE object supports the Enhanced Metafile format.
        /// </summary>
        StoredAsEmf = 0x0008,
    }
}
