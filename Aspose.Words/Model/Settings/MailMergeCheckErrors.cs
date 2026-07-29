// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/10/2009 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Specifies how Microsoft Word will report errors detected during mail merge.
    /// </summary>
    /// <seealso cref="MailMergeSettings.CheckErrors"/>
    [SuppressMessage("Microsoft.Naming", "CA1717:OnlyFlagsEnumsShouldHavePluralNames",
        Justification = "Public API, as designed.")]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "Public API, as designed.")]
    public enum MailMergeCheckErrors
    {
        /// <summary>
        /// Simulate the merge and report errors in a new document.
        /// </summary>
        Simulate = 1,
        /// <summary>
        /// Complete the merge and pause to report errors.
        /// </summary>
        PauseOnError = 2,
        /// <summary>
        /// Complete the merge and report errors in a new document.
        /// </summary>
        CollectErrors = 3,

        /// <summary>
        /// Equals to the <see cref="PauseOnError"/> value.
        /// </summary>
        Default = PauseOnError
    }
}
