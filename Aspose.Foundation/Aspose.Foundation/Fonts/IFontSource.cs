// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2019 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Fonts
{
    /// <summary>
    /// Interface for the font sources.
    /// </summary>
    /// <remarks>
    /// Internal suffix is required because there is a name conflict with public API property.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppVirtualInheritance("System.Object")]
    public interface IFontSource
    {
        /// <summary>
        /// Font source priority.
        /// </summary>
        int PriorityInternal { get; }

        /// <summary>
        /// Gets available font data.
        /// </summary>
        IEnumerable<IFontData> GetFontDataInternal();
    }
}
