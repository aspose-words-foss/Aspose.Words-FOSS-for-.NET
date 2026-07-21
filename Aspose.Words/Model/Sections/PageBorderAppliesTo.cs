// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies which pages the page border is printed on.
    /// </summary>
    /// <seealso cref="PageSetup"/>
    /// <seealso cref="PageSetup.BorderAppliesTo"/>
    public enum PageBorderAppliesTo
    {
        /// <summary>
        /// Page border is shown on all pages of the section.
        /// </summary>
        AllPages = 0,
        /// <summary>
        /// Page border is shown on the first page of the section only.
        /// </summary>
        FirstPage = 1,
        /// <summary>
        /// Page border is shown on all pages except the first page of the section.
        /// </summary>
        OtherPages = 2
    }
}
