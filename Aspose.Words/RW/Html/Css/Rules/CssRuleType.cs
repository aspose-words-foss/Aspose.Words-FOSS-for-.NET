// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Types of rules returned by our CSS parser.
    /// </summary>
    internal enum CssRuleType
    {
        /// <summary>
        /// A style rule: selectors plus declarations.
        /// </summary>
        Style,

        /// <summary>
        /// An @import rule.
        /// </summary>
        Import,

        /// <summary>
        /// A @media rule.
        /// </summary>
        Media,

        /// <summary>
        /// A @page rule.
        /// </summary>
        Page,

        /// <summary>
        /// A @font-face rule.
        /// </summary>
        FontFace,

        /// <summary>
        /// A @list rule.
        /// </summary>
        List
    }
}
