// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramLayoutWriter
    {
        internal static void Write(DmlDiagramLayout layout, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartDocument("dgm:layoutDef");
            builder.WriteAttribute("xmlns:dgm", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, isIsoStrict));
            builder.WriteAttribute("xmlns:a", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));

            builder.WriteAttribute("uniqueId", layout.UniqueId);
            builder.WriteAttribute("minVer", layout.MinVersion);
            builder.WriteAttribute("defStyle", layout.DefaultStyle);

            DmlDiagramComplexTypesWriter.WriteStrings("dgm:title", layout.Titles, writer);
            DmlDiagramComplexTypesWriter.WriteStrings("dgm:desc", layout.Descriptions, writer);
            DmlDiagramComplexTypesWriter.WriteCategoryList(layout.Categories, writer);
            DmlDiagramDataWriter.WriteSampleData("dgm:sampData", layout.SampleData, writer);
            DmlDiagramDataWriter.WriteSampleData("dgm:styleData", layout.StyleData, writer);
            DmlDiagramDataWriter.WriteSampleData("dgm:clrData", layout.ColorTransformSampleData, writer);

            WriteLayoutNode(layout.LayoutNode, writer);
            DmlExtensionListWriter.Write("dgm", layout.Extensions, writer);

            builder.EndDocument();
        }

        private static void WriteLayoutNode(DmlDiagramLayoutNode layoutNode, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:layoutNode");
            builder.WriteAttribute("name", layoutNode.Name);
            builder.WriteAttribute("styleLbl", layoutNode.StyleLabel);
            if (layoutNode.ChildOrder != DmlChildOrder.Bottom)
                builder.WriteAttribute("chOrder", DmlDiagramEnum.ChildOrderToDml(layoutNode.ChildOrder));
            builder.WriteAttribute("moveWith", layoutNode.MoveWith);

            foreach (DmlDiagramLayoutNodeContentItem item in layoutNode.Content)
                WriteLayoutNodeContentItem(item, writer);

            DmlExtensionListWriter.Write("dgm", layoutNode.Extensions, writer);

            builder.EndElement("dgm:layoutNode");
        }

        private static void WriteLayoutNodeContentItem(DmlDiagramLayoutNodeContentItem item, DocxDocumentWriterBase writer)
        {
            switch (item.ContentItemType)
            {
                case DmlDiagramLayoutNodeContentItemType.Algorithm:
                    WriteAlgorithm((DmlAlgorithm)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.Choose:
                    WriteChoose((DmlChoose)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.ConstraintList:
                    WriteConstraintList((DmlConstraintList)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.LayoutNode:
                    WriteLayoutNode((DmlDiagramLayoutNode)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.Shape:
                    WriteShape((DmlDiagramShape)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.ForEach:
                    WriteForEach((DmlForEach)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.LayoutVariablePropertySet:
                    DmlDiagramComplexTypesWriter.WriteLayoutVariables("dgm:varLst", (DmlLayoutVariablePropertySet)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.NumericRuleList:
                    WriteNumericRuleList((DmlNumericRuleList)item, writer);
                    break;
                case DmlDiagramLayoutNodeContentItemType.PresentationOf:
                    WritePresentationOf((DmlPresentationOf)item, writer);
                    break;
                default:
                    throw new ArgumentException("Unexpected Diagram Layout Node Content Item Type");
            }
        }

        private static void WriteAlgorithm(DmlAlgorithm alg, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:alg");
            builder.WriteAttribute("type", DmlDiagramEnum.AlgorithmTypeToDml(alg.Type));
            builder.WriteAttributeIfNotZero("rev", alg.Revision);

            foreach (KeyValuePair<string, string> entry in alg.Params)
                builder.WriteElementWithAttributes("dgm:param", "type", entry.Key, "val", entry.Value);

            DmlExtensionListWriter.Write("dgm", alg.Extensions, writer);

            builder.EndElement("dgm:alg");
        }

        private static void WriteChoose(DmlChoose choose, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:choose");
            builder.WriteAttribute("name", choose.Name);

            WriteWhen(choose.If, writer);
            WriteOtherwise(choose.Else, writer);

            builder.EndElement("dgm:choose");
        }

        private static void WriteWhen(IEnumerable<DmlWhen> whens, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            foreach (DmlWhen when in whens)
            {
                builder.StartElement("dgm:if");
                builder.WriteAttribute("name", when.Name);
                WriteIteratorAttributes(when.IteratorAttributes, builder);
                builder.WriteAttribute("func", DmlDiagramEnum.FunctionTypeToDml(when.Function));
                builder.WriteAttribute("arg", DmlDiagramEnum.VariableTypeToDml(when.Argument));
                builder.WriteAttribute("op", DmlDiagramEnum.FunctionOperatorToDml(when.Operator));
                builder.WriteAttribute("val", when.Value);

                foreach (DmlDiagramLayoutNodeContentItem item in when.Content)
                    WriteLayoutNodeContentItem(item, writer);

                DmlExtensionListWriter.Write("dgm", when.Extensions, writer);

                builder.EndElement("dgm:if");
            }
        }

        private static void WriteOtherwise(DmlOtherwise otherwise, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            builder.StartElement("dgm:else");
            builder.WriteAttribute("name", otherwise.Name);

            foreach (DmlDiagramLayoutNodeContentItem item in otherwise.Content)
                WriteLayoutNodeContentItem(item, writer);

            DmlExtensionListWriter.Write("dgm", otherwise.Extensions, writer);

            builder.EndElement("dgm:else");
        }

        private static void WriteConstraintList(DmlConstraintList constrLst, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:constrLst");

            foreach (DmlConstraint value in constrLst.Values)
            {
                builder.StartElement("dgm:constr");
                WriteConstraintAttributes(value.ConstraintAttributes, builder);
                WriteConstraintRefAttributess(value.ConstraintReferenceAttributes, builder);
                builder.WriteAttributeIfNotDefault("op", DmlDiagramEnum.BooleanOperatorToDml(value.Operator), "none");
                if (!MathUtil.AreEqual(value.Value, 0d))
                    builder.WriteAttribute("val", GetDoubleValue(value.Value));
                if (!MathUtil.AreEqual(value.Factor, 1.0d))
                    builder.WriteAttribute("fact", GetDoubleValue(value.Factor));
                builder.EndElement("dgm:constr");
            }

            builder.EndElement("dgm:constrLst");
        }

        private static void WriteShape(DmlDiagramShape shape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:shape");
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            
            // It seems MS Word writes this value in degrease. So do the same.
            builder.WriteAttributeIfNotZero("rot", shape.Rotation.ValueInDegrees);
            builder.WriteAttributeIfNotDefault("type", shape.Type, "none");
            builder.WriteAttribute("xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));
            builder.WriteAttributeString("r:blip", shape.BlipReference);
            builder.WriteAttributeIfNotZero("zOrderOff", shape.ZOrder);
            builder.WriteAttributeIfTrue("hideGeom", shape.HideGeometry);
            builder.WriteAttributeIfTrue("lkTxEntry", shape.PreventTextEditing);
            builder.WriteAttributeIfTrue("blipPhldr", shape.ImagePlaceholder);

            if (shape.AdjustList != null)
            {
                builder.StartElement("dgm:adjLst");

                foreach (DmlShapeAdjust adjust in shape.AdjustList)
                    builder.WriteElementWithAttributes("dgm:adj", "idx", adjust.HandleIndex, "val", adjust.Value);

                builder.EndElement("dgm:adjLst");
            }

            DmlExtensionListWriter.Write("dgm", shape.Extensions, writer);

            builder.EndElement("dgm:shape");
        }

        private static void WriteForEach(DmlForEach forEach, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:forEach");
            builder.WriteAttribute("name", forEach.Name);
            builder.WriteAttribute("ref", forEach.Reference);
            WriteIteratorAttributes(forEach.IteratorAttributes, builder);

            foreach (DmlDiagramLayoutNodeContentItem item in forEach.Content)
                WriteLayoutNodeContentItem(item, writer);

            DmlExtensionListWriter.Write("dgm", forEach.Extensions, writer);

            builder.EndElement("dgm:forEach");
        }

        private static void WriteNumericRuleList(DmlNumericRuleList ruleLst, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dgm:ruleLst");

            foreach (DmlNumericRule rule in ruleLst.Values)
            {
                builder.StartElement("dgm:rule");
                WriteConstraintAttributes(rule.ConstraintAttributes, builder);
                builder.WriteAttribute("val", GetDoubleValue(rule.Value));
                builder.WriteAttribute("fact", GetDoubleValue(rule.Factor));
                builder.WriteAttribute("max", GetDoubleValue(rule.Max));
                builder.EndElement("dgm:rule");
            }

            builder.EndElement("dgm:ruleLst");
        }

        private static string GetDoubleValue(double val)
        {
            if (double.IsPositiveInfinity(val))
                return "INF";
            if (double.IsNegativeInfinity(val))
                return "-INF";
            if (double.IsNaN(val))
                return "NaN";

            return FormatterPal.DoubleToStr9Decimals(val);
        }

        private static void WritePresentationOf(DmlPresentationOf presOf, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            builder.StartElement("dgm:presOf");
            WriteIteratorAttributes(presOf.IteratorAttributes, builder);
            builder.EndElement("dgm:presOf");
        }

        private static void WriteIteratorAttributes(DmlIteratorAttributes attrs, DocxBuilder builder)
        {
            builder.WriteAttributeIfNotDefault("axis", DmlDiagramEnum.AxisTypeListToDml(attrs.AxisType), "none");
            builder.WriteAttributeIfNotDefault("ptType", DmlDiagramEnum.ElementTypeListToDml(attrs.PointType), "all");
            builder.WriteAttributeIfNotDefault("hideLastTrans", DmlDiagramEnum.BoolListToDml(attrs.HideLastTransition), "true");
            builder.WriteAttributeIfNotDefault("st", DmlDiagramEnum.IntListToDml(attrs.Start), "1");
            builder.WriteAttributeIfNotDefault("cnt", DmlDiagramEnum.IntListToDml(attrs.Count), "0");
            builder.WriteAttributeIfNotDefault("step", DmlDiagramEnum.IntListToDml(attrs.Step), "1");
        }

        private static void WriteConstraintAttributes(DmlConstraintAttributes attrs, DocxBuilder builder)
        {
            builder.WriteAttribute("type", DmlDiagramEnum.ConstraintTypeToDml(attrs.Type));
            builder.WriteAttributeIfNotDefault("for", DmlDiagramEnum.ConstraintRelationshipToDml(attrs.For), "self");
            builder.WriteAttribute("forName", attrs.ForName);
            builder.WriteAttributeIfNotDefault("ptType", DmlDiagramEnum.ElementTypeToDml(attrs.PointType), "all");
        }

        private static void WriteConstraintRefAttributess(DmlConstraintAttributes attrs, DocxBuilder builder)
        {
            builder.WriteAttributeIfNotDefault("refType", DmlDiagramEnum.ConstraintTypeToDml(attrs.Type), "none");
            builder.WriteAttributeIfNotDefault("refFor", DmlDiagramEnum.ConstraintRelationshipToDml(attrs.For), "self");
            builder.WriteAttribute("refForName", attrs.ForName);
            builder.WriteAttributeIfNotDefault("refPtType", DmlDiagramEnum.ElementTypeToDml(attrs.PointType), "all");
        }
    }
}
