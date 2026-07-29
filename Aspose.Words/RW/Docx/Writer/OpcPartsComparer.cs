// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2019 by Artem Ptitsin

using System.Collections.Generic;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Comparer for sorting opc types in the OOXML formats.
    /// </summary>
    /// <remarks>The main goal is to sort the first three opc parts. Read more information on WORDSNET-19478</remarks>
    internal class OpcPartsComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null || y == null)
                return StringOrdinalComparer.Default.Compare(x, y);

            int xOrdinal = gOverrideTypes.GetValueOrDefault(x, 3);
            int yOrdinal = gOverrideTypes.GetValueOrDefault(y, 3);

            return xOrdinal == yOrdinal
                ? StringOrdinalComparer.Default.Compare(x, y)
                : xOrdinal.CompareTo(yOrdinal);
        }

        private static readonly Dictionary<string, int> gOverrideTypes = new Dictionary<string, int>
        {
            {"/[Content_Types].xml", 0},
            {"/_rels/.rels", 1},
            {"/word/document.xml", 2},
        };
    }
}
