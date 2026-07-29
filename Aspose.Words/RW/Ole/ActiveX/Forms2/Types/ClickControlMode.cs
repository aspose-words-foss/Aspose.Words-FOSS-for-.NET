// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2019 by Ilya Navrotskiy

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies control behavior when the control is clicked.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS] 2.5.12 ClickControlMode.
    /// </dev>
    internal enum ClickControlMode
    {
        InsertionPoint = 0x00,
        SelectThenInsert = 0x01,
        Inherit = 0xFE,
        Default = 0xFF
    }
}
