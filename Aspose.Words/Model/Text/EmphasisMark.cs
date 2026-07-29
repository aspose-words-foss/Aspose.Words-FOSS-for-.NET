// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2011 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies possible types of emphasis mark.
    /// </summary>
    /// <dev>
    /// Names borrowed from https://docs.microsoft.com/en-us/dotnet/api/microsoft.office.interop.word.wdemphasismark?view=word-pia
    /// </dev>
    public enum EmphasisMark
    {
        /// <summary>
        /// No emphasis mark.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Emphasis mark is a solid black circle displayed above text.
        /// </summary>
        OverSolidCircle = 0x01,

        /// <summary>
        /// Emphasis mark is a comma character displayed above text.
        /// </summary>
        OverComma = 0x02,

        /// <summary>
        /// Emphasis mark is an empty white circle displayed above text.
        /// </summary>
        OverWhiteCircle = 0x03,

        /// <summary>
        /// Emphasis mark is a solid black circle displayed below text.
        /// </summary>
        UnderSolidCircle = 0x04,
    }
}
