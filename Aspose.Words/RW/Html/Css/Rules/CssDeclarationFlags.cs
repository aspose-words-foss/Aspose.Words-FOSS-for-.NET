// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2016 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Flags that are used to mark declarations in a collection. For example, to indicate the source of each declaration.
    /// </summary>
    [Flags]
    internal enum CssDeclarationFlags
    {
        /// <summary>
        /// A declaration isn't marked with any flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// A declaration comes from our User Agent stylesheet.
        /// </summary>
        UserAgent = 1
    }
}
