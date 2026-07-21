// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/08/2016 by Victor Chebotok

using System.Collections.Generic;
using Aspose.Fonts.TrueType;

namespace Aspose.Fonts
{
    /// <summary>
    /// This class helps to load <see cref="FontSearchInfo"/> from font sources.
    /// </summary>
    internal class FontSearchInfoLoader : ThreadPal
    {
        /// <summary>
        /// Creates an instance of the class that can be used to load font information from font sources asynchronously
        /// (in a separate worker thread).
        /// </summary>
        public FontSearchInfoLoader(IEnumerable<FontSearchDataContainer> dataList)
        {
            Debug.Assert(dataList != null);
            mDataList = dataList;
        }

        public static ICollection<FontSearchInfo> Load(IEnumerable<FontSearchDataContainer> dataList)
        {
            return new FontSearchInfoLoader(dataList).LoadImpl();
        }

        protected override void DoWork()
        {
            mFontSearchInfos = LoadImpl();
        }

        /// <summary>
        /// Font information that has been loaded from font sources asynchronously.
        /// This property is filled with actual values only after the worker thread stops.
        /// </summary>
        public ICollection<FontSearchInfo> FontSearchInfos
        {
            get { return mFontSearchInfos; }
        }

        private ICollection<FontSearchInfo> LoadImpl()
        {
            // Register encodings support if required.
            EncodingUtil.RegisterEncodings();

            try
            {
                List<FontSearchInfo> result = new List<FontSearchInfo>();
                TTFontFiler filer = new TTFontFiler();
                foreach (FontSearchDataContainer data in mDataList)
                {
                    if (IsCancelled())
                        return gEmptyFontSearchInfos;
                    filer.ExtractFontSearchInfo(result, data.FontData, data.SourcePriority);
                }

                return result;
            }
            catch
            {
                // Suppress exceptions. We don't want to crash while accessing the font directories.
                return gEmptyFontSearchInfos;
            }
        }

        private readonly IEnumerable<FontSearchDataContainer> mDataList;

        /// <summary>
        /// Empty font information. Used when no real font information is available.
        /// </summary>
        private static readonly ICollection<FontSearchInfo> gEmptyFontSearchInfos = new List<FontSearchInfo>();

        /// <summary>
        /// Font information loaded from font sources by the worker thread. Filled after the thread stops.
        /// </summary>
        private ICollection<FontSearchInfo> mFontSearchInfos = gEmptyFontSearchInfos;
    }
}
