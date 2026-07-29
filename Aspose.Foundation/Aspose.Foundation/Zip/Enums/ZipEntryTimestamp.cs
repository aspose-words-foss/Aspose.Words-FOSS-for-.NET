// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;

using Aspose.JavaAttributes;

namespace Aspose.Zip
{
    /// <summary>
    /// An enum that specifies the type of timestamp available on the ZipEntry. 
    /// </summary>
    [Flags]
    [JavaDelete("For ZIP on Java we use a completely different implementation.")]
    public enum ZipEntryTimestamp
    {
        /// <summary>
        /// Default value.  
        /// </summary>
        None = 0,

        /// <summary>
        /// A DOS timestamp with 2-second precision.
        /// </summary>
        DOS = 1,

        /// <summary>
        /// A Windows timestamp with 100-ns precision.
        /// </summary>
        Windows = 2,

        /// <summary>
        /// A Unix timestamp with 1-second precision.
        /// </summary>
        Unix = 4,

        /// <summary>
        /// A Unix timestamp with 1-second precision, stored in InfoZip v1 format.  This
        /// format is outdated and is supported for reading archives only.  
        /// </summary>
        InfoZip1 = 8,
    }
}