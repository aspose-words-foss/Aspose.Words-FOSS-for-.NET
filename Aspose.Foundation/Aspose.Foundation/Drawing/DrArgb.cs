// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Michael Morozoff

namespace Aspose.Drawing
{
    /// <summary>
    /// Represents Color as uint value (combination of alpha, red, greed and blue components).
    /// Values taken from Reflector.
    /// 
    /// Why do we need this class here? It is required by <see cref="DrColor"/>, see comments there.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class DrArgb
    {
        public const int Empty = 0x00000000;
        public const int AliceBlue = unchecked((int)0xfff0f8ff);
        public const int AntiqueWhite = unchecked((int)0xfffaebd7);
        public const int Aqua = unchecked((int)0xff00ffff);
        public const int Aquamarine = unchecked((int)0xff7fffd4);
        public const int Azure = unchecked((int)0xfff0ffff);
        public const int Beige = unchecked((int)0xfff5f5dc);
        public const int Bisque = unchecked((int)0xffffe4c4);
        public const int Black = unchecked((int)0xff000000);
        public const int BlanchedAlmond = unchecked((int)0xffffebcd);
        public const int Blue = unchecked((int)0xff0000ff);
        public const int BlueViolet = unchecked((int)0xff8a2be2);
        public const int Brown = unchecked((int)0xffa52a2a);
        public const int BurlyWood = unchecked((int)0xffdeb887);
        public const int CadetBlue = unchecked((int)0xff5f9ea0);
        public const int Chartreuse = unchecked((int)0xff7fff00);
        public const int Chocolate = unchecked((int)0xffd2691e);
        public const int Coral = unchecked((int)0xffff7f50);
        public const int CornflowerBlue = unchecked((int)0xff6495ed);
        public const int Cornsilk = unchecked((int)0xfffff8dc);
        public const int Crimson = unchecked((int)0xffdc143c);
        public const int Cyan = unchecked((int)0xff00ffff);
        public const int DarkBlue = unchecked((int)0xff00008b);
        public const int DarkCyan = unchecked((int)0xff008b8b);
        public const int DarkGoldenrod = unchecked((int)0xffb8860b);
        public const int DarkGray = unchecked((int)0xffa9a9a9);
        public const int DarkGreen = unchecked((int)0xff006400);
        public const int DarkKhaki = unchecked((int)0xffbdb76b);
        public const int DarkMagenta = unchecked((int)0xff8b008b);
        public const int DarkOliveGreen = unchecked((int)0xff556b2f);
        public const int DarkOrange = unchecked((int)0xffff8c00);
        public const int DarkOrchid = unchecked((int)0xff9932cc);
        public const int DarkRed = unchecked((int)0xff8b0000);
        public const int DarkSalmon = unchecked((int)0xffe9967a);
        public const int DarkSeaGreen = unchecked((int)0xff8fbc8f);
        public const int DarkSlateBlue = unchecked((int)0xff483d8b);
        public const int DarkSlateGray = unchecked((int)0xff2f4f4f);
        public const int DarkTurquoise = unchecked((int)0xff00ced1);
        public const int DarkViolet = unchecked((int)0xff9400d3);
        public const int DeepPink = unchecked((int)0xffff1493);
        public const int DeepSkyBlue = unchecked((int)0xff00bfff);
        public const int DimGray = unchecked((int)0xff696969);
        public const int DodgerBlue = unchecked((int)0xff1e90ff);
        public const int Firebrick = unchecked((int)0xffb22222);
        public const int FloralWhite = unchecked((int)0xfffffaf0);
        public const int ForestGreen = unchecked((int)0xff228b22);
        public const int Fuchsia = unchecked((int)0xffff00ff);
        public const int Gainsboro = unchecked((int)0xffdcdcdc);
        public const int GhostWhite = unchecked((int)0xfff8f8ff);
        public const int Gold = unchecked((int)0xffffd700);
        public const int Goldenrod = unchecked((int)0xffdaa520);
        public const int Gray = unchecked((int)0xff808080);
        public const int Green = unchecked((int)0xff008000);
        public const int GreenYellow = unchecked((int)0xffadff2f);
        public const int Honeydew = unchecked((int)0xfff0fff0);
        public const int HotPink = unchecked((int)0xffff69b4);
        public const int IndianRed = unchecked((int)0xffcd5c5c);
        public const int Indigo = unchecked((int)0xff4b0082);
        public const int Ivory = unchecked((int)0xfffffff0);
        public const int Khaki = unchecked((int)0xfff0e68c);
        public const int Lavender = unchecked((int)0xffe6e6fa);
        public const int LavenderBlush = unchecked((int)0xfffff0f5);
        public const int LawnGreen = unchecked((int)0xff7cfc00);
        public const int LemonChiffon = unchecked((int)0xfffffacd);
        public const int LightBlue = unchecked((int)0xffadd8e6);
        public const int LightCoral = unchecked((int)0xfff08080);
        public const int LightCyan = unchecked((int)0xffe0ffff);
        public const int LightGoldenrodYellow = unchecked((int)0xfffafad2);
        public const int LightGray = unchecked((int)0xffd3d3d3);
        public const int LightGreen = unchecked((int)0xff90ee90);
        public const int LightPink = unchecked((int)0xffffb6c1);
        public const int LightSalmon = unchecked((int)0xffffa07a);
        public const int LightSeaGreen = unchecked((int)0xff20b2aa);
        public const int LightSkyBlue = unchecked((int)0xff87cefa);
        public const int LightSlateGray = unchecked((int)0xff778899);
        public const int LightSteelBlue = unchecked((int)0xffb0c4de);
        public const int LightYellow = unchecked((int)0xffffffe0);
        public const int Lime = unchecked((int)0xff00ff00);
        public const int LimeGreen = unchecked((int)0xff32cd32);
        public const int Linen = unchecked((int)0xfffaf0e6);
        public const int Magenta = unchecked((int)0xffff00ff);
        public const int Maroon = unchecked((int)0xff800000);
        public const int MediumAquamarine = unchecked((int)0xff66cdaa);
        public const int MediumBlue = unchecked((int)0xff0000cd);
        public const int MediumOrchid = unchecked((int)0xffba55d3);
        public const int MediumPurple = unchecked((int)0xff9370db);
        public const int MediumSeaGreen = unchecked((int)0xff3cb371);
        public const int MediumSlateBlue = unchecked((int)0xff7b68ee);
        public const int MediumSpringGreen = unchecked((int)0xff00fa9a);
        public const int MediumTurquoise = unchecked((int)0xff48d1cc);
        public const int MediumVioletRed = unchecked((int)0xffc71585);
        public const int MidnightBlue = unchecked((int)0xff191970);
        public const int MintCream = unchecked((int)0xfff5fffa);
        public const int MistyRose = unchecked((int)0xffffe4e1);
        public const int Moccasin = unchecked((int)0xffffe4b5);
        public const int NavajoWhite = unchecked((int)0xffffdead);
        public const int Navy = unchecked((int)0xff000080);
        public const int OldLace = unchecked((int)0xfffdf5e6);
        public const int Olive = unchecked((int)0xff808000);
        public const int OliveDrab = unchecked((int)0xff6b8e23);
        public const int Orange = unchecked((int)0xffffa500);
        public const int OrangeRed = unchecked((int)0xffff4500);
        public const int Orchid = unchecked((int)0xffda70d6);
        public const int PaleGoldenrod = unchecked((int)0xffeee8aa);
        public const int PaleGreen = unchecked((int)0xff98fb98);
        public const int PaleTurquoise = unchecked((int)0xffafeeee);
        public const int PaleVioletRed = unchecked((int)0xffdb7093);
        public const int PapayaWhip = unchecked((int)0xffffefd5);
        public const int PeachPuff = unchecked((int)0xffffdab9);
        public const int Peru = unchecked((int)0xffcd853f);
        public const int Pink = unchecked((int)0xffffc0cb);
        public const int Plum = unchecked((int)0xffdda0dd);
        public const int PowderBlue = unchecked((int)0xffb0e0e6);
        public const int Purple = unchecked((int)0xff800080);
        public const int Red = unchecked((int)0xffff0000);
        public const int RosyBrown = unchecked((int)0xffbc8f8f);
        public const int RoyalBlue = unchecked((int)0xff4169e1);
        public const int SaddleBrown = unchecked((int)0xff8b4513);
        public const int Salmon = unchecked((int)0xfffa8072);
        public const int SandyBrown = unchecked((int)0xfff4a460);
        public const int SeaGreen = unchecked((int)0xff2e8b57);
        public const int SeaShell = unchecked((int)0xfffff5ee);
        public const int Sienna = unchecked((int)0xffa0522d);
        public const int Silver = unchecked((int)0xffc0c0c0);
        public const int SkyBlue = unchecked((int)0xff87ceeb);
        public const int SlateBlue = unchecked((int)0xff6a5acd);
        public const int SlateGray = unchecked((int)0xff708090);
        public const int Snow = unchecked((int)0xfffffafa);
        public const int SpringGreen = unchecked((int)0xff00ff7f);
        public const int SteelBlue = unchecked((int)0xff4682b4);
        public const int Tan = unchecked((int)0xffd2b48c);
        public const int Teal = unchecked((int)0xff008080);
        public const int Thistle = unchecked((int)0xffd8bfd8);
        public const int Tomato = unchecked((int)0xffff6347);
        public const int Transparent = unchecked((int)0x00ffffff);
        public const int Turquoise = unchecked((int)0xff40e0d0);
        public const int Violet = unchecked((int)0xffee82ee);
        public const int Wheat = unchecked((int)0xfff5deb3);
        public const int White = unchecked((int)0xffffffff);
        public const int WhiteSmoke = unchecked((int)0xfff5f5f5);
        public const int Yellow = unchecked((int)0xffffff00);
        public const int YellowGreen = unchecked((int)0xff9acd32);
    }
}
