// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2021 by Alexey Minenkov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its methods convert a number into ThaiArabic numbering style.
    /// </summary>
    internal static class ThaiArabicNumber
    {
        internal static string ToThaiArabic(long value)
        {
            List<int> groups = GroupSplitter.SplitToGroups(value, 1);
            StringBuilder sb = new StringBuilder();

            for (int rank = groups.Count - 1; rank >= 0; rank--)
            {
                int groupValue = groups[rank];
                sb.Append(Digits[groupValue]);
            }

            return sb.ToString();
        }

        private const string Digits = "๐๑๒๓๔๕๖๗๘๙";
    }
}
