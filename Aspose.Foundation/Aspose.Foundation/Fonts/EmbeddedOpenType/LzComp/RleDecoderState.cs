// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

namespace Aspose.Fonts.EmbeddedOpenType.LzComp
{
    internal enum RleDecoderState
    {
        Initial,
        Normal,
        SeenEscape,
        NeedBytes
    }
}
