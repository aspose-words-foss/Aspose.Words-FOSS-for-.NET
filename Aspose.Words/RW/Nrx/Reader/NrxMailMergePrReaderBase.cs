// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2009 by Roman Korchagin

using System;
using System.Text;
using Aspose.JavaAttributes;
using Aspose.Words.Nrx;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Base class, helps to read mail merge settings from DOCX and WML documents.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxMailMergePrReaderBase
    {
        /// <summary>
        /// Reads 2.14.20 mailMerge (Mail Merge Settings)
        /// </summary>
        internal void Read(NrxDocumentReaderBase reader, MailMergeSettings mailMergeSettings)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // The spec says this is the default value, but I think this is the default value only when the mail merge settings are present.
            // When mail merge settings are not specified, then the default value is NotAMergeDocument. So I have to set this value as a default
            // when encounter mail merge settings to read.
            mailMergeSettings.MainDocumentType = MailMergeMainDocumentType.FormLetters;

            while (xmlReader.ReadChild("mailMerge"))
            {
                switch (xmlReader.LocalName)
                {
                    case "activeRecord":    // docx and wml
                        mailMergeSettings.ActiveRecord = xmlReader.ReadIntVal();
                        break;
                    case "addressFieldName":    // docx and wml
                        mailMergeSettings.AddressFieldName = xmlReader.ReadVal();
                        break;
                    case "checkErrors": // docx and wml
                        mailMergeSettings.CheckErrors = (MailMergeCheckErrors)xmlReader.ReadIntVal();
                        break;
                    case "connectString": // docx and wml
                    {
                        string connectString = xmlReader.ReadVal();

                        if (!string.IsNullOrEmpty(connectString))
                            mailMergeSettings.ConnectString = connectString;
                        break;
                    }
                    case "dataSource":  // docx and wml
                        mailMergeSettings.DataSource = reader.IsDocx
                            ? reader.GetRelationshipTarget(xmlReader.ReadId())
                            : xmlReader.ReadVal();
                        break;
                    case "dataType":    // docx and wml
                        mailMergeSettings.DataType = MailMergeEnum.XmlToMailMergeDataType(xmlReader.ReadVal());
                        break;
                    case "destination": // docx and wml
                        mailMergeSettings.Destination = MailMergeEnum.XmlToMailMergeDestination(xmlReader.ReadVal());
                        break;
                    case "doNotSuppressBlankLines": // docx and wml
                        mailMergeSettings.DoNotSupressBlankLines = xmlReader.ReadBoolVal();
                        break;
                    case "headerSource":    // docx and wml
                        mailMergeSettings.HeaderSource = reader.IsDocx
                            ? reader.GetRelationshipTarget(xmlReader.ReadId())
                            : xmlReader.ReadVal();
                        break;
                    case "linkToQuery": // docx and wml
                        mailMergeSettings.LinkToQuery = xmlReader.ReadBoolVal();
                        break;
                    case "mailAsAttachment":    // docx and wml
                        mailMergeSettings.MailAsAttachment = xmlReader.ReadBoolVal();
                        break;
                    case "mailSubject": // docx and wml
                        mailMergeSettings.MailSubject = xmlReader.ReadVal();
                        break;
                    case "mainDocumentType":    // docx and wml
                        mailMergeSettings.MainDocumentType = MailMergeEnum.XmlToMailMergeMainDocumentType(xmlReader.ReadVal());
                        break;
                    case "odso":    // docx and wml
                        ReadOdso(reader, mailMergeSettings.Odso);
                        break;
                    case "query":   // docx and wml
                        mailMergeSettings.Query = xmlReader.ReadVal();
                        break;
                    case "viewMergedData":  // docx and wml
                        mailMergeSettings.ViewMergedData = xmlReader.ReadBoolVal();
                        break;
                    // This is WML only. Ignore for now.
                    case "defaultSQL":
                        xmlReader.IgnoreElement();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        [JavaThrows(true)]
        protected virtual void ReadRecipientData(NrxDocumentReaderBase reader, Odso odso)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OdsoRecipientData result = new OdsoRecipientData();

            while (xmlReader.ReadChild("recipientData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "active":
                        result.Active = xmlReader.ReadBoolVal();
                        break;
                    case "column":
                        result.Column = xmlReader.ReadIntVal();
                        break;
                    case "uniqueTag":
                        // Word 2007 in DOCX writes base64 encoded, but for WordML this seems to be just plain bytes.
                        if (reader.IsDocx)
                            result.UniqueTag = Convert.FromBase64String(xmlReader.ReadVal());
                        else
                            result.UniqueTag = Encoding.Unicode.GetBytes(xmlReader.ReadVal());
                        break;
                    case "hash": // wml only
                        result.Hash = xmlReader.ReadIntVal();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            odso.RecipientDatas.Add(result);
        }

        /// <summary>
        /// Reads 2.14.25 odso (Office Data Source Object Settings)
        /// </summary>
        private void ReadOdso(NrxDocumentReaderBase reader, Odso odso)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.ReadChild("odso"))
            {
                switch (xmlReader.LocalName)
                {
                    case "colDelim":    // docx and wml
                        odso.ColumnDelimiter = (Char)xmlReader.ReadIntVal();
                        break;
                    case "fHdr":    // docx and wml
                        odso.FirstRowContainsColumnNames = xmlReader.ReadBoolVal();
                        break;
                    case "fieldMapData":    // docx and wml
                        ReadFieldMapData(reader, odso);
                        break;
                    case "recipientData":   // docx and wml
                    {
                        ReadRecipientData(reader, odso);
                        break;
                    }
                    case "src": // docx and wml
                        // RK When reading from DOCX force unescaping because it might an incomplete path
                        // and does not get unescaped by the lower level functions.
                        odso.DataSource = reader.IsDocx
                            ? UriUtil.UnescapeHref(reader.GetRelationshipTarget(xmlReader.ReadId()))
                            : xmlReader.ReadVal();
                        break;
                    case "table":   // docx and wml
                        odso.TableName = xmlReader.ReadVal();
                        break;
                    case "type":    // docx only
                    case "jdsoType":    // wml only
                        odso.DataSourceType = MailMergeEnum.XmlToOdsoDataSourceType(xmlReader.ReadVal());
                        break;
                    case "udl": // docx and wml
                        odso.UdlConnectString = xmlReader.ReadVal();
                        break;

                    // These are WML only. Let's ignore for now.
                    case "filter":
                    case "sort":
                        xmlReader.IgnoreElement();
                        break;

                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 2.14.15 fieldMapData (External Data 1 Source to Merge Field Mapping).
        /// </summary>
        private static void ReadFieldMapData(NrxDocumentReaderBase reader, Odso odso)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OdsoFieldMapData result = new OdsoFieldMapData();

            while (xmlReader.ReadChild("fieldMapData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "column":  // docx and wml
                        result.SetColumnSafe(xmlReader.ReadIntVal());
                        break;
                    case "dynamicAddress":  // docx and wml
                        result.DynamicAddress = xmlReader.ReadBoolVal();
                        break;
                    case "lid": // docx and wml
                    {
                        string lid = xmlReader.ReadVal();
                        int language = reader.IsDocx
                            ? LocaleConverter.DocxTagToLocale(lid)
                            : LocaleConverter.WmlTagToLocale(lid);

                        result.Language = (Language)language;
                        break;
                    }
                    case "mappedName":  // docx and wml
                        result.MappedName = xmlReader.ReadVal();
                        break;
                    case "name":    // docx and wml
                        result.Name = xmlReader.ReadVal();
                        break;
                    case "type":    // docx and wml
                        result.Type = MailMergeEnum.XmlToOdsoFieldMappingType(xmlReader.ReadVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            odso.FieldMapDatas.Add(result);
        }
    }
}
