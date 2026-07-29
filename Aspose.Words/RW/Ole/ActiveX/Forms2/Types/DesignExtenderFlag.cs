// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2019 by Ilya Navrotskiy

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies boolean design-time properties of a UserForm.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS] 2.5.5.1 DX_MODE.
    /// </dev>
    [Flags]
    internal enum DesignExtenderFlag : uint
    {
        None = 0x00000000,
        InheritDesign = 0x00000001,
        Design = 0x00000002,
        InheritShowToolbox = 0x00000004,
        ShowToolbox = 0x00000008,
        InheritShowGrid = 0x00000010,
        ShowGrid = 0x00000020,
        InheritSnapToGrid = 0x00000040,
        SnapToGrid = 0x00000080,
        InheritGridX = 0x00000100,
        InheritGridY = 0x00000200,
        InheritClickControl = 0x00000400,
        InheritDblClickControl = 0x00000800,
        InheritShowInvisible = 0x00001000,
        ShowInvisible = 0x00002000,
        InheritShowTooltips = 0x00004000,
        ShowTooltips = 0x00008000,
        InheritLayoutImmediate = 0x00010000,
        LayoutImmediate = 0x00020000
    }
}
