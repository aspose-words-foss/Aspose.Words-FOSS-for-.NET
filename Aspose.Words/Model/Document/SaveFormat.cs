// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2006 by Roman Korchagin

using System.IO;
using Aspose.Words.Saving;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Indicates the format in which the document is saved.
    /// </summary>
    /// <seealso cref="Document.Save(Stream, SaveFormat)"/>
    /// <seealso cref="SaveOptions"/>
    /// <dev>
    /// We should only use values 0..255 here because these values are used as bit flags in the UnifiedScenario enum.
    /// </dev>
    [CppEnumEnableMetadata]
    [CppEnumWithOperators]
    public enum SaveFormat
    {
        /// <summary>
        /// Default, invalid value for file format.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Saves the document in the Microsoft Word 97 - 2007 Document format.
        /// </summary>
        Doc = 10,
        /// <summary>
        /// Saves the document in the Microsoft Word 97 - 2007 Template format.
        /// </summary>
        Dot = 11,

        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Document (macro-free).
        /// </summary>
        Docx = 20,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Macro-Enabled Document.
        /// </summary>
        Docm = 21,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Template (macro-free).
        /// </summary>
        Dotx = 22,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Macro-Enabled Template.
        /// </summary>
        Dotm = 23,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpc = 24,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Macro-Enabled Document stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpcMacroEnabled = 25,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Template (macro-free) stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpcTemplate = 26,
        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Macro-Enabled Template stored in a flat XML file instead of a ZIP package.
        /// </summary>
        FlatOpcTemplateMacroEnabled = 27,
        /// <summary>
        /// Saves the document in the RTF format.
        /// All characters above 7-bits are escaped as hexadecimal or Unicode characters.
        /// </summary>
        Rtf = 30,
        /// <summary>
        /// Saves the document in the Microsoft Word 2003 WordprocessingML format.
        /// </summary>
        WordML = 31,

        /// <summary>
        /// Saves the document as PDF (Adobe Portable Document) format.
        /// </summary>
        Pdf = 40,
        /// <summary>
        /// Saves the document in the XPS (XML Paper Specification) format.
        /// </summary>
        Xps = 41,
        /// <summary>
        /// Saves the document in the Extensible Application Markup Language (XAML) format as a fixed document.
        /// </summary>
        XamlFixed = 42,
        /// <summary>
        /// Saves the document in the Svg (Scalable Vector Graphics) format.
        /// </summary>
        Svg = 44,
        /// <summary>
        /// Saves the document in the HTML format using absolutely positioned elements
        /// </summary>
        HtmlFixed = 45,
        /// <summary>
        /// Saves the document in the OpenXPS (Ecma-388) format.
        /// </summary>
        OpenXps = 46,
        /// <summary>
        /// Saves the document in the PS (PostScript) format.
        /// </summary>
        Ps = 47,
        /// <summary>
        /// Saves the document in the PCL (Printer Control Language) format.
        /// </summary>
        Pcl = 48,
        /// <summary>
        /// Saves the document in the HTML format.
        /// </summary>
        Html = 50,
        /// <summary>
        /// Saves the document in the MHTML (Web archive) format.
        /// </summary>
        Mhtml = 51,
        /// <summary>
        /// Saves the document in the EPUB format.
        /// </summary>
        Epub = 52,
        /// <summary>
        /// Saves the document in the AZW3 format.
        /// </summary>
        Azw3 = 53,
        /// <summary>
        /// Saves the document in the MOBI format.
        /// </summary>
        Mobi = 54,

        /// <summary>
        /// Saves the document as an ODF Text Document.
        /// </summary>
        Odt = 60,
        /// <summary>
        /// Saves the document as an ODF Text Document Template.
        /// </summary>
        Ott = 61,

        /// <summary>
        /// Saves the document in the plain text format.
        /// </summary>
        Text = 70,
        /// <summary>
        /// <b>Beta.</b> Saves the document in the Extensible Application Markup Language (XAML) format as a flow document.
        /// </summary>
        XamlFlow = 71,
        /// <summary>
        /// <b>Beta.</b> Saves the document in the Extensible Application Markup Language (XAML) package format as a flow document.
        /// </summary>
        XamlFlowPack = 72,
        /// <summary>
        /// Saves the document in the Markdown format.
        /// </summary>
        Markdown = 73,

        /// <summary>
        /// Saves the document as an Office Open XML SpreadsheetML Document (macro-free).
        /// </summary>
        Xlsx = 80,

        /// <summary>
        /// Saves the document in Docling JSON format.
        /// </summary>
        Docling = 81,

        /// <summary>
        /// Renders a page or pages of the document and saves them into a single or multipage TIFF file.
        /// </summary>
        Tiff = 100,
        /// <summary>
        /// Renders a page of the document and saves it as a PNG file.
        /// </summary>
        Png = 101,
        /// <summary>
        /// Renders a page of the document and saves it as a BMP file.
        /// </summary>
        Bmp = 102,
        /// <summary>
        /// Renders a page of the document and saves it as a vector EMF (Enhanced Meta File) file.
        /// </summary>
        Emf = 103,
        /// <summary>
        /// Renders a page of the document and saves it as a JPEG file.
        /// </summary>
        Jpeg = 104,
        /// <summary>
        /// Renders a page of the document and saves it as a GIF file.
        /// </summary>
        Gif = 105,
        /// <summary>
        /// Renders a page of the document and saves it as an EPS file.
        /// </summary>
        Eps = 106,
#if NETSTANDARD || NET462_OR_GREATER || JAVA
        /// <summary>
        /// Renders a page of the document and saves it as a WebP file.
        /// </summary>
        WebP = 107
#endif
    }
}
