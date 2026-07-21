// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/14/2013 by Alexey Noskov

using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers
{
    public interface IToComparableConverter
    {
        /// <summary>
        /// Converts input file to comparable file type, like image, text or zip.
        /// </summary>
        [JavaThrows(true)]
        string[] Convert(string inFileName);

        /// <summary>
        /// Returns type of output comparable file.
        /// </summary>
        ComparableFileType FileType { get; }
    }
}