// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2009 by Roman Korchagin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes mail merge settings to DOCX.
    /// </summary>
    internal class DocxMailMergePrWriter : NrxMailMergePrWriter
    {
        /// <summary>
        /// Writes the specified settings to the specified writer.
        /// </summary>
        internal void Write(MailMergeSettings mailMergeSettings, DocxDocumentWriterBase writer)
        {
            mWriter = writer;
            WriteCore(mailMergeSettings, writer);
        }

        protected override void DoWriteDataSource(string value)
        {
            WriteSource("w:dataSource", mWriter.RelTypes.MailMergeDataSource, value);
        }

        protected override void DoWriteHeaderSource(string value)
        {
            WriteSource("w:headerSource", mWriter.RelTypes.MailMergeHeaderSource, value);
        }

        protected override void DoWriteOdsoSource(string value)
        {
            // RK Force escaping ODSO source because it might be a file path and will not be escaped by the packaging code.
            WriteSource("w:src", mWriter.RelTypes.MailMergeDataSource, UriUtil.EscapeHref(value));
        }

        private void WriteSource(string elemName, string relationshipType, string value)
        {
            if (StringUtil.HasChars(value))
            {
                DocxBuilder builder = mWriter.CurrentBuilder;
                builder.WriteRelationshipId(elemName, builder.Part.Rels.Add(relationshipType, value, true));
            }
        }

        protected override void DoWriteRecipientData(Odso odso)
        {
            string relId = WriteRecipientDataPart(odso);
            if (StringUtil.HasChars(relId))
                mWriter.CurrentBuilder.WriteRelationshipId("w:recipientData", relId);
        }

        protected override void DoWriteOdsoTypeAndHeader(Odso odso)
        {
            mWriter.CurrentBuilder.WriteVal("w:type", MailMergeEnum.OdsoDataSourceTypeToXml(odso.DataSourceType, true));
            mWriter.CurrentBuilder.WriteValIfTrue("w:fHdr", odso.FirstRowContainsColumnNames);
        }

        private string WriteRecipientDataPart(Odso odso)
        {
            if (odso.RecipientDatas.Count == 0)
                return null;

            // This creates a child part of the settings.xml part.
            string relId;
            DocxBuilder builder = mWriter.CreateChildPartAndBuilder(
                mWriter.CurrentBuilder.Part,
                "recipientData.xml",
                DocxContentType.MailMergeRecipientData,
                mWriter.RelTypes.MailMergeRecipientData,
                out relId);

            builder.StartDocumentWithStandardNamespaces("w:recipients");

            foreach (OdsoRecipientData recipientData in odso.RecipientDatas)
            {
                builder.StartElement("w:recipientData");
                builder.WriteVal("w:active", recipientData.Active);
                builder.WriteVal("w:column", recipientData.Column);         // required
                builder.WriteVal("w:uniqueTag", recipientData.UniqueTag);   // required
                builder.EndElement();   //w:recipientData
            }

            builder.EndDocument();   //w:recipients

            return relId;
        }

        private DocxDocumentWriterBase mWriter;
    }
}
