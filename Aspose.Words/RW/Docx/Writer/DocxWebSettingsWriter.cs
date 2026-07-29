// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/08/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Styles;


namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides static method for building "Web Settings" package part
    /// </summary>
    internal static class DocxWebSettingsWriter
    {
        /// <summary>
        /// Writes "Web Settings" document part for the specified document.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer, bool hasHtmlBlockReferences)
        {
            DocumentBase doc = writer.Document;
            DocPr docPr = doc.DocPr;

            DocxBuilder builder = writer.CreateChildPartAndBuilder("webSettings.xml", DocxContentType.WebSettings, writer.RelTypes.WebSettings);
            builder.StartWebSettingsDocumentPart();

            // Write PgpInfo for main document only.
            if ((doc.NodeType == NodeType.Document) && hasHtmlBlockReferences)
                NrxSettingsWriter.WriteDivs((Document)doc, builder);

            builder.WriteVal("w:encoding", docPr.WebPageEncoding);

            // Write document frames only for the main document.
            if (doc.NodeType == NodeType.Document)
                NrxFramesetWriter.Write((Document)doc, builder, true, writer.RelTypes.Frameset);

            if (docPr.OptimizeForBrowser)
            {
                builder.StartElement("w:optimizeForBrowser");
                if ((writer.Compliance != OoxmlComplianceCore.Ecma376) && // if iso29500 or later,
                    (docPr.WebTarget != WebTarget.None))
                    builder.WriteAttribute("target", DocxDopEnum.WebTargetToDocx(docPr.WebTarget));
                builder.EndElement();
            }
            builder.WriteValIfTrue("w:relyOnVML", docPr.RelyOnVml);
            builder.WriteValIfTrue("w:allowPNG", docPr.AllowPng);
            builder.WriteValIfTrue("w:doNotRelyOnCSS", docPr.DoNotRelyOnCss);
            builder.WriteValIfTrue("w:doNotSaveAsSingleFile", docPr.DoNotSaveWebPagesAsSingleFile);
            builder.WriteValIfTrue("w:doNotOrganizeInFolder", docPr.DoNotOrganizeInFolder);
            builder.WriteValIfTrue("w:doNotUseLongFileNames", docPr.DoNotUseLongFileNames);
            if (docPr.PixelsPerInch != DocPr.PixelsPerInchDefault)
                builder.WriteVal("w:pixelsPerInch", docPr.PixelsPerInch);
            if (docPr.TargetScreenSize != DocPr.TargetScreenSizeDefault)
                builder.WriteVal("w:targetScreenSz", StyleConvertUtil.ScreenSizeToXml(docPr.TargetScreenSize));

            builder.EndDocument(); //w:webSettings
        }
    }
}
