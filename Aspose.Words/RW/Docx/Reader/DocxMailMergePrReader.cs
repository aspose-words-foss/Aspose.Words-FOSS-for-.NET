// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/01/2017 by Alexey Butalov

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal class DocxMailMergePrReader : NrxMailMergePrReaderBase
    {
        /// <summary>
        /// Reads 2.14.27 recipientData (Reference to Inclusion/Exclusion Data for Data Source)
        /// </summary>
        protected override void ReadRecipientData(NrxDocumentReaderBase reader, Odso odso)
        {
            Debug.Assert(reader is DocxDocumentReaderBase);
            DocxDocumentReaderBase docReader = (DocxDocumentReaderBase)reader;

            string relId = docReader.XmlReader.ReadId();
            docReader.SwitchToPartReaderByRelId(relId);
            try
            {
                NrxXmlReader xmlReader = docReader.XmlReader;

                // RK I had a document created with MS Word 2007 Beta and it writes this data in a different format, 
                // let's ignore for now. If someone really wants it we can support it. Reading is not a problem,
                // just need to specify different content type when writing, different namespace and also write hash
                // instead of unique tag.
                if (xmlReader.NamespaceURI != DocxNamespaces.GetNamespace(DocxNamespace.Main, docReader.ComplianceInfo.IsIsoStrict))
                    return;

                while (xmlReader.ReadChild("recipients"))
                {
                    switch (xmlReader.LocalName)
                    {
                        case "recipientData":
                            base.ReadRecipientData(docReader, odso);
                            break;
                        default:
                            xmlReader.IgnoreElement();
                            break;
                    }
                }
            }
            finally
            {
                docReader.RestorePartReader();
            }
        }
    }
}