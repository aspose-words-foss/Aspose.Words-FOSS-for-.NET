// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Implements [MS-OFORMS] 2.5.96.1 VariousPropertiesBitfield.
    /// </summary>
    [Flags]
    internal enum VariousPropertiesBits : uint
    {
        None = 0x00000000,
        Reserved1 = 0x00000001,
        Enabled = 0x00000002,
        Locked = 0x00000004,
        BackStyle = 0x00000008,
        Reserved2 = 0x00000010,
        UnusedBits11 = 0x00000020,
        UnusedBits12 = 0x00000040,
        UnusedBits13 = 0x00000080,
        UnusedBits14 = 0x00000100,
        UnusedBits15 = 0x00000200,
        ColumnHeads = 0x00000400,
        IntegralHeight = 0x00000800,
        MatchRequired = 0x00001000,
        Alignment = 0x00002000,
        Editable = 0x00004000,
        IMEMode = 0x00078000,
        DragBehavior = 0x00080000,
        EnterKeyBehavior = 0x00100000,
        EnterFieldBehavior = 0x00200000,
        TabKeyBehavior = 0x00400000,
        WordWrap = 0x00800000,
        UnusedBits2 = 0x01000000,
        BordersSuppress = 0x02000000,
        SelectionMargin = 0x04000000,
        AutoWordSelect = 0x08000000,
        AutoSize = 0x10000000,
        HideSelection = 0x20000000,
        AutoTab = 0x40000000,
        MultiLine = 0x80000000
    }
}
