// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Victor Sotnikov

using System.Collections.Generic;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents font substitution strategy.
    /// </summary>
    public interface IFontSubstitutionStrategy
    {
        /// <summary>
        /// Tries to find font substitution.
        /// </summary>
        /// <param name="fontInfo">Missing font info to be substituted.</param>
        /// <param name="availableFonts">Array of fonts where to locate substitution.</param>
        /// <returns>Substitution font family name. May be null.</returns>
        string GetSubstitution(FontSubstitutionInfo fontInfo, IEnumerable<FontSearchInfo> availableFonts);
    }
}
