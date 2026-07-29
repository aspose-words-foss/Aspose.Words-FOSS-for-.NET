// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlChartWriter
    {
        internal static void Write(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            bool isChartEx = dml.DmlNode.DmlNodeType == DmlNodeType.ChartEx;
            string prefix = isChartEx ? "cx" : "c";

            builder.StartElement(prefix + ":chart");
            builder.WriteAttribute("xmlns:" + prefix, DocxNamespaces.GetNamespace(
                isChartEx ? DocxNamespace.DrawingMLChartEx : DocxNamespace.DrawingMLChart, isIsoStrict));
            builder.WriteAttribute("xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));

            string chartRelType = DocxRelationshipTypes.GetType(
                isChartEx ? DocxRelationshipType.ChartEx : DocxRelationshipType.Chart, isIsoStrict);
            int chartPartNumber = writer.GetNextEmbeddedPartNumber(chartRelType);
            string chartPartName = string.Format("/word/charts/{0}{1}.xml", 
                isChartEx ? "chartEx" : "chart", chartPartNumber);

            string relId;
            DocxBuilder chartBuilder = writer.CreateChildPartAndBuilder(
                writer.CurrentBuilder.Part, 
                chartPartName,
                isChartEx ? DocxContentType.ChartEx : DocxContentType.Chart, 
                chartRelType, 
                out relId);

            builder.WriteAttribute("r:id", relId);

            writer.PushBuilder(chartBuilder);
            DmlChartSpaceWriter.Write(dml, writer);
            writer.PopBuilder();

            builder.EndElement(prefix + ":chart");
        }
    }
}
