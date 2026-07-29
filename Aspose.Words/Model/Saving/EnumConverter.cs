// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2011 by Alexey Titov

using System;
using Aspose.Fonts.TrueType;
using Aspose.Words.Fonts;

namespace Aspose.Words.Saving
{
    internal class EnumConverter
    {
        internal static FontEmbeddingUsagePermissions FontEmbeddingUsagePermissionsFromCore(
            FontFsTypePermissions value)
        {
            switch (value)
            {
                case FontFsTypePermissions.Editable:
                    return FontEmbeddingUsagePermissions.Editable;
                case FontFsTypePermissions.Installable:
                    return FontEmbeddingUsagePermissions.Installable;
                case FontFsTypePermissions.RestrictedLicense:
                    return FontEmbeddingUsagePermissions.RestrictedLicense;
                case FontFsTypePermissions.PrintAndPreview:
                    return FontEmbeddingUsagePermissions.PrintAndPreview;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }
}
