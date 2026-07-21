// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2007 by Vladimir Averkin

using System.Collections.Generic;
using System.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Encapsulates an XML reader together with a package part.
    /// </summary>
    internal class DocxXmlReader : NrxXmlReader
    {
        internal DocxXmlReader(OpcPackagePart part, OoxmlComplianceInfo cInfo)
            : this(part, null, WarningSource.Docx, cInfo)
        {
        }

        internal DocxXmlReader(OpcPackagePart part, IWarningCallback warningCallback, WarningSource warningSource, OoxmlComplianceInfo cInfo)
            : base(part.Stream, warningCallback, warningSource)
        {
            Initialize(part, cInfo);
        }

        internal DocxXmlReader(Stream stream, OoxmlComplianceInfo cInfo) :
            this(stream, null, cInfo)
        {
        }

        internal DocxXmlReader(Stream stream, IWarningCallback warningCallback, OoxmlComplianceInfo cInfo) :
            base(stream, warningCallback, WarningSource.Docx)
        {
            Initialize(null, cInfo);
        }

        internal DocxXmlReader(OpcPackagePart part, OoxmlComplianceInfo cInfo, string xml,
            IDictionary<string, string> namespaces)
            : base(xml, namespaces)
        {
            Initialize(part, cInfo);
        }

        /// <summary>
        /// Defined in <see cref="NrxXmlReader"/>, allows to share the implementation of reading borders.
        /// </summary>
        protected override LineStyle ReadLineStyle()
        {
            return DocxEnum.DocxToLineStyle(Value);
        }

        /// <summary>
        /// Initializes state of the reader.
        /// </summary>
        private void Initialize(OpcPackagePart part, OoxmlComplianceInfo cInfo)
        {
            mPart = part;
            mComplianceInfo = cInfo;
        }

        /// <summary>
        /// The OPC part that this reader is reading.
        /// </summary>
        internal OpcPackagePart Part
        {
            get { return mPart; }
        }

        internal OoxmlComplianceInfo ComplianceInfo
        {
            get { return mComplianceInfo; }
        }

        private OpcPackagePart mPart;
        private OoxmlComplianceInfo mComplianceInfo;
    }
}
