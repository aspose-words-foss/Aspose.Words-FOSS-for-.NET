// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2011 by Roman Korchagin

using System;

namespace Aspose.Words
{
    [Flags]
    internal enum ParaPrExpandFlags
    {
        /// <summary>
        /// Performs the normal "plain" expansion of attributes.
        /// </summary>
        Normal = 0x0000,

        /// <summary>
        /// Set to expand default run properties stored for this document.
        /// I do not have a good explanation why we sometimes need to expand and sometimes do not.
        /// </summary>
        DocumentDefaults = 0x0001,

        /// <summary>
        /// When set, the "clear" tab stops are not included in the result.
        /// </summary>
        RemoveClearTabStops = 0x0002,

        /// <summary>
        /// Set to expand table style properties.
        /// It seems we do not need to expand table style for RTF.
        /// </summary>
        ExpandTableStyle = 0x0004,

        /// <summary>
        /// Set to expand HTML related information <see cref="HtmlBlock" /> for paragraphs and rows.
        /// It seems we do not need to expand this for paragraphs in RTF.
        /// </summary>
        ExpandHtmlBlocks = 0x0008,

        /// <summary>
        /// This is used to access revised properties of attributes. Code shall check if format revision is available
        /// and return corresponding attributes instead.
        /// </summary>
        Revised = 0x0010,

        /// <summary>
        /// Returns "raw" i.e not accepted format revision properties.
        /// Used only during import for PositiveDifference calculation.
        /// </summary>
        AfterChanges = 0x0020,

        /// <summary>
        /// When set, all global defaults are expanded.
        /// </summary>
        GlobalDefaults = 0x0040,

        /// <summary>
        /// Expand only inherited attributes.
        /// </summary>
        NoDirectFormatting = 0x0080,

        /// <summary>
        /// Expand legacy list formatting into legacy tab.
        /// </summary>
        ExpandLegacyTab = 0x0100,

        /// <summary>
        /// This combination is normally used for layout purposes. It includes document defaults and paragraph group properties.
        /// </summary>
        Layout = DocumentDefaults | ExpandHtmlBlocks | ExpandLegacyTab
    }
}
