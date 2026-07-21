// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/01/2011 by Andrey Soldatov

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Specifies the style of an embedded font inside a <see cref="FontInfo"/> object.
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames",
        Justification = "Public API.")]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "Public API, as designed. Integer values of enum items are used on reading/writing DOC files. So do not change them.")]
    public enum EmbeddedFontStyle
    {
        //Integer values of enum items are used on reading/writing DOC files. So do not change them.

        /// <summary>
        /// Specifies the Regular embedded font.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Specifies the Bold embedded font.
        /// </summary>
        Bold = 1,

        /// <summary>
        /// Specifies the Italic embedded font.
        /// </summary>
        Italic = 2,

        /// <summary>
        /// Specifies the Bold-Italic embedded font.
        /// </summary>
        BoldItalic = 3,
    }
}
