// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Xml;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class used to read DML diagrams layout definition.
    /// </summary>
    internal class DmlDiagramLayoutReader : DmlReaderBase
    {
        private DmlDiagramLayoutReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        internal static DmlDiagramLayout Read(DocxDocumentReaderBase reader, string relId)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            if (xmlReader.LocalName != "layoutDef")
                return null;

            DmlDiagramLayoutReader diagramLayoutReader = new DmlDiagramLayoutReader(reader);

            DmlDiagramLayout layout = new DmlDiagramLayout(relId);
            layout.UniqueId = xmlReader.ReadAttribute("uniqueId", "");
            layout.MinVersion = xmlReader.ReadAttribute("minVer", "");
            layout.DefaultStyle = xmlReader.ReadAttribute("defStyle", "");

            List<DmlDiagramString> titles = new List<DmlDiagramString>();
            List<DmlDiagramString> descriptions = new List<DmlDiagramString>();
            while (xmlReader.ReadChild("layoutDef"))
            {
                switch (xmlReader.LocalName)
                {
                    case "title":
                        titles.Add(DmlDiagramComplexTypesReader.ReadString(xmlReader));
                        break;
                    case "desc":
                        descriptions.Add(DmlDiagramComplexTypesReader.ReadString(xmlReader));
                        break;
                    case "catLst":
                        layout.Categories = DmlDiagramComplexTypesReader.ReadCategories(xmlReader);
                        break;
                    case "sampData":
                        layout.SampleData = diagramLayoutReader.ReadSampleData();
                        break;
                    case "styleData":
                        layout.StyleData = diagramLayoutReader.ReadSampleData();
                        break;
                    case "clrData":
                        layout.ColorTransformSampleData = diagramLayoutReader.ReadSampleData();
                        break;
                    case "layoutNode":
                        layout.LayoutNode = diagramLayoutReader.ReadLayoutNode();
                        break;
                    case "extLst":
                        layout.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
            layout.Titles = titles.ToArray();
            layout.Descriptions = descriptions.ToArray();

            return layout;
        }

        private DmlDiagramDataModel ReadSampleData()
        {
            string tagName = XmlReader.LocalName;
            bool useDefault = AnyXmlReader.ConvertToBool(XmlReader.ReadAttribute("useDef", ""));
            XmlReader.MoveToElement();

            DmlDiagramDataModel data = null;
            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "dataModel":
                        data = DmlDiagramDataReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            if (data != null)
                data.UseDefault = useDefault;

            return data;
        }

        private DmlDiagramLayoutNode ReadLayoutNode()
        {
            DmlDiagramLayoutNode node = new DmlDiagramLayoutNode();
            node.Name = XmlReader.ReadAttribute("name", "");
            node.StyleLabel = XmlReader.ReadAttribute("styleLbl", "");
            node.ChildOrder = DmlDiagramEnum.DmlToChildOrder(XmlReader.ReadAttribute("chOrder", "b"));
            node.MoveWith = XmlReader.ReadAttribute("moveWith", "");

            node.Content = new List<DmlDiagramLayoutNodeContentItem>();
            while (XmlReader.ReadChild("layoutNode"))
            {
                switch (XmlReader.LocalName)
                {
                    case "alg":
                        node.Content.Add(ReadAlgorithm());
                        break;
                    case "shape":
                        node.Content.Add(ReadShape());
                        break;
                    case "presOf":
                        node.Content.Add(ReadPresentation());
                        break;
                    case "constrLst":
                        node.Content.Add(ReadConstraintList());
                        break;
                    case "ruleLst":
                        node.Content.Add(ReadRuleList());
                        break;
                    case "varLst":
                        node.Content.Add(DmlDiagramComplexTypesReader.ReadLayoutVariableList(XmlReader));
                        break;
                    case "forEach":
                        node.Content.Add(ReadForEach());
                        break;
                    case "layoutNode":
                        node.Content.Add(ReadLayoutNode());
                        break;
                    case "choose":
                        node.Content.Add(ReadChoose());
                        break;
                    case "extLst":
                        node.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return node;
        }

        private DmlAlgorithm ReadAlgorithm()
        {
            DmlAlgorithm algorithm = new DmlAlgorithm();
            algorithm.Type = DmlDiagramEnum.DmlToAlgorithmType(XmlReader.ReadAttribute("type", ""));
            algorithm.Revision = XmlReader.ReadIntAttribute("rev", 0);

            while (XmlReader.ReadChild("alg"))
            {
                switch (XmlReader.LocalName)
                {
                    case "param":
                    {
                        DmlParameter parameter = ReadParameter();
                        // Experiments shows that MW uses only first parameter value of the specified type.
                        if (!algorithm.Params.ContainsKey(parameter.Type))
                            algorithm.Params.Add(parameter.Type, parameter.Value);
                        break;
                    }
                    case "extLst":
                        algorithm.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return algorithm;
        }

        private DmlParameter ReadParameter()
        {
            DmlParameter parameter = new DmlParameter();
            parameter.Type = XmlReader.ReadAttribute("type", "");
            parameter.Value = XmlReader.ReadAttribute("");
            return parameter;
        }

        private DmlDiagramShape ReadShape()
        {
            DmlDiagramShape shape = new DmlDiagramShape();
            shape.Type = XmlReader.ReadAttribute("type", "none");
            shape.Rotation = DmlAngle.FromDegrees(XmlReader.ReadDoubleAttribute("rot", 0));
            shape.ZOrder = XmlReader.ReadIntAttribute("zOrderOff", 0);
            shape.HideGeometry = XmlReader.ReadBoolAttribute("hideGeom", false);
            shape.PreventTextEditing = XmlReader.ReadBoolAttribute("lkTxEntry", false);
            shape.ImagePlaceholder = XmlReader.ReadBoolAttribute("blipPhldr", false);

            shape.BlipReference = XmlReader.ReadAttribute("blip", "");
            if (StringUtil.HasChars(shape.BlipReference))
            {
                Debug.WriteLine(shape.BlipReference);
                shape.BlipData = mDocumentReader.GetBinData(shape.BlipReference);
            }

            while (XmlReader.ReadChild("shape"))
            {
                switch (XmlReader.LocalName)
                {
                    case "adjLst":
                        shape.AdjustList = ReadAdjustList();
                        break;
                    case "extLst":
                        shape.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return shape;
        }

        private DmlShapeAdjust[] ReadAdjustList()
        {
            List<DmlShapeAdjust> list = new List<DmlShapeAdjust>();
            while (XmlReader.ReadChild("adjLst"))
            {
                switch (XmlReader.LocalName)
                {
                    case "adj":
                        list.Add(ReadAdjust());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return list.ToArray();
        }

        private DmlShapeAdjust ReadAdjust()
        {
            DmlShapeAdjust adjust = new DmlShapeAdjust();
            adjust.HandleIndex = XmlReader.ReadIntAttribute("idx", 0);
            adjust.Value = XmlReader.ReadDoubleAttribute(0);
            return adjust;
        }

        private DmlPresentationOf ReadPresentation()
        {
            DmlPresentationOf presentation = new DmlPresentationOf();
            ReadIteratorAttributes(presentation.IteratorAttributes);
            return presentation;
        }

        private void ReadIteratorAttributes(DmlIteratorAttributes attributes)
        {
            attributes.AxisType = DmlDiagramEnum.DmlToAxisTypeList(XmlReader.ReadAttribute("axis", "none"));
            attributes.PointType = DmlDiagramEnum.DmlToElementTypeList(XmlReader.ReadAttribute("ptType", "all"));
            attributes.HideLastTransition = DmlDiagramEnum.DmlToBoolList(XmlReader.ReadAttribute("hideLastTrans", "true"));
            attributes.Start = DmlDiagramEnum.DmlToIntList(XmlReader.ReadAttribute("st", "1"));
            attributes.Count = DmlDiagramEnum.DmlToIntList(XmlReader.ReadAttribute("cnt", "0"));
            attributes.Step = DmlDiagramEnum.DmlToIntList(XmlReader.ReadAttribute("step", "1"));
        }

        private DmlConstraintList ReadConstraintList()
        {
            DmlConstraintList result = new DmlConstraintList();

            result.Values = new List<DmlConstraint>();
            while (XmlReader.ReadChild("constrLst"))
            {
                switch (XmlReader.LocalName)
                {
                    case "constr":
                        result.Values.Add(ReadConstraint());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return result;
        }

        private DmlConstraint ReadConstraint()
        {
            DmlConstraint constraint = new DmlConstraint();
            constraint.Operator = DmlDiagramEnum.DmlToBooleanOperator(XmlReader.ReadAttribute("op", "none"));
            constraint.Value = ReadDoubleValue("val", 0);
            constraint.Factor = ReadDoubleValue("fact", 1);
            constraint.ConstraintAttributes = ReadConstraintAttributes();
            constraint.ConstraintReferenceAttributes = ReadConstraintReferenceAttributes();
            return constraint;
        }

        private DmlConstraintAttributes ReadConstraintReferenceAttributes()
        {
            DmlConstraintType type = DmlDiagramEnum.DmlToConstraintType(XmlReader.ReadAttribute("refType", "none"));
            DmlConstraintRelationship forAxis = DmlDiagramEnum.DmlToConstraintRelationship(XmlReader.ReadAttribute("refFor", "self"));
            string forName = XmlReader.ReadAttribute("refForName", "");
            DmlElementType pointType = DmlDiagramEnum.DmlToElementType(XmlReader.ReadAttribute("refPtType", "all"));
            return new DmlConstraintAttributes(type, forAxis, forName, pointType);
        }

        private DmlConstraintAttributes ReadConstraintAttributes()
        {
            DmlConstraintType type = DmlDiagramEnum.DmlToConstraintType(XmlReader.ReadAttribute("type", ""));
            DmlConstraintRelationship forAxis = DmlDiagramEnum.DmlToConstraintRelationship(XmlReader.ReadAttribute("for", "self"));
            string forName = XmlReader.ReadAttribute("forName", "");
            DmlElementType pointType = DmlDiagramEnum.DmlToElementType(XmlReader.ReadAttribute("ptType", "all"));
            return new DmlConstraintAttributes(type, forAxis, forName, pointType);
        }

        private DmlNumericRuleList ReadRuleList()
        {
            DmlNumericRuleList result = new DmlNumericRuleList();

            result.Values = new List<DmlNumericRule>();
            while (XmlReader.ReadChild("ruleLst"))
            {
                switch (XmlReader.LocalName)
                {
                    case "rule":
                        result.Values.Add(ReadRule());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return result;
        }

        private DmlNumericRule ReadRule()
        {
            DmlNumericRule rule = new DmlNumericRule();
            rule.Value = ReadDoubleValue("val", double.NaN);
            rule.Factor = ReadDoubleValue("fact", double.NaN);
            rule.Max = ReadDoubleValue("max", double.NaN);
            rule.ConstraintAttributes = ReadConstraintAttributes();
            return rule;
        }

        private double ReadDoubleValue(string name, double defaultValue)
        {
            string val = XmlReader.ReadAttribute(name, null);

            if (val == null)
                return defaultValue;

            if (string.Equals(val, "inf", StringComparison.OrdinalIgnoreCase))
                return double.PositiveInfinity;
            if (string.Equals(val, "-inf", StringComparison.OrdinalIgnoreCase))
                return double.NegativeInfinity;
            if (string.Equals(val, "nan", StringComparison.OrdinalIgnoreCase))
                return double.NaN;

            return FormatterPal.TryParseDoubleInvariant(val);
        }

        private DmlForEach ReadForEach()
        {
            DmlForEach forEach = new DmlForEach();
            forEach.Name = XmlReader.ReadAttribute("name", "");
            forEach.Reference = XmlReader.ReadAttribute("ref", "");
            ReadIteratorAttributes(forEach.IteratorAttributes);

            forEach.Content = new List<DmlDiagramLayoutNodeContentItem>();
            while (XmlReader.ReadChild("forEach"))
            {
                switch (XmlReader.LocalName)
                {
                    case "alg":
                        forEach.Content.Add(ReadAlgorithm());
                        break;
                    case "shape":
                        forEach.Content.Add(ReadShape());
                        break;
                    case "presOf":
                        forEach.Content.Add(ReadPresentation());
                        break;
                    case "constrLst":
                        forEach.Content.Add(ReadConstraintList());
                        break;
                    case "ruleLst":
                        forEach.Content.Add(ReadRuleList());
                        break;
                    case "forEach":
                        forEach.Content.Add(ReadForEach());
                        break;
                    case "layoutNode":
                        forEach.Content.Add(ReadLayoutNode());
                        break;
                    case "choose":
                        forEach.Content.Add(ReadChoose());
                        break;
                    case "extLst":
                        forEach.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return forEach;
        }

        private DmlChoose ReadChoose()
        {
            DmlChoose choose = new DmlChoose();
            choose.Name = XmlReader.ReadAttribute("name", "");

            choose.If = new List<DmlWhen>();
            while (XmlReader.ReadChild("choose"))
            {
                switch (XmlReader.LocalName)
                {
                    case "if":
                        choose.If.Add(ReadWhen());
                        break;
                    case "else":
                        choose.Else = ReadOtherwise();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return choose;
        }

        private DmlWhen ReadWhen()
        {
            DmlWhen when = new DmlWhen();
            when.Name = XmlReader.ReadAttribute("name", "");
            ReadIteratorAttributes(when.IteratorAttributes);
            when.Function = DmlDiagramEnum.DmlToFunctionType(XmlReader.ReadAttribute("func", ""));
            when.Argument = DmlDiagramEnum.DmlToVariableType(XmlReader.ReadAttribute("arg", "none"));
            when.Operator = DmlDiagramEnum.DmlToFunctionOperator(XmlReader.ReadAttribute("op", ""));
            when.Value = XmlReader.ReadAttribute("");

            when.Content = new List<DmlDiagramLayoutNodeContentItem>();
            while (XmlReader.ReadChild("if"))
            {
                switch (XmlReader.LocalName)
                {
                    case "alg":
                        when.Content.Add(ReadAlgorithm());
                        break;
                    case "shape":
                        when.Content.Add(ReadShape());
                        break;
                    case "presOf":
                        when.Content.Add(ReadPresentation());
                        break;
                    case "constrLst":
                        when.Content.Add(ReadConstraintList());
                        break;
                    case "ruleLst":
                        when.Content.Add(ReadRuleList());
                        break;
                    case "forEach":
                        when.Content.Add(ReadForEach());
                        break;
                    case "layoutNode":
                        when.Content.Add(ReadLayoutNode());
                        break;
                    case "choose":
                        when.Content.Add(ReadChoose());
                        break;
                    case "extLst":
                        when.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return when;
        }

        private DmlOtherwise ReadOtherwise()
        {
            DmlOtherwise otherwise = new DmlOtherwise();
            otherwise.Name = XmlReader.ReadAttribute("name", "");

            otherwise.Content = new List<DmlDiagramLayoutNodeContentItem>();
            while (XmlReader.ReadChild("else"))
            {
                switch (XmlReader.LocalName)
                {
                    case "alg":
                        otherwise.Content.Add(ReadAlgorithm());
                        break;
                    case "shape":
                        otherwise.Content.Add(ReadShape());
                        break;
                    case "presOf":
                        otherwise.Content.Add(ReadPresentation());
                        break;
                    case "constrLst":
                        otherwise.Content.Add(ReadConstraintList());
                        break;
                    case "ruleLst":
                        otherwise.Content.Add(ReadRuleList());
                        break;
                    case "forEach":
                        otherwise.Content.Add(ReadForEach());
                        break;
                    case "layoutNode":
                        otherwise.Content.Add(ReadLayoutNode());
                        break;
                    case "choose":
                        otherwise.Content.Add(ReadChoose());
                        break;
                    case "extLst":
                        otherwise.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return otherwise;
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
