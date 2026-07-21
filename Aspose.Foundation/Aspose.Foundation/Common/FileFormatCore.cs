// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2012 by Andrey Soldatov

using System;
using Aspose.Collections.Generic;

namespace Aspose.Common
{
    /// <summary>
    /// Helper class with operations over <see cref="FileFormat"/>.
    ///
    /// For now, provides conversion to/from:
    /// 1. IANA Content Type;
    /// 2. Standard file extensions.
    /// </summary>
    public static class FileFormatCore
    {
        /// <summary>
        /// Converts IANA Content Type to <see cref="FileFormat"/>.
        /// </summary>
        public static FileFormat FromContentType(string contentType)
        {
            if (!StringUtil.HasChars(contentType))
                return FileFormat.Unknown;

            return gFromContentType.GetSafe(contentType, FileFormat.Unknown);
        }

        /// <summary>
        /// Converts file extension to <see cref="FileFormat"/>.
        /// </summary>
        public static FileFormat FromExt(string extension)
        {
            if (!StringUtil.HasChars(extension))
                return FileFormat.Unknown;

            if (extension.StartsWith(".", StringComparison.Ordinal))
                extension = extension.Substring(1);

            return gFromExtension.GetSafe(extension, FileFormat.Unknown);
        }

        /// <summary>
        /// Converts <see cref="FileFormat"/> to IANA Content Type.
        /// </summary>
        public static string ToContentType(FileFormat fileFormat)
        {
            string contentType = gToContentType[(int)fileFormat];
            return contentType;
        }

        /// <summary>
        /// Converts <see cref="FileFormat"/> to standard file extension.
        /// </summary>
        public static string ToExt(FileFormat fileFormat)
        {
            string extension = gToExtension[(int)fileFormat];
            return extension;
        }

        /// <summary>
        /// Used instead of enum.ToString() that is prohibited by Aspose.Auckland practics.
        /// </summary>
        public static string ToString(FileFormat fileFormat)
        {
            string name = gToString[(int)fileFormat];
            Debug.Assert(StringUtil.HasChars(name));
            return name;
        }

        private static void InitMap(FileFormat fileFormat, string name, string contentType, string ext)
        {
            if (!gToString.ContainsKey((int)fileFormat))
                gToString.Add((int)fileFormat, name);

            // Null Content Type is used to define several extensions for a file format.
            if (contentType != null)
            {
                if (!gToContentType.ContainsKey((int)fileFormat))
                    gToContentType.Add((int)fileFormat, contentType);

                // First appearance of a content type in the list defines conversion to file format.
                if (!gFromContentType.ContainsKey(contentType))
                    gFromContentType.Add(contentType, fileFormat);
            }

            if (ext != null)
            {
                // First appearance of a file format in the list defines conversion to extension.
                if (!gToExtension.ContainsKey((int)fileFormat))
                    gToExtension.Add((int)fileFormat, ext);

                // First appearance of an extension in the list define conversion to file format.
                if (!gFromExtension.ContainsKey(ext))
                    gFromExtension.Add(ext, fileFormat);
            }
        }

        static FileFormatCore()
        {
            // WORDSNET-22697 We can't associate the 'xml' file extension with the XML file format,
            // because WordML documents use that extension too.
            InitMap(FileFormat.Xml, "Xml", "text/xml", null);
            InitMap(FileFormat.Xml, "Xml", "application/xml", null);
            InitMap(FileFormat.Tiff, "Tiff", "image/tiff", "tif");
            InitMap(FileFormat.Tiff, "Tiff", null, "tiff");
            InitMap(FileFormat.Bmp, "Bmp", "image/bmp", "bmp");
            InitMap(FileFormat.Png, "Png", "image/png", "png");
            InitMap(FileFormat.Jpeg, "Jpeg", "image/jpeg", "jpeg");
            InitMap(FileFormat.Jpeg, "Jpeg", null, "jpg");
            InitMap(FileFormat.Emf, "Emf", "image/x-emf", "emf");
            InitMap(FileFormat.Wmf, "Wmf", "image/x-wmf", "wmf");
            InitMap(FileFormat.Pict, "Pict", "image/x-pict", "pict");
            InitMap(FileFormat.Pict, "Pict", null, "pct");
            InitMap(FileFormat.Gif, "Gif", "image/gif", "gif");
            InitMap(FileFormat.WebP, "WebP", "image/webp", "webp");

            InitMap(FileFormat.Xps, "Xps", "application/vnd.ms-xpsdocument", "xps");
            InitMap(FileFormat.OpenXps, "Oxps", "application/oxps", "oxps");
            InitMap(FileFormat.Pdf, "Pdf", "application/pdf", "pdf");
            InitMap(FileFormat.Ps, "Ps", "application/postscript", "ps");
            InitMap(FileFormat.Eps, "Eps", "image/x-eps", "eps");
            InitMap(FileFormat.Pcl, "Pcl", "application/pcl", "pcl");
            InitMap(FileFormat.Svg, "Svg", "image/svg+xml", "svg");
            InitMap(FileFormat.Epub, "Epub", "application/epub+zip", "epub");
            InitMap(FileFormat.Azw3, "Azw3", "application/x-mobi8-ebook", "azw3");
            InitMap(FileFormat.Html, "Html", "text/html", "html");
            InitMap(FileFormat.Html, "Html", null, "htm");
            InitMap(FileFormat.Html, "Html", null, "xhtml");
            InitMap(FileFormat.HtmlFixed, "HtmlFixed", null, "html");
            InitMap(FileFormat.Txt, "Txt", "text/plain", "txt");

            InitMap(FileFormat.Markdown, "Md", "text/markdown", "md");
            InitMap(FileFormat.Markdown, "Md", null, "markdown");
            InitMap(FileFormat.Markdown, "Md", null, "markdn");
            InitMap(FileFormat.Markdown, "Md", null, "mdown");
            InitMap(FileFormat.Markdown, "Md", null, "mdwn");
            InitMap(FileFormat.Markdown, "Md", null, "mkdn");
            InitMap(FileFormat.Markdown, "Md", null, "mkd");

            InitMap(FileFormat.Doc, "Doc", "application/msword", "doc");
            InitMap(FileFormat.Dot, "Dot", "application/msword", "dot");
            InitMap(FileFormat.DocPreWord97, "DocPreWord97", null, "doc");
            InitMap(FileFormat.Rtf, "Rtf", "application/rtf", "rtf");
            InitMap(FileFormat.WordML, "WordML", "text/xml", "wml");
            InitMap(FileFormat.WordML, "WordML", null, "wordml");
            InitMap(FileFormat.WordML, "WordML", null, "xml");
            InitMap(FileFormat.Mhtml, "Mhtml", "multipart/related", "mht");
            InitMap(FileFormat.Mhtml, "Mhtml", null, "mhtml");
            InitMap(FileFormat.Mhtml, "Mhtml", null, "mhtm");
            InitMap(FileFormat.Mhtml, "Mhtml", null, "msg");
            InitMap(FileFormat.Mhtml, "Mhtml", null, "eml");

            InitMap(FileFormat.Mobi, "Mobi", "application/x-mobipocket-ebook", "mobi");
            InitMap(FileFormat.Mobi, "Mobi", null, "prc");

            InitMap(FileFormat.Chm, "Chm", "application/vnd.ms-htmlhelp", "chm");

            InitMap(FileFormat.Docx, "Docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
            InitMap(FileFormat.Docm, "Docm", "application/vnd.ms-word.document.macroEnabled.12", "docm");
            InitMap(FileFormat.Dotx, "Dotx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx");
            InitMap(FileFormat.Dotm, "Dotm", "application/vnd.ms-word.template.macroEnabled.12", "dotm");
            InitMap(FileFormat.Odt, "Odt", "application/vnd.oasis.opendocument.text", "odt");
            InitMap(FileFormat.Ott, "Ott", "application/vnd.oasis.opendocument.text-template", "ott");

            // AM. I'm not sure about .fopc extension here. Word uses .xml for any FlatOpc.
            InitMap(FileFormat.FlatOpc, "FlatOpc",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document+xml", "fopc");
            InitMap(FileFormat.FlatOpcMacroEnabled, "FlatOpcMacroEnabled",
                "application/vnd.ms-word.document.macroEnabled.main+xml", "fopc");
            InitMap(FileFormat.FlatOpcTemplate, "FlatOpcTemplate",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml", "fopc");
            InitMap(FileFormat.FlatOpcTemplateMacroEnabled, "FlatOpcTemplateMacroEnabled",
                "application/vnd.ms-word.template.macroEnabledTemplate.main+xml", "fopc");

            InitMap(FileFormat.XamlFlow, "XamlFlow", "application/xaml+xml", "xaml");
            InitMap(FileFormat.XamlFlowPack, "XamlFlowPack", "application/xaml+xml", "xamlpack");
            InitMap(FileFormat.XamlFixed, "XamlFixed", "application/xml", "xaml");

            InitMap(FileFormat.Mov, "QuickTimeMovie", "application/movie", "mov");
            InitMap(FileFormat.Ico, "Ico", "image/x-icon", "ico");
            InitMap(FileFormat.Odttf, "Odttf", "application/vnd.openxmlformats-officedocument.obfuscatedFont", "odttf");

            InitMap(FileFormat.Xlsx, "Xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");

            InitMap(FileFormat.MsWorks, "Wps", null, "wps");

            InitMap(FileFormat.Docling, "Docling", "application/json", "json");
        }

        private static readonly SortedIntegerListGeneric<string> gToString = new SortedIntegerListGeneric<string>(50);
        private static readonly SortedStringListGeneric<FileFormat> gFromContentType = new SortedStringListGeneric<FileFormat>(false);
        private static readonly SortedStringListGeneric<FileFormat> gFromExtension = new SortedStringListGeneric<FileFormat>(false);

        private static readonly SortedIntegerListGeneric<string> gToContentType = new SortedIntegerListGeneric<string>();
        private static readonly SortedIntegerListGeneric<string> gToExtension = new SortedIntegerListGeneric<string>();
    }
}
