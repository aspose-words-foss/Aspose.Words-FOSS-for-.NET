// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Zip
{
    /// <summary>
    /// An enum that specifies the source of the ZipEntry. 
    /// </summary>
    [JavaDelete("For ZIP on Java we use a completely different implementation.")]
    public enum ZipEntrySource
    {
        /// <summary>
        /// Default value.  Invalid on a bonafide ZipEntry.
        /// </summary>
        None = 0,

        /// <summary>
        /// The entry was instantiated by calling AddFile() or another method that 
        /// added an entry from the filesystem.
        /// </summary>
        FileSystem,

        /// <summary>
        /// The entry was instantiated via <see cref="Aspose.Zip.ZipFile.AddEntry(string,string,string)"/> or
        /// <see cref="Aspose.Zip.ZipFile.AddEntry(string,string,System.IO.Stream)"/> .
        /// </summary>
        Stream,

        /// <summary>
        /// The ZipEntry was instantiated by reading a zipfile.
        /// </summary>
        ZipFile,
    }
}