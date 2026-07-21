// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2012 by Andrey Soldatov

using CodePorting.Translator.Cs2Cpp;

namespace Aspose
{
    /// <summary>
    /// <para>File format enum. It can contain file formats used in any context.</para>
    /// <para>The <c>enum</c> is used as intermediate format in <see cref="FileFormatCore"/> and <c>FileFormatUtil</c>.</para>
    /// </summary>
    /// <remarks>
    /// This enum is center point to conversions between:
    /// - File Extension : string,
    /// - Content Type : string,
    /// - LoadFormat,
    /// - SaveFormat,
    /// - ImageType.
    ///
    /// Although LoadFormat, SaveFormat and ImageType are parts of AW, this enum is completely standalone.
    /// By design, it can contain any file formats used in any context. That's why it's in Aspose.Foundation area.
    ///
    /// In Aspose.Foundation it can be used for conversions between File Extension and Content Type.
    /// </remarks>
    [CppEnumEnableMetadata]
    public enum FileFormat
    {
        /// <summary>
        /// An unknown file type.
        /// </summary>
        Unknown,

        Xml,

        /// <summary>
        /// Windows Enhanced Metafile.
        /// </summary>
        Emf,

        /// <summary>
        /// Windows Metafile.
        /// </summary>
        Wmf,

        /// <summary>
        /// Macintosh PICT.
        /// </summary>
        Pict,

        /// <summary>
        /// JPEG JFIF.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Portable Network Graphics.
        /// </summary>
        Png,

        /// <summary>
        /// Windows Bitmap.
        /// </summary>
        Bmp,

        /// <summary>
        /// Tiff.
        /// </summary>
        Tiff,

        /// <summary>
        /// Gif.
        /// </summary>
        Gif,

        /// <summary>
        /// Encapsulated PostScript.
        /// </summary>
        Eps,

        /// <summary>
        /// WebP.
        /// </summary>
        WebP,

        DocPreWord97,
        Doc,
        Dot,
        Docx,
        Docm,
        Dotx,
        Dotm,
        WordML,

        FlatOpc,
        FlatOpcMacroEnabled,
        FlatOpcTemplate,
        FlatOpcTemplateMacroEnabled,

        Odt,
        Ott,

        Rtf,
        Html,
        HtmlFixed,
        Mhtml,
        Mobi,
        Chm,
        Epub,
        Azw3,

        Txt,
        Markdown,

        Pdf,
        Xps,
        OpenXps,
        Svg,
        XamlFixed,
        XamlFlow,
        XamlFlowPack,
        Ps,
        Pcl,

        Mov,
        Ico,

        /// <summary>
        /// Embedded font.
        /// </summary>
        Odttf,

        /// <summary>
        /// Spreadsheet Open Office XML format.
        /// </summary>
        Xlsx,

        /// <summary>
        /// Microsoft Works format.
        /// </summary>
        MsWorks,

        /// <summary>
        /// Docling JSON format.
        /// </summary>
        Docling
    }
}
