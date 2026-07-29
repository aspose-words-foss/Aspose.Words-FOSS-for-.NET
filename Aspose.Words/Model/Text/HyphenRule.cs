// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Hyphenation rule for text runs.
    /// RK It seems this is not available in DOCX.
    /// </summary>
    internal enum HyphenRule
    {
        None = 0,
        Normal = 1,
        AddBefore = 2,
        ChangeBefore = 3,
        DeleteBefore = 4,
        ChangeAfter = 5,
        DeleteAndChange = 6
    }
}
