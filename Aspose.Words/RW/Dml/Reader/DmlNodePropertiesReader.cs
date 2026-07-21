// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2011 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    internal class DmlNodePropertiesReader : DmlReaderBase
    {
        private DmlNodePropertiesReader()
        {
        }

        /// <summary>
        /// Reads the visual group shape properties.
        /// </summary>
        /// <remarks>
        /// 20.1.2.2.22 grpSpPr (Visual Group Shape Properties)
        /// This element specifies the properties that are to be common across all of the shapes
        /// within the corresponding group. If there are any conflicting properties within the
        /// group shape properties and the individual shape properties then the individual shape
        /// properties should take precedence.
        /// </remarks>
        internal static void ReadVisualGroupShapeProperties(DocxDocumentReaderBase reader, DmlCompositeNode node)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            if ((node.DmlNodeType == DmlNodeType.WordprocessingGroupShape) ||
                (node.DmlNodeType == DmlNodeType.GroupShape))
            {
                ((DmlGroupShape)node).BWMode = DmlEnum.DmlToBWMode(xmlReader.ReadAttribute("bwMode", "auto"));
            }

            while (xmlReader.ReadChild("grpSpPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "xfrm":
                        node.Transform = DmlTransformReader.ReadGroupTransform(xmlReader, complianceInfo);
                        break;
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "noFill":
                    case "pattFill":
                    case "solidFill":
                        node.Fill = DmlFillReader.Read(reader);
                        break;
                    case "effectLst":
                        node.Effects = DmlShapeEffectReader.ReadEffects(reader, false, false);
                        break;
                    case "effectDag":
                        node.Effects = DmlShapeEffectReader.ReadEffects(reader, false, true);
                        break;
                    case "scene3d":
                        node.Scene3DProperties = DmlScene3DReader.ReadScene3DProperties(reader, complianceInfo);
                        break;
                    case "extLst":
                        node.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the shape properties.
        /// </summary>
        /// <remarks>
        /// 20.1.2.2.35 spPr (Shape Properties)
        /// This element specifies the visual shape properties that can be applied to a shape.
        /// </remarks>
        internal static void ReadShapeProperties(DocxDocumentReaderBase reader, IDmlShapePrSource shape)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            // Read attributes.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "bwMode":
                        shape.BWMode = DmlEnum.DmlToBWMode(xmlReader.Value);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            while (xmlReader.ReadChild("spPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "custGeom":
                        shape.Geometry = DmlGeometryReader.Read(xmlReader, complianceInfo);
                        break;
                    case "xfrm":
                        shape.Transform = DmlTransformReader.ReadTransform(xmlReader, complianceInfo);
                        break;
                    case "prstGeom":
                        shape.Geometry = DmlGeometryReader.Read(xmlReader, complianceInfo);
                        break;
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "noFill":
                    case "pattFill":
                    case "solidFill":
                        ((IDmlCommonShapePrSource)shape).Fill = DmlFillReader.Read(reader);
                        break;
                    case "ln":
                        ((IDmlCommonShapePrSource)shape).Outline = DmlOutlineReader.Read(reader);
                        break;
                    case "effectLst":
                        shape.Effects = DmlShapeEffectReader.ReadEffects(reader, false, false);
                        break;
                    case "effectDag":
                        shape.Effects = DmlShapeEffectReader.ReadEffects(reader, false, true);
                        break;
                    case "scene3d":
                        shape.Scene3DProperties = DmlScene3DReader.ReadScene3DProperties(reader, complianceInfo);
                        break;
                    case "sp3d":
                        shape.Shape3DProperties = DmlScene3DReader.ReadShape3DProperties(reader, complianceInfo);
                        break;
                    case "extLst":
                        shape.SpPrExtensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// 20.1.2.2.25 nvCxnSpPr (Non-Visual Properties for a Connection Shape)
        /// 20.1.2.2.26 nvGraphicFramePr (Non-Visual Properties for a Graphic Frame)
        /// 20.1.2.2.27 nvGrpSpPr (Non-Visual Properties for a Group Shape)
        /// 20.1.2.2.28 nvPicPr (Non-Visual Properties for a Picture)
        /// 20.1.2.2.29 nvSpPr (Non-Visual Properties for a Shape)
        /// </summary>
        internal static void ReadNonVisualProperties(DmlNvPrBase nvPr, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "cNvCxnSpPr":
                    case "cNvGraphicFramePr":
                    case "cNvGrpSpPr":
                    case "cNvPicPr":
                    case "cNvSpPr":
                    case "cNvContentPartPr":
                        nvPr.CNvProperties = ReadNonVisualConnectorProperties(reader);
                        break;
                    case "cNvPr":
                        nvPr.NvDrawingProperties = ReadNonVisualDrawingProperties(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// 20.1.2.2.4 cNvCxnSpPr (Non-Visual Connector Shape Drawing Properties)
        /// 20.1.2.2.5 cNvGraphicFramePr (Non-Visual Graphic Frame Drawing Properties)
        /// 20.1.2.2.6 cNvGrpSpPr (Non-Visual Group Shape Drawing Properties)
        /// 20.1.2.2.7 cNvPicPr (Non-Visual Picture Drawing Properties)
        /// 20.1.2.2.9 cNvSpPr (Non-Visual Shape Drawing Properties)
        /// </summary>
        internal static DmlCnvPrBase ReadNonVisualConnectorProperties(DocxDocumentReaderBase docReader)
        {
            NrxXmlReader reader = docReader.XmlReader;
            DmlCnvPrBase pr = CreateConnectorProperties(reader.LocalName);

            // Read attributes.
            if (pr.Holder == DmlNvHolder.Shape)
                ((DmlCnvPrShape)pr).TextBox = reader.ReadBoolAttribute("txBox", false);

            if(pr.Holder == DmlNvHolder.Picture)
                ((DmlCnvPrPicture)pr).PreferRelativeResize = reader.ReadBoolAttribute("preferRelativeResize", true);

            if (pr.Holder == DmlNvHolder.ContentPart)
                ((DmlCnvPrContentPart)pr).IsComment = reader.ReadBoolAttribute("isComment", true);

            reader.MoveToElement();

            string tagName = reader.LocalName;
            while (reader.ReadChild(tagName))
            {
                switch (reader.LocalName)
                {
                    case "extLst":
                        pr.Extensions = DmlExtensionListReader.Read(docReader);
                        break;
                    case "cxnSpLocks":
                    case "graphicFrameLocks":
                    case "grpSpLocks":
                    case "picLocks":
                    case "spLocks":
                    case "cpLocks":
                        ReadShapeLocks(pr.Locks, docReader);
                        break;
                    case "stCxn":
                        ((DmlCnvPrConnectorShape)pr).ConnectionStart = ReadConnection(reader);
                        break;
                    case "endCxn":
                        ((DmlCnvPrConnectorShape)pr).ConnectionEnd = ReadConnection(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }

            return pr;
        }

        private static DmlConnection ReadConnection(NrxXmlReader reader)
        {
            DmlConnection connection  = new DmlConnection();

            // WORDSNET-23908 Identifiers of shape connectors have "unsignedInt" type.
            connection.Id = reader.ReadUIntAttribute("id", 0);
            connection.Index = reader.ReadUIntAttribute("idx", 0);

            reader.MoveToElement();
            reader.IgnoreElementNoWarn();
            return connection;
        }

        private static DmlCnvPrBase CreateConnectorProperties(string tagName)
        {
            switch (tagName)
            {
                case "cNvCxnSpPr":
                case "cNvCnPr": // This connector shape properties in ISO 29500.
                    return new DmlCnvPrConnectorShape();
                case "cNvFrPr":
                case "cNvGraphicFramePr":
                    return new DmlCnvPrGraphicFrame();
                case "cNvGrpSpPr":
                    return new DmlCnvPrGroupShape();
                case "cNvPicPr":
                    return new DmlCnvPrPicture();
                case "cNvSpPr":
                    return new DmlCnvPrShape();
                case "cNvContentPartPr":
                    return new DmlCnvPrContentPart();
                default:
                    throw new ArgumentException("Unexpected connector properties type.");
            }
        }

        /// <summary>
        /// Reads 'cNvPr' Non-Visual Drawing Properties.
        /// </summary>
        internal static DmlNvDrawingProperties ReadNonVisualDrawingProperties(DocxDocumentReaderBase reader)
        {
            DmlNvDrawingProperties nvPr = new DmlNvDrawingProperties();

            NrxXmlReader xmlReader = reader.XmlReader;
            // Read attributes.
            nvPr.Description = xmlReader.ReadAttribute("descr", "");
            nvPr.Name = xmlReader.ReadAttribute("name", "");
            nvPr.Title = xmlReader.ReadAttribute("title", "");
            nvPr.Hidden = xmlReader.ReadBoolAttribute("hidden", false);
            nvPr.Id = xmlReader.ReadIntAttribute("id", 0);

            while (xmlReader.ReadChild("cNvPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "hlinkClick":
                        nvPr.HlinkClick = DmlHlinkReader.Read(reader);
                        break;
                    case "extLst":
                        nvPr.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    case "hlinkHover":
                        WarnNotSupportedAndIgnoreElement(xmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return nvPr;
        }

        /// <summary>
        /// Reads Shape Locks.
        /// </summary>
        private static void ReadShapeLocks(DmlLocks locks, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            ReadLocks(locks, xmlReader);

            xmlReader.MoveToElement();

            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "extLst":
                        locks.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        private static void ReadLocks(DmlLocks locks, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "noAdjustHandles":
                        locks.AddLock(DmlLock.NoAdjustHandles, reader.ValueAsBool);
                        break;
                    case "noChangeArrowheads":
                        locks.AddLock(DmlLock.NoChangeArrowheads, reader.ValueAsBool);
                        break;
                    case "noChangeAspect":
                        locks.AddLock(DmlLock.NoChangeAspect, reader.ValueAsBool);
                        break;
                    case "noChangeShapeType":
                        locks.AddLock(DmlLock.NoChangeShapeType, reader.ValueAsBool);
                        break;
                    case "noEditPoints":
                        locks.AddLock(DmlLock.NoEditPoints, reader.ValueAsBool);
                        break;
                    case "noGrp":
                        locks.AddLock(DmlLock.NoGroup, reader.ValueAsBool);
                        break;
                    case "noUngrp":
                        locks.AddLock(DmlLock.NoUngroup, reader.ValueAsBool);
                        break;
                    case "noMove":
                        locks.AddLock(DmlLock.NoMove, reader.ValueAsBool);
                        break;
                    case "noResize":
                        locks.AddLock(DmlLock.NoResize, reader.ValueAsBool);
                        break;
                    case "noRot":
                        locks.AddLock(DmlLock.NoRotation, reader.ValueAsBool);
                        break;
                    case "noSelect":
                        locks.AddLock(DmlLock.NoSelect, reader.ValueAsBool);
                        break;
                    case "noCrop":
                        locks.AddLock(DmlLock.NoCrop, reader.ValueAsBool);
                        break;
                    case "noTextEdit":
                        locks.AddLock(DmlLock.NoTextEdit, reader.ValueAsBool);
                        break;
                    case "noDrilldown":
                        locks.AddLock(DmlLock.NoDrilldown, reader.ValueAsBool);
                        break;
                    default:
                        WarnUnexpected(reader);
                        break;
                }
            }
        }
    }
}
