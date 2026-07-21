// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2019 by Alexey Butalov

using System.IO;
using Aspose.Common;

namespace Aspose.Words
{
    /// <summary>
    /// Helps to fetch resources in Aspose.Words.dll assembly.
    /// </summary>
    internal static class ResourceUtil
    {
        /// <summary>
        /// Returns a resource stream in Aspose.Words.dll assembly or throws if the resource cannot be found.
        /// </summary>
        /// <param name="fullResourceName">Full resource name.</param>
        public static Stream FetchResourceStream(string fullResourceName)
        {
            return SystemPal.FetchResourceStream(fullResourceName, typeof(Document));
        }
    }
}
