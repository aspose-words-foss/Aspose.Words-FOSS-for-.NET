// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Determines when automatic line numbering restarts.
    /// </summary>
    /// <seealso cref="PageSetup"/>
    /// <seealso cref="PageSetup.LineNumberRestartMode"/>
    public enum LineNumberRestartMode
    {
        /// <summary>
        /// Line numbering restarts at the start of every page.
        /// </summary>
        RestartPage = 0,
        /// <summary>
        /// Line numbering restarts at the section start.
        /// </summary>
        RestartSection = 1,
        /// <summary>
        /// Line numbering continuous from the previous section.
        /// </summary>
        Continuous = 2
    }
}
