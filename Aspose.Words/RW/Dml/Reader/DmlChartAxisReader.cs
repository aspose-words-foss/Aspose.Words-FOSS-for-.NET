// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2012 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class is used to read axis of charts.
    /// 5.7.2.25 catAx (Category Axis Data)
    /// 5.7.2.39 dateAx (Date Axis)
    /// 5.7.2.176 serAx (Series Axis)
    /// 5.7.2.227 valAx (Value Axis)
    /// </summary>
    internal class DmlChartAxisReader : DmlReaderBase
    {
        private DmlChartAxisReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        public static ChartAxis Read(DocxDocumentReaderBase reader, DmlChart dmlChart, bool isChartEx)
        {
            DmlChartAxisReader chartAxisReader = new DmlChartAxisReader(reader);
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;
            ChartAxisType axisType = GetAxisType(tagName);

            ChartAxis axis = new ChartAxis(axisType, dmlChart, reader.Document, isChartEx);

            if (axis.IsCategory)
                axis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.IsDateCategoryAxis, tagName == "dateAx");

            if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                // Read attributes of the 2.24.3.3 CT_Axis complex type [MS-ODRAWXML].
                while (xmlReader.MoveToNextAttribute())
                {
                    switch (xmlReader.LocalName)
                    {
                        case "id":
                            axis.AxId = xmlReader.ValueAsInt;
                            axis.Direction = dmlChart.AddAxId(axis.AxId);
                            break;
                        case "hidden":
                            axis.SetHiddenInternal(xmlReader.ValueAsBool);
                            break;
                        default:
                            WarnUnexpected(xmlReader);
                            break;
                    }
                }

                // If the tickLabels element of the 2.24.3.3 CT_Axis type does not exist, labels are not shown.
                axis.AreTickLabelsVisible = false;
            }

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "axId":
                        axis.AxId = xmlReader.ReadIntAttribute(0);
                        break;
                    case "catScaling": // 2.24.3.8 CT_CategoryAxisScaling [MS-ODRAWXML]
                    {
                        axis.SetType(ChartAxisType.Category);
                        DmlChartComplexTypesReader.ReadCategoryScaling(xmlReader, axis.Scaling);
                        break;
                    }
                    case "valScaling": // 2.24.3.82 CT_ValueAxisScaling [MS-ODRAWXML]
                    {
                        axis.SetType(ChartAxisType.Value);
                        DmlChartComplexTypesReader.ReadValueScaling(xmlReader, axis);
                        break;
                    }
                    case "scaling":
                        DmlChartComplexTypesReader.ReadChartScaling(reader, axis.Scaling);
                        break;
                    case "delete":
                        axis.SetHiddenInternal(xmlReader.ReadBoolAttribute(true));
                        break;
                    case "axPos":
                        axis.AxPos = DmlChartsEnum.DmlToAxisPosition(xmlReader.ReadAttribute(""));
                        break;
                    case "majorGridlines":
                        axis.MajorGridlines = chartAxisReader.ReadGridLines("majorGridlines");
                        break;
                    case "minorGridlines":
                        axis.MinorGridlines = chartAxisReader.ReadGridLines("minorGridlines");
                        break;
                    case "title":
                        axis.TitleInternal = DmlChartComplexTypesReader.ReadChartTitle(axis, reader);
                        break;
                    case "numFmt":
                        axis.NumFmt = DmlChartComplexTypesReader.ReadChartNumFormat(xmlReader);
                        break;
                    case "majorTickMark":
                        axis.MajorTickMark = DmlChartsEnum.DmlToTickMark(xmlReader.ReadAttribute(""));
                        break;
                    case "minorTickMark":
                        axis.MinorTickMark = DmlChartsEnum.DmlToTickMark(xmlReader.ReadAttribute(""));
                        break;
                    case "majorTickMarks":
                        DmlChartComplexTypesReader.ReadTickMarks(reader, axis.ChartAxisPr,
                            DmlChartAxisAttrs.MajorTickMark, DmlChartAxisAttrs.MajorTickMarkExtensions);
                        break;
                    case "minorTickMarks":
                        DmlChartComplexTypesReader.ReadTickMarks(reader, axis.ChartAxisPr,
                            DmlChartAxisAttrs.MinorTickMark, DmlChartAxisAttrs.MinorTickMarkExtensions);
                        break;
                    case "tickLabels": // of 2.24.3.80 CT_TickLabels type [MS-ODRAWXML]
                    {
                        axis.AreTickLabelsVisible = true;
                        ReadTickLabels(reader, axis);
                        break;
                    }
                    case "tickLblPos":
                        axis.TickLabels.Position = DmlChartsEnum.DmlToTickLabelPosition(xmlReader.ReadAttribute(""));
                        break;
                    case "spPr":
                        DmlChartComplexTypesReader.ReadChartSpPr(reader, axis.SpPr);
                        break;
                    case "txPr":
                        DmlChartComplexTypesReader.ReadChartTxPr(reader, axis.TxPr);
                        break;
                    case "crossAx":
                        axis.CrossAx = xmlReader.ReadIntAttribute(0);
                        break;
                    case "crosses":
                        axis.Crosses = DmlChartsEnum.DmlToCrosses(xmlReader.ReadAttribute(""));
                        break;
                    case "crossesAt":
                        axis.CrossesAt = xmlReader.ReadDoubleAttribute(0);
                        axis.Crosses = AxisCrosses.Custom;
                        break;
                    case "crossBetween":
                        axis.CrossBetween = DmlChartsEnum.DmlToCrossBetween(xmlReader.ReadAttribute(""));
                        break;
                    case "majorUnit":
                        axis.MajorUnitPr.Value = xmlReader.ReadDoubleAttribute(1);   // set without checking value
                        break;
                    case "minorUnit":
                        axis.MinorUnitPr.Value = xmlReader.ReadDoubleAttribute(0.5); // set without checking value
                        break;
                    case "dispUnits":
                    case "units": // 2.24.3.5 CT_AxisUnits [MS-ODRAWXML]
                        axis.SetDisplayUnit(DmlChartComplexTypesReader.ReadChartDisplayUnits(axis, reader));
                        break;
                    case "auto":
                        if (xmlReader.ReadBoolAttribute(true))
                            axis.CategoryType = AxisCategoryType.Automatic;
                        break;
                    case "lblAlgn":
                        axis.LblAlgn = DmlChartsEnum.DmlToLabelAlignment(xmlReader.ReadAttribute(""));
                        break;
                    case "lblOffset":
                        // Use SetProperty to not check input value.
                        axis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLabelOffset, xmlReader.ReadIntAttribute(100));
                        break;
                    case "tickLblSkip":
                        // Use SetProperty to not check input value.
                        axis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickLabelSpacing, xmlReader.ReadIntAttribute(1));
                        axis.TickLabels.IsAutoSpacing = false;
                        break;
                    case "tickMarkSkip":
                        // Use SetProperty to not check input value.
                        axis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.TickMarkSpacing, xmlReader.ReadIntAttribute(1));
                        break;
                    case "noMultiLvlLbl":
                        axis.NoMultiLvlLbl = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "baseTimeUnit":
                        axis.BaseTimeUnit = DmlChartsEnum.DmlToTimeUnit(xmlReader.ReadAttribute(""));
                        break;
                    case "majorTimeUnit":
                        axis.MajorUnitScale = DmlChartsEnum.DmlToTimeUnit(xmlReader.ReadAttribute(""));
                        break;
                    case "minorTimeUnit":
                        axis.MinorUnitScale = DmlChartsEnum.DmlToTimeUnit(xmlReader.ReadAttribute(""));
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)axis).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            SetDefaultTitleRotation(axis);
            ValidateLabelRotation(axis);

            return axis;
        }

        /// <summary>
        /// Reads an element of the 2.24.3.80 CT_TickLabels complex type [MS-ODRAWXML].
        /// </summary>
        internal static void ReadTickLabels(DocxDocumentReaderBase reader, ChartAxis axis)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read children
            while (xmlReader.ReadChild("tickLabels"))
            {
                switch (xmlReader.LocalName)
                {
                    case "extLst":
                        axis.TickLabelExtensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets default rotation for vertical axis' title (WORDSNET-8556).
        /// </summary>
        internal static void SetDefaultTitleRotation(ChartAxis axis)
        {
            DmlTextBodyProperties parentBodyPr = new DmlTextBodyProperties();
            if (axis.IsVertical)
                parentBodyPr.Rotation = DmlAngle.FromDegrees(-90);

            if (axis.TitleInternal != null)
                axis.TitleInternal.TxPr.BodyPr.SetParentProperties(parentBodyPr);

            if (axis.DisplayUnit.Label != null)
                axis.DisplayUnit.Label.TxPr.BodyPr.SetParentProperties(parentBodyPr);
        }

        /// <summary>
        /// For some reason MS Word 2013 sets axis labels rotation to -1000 degrease by default.
        /// But this value is treated as zero in MS Word. So to fix the problem simply reset value to zero.
        /// </summary>
        private static void ValidateLabelRotation(ChartAxis axis)
        {
            if (MathUtil.AreEqual(axis.TxPr.BodyPr.Rotation.ValueInDegrees, -1000.0d))
            {
                axis.TxPr.BodyPr.Remove(DmlTextBodyPropertiesDefaultsIds.Rotation);
                axis.TxPr.BodyPr.HasDefaultRotation = true;
            }
        }

        private DmlChartGridlines ReadGridLines(string tagName)
        {
            DmlChartGridlines gridlines = new DmlChartGridlines();

            while (mDocumentReader.XmlReader.ReadChild(tagName))
            {
                switch (mDocumentReader.XmlReader.LocalName)
                {
                    case "spPr":
                        DmlChartComplexTypesReader.ReadChartSpPr(mDocumentReader, gridlines.SpPr);
                        break;
                    case "extLst": // in 2.24.3.52 CT_Gridlines type [MS-ODRAWXML]
                        gridlines.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mDocumentReader.XmlReader);
                        break;
                }
            }

            return gridlines;
        }

        private static ChartAxisType GetAxisType(string tagName)
        {
            switch (tagName)
            {
                case "catAx":
                case "dateAx":
                    return ChartAxisType.Category;
                case "serAx":
                    return ChartAxisType.Series;
                case "valAx":
                    return ChartAxisType.Value;
                case "axis": // axis element of 2.24.3.64 CT_PlotArea [MS-ODRAWXML]
                    // Type is defined by catScaling/valScaling element; return Value for now.
                    return ChartAxisType.Value;
                default:
                    throw new ArgumentException("Unexpected axis type.");
            }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
