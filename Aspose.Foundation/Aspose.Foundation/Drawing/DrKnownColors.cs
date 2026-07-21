// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2010 by Roman Korchagin

using Aspose.Collections;

namespace Aspose.Drawing
{
    /// <summary>
    /// Provides methods for converting colors names to color codes.
    /// Made into a separate class to avoid loading the hashtable unless really used.
    /// </summary>
    public static class DrKnownColors
    {
        /// <summary>
        /// Converts a case-insensitive color name to a color value. Returns <see cref="DrColor.Empty"/> if cannot find a color.
        /// </summary>
        public static DrColor FromName(string value)
        {
            DrColor color = gKnownColorImport[value];
            return (color != null) ? color : DrColor.Empty;
        }

        private static readonly StringToObjDictionary<DrColor> gKnownColorImport;

        static DrKnownColors()
        {
            gKnownColorImport = new StringToObjDictionary<DrColor>(false);

            gKnownColorImport.Add("aliceBlue", DrColor.AliceBlue);
            gKnownColorImport.Add("antiqueWhite", DrColor.AntiqueWhite);
            gKnownColorImport.Add("aqua", DrColor.Aqua);
            gKnownColorImport.Add("aquamarine", DrColor.Aquamarine);
            gKnownColorImport.Add("azure", DrColor.Azure);

            gKnownColorImport.Add("beige", DrColor.Beige);
            gKnownColorImport.Add("bisque", DrColor.Bisque);
            gKnownColorImport.Add("black", DrColor.Black);
            gKnownColorImport.Add("blanchedAlmond", DrColor.BlanchedAlmond);
            gKnownColorImport.Add("blue", DrColor.Blue);
            gKnownColorImport.Add("blueViolet", DrColor.BlueViolet);
            gKnownColorImport.Add("brown", DrColor.Brown);
            gKnownColorImport.Add("burlyWood", DrColor.BurlyWood);

            gKnownColorImport.Add("cadetBlue", DrColor.CadetBlue);
            gKnownColorImport.Add("chartreuse", DrColor.Chartreuse);
            gKnownColorImport.Add("chocolate", DrColor.Chocolate);
            gKnownColorImport.Add("coral", DrColor.Coral);
            gKnownColorImport.Add("cornflowerBlue", DrColor.CornflowerBlue);
            gKnownColorImport.Add("cornsilk", DrColor.Cornsilk);
            gKnownColorImport.Add("crimson", DrColor.Crimson);
            gKnownColorImport.Add("cyan", DrColor.Cyan);

            gKnownColorImport.Add("darkBlue", DrColor.DarkBlue);
            gKnownColorImport.Add("darkCyan", DrColor.DarkCyan);
            gKnownColorImport.Add("darkGoldenrod", DrColor.DarkGoldenrod);
            gKnownColorImport.Add("darkGray", DrColor.DarkGray);
            gKnownColorImport.Add("darkGreen", DrColor.DarkGreen);
            gKnownColorImport.Add("darkGrey", DrColor.DarkGray);
            gKnownColorImport.Add("darkKhaki", DrColor.DarkKhaki);
            gKnownColorImport.Add("darkMagenta", DrColor.DarkMagenta);
            gKnownColorImport.Add("darkOliveGreen", DrColor.DarkOliveGreen);
            gKnownColorImport.Add("darkOrange", DrColor.DarkOrange);
            gKnownColorImport.Add("darkOrchid", DrColor.DarkOrchid);
            gKnownColorImport.Add("darkRed", DrColor.DarkRed);
            gKnownColorImport.Add("darkSalmon", DrColor.DarkSalmon);
            gKnownColorImport.Add("darkSeaGreen", DrColor.DarkSeaGreen);
            gKnownColorImport.Add("darkSlateBlue", DrColor.DarkSlateBlue);
            gKnownColorImport.Add("darkSlateGray", DrColor.DarkSlateGray);
            gKnownColorImport.Add("darkSlateGrey", DrColor.DarkSlateGray);
            gKnownColorImport.Add("darkTurquoise", DrColor.DarkTurquoise);
            gKnownColorImport.Add("darkViolet", DrColor.DarkViolet);
            gKnownColorImport.Add("deepPink", DrColor.DeepPink);
            gKnownColorImport.Add("deepSkyBlue", DrColor.DeepSkyBlue);
            gKnownColorImport.Add("dimGray", DrColor.DimGray);
            gKnownColorImport.Add("dimGrey", DrColor.DimGray);
            gKnownColorImport.Add("dkBlue", DrColor.DarkBlue);
            gKnownColorImport.Add("dkCyan", DrColor.DarkCyan);
            gKnownColorImport.Add("dkGoldenrod", DrColor.DarkGoldenrod);
            gKnownColorImport.Add("dkGray", DrColor.DarkGray);
            gKnownColorImport.Add("dkGreen", DrColor.DarkGreen);
            gKnownColorImport.Add("dkGrey", DrColor.DarkGray);
            gKnownColorImport.Add("dkKhaki", DrColor.DarkKhaki);
            gKnownColorImport.Add("dkMagenta", DrColor.DarkMagenta);
            gKnownColorImport.Add("dkOliveGreen", DrColor.DarkOliveGreen);
            gKnownColorImport.Add("dkOrange", DrColor.DarkOrange);
            gKnownColorImport.Add("dkOrchid", DrColor.DarkOrchid);
            gKnownColorImport.Add("dkRed", DrColor.DarkRed);
            gKnownColorImport.Add("dkSalmon", DrColor.DarkSalmon);
            gKnownColorImport.Add("dkSeaGreen", DrColor.DarkSeaGreen);
            gKnownColorImport.Add("dkSlateBlue", DrColor.DarkSlateBlue);
            gKnownColorImport.Add("dkSlateGray", DrColor.DarkSlateGray);
            gKnownColorImport.Add("dkSlateGrey", DrColor.DarkSlateGray);
            gKnownColorImport.Add("dkTurquoise", DrColor.DarkTurquoise);
            gKnownColorImport.Add("dkViolet", DrColor.DarkViolet);
            gKnownColorImport.Add("dodgerBlue", DrColor.DodgerBlue);

            gKnownColorImport.Add("firebrick", DrColor.Firebrick);
            gKnownColorImport.Add("floralWhite", DrColor.FloralWhite);
            gKnownColorImport.Add("forestGreen", DrColor.ForestGreen);
            gKnownColorImport.Add("fuchsia", DrColor.Fuchsia);

            gKnownColorImport.Add("gainsboro", DrColor.Gainsboro);
            gKnownColorImport.Add("ghostWhite", DrColor.GhostWhite);
            gKnownColorImport.Add("gold", DrColor.Gold);
            gKnownColorImport.Add("goldenrod", DrColor.Goldenrod);
            gKnownColorImport.Add("gray", DrColor.Gray);
            gKnownColorImport.Add("green", DrColor.Green);
            gKnownColorImport.Add("greenYellow", DrColor.GreenYellow);
            gKnownColorImport.Add("grey", DrColor.Gray);

            gKnownColorImport.Add("honeydew", DrColor.Honeydew);
            gKnownColorImport.Add("hotPink", DrColor.HotPink);

            gKnownColorImport.Add("indianRed", DrColor.IndianRed);
            gKnownColorImport.Add("indigo", DrColor.Indigo);
            gKnownColorImport.Add("ivory", DrColor.Ivory);

            gKnownColorImport.Add("khaki", DrColor.Khaki);

            gKnownColorImport.Add("lavender", DrColor.Lavender);
            gKnownColorImport.Add("lavenderBlush", DrColor.LavenderBlush);
            gKnownColorImport.Add("lawnGreen", DrColor.LawnGreen);
            gKnownColorImport.Add("lemonChiffon", DrColor.LemonChiffon);
            gKnownColorImport.Add("lightBlue", DrColor.LightBlue);
            gKnownColorImport.Add("lightCoral", DrColor.LightCoral);
            gKnownColorImport.Add("lightCyan", DrColor.LightCyan);
            gKnownColorImport.Add("lightGoldenrodYellow", DrColor.LightGoldenrodYellow);
            gKnownColorImport.Add("lightGray", DrColor.LightGray);
            gKnownColorImport.Add("lightGreen", DrColor.LightGreen);
            gKnownColorImport.Add("lightGrey", DrColor.LightGray);
            gKnownColorImport.Add("lightPink", DrColor.LightPink);
            gKnownColorImport.Add("lightSalmon", DrColor.LightSalmon);
            gKnownColorImport.Add("lightSeaGreen", DrColor.LightSeaGreen);
            gKnownColorImport.Add("lightSkyBlue", DrColor.LightSkyBlue);
            gKnownColorImport.Add("lightSlateGray", DrColor.LightSlateGray);
            gKnownColorImport.Add("lightSlateGrey", DrColor.LightSlateGray);
            gKnownColorImport.Add("lightSteelBlue", DrColor.LightSteelBlue);
            gKnownColorImport.Add("lightYellow", DrColor.LightYellow);
            gKnownColorImport.Add("lime", DrColor.Lime);
            gKnownColorImport.Add("limeGreen", DrColor.LimeGreen);
            gKnownColorImport.Add("linen", DrColor.Linen);
            gKnownColorImport.Add("ltBlue", DrColor.LightBlue);
            gKnownColorImport.Add("ltCoral", DrColor.LightCoral);
            gKnownColorImport.Add("ltCyan", DrColor.LightCyan);
            gKnownColorImport.Add("ltGoldenrodYellow", DrColor.LightGoldenrodYellow);
            gKnownColorImport.Add("ltGray", DrColor.LightGray);
            gKnownColorImport.Add("ltGreen", DrColor.LightGreen);
            gKnownColorImport.Add("ltGrey", DrColor.LightGray);
            gKnownColorImport.Add("ltPink", DrColor.LightPink);
            gKnownColorImport.Add("ltSalmon", DrColor.LightSalmon);
            gKnownColorImport.Add("ltSeaGreen", DrColor.LightSeaGreen);
            gKnownColorImport.Add("ltSkyBlue", DrColor.LightSkyBlue);
            gKnownColorImport.Add("ltSlateGray", DrColor.LightSlateGray);
            gKnownColorImport.Add("ltSlateGrey", DrColor.LightSlateGray);
            gKnownColorImport.Add("ltSteelBlue", DrColor.LightSteelBlue);
            gKnownColorImport.Add("ltYellow", DrColor.LightYellow);

            gKnownColorImport.Add("magenta", DrColor.Magenta);
            gKnownColorImport.Add("maroon", DrColor.Maroon);
            gKnownColorImport.Add("medAquamarine", DrColor.MediumAquamarine);
            gKnownColorImport.Add("medBlue", DrColor.MediumBlue);
            gKnownColorImport.Add("mediumAquamarine", DrColor.MediumAquamarine);
            gKnownColorImport.Add("mediumBlue", DrColor.MediumBlue);
            gKnownColorImport.Add("mediumOrchid", DrColor.MediumOrchid);
            gKnownColorImport.Add("mediumPurple", DrColor.MediumPurple);
            gKnownColorImport.Add("mediumSeaGreen", DrColor.MediumSeaGreen);
            gKnownColorImport.Add("mediumSlateBlue", DrColor.MediumSlateBlue);
            gKnownColorImport.Add("mediumSpringGreen", DrColor.MediumSpringGreen);
            gKnownColorImport.Add("mediumTurquoise", DrColor.MediumTurquoise);
            gKnownColorImport.Add("mediumVioletRed", DrColor.MediumVioletRed);
            gKnownColorImport.Add("medOrchid", DrColor.MediumOrchid);
            gKnownColorImport.Add("medPurple", DrColor.MediumPurple);
            gKnownColorImport.Add("medSeaGreen", DrColor.MediumSeaGreen);
            gKnownColorImport.Add("medSlateBlue", DrColor.MediumSlateBlue);
            gKnownColorImport.Add("medSpringGreen", DrColor.MediumSpringGreen);
            gKnownColorImport.Add("medTurquoise", DrColor.MediumTurquoise);
            gKnownColorImport.Add("medVioletRed", DrColor.MediumVioletRed);
            gKnownColorImport.Add("midnightBlue", DrColor.MidnightBlue);
            gKnownColorImport.Add("mintCream", DrColor.MintCream);
            gKnownColorImport.Add("mistyRose", DrColor.MistyRose);
            gKnownColorImport.Add("moccasin", DrColor.Moccasin);

            gKnownColorImport.Add("navajoWhite", DrColor.NavajoWhite);
            gKnownColorImport.Add("navy", DrColor.Navy);

            gKnownColorImport.Add("oldLace", DrColor.OldLace);
            gKnownColorImport.Add("olive", DrColor.Olive);
            gKnownColorImport.Add("oliveDrab", DrColor.OliveDrab);
            gKnownColorImport.Add("orange", DrColor.Orange);
            gKnownColorImport.Add("orangeRed", DrColor.OrangeRed);
            gKnownColorImport.Add("orchid", DrColor.Orchid);

            gKnownColorImport.Add("paleGoldenrod", DrColor.PaleGoldenrod);
            gKnownColorImport.Add("paleGreen", DrColor.PaleGreen);
            gKnownColorImport.Add("paleTurquoise", DrColor.PaleTurquoise);
            gKnownColorImport.Add("paleVioletRed", DrColor.PaleVioletRed);
            gKnownColorImport.Add("papayaWhip", DrColor.PapayaWhip);
            gKnownColorImport.Add("peachPuff", DrColor.PeachPuff);
            gKnownColorImport.Add("peru", DrColor.Peru);
            gKnownColorImport.Add("pink", DrColor.Pink);
            gKnownColorImport.Add("plum", DrColor.Plum);
            gKnownColorImport.Add("powderBlue", DrColor.PowderBlue);
            gKnownColorImport.Add("purple", DrColor.Purple);

            gKnownColorImport.Add("red", DrColor.Red);
            gKnownColorImport.Add("rosyBrown", DrColor.RosyBrown);
            gKnownColorImport.Add("royalBlue", DrColor.RoyalBlue);

            gKnownColorImport.Add("saddleBrown", DrColor.SaddleBrown);
            gKnownColorImport.Add("salmon", DrColor.Salmon);
            gKnownColorImport.Add("sandyBrown", DrColor.SandyBrown);
            gKnownColorImport.Add("seaGreen", DrColor.SeaGreen);
            gKnownColorImport.Add("seaShell", DrColor.SeaShell);
            gKnownColorImport.Add("sienna", DrColor.Sienna);
            gKnownColorImport.Add("silver", DrColor.Silver);
            gKnownColorImport.Add("skyBlue", DrColor.SkyBlue);
            gKnownColorImport.Add("slateBlue", DrColor.SlateBlue);
            gKnownColorImport.Add("slateGray", DrColor.SlateGray);
            gKnownColorImport.Add("slateGrey", DrColor.SlateGray);
            gKnownColorImport.Add("snow", DrColor.Snow);
            gKnownColorImport.Add("springGreen", DrColor.SpringGreen);
            gKnownColorImport.Add("steelBlue", DrColor.SteelBlue);

            gKnownColorImport.Add("tan", DrColor.Tan);
            gKnownColorImport.Add("teal", DrColor.Teal);
            gKnownColorImport.Add("thistle", DrColor.Thistle);
            gKnownColorImport.Add("tomato", DrColor.Tomato);
            gKnownColorImport.Add("turquoise", DrColor.Turquoise);

            gKnownColorImport.Add("violet", DrColor.Violet);

            gKnownColorImport.Add("wheat", DrColor.Wheat);
            gKnownColorImport.Add("white", DrColor.White);
            gKnownColorImport.Add("whiteSmoke", DrColor.WhiteSmoke);

            gKnownColorImport.Add("yellow", DrColor.Yellow);
            gKnownColorImport.Add("yellowGreen", DrColor.YellowGreen);
        }
    }
}
