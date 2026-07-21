// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2011 by Roman Korchagin

using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Xml;

namespace Aspose.Words.RW.Nrx.Writer
{
    internal static class CustomXmlDataStorePropertiesWriter
    {
        internal static void Write(CustomXmlPart customXmlPart, AnyXmlBuilder builder, bool isDocxIsoStrict)
        {
            builder.StartDocument("ds:datastoreItem");
            // Write attribute first, followed by the namespace URI, this is the way MS Word writes and makes the gold more similar to the original file.
            builder.WriteAttributeString("ds:itemID", customXmlPart.Id);

            builder.WriteAttributeString("xmlns:ds", 
                DocxNamespaces.GetNamespace(DocxNamespace.CustomXml, isDocxIsoStrict));

            builder.StartElement("ds:schemaRefs");
            foreach (string namespaceUri in customXmlPart.Schemas)
            {
                builder.StartElement("ds:schemaRef");
                builder.WriteAttributeString("ds:uri", namespaceUri);
                builder.EndElement("ds:schemaRef");
            }
            builder.EndElement("ds:schemaRefs");

            builder.EndDocument();
        }
    }
}
