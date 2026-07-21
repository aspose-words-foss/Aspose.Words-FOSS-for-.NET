// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/05/2018 by Konstantin Sidorenko

using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose
{
    /// <summary>
    /// Port this class manually to Java, C++, etc.
    /// </summary>
    [JavaManual("Platform abstraction for List utilities. Manual porting by design.")]
    internal static class ListUtilPal
    {
        /// <summary>
        /// Manual implementation for <see cref="ListUtil.EnsureCapacity<T>(List<T> list, int capacity)"/>
        /// </summary>
        public static void EnsureCapacity<T>(List<T> list, int capacity)
        {
            if (list.Capacity < capacity)
                list.Capacity = capacity;
        }

        public static void Sort(List<string> strings)
        {
            strings.Sort();
        }
    }
}
