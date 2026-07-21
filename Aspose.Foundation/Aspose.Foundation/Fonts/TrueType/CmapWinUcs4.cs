// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2012 by Konstantin Kornilov

using Aspose.Collections.Generic;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'cmap' table for Windows platform and Unicode UCS-4 encoding.
    /// </summary>
    internal class CmapWinUcs4 : Cmap
    {
        public CmapWinUcs4(SortedIntegerListGeneric<int> charMap, int language) : base(charMap, language)
        {
        }

        protected override CmapSubtable[] BuildSubtables()
        {
            // Specification says that format 4 subtable should be included along with format 12 subtable
            // for backward compatibility.
            // Subtables should be sorted by platform id and then by encoding id.
            return new CmapSubtable[]
                {
                    new CmapSubtableFormat4(PlatformIdMicrosoft, MicrosoftEncodingIdUnicodeUcs2, CharMap, Language),
                    new CmapSubtableFormat12(PlatformIdMicrosoft, MicrosoftEncodingIdUnicodeUcs4, CharMap, Language)
                };
        }

        /// <summary>
        /// True if MicrosoftSymbolEncoding is specified.
        /// </summary>
        public override bool IsSymbolEncoding
        {
            get { return false; }
        }
    }
}
