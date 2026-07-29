// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2011 by Alexey Titov

namespace Aspose.Words.Themes
{
    /// <summary>
    /// Specifies the theme colors for document themes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-styles-and-themes/">Working with Styles and Themes</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// The specified theme color is a reference to one of the predefined theme colors, located in the
    /// document's Theme part, which allows color information to be set centrally in the document.
    /// </remarks>
    /// <dev>
    /// The Color Scheme Color elements appear in a sequence.
    /// See also https://docs.microsoft.com/en-us/office/vba/api/word.wdthemecolorindex
    /// </dev>
    public enum ThemeColor
    {
        /// <summary>
        /// No color.
        /// </summary>
        None = -1,

        /// <summary>
        /// Dark main color 1.
        /// </summary>
        Dark1,

        /// <summary>
        /// Light main color 1.
        /// </summary>
        Light1,

        /// <summary>
        /// Dark main color 2.
        /// </summary>
        Dark2,

        /// <summary>
        /// Light main color 2.
        /// </summary>
        Light2,

        /// <summary>
        /// Accent color 1.
        /// </summary>
        Accent1,

        /// <summary>
        /// Accent color 2.
        /// </summary>
        Accent2,

        /// <summary>
        /// Accent color 3.
        /// </summary>
        Accent3,

        /// <summary>
        /// Accent color 4.
        /// </summary>
        Accent4,

        /// <summary>
        /// Accent color 5.
        /// </summary>
        Accent5,

        /// <summary>
        /// Accent color 6.
        /// </summary>
        Accent6,

        /// <summary>
        /// Hyperlink color.
        /// </summary>
        Hyperlink,

        /// <summary>
        /// Followed hyperlink color.
        /// </summary>
        FollowedHyperlink,

        /// <summary>
        /// Text color 1.
        /// </summary>
        Text1,

        /// <summary>
        /// Text color 2.
        /// </summary>
        Text2,

        /// <summary>
        /// Background color 1.
        /// </summary>
        Background1,

        /// <summary>
        /// Background color 2.
        /// </summary>
        Background2
    }
}
