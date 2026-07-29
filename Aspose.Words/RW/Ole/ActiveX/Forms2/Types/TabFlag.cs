// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/01/2019 by Ilya Navrotskiy

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies whether a tab is visible and whether it is enabled.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS], 2.2.9.7 TabStripTabFlag.
    /// </dev>
    [Flags]
    internal enum TabFlag
    {
        None = 0x00000000,
        Visible = 0x00000001,
        Enabled = 0x00000002
    }
}
