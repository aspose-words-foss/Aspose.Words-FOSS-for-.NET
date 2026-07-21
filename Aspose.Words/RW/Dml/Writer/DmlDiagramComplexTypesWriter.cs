// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/01/2014 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramComplexTypesWriter
    {
        internal static void WriteLayoutVariables(string tagName, DmlLayoutVariablePropertySet layoutPrSet, DocxDocumentWriterBase writer)
        {
            if (layoutPrSet == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            string prefix = tagName.Split(':')[0];
            
            builder.StartElement(tagName);

            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.OrgChart, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.ChildMax, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.ChildPref, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.BulletEnabled, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.Direction, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.HierBranchStyle, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.AnimOne, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.AnimLevel, layoutPrSet, builder, prefix);
            WriteLayoutVariableAttribute(DmlLayoutVariableAttr.ResizeHandles, layoutPrSet, builder, prefix);

            builder.EndElement(tagName);
        }

        private static void WriteLayoutVariableAttribute(
           DmlLayoutVariableAttr id,
           DmlLayoutVariablePropertySet props,
           DocxBuilder builder, 
           string prefix)
        {
            object val = props.GetDirectProperty(id);
            if (val == null)
                return;

            switch (id)
            {
                case DmlLayoutVariableAttr.OrgChart:
                    builder.WriteElementWithAttributes(string.Format("{0}:orgChart", prefix), "val", val);
                    break;
                case DmlLayoutVariableAttr.ChildMax:
                    builder.WriteElementWithAttributes(string.Format("{0}:chMax", prefix), "val",
                                                       ((DmlDiagramNodeCount)val).Value);
                    break;
                case DmlLayoutVariableAttr.ChildPref:
                    builder.WriteElementWithAttributes(string.Format("{0}:chPref", prefix), "val",
                                                       ((DmlDiagramNodeCount)val).Value);
                    break;
                case DmlLayoutVariableAttr.BulletEnabled:
                    builder.WriteElementWithAttributes(string.Format("{0}:bulletEnabled", prefix), "val", val);
                    break;
                case DmlLayoutVariableAttr.Direction:
                    builder.WriteElementWithAttributes(string.Format("{0}:dir", prefix), "val",
                                                       DmlDiagramEnum.DirectionToDml((DmlDiagramDirection)val));
                    break;
                case DmlLayoutVariableAttr.HierBranchStyle:
                    builder.WriteElementWithAttributes(string.Format("{0}:hierBranch", prefix), "val",
                                                       DmlDiagramEnum.HierBranchStyleToDml((DmlHierBranchStyle)val));
                    break;
                case DmlLayoutVariableAttr.AnimOne:
                    builder.WriteElementWithAttributes(string.Format("{0}:animOne", prefix), "val",
                                                       DmlDiagramEnum.AnimOneToDml((DmlAnimOne)val));
                    break;
                case DmlLayoutVariableAttr.AnimLevel:
                    builder.WriteElementWithAttributes(string.Format("{0}:animLvl", prefix), "val",
                                                       DmlDiagramEnum.AnimLevelToDml((DmlAnimLevel)val));
                    break;
                case DmlLayoutVariableAttr.ResizeHandles:
                    builder.WriteElementWithAttributes(string.Format("{0}:resizeHandles", prefix), "val",
                                                       DmlDiagramEnum.ResizeHandlesToDml((DmlResizeHandles)val));
                    break;
                default:
                    break;
            }
        }

        internal static void WriteStrings(string tagName, DmlDiagramString[] dmlDiagramStrings, DocxDocumentWriterBase writer)
        {
            if (dmlDiagramStrings == null || dmlDiagramStrings.Length == 0)
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            foreach (DmlDiagramString s in dmlDiagramStrings)
            {
                builder.StartElement(tagName);
                builder.WriteAttribute("lang", s.Language);
                builder.WriteAttributeString("val", s.Value);
                builder.EndElement(tagName);
            }
        }

        internal static void WriteCategoryList(DmlColorTransformCategory[] categories, DocxDocumentWriterBase writer)
        {
            if (categories == null || categories.Length == 0)
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            builder.StartElement("dgm:catLst");

            foreach (DmlColorTransformCategory category in categories)
                builder.WriteElementWithAttributes("dgm:cat", "type", category.CategoryType, "pri", category.Priority);

            builder.EndElement("dgm:catLst");
        }
    }
}
