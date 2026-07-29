// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramDataWriter
    {
        internal static void Write(DmlDiagramDataModel data, string drawingId, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartDocument("dgm:dataModel");
            builder.WriteAttribute("xmlns:dgm", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, isIsoStrict));
            builder.WriteAttribute("xmlns:a", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));

            WritePointList(data.PointList, writer);
            WriteConnectionList(data.ConnectionList, writer);
            WriteBackgroundFormatting(data.Background, writer);
            WriteWholeFormatting(data.WholeFormatting, writer);

            UpdateDrawingRelId(data, drawingId);
            DmlExtensionListWriter.Write("dgm", data.Extensions, writer);

            builder.EndDocument();
        }

        internal static void WriteSampleData(string rootTagName, DmlDiagramDataModel data, DocxDocumentWriterBase writer)
        {
            // WORDSNET-17927 According to OOXML spec "sampData", "styleData", "clrData" may be absent. So, skip these items when
            // appropriate properties are null.
            if (data != null)
            {
                DocxBuilder builder = writer.CurrentBuilder;

                builder.StartElement(rootTagName);
                builder.WriteAttributeIfTrue("useDef", data.UseDefault);

                builder.StartElement("dgm:dataModel");

                WritePointList(data.PointList, writer);
                WriteConnectionList(data.ConnectionList, writer);
                WriteBackgroundFormatting(data.Background, writer);
                WriteWholeFormatting(data.WholeFormatting, writer);

                builder.EndElement("dgm:dataModel");

                builder.EndElement(rootTagName);
            }
        }

        private static void WritePointList(DmlDiagramPoint[] pointList, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:ptLst");

            foreach (DmlDiagramPoint point in pointList)
                WritePoint(point, writer);

            builder.EndElement("dgm:ptLst");
        }

        private static void WritePoint(DmlDiagramPoint point, DocxDocumentWriterBase writer)
        {
            if (point.AlternateContentFallbackPoint == null)
            {
                WritePointCore(point, writer);
            }
            else
            {
                DocxBuilder builder = writer.CurrentBuilder;
                builder.StartElement("mc:AlternateContent");
                foreach (string attr in point.AlternateContentAttributes.Keys)
                    builder.WriteAttribute(attr, point.AlternateContentAttributes[attr]);

                builder.StartElement("mc:Choice");
                foreach (string attr in point.AlternateContentChoiceAttributes.Keys)
                    builder.WriteAttribute(attr, point.AlternateContentChoiceAttributes[attr]);

                WritePointCore(point, writer);
                builder.EndElement("mc:Choice");

                builder.StartElement("mc:Fallback");
                WritePointCore(point.AlternateContentFallbackPoint, writer);
                builder.EndElement("mc:Fallback");

                builder.EndElement("mc:AlternateContent");
            }
        }

        private static void WritePointCore(DmlDiagramPoint point, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:pt");

            builder.WriteAttribute("modelId", DmlDiagramEnum.ModelIdToDml(point.ModelId));
            builder.WriteAttributeIfNotDefault("type", DmlDiagramEnum.PointTypeToDml(point.Type), "node");
            builder.WriteAttributeIfNotDefault("cxnId", DmlDiagramEnum.ModelIdToDml(point.ConnectionId), "0");

            WritePropertySet(point.PropertySet, writer);
            DmlShapePropertiesWriter.Write("dgm", point.ShapeProperties, writer);
            DmlTextShapeWriter.WriteDmlShapeTextBody("dgm:t", point.TextBody, writer);
            DmlExtensionListWriter.Write("dgm", point.Extensions, writer);

            builder.EndElement("dgm:pt");
        }

        private static void WritePropertySet(DmlPropertySet prSet, DocxDocumentWriterBase writer)
        {
            if (prSet == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:prSet");

            DmlPropertySetPr dmlPropertySetPr = prSet.PrSet;

            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.PresAssocId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.PresName, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.PresStyleLbl, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.PresStyleIdx, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.PresStyleCnt, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.LoTypeId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.LoCatId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.QsTypeId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.QsCatId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CsTypeId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CsCatId, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.Coherent3DOff, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.PhldrT, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.Phldr, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustAng, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustFlipVert, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustFlipHor, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustSzX, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustSzY, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustScaleX, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustScaleY, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustT, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustLinFactX, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustLinFactY, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustLinFactNeighborX, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustLinFactNeighborY, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustRadScaleRad, dmlPropertySetPr, builder);
            WritePropertySetPrAttributeIfNotNull(DmlPropertySetAttr.CustRadScaleInc, dmlPropertySetPr, builder);

            DmlDiagramComplexTypesWriter.WriteLayoutVariables("dgm:presLayoutVars", prSet.LayoutVariablePropertySet, writer);
            DmlShapeStyleWriter.Write("dgm", prSet.Style, writer);

            builder.EndElement("dgm:prSet");
        }

        private static void WritePropertySetPrAttributeIfNotNull(
            DmlPropertySetAttr id,
            DmlPropertySetPr props,
            DocxBuilder builder)
        {
            object val = props.GetProperty(id);
            if (val == null)
                return;
            bool isIsoStrict = builder.OoxmlCompliance == OoxmlComplianceCore.IsoStrict;

            switch (id)
            {
                case DmlPropertySetAttr.PresAssocId:
                    builder.WriteAttribute("presAssocID", DmlDiagramEnum.ModelIdToDml((DmlModelId)val));
                    break;
                case DmlPropertySetAttr.PresName:
                    builder.WriteAttribute("presName", val);
                    break;
                case DmlPropertySetAttr.PresStyleLbl:
                    builder.WriteAttribute("presStyleLbl", val);
                    break;
                case DmlPropertySetAttr.PresStyleIdx:
                    builder.WriteAttribute("presStyleIdx", val);
                    break;
                case DmlPropertySetAttr.PresStyleCnt:
                    builder.WriteAttribute("presStyleCnt", val);
                    break;
                case DmlPropertySetAttr.LoTypeId:
                    builder.WriteAttribute("loTypeId", val);
                    break;
                case DmlPropertySetAttr.LoCatId:
                    builder.WriteAttribute("loCatId", val);
                    break;
                case DmlPropertySetAttr.QsTypeId:
                    builder.WriteAttribute("qsTypeId", val);
                    break;
                case DmlPropertySetAttr.QsCatId:
                    builder.WriteAttribute("qsCatId", val);
                    break;
                case DmlPropertySetAttr.CsTypeId:
                    builder.WriteAttribute("csTypeId", val);
                    break;
                case DmlPropertySetAttr.CsCatId:
                    builder.WriteAttribute("csCatId", val);
                    break;
                case DmlPropertySetAttr.Coherent3DOff:
                    builder.WriteAttribute("coherent3DOff", val);
                    break;
                case DmlPropertySetAttr.PhldrT:
                    builder.WriteAttribute("phldrT", val);
                    break;
                case DmlPropertySetAttr.Phldr:
                    builder.WriteAttribute("phldr", val);
                    break;
                case DmlPropertySetAttr.CustAng:
                    builder.WriteAttribute("custAng", val);
                    break;
                case DmlPropertySetAttr.CustFlipVert:
                    builder.WriteAttribute("custFlipVert", val);
                    break;
                case DmlPropertySetAttr.CustFlipHor:
                    builder.WriteAttribute("custFlipHor", val);
                    break;
                case DmlPropertySetAttr.CustSzX:
                    builder.WriteAttribute("custSzX", val);
                    break;
                case DmlPropertySetAttr.CustSzY:
                    builder.WriteAttribute("custSzY", val);
                    break;
                case DmlPropertySetAttr.CustScaleX:
                    builder.WriteAttribute("custScaleX", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustScaleY:
                    builder.WriteAttribute("custScaleY", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustT:
                    builder.WriteAttribute("custT", val);
                    break;
                case DmlPropertySetAttr.CustLinFactX:
                    builder.WriteAttribute("custLinFactX", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustLinFactY:
                    builder.WriteAttribute("custLinFactY", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustLinFactNeighborX:
                    builder.WriteAttribute("custLinFactNeighborX", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustLinFactNeighborY:
                    builder.WriteAttribute("custLinFactNeighborY", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustRadScaleRad:
                    builder.WriteAttribute("custRadScaleRad", ToPercent((int)val, isIsoStrict));
                    break;
                case DmlPropertySetAttr.CustRadScaleInc:
                    builder.WriteAttribute("custRadScaleInc", ToPercent((int)val, isIsoStrict));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Converts value in 1000th of percent to value that is appropriate to document format defined by 
        /// the <paramref name="isIsoStrict"/> parameter.
        /// </summary>
        private static string ToPercent(int value, bool isIsoStrict)
        {
            double asFraction = DmlPercentageUtil.FromDmlPercent(value);
            return DmlPercentageUtil.ToPercentOrDmlPercent(asFraction, isIsoStrict);
        }

        private static void WriteConnectionList(DmlDiagramConnection[] connectionList, DocxDocumentWriterBase writer)
        {
            if (connectionList == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:cxnLst");

            foreach (DmlDiagramConnection connection in connectionList)
                WriteConnection(connection, writer);

            builder.EndElement("dgm:cxnLst");
        }

        private static void WriteConnection(DmlDiagramConnection connection, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:cxn");

            builder.WriteAttribute("modelId", DmlDiagramEnum.ModelIdToDml(connection.ModelId));
            builder.WriteAttributeIfNotDefault("type", DmlDiagramEnum.ConnectionTypeToDml(connection.Type), "parOf");
            builder.WriteAttribute("srcId", DmlDiagramEnum.ModelIdToDml(connection.SourceId));
            builder.WriteAttribute("destId", DmlDiagramEnum.ModelIdToDml(connection.DestinationId));
            builder.WriteAttribute("srcOrd", connection.SourceOrder);
            builder.WriteAttribute("destOrd", connection.DestinationOrder);
            builder.WriteAttributeIfNotDefault("parTransId", DmlDiagramEnum.ModelIdToDml(connection.ParentTransitionId), "0");
            builder.WriteAttributeIfNotDefault("sibTransId", DmlDiagramEnum.ModelIdToDml(connection.SiblingTransitionId), "0");
            builder.WriteAttributeIfNotDefault("presId", connection.PresentationId, "");

            builder.EndElement("dgm:cxn");
        }

        private static void WriteBackgroundFormatting(DmlDiagramBackground background, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:bg");

            if (background.Fill.DmlFillType != DmlFillType.StyleFill)
                DmlFillWriter.Write(background.Fill, writer, false);

            DmlShapeEffectsWriter.Write(background.Effects, writer, false);

            builder.EndElement("dgm:bg");
        }

        private static void WriteWholeFormatting(DmlDiagramWholeFormatting wholeFormatting, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:whole");

            DmlOutlineWriter.Write("a:ln", wholeFormatting.Outline, writer);

            DmlShapeEffectsWriter.Write(wholeFormatting.Effects, writer, false);

            builder.EndElement("dgm:whole");
        }

        /// <summary>
        /// Updates drawingRelId inside dataModelExt extension.
        /// </summary>
        private static void UpdateDrawingRelId(DmlDiagramDataModel data, string drawingRelId)
        {
            if (!data.HasExtensions)
                return;

            // Update drawingRelId inside dataModelExt.
            if (StringUtil.HasChars(drawingRelId))
            {
                DmlExtension dataModelExt = data.Extensions[DmlExtensionUri.DataModelExt];

                if (dataModelExt != null)
                    dataModelExt.DrawingRelId = drawingRelId;
            }    
        }
    }
}
