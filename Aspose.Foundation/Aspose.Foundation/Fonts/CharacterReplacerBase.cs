// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2012 by Konstantin Kornilov

using Aspose.Collections;

namespace Aspose.Fonts
{
    /// <summary>
    /// Base class for character replacers.
    /// </summary>
    /// <remarks>
    /// Experiments show that if MS Word fails to find the glyph for specific character in the font then it tries to replace 
    /// this character according to some rules. If one of replacement characters is found in the font then MS Word draws it 
    /// instead of original character.
    /// Implementations of this class represents specific replacement rule.
    /// </remarks>
    internal abstract class CharacterReplacerBase
    {
        /// <summary>
        /// Tries to replace the specified character.
        /// </summary>
        internal abstract void Replace(int charCode, IntList replacements);
    }
}
