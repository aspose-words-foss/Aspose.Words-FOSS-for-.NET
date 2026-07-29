// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/09/2020 by Alexander Zhiltsov

using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Implements reading 2.1.4 commentsIds [MS-DOCX] document parts.
    /// </summary>
    internal static class DocxCommentsIdsReader
    {
        /// <summary>
        /// Reads a commentsIds document part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.CommentsIds);
            if (xmlReader == null)
                return;

            reader.ComplianceInfo.MarkAsHasDocxExtensionsOf(MsWordVersionCore.Word2016);

            while (xmlReader.ReadChild("commentsIds"))
            {
                switch (xmlReader.LocalName)
                {
                    case "commentId":
                    {
                        int paraId = NrxXmlUtil.TryHexToInt(xmlReader.ReadAttribute("paraId", "0"));
                        int durableId = NrxXmlUtil.TryHexToInt(xmlReader.ReadAttribute("durableId", "0"));
                        reader.MapParaIdAndDurableId(paraId, durableId);
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }
    }
}
