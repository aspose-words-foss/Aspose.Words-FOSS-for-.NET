// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2012 by Konstantin Kornilov

using Aspose.Collections.Generic;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'cmap' table with platform-encoding which should contain format 4 subtable (Unicode UCS-2).
    /// </summary>
    internal class CmapUcs2 : Cmap
    {
        public CmapUcs2(SortedIntegerListGeneric<int> charMap, int language, int platformId, int encodingId) 
            : base(charMap, language)
        {
            mPlatformId = platformId;
            mEncodingId = encodingId;
        }

        protected override CmapSubtable[] BuildSubtables()
        {
            return new CmapSubtable[]
                {
                    new CmapSubtableFormat4(mPlatformId, mEncodingId, CharMap, Language)
                };
        }

        /// <summary>
        /// True if MicrosoftSymbolEncoding is specified.
        /// </summary>
        public override bool IsSymbolEncoding
        {
            get { return ((mPlatformId == PlatformIdMicrosoft) && (mEncodingId == MicrosoftEncodingIdSymbol)); }
        }

        private readonly int mPlatformId;
        private readonly int mEncodingId;
    }
}
