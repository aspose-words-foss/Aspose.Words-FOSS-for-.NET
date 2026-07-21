// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2022 by Edward Voronov

using System;
using Aspose.EnumExtensionsGenerator;

namespace Aspose
{
    [Flags]
    [EnumExtensions(IsPublic = true)]
    public enum NumberFormattingOptions
    {
        Default = 0x0,

        /// <summary>
        /// DO NOT replace format digit placeholder '#" with space if there is no corresponding digit in value.
        /// </summary>
        /// <example>
        /// value=5 format="###"
        /// w/o option => result="  5"
        /// w/  option => result="5"
        /// </example>
        IgnoreUnmatchedDigitPlaceholder = 0x1,

        /// <summary>
        /// DO multiply value by 100 if format includes percent % character.
        /// w/o option: value=5 format="#%" => result="5%"
        /// w/  option: value=5 format="#%" => result="500%"
        /// </summary>
        IsMultiplyPercent = 0x2,

        /// <summary>
        /// Use <see cref="double.ToString(string,IFormatProvider)"/> method which does not fit MS Word behaviour.
        /// w/o option: value=12345 format="x##" => result="345"
        /// w/  option: value=12345 format="x##" => result="x12345"
        /// </summary>
        LegacyNumberFormat = 0x4,

        /// <summary>
        /// DO NOT convert format to invariant culture.
        /// w/o option: value=12345 format="0,000" culture=ru-RU => result="12345,000"
        /// w/  option: value=12345 format="0,000" culture=ru-RU => result="12 345"
        /// </summary>
        FormatIsInInvariantCulture = 0x8
    }
}
