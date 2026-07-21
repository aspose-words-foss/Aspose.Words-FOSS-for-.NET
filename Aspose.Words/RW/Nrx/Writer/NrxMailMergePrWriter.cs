// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2009 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Common base class for writing mail merge settings to DOCX an WML.
    /// </summary>
    internal abstract class NrxMailMergePrWriter
    {
        /// <summary>
        /// The concrete implementations must call this method to initiate writing of mail merge settings.
        /// </summary>
        protected void WriteCore(MailMergeSettings mailMergeSettings, INrxWriterContext writer)
        {
            if (mailMergeSettings.IsEmpty)
                return;

            mMailMergeSettings = mailMergeSettings;
            mWriter = writer;

            NrxXmlBuilder builder = mWriter.Builder;
            builder.StartElement("w:mailMerge");

            builder.WriteValRequired("w:mainDocumentType", MailMergeEnum.MailMergeMainDocumentTypeToXml(mMailMergeSettings.MainDocumentType, writer.IsDocx));
            builder.WriteValIfTrue("w:linkToQuery", mMailMergeSettings.LinkToQuery);
            builder.WriteValRequired("w:dataType", MailMergeEnum.MailMergeDataTypeToXml(mMailMergeSettings.DataType, writer.IsDocx));
            builder.WriteValEvenIfEmpty("w:connectString", mMailMergeSettings.ConnectString);
            builder.WriteVal("w:query", mMailMergeSettings.Query);
            DoWriteDataSource(mMailMergeSettings.DataSource);
            DoWriteHeaderSource(mMailMergeSettings.HeaderSource);
            builder.WriteValIfTrue("w:doNotSuppressBlankLines", mMailMergeSettings.DoNotSupressBlankLines);
            builder.WriteVal("w:destination", MailMergeEnum.MailMergeDestinationToXml(mMailMergeSettings.Destination, writer.IsDocx));
            builder.WriteVal("w:addressFieldName", mMailMergeSettings.AddressFieldName);
            builder.WriteVal("w:mailSubject", mMailMergeSettings.MailSubject);
            builder.WriteValIfTrue("w:mailAsAttachment", mMailMergeSettings.MailAsAttachment);
            builder.WriteValIfTrue("w:viewMergedData", mMailMergeSettings.ViewMergedData);
            builder.WriteValIfNotDefault("w:activeRecord", mMailMergeSettings.ActiveRecord, 0);
            builder.WriteValIfNotDefault("w:checkErrors", (int)mMailMergeSettings.CheckErrors, (int)MailMergeCheckErrors.Default);
            WriteOdso();

            builder.EndElement(); //w:mailMerge
        }

        private void WriteOdso()
        {
            NrxXmlBuilder builder = mWriter.Builder;

            builder.StartElement("w:odso");

            Odso odso = mMailMergeSettings.Odso;
            builder.WriteVal("w:udl", odso.UdlConnectString);
            builder.WriteVal("w:table", odso.TableName);

            DoWriteOdsoSource(odso.DataSource);

            int colDelim = odso.ColumnDelimiter; // RK Conversion to integer is required in order to invoke the correct overload for writing the value.
            builder.WriteValIfNotDefault("w:colDelim", colDelim, 0);

            DoWriteOdsoTypeAndHeader(odso);

            foreach (OdsoFieldMapData fieldMapData in odso.FieldMapDatas)
                WriteFieldMapData(fieldMapData, builder);

            DoWriteRecipientData(odso);

            builder.EndElement();   //w:odso
        }

        private void WriteFieldMapData(OdsoFieldMapData fieldMapData, NrxXmlBuilder builder)
        {
            builder.StartElement("w:fieldMapData");

            builder.WriteVal("w:type", MailMergeEnum.OdsoFieldMappingTypeToXml(fieldMapData.Type, mWriter.IsDocx));
            builder.WriteVal("w:name", fieldMapData.Name);
            builder.WriteVal("w:mappedName", fieldMapData.MappedName);
            
            // MS Word seems to write the column index to DOCX only if the mapping is initialized and always writes to WML.
            if (!mWriter.IsDocx || StringUtil.HasChars(fieldMapData.Name))
                builder.WriteVal("w:column", fieldMapData.Column);

            string lidTag = mWriter.IsDocx
                ? LocaleConverter.LocaleToDocxTag((int)fieldMapData.Language)
                : LocaleConverter.LocaleToWmlTag((int)fieldMapData.Language);

            builder.WriteVal("w:lid", lidTag);
            builder.WriteValIfTrue("w:dynamicAddress", fieldMapData.DynamicAddress);

            builder.EndElement();   //w:fieldMapData
        }

        // These methods are to be implemented by DOCX and WML because they are different.
        [JavaThrows(true)]  // IO Exceptions.
        protected abstract void DoWriteDataSource(string value);
        [JavaThrows(true)]  // IO Exceptions.
        protected abstract void DoWriteHeaderSource(string value);
        [JavaThrows(true)]  // IO Exceptions.
        protected abstract void DoWriteOdsoSource(string value);
        [JavaThrows(true)]  // IO Exceptions.
        protected abstract void DoWriteRecipientData(Odso odso);
        [JavaThrows(true)]  // IO Exceptions.
        protected abstract void DoWriteOdsoTypeAndHeader(Odso odso);

        private MailMergeSettings mMailMergeSettings;
        private INrxWriterContext mWriter;
    }
}
