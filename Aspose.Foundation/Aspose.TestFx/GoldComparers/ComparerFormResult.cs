// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2010 by Roman Korchagin

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// An exit code returned from the comparer form external process.
    /// </summary>
    public enum ComparerFormResult
    {
        /// <summary>
        /// Invalid value. To satisfy FxCop.
        /// </summary>
        None = 0,
        /// <summary>
        /// The user has pressed the Accept button.
        /// The new gold needs to be accepted.
        /// </summary>
        Accept = 101,
        /// <summary>
        /// The user has pressed the Skip button or canceled the dialog box.
        /// The difference exception needs to be thrown to indicate the test has failed.
        /// </summary>
        SkipAndThrow = 102,
        /// <summary>
        /// The user has pressed the Ignore button.
        /// The difference exception should not be thrown, but a warning needs to be logged.
        /// </summary>
        SkipAndLog = 103,
        UnknownError = 999
    }
}
