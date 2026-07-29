// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/11/2020 by Ilya Navrotskiy

namespace Aspose.Words.Themes
{
    /// <summary>
    /// Specifies the types of theme font names for document themes.
    /// </summary>
    /// <remarks>
    /// Specifies a theme font type which can be referenced as a theme font within the parent object properties.
    /// This theme font is a reference to one of the predefined theme fonts, located in the document's
    /// Theme part, which allows for font information to be set centrally in the document.
    /// </remarks>
    /// <dev>
    /// See https://docs.microsoft.com/en-us/office/vba/api/excel.font.themefont
    /// and ISO29500 17.18.96 ST_Theme (Theme Font).
    /// </dev>
    public enum ThemeFont
    {
        /// <summary>
        /// No theme font.
        /// </summary>
        None,

        /// <summary>
        /// Major theme font.
        /// </summary>
        Major,

        /// <summary>
        /// Minor theme font.
        /// </summary>
        Minor
    }
}
