// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Common
{
    /// <summary>
    /// Options for using ZIP64 extensions when saving zip archives. 
    /// </summary>
    public enum Zip64Option
    {
        /// <summary>
        /// The default behavior, which is "Never".
        /// (For COM clients, this is a 0 (zero).)
        /// </summary>
        Default = 0,
        /// <summary>
        /// Do not use ZIP64 extensions when writing zip archives.
        /// (For COM clients, this is a 0 (zero).)
        /// </summary>
        Never = 0,
        /// <summary>
        /// Use ZIP64 extensions when writing zip archives, as necessary. 
        /// For example, when a single entry exceeds 0xFFFFFFFF in size, or when the archive as a whole 
        /// exceeds 0xFFFFFFFF in size, or when there are more than 65535 entries in an archive.
        /// (For COM clients, this is a 1.)
        /// </summary>
        AsNecessary = 1,
        /// <summary>
        /// Always use ZIP64 extensions when writing zip archives, even when unnecessary.
        /// (For COM clients, this is a 2.)
        /// </summary>
        Always = 2
    }
}
