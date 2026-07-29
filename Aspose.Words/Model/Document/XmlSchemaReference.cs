// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2013 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Represents an individual schema that is attached to a document.
    /// </summary>
    internal class XmlSchemaReference
    {
        internal XmlSchemaReference(string uri, string location)
        {
            mUri = uri;
            mLocation = location;
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
        internal string Location
        {
            get { return mLocation; }
        }

        internal XmlSchemaReference Clone()
        {
            return (XmlSchemaReference)MemberwiseClone();
        }

        private readonly string mUri;
        private readonly string mLocation;
    }
}
