// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2022 by Artem Shabarshin

using System;
using System.IO;
using System.Net;
using System.Xml;
using Aspose.IO;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Resolves XHTML entities.
    /// </summary>
    /// <remarks>
    /// We need to make sure the XML parser recognizes XHTML entities in case the document references XHTML DTD. However, we
    /// cannot allow the XML parser to load DTD from external resources. As a partial solution, we provide an artificial
    /// XHTML DTD containing only entity definitions.
    /// </remarks>
    [JavaAttributes.JavaManual(".Net XmlResolver.GetEntity() is changed to Java's XMLResolver.resolveEntity()")]
    internal class XhtmlXmlResolver : XmlResolver
    {
        public override object GetEntity(
            Uri absoluteUri,
            string role,
            Type ofObjectToReturn)
        {
            if (ofObjectToReturn == typeof(Stream))
            {
                // The XHTML entity may be resolved during parsing the DocumentType node as well as during parsing Element nodes.
                // If the XHTML entity is resolved during the Element node then it is an external entity.
                // Like Adobe Digital Editions, we don't support external entities and resolve them to empty strings.
                if (mInsideElementNode)
                    return new MemoryStream(new byte[0], false);

                string uri = absoluteUri.ToString();

                // If XML parser resolves XHTML 1.1 DTD resource then we return all XHTML entities.
                // XHTML doctype declarations list: https://www.w3.org/QA/2002/04/valid-dtd-list.html
                if ((uri == "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd") ||
                    (uri == "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd") ||
                    (uri == "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd") ||
                    (uri == "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd") ||
                    (uri == "http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd"))
                {
                    byte[] xhtmlDtd;
                    using (Stream stream = ResourceUtil.FetchResourceStream("Aspose.Words.Resources.XhtmlEntities.dtd"))
                    {
                        xhtmlDtd = StreamUtil.CopyStreamToByteArray(stream);
                    }

                    // We return a read-only stream, because we don't want the calling code to modify the content of DTD.
                    return new MemoryStream(xhtmlDtd, false);
                }

                // The XML parser first tries to load the XHTML DTD using the public identifier. It appends the public
                // identifier to the base URL of the XHTML document and passes an URL like
                // "file:///C:/Users/Username/AppData/Local/Temp/-/W3C/DTD%20XHTML%201.1/EN".
                // We return null if we detect that the XML parser passed the public identifier. This makes the XML parser
                // try to load XHTML DTD using the system identifier (an URI). If we don't return null here, the XML parser
                // won't use the system identifier and won't load the DTD at all.
                // Note that public identifiers are case-insensitive.
                if (StringUtil.IndexOfIgnoreCase(uri, "-/W3C/DTD XHTML") >= 0)
                {
                    return null;
                }

                // It is an unknown entity in the DocumentType node.
                // Resolve it to an empty string.
                return new MemoryStream(new byte[0], false);
            }

            return null;
        }

        [JavaAttributes.JavaDelete]
        public override ICredentials Credentials
        {
            set
            {
                // Nothing to do. We have to override this property, because it is abstract in .NET 2.0.
            }
        }

        internal void SetCurrentNodeType(XmlNodeType nodeType)
        {
            mInsideElementNode = nodeType == XmlNodeType.Element;
        }

        /// <summary>
        /// Indicates whether a current parsing XML node has the Element type.
        /// </summary>
        private bool mInsideElementNode;
    }
}
