// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2024 by Konstantin Kornilov

namespace Aspose.Fonts.Ttc
{
    /// <summary>
    /// Physical font data for a font in TrueType Collection.
    /// </summary>
    public class TtcPhysicalFontData : PhysicalFontData
    {
        public TtcPhysicalFontData(IFontData fileData, int ttcIndex, string fullFontName)
            : base(fileData)
        {
            TtcIndex = ttcIndex;
            FullFontName = fullFontName;
        }

        public int TtcIndex { get; }

        public bool TtcIndexPresent
        {
            get { return TtcIndex >= 0; }
        }

        public string FullFontName { get; }

        public override bool IsTtc
        {
            get { return true; }
        }
    }
}
