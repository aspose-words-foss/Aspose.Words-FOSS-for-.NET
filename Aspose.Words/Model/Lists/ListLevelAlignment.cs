// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Specifies alignment for the list number or bullet.
    /// </summary>
    /// <remarks>
    /// <p>Used as a value for the <see cref="ListLevel.Alignment"/> property.</p>
    /// </remarks>
    [CppEnumEnableMetadata]
    public enum ListLevelAlignment
    {
        /// <summary>
        /// The list label is aligned to the left of the number position.
        /// </summary>
        Left = 0,
        /// <summary>
        /// The list label is centered at the number position.
        /// </summary>
        Center = 1,
        /// <summary>
        /// This list label is aligned to the right of the number position.
        /// </summary>
        Right = 2
    }
}
