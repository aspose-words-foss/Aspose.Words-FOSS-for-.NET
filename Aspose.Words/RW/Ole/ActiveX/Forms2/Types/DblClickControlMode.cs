// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2019 by Ilya Navrotskiy

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies the behavior when the user double-clicks a form or an item on the form.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS] 2.5.18 DblClickControlMode.
    /// </dev>
    internal enum DblClickControlMode
    {
        SelectText = 0x00,
        EditCode = 0x01,
        EditProperties = 0x02,
        Inherit = 0xFE
    }
}
