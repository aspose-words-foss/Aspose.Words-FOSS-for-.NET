// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2008 by Roman Korchagin

using System.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Reads common DrawingML elements from the 5.1.2 Basics section of the OOXML specification.
    /// </summary>
    internal static class DmlBasicsReader
    {
        /// <summary>
        /// Reads an element that contains two attributes x and y with integer values.
        /// </summary>
        internal static Point ReadXY(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            int x = 0;
            int y = 0;

            // Has attributes only.
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "x":
                        x = MathUtil.DoubleToInt(reader.GetValueAsEmus(complianceInfo));
                        break;
                    case "y":
                        y = MathUtil.DoubleToInt(reader.GetValueAsEmus(complianceInfo));
                        break;
                    default:
                        reader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, reader.LocalName));
                        break;
                }
            }

            return new Point(x, y);
        }

        /// <summary>
        /// Reads an element that contains two attributes cx and cy with integer values.
        /// </summary>
        internal static Size ReadCxCy(NrxXmlReader reader, OoxmlComplianceInfo complianceInfo)
        {
            int cx = 0;
            int cy = 0;

            // Has attributes only.
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "cx":
                        cx = MathUtil.DoubleToInt(reader.GetValueAsEmus(complianceInfo));
                        break;
                    case "cy":
                        cy = MathUtil.DoubleToInt(reader.GetValueAsEmus(complianceInfo));
                        break;
                    default:
                        reader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, reader.LocalName));
                        break;
                }
            }

            return new Size(cx, cy);
        }

        internal static Rectangle ReadRect(DocxXmlReader reader)
        {
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "l":
                        left = NrxXmlReader.XmlToPercent(reader.Value, reader.ComplianceInfo);
                        break;
                    case "t":
                        top = NrxXmlReader.XmlToPercent(reader.Value, reader.ComplianceInfo);
                        break;
                    case "r":
                        right = NrxXmlReader.XmlToPercent(reader.Value, reader.ComplianceInfo);
                        break;
                    case "b":
                        bottom = NrxXmlReader.XmlToPercent(reader.Value, reader.ComplianceInfo);
                        break;
                    default:
                        reader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, reader.LocalName));
                        break;
                }
            }
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        /// <summary>
        /// Reads 5.1.2.1.19 graphicFrameLocks (Graphic Frame Locks).
        /// Reads 5.1.2.1.31 picLocks (Picture Locks).
        /// 
        /// This element specifies all locking properties for a graphic frame. These properties 
        /// inform the generating application about specific properties that have been previously 
        /// locked and thus should not be changed.
        /// </summary>
        internal static void ReadLocks(NrxXmlReader reader, ShapePr shapePr)
        {
            // Read attributes.
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "noAdjustHandles":
                        // picLocks
                        shapePr.SetAttr(ShapeAttr.LockAdjustHandles, reader.ValueAsBool);
                        break;
                    case "noChangeArrowheads":
                        // picLocks
                        // Not in VML.
                        shapePr.SetAttr(ShapeAttr.DmlLockArrowHeads, reader.ValueAsBool);
                        break;
                    case "noChangeAspect":
                        // picLocks
                        // graphicFrameLocks
                        shapePr.SetAttr(ShapeAttr.LockAspectRatio, reader.ValueAsBool);
                        break;
                    case "noDrilldown":
                        // graphicFrameLocks
                        // Not in VML.
                        shapePr.SetAttr(ShapeAttr.DmlLockDrillDown, reader.ValueAsBool);
                        break;
                    case "noChangeShapeType":
                        // picLocks
                        shapePr.SetAttr(ShapeAttr.LockShapeType, reader.ValueAsBool);
                        break;
                    case "noCrop":
                        // picLocks
                        shapePr.SetAttr(ShapeAttr.LockCropping, reader.ValueAsBool);
                        break;
                    case "noEditPoints":
                        // picLocks
                        shapePr.SetAttr(ShapeAttr.LockVertices, reader.ValueAsBool);
                        break;
                    case "noGrp":
                        // picLocks
                        // graphicFrameLocks
                        shapePr.SetAttr(ShapeAttr.LockAgainstGrouping, reader.ValueAsBool);
                        break;
                    case "noMove":
                        // picLocks
                        // graphicFrameLocks
                        shapePr.SetAttr(ShapeAttr.LockPosition, reader.ValueAsBool);
                        break;
                    case "noResize":
                        // picLocks
                        // graphicFrameLocks
                        // Not in VML.
                        shapePr.SetAttr(ShapeAttr.DmlLockResize, reader.ValueAsBool);
                        break;
                    case "noRot":
                        // picLocks
                        shapePr.SetAttr(ShapeAttr.LockRotation, reader.ValueAsBool);
                        break;
                    case "noSelect":
                        // picLocks
                        // graphicFrameLocks
                        shapePr.SetAttr(ShapeAttr.LockAgainstSelect, reader.ValueAsBool);
                        break;
                    case "a":
                        // This is xmlns:a attribute, ignore.
                        break;
                    default:
                        reader.Warn(WarningType.UnexpectedContent, WarningSource.DrawingML, string.Format(WarningStrings.UnexpectedTagOrAttribute, reader.LocalName));
                        break;
                }
            }

            // Read elements.
            reader.MoveToElement();
            string tagName = reader.LocalName;
            while (reader.ReadChild(tagName))
            {
                switch (reader.LocalName)
                {
                    case "extLst":
                        // Not supported.
                        break;
                    default:
                        Debug.Fail(reader.LocalName);
                        reader.IgnoreElement();
                        break;
                }
            }
        }
    }
}
