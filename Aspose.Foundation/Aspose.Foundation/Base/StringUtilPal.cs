// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2010 by Roman Korchagin

using System.Globalization;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// Port this class manually to Java.
    /// </summary>
    [JavaManual("Platform abstraction for string and character utilities. Manual porting by design.")]
    internal static class StringUtilPal
    {
        /// <summary>
        /// Manual implementation for <see cref="StringUtil.IsPrivateUseCategory"/>
        /// </summary>
        public static bool IsPrivateUseCategory(char c)
        {
            return (char.GetUnicodeCategory(c) == UnicodeCategory.PrivateUse);
        }

        /// <summary>
        /// Manual implementation for <see cref="StringUtil.CompareStringSort"/>
        /// </summary>
        public static int CompareStringSort(string a, string b)
        {
            // This is the way it needs to written to compile on .NET 1.1 and 2.0.
            CompareInfo compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            return compareInfo.Compare(a, b, CompareOptions.StringSort);
        }

        /// <summary>
        /// Manual implementation for <see cref="String.PadRight"/>
        /// </summary>
        public static string PadRight(string s, int totalWidth, char paddingChar)
        {
            return s.PadRight(totalWidth,paddingChar);
        }
    }
}
