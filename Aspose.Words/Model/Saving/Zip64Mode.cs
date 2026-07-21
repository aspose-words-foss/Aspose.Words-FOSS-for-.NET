// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/11/2023 by Edward Voronov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies when to use ZIP64 format extensions for OOXML files.
    /// </summary>
    /// <remarks>
    /// OOXML file is a ZIP-archive that has a 4 GB (2^32 bytes) limit on uncompressed size of a file,
    /// compressed size of a file, and total size of the archive, as well as a limit of 65,535 (2^16-1) files in archive.
    /// ZIP64 format extensions increase the limits to 2^64.
    /// </remarks>
    /// <seealso cref="OoxmlSaveOptions.Zip64Mode"/>
    public enum Zip64Mode
    {
        /// <summary>
        /// Do not use ZIP64 format extensions.
        /// </summary>
        Never = 0,

        /// <summary>
        /// If necessary use ZIP64 format extensions.
        /// </summary>
        IfNecessary = 1,

        /// <summary>
        /// Always use ZIP64 format extensions.
        /// </summary>
        Always = 2
    }
}
