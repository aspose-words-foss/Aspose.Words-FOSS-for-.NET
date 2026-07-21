// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2024 by Konstantin Kornilov

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Represents the font embedding usage permissions.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum FontEmbeddingUsagePermissions
    {
        /// <summary>
        /// The font may be embedded, and may be permanently installed for use on a remote systems, or for use by
        /// other users.
        /// </summary>
        Installable = 0,
        /// <summary>
        /// The font must not be modified, embedded or exchanged in any manner without first obtaining explicit
        /// permission of the legal owner.
        /// </summary>
        RestrictedLicense = 1,
        /// <summary>
        /// The font may be embedded, and may be temporarily loaded on other systems for purposes of viewing or
        /// printing the document.
        /// </summary>
        /// <remarks>
        /// Documents containing Preview &amp; Print fonts must be opened “read-only”; no edits may
        /// be applied to the document.
        /// </remarks>
        PrintAndPreview = 2,
        /// <summary>
        /// The font may be embedded, and may be temporarily loaded on other systems.
        /// </summary>
        /// <remarks>
        /// As with Preview &amp; Print embedding, documents containing Editable fonts may be opened for reading.
        /// In addition, editing is permitted, including ability to format new text using the embedded font, and
        /// changes may be saved.
        /// </remarks>
        Editable = 3
    }
}
