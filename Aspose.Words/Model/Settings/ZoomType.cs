// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Possible values for how large or small the document appears on the screen in Microsoft Word.
    /// </summary>
    /// <seealso cref="ViewOptions"/>
    /// <seealso cref="ViewOptions.ZoomType"/>
    public enum ZoomType
    {
        /// <summary>
        /// Zoom percentage is set explicitly. It is not recalculated automatically when control size changes.
        /// </summary>
        Custom = 0,

        /// <summary>
        /// Indicates to use the explicit zoom percentage. Same as <see cref="Custom"/>.
        /// </summary>
        None = Custom,

        /// <summary>
        /// Zoom percentage is automatically recalculated to fit one full page.
        /// </summary>
        FullPage = 1,

        /// <summary>
        /// Zoom percentage is automatically recalculated to fit page width.
        /// </summary>
        PageWidth = 2,

        /// <summary>
        /// Zoom percentage is automatically recalculated to fit text.
        /// </summary>
        TextFit = 3

    }
}
