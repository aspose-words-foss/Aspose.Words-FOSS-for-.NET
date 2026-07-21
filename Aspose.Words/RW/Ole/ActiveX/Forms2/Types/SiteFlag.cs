// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/01/2019 by Ilya Navrotskiy

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies properties of an embedded control on a form.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS] 2.5.4.1 SITE_FLAG.
    /// </dev>
    [Flags]
    internal enum SiteFlag : uint
    {
        None = 0x00000000,
        TabStop = 0x00000001,
        Visible = 0x00000002,
        Default = 0x00000004,
        Cancel = 0x00000008,
        Streamed = 0x00000010,
        AutoSize = 0x00000020,
        Unused1 = 0x000000C0,
        PreserveHeight = 0x00000100,
        FitToParent = 0x00000200,
        Reserved1 = 0x00001C00,
        SelectChild = 0x00002000,
        Unused2 = 0x0003C000,
        PromoteControls = 0x00040000
    }
}
