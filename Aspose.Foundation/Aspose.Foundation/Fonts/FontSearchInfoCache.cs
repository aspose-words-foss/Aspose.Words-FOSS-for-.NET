// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2021 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents cache of loaded font search infos. Also manages background loading and serialization.
    /// </summary>
    internal class FontSearchInfoCache
    {
        public FontSearchInfoCache(IEnumerable<IFontSource> fontSources)
        {
            if (fontSources == null)
                throw new ArgumentNullException("fontSources");

            mDataList = GetDataListFromSources(fontSources);
        }

        private FontSearchInfoCache(IEnumerable<FontSearchDataContainer> dataList, ICollection<FontSearchInfo> fontSearchInfos)
        {
            mDataList = dataList;
            mSearchInfos = fontSearchInfos;
        }

        private static IEnumerable<FontSearchDataContainer> GetDataListFromSources(IEnumerable<IFontSource> fontSources)
        {
            List<FontSearchDataContainer> result = new List<FontSearchDataContainer>();
            foreach (IFontSource source in fontSources)
                foreach (IFontData fontData in source.GetFontDataInternal())
                    result.Add(new FontSearchDataContainer(fontData, source.PriorityInternal));

            return result;
        }

        public static FontSearchInfoCache LoadFromXml(IEnumerable<IFontSource> fontSources, Stream inputStream)
        {
            IEnumerable<FontSearchDataContainer> dataList = GetDataListFromSources(fontSources);
            return new FontSearchInfoCache(dataList, FontSearchInfoSerializer.LoadXml(inputStream, dataList));
        }

        public void SaveXml(Stream outputStream)
        {
            FontSearchInfoSerializer.SaveXml(outputStream, SearchInfos);
        }

        public void PreloadInBackground()
        {
            // We ignore repeated calls to this method.
            if ((mSearchInfos == null) && (mBackgroundLoader == null))
            {
                mBackgroundLoader = new FontSearchInfoLoader(mDataList);
                mBackgroundLoader.Start();
            }
        }

        public bool FontsSearchInfosLoadingInBackground
        {
            get { return (mBackgroundLoader != null) && mBackgroundLoader.IsAlive; }
        }

        /// <summary>
        /// Information about fonts located in the font sources.
        /// </summary>
        /// <remarks>
        /// After this collection is filled, it doesn't change.
        /// </remarks>
        public ICollection<FontSearchInfo> SearchInfos
        {
            get
            {
                EnsureFontsSearchInfosAreLoaded();
                return mSearchInfos;
            }
        }

        [JavaThrows(false)]
        private void EnsureFontsSearchInfosAreLoaded()
        {
            if (mSearchInfos != null)
            {
                // Any background font loading must be finished already.
                Debug.Assert(mBackgroundLoader == null);
                return;
            }

            if (mBackgroundLoader != null)
            {
                mBackgroundLoader.Join();
                mSearchInfos = mBackgroundLoader.FontSearchInfos;
                mBackgroundLoader = null;
            }
            else
            {
                mSearchInfos = FontSearchInfoLoader.Load(mDataList);
            }

            // At this point, font infos must be loaded.
            Debug.Assert(mSearchInfos != null);
        }

        private readonly IEnumerable<FontSearchDataContainer> mDataList;
        private FontSearchInfoLoader mBackgroundLoader;
        private ICollection<FontSearchInfo> mSearchInfos;
    }
}
