// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2014 by Andrey Noskov

using System;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Xml;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Container class for Office Drawing Extensions to Office Open XML Structure.
    /// </summary>
    internal class DmlExtension
    {
        internal DmlExtension(string uri, string xmlDoc)
        {
            mUri = uri;
            mXmlDoc = xmlDoc;
        }

        /// <summary>
        /// Clones this <see cref="DmlExtension"/> object.
        /// </summary>
        internal virtual DmlExtension Clone()
        {
            DmlExtension lhs = (DmlExtension)MemberwiseClone();

            if (mNvPr != null)
                lhs.mNvPr = mNvPr.Clone();

            if (mDmlFillPr != null)
                lhs.FillPr = mDmlFillPr.Clone();

            if (mOutlinePr != null)
                lhs.mOutlinePr = mOutlinePr.Clone();

            if (mWebVideoPr != null)
                lhs.mWebVideoPr = mWebVideoPr.Clone();

            if (mDataLabelPr != null)
                lhs.mDataLabelPr = mDataLabelPr.Clone();

            if (mDatalabelsRangeData != null)
                lhs.mDatalabelsRangeData = mDatalabelsRangeData.Clone();

            if (mSvgBlip != null)
                lhs.mSvgBlip = mSvgBlip.Clone();

            return lhs;
        }

        /// <summary>
        /// Searches for an extension by its URI in the specified dictionary and creates it if not found.
        /// </summary>
        internal static DmlExtension GetOrCreateExtension(StringToObjDictionary<DmlExtension> extensions, string uri)
        {
            DmlExtension extension = extensions[uri];
            if (extension == null)
            {
                extension = new DmlExtension(uri, null);
                extensions[uri] = extension;
            }

            return extension;
        }

        /// <summary>
        /// Creates svgBlip extension with the specified svg image bytes. 
        /// </summary>
        internal static DmlExtension CreateSvgBlipExtension(byte[] svgImageBytes, DocumentBase doc)
        {
            DmlBlip svgBlip = new DmlBlip();
            svgBlip.Document = doc;
            svgBlip.EmbedImage = svgImageBytes;
            DmlExtension svgBlipExt = new DmlExtension(DmlExtensionUri.SvgBlip, null);
            svgBlipExt.SvgBlip = svgBlip;
            return svgBlipExt;
        }

        /// <summary>
        /// Returns XML converted to ISO Transitional or ISO Strict format depending on the 'asIsoStrict' parameter.
        /// </summary>
        internal string GetXmlToWrite(OoxmlComplianceCore compliance)
        {
            AnyXmlReader xmlReader = new AnyXmlReader(mXmlDoc, null);

            bool asIsoStrict = (compliance == OoxmlComplianceCore.IsoStrict);

            bool isSourceIsoStrict = xmlReader.NamespaceURI != DocxNamespaces.ToIsoTransitional(xmlReader.NamespaceURI);
            bool isSourceIsoTransitional = xmlReader.NamespaceURI != DocxNamespaces.ToIsoStrict(xmlReader.NamespaceURI);

            // If format of stored source XML is recognized and we write into the same format then just return
            // the stored XML.
            if ((isSourceIsoStrict != isSourceIsoTransitional) && (isSourceIsoStrict == asIsoStrict))
                return mXmlDoc;

            return xmlReader.ReadOuterXml(new DmlIsoStrictXmlUpdater(asIsoStrict), null);
        }

        /// <summary>
        /// Raw XML string that represents DrawingML extension.
        /// </summary>
        /// <remarks>
        /// We need this to provide round-trip of unknown extensions.
        /// The root element of this XML is a:extLst.
        /// </remarks>
        internal string XmlDoc
        {
            get { return mXmlDoc; }
            set { mXmlDoc = value; }
        }

        /// <summary>
        /// Uri of this DrawingML extension. Each extension has an Uri attribute, which serves as an identifier
        /// to indicate information about the extension.
        /// </summary>
        internal string Uri
        {
            get { return mUri; }
            set { mUri = value; }
        }

        /// <summary>
        /// DrawingML fill properties.
        /// </summary>
        internal DmlFill FillPr
        {
            get { return mDmlFillPr; }
            set { mDmlFillPr = value; }
        }

        /// <summary>
        /// Represents SpPr element in charts.
        /// </summary>
        internal DmlChartSpPr DmlChartSpPr
        {
            get { return mDmlChartSpPr; }
            set { mDmlChartSpPr = value; }
        }

        /// <summary>
        /// Represents content of the outline properties.
        /// </summary>
        /// <remarks>
        ///  Stores the line information of an object when the line fill
        ///  has been set to invisible (on VML->DML conversion).
        /// </remarks>
        internal DmlOutline OutlinePr
        {
            get { return mOutlinePr; }
            set { mOutlinePr = value; }
        }

        /// <summary>
        /// Non-visual drawing properties.
        /// </summary>
        internal DmlNvDrawingProperties NvPr
        {
            get { return mNvPr; }
            set { mNvPr = value; }
        }

        /// <summary>
        /// Indicating that the local BLIP compression setting overrides the document default compression setting.
        /// </summary>
        internal bool UseLocalDpi
        {
            get { return mUseLocalDpi; }
            set { mUseLocalDpi = value; }
        }

        /// <summary>
        /// Element that specifies the properties for displaying an online video to the user.
        /// </summary>
        internal DmlWebVideoProperties WebVideoPr
        {
            get { return mWebVideoPr; }
            set { mWebVideoPr = value; }
        }

        /// <summary>
        /// Chart data label properties.
        /// </summary>
        internal DmlChartDataLabelPr DataLabelPr
        {
            get { return mDataLabelPr; }
            set { mDataLabelPr = value; }
        }

        /// <summary>
        /// Specifies the data for the data labels. 
        /// </summary>
        internal DmlChartDataSource DataLabelsRangeData
        {
            get
            {
                if (mDatalabelsRangeData == null)
                    mDatalabelsRangeData = new DmlChartDataSource();

                return mDatalabelsRangeData;
            }
        }

        /// <summary>
        /// Specifies unique identifier of a data label. 
        /// </summary>
        internal Guid DataLabelId
        {
            get { return mDataLabelId; }
            set { mDataLabelId = value; }
        }

        /// <summary>
        /// Defines the explicit part location of the Diagram Drawing and the minimum application 
        /// version required to layout this diagram.
        /// </summary>
        internal string DrawingRelId
        {
            get { return mDrawingRelId; }
            set { mDrawingRelId = value; }
        }

        /// <summary>
        /// Specifies a graphic element in Scalable Vector Graphics (SVG) format.
        /// https://msdn.microsoft.com/en-us/library/mt765155(v=office.12).aspx
        /// </summary>
        internal DmlBlip SvgBlip
        {
            get { return mSvgBlip; }
            set { mSvgBlip = value; }
        }

        /// <summary>
        /// Stores information about embedded images, which placed in a non-prased xml content. 
        /// </summary>
        internal SortedStringListGeneric<byte[]> EmbeddedImages
        {
            get { return mEmbeddedImages; }
        }

        /// <summary>
        /// Indicates that the shape is decorative.
        /// </summary>
        internal bool Decorative
        {
            get { return mDecorative; }
            set { mDecorative = value; }
        }

        private string mUri;
        private string mXmlDoc;
        private DmlFill mDmlFillPr;
        private DmlChartSpPr mDmlChartSpPr;
        private bool mUseLocalDpi;
        private DmlOutline mOutlinePr;
        private DmlNvDrawingProperties mNvPr;
        private DmlWebVideoProperties mWebVideoPr;
        private DmlChartDataLabelPr mDataLabelPr;
        private DmlChartDataSource mDatalabelsRangeData;
        private string mDrawingRelId;
        private DmlBlip mSvgBlip;
        private Guid mDataLabelId = Guid.Empty;
        private bool mDecorative;

        /// <summary>
        /// Maps relations id with the binary data of embedded images.
        /// </summary>
        private readonly SortedStringListGeneric<byte[]> mEmbeddedImages = new SortedStringListGeneric<byte[]>();

    }
}
