// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Enumerates all possible priorities of the <see cref="Delimiter"/> objects.
    /// </summary>
    internal enum DelimiterPriority
    {
        Lowest = int.MinValue,
        Emphases,
        LinkText,
        LinkDestination,
        AutoLink,
        InlineCode = AutoLink,
        Highest = int.MaxValue
    }
}
