// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/04/2013 by Alexey Morozov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies types of ligatures are available for the run of text.
    /// </summary>
    [Flags]
    internal enum Ligature
    {
        /// <summary>
        /// Specifies that the text is not displayed using ligatures.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Specifies that the text is displayed using standard ligatures if they are supported by the font.
        /// </summary>
        Standard = 0x01,

        /// <summary>
        /// Specifies that the text is displayed using contextual ligatures if they are supported by the font.
        /// </summary>
        Contextual = 0x02,

        /// <summary>
        /// Specifies that the text is displayed using historical ligatures if they are supported by the font.
        /// </summary>
        Historical = 0x04,

        /// <summary>
        /// Specifies that the text is displayed using discretional ligatures if they are supported by the font.
        /// </summary>
        Discretional = 0x08,

        /// <summary>
        /// Specifies that the text is displayed using standard, historical, discretional, and contextual ligatures if they are supported by the font.
        /// </summary>
        All = Standard | Historical | Discretional | Contextual,

        /// <summary>
        /// Default value.
        /// </summary>
        Default = None
    }
}
