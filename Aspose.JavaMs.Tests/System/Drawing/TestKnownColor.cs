// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2007 by Konstantin Sidorenko
// 2016/01/27 by Anatoliy Sidorenko

using System;
using System.Drawing;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Drawing
{
    /// <summary>
    /// System.Drawing.KnownColor names and values are hardcoded in java com.aspose.ms.System.Drawing.KnownColor.
    /// When .Net original is changed - hardcoded tables in this test and java's KnownColor should be updated as well.
    /// So, please call java team if some of tests fail.
    /// </summary>
    [TestFixture, Ignore("Used for Java porting only.")]
    public class TestKnownColor
    {
        /// <summary>
        /// Compares a number of colors in KnownColor Enum in .Net and Java.
        /// </summary>
        [Test]
        public void TestSize()
        {
            // Preprocessor needed for the tests since java' KnownColor is enum 
            // (not final class with integer constans) so it non-autportable. 
#if JAVA
            int length = KnownColor.values().length;
#else
            // .Net does not contain value for None color, enumeration starts from ActiveBorder = 1.
            int length = Enum.GetValues(typeof(KnownColor)).Length;
            length++; 
#endif
            Assert.That(gKnownColorNames.Length, Is.EqualTo(length));
            Assert.That(gKnownColorValues.Length, Is.EqualTo(length));
        }

        /// <summary>
        /// Compares current system .Net KnownColor names/values and hardcoded Java analogs.
        /// </summary>
        [Test]
        public void TestValues()
        {
            for (int ordinal = 1; ordinal < gKnownColorNames.Length; ordinal++)
            {
#if JAVA
                string colorName = KnownColor.get(gKnownColorNames[ordinal]).name();
                int colorValue = KnownColor.get(colorName).value();
#else
                string colorName = ((KnownColor) ordinal).ToString();
                int colorValue = Color.FromName(colorName).ToArgb();
#endif
                try
                {
                    Assert.That(gKnownColorNames[ordinal], Is.EqualTo(colorName ));
                    Assert.That((int)gKnownColorValues[ordinal], Is.EqualTo(colorValue));
                }
                catch (AssertionException)
                {
                    // "Desktop" KnownColor value can vary depending on local system personalization settings.
                    if (colorName == "Desktop")
                        break;

                    throw new AssertionException(string.Format(
                        "Please update gKnownColorNames/gKnownColorValues! The tables are used to update Java's hardcoded KnownColor class after autoporting.\r\n" +
                        "ordinal: {0}\r\n" +
                        "colorName/colorValue                       : {1}/{2}\r\n" +
                        "gKnownColorNames/gKnownColorValues[ordinal]: {3}/{4})",
                        ordinal, colorName, FormatterPal.IntToStrX(colorValue),
                        gKnownColorNames[ordinal], FormatterPal.IntToStrX((int)gKnownColorValues[ordinal])));
                }
            }
        }

        // Names from .Net KnownColor implementation.
        private static readonly string[] gKnownColorNames = new string[]
        {
            "None",
            // Special colors.
            "ActiveBorder",
            "ActiveCaption",
            "ActiveCaptionText",
            "AppWorkspace",
            "Control",
            "ControlDark",
            "ControlDarkDark",
            "ControlLight",
            "ControlLightLight",
            "ControlText",
            "Desktop",
            "GrayText",
            "Highlight",
            "HighlightText",
            "HotTrack",
            "InactiveBorder",
            "InactiveCaption",
            "InactiveCaptionText",
            "Info",
            "InfoText",
            "Menu",
            "MenuText",
            "ScrollBar",
            "Window",
            "WindowFrame",
            "WindowText",

            // Specific colors.
            "Transparent",
            "AliceBlue",
            "AntiqueWhite",
            "Aqua",
            "Aquamarine",
            "Azure",
            "Beige",
            "Bisque",
            "Black",
            "BlanchedAlmond",
            "Blue",
            "BlueViolet",
            "Brown",
            "BurlyWood",
            "CadetBlue",
            "Chartreuse",
            "Chocolate",
            "Coral",
            "CornflowerBlue",
            "Cornsilk",
            "Crimson",
            "Cyan",
            "DarkBlue",
            "DarkCyan",
            "DarkGoldenrod",
            "DarkGray",
            "DarkGreen",
            "DarkKhaki",
            "DarkMagenta",
            "DarkOliveGreen",
            "DarkOrange",
            "DarkOrchid",
            "DarkRed",
            "DarkSalmon",
            "DarkSeaGreen",
            "DarkSlateBlue",
            "DarkSlateGray",
            "DarkTurquoise",
            "DarkViolet",
            "DeepPink",
            "DeepSkyBlue",
            "DimGray",
            "DodgerBlue",
            "Firebrick",
            "FloralWhite",
            "ForestGreen",
            "Fuchsia",
            "Gainsboro",
            "GhostWhite",
            "Gold",
            "Goldenrod",
            "Gray",
            "Green",
            "GreenYellow",
            "Honeydew",
            "HotPink",
            "IndianRed",
            "Indigo",
            "Ivory",
            "Khaki",
            "Lavender",
            "LavenderBlush",
            "LawnGreen",
            "LemonChiffon",
            "LightBlue",
            "LightCoral",
            "LightCyan",
            "LightGoldenrodYellow",
            "LightGray",
            "LightGreen",
            "LightPink",
            "LightSalmon",
            "LightSeaGreen",
            "LightSkyBlue",
            "LightSlateGray",
            "LightSteelBlue",
            "LightYellow",
            "Lime",
            "LimeGreen",
            "Linen",
            "Magenta",
            "Maroon",
            "MediumAquamarine",
            "MediumBlue",
            "MediumOrchid",
            "MediumPurple",
            "MediumSeaGreen",
            "MediumSlateBlue",
            "MediumSpringGreen",
            "MediumTurquoise",
            "MediumVioletRed",
            "MidnightBlue",
            "MintCream",
            "MistyRose",
            "Moccasin",
            "NavajoWhite",
            "Navy",
            "OldLace",
            "Olive",
            "OliveDrab",
            "Orange",
            "OrangeRed",
            "Orchid",
            "PaleGoldenrod",
            "PaleGreen",
            "PaleTurquoise",
            "PaleVioletRed",
            "PapayaWhip",
            "PeachPuff",
            "Peru",
            "Pink",
            "Plum",
            "PowderBlue",
            "Purple",
            "Red",
            "RosyBrown",
            "RoyalBlue",
            "SaddleBrown",
            "Salmon",
            "SandyBrown",
            "SeaGreen",
            "SeaShell",
            "Sienna",
            "Silver",
            "SkyBlue",
            "SlateBlue",
            "SlateGray",
            "Snow",
            "SpringGreen",
            "SteelBlue",
            "Tan",
            "Teal",
            "Thistle",
            "Tomato",
            "Turquoise",
            "Violet",
            "Wheat",
            "White",
            "WhiteSmoke",
            "Yellow",
            "YellowGreen",
            "ButtonFace",
            "ButtonHighlight",
            "ButtonShadow",
            "GradientActiveCaption",
            "GradientInactiveCaption",
            "MenuBar",
            "MenuHighlight",

        };

        // Values from .Net KnownColor implementation.
        private static readonly long[] gKnownColorValues =
        {
            // Placeholder for KnownColor value of zero.
            0x00000000,//None

            // Special colors - may be overridden by profiles.
            0xFFB4B4B4,//ActiveBorder
            0xFF99B4D1,//ActiveCaption
            0xFF000000,//ActiveCaptionText
            0xFFABABAB,//AppWorkspace
            0xFFF0F0F0,//Control
            0xFFA0A0A0,//ControlDark
            0xFF696969,//ControlDarkDark
            0xFFE3E3E3,//ControlLight
            0xFFFFFFFF,//ControlLightLight
            0xFF000000,//ControlText
            0xFF000000,//Desktop
            0xFF6D6D6D,//GrayText
            0xFF3399FF,//Highlight
            0xFFFFFFFF,//HighlightText
            0xFF0066CC,//HotTrack
            0xFFF4F7FC,//InactiveBorder
            0xFFBFCDDB,//InactiveCaption
            0xFF000000,//InactiveCaptionText
            0xFFFFFFE1,//Info
            0xFF000000,//InfoText
            0xFFF0F0F0,//Menu
            0xFF000000,//MenuText
            0xFFC8C8C8,//ScrollBar
            0xFFFFFFFF,//Window
            0xFF646464,//WindowFrame
            0xFF000000,//WindowText

            // Specific colors.
            0xFFFFFF,//Transparent
            0xFFF0F8FF,//AliceBlue
            0xFFFAEBD7,//AntiqueWhite
            0xFF00FFFF,//Aqua
            0xFF7FFFD4,//Aquamarine
            0xFFF0FFFF,//Azure
            0xFFF5F5DC,//Beige
            0xFFFFE4C4,//Bisque
            0xFF000000,//Black
            0xFFFFEBCD,//BlanchedAlmond
            0xFF0000FF,//Blue
            0xFF8A2BE2,//BlueViolet
            0xFFA52A2A,//Brown
            0xFFDEB887,//BurlyWood
            0xFF5F9EA0,//CadetBlue
            0xFF7FFF00,//Chartreuse
            0xFFD2691E,//Chocolate
            0xFFFF7F50,//Coral
            0xFF6495ED,//CornflowerBlue
            0xFFFFF8DC,//Cornsilk
            0xFFDC143C,//Crimson
            0xFF00FFFF,//Cyan
            0xFF00008B,//DarkBlue
            0xFF008B8B,//DarkCyan
            0xFFB8860B,//DarkGoldenrod
            0xFFA9A9A9,//DarkGray
            0xFF006400,//DarkGreen
            0xFFBDB76B,//DarkKhaki
            0xFF8B008B,//DarkMagenta
            0xFF556B2F,//DarkOliveGreen
            0xFFFF8C00,//DarkOrange
            0xFF9932CC,//DarkOrchid
            0xFF8B0000,//DarkRed
            0xFFE9967A,//DarkSalmon
            0xFF8FBC8B,//DarkSeaGreen
            0xFF483D8B,//DarkSlateBlue
            0xFF2F4F4F,//DarkSlateGray
            0xFF00CED1,//DarkTurquoise
            0xFF9400D3,//DarkViolet
            0xFFFF1493,//DeepPink
            0xFF00BFFF,//DeepSkyBlue
            0xFF696969,//DimGray
            0xFF1E90FF,//DodgerBlue
            0xFFB22222,//Firebrick
            0xFFFFFAF0,//FloralWhite
            0xFF228B22,//ForestGreen
            0xFFFF00FF,//Fuchsia
            0xFFDCDCDC,//Gainsboro
            0xFFF8F8FF,//GhostWhite
            0xFFFFD700,//Gold
            0xFFDAA520,//Goldenrod
            0xFF808080,//Gray
            0xFF008000,//Green
            0xFFADFF2F,//GreenYellow
            0xFFF0FFF0,//Honeydew
            0xFFFF69B4,//HotPink
            0xFFCD5C5C,//IndianRed
            0xFF4B0082,//Indigo
            0xFFFFFFF0,//Ivory
            0xFFF0E68C,//Khaki
            0xFFE6E6FA,//Lavender
            0xFFFFF0F5,//LavenderBlush
            0xFF7CFC00,//LawnGreen
            0xFFFFFACD,//LemonChiffon
            0xFFADD8E6,//LightBlue
            0xFFF08080,//LightCoral
            0xFFE0FFFF,//LightCyan
            0xFFFAFAD2,//LightGoldenrodYellow
            0xFFD3D3D3,//LightGray
            0xFF90EE90,//LightGreen
            0xFFFFB6C1,//LightPink
            0xFFFFA07A,//LightSalmon
            0xFF20B2AA,//LightSeaGreen
            0xFF87CEFA,//LightSkyBlue
            0xFF778899,//LightSlateGray
            0xFFB0C4DE,//LightSteelBlue
            0xFFFFFFE0,//LightYellow
            0xFF00FF00,//Lime
            0xFF32CD32,//LimeGreen
            0xFFFAF0E6,//Linen
            0xFFFF00FF,//Magenta
            0xFF800000,//Maroon
            0xFF66CDAA,//MediumAquamarine
            0xFF0000CD,//MediumBlue
            0xFFBA55D3,//MediumOrchid
            0xFF9370DB,//MediumPurple
            0xFF3CB371,//MediumSeaGreen
            0xFF7B68EE,//MediumSlateBlue
            0xFF00FA9A,//MediumSpringGreen
            0xFF48D1CC,//MediumTurquoise
            0xFFC71585,//MediumVioletRed
            0xFF191970,//MidnightBlue
            0xFFF5FFFA,//MintCream
            0xFFFFE4E1,//MistyRose
            0xFFFFE4B5,//Moccasin
            0xFFFFDEAD,//NavajoWhite
            0xFF000080,//Navy
            0xFFFDF5E6,//OldLace
            0xFF808000,//Olive
            0xFF6B8E23,//OliveDrab
            0xFFFFA500,//Orange
            0xFFFF4500,//OrangeRed
            0xFFDA70D6,//Orchid
            0xFFEEE8AA,//PaleGoldenrod
            0xFF98FB98,//PaleGreen
            0xFFAFEEEE,//PaleTurquoise
            0xFFDB7093,//PaleVioletRed
            0xFFFFEFD5,//PapayaWhip
            0xFFFFDAB9,//PeachPuff
            0xFFCD853F,//Peru
            0xFFFFC0CB,//Pink
            0xFFDDA0DD,//Plum
            0xFFB0E0E6,//PowderBlue
            0xFF800080,//Purple
            0xFFFF0000,//Red
            0xFFBC8F8F,//RosyBrown
            0xFF4169E1,//RoyalBlue
            0xFF8B4513,//SaddleBrown
            0xFFFA8072,//Salmon
            0xFFF4A460,//SandyBrown
            0xFF2E8B57,//SeaGreen
            0xFFFFF5EE,//SeaShell
            0xFFA0522D,//Sienna
            0xFFC0C0C0,//Silver
            0xFF87CEEB,//SkyBlue
            0xFF6A5ACD,//SlateBlue
            0xFF708090,//SlateGray
            0xFFFFFAFA,//Snow
            0xFF00FF7F,//SpringGreen
            0xFF4682B4,//SteelBlue
            0xFFD2B48C,//Tan
            0xFF008080,//Teal
            0xFFD8BFD8,//Thistle
            0xFFFF6347,//Tomato
            0xFF40E0D0,//Turquoise
            0xFFEE82EE,//Violet
            0xFFF5DEB3,//Wheat
            0xFFFFFFFF,//White
            0xFFF5F5F5,//WhiteSmoke
            0xFFFFFF00,//Yellow
            0xFF9ACD32,//YellowGreen
            0xFFF0F0F0,//ButtonFace
            0xFFFFFFFF,//ButtonHighlight
            0xFFA0A0A0,//ButtonShadow
            0xFFB9D1EA,//GradientActiveCaption
            0xFFD7E4F2,//GradientInactiveCaption
            0xFFF0F0F0,//MenuBar
            0xFF3399FF,//MenuHighlight
        };
    }
}
