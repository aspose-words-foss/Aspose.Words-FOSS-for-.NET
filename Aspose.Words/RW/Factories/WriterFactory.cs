// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2010 by Roman Korchagin

using System;
using Aspose.Words.Drawing;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Vml;
using Aspose.Words.Saving;
using Aspose.Words.Validation;

namespace Aspose.Words.RW.Factories
{
    internal static class WriterFactory
    {
        /// <summary>
        /// Creates and returns a document writer object for the specified file format.
        /// </summary>
        internal static IDocumentWriter CreateWriter(SaveFormat fileFormat)
        {
            switch (fileFormat)
            {
                case SaveFormat.Doc:
                case SaveFormat.Dot:
                    throw new NotSupportedException("FOSS");

                case SaveFormat.Docx:
                case SaveFormat.Docm:
                case SaveFormat.Dotx:
                case SaveFormat.Dotm:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                    return new Docx.Writer.DocxWriter();
                case SaveFormat.Rtf:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.WordML:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Pdf:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Ps:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Pcl:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Xps:
                case SaveFormat.OpenXps:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.XamlFixed:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Svg:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.HtmlFixed:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Html:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Mhtml:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Odt:
                case SaveFormat.Ott:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Text:
                    return new Txt.Writer.TxtWriter();
                case SaveFormat.Markdown:
                    return new Markdown.Writer.MarkdownWriter();
                case SaveFormat.Epub:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Azw3:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Mobi:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.XamlFlow:
                case SaveFormat.XamlFlowPack:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Emf:
                case SaveFormat.Tiff:
                case SaveFormat.Png:
                case SaveFormat.Bmp:
                case SaveFormat.Jpeg:
                case SaveFormat.Gif:
                case SaveFormat.Eps:
#if NETSTANDARD || NET462_OR_GREATER || JAVA
                case SaveFormat.WebP:
#endif
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Xlsx:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Docling:
                    throw new NotSupportedException("FOSS");
                default:
                    throw new ArgumentException("Invalid file format requested.");
            }
        }

        /// <summary>
        /// Creates concrete instance of <see cref="OfficeMathToShapeConverter"/> class.
        /// </summary>
        internal static OfficeMathToShapeConverter CreateOfficeMathToShapeConverter(Document doc, SaveOptions saveOptions)
        {
            switch (saveOptions.SaveFormat)
            {
                case SaveFormat.Html:
                case SaveFormat.Mhtml:
                case SaveFormat.Epub:
                case SaveFormat.Azw3:
                case SaveFormat.Mobi:
                {
                    HtmlSaveOptions htmlSaveOptions = (HtmlSaveOptions)saveOptions;
                    return htmlSaveOptions.IsConvertOfficeMathToShape
                        ? new OfficeMathToShapeConverter(doc, saveOptions.SaveFormat)
                        : new NullOfficeMathToShapeConverter(doc);
                }
                case SaveFormat.Doc:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.WordML:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Rtf:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Bmp:
                case SaveFormat.Docm:
                case SaveFormat.Docx:
                case SaveFormat.Dot:
                case SaveFormat.Dotm:
                case SaveFormat.Dotx:
                case SaveFormat.Emf:
                case SaveFormat.Eps:
                case SaveFormat.FlatOpc:
                case SaveFormat.FlatOpcMacroEnabled:
                case SaveFormat.FlatOpcTemplate:
                case SaveFormat.FlatOpcTemplateMacroEnabled:
                case SaveFormat.Gif:
                case SaveFormat.HtmlFixed:
                case SaveFormat.Jpeg:
                case SaveFormat.Odt:
                case SaveFormat.OpenXps:
                case SaveFormat.Ott:
                case SaveFormat.Pdf:
                case SaveFormat.Png:
                case SaveFormat.Ps:
                case SaveFormat.Pcl:
                case SaveFormat.Svg:
                case SaveFormat.Text:
                case SaveFormat.Tiff:
                case SaveFormat.XamlFixed:
                case SaveFormat.XamlFlow:
                case SaveFormat.XamlFlowPack:
                case SaveFormat.Xps:
                case SaveFormat.Xlsx:
                case SaveFormat.Docling:
#if NETSTANDARD || NET462_OR_GREATER || JAVA
                case SaveFormat.WebP:
#endif
                    return new NullOfficeMathToShapeConverter(doc);

                // WORDSNET-27646 Implemented export option for OfficeMath in Markdown.
                case SaveFormat.Markdown:
                {
                    MarkdownSaveOptions markdownSaveOptions = (MarkdownSaveOptions)saveOptions;
                    return (markdownSaveOptions.OfficeMathExportMode == MarkdownOfficeMathExportMode.Image)
                        ? new OfficeMathToShapeConverter(doc, saveOptions.SaveFormat)
                        : new NullOfficeMathToShapeConverter(doc);
                }

                default:
                    Debug.Assert(false, "Unexpected file format.");
                    return null;
            }
        }

        internal static IVmlShapeTypeReader CreateVmlShapeTypeReader()
        {
            return new VmlShapeTypeReader();
        }
    }
}
