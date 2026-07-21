// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/11/2021 by Konstantin Kornilov

namespace Aspose.Fonts
{
    /// <summary>
    /// Container for font data used in <see cref="FontSearchInfoCache"/>.
    /// </summary>
    internal class FontSearchDataContainer
    {
        public FontSearchDataContainer(IFontData fontData, int priority)
        {
            FontData = fontData;
            SourcePriority = priority;
        }

        public IFontData FontData { get; }
        public int SourcePriority { get; }
    }
}
