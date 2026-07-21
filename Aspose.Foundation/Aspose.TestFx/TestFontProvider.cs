// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2015 by Konstantin Kornilov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;
using Aspose.JavaAttributes;

namespace Aspose.TestFx
{
    /// <summary>
    /// Implementation of <see cref="IDrFontProvider"/> for unit testing.
    /// </summary>
    public class TestFontProvider : IDrFontProvider
    {
        [JavaThrows(false)]
        public TestFontProvider()
            : this(new FontSourceBaseCore[]
            {
                new FolderFontSourceCore(TestFxUtil.BuildRootTestFileName(@"FontFiles\Common"), false, 1),
                new SystemFontSourceCore(0),
                new FolderFontSourceCore(TestEnvironment.GetWinFontsFolder(), false, 2)
            })
        {
            // Use common fonts from source control and system fonts.
            // All the initialization is done by another constructor.
        }

        public TestFontProvider(string fontsFolder)
            : this(new FontSourceBaseCore[] { new FolderFontSourceCore(fontsFolder, false) })
        {
            // All the initialization is done by another constructor.
        }

        public TestFontProvider(FontSourceBaseCore[] fontSources)
        {
            mFontCache = new ExternalFontCache((IFontSource[])fontSources);
        }

        public static TestFontProvider CreateWithFonts(params string[] fontNames)
        {
            List<FontSourceBaseCore> sources = new List<FontSourceBaseCore>();
            foreach (string name in fontNames)
            {
                FontSourceBaseCore source = GetFontSourceFromSystem(name);
                if (source != null)
                    sources.Add(source);
            }

            return new TestFontProvider(sources.ToArray());
        }

        private static FontSourceBaseCore GetFontSourceFromSystem(string familyName)
        {
            string path = GetSystemFontPath(familyName, FontStyle.Regular);
            return StringUtil.HasChars(path)
                ? new FileFontSourceCore(path)
                : null;
        }

        public static string GetSystemFontPath(string familyName, FontStyle style)
        {
            TTFont font = StaticInstance.FetchTTFont(familyName, style);
            return font.Data.GetFilePath();
        }

        public TTFont GetTTFont(string familyName, FontStyle style)
        {
            return mFontCache.GetFont(familyName, style);
        }

        public TTFont FetchTTFont(string familyName, FontStyle style)
        {
            TTFont font = GetTTFont(familyName, style);

            if (font == null)
                font = mFontCache.GetFont("Times New Roman", style);

            if (font == null)
                font = mFontCache.GetAnyFont();

            return font;
        }

        public TTFont GetFallbackFont(TTFont font, int charCode, bool useCharacterReplacements)
        {
            // FOSS: font fallback resolution was removed, matching DocumentFontProvider.
            return null;
        }

        public DrFont FetchDrFont(string familyName, float sizePoints, FontStyle style)
        {
            return FetchDrFont(familyName, sizePoints, style, style, false, false);
        }

        public DrFont FetchDrFont(string familyName, float sizePoints, FontStyle actualStyle, FontStyle fontFaceStyle, bool isVertical, bool useWord97FontMetrics)
        {
            return FetchDrFont(familyName, sizePoints, actualStyle, fontFaceStyle, false, true, false);
        }

        public DrFont FetchDrFont(string familyName,
            float sizePoints,
            FontStyle actualStyle,
            FontStyle fontFaceStyle,
            bool isVertical,
            bool adjustCjkFontMetrics,
            bool useWord97FontMetrics)
        {
            return new DrFont(sizePoints, actualStyle, FetchTTFont(familyName, fontFaceStyle), isVertical, adjustCjkFontMetrics, useWord97FontMetrics);
        }

        public ExternalFontCache FontCache
        {
            get { return mFontCache; }
        }

        /// <summary>
        /// Static instance with system fonts.
        /// </summary>
        public static TestFontProvider StaticInstance
        {
            get { return gStaticInstance; }
        }

        /// <summary>
        /// Static instance with system fonts.
        /// </summary>
        /// <remarks>
        /// This instance is not supposed to be modified. If tests requires custom font sources or other cache
        /// modifications, it is possible to create new instance instead of modifying static one.
        /// </remarks>
        public static ExternalFontCache StaticCache
        {
            get { return gStaticInstance.FontCache; }
        }

        private readonly ExternalFontCache mFontCache;
        private static readonly TestFontProvider gStaticInstance = new TestFontProvider();
    }
}
