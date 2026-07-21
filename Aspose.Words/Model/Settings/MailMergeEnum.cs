// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2009 by Roman Korchagin

using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Settings;

namespace Aspose.Words.Settings
{
    /// <summary>
    /// Converts mail merge settings enums to/from DOCX and WML.
    /// </summary>
    internal static class MailMergeEnum
    {
        internal static MailMergeMainDocumentType XmlToMailMergeMainDocumentType(string value)
        {
            switch (value)
            {
                case "catalog": return MailMergeMainDocumentType.Catalog;
                case "email": return MailMergeMainDocumentType.Email;
                case "envelopes": return MailMergeMainDocumentType.Envelopes;
                case "fax": return MailMergeMainDocumentType.Fax;
                case "formLetters": return MailMergeMainDocumentType.FormLetters;
                case "form-letters": return MailMergeMainDocumentType.FormLetters;
                case "mailingLabels": return MailMergeMainDocumentType.MailingLabels;
                case "mailing-labels": return MailMergeMainDocumentType.MailingLabels;
                // This returns a value different from .Default because in the model and in OOXML the default value is different.
                default: return MailMergeMainDocumentType.FormLetters;
            }
        }

        internal static string MailMergeMainDocumentTypeToXml(MailMergeMainDocumentType value, bool isDocx)
        {
            // This has no usual check for .Default because in the model and in OOXML the default value is different.

            switch (value)
            {
                case MailMergeMainDocumentType.Catalog: return "catalog";
                case MailMergeMainDocumentType.Email: return "email";
                case MailMergeMainDocumentType.Envelopes: return "envelopes";
                case MailMergeMainDocumentType.Fax: return "fax";
                case MailMergeMainDocumentType.FormLetters: return (isDocx) ? "formLetters" : "form-letters";
                case MailMergeMainDocumentType.MailingLabels: return (isDocx) ? "mailingLabels" : "mailing-labels";
                // It is better to throw here because this value must be written according to the schema.
                default: throw new InvalidOperationException("Unknown mail merge main document type.");
            }
        }

        internal static MailMergeDestination XmlToMailMergeDestination(string value)
        {
            switch (value)
            {
                case "newDocument": return MailMergeDestination.NewDocument;
                case "new-document": return MailMergeDestination.NewDocument;
                case "printer": return MailMergeDestination.Printer;
                case "email": return MailMergeDestination.Email;
                case "fax": return MailMergeDestination.Fax;
                default: return MailMergeDestination.Default;
            }
        }

        internal static string MailMergeDestinationToXml(MailMergeDestination value, bool isDocx)
        {
            if (value == MailMergeDestination.Default)
                return "";

            switch (value)
            {
                case MailMergeDestination.NewDocument: return (isDocx) ? "newDocument" : "new-document";
                case MailMergeDestination.Printer: return "printer";
                case MailMergeDestination.Email: return "email";
                case MailMergeDestination.Fax: return "fax";
                default: return "";
            }
        }

        internal static MailMergeDataType XmlToMailMergeDataType(string value)
        {
            switch (value)
            {
                case "database": return MailMergeDataType.Database;
                case "Access": return MailMergeDataType.Database;

                case "native": return MailMergeDataType.Native;
                case "ODSO": return MailMergeDataType.Native;

                case "odbc": return MailMergeDataType.Odbc;
                case "ODBC": return MailMergeDataType.Odbc;

                case "query": return MailMergeDataType.Query;
                case "QT": return MailMergeDataType.Query;

                case "spreadsheet": return MailMergeDataType.Spreadsheet;
                case "Excel": return MailMergeDataType.Spreadsheet;

                case "textFile": return MailMergeDataType.TextFile;
                case "file": return MailMergeDataType.TextFile;
                    
                default: return MailMergeDataType.Default;
            }
        }

        internal static string MailMergeDataTypeToXml(MailMergeDataType value, bool isDocx)
        {
            switch (value)
            {
                case MailMergeDataType.Database: return isDocx ? "database" : "Access";
                case MailMergeDataType.Native: return isDocx ? "native" : "ODSO";
                case MailMergeDataType.Odbc: return isDocx ? "odbc" : "ODBC";
                case MailMergeDataType.Query: return isDocx ? "query" : "QT";
                case MailMergeDataType.Spreadsheet: return isDocx ? "spreadsheet" : "Excel";
                case MailMergeDataType.TextFile: return isDocx ? "textFile" : "file";
                default: throw new InvalidOperationException("Unknown mail merge data source type.");
            }
        }

        internal static OdsoDataSourceType XmlToOdsoDataSourceType(string value)
        {
            switch (value)
            {
                case "addressBook": return OdsoDataSourceType.AddressBook;
                case "database": return OdsoDataSourceType.Database;
                case "document1": return OdsoDataSourceType.Document1;
                case "document2": return OdsoDataSourceType.Document2;
                case "email": return OdsoDataSourceType.Email;
                case "legacy": return OdsoDataSourceType.Legacy;
                case "master": return OdsoDataSourceType.Master;
                case "native": return OdsoDataSourceType.Native;
                case "text": return OdsoDataSourceType.Text;
                default:
                {
                    int result = FormatterPal.TryParseInt(value);
                    if (result != int.MinValue)
                        return (OdsoDataSourceType)result;
                    else
                        return OdsoDataSourceType.Default;
                }
            }
        }

        internal static string OdsoDataSourceTypeToXml(OdsoDataSourceType value, bool isDocx)
        {
            if (value == OdsoDataSourceType.Default)
                return "";

            if (isDocx)
            {
                switch (value)
                {
                    case OdsoDataSourceType.AddressBook: return "addressBook";
                    case OdsoDataSourceType.Database: return "database";
                    case OdsoDataSourceType.Document1: return "document1";
                    case OdsoDataSourceType.Document2: return "document2";
                    case OdsoDataSourceType.Email: return "email";
                    case OdsoDataSourceType.Legacy: return "legacy";
                    case OdsoDataSourceType.Master: return "master";
                    case OdsoDataSourceType.Native: return "native";
                    case OdsoDataSourceType.Text: return "text";
                    default: return "";
                }
            }
            else
            {
                return ((int)value).ToString();
            }
        }

        internal static OdsoFieldMappingType XmlToOdsoFieldMappingType(string value)
        {
            switch (value)
            {
                case "dbColumn": return OdsoFieldMappingType.Column;
                case "db-column": return OdsoFieldMappingType.Column;

                case "null": return OdsoFieldMappingType.Null;

                // These are present only in WML but not in DOCX. Let's default them for now.
                case "address-block":
                case "salutation":
                case "mapped":
                case "barcode":
                    return OdsoFieldMappingType.Default;

                default: return OdsoFieldMappingType.Default;
            }
        }

        internal static string OdsoFieldMappingTypeToXml(OdsoFieldMappingType value, bool isDocx)
        {
            if (value == OdsoFieldMappingType.Default)
                return "";

            switch (value)
            {
                case OdsoFieldMappingType.Column: return isDocx ? "dbColumn" : "db-column";
                case OdsoFieldMappingType.Null: return "null";
                default: return "";
            }
        }

        internal static PredefinedMergeFieldName StringToPredefinedMergeFieldName(string value)
        {
            return (PredefinedMergeFieldName)gPredefinedMergeFieldNameMap.GetValue(value, (int)PredefinedMergeFieldName.Invalid);
        }

        internal static string PredefinedMergeFieldNameToString(PredefinedMergeFieldName value)
        {
            return gPredefinedMergeFieldNameMap.GetValue((int)value, "");
        }

        private static readonly StringToIntBidirectionalMap gPredefinedMergeFieldNameMap = new StringToIntBidirectionalMap();
        static MailMergeEnum()
        {
            // 30 standard mail merge address fields. Defined in 2.9.162 ODSOPropertyBase in the DOC specification.
            gPredefinedMergeFieldNameMap.AddEntry("Unique Identifier", (int)PredefinedMergeFieldName.UniqueIdentifier);
            gPredefinedMergeFieldNameMap.AddEntry("Courtesy Title", (int)PredefinedMergeFieldName.CourtesyTitle);
            gPredefinedMergeFieldNameMap.AddEntry("First Name", (int)PredefinedMergeFieldName.FirstName);
            gPredefinedMergeFieldNameMap.AddEntry("Middle Name", (int)PredefinedMergeFieldName.MiddleName);
            gPredefinedMergeFieldNameMap.AddEntry("Last Name", (int)PredefinedMergeFieldName.LastName);
            gPredefinedMergeFieldNameMap.AddEntry("Suffix", (int)PredefinedMergeFieldName.Suffix);
            gPredefinedMergeFieldNameMap.AddEntry("Nickname", (int)PredefinedMergeFieldName.Nickname);
            gPredefinedMergeFieldNameMap.AddEntry("Job Title", (int)PredefinedMergeFieldName.JobTitle);
            gPredefinedMergeFieldNameMap.AddEntry("Company", (int)PredefinedMergeFieldName.Company);
            gPredefinedMergeFieldNameMap.AddEntry("Address 1", (int)PredefinedMergeFieldName.Address1);
            gPredefinedMergeFieldNameMap.AddEntry("Address 2", (int)PredefinedMergeFieldName.Address2);
            gPredefinedMergeFieldNameMap.AddEntry("City", (int)PredefinedMergeFieldName.City);
            gPredefinedMergeFieldNameMap.AddEntry("State", (int)PredefinedMergeFieldName.State);
            gPredefinedMergeFieldNameMap.AddEntry("Postal Code", (int)PredefinedMergeFieldName.PostalCode);
            gPredefinedMergeFieldNameMap.AddEntry("Country or Region", (int)PredefinedMergeFieldName.CountryOrRegion);
            gPredefinedMergeFieldNameMap.AddEntry("Business Phone", (int)PredefinedMergeFieldName.BusinessPhone);
            gPredefinedMergeFieldNameMap.AddEntry("Business Fax", (int)PredefinedMergeFieldName.BusinessFax);
            gPredefinedMergeFieldNameMap.AddEntry("Home Phone", (int)PredefinedMergeFieldName.HomePhone);
            gPredefinedMergeFieldNameMap.AddEntry("Home Fax", (int)PredefinedMergeFieldName.HomeFax);
            gPredefinedMergeFieldNameMap.AddEntry("E-mail Address", (int)PredefinedMergeFieldName.EmailAddress);
            gPredefinedMergeFieldNameMap.AddEntry("Web Page", (int)PredefinedMergeFieldName.WebPage);
            gPredefinedMergeFieldNameMap.AddEntry("Spouse Courtesy Title", (int)PredefinedMergeFieldName.SpouseCourtesyTitle);
            gPredefinedMergeFieldNameMap.AddEntry("Spouse First Name", (int)PredefinedMergeFieldName.SpouseFirstName);
            gPredefinedMergeFieldNameMap.AddEntry("Spouse Middle Name", (int)PredefinedMergeFieldName.SpouseMiddleName);
            gPredefinedMergeFieldNameMap.AddEntry("Spouse Last Name", (int)PredefinedMergeFieldName.SpouseLastName);
            gPredefinedMergeFieldNameMap.AddEntry("Spouse Nickname", (int)PredefinedMergeFieldName.SpouseNickname);
            gPredefinedMergeFieldNameMap.AddEntry("Phonetic Guide for First Name", (int)PredefinedMergeFieldName.PhoneticGuideForFirstName);
            gPredefinedMergeFieldNameMap.AddEntry("Phonetic Guide for Last Name", (int)PredefinedMergeFieldName.PhoneticGuideForLastName);
            gPredefinedMergeFieldNameMap.AddEntry("Address 3", (int)PredefinedMergeFieldName.Address3);
            gPredefinedMergeFieldNameMap.AddEntry("Department", (int)PredefinedMergeFieldName.Department);

        }
    }
}
