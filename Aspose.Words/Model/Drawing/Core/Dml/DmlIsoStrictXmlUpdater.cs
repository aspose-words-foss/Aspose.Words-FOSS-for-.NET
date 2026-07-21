// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/05/2016 by Alexander Zhiltsov

using Aspose.Words.Nrx;
using Aspose.Xml;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Implements the <see cref="IXmlUpdater"/> interface to update XML that is rewritten from a source document
    /// to a destination. Now the class is used to change namespace URLs from ISO Transitional to ISO Strict or
    /// backward in DML extensions when destination OOXML format is changed. Additional XML updates may be implemented
    /// in the future.
    /// </summary>
    internal class DmlIsoStrictXmlUpdater : IXmlUpdater
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="toIsoStrict"> A flag indicating that destination XML should conform to 
        /// OOXML ISO Strict format.</param>
        internal DmlIsoStrictXmlUpdater(bool toIsoStrict)
        {
            mToIsoStrict = toIsoStrict;
        }

        /// <summary>
        /// Replaces namespace URLs from ISO Transitional to ISO Strict or backward depending on 
        /// the <see cref="mToIsoStrict"/> flag.
        /// </summary>
        public string ReplaceNamespace(string namespaceUrl)
        {
            return mToIsoStrict ? DocxNamespaces.ToIsoStrict(namespaceUrl) : DocxNamespaces.ToIsoTransitional(namespaceUrl);
        }

        private readonly bool mToIsoStrict;
    }
}
