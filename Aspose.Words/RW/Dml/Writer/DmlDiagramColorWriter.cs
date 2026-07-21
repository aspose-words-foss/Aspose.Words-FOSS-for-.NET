// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramColorWriter
    {
        internal static void Write(DmlDiagramColorTransform colorDef, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartDocument("dgm:colorsDef");
            builder.WriteAttribute("xmlns:dgm", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, isIsoStrict));
            builder.WriteAttribute("xmlns:a", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));

            builder.WriteAttributeIfNotDefault("uniqueId", colorDef.UniqueId, "");
            builder.WriteAttributeIfNotDefault("minVer", colorDef.MinVersion, "");

            DmlDiagramComplexTypesWriter.WriteStrings("dgm:title", colorDef.Titles, writer);
            DmlDiagramComplexTypesWriter.WriteStrings("dgm:desc", colorDef.Descriptions, writer);
            DmlDiagramComplexTypesWriter.WriteCategoryList(colorDef.Categories, writer);
            WriteStyleLabels(colorDef.StyleLabels, writer);

            DmlExtensionListWriter.Write("dgm", colorDef.Extensions, writer);

            builder.EndDocument();
        }

        private static void WriteStyleLabels(DmlColorTransformationStyleLabel[] styleLabels, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (DmlColorTransformationStyleLabel styleLabel in styleLabels)
            {
                builder.StartElement("dgm:styleLbl");
                builder.WriteAttribute("name", styleLabel.Name);

                WriteDiagramColors("dgm:fillClrLst", styleLabel.FillColorList, writer);
                WriteDiagramColors("dgm:linClrLst", styleLabel.LineColorList, writer);
                WriteDiagramColors("dgm:effectClrLst", styleLabel.EffectColorList, writer);
                WriteDiagramColors("dgm:txLinClrLst", styleLabel.TextLineColorList, writer);
                WriteDiagramColors("dgm:txFillClrLst", styleLabel.TextFillColorList, writer);
                WriteDiagramColors("dgm:txEffectClrLst", styleLabel.TextEffectColorList, writer);
                DmlExtensionListWriter.Write("dgm", styleLabel.Extensions, writer);

                builder.EndElement("dgm:styleLbl");
            }
        }

        private static void WriteDiagramColors(string tagName, DmlDiagramColors fillColorList, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(tagName);

            builder.WriteAttributeIfNotDefault("meth", DmlDiagramEnum.ColorApplicationMethodToDml(fillColorList.ColorApplicationMethod), "span");
            builder.WriteAttributeIfNotDefault("hueDir", DmlDiagramEnum.HueDirectionToDml(fillColorList.HueDirection), "cw");

            foreach (DmlColor color in fillColorList.Colors)
                DmlColorWriter.Write(color, writer);
            
            builder.EndElement(tagName);
        }
    }
}
