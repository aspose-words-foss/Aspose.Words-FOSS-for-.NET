// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2010 by Roman Korchagin
using System;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies what document events are active (turned on) for a document.
    /// </summary>
    /// <dev>Do not renumber these because they match the binary DOC values.</dev>
    [Flags]
    internal enum VbaDocumentEvents
    {
        New = 0x0001,
        Open = 0x0002,
        Close = 0x0004,
        Sync = 0x0008,
        XmlAfterInsert = 0x0010,
        XmlBeforeDelete = 0x0020,
        ContentControlAfterInsert = 0x0100,
        ContentControlBeforeDelete = 0x0200,
        ContentControlOnExit = 0x0400,
        ContentControlOnEnter = 0x0800,
        StoreUpdate = 0x1000,
        ContentControlContentUpdate = 0x2000,
        BuildingBlockAfterInsert = 0x4000
    }
}
