// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/06/2011 by Roman Korchagin

namespace Aspose
{
    /// <summary>
    /// Specifies the operating system. This enum exists because PlatformID available on .NET is confusing and not enough for our purposes.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Any Windows OS.
        /// </summary>
        Windows,
        /// <summary>
        /// Any Unix or Linux OS.
        /// </summary>
        Unix,
        /// <summary>
        /// Mac OS X.
        /// </summary>
        MacOS,

        /// <summary>
        /// Android.
        /// </summary>
        Android,

        /// <summary>
        /// Apple iOS.
        /// </summary>
        iOS
    }
}
