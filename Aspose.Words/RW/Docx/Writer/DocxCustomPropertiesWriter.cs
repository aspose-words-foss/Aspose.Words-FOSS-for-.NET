// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/08/2007 by Vladimir Averkin
using System;
using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides static method for building "Custom Properties" package part
    /// </summary>
    internal static class DocxCustomPropertiesWriter
    {
        /// <summary>
        /// Writes "Custom Properties" document part for the specified document.
        /// </summary>
        internal static void Write(DocxWriter writer)
        {
            Document doc = writer.Document;

            if (!NrxDocPropertiesWriter.HasWritableProperties(doc.CustomDocumentProperties))
                return;

            DocxBuilder builder = writer.CreatePartAndBuilder(@"/docProps/custom.xml", DocxContentType.CustomProperties, writer.RelTypes.CustomProperties);
            builder.StartCustomPropertiesDocumentPart();


            // RK It looks like MS Word here generates property ids instead of using existing ones, we do the same.
            int pid = DocumentProperty.MinUserPropId;
            foreach (DocumentProperty prop in doc.CustomDocumentProperties)
            {
                if (NrxDocPropertiesWriter.IsWritableProperty(prop))
                {
                    builder.StartElement("property");
                    // This is the standard guid for the custom document properties.
                    builder.WriteAttributeString("fmtid", "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}");
                    builder.WriteAttributeString("pid", FormatterPal.IntToXml(pid));
                    builder.WriteAttributeString("name", prop.Name);
                    builder.WriteAttribute("linkTarget", prop.LinkTarget);
                    WritePropertyValue(prop, builder);
                    builder.EndElement();
                    pid++;
                }
            }

            builder.EndDocument(); //Properties
        }

        private static void WritePropertyValue(DocumentProperty prop, NrxXmlBuilder builder)
        {
            switch (prop.Type)
            {
                case PropertyType.Boolean:
                    builder.StartElement("vt:bool");
                    builder.WriteString(prop.ToBool() ? "true" : "false");
                    builder.EndElement();
                    break;
                case PropertyType.Number:
                    builder.StartElement("vt:i4");
                    builder.WriteString(FormatterPal.IntToXml(prop.ToInt()));
                    builder.EndElement();
                    break;
                case PropertyType.Double:
                    builder.StartElement("vt:r8");
                    builder.WriteString(FormatterPal.DoubleToStr(prop.ToDouble()));
                    builder.EndElement();
                    break;
                case PropertyType.String:
                    builder.StartElement("vt:lpwstr");
                    builder.WriteStringConvertSpecialCharacters((string)prop.Value);
                    builder.EndElement();
                    break;
                case PropertyType.DateTime:
                    builder.StartElement("vt:filetime");
                    builder.WriteString(FormatterPal.DateTimeToXmlUtc(prop.ToDateTime()));
                    builder.EndElement();
                    break;
                default:
                    // We should not get here because we are not writing properties without a value as
                    // it makes an OOXML document that MS Word refuses to open.
                    throw new InvalidOperationException("Unexpected property type.");
            }
        }
    }
}
