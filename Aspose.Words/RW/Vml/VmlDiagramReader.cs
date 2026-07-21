// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2015 by Andrey Noskov

using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Vml
{
    internal class VmlDiagramReader : VmlShapeReaderBase
    {
        internal static void ReadDiagram(ShapeBase shape, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "ext": // v:ext
                        // Always "edit", ignore.
                        break;
                    case "dgmstyle":
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramStyle, reader.ValueAsInt);
                        break;
                    case "dgmscalex":
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramScaleX, reader.ValueAsInt);
                        break;
                    case "dgmscaley":
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramScaleY, reader.ValueAsInt);
                        break;
                    case "dgmfontsize":
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramFontSize, reader.ValueAsInt);
                        break;
                    case "constrainbounds":
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramConstrainBounds, VmlUtil.VmlToIntArray(reader.Value));
                        break;
                    case "autoformat":
                        SetBoolAttribute(shape, ShapeAttr.DiagramAutoFormat, reader.Value);
                        break;
                    case "reverse":
                        SetBoolAttribute(shape, ShapeAttr.DiagramReverse, reader.Value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }

            while (reader.ReadChild("diagram"))
            {
                if (reader.LocalName == "relationtable") // o:relationtable
                    ReadDiagramRelationTable(shape, reader);
            }
        }

        private static void ReadDiagramRelationTable(ShapeBase shape, NrxXmlReader reader)
        {
            // Attribute "v:ext" is always "edit", ignore.

            // Read "o:rel" child elements.

            List<DiagramNodeRelation> relationsList = new List<DiagramNodeRelation>();

            while (reader.ReadChild("relationtable"))
            {
                if (reader.LocalName == "rel") // o:rel
                    relationsList.Add(ReadDiagramNodeRelation(reader));
            }

            DiagramNodeRelation[] relations = relationsList.ToArray();
            shape.SetShapeAttrInternal(ShapeAttr.DiagramRelationsTable, relations);
        }

        private static DiagramNodeRelation ReadDiagramNodeRelation(NrxXmlReader reader)
        {
            DiagramNodeRelation relation = new DiagramNodeRelation();

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "ext":
                        // Always "edit", ignore.
                        break;
                    case "idsrc":
                        relation.A = VmlUtil.CalculateDiagramHash(value);
                        break;
                    case "iddest":
                        relation.B = VmlUtil.CalculateDiagramHash(value);
                        break;
                    case "idcntr":
                        relation.C = VmlUtil.CalculateDiagramHash(value);
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }

            return relation;
        }
    }
}
