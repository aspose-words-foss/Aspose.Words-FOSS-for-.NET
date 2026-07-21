// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2018 by Konstantin Kornilov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how bookmarks in headers/footers are exported.
    /// </summary>
    public enum HeaderFooterBookmarksExportMode
    {
        /// <summary>
        /// Bookmarks in headers/footers are not exported.
        /// </summary>
        None = 0,
        /// <summary>
        /// Only bookmark in first header/footer of the section is exported.
        /// </summary>
        First = 1,
        /// <summary>
        /// Bookmarks in all headers/footers are exported.
        /// </summary>
        All = 2
    }
}
