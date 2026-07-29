// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/02/2011 by Alexey Titov

using System;
using System.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// <p>Represents the color scheme of the document theme which contains twelve colors.</p>
    /// <p><see cref="ThemeColors"/> object contains six accent colors, two dark colors, two light colors 
    /// and a color for each of a hyperlink and followed hyperlink.</p>
    /// </summary>
    /// <dev>
    /// 20.1.6.2 clrScheme (Color Scheme)
    /// Defines a set of colors which are referred to as a color scheme. The Color Scheme Color elements appear in a sequence. 
    /// </dev>
    public class ThemeColors
    {
        internal ThemeColors(IThemeProvider theme)
        {
            mTheme = theme;
        }

        /// <summary>
        /// Specifies color Accent 1.
        /// </summary>
        public Color Accent1
        {
            get { return GetNativeColor(ThemeColor.Accent1); }
            set { SetNativeColor(ThemeColor.Accent1, value); }
        }

        /// <summary>
        /// Specifies color Accent 2.
        /// </summary>
        public Color Accent2
        {
            get { return GetNativeColor(ThemeColor.Accent2); }
            set { SetNativeColor(ThemeColor.Accent2, value); }
        }

        /// <summary>
        /// Specifies color Accent 3.
        /// </summary>
        public Color Accent3
        {
            get { return GetNativeColor(ThemeColor.Accent3); }
            set { SetNativeColor(ThemeColor.Accent3, value); }
        }

        /// <summary>
        /// Specifies color Accent 4.
        /// </summary>
        public Color Accent4
        {
            get { return GetNativeColor(ThemeColor.Accent4); }
            set { SetNativeColor(ThemeColor.Accent4, value); }
        }

        /// <summary>
        /// Specifies color Accent 5.
        /// </summary>
        public Color Accent5
        {
            get { return GetNativeColor(ThemeColor.Accent5); }
            set { SetNativeColor(ThemeColor.Accent5, value); }
        }

        /// <summary>
        /// Specifies color Accent 6.
        /// </summary>
        public Color Accent6
        {
            get { return GetNativeColor(ThemeColor.Accent6); }
            set { SetNativeColor(ThemeColor.Accent6, value); }
        }

        /// <summary>
        /// Specifies color Dark 1.
        /// </summary>
        public Color Dark1
        {
            get { return GetNativeColor(ThemeColor.Dark1); }
            set { SetNativeColor(ThemeColor.Dark1, value); }
        }

        /// <summary>
        /// Specifies color Dark 2.
        /// </summary>
        public Color Dark2
        {
            get { return GetNativeColor(ThemeColor.Dark2); }
            set { SetNativeColor(ThemeColor.Dark2, value); }
        }

        /// <summary>
        /// Specifies color for a clicked hyperlink.
        /// </summary>
        public Color FollowedHyperlink
        {
            get { return GetNativeColor(ThemeColor.FollowedHyperlink); }
            set { SetNativeColor(ThemeColor.FollowedHyperlink, value); }
        }

        /// <summary>
        /// Specifies color for a hyperlink.
        /// </summary>
        public Color Hyperlink
        {
            get { return GetNativeColor(ThemeColor.Hyperlink); }
            set { SetNativeColor(ThemeColor.Hyperlink, value); }
        }

        /// <summary>
        /// Specifies color Light 1.
        /// </summary>
        public Color Light1
        {
            get { return GetNativeColor(ThemeColor.Light1); }
            set { SetNativeColor(ThemeColor.Light1, value); }
        }

        /// <summary>
        /// Specifies color Light 2.
        /// </summary>
        public Color Light2
        {
            get { return GetNativeColor(ThemeColor.Light2); }
            set { SetNativeColor(ThemeColor.Light2, value); }
        }

        /// <summary>
        /// Clones this instance of theme colors.
        /// </summary>
        internal ThemeColors Clone()
        {
            ThemeColors lhs = (ThemeColors)MemberwiseClone();

            lhs.mColors = new DmlColor[ThemeColorCount];
            for (int i = 0; i < ThemeColorCount; i++)
            {
                if (mColors[i] != null)
                    lhs.mColors[i] = mColors[i].Clone();
            }

            return lhs;
        }

        /// <summary>
        /// Sets parent theme of this theme colors.
        /// </summary>
        internal void SetTheme(IThemeProvider theme)
        {
            mTheme = theme;
        }

        /// <summary>
        /// Adds a color. If a color with the specified name already exists then
        /// the existing color will be overwritten.
        /// </summary>
        internal void AddColor(DmlColor color, ThemeColor colorName)
        {
            mColors[(int)colorName] = color;
        }

        /// <summary>
        /// Gets a color by its name. Returns null if a color with the specified
        /// name doesn't exist.
        /// </summary>
        internal DmlColor GetColor(ThemeColor colorName)
        {
            colorName = MapColors(colorName);
            return mColors[(int)colorName];
        }

        /// <summary>
        /// Gets a color by its string representation in Xml (theme color name). Returns null if a color with the specified
        /// string representation doesn't exist.
        /// </summary>
        internal DmlColor GetColor(string themeColorStr)
        {
            return GetColor(ThemeColorConverter.FromString(themeColorStr));
        }

        /// <summary>
        /// The common name for this color scheme. 
        /// This name can show up in the user interface in a list of color schemes.
        /// </summary>
        internal string Name
        {
            get
            {
                if (mName == null)
                    mName = String.Empty;
                return mName;
            }
            set { mName = value; }
        }

        /// <summary>
        /// Maps semantic colors to actual theme colors.
        /// </summary>
        private static ThemeColor MapColors(ThemeColor color)
        {
            switch (color)
            {
                case ThemeColor.Text1:
                    return ThemeColor.Dark1;
                case ThemeColor.Text2:
                    return ThemeColor.Dark2;
                case ThemeColor.Background1:
                    return ThemeColor.Light1;
                case ThemeColor.Background2:
                    return ThemeColor.Light2;
                default:
                    return color;
            }
        }

        private Color GetNativeColor(ThemeColor colorIndex)
        {
            DmlColor dmlColor = GetColor(colorIndex);
            return (dmlColor == null) ? Color.Empty : dmlColor.CreateDrColor(mTheme, null).ToNativeColor();
        }

        private void SetNativeColor(ThemeColor colorIndex, Color color)
        {
            DmlHexRgbColor newDmlColor = new DmlHexRgbColor(string.Format("{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B));
            AddColor(newDmlColor, colorIndex);
            mIsModified = true;

            // WORDSNET-15915 Update document immediately.
            mTheme.OnChange();
        }

        /// <summary>
        /// Indicates that the object was modified by public accessors.
        /// </summary>
        internal bool IsModified
        {
            get { return mIsModified; }
        }

        /// <summary>
        /// Indicates whether all colors are set.
        /// </summary>
        internal bool AreAllColorsSet
        {
            get
            {
                foreach (ThemeColor themeColor in AllThemeColors)
                    if (GetColor(themeColor) == null)
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Possible theme colors in order of appearance.
        /// </summary>
        internal static readonly ThemeColor[] AllThemeColors = new ThemeColor[]
        {
            ThemeColor.Dark1, ThemeColor.Light1, ThemeColor.Dark2, ThemeColor.Light2,
            ThemeColor.Accent1, ThemeColor.Accent2, ThemeColor.Accent3, ThemeColor.Accent4, ThemeColor.Accent5, ThemeColor.Accent6,
            ThemeColor.Background1, ThemeColor.Background2,
            ThemeColor.Hyperlink, ThemeColor.FollowedHyperlink,
            ThemeColor.Text1, ThemeColor.Text2
        };

        private bool mIsModified;
        private DmlColor[] mColors = new DmlColor[ThemeColorCount];
        private string mName;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private IThemeProvider mTheme;
        private const int ThemeColorCount = 17;
    }
}
