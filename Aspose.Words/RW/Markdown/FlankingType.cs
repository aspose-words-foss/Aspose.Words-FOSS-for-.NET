// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Enumerates all possible flanking types of the <see cref="Delimiter"/> object.
    /// </summary>
    /// <remarks>
    /// See an algorithm for parsing nested emphasis and links at https://spec.commonmark.org
    /// </remarks>
    internal enum FlankingType
    {
        None,
        Left,
        Right,
        Both
    }
}
