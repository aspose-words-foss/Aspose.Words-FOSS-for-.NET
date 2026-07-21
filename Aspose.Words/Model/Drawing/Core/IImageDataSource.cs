// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2011 by Alexey Titov

using Aspose.JavaAttributes;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Interface to define a data source for image data.
    /// </summary>
    internal interface IImageDataSource
    {
        [JavaThrows(true)]
        string SourceFullName { get; set; }

        [JavaThrows(true)]
        byte[] ImageBytes
        {
            get;
            set;
        }

        [JavaThrows(true)]

        bool HasImageBytes
        {
            get;
        }

    }
}
