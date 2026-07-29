// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2009 by Roman Korchagin

using System;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Themes
{
    /// <summary>
    /// Corresponds to 2.18.103 ST_Theme (Theme Font).
    /// </summary>
    /// <dev>
    /// This is internal theme fonts related to public <see cref="ThemeFont"/>.
    /// </dev>
    [Flags]
    internal enum ThemeFontCore
    {
        None            = 0x00000000,

        MajorAscii      = 0x00010001,
        MajorBidi       = 0x00010002,
        MajorEastAsia   = 0x00010003,
        MajorHAnsi      = 0x00010004,

        MinorAscii      = 0x00020001,
        MinorBidi       = 0x00020002,
        MinorEastAsia   = 0x00020003,
        MinorHAnsi      = 0x00020004,


        GroupMask       = 0x00ff0000,
        Major           = 0x00010000,
        Minor           = 0x00020000,

        RegionMask      = 0x0000ffff,
        Ascii           = 0x00000001,
        Bidi            = 0x00000002,
        EastAsia        = 0x00000003,
        HAnsi           = 0x00000004,
    }
}
