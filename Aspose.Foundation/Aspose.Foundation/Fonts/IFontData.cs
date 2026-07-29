// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2011 by Konstantin Kornilov

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    /// <summary>
    /// Base class for binary font data.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppVirtualInheritance("System.Object")]
    public interface IFontData
    {
        /// <summary>
        /// Opens the stream with font data. The caller is responsible for disposing the stream.
        /// </summary>
        [JavaThrows(true)]
        Stream OpenStream();

        /// <summary>
        /// Returns the size of the font data in bytes.
        /// </summary>
        int GetSize();

        /// <summary>
        /// Returns path to the font file if any.
        /// </summary>
        string GetFilePath();

        /// <summary>
        /// Returns the cache key of this source.
        /// </summary>
        /// <remarks>
        /// The name is changed to *Internal for Java.
        /// It was name conflict in StreamFontSource between this method IFontData.GetCacheKey() and public property CacheKey:
        /// both of them became public getCacheKey() on Java.
        /// </remarks>
        string GetCacheKeyInternal();

        /// <summary>
        /// Returns font data as byte array.
        /// </summary>
        [JavaThrows(true)]
        byte[] GetFontBytes();

        /// <summary>
        /// True if font data is embedded into source document.
        /// </summary>
        bool IsEmbedded { get; }
    }
}
