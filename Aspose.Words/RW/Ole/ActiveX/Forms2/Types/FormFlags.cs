// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies Boolean properties of a form.
    /// </summary>
    [Flags]
    internal enum FormFlags : uint
    {
        Unused11 = 0x00000001,
        Unused12 = 0x00000002,
        Enabled = 0x00000004,

        Unused21 = 0x00000008,
        Unused22 = 0x00000010,
        Unused23 = 0x00000020,
        Unused24 = 0x00000040,
        Unused25 = 0x00000080,
        Unused26 = 0x00000100,
        Unused27 = 0x00000200,
        Unused28 = 0x00000400,
        Unused29 = 0x00000800,
        Unused2A = 0x00001000,
        Unused2B = 0x00002000,

        DesinkPersisted = 0x00004000,
        DontSaveClassTable = 0x00008000
    }
}
