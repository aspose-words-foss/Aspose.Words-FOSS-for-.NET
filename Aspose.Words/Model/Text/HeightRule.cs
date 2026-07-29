// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the rule for determining the height of an object.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum HeightRule
    {
        /// <summary>
        /// The height will be at least the specified height in points. It will grow, if needed,
        /// to accommodate all text inside an object.
        /// </summary>
        AtLeast,
        /// <summary>
        /// The height is specified exactly in points. Please note that if the text cannot
        /// fit inside the object of this height, it will appear truncated.
        /// </summary>
        Exactly,
        /// <summary>
        /// The height will grow automatically to accommodate all text inside an object.
        /// </summary>
        Auto
    }
}
