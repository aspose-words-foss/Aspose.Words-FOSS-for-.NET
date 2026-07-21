// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2017 by Alexey Butalov

using System;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Words.Loading;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Html.Reader;
using Aspose.Words.RW.Markdown.Reader;
using Aspose.Words.RW.Txt.Reader;
using Aspose.Words.Themes;

namespace Aspose.Words.RW.Factories
{
    internal static class ReaderFactory
    {
        /// <summary>
        /// Creates and returns a document reader object for the specified file format.
        /// </summary>
        internal static IDocumentReader CreateReader(
            Stream stream,
            LoadOptions loadOptions,
            LoadFormat loadFormat,
            FileFormatInfo fileFormatInfo,
            Document document)
        {
            switch (loadFormat)
            {
                case LoadFormat.Doc:
                case LoadFormat.Dot:
                case LoadFormat.DocPreWord60:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Html:
                    throw new NotSupportedException("FOSS: HTML file format is not supported in the Aspose.Words FOSS version.");
                case LoadFormat.Mhtml:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Mobi:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Azw3:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Chm:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Epub:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Rtf:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.WordML:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Docx:
                case LoadFormat.Docm:
                case LoadFormat.Dotx:
                case LoadFormat.Dotm:
                case LoadFormat.FlatOpc:
                case LoadFormat.FlatOpcMacroEnabled:
                case LoadFormat.FlatOpcTemplate:
                case LoadFormat.FlatOpcTemplateMacroEnabled:
                    return new OpenXmlDocumentReader(stream, loadOptions, fileFormatInfo, document);
                case LoadFormat.Odt:
                case LoadFormat.Ott:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.Text:
                    return new TxtReader(stream, loadOptions, fileFormatInfo, document);
                case LoadFormat.Markdown:
                    return new MarkdownReader(stream, loadOptions, fileFormatInfo, document);
#if !CPLUSPLUS
                case LoadFormat.Pdf:
                    throw new NotSupportedException("FOSS");
#endif
                case LoadFormat.Xml:
                    throw new NotSupportedException("FOSS");
                case LoadFormat.MsWorks:
                    throw new NotSupportedException("FOSS");
                default:
                    throw new UnsupportedFileFormatException("Unsupported file format: " + loadFormat.ToString());
            }
        }

        internal static IDocumentReader CreateHtmlDocumentInserter(
            string html,
            HtmlInsertOptions insertionOptions,
            DocumentBuilder documentBuilder,
            HashSetGeneric<string> bookmarkNames)
        {
            return new HtmlDocumentInserter(html, insertionOptions, documentBuilder, bookmarkNames);
        }

        internal static IChartInserter CreateChartInserter()
        {
            return new ChartInserter();
        }

        internal static IDefaultThemeProvider CreateDefaultThemeProvider()
        {
            return new DocxDefaultThemeReader();
        }
    }
}
