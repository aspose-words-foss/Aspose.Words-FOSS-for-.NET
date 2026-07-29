// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2006 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies shading texture.
    /// </summary>
    public enum TextureIndex
    {
        /// <summary>
        /// </summary>
        Texture10Percent = 3,
        /// <summary>
        /// </summary>
        Texture12Pt5Percent = 37,
        /// <summary>
        /// </summary>
        Texture15Percent = 38,
        /// <summary>
        /// </summary>
        Texture17Pt5Percent = 39,
        /// <summary>
        /// </summary>
        Texture20Percent = 4,
        /// <summary>
        /// </summary>
        Texture22Pt5Percent = 40,
        /// <summary>
        /// </summary>
        Texture25Percent = 5,
        /// <summary>
        /// </summary>
        Texture27Pt5Percent = 41,
        /// <summary>
        /// </summary>
        Texture2Pt5Percent = 35,
        /// <summary>
        /// </summary>
        Texture30Percent = 6,
        /// <summary>
        /// </summary>
        Texture32Pt5Percent = 42,
        /// <summary>
        /// </summary>
        Texture35Percent = 43,
        /// <summary>
        /// </summary>
        Texture37Pt5Percent = 44,
        /// <summary>
        /// </summary>
        Texture40Percent = 7,
        /// <summary>
        /// </summary>
        Texture42Pt5Percent = 45,
        /// <summary>
        /// </summary>
        Texture45Percent = 46,
        /// <summary>
        /// </summary>
        Texture47Pt5Percent = 47,
        /// <summary>
        /// </summary>
        Texture50Percent = 8,
        /// <summary>
        /// </summary>
        Texture52Pt5Percent = 48,
        /// <summary>
        /// </summary>
        Texture55Percent = 49,
        /// <summary>
        /// </summary>
        Texture57Pt5Percent = 50,
        /// <summary>
        /// </summary>
        Texture5Percent = 2,
        /// <summary>
        /// </summary>
        Texture60Percent = 9,
        /// <summary>
        /// </summary>
        Texture62Pt5Percent = 51,
        /// <summary>
        /// </summary>
        Texture65Percent = 52,
        /// <summary>
        /// </summary>
        Texture67Pt5Percent = 53,
        /// <summary>
        /// </summary>
        Texture70Percent = 10,
        /// <summary>
        /// </summary>
        Texture72Pt5Percent = 54,
        /// <summary>
        /// </summary>
        Texture75Percent = 11,
        /// <summary>
        /// </summary>
        Texture77Pt5Percent = 55,
        /// <summary>
        /// </summary>
        Texture7Pt5Percent = 36,
        /// <summary>
        /// </summary>
        Texture80Percent = 12,
        /// <summary>
        /// </summary>
        Texture82Pt5Percent = 56,
        /// <summary>
        /// </summary>
        Texture85Percent = 57,
        /// <summary>
        /// </summary>
        Texture87Pt5Percent = 58,
        /// <summary>
        /// </summary>
        Texture90Percent = 13,
        /// <summary>
        /// </summary>
        Texture92Pt5Percent = 59,
        /// <summary>
        /// </summary>
        Texture95Percent = 60,
        /// <summary>
        /// </summary>
        Texture97Pt5Percent = 61,
        /// <summary>
        /// </summary>
        TextureCross = 24,
        /// <summary>
        /// </summary>
        TextureDarkCross = 18,
        /// <summary>
        /// </summary>
        TextureDarkDiagonalCross = 19,
        /// <summary>
        /// </summary>
        TextureDarkDiagonalDown = 16,
        /// <summary>
        /// </summary>
        TextureDarkDiagonalUp = 17,
        /// <summary>
        /// </summary>
        TextureDarkHorizontal = 14,
        /// <summary>
        /// </summary>
        TextureDarkVertical = 15,
        /// <summary>
        /// </summary>
        TextureDiagonalCross = 25,
        /// <summary>
        /// </summary>
        TextureDiagonalDown = 22,
        /// <summary>
        /// </summary>
        TextureDiagonalUp = 23,
        /// <summary>
        /// </summary>
        TextureHorizontal = 20,
        /// <summary>
        /// </summary>
        TextureNone = 0,
        /// <summary>
        /// </summary>
        TextureSolid = 1,
        /// <summary>
        /// </summary>
        TextureVertical = 21,
        /// <summary>
        /// Specifies that there shall be no pattern used on the current shaded region 
        /// (i.e. the pattern shall be a complete fill with the background color).
        /// </summary>
        /// <dev>
        /// Actually the Word does not renders shading with such pattern type i.e. shading is invisible (WORDSNET-17402).
        /// </dev>
        TextureNil = 65535,
    }
}
