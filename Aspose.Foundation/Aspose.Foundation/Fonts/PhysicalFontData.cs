// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2024 by Konstantin Kornilov

namespace Aspose.Fonts
{
    /// <summary>
    /// Base class for typed TrueType/OpenType physical font data.
    /// </summary>
    public class PhysicalFontData
    {
        public PhysicalFontData(IFontData fileData)
        {
            FileData = fileData;
        }

        public IFontData FileData { get; }

        public virtual bool IsTtc
        {
            get { return false; }
        }
    }
}
