// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/10/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies the type of an OLE object.
    /// Ideally I would want this to replace the two OLE-related shape types.
    /// </summary>
    internal enum OleObjectType
    {
        /// <summary>
        /// The object is not an OLE object.
        /// </summary>
        None,
        /// <summary>
        /// The object is an embedded OLE object.
        /// </summary>
        Embedded,
        /// <summary>
        /// The object is a linked OLE object.
        /// </summary>
        Linked,
        /// <summary>
        /// The object is an ActiveX control.
        /// </summary>
        Control,
    }
}
