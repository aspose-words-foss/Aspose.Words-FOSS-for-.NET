// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2011 by Alexey Titov

using System;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Core.Dml.Themes
{
    /// <summary>
    /// Converts ThemeColor enumeration values to or from string.
    /// </summary>
    internal static class ThemeColorConverter
    {
        internal static ThemeColor FromString(string color)
        {
            if (!StringUtil.HasChars(color))
                return ThemeColor.None;
            
            switch (color)
            {
                case "accent1":
                    return ThemeColor.Accent1;
                case "accent2":
                    return ThemeColor.Accent2;
                case "accent3":
                    return ThemeColor.Accent3;
                case "accent4":
                    return ThemeColor.Accent4;
                case "accent5":
                    return ThemeColor.Accent5;
                case "accent6":
                    return ThemeColor.Accent6;
                case "dk1":
                    return ThemeColor.Dark1;
                case "dk2":
                    return ThemeColor.Dark2;
                case "folHlink":
                case "followedHyperlink":
                    return ThemeColor.FollowedHyperlink;
                case "hlink":
                case "hyperlink":
                    return ThemeColor.Hyperlink;
                case "lt1":
                case "light1":
                    return ThemeColor.Light1;
                case "lt2":
                case "light2":
                    return ThemeColor.Light2;
                case "tx1":
                case "text1":
                    return ThemeColor.Text1;
                case "text2":
                case "tx2":
                    return ThemeColor.Text2;
                case "bg1":
                case "background1":
                    return ThemeColor.Background1;
                case "bg2":
                case "background2":
                    return ThemeColor.Background2;
                default:
                    return ThemeColor.Dark1;
            }
        }

        internal static string ToString(ThemeColor color)
        {
            switch (color)
            {
                case ThemeColor.Accent1:
                    return "accent1";
                case ThemeColor.Accent2:
                    return "accent2";
                case ThemeColor.Accent3:
                    return "accent3";
                case ThemeColor.Accent4:
                    return "accent4";
                case ThemeColor.Accent5:
                    return "accent5";
                case ThemeColor.Accent6:
                    return "accent6";
                case ThemeColor.Dark1:
                    return "dk1";
                case ThemeColor.Dark2:
                    return "dk2";
                case ThemeColor.FollowedHyperlink:
                    return "folHlink";
                case ThemeColor.Hyperlink:
                    return "hlink";
                case ThemeColor.Light1:
                    return "lt1";
                case ThemeColor.Light2:
                    return "lt2";
                case ThemeColor.Text1:
                    return "tx1";
                case ThemeColor.Text2:
                    return "tx2";
                case ThemeColor.Background1:
                    return "bg1";
                case ThemeColor.Background2:
                    return "bg2";
                case ThemeColor.None:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
        }

        /// <summary>
        /// Converts a specified theme color reference to a string.
        /// </summary>
        /// <remarks>
        /// The values are restricted by the list of 17.18.97 ST_ThemeColor (Theme Color) of ISO29500-1.  
        /// </remarks>
        internal static string ThemeReferenceToString(ThemeColor color)
        {
            switch (color)
            {
                case ThemeColor.Accent1:
                    return "accent1";
                case ThemeColor.Accent2:
                    return "accent2";
                case ThemeColor.Accent3:
                    return "accent3";
                case ThemeColor.Accent4:
                    return "accent4";
                case ThemeColor.Accent5:
                    return "accent5";
                case ThemeColor.Accent6:
                    return "accent6";
                case ThemeColor.Background1:
                    return "background1";
                case ThemeColor.Background2:
                    return "background2";
                case ThemeColor.Dark1:
                    return "dark1";
                case ThemeColor.Dark2:
                    return "dark2";
                case ThemeColor.FollowedHyperlink:
                    return "followedHyperlink";
                case ThemeColor.Hyperlink:
                    return "hyperlink";
                case ThemeColor.Light1:
                    return "light1";
                case ThemeColor.Light2:
                    return "light2";
                case ThemeColor.Text1:
                    return "text1";
                case ThemeColor.Text2:
                    return "text2";
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
        }
    }
}
