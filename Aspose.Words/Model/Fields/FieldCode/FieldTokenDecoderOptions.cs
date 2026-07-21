// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/05/2023 by Edward Voronov

using System;
using Aspose.EnumExtensionsGenerator;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents options for the <see cref="FieldTokenDecoder"/> class.
    /// </summary>
    [Flags]
    [EnumExtensions]
    internal enum FieldTokenDecoderOptions
    {
        None = 0,
        EscapeChars = 1,
        TrimDoubleQuotes = 2,
        PreserveSymbolicQuotes = 4,

        All = EscapeChars | TrimDoubleQuotes | PreserveSymbolicQuotes
    }
}
