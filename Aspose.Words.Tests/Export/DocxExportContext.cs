// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2016 by Alexander Zhiltsov

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.Saving;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export.Docx
{
    /// <summary>
    /// Saves document as Docx or XLSX and allows searching for XML nodes in necessary part of document internal structure.
    /// </summary>
    internal class DocxExportContext : IXmlExportContext
    {
        /// <summary>
        /// Ctor that allows specifying output DOCX part and save options.
        /// </summary>
        internal DocxExportContext(Document doc, string documentPart, OoxmlSaveOptions saveOptions)
        {
            Debug.Assert(saveOptions.SaveFormat == SaveFormat.Docx);
            mCompliance = OoxmlComplianceInfo.GetCompliance(doc.ComplianceInfo, saveOptions);
            XmlDocument = SaveAndGetXmlDocument(doc, saveOptions, documentPart);
            mMappings = GetNamespaceMappings();
        }

        /// <summary>
        /// Ctor that allows specifying output DOCX part and compliance.
        /// </summary>
        internal DocxExportContext(Document doc, string documentPart, OoxmlComplianceCore compliance)
            : this(doc, documentPart, OoxmlSaveOptions.DocxWithCompliance(compliance))
        {
        }

        /// <summary>
        /// Ctor that generates output DOCX with default OOXML compliance.
        /// </summary>
        internal DocxExportContext(Document doc, string documentPart)
            : this(doc, documentPart, new OoxmlSaveOptions())
        {
        }

        /// <summary>
        /// Ctor that allows specifying document and compliance. The document.xml part is extracted.
        /// </summary>
        internal DocxExportContext(Document doc, OoxmlComplianceCore compliance)
            : this(doc, @"word/document.xml", compliance)
        {
        }

        /// <summary>
        /// Ctor that generates output DOCX and extracts document.xml part.
        /// </summary>
        internal DocxExportContext(Document doc)
            : this(doc, @"word/document.xml")
        {
        }

        /// <inheritdoc />
        public XmlNode GetXmlNode(XmlElement parentNode, string xpath)
        {
            return XmlUtilPal.SelectSingleNode(parentNode, xpath, mMappings);
        }

        /// <summary>
        /// Searches for a XML node from document root with using specified XPath query. First found node is returned.
        /// </summary>
        internal XmlNode GetXmlNode(string xpath)
        {
            return GetXmlNode(XmlDocument.DocumentElement, xpath);
        }

        /// <inheritdoc />
        public IList<XmlNode> GetXmlNodes(XmlElement element, string xpath)
        {
            return XmlUtilPal.SelectNodes(element, xpath, mMappings);
        }

        /// <summary>
        /// Searches for XML nodes from document root with using specified XPath query. Returns all found nodes.
        /// </summary>
        internal IList<XmlNode> GetXmlNodes(string xpath)
        {
            return GetXmlNodes(XmlDocument.DocumentElement, xpath);
        }

        /// <summary>
        /// Add custom namespace mapping
        /// </summary>
        internal void AddCustomNamespaceMapping(string key, string space)
        {
            mMappings.Add(key, space);
        }

        /// <summary>
        /// Returns necessary namespace mappings to parse output XML document.
        /// </summary>
        private IDictionary<string, string> GetNamespaceMappings()
        {
            bool isIsoStrict = mCompliance == OoxmlComplianceCore.IsoStrict;
            IDictionary<string, string> mappings = new Dictionary<string, string>();
            mappings.Add("w", DocxNamespaces.GetNamespace(DocxNamespace.Main, isIsoStrict));
            mappings.Add("a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));
            mappings.Add("w14", DocxNamespaces.GetNamespace(DocxNamespace.W14Markup, isIsoStrict));
            mappings.Add("wp", DocxNamespaces.GetNamespace(DocxNamespace.DrawingML, isIsoStrict));
            mappings.Add("wps", DocxNamespaces.GetNamespace(DocxNamespace.WordrocessingShape, isIsoStrict));
            mappings.Add("mc", DocxNamespaces.GetNamespace(DocxNamespace.MarkupCompatibility, isIsoStrict));
            mappings.Add("wp14", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLIso29500, isIsoStrict));
            mappings.Add("dsp", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram2008, isIsoStrict));
            mappings.Add("cx", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, isIsoStrict));
            mappings.Add("dgm", DmlNamespaceUtil.GetNamespace(DmlNodeType.Diagram, isIsoStrict));
            mappings.Add("cs", DmlExtensionsNamespace.ChartStyle);
            mappings.Add("v", NrxNamespaces.Vml);
            mappings.Add("lc", DmlNamespaceUtil.GetNamespace(DmlNodeType.LockedCanvas, isIsoStrict));
            mappings.Add("w16cid", DocxNamespaces.GetNamespace(DocxNamespace.W16Cid, isIsoStrict));
            mappings.Add("w16cex", DocxNamespaces.GetNamespace(DocxNamespace.W16Cex, isIsoStrict));
            mappings.Add("c", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChart, isIsoStrict));
            return mappings;
        }

        /// <summary>
        /// Saves the document using the specified save options, extracts the specified part and returns it as
        /// <see cref="System.Xml.XmlDocument"/> object.
        /// </summary>
        internal static XmlDocument SaveAndGetXmlDocument(Document doc, SaveOptions saveOptions,
            string returningPart)
        {
            XmlDocument[] xmlDocs = SaveAndGetXmlDocuments(doc, saveOptions, new string[] {returningPart});

            return xmlDocs[0];
        }

        /// <summary>
        /// Saves the document using the specified save options, extracts the specified parts and returns it as
        /// <see cref="System.Xml.XmlDocument"/> object array.
        /// </summary>
        internal static XmlDocument[] SaveAndGetXmlDocuments(Document doc, SaveOptions saveOptions,
            string[] returningParts)
        {
            MemoryStream[] requestedParts = SaveAndGetParts(doc, saveOptions, returningParts);
            XmlDocument[] xmlDocs = new XmlDocument[returningParts.Length];

            for(int i = 0; i < requestedParts.Length; ++i)
                xmlDocs[i] = XmlUtilPal.LoadXml(requestedParts[i], false);

            return xmlDocs;
        }

        /// <summary>
        /// Saves the document using the specified save options, extracts the specified parts and returns it
        /// as memory stream.
        /// </summary>
        internal static MemoryStream[] SaveAndGetParts(Document doc, SaveOptions saveOptions,
            string[] returningParts)
        {
            Debug.Assert((returningParts != null) && (returningParts.Length > 0));

            MemoryStream docxStream = new MemoryStream();
            doc.Save(docxStream, saveOptions);
            docxStream.Position = 0;

            MemoryStream[] docParts = new MemoryStream[returningParts.Length];
            MemoryStream docPartStream;
            int extractedXmlCount = 0;
            int partNameIndex;

            using (ZipReaderPal zipReader = new ZipReaderPal(docxStream))
            {
                while (zipReader.MoveToNextEntry())
                {
                    partNameIndex = GetPartNameIndex(zipReader.EntryFileName, returningParts);

                    // Indexes starts from 0.
                    if (partNameIndex > -1)
                    {
                        docPartStream = new MemoryStream();
                        zipReader.ExtractEntryToStream(docPartStream);

                        if (docPartStream.Length == 0)
                            Assert.Fail("Cannot extract document part.");

                        docPartStream.Position = 0;
                        docParts[partNameIndex] = docPartStream;
                        extractedXmlCount++;
                    }
                }
            }

            Assert.That(returningParts.Length, Is.EqualTo(extractedXmlCount), "Some of document parts were not found.");

            return docParts;
        }

        /// <summary>
        /// Searches index of the specified part in the collection of parts names.
        /// </summary>
        /// <param name="currentPart">Name of the part for search.</param>
        /// <param name="returningPart">Collection with names of parts.</param>
        /// <returns>Index of the part, otherwise -1.</returns>
        private static int GetPartNameIndex(string currentPart, string[] returningPart)
        {
            int partNameIndex = -1;

            for (int i = 0; i < returningPart.Length; ++i)
            {
                if (StringUtil.EqualsOrdinalIgnoreCase(currentPart, returningPart[i]))
                {
                    partNameIndex = i;
                    break;
                }
            }

            return partNameIndex;
        }

        /// <summary>
        /// Returns XML document of the Docx document part.
        /// </summary>
        public XmlDocument XmlDocument { get; }

        private readonly IDictionary<string, string> mMappings;
        private readonly OoxmlComplianceCore mCompliance;
    }
}
