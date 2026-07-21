// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2006 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words
{
    /// <summary>
    /// Represents type of the style.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "Public API, as designed.")]
    public enum StyleType
    {
        // Integer value of the type is made to match with std.sgc value.
        /// <summary>
        /// The style is a paragraph style.
        /// </summary>
        Paragraph = 1,
        /// <summary>
        /// The style is a character style.
        /// </summary>
        Character = 2,
        /// <summary>
        /// The style is a table style.
        /// </summary>
        Table = 3,
        /// <summary>
        /// The style is a list style.
        /// </summary>
        List = 4
    }
}
