// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramStyleWriter
    {
        internal static void Write(DmlDiagramStyleDefinition styleDef, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartDocument("dgm:styleDef");
            builder.WriteAttribute("xmlns:dgm", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, isIsoStrict));
            builder.WriteAttribute("xmlns:a", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));

            builder.WriteAttribute("uniqueId", styleDef.UniqueId);
            builder.WriteAttribute("minVer", styleDef.MinVersion);

            DmlDiagramComplexTypesWriter.WriteStrings("dgm:title", styleDef.Titles, writer);
            DmlDiagramComplexTypesWriter.WriteStrings("dgm:desc", styleDef.Descriptions, writer);
            DmlDiagramComplexTypesWriter.WriteCategoryList(styleDef.Categories, writer);
            Dml3DPropertiesWriter.WriteScene3D("dgm", styleDef.Scene3D, writer, false);
            WriteStyleLabels(styleDef.StyleLabels, writer);

            DmlExtensionListWriter.Write("dgm", styleDef.Extensions, writer);

            builder.EndDocument();
        }

        private static void WriteStyleLabels(DmlStyleLabel[] dmlStyleLabel, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (DmlStyleLabel style in dmlStyleLabel)
            {
                builder.StartElement("dgm:styleLbl");
                builder.WriteAttributeString("name", style.Name);

                Dml3DPropertiesWriter.WriteScene3D("dgm", style.Scene3D, writer, false);
                Dml3DPropertiesWriter.WriteShape3D("dgm", style.Shape3D, writer, false);
                WriteTxPr(style.TextProperties, writer);
                DmlShapeStyleWriter.Write("dgm", style.Style, writer);

                DmlExtensionListWriter.Write("dgm", style.Extensions, writer);

                builder.EndElement("dgm:styleLbl");
            }
        }

        private static void WriteTxPr(DmlDiagramTextPr txPr, DocxDocumentWriterBase writer)
        {
            if (txPr == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:txPr");

            if (txPr.FlatText != null)
                builder.WriteElementWithAttributes("dgm:flatTx", "z", txPr.FlatText.ZCoordinate);

            Dml3DPropertiesWriter.WriteShape3D("a", txPr.Shape3DProperties, writer, false);

            builder.EndElement("dgm:txPr");
        }
    }
}
