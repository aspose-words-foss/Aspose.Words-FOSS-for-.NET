// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2013 by Alexey Morozov

using Aspose.Words.Nrx;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Implements writing common elements related to document setting for both DOCX and WordML formats.
    /// </summary>
    internal static class NrxSettingsWriter
    {
        internal static void WriteRsids(DocPr docPr, NrxXmlBuilder builder, SaveFormat sf)
        {
            Debug.Assert((sf == SaveFormat.Docx) || (sf == SaveFormat.WordML));

            string prefix = (sf == SaveFormat.Docx)
                ? "w"
                : "wsp";    // WordML

            builder.StartElement(string.Format("{0}:rsids", prefix));
            builder.WriteVal(string.Format("{0}:rsidRoot", prefix), NrxXmlUtil.IntToHex(docPr.RsidRoot));
            for (int i = 0; i < docPr.Rsids.Count; i++)
                builder.WriteVal(string.Format("{0}:rsid", prefix), NrxXmlUtil.IntToHex(docPr.Rsids[i]));
            builder.EndElement();
        }

        internal static void WriteDivs(Document doc, NrxXmlBuilder builder)
        {
            if (doc.HtmlBlockCollection.Count == 0)
                return;

            builder.StartElement("w:divs");

            WriteDivChild(builder, doc.HtmlBlockCollection, 0);

            builder.EndElement("w:divs");
        }

        /// <summary>
        /// Writes child HtmlBlocks.
        /// </summary>
        /// <remarks>
        /// AM. This is rare feature and HtmlBlock table is not very large in common. 
        /// So I get child nodes using straight approach.
        /// </remarks>
        private static void WriteDivChild(NrxXmlBuilder builder, HtmlBlockCollection htmlBlockCollection, int parentId)
        {
            for (int i = 0; i < htmlBlockCollection.Count; i++)
            {
                HtmlBlock htmlBlock = htmlBlockCollection.GetHtmlBlockByIndex(i);

                if (htmlBlock.ParentId == parentId)
                    WriteDiv(builder, htmlBlockCollection, htmlBlock);
            }
        }

        private static void WriteDiv(NrxXmlBuilder builder, HtmlBlockCollection htmlBlockCollection, HtmlBlock htmlBlock)
        {
            builder.StartElement("w:div");
            builder.WriteAttribute("w:id", htmlBlock.Id);

            if (htmlBlock.HtmlBlockType != HtmlBlockType.Div)
                builder.WriteBoolValExplicit((htmlBlock.HtmlBlockType == HtmlBlockType.BlockQuote) ? "w:blockQuote" : "w:bodyDiv", true);

            ParaPr paraPr = htmlBlock.ParaPr;
            builder.WriteVal("w:marLeft", paraPr.FetchAttr(ParaAttr.HtmlMarginLeft));
            builder.WriteVal("w:marRight", paraPr.FetchAttr(ParaAttr.HtmlMarginRight));
            builder.WriteVal("w:marTop", paraPr.FetchAttr(ParaAttr.HtmlMarginTop));
            builder.WriteVal("w:marBottom", paraPr.FetchAttr(ParaAttr.HtmlMarginBottom));

            builder.StartElement("w:divBdr");

            builder.WriteBorder("w:top", paraPr.BorderTop);
            builder.WriteBorder("w:left", paraPr.BorderLeft);
            builder.WriteBorder("w:bottom", paraPr.BorderBottom);
            builder.WriteBorder("w:right", paraPr.BorderRight);

            builder.EndElement();

            if (htmlBlock.HasChild)
            {
                builder.StartElement("w:divsChild");

                WriteDivChild(builder, htmlBlockCollection, htmlBlock.Id);

                builder.EndElement("w:divsChild");
            }

            builder.EndElement();
        }
    }
}
