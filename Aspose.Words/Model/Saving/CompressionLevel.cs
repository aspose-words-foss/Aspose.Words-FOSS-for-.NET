// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2020 by Mikhail Nepreteamov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Compression level for OOXML files. 
    /// <para>
    /// (DOCX and DOTX files are internally a ZIP-archive, this property controls the compression level of the archive.
    /// </para>
    /// <para>
    /// Note, that FlatOpc file is not a ZIP-archive, therefore, this property does not affect the FlatOpc files.)
    /// </para>
    /// </summary>
    public enum CompressionLevel
    {
        /// <summary>
        /// Normal compression level. Default compression level used by Aspose.Words.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Maximum compression level.
        /// </summary>
        Maximum = 1,

        /// <summary>
        /// Fast compression level.
        /// </summary>
        Fast = 2,

        /// <summary>
        /// Super Fast compression level. Microsoft Word uses this compression level.
        /// </summary>
        SuperFast = 3
    }
}
