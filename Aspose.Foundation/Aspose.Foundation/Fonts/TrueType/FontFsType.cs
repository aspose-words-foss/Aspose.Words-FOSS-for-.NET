// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2024 by Konstantin Kornilov

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents fsType field in 'OS/2' table.
    /// </summary>
    public class FontFsType
    {
        public FontFsType(ushort value)
        {
            Value = value;
            Permissions = ParsePermissions(value);
            NoSubsetting = (value & NoSubsettingMask) != 0;
            BitmapEmbeddingOnly = (value & BitmapEmbeddingOnlyMask) != 0;
        }

        private static FontFsTypePermissions ParsePermissions(ushort fsType)
        {
            // Quote from spec:
            // this version of the OS/2 table makes bits 0 - 3 a set of exclusive bits.
            // in the event that more than one of bits 0 to 3 are set in a given font, then the least-restrictive
            // permission indicated take precedence.
            if ((fsType & EditableEmbeddingMask) != 0)
                return FontFsTypePermissions.Editable;
            if ((fsType & PreviewAndPrintMask) != 0)
                return FontFsTypePermissions.PrintAndPreview;
            if ((fsType & RestrictedLicenseMask) != 0)
                return FontFsTypePermissions.RestrictedLicense;
            return FontFsTypePermissions.Installable;
        }

        public ushort Value { get; }
        public FontFsTypePermissions Permissions { get; }
        public bool NoSubsetting { get; }
        public bool BitmapEmbeddingOnly { get; }

        private const int RestrictedLicenseMask = 0x0002;
        private const int PreviewAndPrintMask = 0x0004;
        private const int EditableEmbeddingMask = 0x0008;
        private const int NoSubsettingMask = 0x0100;
        private const int BitmapEmbeddingOnlyMask = 0x0200;
    }
}
