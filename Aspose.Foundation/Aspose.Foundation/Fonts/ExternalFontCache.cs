// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2011 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Collections;
using Aspose.Fonts.TrueType;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    /// <summary>
    /// Keeps a cache of external fonts.
    /// </summary>
    public class ExternalFontCache
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="sources">Sources to locate fonts.</param>
        public ExternalFontCache(IEnumerable<IFontSource> sources)
            : this(sources, new FontSearchInfoCache(sources))
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <remarks>
        /// Used for Java to convert array to Iterable.
        /// </remarks>
        /// <param name="sources">Sources to locate fonts.</param>
        public ExternalFontCache(IFontSource[] sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");

            mFontSources = sources;// Java: Autoporter converts array to Iterable here.
            mFontSearchInfoCache = new FontSearchInfoCache(mFontSources);

            // First try to use MW specific strategy. If it fails - use general.
            mSubstitutionStrategies = NewFontSubstitutionStrategy();
        }

        private ExternalFontCache(IEnumerable<IFontSource> sources, FontSearchInfoCache searchInfoCache)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");

            mFontSources = sources;
            mFontSearchInfoCache = searchInfoCache;

            // First try to use MW specific strategy. If it fails - use general.
            mSubstitutionStrategies = NewFontSubstitutionStrategy();
        }

        private static IFontSubstitutionStrategy[] NewFontSubstitutionStrategy()
        {
            return new IFontSubstitutionStrategy[]
            {
                new FontSubstitutionStrategyMsWordSpecific(),
                new FontSubstitutionStrategyGeneral()
            };
        }

        /// <summary>
        /// Gets the list of sources where Aspose.Words looks for TrueType fonts.
        /// </summary>
        public IEnumerable<IFontSource> GetFontsSources()
        {
            return mFontSources;
        }

        /// <summary>
        /// Gets the font from cache.
        /// Returns <c>null</c> if font is not available.
        /// </summary>
        public TTFont GetFont(string familyName, FontStyle style)
        {
            if (!StringUtil.HasChars(familyName))
                return null;

            TTFont font = FindOrLoadFont(familyName, style);
            if (font != null)
                return font;

            // WORDSNET-8184 Try to trim spaces. It seems that MS Word actually tries to trim spaces before comparing font info.
            familyName = familyName.Trim();

            // WORDSNET-11130 Several CJK fonts in Windows have the "@FontName" version along original font. It seems to be related to
            // the rudimentary vertical writing support. Seems that MW treats them as original fonts at least for horizontal writing.
            // Simply trim the '@' char for now.
            familyName = familyName.TrimStart('@');

            if (!StringUtil.HasChars(familyName))
                return null;

            font = FindOrLoadFont(familyName, style);
            if (font != null)
                return font;

            return CheckSubstitutes(familyName, style);
        }

        private TTFont CheckSubstitutes(string familyName, FontStyle style)
        {
            if (FontUtil.FontSubstitutesCache.ContainsKey(familyName))
                return FindOrLoadFont(FontUtil.FontSubstitutesCache[familyName], style);

            return null;
        }

        /// <summary>
        /// Gets the substitution font from cache. Returns <c>null</c> if can't find substitution.
        /// </summary>
        public TTFont GetSubstitution(FontSubstitutionInfo info, FontStyle style)
        {
            if (!mSubstitutionCache.ContainsKey(info))
                mSubstitutionCache.Add(info, PerformSubstitution(info));

            return GetFont(mSubstitutionCache[info], style);
        }

        private string PerformSubstitution(FontSubstitutionInfo info)
        {
            foreach (IFontSubstitutionStrategy strategy in mSubstitutionStrategies)
            {
                string familyName = strategy.GetSubstitution(info, FontSearchInfos);
                if (GetFont(familyName, FontStyle.Regular) != null)
                    return familyName;
            }

            return null;
        }

        /// <summary>
        /// Gets any font from the cache. Returns <c>null</c> if no font is found.
        /// </summary>
        public TTFont GetAnyFont()
        {
            foreach (FontSearchInfo info in FontSearchInfos)
            {
                TTFont font = GetFont(info.FontFamilyName, FontStyle.Regular);
                if (font != null)
                    return font;
            }
            return null;
        }

        /// <summary>
        /// Gets the font from cache or load if font is not in cache.
        /// If font is not available returns <c>null</c>.
        /// </summary>
        [JavaThrows(false)]
        public TTFont FindOrLoadFont(string familyName, FontStyle style)
        {
            if (!StringUtil.HasChars(familyName))
                return null;

            // If we have already loaded the font family, it must be in the cache.
            TTFontFamily family = mFamiliesCache[familyName];

            // Load the family if not in the cache.
            if (family == null)
            {
                try
                {
                    family = ReadFamily(familyName);
                    // FOSS
                    mFamiliesCache[familyName] = family;
                }
                catch
                {
                    // Suppress all the exception to let the calling method to try to get another suitable
                    // one according to an algorithm.
                    return null;
                }
            }

            return family.GetFont(style, false);
        }

        private TTFontFamily ReadFamily(string familyName)
        {
            IEnumerable<FontSearchInfo> aptInfos = FindAptFontsInfo(familyName);
            IntToObjDictionary<FontSearchInfo> fontStyles = SelectFontStyles(aptInfos);
            return BuildFontFamily(familyName, fontStyles);
        }

        /// <summary>
        /// Finds appropriate fonts for specified family name.
        /// </summary>
        private IEnumerable<FontSearchInfo> FindAptFontsInfo(string familyName)
        {
            List<FontSearchInfo> aptInfos = new List<FontSearchInfo>();
            foreach (FontSearchInfo info in FontSearchInfos)
                if (info.FontFamilyNamesAllLanguages.Contains(familyName))
                    aptInfos.Add(info);

            // WORDSNET-7130 Full font name could be specified instead of family name.
            // In some cases MS Word tries to find the font by full name and in some cases doesn't (for example
            // "Calibri Bold" could be used in MS Word and "Arial Bold" could not). Not sure why MS Word
            // distinguish these fonts so try full font name in all cases.
            if (aptInfos.Count == 0)
            {
                foreach (FontSearchInfo info in FontSearchInfos)
                    if (info.FontFullNamesAllLanguages.Contains(familyName))
                        aptInfos.Add(info);
            }

            // Note: Family name in MW document is limited to 31 chars. Longer families are truncated.
            if (aptInfos.Count == 0 && familyName.Length == 31)
            {
                foreach (FontSearchInfo info in FontSearchInfos)
                foreach (string name in info.FontFullNamesAllLanguages)
                {
                    if (name.StartsWith(familyName, StringComparison.InvariantCultureIgnoreCase))
                        aptInfos.Add(info);
                }
            }

            return aptInfos;
        }

        /// <summary>
        /// Select specific font styles from appropriate font infos.
        /// </summary>
        private static IntToObjDictionary<FontSearchInfo> SelectFontStyles(IEnumerable<FontSearchInfo> aptInfos)
        {
            IntToObjDictionary<FontSearchInfo> fontStyles = new IntToObjDictionary<FontSearchInfo>();
            foreach (FontSearchInfo info in aptInfos)
            {
                FontSearchInfo existingInfo = fontStyles[(int)info.Style];
                if (existingInfo == null)
                    fontStyles[(int)info.Style] = info;
                else
                    fontStyles[(int)info.Style] = ChooseFont(existingInfo, info);
            }
            return fontStyles;
        }

        /// <summary>
        /// Choose between two fonts with the same family name and style.
        /// </summary>
        private static FontSearchInfo ChooseFont(FontSearchInfo info1, FontSearchInfo info2)
        {
            // First, check the font source priority.
            if (info1.FontSourcePriority > info2.FontSourcePriority)
                return info1;
            if (info2.FontSourcePriority > info1.FontSourcePriority)
                return info2;

            // Prefer fonts from TTC collections to simple TTF/OTF fonts.
            if (info1.IsTtc && !info2.IsTtc)
                return info1;
            if (info2.IsTtc && !info1.IsTtc)
                return info2;

            // Prefer fonts with TrueTypes outlines instead of fonts with CFF outlines.
            if (!info1.IsCff && info2.IsCff)
                return info1;
            if (!info2.IsCff && info1.IsCff)
                return info2;

            // Otherwise, there is no difference.
            return info1;
        }

        private static TTFontFamily BuildFontFamily(string familyName, IntToObjDictionary<FontSearchInfo> fontStyles)
        {
            List<TTFont> fonts = new List<TTFont>();
            foreach (FontSearchInfo info in fontStyles.Values)
            {
                TTFont font = TTFontBuilder.Read(info.FontData);
                fonts.Add(font);
            }
            return new TTFontFamily(familyName, fonts);
        }

        /// <summary>
        /// List of search infos from available font sources.
        /// </summary>
        public ICollection<FontSearchInfo> FontSearchInfos
        {
            get { return mFontSearchInfoCache.SearchInfos; }
        }

        /// <summary>
        /// Saves parsed search info cache into an stream as XML.
        /// </summary>
        public void SaveSearchInfoCache(Stream outputStream)
        {
            mFontSearchInfoCache.SaveXml(outputStream);
        }

        /// <summary>
        /// Creates a new instance and loads search info cache from XML stream.
        /// </summary>
        public static ExternalFontCache CreateAndLoadSearchInfoCache(IEnumerable<IFontSource> sources, Stream inputStream)
        {
            return new ExternalFontCache(sources, FontSearchInfoCache.LoadFromXml(sources, inputStream));
        }

        /// <summary>
        /// Starts searching the current set of font sources for information about available fonts.
        /// The search is performed in a separate thread.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Sources are searched for fonts only once - repeated calls to this method have no effect.
        /// </para>
        /// <para>
        /// Please notice that preload is an optional operation that is not required for correct functioning of the cache.
        /// Even if font information is not preloaded in background, it will be loaded in foreground before a font is retrieved
        /// from the cache.
        /// </para>
        /// </remarks>
        public void PreloadInBackground()
        {
            mFontSearchInfoCache.PreloadInBackground();
        }

        /// <summary>
        /// Indicates that background fonts searching (started by <see cref="PreloadInBackground"/> method) is in progress.
        /// Used for test purpose only.
        /// </summary>
        internal bool FontsSearchInfosLoadingInBackground
        {
            get { return mFontSearchInfoCache.FontsSearchInfosLoadingInBackground; }
        }


        private readonly IEnumerable<IFontSource> mFontSources;
        private readonly StringToObjDictionary<TTFontFamily> mFamiliesCache = new StringToObjDictionary<TTFontFamily>(false);
        private readonly FontSearchInfoCache mFontSearchInfoCache;

        private readonly IEnumerable<IFontSubstitutionStrategy> mSubstitutionStrategies;
        private readonly Dictionary<FontSubstitutionInfo, string> mSubstitutionCache = new Dictionary<FontSubstitutionInfo, string>();
    }
}
