// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/12/2015 by Alexey Morozov

using System;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Word 6.0 headers/footers presence flags.
    /// </summary>
    [Flags]
    internal enum GrpfIhdt
    {
        None = 0x00,

        HeaderEven = 0x01,
        HeaderPrimary = 0x02,
        FooterEven = 0x04,
        FooterPrimary = 0x08,
        HeaderFirst = 0x10,
        FooterFirst = 0x20,

        All = FooterFirst | HeaderFirst | FooterPrimary | FooterEven | HeaderPrimary | HeaderEven
    }

    /// <summary>
    /// Word 6.0 footnotes presence flags.
    /// </summary>
    [Flags]
    internal enum GrpfIhdtDop
    {
        None = 0x00,

        FootnoteSeparator = 0x01,
        FootnoteContinuationSeparator = 0x02,
        FootnoteContinuationNotice = 0x04,

        EndnoteSeparator = 0x08,
        EndnoteContinuationSeparator = 0x10,
        EndnoteContinuationNotice = 0x20
    }
}
