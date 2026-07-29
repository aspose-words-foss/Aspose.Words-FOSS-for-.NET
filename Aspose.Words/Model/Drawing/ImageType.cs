// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2006 by Roman Korchagin


namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the type (format) of an image in a Microsoft Word document.
    /// </summary>
    /// <seealso cref="ImageData.ImageType"/>
    /// <dev>
    /// Do not renumber these. The values correspond to MSOBLIPTYPE in the Binary Drawing format.
    /// </dev>
    public enum ImageType
    {
        /// <summary>
        /// The is no image data.
        /// </summary>
        /// <dev>
        /// The spec says: "An error occurred during loading."
        /// I've seen this in some places, more likely it means "this blip record is unused".
        /// </dev>
        NoImage = 0,
        /// <summary>
        /// An unknown image type or image type that cannot be directly stored inside a Microsoft Word document.
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// Windows Enhanced Metafile.
        /// </summary>
        Emf = 2,
        /// <summary>
        /// Windows Metafile.
        /// </summary>
        Wmf = 3,
        /// <summary>
        /// Macintosh PICT. An existing image will be preserved in a document, but inserting new
        /// PICT images into a document is not supported.
        /// </summary>
        Pict = 4,
        /// <summary>
        /// JPEG JFIF.
        /// </summary>
        Jpeg = 5,
        /// <summary>
        /// Portable Network Graphics.
        /// </summary>
        Png = 6,
        /// <summary>
        /// Windows Bitmap.
        /// </summary>
        /// <dev>
        /// Note this is BMP in my model, but in a Word document this is stored as a DIB.
        /// DIB is basically a BMP without a header.
        /// </dev>
        Bmp = 7,
        /// <summary>
        /// Encapsulated PostScript.
        /// </summary>
        Eps = 8,
        /// <summary>
        /// WebP.
        /// </summary>
        WebP = 9,
        /// <summary>
        /// GIF
        /// </summary>
        Gif = 10,
        // <dev>
        // I've seen image type 18. It was actually JPEG. We support it, but no need to show in the public API.
        // </dev>
    }
}
