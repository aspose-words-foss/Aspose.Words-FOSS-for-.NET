// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2014 by Andrey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Writer for DrawingML Click/Mouse-Over Hyperlink.
    /// </summary>
    internal static class DmlHlinkWriter
    {
        /// <summary>
        /// Writes hlinkClick, hlinkHover, hlinkMouseOver.
        /// 5.1.5.3.5 hlinkClick (Click Hyperlink)
        /// 5.1.2.1.23 hlinkHover (Hyperlink for Hover)
        /// 21.1.2.3.5 hlinkClick (Click Hyperlink)
        /// 21.1.2.3.6 hlinkMouseOver (Mouse-Over Hyperlink)
        /// </summary>
        internal static void WriteHlink(
            string elementName,
            string targetId,
            string targetFrame,
            string tooltip,
            StringToObjDictionary<DmlExtension> extList,
            IDmlShapeWriterContext writer)
        {
            // WORDSNET-23935 Hyperlink maximum length, determined experimentally.
            const int hyperlinkTargetMaxLength = 2235;

            if (targetId == null)
                return;

            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(elementName);
            builder.WriteAttribute("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));
            // Escaping some characters in Href may cause the string to be extended beyond the maximum value.
            if (UriUtil.EscapeHref(UriUtil.UnescapeHref(targetId)).Length > hyperlinkTargetMaxLength)
                targetId = string.Empty;
            // WORDSNET-5640 hLinkAddress can be empty per spec.
            string hyperlinkTarget = StringUtil.HasChars(targetId) ? writer.AddHyperlinkRelationship(targetId) : "";
            builder.WriteAttributeString("r:id", hyperlinkTarget);
            builder.WriteAttribute("xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));
            builder.WriteAttribute("tgtFrame", targetFrame);
            builder.WriteAttribute("tooltip", tooltip);
            DmlExtensionListWriter.Write(extList, writer);
            builder.EndElement();
        }
    }
}
