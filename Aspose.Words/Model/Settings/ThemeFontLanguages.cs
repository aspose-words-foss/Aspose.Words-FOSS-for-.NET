// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/07/2010 by Roman Korchagin

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Corresponds to 17.15.1.88 themeFontLang (Theme Font Languages).
    /// </summary>
    internal class ThemeFontLanguages
    {
        internal ThemeFontLanguages Clone()
        {
            return (ThemeFontLanguages)MemberwiseClone();
        }

        internal Language Latin = Language.NoProof;
        internal Language EastAsia = Language.NoProof;
        internal Language Bidi = Language.NoProof;
    }
}
