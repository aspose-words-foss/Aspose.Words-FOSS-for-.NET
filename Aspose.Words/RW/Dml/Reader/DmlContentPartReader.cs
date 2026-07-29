// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/26/2014 by Alexey Noskov

using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    internal class DmlContentPartReader : DmlReaderBase
    {
        private DmlContentPartReader()
        {
        }

        internal static Shape Read(DocxDocumentReaderBase reader)
        {
            string tagName = reader.XmlReader.LocalName;
            if (tagName != "contentPart")
                return null;

            Shape dml = new Shape(reader.Document, ShapeMarkupLanguage.Dml);
            reader.AddAndPushContainer(dml);
            dml.DmlNode = ReadCore(reader);
            reader.PopContainer(NodeType.Shape);

            return dml;
        }

        private static DmlContentPart ReadCore(DocxDocumentReaderBase reader)
        {
            DmlContentPart contentPart = new DmlContentPart();

            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            contentPart.BWMode = DmlEnum.DmlToBWMode(xmlReader.ReadAttribute("bwMode", "auto"));

            // Keep content of content part unparsed.
            string contentPartId = xmlReader.ReadAttribute("id", "");
            contentPart.ContentPart = reader.FetchPartByName(reader.GetRelationshipTarget(contentPartId));

            // Also store unparsed in the model all internal child parts that are related to the theme.
            foreach (OpcRelationship rel in contentPart.ContentPart.Rels)
            {
                if (!rel.IsExternal)
                {
                    OpcPackagePart part = reader.FetchPartByName(contentPart.ContentPart.GetRelatedPartName(rel));
                    contentPart.RelatedParts[rel.Id] = part;
                }
            }

            xmlReader.MoveToElement();

            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "nvContentPartPr":
                    {
                        if (contentPart.NonVisualPr == null)
                            contentPart.NonVisualPr = new DmlNvPrContentPart();
                        DmlNodePropertiesReader.ReadNonVisualProperties(contentPart.NonVisualPr, reader);
                        break;
                    }
                    case "xfrm":
                        contentPart.Transform = DmlTransformReader.ReadTransform(xmlReader, complianceInfo);
                        break;
                    case "extLst":
                        contentPart.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    case "nvPr":
                        WarnNotSupportedAndIgnoreElement(xmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return contentPart;
        }
    }
}
