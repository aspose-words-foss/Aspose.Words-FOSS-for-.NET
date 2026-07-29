// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/09/2016 by Alexey Morozov

using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Implements reading of comments extended data.
    /// </summary>
    internal static class DocxCommentsExReader
    {
        /// <summary>
        /// Reads the "CommentsEx" part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.CommentsEx);
            if (xmlReader == null)
                return;

            while (xmlReader.ReadChild("commentsEx"))
            {
                switch (xmlReader.LocalName)
                {
                    case "commentEx":
                    {
                        int paraId = NrxXmlUtil.TryHexToInt(xmlReader.ReadAttribute("paraId", "00000000"));
                        int paraIdParent = NrxXmlUtil.TryHexToInt(xmlReader.ReadAttribute("paraIdParent", "00000000"));
                        bool done = (xmlReader.ReadAttribute("done", "0") != "0");
                        reader.MapParaIdAndParentParaId(paraId, paraIdParent, done);
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
