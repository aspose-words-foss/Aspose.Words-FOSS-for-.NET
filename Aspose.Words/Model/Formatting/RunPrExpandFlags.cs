// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/08/2009 by Roman Korchagin
using System;

namespace Aspose.Words
{
    [Flags]
    internal enum RunPrExpandFlags
    {
        /// <summary>
        /// Performs the normal "plain" expansion of attributes.
        /// </summary>
        Normal = 0x0000,

        /// <summary>
        /// Set to expand default run properties stored for this document.
        /// </summary>
        DocumentDefaults = 0x0001,

        /// <summary>
        /// In some cases MS Word ignores font size 10 and 12 specified on Normal paragraph style
        /// when text is inside a table.
        /// </summary>
        /// <remarks>This flag is used internally when expanding attribute collection. Do NOT use it anywhere else.</remarks>
        // See https://msdn.microsoft.com/en-us/library/dd944611%28v=office.12%29.aspx?f=255&MSPPError=-2147217396
        IgnoreNormalFontSize = 0x0002,

        /// <summary>
        /// This is requested during layout for runs that are inside a TOC field. They are likely to have the
        /// Hyperlink style applied, but MS Word outputs them as if it was not applied and we have to do the same.
        /// </summary>
        IgnoreHyperlinkCharStyle = 0x0004,

        /// <summary>
        /// This is used to access revised properties of attributes. Code shall check if format revision is available
        /// and return corresponding attributes instead.
        /// </summary>
        Revised = 0x0008,

        /// <summary>
        /// Returns "raw" i.e not accepted format revision properties. 
        /// Used only during import for PositiveDifference calculation.
        /// </summary>
        AfterChanges = 0x0010,

        /// <summary>
        /// When set, all global defaults are expanded.
        /// </summary>
        GlobalDefaults = 0x0020,
        
        /// <summary>
        /// When set, table style is not expanded.
        /// </summary>
        NoTableStyle = 0x0040,

        /// <summary>
        /// Expand only inherited attributes.
        /// </summary>
        NoDirectFormatting = 0x0080,

        /// <summary>
        /// When set, directly defined paragraph style is skipped.
        /// </summary>
        NoParaStyle = 0x0100,

        /// <summary>
        /// When set, a text size inside Comment will not be changed automatically to a size defined in BalloonText style.
        /// </summary>
        NoChangeCommentSize = 0x0200,

        /// <summary>
        /// This combination is normally used for layout purposes. It includes document defaults and theme fonts.
        /// </summary>
        Layout = DocumentDefaults
    }
}
