// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2016 by Ivan Lyagin
// 
using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Provides document streams to perform a GOLD comparison test.
    /// </summary>
    public interface IComparisonStreamProvider
    {
        /// <summary>
        /// Returns an out document stream.
        /// </summary>
        [JavaThrows(true)]
        Stream GetStreamOut();
        /// <summary>
        /// Returns a GOLD document stream.
        /// </summary>
        [JavaThrows(true)]
        Stream GetStreamGold();
        /// <summary>
        /// Returns an original GOLD document stream. When Java GOLD document exists, .Net GOLD document 
        /// is referred as original GOLD.
        /// </summary>
        [JavaThrows(true)]
        Stream GetStreamOriginalGold();
        /// <summary>
        /// Returns an MS Word document stream.
        /// </summary>
        [JavaThrows(true)]
        Stream GetStreamMS();
    }
}
