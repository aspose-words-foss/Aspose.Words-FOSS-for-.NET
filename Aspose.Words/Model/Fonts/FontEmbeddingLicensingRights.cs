// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2024 by Konstantin Kornilov

using Aspose.Fonts.TrueType;
using Aspose.Words.Saving;

namespace Aspose.Words.Fonts
{
    /// <summary>
    /// Represents embedding licensing rights for the font.
    /// </summary>
    /// <remarks>
    /// To learn more, visit the
    /// <a href="https://learn.microsoft.com/en-us/typography/opentype/spec/os2#fstype">OpenType specification section</a>
    /// on Microsoft Typography portal.
    /// </remarks>
    public class FontEmbeddingLicensingRights
    {
        internal FontEmbeddingLicensingRights(
            FontEmbeddingUsagePermissions embeddingUsagePermissions,
            bool noSubsetting,
            bool bitmapEmbeddingOnly)
        {
            EmbeddingUsagePermissions = embeddingUsagePermissions;
            NoSubsetting = noSubsetting;
            BitmapEmbeddingOnly = bitmapEmbeddingOnly;
        }

        internal static FontEmbeddingLicensingRights FromFsType(FontFsType fsType)
        {
            return new FontEmbeddingLicensingRights(
                EnumConverter.FontEmbeddingUsagePermissionsFromCore(fsType.Permissions),
                fsType.NoSubsetting,
                fsType.BitmapEmbeddingOnly);
        }

        /// <summary>
        /// Usage permissions.
        /// </summary>
        public FontEmbeddingUsagePermissions EmbeddingUsagePermissions { get; }

        /// <summary>
        /// Indicates the "No subsetting" restriction.
        /// </summary>
        /// <remarks>
        /// When this flag is set, the font must not be subsetted prior to embedding. Other embedding restrictions
        /// also apply.
        /// </remarks>
        public bool NoSubsetting { get; }

        /// <summary>
        /// Indicates the "Bitmap embedding only" restriction.
        /// </summary>
        /// <remarks>
        /// When this bit is set, only bitmaps contained in the font may be embedded. No outline data may be embedded.
        /// If there are no bitmaps available in the font, then the font is considered unembeddable and the embedding
        /// services will fail. Other embedding restrictions also apply.
        /// </remarks>
        public bool BitmapEmbeddingOnly { get; }
    }
}
