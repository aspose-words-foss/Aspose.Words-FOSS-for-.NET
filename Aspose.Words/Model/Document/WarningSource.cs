// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/10/2011 by Alexey Titov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the module that produces a warning during document loading or saving.
    /// </summary>
    public enum WarningSource
    {
        /// <summary>
        /// The warning source is not specified.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Module that builds a document layout.
        /// </summary>
        Layout = 1,

        /// <summary>
        /// Module that renders DrawingML shapes.
        /// </summary>
        DrawingML = 2,

        /// <summary>
        /// Module that renders OfficeMath.
        /// </summary>
        OfficeMath = 3,

        /// <summary>
        /// Module that renders ordinary shapes.
        /// </summary>
        Shapes = 4,

        /// <summary>
        /// Module that renders metafiles.
        /// </summary>
        Metafile = 5,

        /// <summary>
        /// Module that renders XPS.
        /// </summary>
        Xps = 6,

        /// <summary>
        /// Module that renders PDF.
        /// </summary>
        Pdf = 7,

        /// <summary>
        /// Module that renders images.
        /// </summary>
        Image = 8,

        /// <summary>
        /// Module that reads/writes DOCX files.
        /// </summary>
        Docx = 9,

        /// <summary>
        /// Module that reads/writes binary DOC files.
        /// </summary>
        Doc = 10,

        /// <summary>
        /// Module that reads/writes plaintext files.
        /// </summary>
        Text = 11,

        /// <summary>
        /// Module that reads/writes RTF files.
        /// </summary>
        Rtf = 12,

        /// <summary>
        /// Module that reads/writes WML files.
        /// </summary>
        WordML = 13,

        /// <summary>
        /// Common modules that are shared between DOCX/WML reader/writer modules.
        /// </summary>
        Nrx = 14,

        /// <summary>
        /// Module that reads/writes ODT files.
        /// </summary>
        Odt = 15,

        /// <summary>
        /// Module that reads/writes HTML/MHTML files.
        /// </summary>
        Html = 16,

        /// <summary>
        /// Module that verifies model consistency and validity.
        /// </summary>
        Validator = 17,

        /// <summary>
        /// Module that reads/writes Xaml files.
        /// </summary>
        Xaml = 18,

        /// <summary>
        /// Module that reads Svm files.
        /// </summary>
        Svm = 19,

        /// <summary>
        /// Module that reads W3C MathML files.
        /// </summary>
        MathML = 20,

        /// <summary>
        /// Module that reads font files.
        /// </summary>
        Font = 21,

        /// <summary>
        /// Module that reads SVG files.
        /// </summary>
        Svg = 22,

        /// <summary>
        /// Module that reads/writes Markdown files.
        /// </summary>
        Markdown = 23,

        /// <summary>
        /// Module that reads CHM files.
        /// </summary>
        Chm = 24,

        /// <summary>
        /// Module that reads/writes EPUB files.
        /// </summary>
        Epub = 25,

        /// <summary>
        /// Module that reads XML files.
        /// </summary>
        Xml = 26,

        /// <summary>
        /// Module that writes XLSX files.
        /// </summary>
        Xlsx = 27,

        /// <summary>
        /// Module that writes Docling JSON files.
        /// </summary>
        Docling = 28
    }
}
