// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/07/2007 by Roman Korchagin

namespace Aspose.Words.Tests
{
    /// <summary>
    /// Specifies whether to verify only the exporter or both the exporter and importer against a gold file.
    /// </summary>
    public enum GoldLevel
    {
        /// <summary>
        /// We usually develop exporter first. Therefore this setting is used when developing and exporter.
        /// </summary>
        ExportOnly,
        /// <summary>
        /// When both exporter and importer are ready, this setting must be used.
        /// </summary>
        ExportImport
    }
}