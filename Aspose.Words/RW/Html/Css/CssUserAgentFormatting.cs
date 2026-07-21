// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/04/2014 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Groups of user agent (default) CSS styles by formatting they affect.
    /// </summary>
    [Flags]
    internal enum CssUserAgentFormatting
    {
        /// <summary>
        /// None of user agent CSS styles.
        /// </summary>
        None = 0,

        /// <summary>
        /// User agent CSS styles that affect font formatting.
        /// </summary>
        Font = 1,

        /// <summary>
        /// User agent CSS styles that affect paragraph formatting.
        /// </summary>
        Paragraph = 2
    }
}
