// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2018 by Ilya Navrotskiy

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies the visual attributes of the text displayed by the control that contains the TextProps to which this
    /// property applies.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS] 2.5.26 FontEffects.
    /// </dev>
    [Flags]
    internal enum FontEffects : uint
    {
        None = 0x00000000,
        Bold = 0x00000001,
        Italic = 0x00000002,
        Underline = 0x00000004,
        Strikeout = 0x00000008,
        Disabled = 0x00002000,
        AutoColor = 0x40000000
    }
}
