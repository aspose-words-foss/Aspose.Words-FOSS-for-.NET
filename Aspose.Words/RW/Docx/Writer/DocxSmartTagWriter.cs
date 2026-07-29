// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2008 by Roman Korchagin

using Aspose.Words.Markup;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    internal static class DocxSmartTagWriter
    {
        internal static void WriteStart(SmartTag smartTag, DocxBuilder builder)
        {
            builder.StartElement("w:smartTag");

            builder.WriteAttribute("w:uri", smartTag.Uri);
            builder.WriteAttribute("w:element", smartTag.Element);

            WriteProperties(smartTag.Properties, builder);
        }

        private static void WriteProperties(CustomXmlPropertyCollection props, DocxBuilder builder)
        {
            if (props.Count == 0)
                return;

            builder.StartElement("w:smartTagPr");

            foreach (CustomXmlProperty prop in props)
            {
                builder.StartElement("w:attr");
                builder.WriteAttribute("w:uri", prop.Uri);
                builder.WriteAttribute("w:name", prop.Name);
                builder.WriteAttribute("w:val", prop.Value);
                builder.EndElement();   // w:attr
            }

            builder.EndElement();   // w:smartTagPr
        }

        internal static void WriteEnd(DocxBuilder builder)
        {
            builder.EndElement();   // w:smartTag
        }
    }
}
