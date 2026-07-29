// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2022 by Edward Voronov

using System;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies format attributes for the <see cref="FieldDatabase"/> \b switch.
    /// </summary>
    [Flags]
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum FieldDatabaseFormatAttributes
    {
        None = 0,
        Borders = 1,
        Shading = 2,
        Font = 4,
        Color = 8,
        AutoFit = 16,
        FirstRow = 32,
        LastRow = 64,
        FirstColumn = 128,
        LastColumn = 256
    }
}
