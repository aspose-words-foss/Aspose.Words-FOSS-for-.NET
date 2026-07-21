// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2010 by Roman Korchagin

using System.Collections.Generic;
using System.Reflection;
using Aspose.Common;

namespace Aspose.Drawing
{
    /// <summary>
    /// Use this class instead of GDI+ Color to make code autoportable to Java.
    /// 
    /// This is a by-reference object with by-value semantics. It is immutable and can be equality compared.
    /// 
    /// Why do we have this class instead of implementing ms.System.Drawing.Color?
    /// 1. .NET Color is too convoluted and awkward to use. For example, it is error prone to compare (have to convert to argb first).
    /// 2. Even if we decided to implement .NET Color in Java, it would be more convoluted than we need because it uses enum.ToString etc.
    /// So it seems best to just implement our own simple color class that is automatically portable.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public class DrColor
    {
        /// <summary>
        /// Normally such method should not be on an abstraction class, but in this case it is autoportable.
        /// </summary>
        public System.Drawing.Color ToNativeColor()
        {
            return ColorPal.ToNativeColor(this);
        }

        /// <summary>
        /// Converts color returned as attribute of document model into color which can be rendered correctly.
        /// </summary>
        public DrColor ToColorFixAlpha()
        {
            return (IsEmpty)
                ? this
                : new DrColor(R, G, B);
        }

        /// <summary>
        /// See https://www.w3.org/TR/AERT/#color-contrast for more info.
        /// </summary>
        public float GetLuminance()
        {
            return (float)((299 * R) + (587 * G) + (114 * B)) / 255000f; // Casting for Java.
        }

        /// <summary>
        /// Normally such method should not be on an abstraction class, but in this case it is autoportable.
        /// </summary>
        public static DrColor FromNativeColor(System.Drawing.Color color)
        {
            return ColorPal.FromNativeColor(color);
        }

        /// <summary>
        /// Same as calling a ctor. Kept to preserve color creation familiar to .NET developers.
        /// </summary>
        public static DrColor FromArgb(int r, int g, int b)
        {
            return new DrColor(r, g, b);
        }

        /// <summary>
        /// Same as calling a ctor. Kept to preserve color creation familiar to .NET developers.
        /// </summary>
        public static DrColor FromArgb(int a, int r, int g, int b)
        {
            return new DrColor(a, r, g, b);
        }

        /// <summary>
        /// Gets value of system color.
        /// </summary>
        /// <param name="systemColorName"></param>
        /// <returns>System color value if system colors are supported otherwise DrColor.Empty</returns>
        public static DrColor GetSystemColor(string systemColorName)
        {
            return ColorPal.GetSystemColor(systemColorName);
        }
        
        public override string ToString()
        {
            string name = FormatterPal.IntToStrX8(mArgb);

#if DEBUG && !CPLUSPLUS
            string standardName;
            if (gStandardColorNames.TryGetValue(this, out standardName))
                name = standardName;
#endif

            return string.Format("DrColor [{0}]", name);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public DrColor(int argb)
        {
            mArgb = argb;
        }

        /// <summary>
        /// Creates a Color from the specified Color, but with the new specified alpha value.
        /// Although this method allows a 32-bit value to be passed for the alpha value, the value is limited to 8 bits.
        /// </summary>
        public DrColor(int a, int r, int g, int b)
        {
            a = ((CorrectColorComponent(a) & 0xff) << ShiftA);
            r = ((CorrectColorComponent(r) & 0xff) << ShiftR);
            g = ((CorrectColorComponent(g) & 0xff) << ShiftG);
            b = ((CorrectColorComponent(b) & 0xff) << ShiftB);
            mArgb = a | r | g | b;
        }

        /// <summary>
        /// Corrects color value in accordance with color range.
        /// </summary>
        private static int CorrectColorComponent(int color)
        {
            if (color > 0xff)
                color = 0xff;
            else if (color < 0x0)
                color = 0x0;

            return color;
        }

        /// <summary>
        /// Creates a color with the specified alpha.
        /// </summary>
        public DrColor(int newAlpha, DrColor color) :
            this(newAlpha, color.R, color.G, color.B)
        {
        }

        /// <summary>
        /// Creates a color with 0xff alpha.
        /// </summary>
        public DrColor(int r, int g, int b) :
            this(0xff, r, g, b)
        {
        }

        public int A
        {
            get { return ((mArgb >> ShiftA) & 0xff); }
        }

        public int R
        {
            get { return ((mArgb >> ShiftR) & 0xff); }
        }

        public int G
        {
            get { return ((mArgb >> ShiftG) & 0xff); }
        }

        public int B
        {
            get { return ((mArgb >> ShiftB) & 0xff); }
        }

        public bool IsEmpty
        {
            get { return mArgb == DrArgb.Empty; }
        }

        public int ToArgb()
        {
            return mArgb;
        }

        /// <summary>
        /// Returns true if color represents the level of gray i.e. R = G = B.
        /// </summary>
        public bool IsGray
        {
            get { return ((R == G) && (G == B)); }
        }

        /// <summary>
        /// Overridden because equals is overridden.
        /// </summary>
        public override int GetHashCode()
        {
            return mArgb;
        }

        public override bool Equals(object obj)
        {
            return (obj is DrColor) && (Equals(this, (DrColor) obj));
        }

        /// <summary>
        /// The actual implementation.
        /// </summary>
        public static bool Equals(DrColor a, DrColor b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (ReferenceEquals(null, a))
                return false;

            if (ReferenceEquals(null, b))
                return false;

            return (a.mArgb == b.mArgb);
        }

        public static bool operator ==(DrColor a, DrColor b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(DrColor a, DrColor b)
        {
            return !Equals(a, b);
        }

        /// <summary>
        /// Index of red component.
        /// </summary>
        public const short RIndex = 2;

        /// <summary>
        /// Index of green component.
        /// </summary>
        public const short GIndex = 1;

        /// <summary>
        /// Index of blue component.
        /// </summary>
        public const short BIndex = 0;

        /// <summary>
        /// Index of alpha component for ARGB images.
        /// </summary>
        public const short AIndex = 3;

        // 0
        public static readonly DrColor AliceBlue;
        public static readonly DrColor AntiqueWhite;
        public static readonly DrColor Aqua;
        public static readonly DrColor Aquamarine;
        public static readonly DrColor Azure;
        public static readonly DrColor Beige;
        public static readonly DrColor Bisque;
        public static readonly DrColor Black;
        public static readonly DrColor BlanchedAlmond;
        public static readonly DrColor Blue;
        // 10
        public static readonly DrColor BlueViolet;
        public static readonly DrColor Brown;
        public static readonly DrColor BurlyWood;
        public static readonly DrColor CadetBlue;
        public static readonly DrColor Chartreuse;
        public static readonly DrColor Chocolate;
        public static readonly DrColor Coral;
        public static readonly DrColor CornflowerBlue;
        public static readonly DrColor Cornsilk;
        public static readonly DrColor Crimson;
        // 20
        public static readonly DrColor Cyan;
        public static readonly DrColor DarkBlue;
        public static readonly DrColor DarkCyan;
        public static readonly DrColor DarkGoldenrod;
        public static readonly DrColor DarkGray;
        public static readonly DrColor DarkGreen;
        public static readonly DrColor DarkKhaki;
        public static readonly DrColor DarkMagenta;
        public static readonly DrColor DarkOliveGreen;
        public static readonly DrColor DarkOrange;
        // 30
        public static readonly DrColor DarkOrchid;
        public static readonly DrColor DarkRed;
        public static readonly DrColor DarkSalmon;
        public static readonly DrColor DarkSeaGreen;
        public static readonly DrColor DarkSlateBlue;
        public static readonly DrColor DarkSlateGray;
        public static readonly DrColor DarkTurquoise;
        public static readonly DrColor DarkViolet;
        public static readonly DrColor DeepPink;
        public static readonly DrColor DeepSkyBlue;
        // 40
        public static readonly DrColor DimGray;
        public static readonly DrColor DodgerBlue;
        public static readonly DrColor Firebrick;
        public static readonly DrColor FloralWhite;
        public static readonly DrColor ForestGreen;
        public static readonly DrColor Fuchsia;
        public static readonly DrColor Gainsboro;
        public static readonly DrColor GhostWhite;
        public static readonly DrColor Gold;
        public static readonly DrColor Gray;
        // 50
        public static readonly DrColor Green;
        public static readonly DrColor GreenYellow;
        public static readonly DrColor Goldenrod;
        public static readonly DrColor Honeydew;
        public static readonly DrColor HotPink;
        public static readonly DrColor IndianRed;
        public static readonly DrColor Indigo;
        public static readonly DrColor Ivory;
        public static readonly DrColor Khaki;
        public static readonly DrColor Lavender;
        // 60
        public static readonly DrColor LavenderBlush;
        public static readonly DrColor LawnGreen;
        public static readonly DrColor LemonChiffon;
        public static readonly DrColor LightBlue;
        public static readonly DrColor LightCoral;
        public static readonly DrColor LightCyan;
        public static readonly DrColor LightGoldenrodYellow;
        public static readonly DrColor LightGray;
        public static readonly DrColor LightGreen;
        public static readonly DrColor LightPink;
        // 70
        public static readonly DrColor LightSalmon;
        public static readonly DrColor LightSeaGreen;
        public static readonly DrColor LightSkyBlue;
        public static readonly DrColor LightSlateGray;
        public static readonly DrColor LightSteelBlue;
        public static readonly DrColor LightYellow;
        public static readonly DrColor Lime;
        public static readonly DrColor LimeGreen;
        public static readonly DrColor Linen;
        public static readonly DrColor Magenta;
        // 80
        public static readonly DrColor Maroon;
        public static readonly DrColor MediumAquamarine;
        public static readonly DrColor MediumBlue;
        public static readonly DrColor MediumOrchid;
        public static readonly DrColor MediumPurple;
        public static readonly DrColor MediumSeaGreen;
        public static readonly DrColor MediumSlateBlue;
        public static readonly DrColor MediumSpringGreen;
        public static readonly DrColor MediumTurquoise;
        public static readonly DrColor MediumVioletRed;
        // 90
        public static readonly DrColor MidnightBlue;
        public static readonly DrColor MintCream;
        public static readonly DrColor MistyRose;
        public static readonly DrColor Moccasin;
        public static readonly DrColor NavajoWhite;
        public static readonly DrColor Navy;
        public static readonly DrColor OldLace;
        public static readonly DrColor Olive;
        public static readonly DrColor OliveDrab;
        public static readonly DrColor Orange;
        // 100
        public static readonly DrColor OrangeRed;
        public static readonly DrColor Orchid;
        public static readonly DrColor PaleGoldenrod;
        public static readonly DrColor PaleGreen;
        public static readonly DrColor PaleTurquoise;
        public static readonly DrColor PaleVioletRed;
        public static readonly DrColor PapayaWhip;
        public static readonly DrColor PeachPuff;
        public static readonly DrColor Peru;
        public static readonly DrColor Pink;
        // 110
        public static readonly DrColor Plum;
        public static readonly DrColor PowderBlue;
        public static readonly DrColor Purple;
        public static readonly DrColor Red;
        public static readonly DrColor RosyBrown;
        public static readonly DrColor RoyalBlue;
        public static readonly DrColor SaddleBrown;
        public static readonly DrColor Salmon;
        public static readonly DrColor SandyBrown;
        public static readonly DrColor SeaGreen;
        // 120
        public static readonly DrColor SeaShell;
        public static readonly DrColor Sienna;
        public static readonly DrColor Silver;
        public static readonly DrColor SkyBlue;
        public static readonly DrColor SlateBlue;
        public static readonly DrColor SlateGray;
        public static readonly DrColor Snow;
        public static readonly DrColor SpringGreen;
        public static readonly DrColor SteelBlue;
        public static readonly DrColor Tan;
        // 130
        public static readonly DrColor Teal;
        public static readonly DrColor Thistle;
        public static readonly DrColor Tomato;
        public static readonly DrColor Turquoise;
        public static readonly DrColor Violet;
        public static readonly DrColor Wheat;
        public static readonly DrColor White;
        public static readonly DrColor WhiteSmoke;
        public static readonly DrColor Yellow;
        public static readonly DrColor YellowGreen;
        // 140 colors

        // System window color code.
        public static readonly DrColor Window;

        /// <summary>
        /// Gets Transparent color. This is full transparency white color.
        ///</summary>
        public static readonly DrColor Transparent;

        /// <summary>
        /// Gets Empty color. This is full transparency black color used in the AW model.
        ///</summary>
        public static readonly DrColor Empty;

        private readonly int mArgb;

        private const int ShiftA = 24;
        private const int ShiftR = 16;
        private const int ShiftG = 8;
        private const int ShiftB = 0;

        static DrColor()
        {
            // RK These are in a static ctor to make auto porting to Java possible.
            AliceBlue = new DrColor(DrArgb.AliceBlue);
            AntiqueWhite = new DrColor(DrArgb.AntiqueWhite);
            Aqua = new DrColor(DrArgb.Aqua);
            Aquamarine = new DrColor(DrArgb.Aquamarine);
            Azure = new DrColor(DrArgb.Azure);
            Beige = new DrColor(DrArgb.Beige);
            Bisque = new DrColor(DrArgb.Bisque);
            Black = new DrColor(DrArgb.Black);
            BlanchedAlmond = new DrColor(DrArgb.BlanchedAlmond);
            Blue = new DrColor(DrArgb.Blue);

            BlueViolet = new DrColor(DrArgb.BlueViolet);
            Brown = new DrColor(DrArgb.Brown);
            BurlyWood = new DrColor(DrArgb.BurlyWood);
            CadetBlue = new DrColor(DrArgb.CadetBlue);
            Chartreuse = new DrColor(DrArgb.Chartreuse);
            Chocolate = new DrColor(DrArgb.Chocolate);
            Coral = new DrColor(DrArgb.Coral);
            CornflowerBlue = new DrColor(DrArgb.CornflowerBlue);
            Cornsilk = new DrColor(DrArgb.Cornsilk);
            Crimson = new DrColor(DrArgb.Crimson);

            Cyan = new DrColor(DrArgb.Cyan);
            DarkBlue = new DrColor(DrArgb.DarkBlue);
            DarkCyan = new DrColor(DrArgb.DarkCyan);
            DarkGoldenrod = new DrColor(DrArgb.DarkGoldenrod);
            DarkGray = new DrColor(DrArgb.DarkGray);
            DarkGreen = new DrColor(DrArgb.DarkGreen);
            DarkKhaki = new DrColor(DrArgb.DarkKhaki);
            DarkMagenta = new DrColor(DrArgb.DarkMagenta);
            DarkOliveGreen = new DrColor(DrArgb.DarkOliveGreen);
            DarkOrange = new DrColor(DrArgb.DarkOrange);

            DarkOrchid = new DrColor(DrArgb.DarkOrchid);
            DarkRed = new DrColor(DrArgb.DarkRed);
            DarkSalmon = new DrColor(DrArgb.DarkSalmon);
            DarkSeaGreen = new DrColor(DrArgb.DarkSeaGreen);
            DarkSlateBlue = new DrColor(DrArgb.DarkSlateBlue);
            DarkSlateGray = new DrColor(DrArgb.DarkSlateGray);
            DarkTurquoise = new DrColor(DrArgb.DarkTurquoise);
            DarkViolet = new DrColor(DrArgb.DarkViolet);
            DeepPink = new DrColor(DrArgb.DeepPink);
            DeepSkyBlue = new DrColor(DrArgb.DeepSkyBlue);

            DimGray = new DrColor(DrArgb.DimGray);
            DodgerBlue = new DrColor(DrArgb.DodgerBlue);
            Firebrick = new DrColor(DrArgb.Firebrick);
            FloralWhite = new DrColor(DrArgb.FloralWhite);
            ForestGreen = new DrColor(DrArgb.ForestGreen);
            Fuchsia = new DrColor(DrArgb.Fuchsia);
            Gainsboro = new DrColor(DrArgb.Gainsboro);
            GhostWhite = new DrColor(DrArgb.GhostWhite);
            Gold = new DrColor(DrArgb.Gold);
            Gray = new DrColor(DrArgb.Gray);

            Green = new DrColor(DrArgb.Green);
            GreenYellow = new DrColor(DrArgb.GreenYellow);
            Goldenrod = new DrColor(DrArgb.Goldenrod);
            Honeydew = new DrColor(DrArgb.Honeydew);
            HotPink = new DrColor(DrArgb.HotPink);
            IndianRed = new DrColor(DrArgb.IndianRed);
            Indigo = new DrColor(DrArgb.Indigo);
            Ivory = new DrColor(DrArgb.Ivory);
            Khaki = new DrColor(DrArgb.Khaki);
            Lavender = new DrColor(DrArgb.Lavender);

            LavenderBlush = new DrColor(DrArgb.LavenderBlush);
            LawnGreen = new DrColor(DrArgb.LawnGreen);
            LemonChiffon = new DrColor(DrArgb.LemonChiffon);
            LightBlue = new DrColor(DrArgb.LightBlue);
            LightCoral = new DrColor(DrArgb.LightCoral);
            LightCyan = new DrColor(DrArgb.LightCyan);
            LightGoldenrodYellow = new DrColor(DrArgb.LightGoldenrodYellow);
            LightGray = new DrColor(DrArgb.LightGray);
            LightGreen = new DrColor(DrArgb.LightGreen);
            LightPink = new DrColor(DrArgb.LightPink);

            LightSalmon = new DrColor(DrArgb.LightSalmon);
            LightSeaGreen = new DrColor(DrArgb.LightSeaGreen);
            LightSkyBlue = new DrColor(DrArgb.LightSkyBlue);
            LightSlateGray = new DrColor(DrArgb.LightSlateGray);
            LightSteelBlue = new DrColor(DrArgb.LightSteelBlue);
            LightYellow = new DrColor(DrArgb.LightYellow);
            Lime = new DrColor(DrArgb.Lime);
            LimeGreen = new DrColor(DrArgb.LimeGreen);
            Linen = new DrColor(DrArgb.Linen);
            Magenta = new DrColor(DrArgb.Magenta);

            Maroon = new DrColor(DrArgb.Maroon);
            MediumAquamarine = new DrColor(DrArgb.MediumAquamarine);
            MediumBlue = new DrColor(DrArgb.MediumBlue);
            MediumOrchid = new DrColor(DrArgb.MediumOrchid);
            MediumPurple = new DrColor(DrArgb.MediumPurple);
            MediumSeaGreen = new DrColor(DrArgb.MediumSeaGreen);
            MediumSlateBlue = new DrColor(DrArgb.MediumSlateBlue);
            MediumSpringGreen = new DrColor(DrArgb.MediumSpringGreen);
            MediumTurquoise = new DrColor(DrArgb.MediumTurquoise);
            MediumVioletRed = new DrColor(DrArgb.MediumVioletRed);

            MidnightBlue = new DrColor(DrArgb.MidnightBlue);
            MintCream = new DrColor(DrArgb.MintCream);
            MistyRose = new DrColor(DrArgb.MistyRose);
            Moccasin = new DrColor(DrArgb.Moccasin);
            NavajoWhite = new DrColor(DrArgb.NavajoWhite);
            Navy = new DrColor(DrArgb.Navy);
            OldLace = new DrColor(DrArgb.OldLace);
            Olive = new DrColor(DrArgb.Olive);
            OliveDrab = new DrColor(DrArgb.OliveDrab);
            Orange = new DrColor(DrArgb.Orange);

            OrangeRed = new DrColor(DrArgb.OrangeRed);
            Orchid = new DrColor(DrArgb.Orchid);
            PaleGoldenrod = new DrColor(DrArgb.PaleGoldenrod);
            PaleGreen = new DrColor(DrArgb.PaleGreen);
            PaleTurquoise = new DrColor(DrArgb.PaleTurquoise);
            PaleVioletRed = new DrColor(DrArgb.PaleVioletRed);
            PapayaWhip = new DrColor(DrArgb.PapayaWhip);
            PeachPuff = new DrColor(DrArgb.PeachPuff);
            Peru = new DrColor(DrArgb.Peru);
            Pink = new DrColor(DrArgb.Pink);

            Plum = new DrColor(DrArgb.Plum);
            PowderBlue = new DrColor(DrArgb.PowderBlue);
            Purple = new DrColor(DrArgb.Purple);
            Red = new DrColor(DrArgb.Red);
            RosyBrown = new DrColor(DrArgb.RosyBrown);
            RoyalBlue = new DrColor(DrArgb.RoyalBlue);
            SaddleBrown = new DrColor(DrArgb.SaddleBrown);
            Salmon = new DrColor(DrArgb.Salmon);
            SandyBrown = new DrColor(DrArgb.SandyBrown);
            SeaGreen = new DrColor(DrArgb.SeaGreen);

            SeaShell = new DrColor(DrArgb.SeaShell);
            Sienna = new DrColor(DrArgb.Sienna);
            Silver = new DrColor(DrArgb.Silver);
            SkyBlue = new DrColor(DrArgb.SkyBlue);
            SlateBlue = new DrColor(DrArgb.SlateBlue);
            SlateGray = new DrColor(DrArgb.SlateGray);
            Snow = new DrColor(DrArgb.Snow);
            SpringGreen = new DrColor(DrArgb.SpringGreen);
            SteelBlue = new DrColor(DrArgb.SteelBlue);
            Tan = new DrColor(DrArgb.Tan);

            Teal = new DrColor(DrArgb.Teal);
            Thistle = new DrColor(DrArgb.Thistle);
            Tomato = new DrColor(DrArgb.Tomato);
            Turquoise = new DrColor(DrArgb.Turquoise);
            Violet = new DrColor(DrArgb.Violet);
            Wheat = new DrColor(DrArgb.Wheat);
            White = new DrColor(DrArgb.White);
            WhiteSmoke = new DrColor(DrArgb.WhiteSmoke);
            Yellow = new DrColor(DrArgb.Yellow);
            YellowGreen = new DrColor(DrArgb.YellowGreen);

            Transparent = new DrColor(DrArgb.Transparent);
            Empty = new DrColor(DrArgb.Empty);

            // System color "Window" that should be resolved elsewhere.
            Window = new DrColor(0xef, 0x11, 0x00, 0x00);

#if DEBUG && !CPLUSPLUS
            gStandardColorNames = BuildStandardColorNames();
#endif
        }

#if DEBUG && !CPLUSPLUS
        private static readonly IDictionary<DrColor, string> gStandardColorNames;
        private static IDictionary<DrColor, string> BuildStandardColorNames()
        {
            Dictionary<DrColor, string> result = new Dictionary<DrColor, string>();

            foreach (FieldInfo field in typeof(DrColor).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (field.FieldType != typeof(DrColor))
                    continue;

                DrColor color = (DrColor)field.GetValue(null);
                if (color == null)
                    continue;

                result[color] = field.Name;
            }

            return result;
        }
#endif
    }
}
