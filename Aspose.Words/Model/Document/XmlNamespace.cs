// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2013 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Represents an individual schema within the Schema Library <see cref="XmlNamespaceCollection" />.
    /// </summary>
    internal class XmlNamespace
    {
        /// <summary>
        /// Creates schema object.
        /// </summary>
        /// <param name="uri">Target namespace for the XML Schema associated with this schema reference.</param>
        /// <param name="manifestLocation">Location of a supplementary XML file.</param>
        /// <param name="schemaLanguage">Media type or the root namespace of the schema language.</param>
        /// <param name="schemaLocation">Location of the XML schema file.</param>
        internal XmlNamespace(string uri, string manifestLocation, string schemaLanguage, string schemaLocation)
        {
            ArgumentUtil.CheckHasChars(uri, "uri");

            mUri = uri;
            mManifestLocation = manifestLocation;
            mSchemaLanguage = schemaLanguage;
            mSchemaLocation = schemaLocation;
        }

        /// <summary>
        /// Specifies the target namespace for the XML Schema associated with this schema reference.
        /// </summary>
        internal string Uri
        {
            get { return mUri; }
        }

        /// <summary>
        /// Specifies the location of a supplementary XML file which can be downloaded and parsed
        /// when this document is loaded in order to provide additional application-defined
        /// capabilities.
        /// </summary>
        internal string ManifestLocation
        {
            get { return mManifestLocation; }
        }

        /// <summary>
        /// Specifies the media type or the root namespace of the schema language.
        /// </summary>
        internal string SchemaLanguage
        {
            get { return mSchemaLanguage; }
        }

        /// <summary>
        /// Specifies the location of the XML schema file which should be downloaded and parsed
        /// when this document is loaded.
        /// </summary>
        internal string SchemaLocation
        {
            get { return mSchemaLocation; }
        }

        /// <summary>
        /// Attaches XML schema to document.
        /// </summary>
        internal void AttachToDocument(Document doc)
        {
            // AM. VBA like method. http://msdn.microsoft.com/en-us/library/office/ff191784.aspx Not sure this is really needed.
            doc.XmlNamespaces.Add(this);
            doc.XmlSchemaReferences.Add(this);
        }

        internal XmlNamespace Clone()
        {
            return (XmlNamespace)MemberwiseClone();
        }

        private readonly string mUri;
        private readonly string mManifestLocation;
        private readonly string mSchemaLanguage;
        private readonly string mSchemaLocation;
    }
}
