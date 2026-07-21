// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/08/2005 by Roman Korchagin

namespace Aspose.Words.Replacing
{
    /// <summary>
    /// Allows the user to specify what happens to the current match during a replace operation.
    /// </summary>
    /// <seealso cref="IReplacingCallback"/>
    /// <seealso cref="Range"/>
    /// <seealso cref="Range.Replace(string, string, FindReplaceOptions)"/>
    public enum ReplaceAction
    {
        /// <summary>
        /// Replace the current match.
        /// </summary>
        Replace,
        /// <summary>
        /// Skip the current match.
        /// </summary>
        Skip,
        /// <summary>
        /// Terminate the replace operation.
        /// </summary>
        Stop
    }
}
