// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2012 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlChartAxisReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlChartAxisReader
    {
        [Test]
        public void TestReadCatAx()
        {
            string xml =
              "<c:catAx xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                "<c:axId val=\"122199040\" />" +
                "<c:scaling>" +
                  "<c:orientation val=\"minMax\" />" +
                "</c:scaling>" +
                "<c:axPos val=\"t\" />" +
                "<c:majorGridlines />" +
                "<c:title>" +
                  "<c:tx>" +
                    "<c:rich>" +
                      "<a:bodyPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                      "<a:lstStyle xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                      "<a:p xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                        "<a:pPr>" +
                          "<a:defRPr />" +
                        "</a:pPr>" +
                        "<a:r>" +
                          "<a:rPr lang=\"en-US\" i=\"1\" strike=\"sngStrike\" />" +
                          "<a:t>Axis Title</a:t>" +
                        "</a:r>" +
                      "</a:p>" +
                    "</c:rich>" +
                  "</c:tx>" +
                "</c:title>" +
                "<c:tickLblPos val=\"nextTo\" />" +
                "<c:crossAx val=\"122958976\" />" +
                "<c:crosses val=\"autoZero\" />" +
                "<c:auto val=\"1\" />" +
                "<c:lblAlgn val=\"ctr\" />" +
                "<c:lblOffset val=\"10\" />" +
                "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                    "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                        "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                            "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                            "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                    "</a:ext> " +
                "</c:extLst> " +
              "</c:catAx>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            ChartAxis axis = DmlChartAxisReader.Read(reader, null, false);

            Assert.That(axis, IsNot.Null());
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));

            // Check simple properties.
            Assert.That(axis.CategoryType, Is.EqualTo(AxisCategoryType.Automatic));
            Assert.That(axis.IsDateCategoryAxis, Is.False);
            Assert.That(axis.AxId, Is.EqualTo(122199040));
            Assert.That(axis.AxPos, Is.EqualTo(AxisPosition.Top));
            Assert.That(axis.BaseTimeUnit, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.CrossAx, Is.EqualTo(122958976));
            Assert.That(axis.CrossBetween, Is.EqualTo(CrossBetween.Between));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.Hidden, Is.EqualTo(false));
            Assert.That(axis.LblAlgn, Is.EqualTo(LabelAlignment.Center));
            Assert.That(axis.TickLabels.Offset, Is.EqualTo(10));
            Assert.That(axis.MajorTickMark, Is.EqualTo(AxisTickMark.Outside));
            Assert.That(axis.MinorTickMark, Is.EqualTo(AxisTickMark.None));
            Assert.That(axis.MajorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MajorUnitIsAuto, Is.True);
            Assert.That(axis.MinorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MinorUnitIsAuto, Is.True);
            Assert.That(axis.NoMultiLvlLbl, Is.EqualTo(false));
            Assert.That(axis.TickLabels.Position, Is.EqualTo(AxisTickLabelPosition.NextToAxis));
            Assert.That(axis.TickLabels.IsAutoSpacing, Is.True);
            Assert.That(axis.TickMarkSpacingIsAuto, Is.True);

            // Check complex properties.
            Assert.That(axis.DisplayUnit.Unit, Is.EqualTo(AxisBuiltInUnit.None));
            Assert.That(axis.MajorGridlines, IsNot.Null());
            Assert.That(axis.MinorGridlines, Is.Null);
            Assert.That(axis.NumFmt, Is.Null);
            Assert.That(axis.Scaling, IsNot.Null());
            Assert.That(axis.Scaling.Orientation, Is.EqualTo(AxisOrientation.MinMax));
            Assert.That(axis.SpPr, IsNot.Null());
            Assert.That(axis.TitleInternal, IsNot.Null());
            Assert.That(axis.TitleInternal.Tx, IsNot.Null());
            Assert.That(axis.TitleInternal.Tx.TxType, Is.EqualTo(DmlChartTxType.ChartText));
            Assert.That(axis.TxPr, IsNot.Null());

            Assert.That(((IDmlExtensionListSource)axis).Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadDateAx()
        {
            string xml =
                "<c:dateAx xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:axId val=\"122579200\" />" +
                    "<c:scaling><c:orientation val=\"minMax\" /></c:scaling>" +
                    "<c:axPos val=\"b\" />" +
                    "<c:numFmt formatCode=\"dd/mm/yyyy\" sourceLinked=\"1\" />" +
                    "<c:tickLblPos val=\"nextTo\" />" +
                    "<c:crossAx val=\"124649472\" />" +
                    "<c:crosses val=\"autoZero\" />" +
                    "<c:auto val=\"1\" />" +
                    "<c:lblOffset val=\"100\" />" +
                "</c:dateAx>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            ChartAxis axis = DmlChartAxisReader.Read(reader, null, false);

            Assert.That(axis, IsNot.Null());
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));

            // Check simple properties.
            Assert.That(axis.CategoryType, Is.EqualTo(AxisCategoryType.Automatic));
            Assert.That(axis.IsDateCategoryAxis, Is.True);
            Assert.That(axis.AxId, Is.EqualTo(122579200));
            Assert.That(axis.AxPos, Is.EqualTo(AxisPosition.Bottom));
            Assert.That(axis.BaseTimeUnit, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.CrossAx, Is.EqualTo(124649472));
            Assert.That(axis.CrossBetween, Is.EqualTo(CrossBetween.Between));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.Hidden, Is.EqualTo(false));
            Assert.That(axis.LblAlgn, Is.EqualTo(LabelAlignment.Default));
            Assert.That(axis.TickLabels.Offset, Is.EqualTo(100));
            Assert.That(axis.MajorTickMark, Is.EqualTo(AxisTickMark.Outside));
            Assert.That(axis.MinorTickMark, Is.EqualTo(AxisTickMark.None));
            Assert.That(axis.MajorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MajorUnitIsAuto, Is.True);
            Assert.That(axis.MinorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MinorUnitIsAuto, Is.True);
            Assert.That(axis.NoMultiLvlLbl, Is.EqualTo(false));
            Assert.That(axis.TickLabels.Position, Is.EqualTo(AxisTickLabelPosition.NextToAxis));
            Assert.That(axis.TickLabels.IsAutoSpacing, Is.True);
            Assert.That(axis.TickMarkSpacingIsAuto, Is.True);

            // Check complex properties.
            Assert.That(axis.DisplayUnit.Unit, Is.EqualTo(AxisBuiltInUnit.None));
            Assert.That(axis.MajorGridlines, Is.Null);
            Assert.That(axis.MinorGridlines, Is.Null);
            Assert.That(axis.NumFmt, IsNot.Null());
            Assert.That(axis.NumFmt.FormatCode, Is.EqualTo("dd/mm/yyyy"));
            Assert.That(axis.NumFmt.SourceLinked, Is.True);
            Assert.That(axis.Scaling, IsNot.Null());
            Assert.That(axis.Scaling.Orientation, Is.EqualTo(AxisOrientation.MinMax));
            Assert.That(axis.SpPr, IsNot.Null());
            Assert.That(axis.TitleInternal, Is.Null);
            Assert.That(axis.TxPr, IsNot.Null());
        }

        [Test]
        public void TestReadSerAx()
        {
            string xml =
                "<c:serAx xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:axId val=\"124911616\" />" +
                    "<c:scaling><c:orientation val=\"minMax\" /></c:scaling>" +
                    "<c:axPos val=\"b\" />" +
                    "<c:tickLblPos val=\"nextTo\" />" +
                    "<c:crossAx val=\"124909440\" />" +
                    "<c:crosses val=\"autoZero\" />" +
                "</c:serAx>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            ChartAxis axis = DmlChartAxisReader.Read(reader, null, false);

            Assert.That(axis, IsNot.Null());
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Series));

            // Check simple properties.
            Assert.That(axis.AxId, Is.EqualTo(124911616));
            Assert.That(axis.AxPos, Is.EqualTo(AxisPosition.Bottom));
            Assert.That(axis.BaseTimeUnit, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.CrossAx, Is.EqualTo(124909440));
            Assert.That(axis.CrossBetween, Is.EqualTo(CrossBetween.Between));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.Hidden, Is.EqualTo(false));
            Assert.That(axis.LblAlgn, Is.EqualTo(LabelAlignment.Default));
            Assert.That(axis.TickLabels.Offset, Is.EqualTo(100));
            Assert.That(axis.MajorTickMark, Is.EqualTo(AxisTickMark.Outside));
            Assert.That(axis.MinorTickMark, Is.EqualTo(AxisTickMark.None));
            Assert.That(axis.MajorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MajorUnitIsAuto, Is.True);
            Assert.That(axis.MinorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MinorUnitIsAuto, Is.True);
            Assert.That(axis.NoMultiLvlLbl, Is.EqualTo(false));
            Assert.That(axis.TickLabels.Position, Is.EqualTo(AxisTickLabelPosition.NextToAxis));
            Assert.That(axis.TickLabels.IsAutoSpacing, Is.True);
            Assert.That(axis.TickMarkSpacingIsAuto, Is.True);

            // Check complex properties.
            Assert.That(axis.DisplayUnit.Unit, Is.EqualTo(AxisBuiltInUnit.None));
            Assert.That(axis.MajorGridlines, Is.Null);
            Assert.That(axis.MinorGridlines, Is.Null);
            Assert.That(axis.NumFmt, Is.Null);
            Assert.That(axis.Scaling, IsNot.Null());
            Assert.That(axis.Scaling.Orientation, Is.EqualTo(AxisOrientation.MinMax));
            Assert.That(axis.SpPr, IsNot.Null());
            Assert.That(axis.TitleInternal, Is.Null);
            Assert.That(axis.TxPr, IsNot.Null());
        }

        [Test]
        public void TestReadValAx()
        {
            string xml =
                "<c:valAx xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:axId val=\"122958976\" />" +
                    "<c:scaling>" +
                        "<c:logBase val=\"2\" />" +
                        "<c:orientation val=\"maxMin\" />" +
                        "<c:max val=\"6\" />" +
                        "<c:min val=\"1\" />" +
                    "</c:scaling>" +
                    "<c:axPos val=\"l\" />" +
                    "<c:majorGridlines />" +
                    "<c:minorGridlines />" +
                    "<c:title>" +
                        "<c:tx>" +
                            "<c:rich>" +
                                "<a:bodyPr rot=\"-5400000\" vert=\"horz\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                                "<a:lstStyle xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                                "<a:p xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                    "<a:pPr><a:defRPr /></a:pPr>" +
                                    "<a:r><a:rPr lang=\"en-US\" i=\"1\" /><a:t>Axis Title</a:t></a:r>" +
                                "</a:p>" +
                            "</c:rich>" +
                        "</c:tx>" +
                        "<c:layout>" +
                            "<c:manualLayout>" +
                                "<c:xMode val=\"edge\" />" +
                                "<c:yMode val=\"edge\" />" +
                                "<c:x val=\"6.9444444444444489E-2\" />" +
                                "<c:y val=\"0.51687664041994752\" />" +
                            "</c:manualLayout>" +
                        "</c:layout>" +
                    "</c:title>" +
                    "<c:numFmt formatCode=\"#,##0.00[$$-C09]\" sourceLinked=\"0\" />" +
                    "<c:majorTickMark val=\"in\" />" +
                    "<c:tickLblPos val=\"low\" />" +
                    "<c:txPr>" +
                        "<a:bodyPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                        "<a:lstStyle xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                        "<a:p xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                            "<a:pPr>" +
                                "<a:defRPr sz=\"1400\" b=\"1\" i=\"1\" baseline=\"0\">" +
                                    "<a:solidFill><a:schemeClr val=\"accent6\" /></a:solidFill>" +
                                    "<a:latin typeface=\"Arial\" pitchFamily=\"34\" charset=\"0\" />" +
                                "</a:defRPr>" +
                            "</a:pPr>" +
                            "<a:endParaRPr lang=\"en-US\" />" +
                        "</a:p>" +
                    "</c:txPr>" +
                    "<c:crossAx val=\"122199040\" />" +
                    "<c:crosses val=\"autoZero\" />" +
                    "<c:crossBetween val=\"between\" />" +
                    "<c:majorUnit val=\"1\" />" +
                    "<c:dispUnits>" +
                        "<c:builtInUnit val=\"hundreds\" />" +
                        "<c:dispUnitsLbl />" +
                    "</c:dispUnits>" +
                "</c:valAx>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            ChartAxis axis = DmlChartAxisReader.Read(reader, null, false);

            Assert.That(axis, IsNot.Null());
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));

            // Check simple properties.
            Assert.That(axis.AxId, Is.EqualTo(122958976));
            Assert.That(axis.AxPos, Is.EqualTo(AxisPosition.Left));
            Assert.That(axis.BaseTimeUnit, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.CrossAx, Is.EqualTo(122199040));
            Assert.That(axis.CrossBetween, Is.EqualTo(CrossBetween.Between));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.Hidden, Is.EqualTo(false));
            Assert.That(axis.LblAlgn, Is.EqualTo(LabelAlignment.Default));
            Assert.That(axis.TickLabels.Offset, Is.EqualTo(100));
            Assert.That(axis.MajorTickMark, Is.EqualTo(AxisTickMark.Inside));
            Assert.That(axis.MinorTickMark, Is.EqualTo(AxisTickMark.None));
            Assert.That(axis.MajorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MajorUnit, Is.EqualTo(1));
            Assert.That(axis.MinorUnitScale, Is.EqualTo(AxisTimeUnit.Automatic));
            Assert.That(axis.MinorUnitIsAuto, Is.True);
            Assert.That(axis.NoMultiLvlLbl, Is.EqualTo(false));
            Assert.That(axis.TickLabels.Position, Is.EqualTo(AxisTickLabelPosition.Low));
            Assert.That(axis.TickLabels.IsAutoSpacing, Is.True);
            Assert.That(axis.TickMarkSpacingIsAuto, Is.True);

            // Check complex properties.
            Assert.That(axis.DisplayUnit.Unit, Is.EqualTo(AxisBuiltInUnit.Hundreds));
            Assert.That(axis.MajorGridlines, IsNot.Null());
            Assert.That(axis.MinorGridlines, IsNot.Null());
            Assert.That(axis.NumFmt, IsNot.Null());
            Assert.That(axis.NumFmt.FormatCode, Is.EqualTo("#,##0.00[$$-C09]"));
            Assert.That(axis.NumFmt.SourceLinked, Is.False);
            Assert.That(axis.Scaling, IsNot.Null());
            Assert.That(axis.Scaling.Orientation, Is.EqualTo(AxisOrientation.MaxMin));
            Assert.That(axis.Scaling.Type, Is.EqualTo(AxisScaleType.Logarithmic));
            Assert.That(axis.Scaling.LogBase, Is.EqualTo(2));
            Assert.That(axis.Scaling.Minimum, Is.EqualTo(new AxisBound(1)));
            Assert.That(axis.Scaling.Maximum, Is.EqualTo(new AxisBound(6)));
            Assert.That(axis.SpPr, IsNot.Null());
            Assert.That(axis.TitleInternal, IsNot.Null());
            Assert.That(axis.TitleInternal.Tx, IsNot.Null());
            Assert.That(axis.TitleInternal.Tx.TxType, Is.EqualTo(DmlChartTxType.ChartText));
            Assert.That(axis.TxPr, IsNot.Null());
        }

        /// <summary>
        /// Tests that axis public properties have the same values as in Word interface.
        /// </summary>
        [Test]
        public void TestPublicProperties()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestAxisPublicProperties.docx");

            // Chart 0
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            ChartAxis axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.CategoryType, Is.EqualTo(AxisCategoryType.Time));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Custom));
            Assert.That(axis.CrossesAt, Is.EqualTo(100));
            Assert.That(axis.ReverseOrder, Is.False);
            Assert.That(axis.MajorUnitIsAuto, Is.True);
            Assert.That(axis.MinorUnitIsAuto, Is.True);
            Assert.That(axis.NumberFormat.FormatCode, Is.EqualTo(@"[$-1409]dddd\,\ d\ mmmm\ yyyy;@"));
            AxisScaling scaling = axis.Scaling;
            Assert.That(scaling.Type, Is.EqualTo(AxisScaleType.Linear));
            Assert.That(scaling.Minimum.IsAuto, Is.True);
            Assert.That(scaling.Maximum.IsAuto, Is.True);

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Custom));
            DateTime baseDate = new DateTime(1899, 12, 30);
            Assert.That(axis.CrossesAt, Is.EqualTo((new DateTime(2002, 6, 1)).Subtract(baseDate).Days));
            Assert.That(axis.AxisBetweenCategories, Is.True);
            scaling = axis.Scaling;
            Assert.That(scaling.Type, Is.EqualTo(AxisScaleType.Linear));
            Assert.That(scaling.Minimum.IsAuto, Is.False);
            Assert.That(scaling.Minimum, Is.EqualTo(new AxisBound(-180)));
            Assert.That(scaling.Maximum.IsAuto, Is.False);
            Assert.That(scaling.Maximum, Is.EqualTo(new AxisBound(240)));

            Assert.That(chart.AxisZ, Is.Null);

            // Chart 1 (ChartEx)
            chart = doc.FirstSection.Body.Shapes[1].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, Is.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.CategoryType, Is.EqualTo(AxisCategoryType.Category));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.MajorTickMark, Is.EqualTo(AxisTickMark.Outside));
            Assert.That(axis.MinorTickMark, Is.EqualTo(AxisTickMark.Inside));

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.NumberFormat.FormatCode, Is.EqualTo("#,##0.0"));

            // Chart 2
            chart = doc.FirstSection.Body.Shapes[2].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.TickLabels.Offset, Is.EqualTo(500));

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.MajorUnit, Is.EqualTo(2.5));
            Assert.That(axis.MinorUnit, Is.EqualTo(1.0));

            axis = chart.AxisZ;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Series));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.TickLabels.Position, Is.EqualTo(AxisTickLabelPosition.High));

            // Chart 3
            chart = doc.FirstSection.Body.Shapes[3].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Maximum));
            Assert.That(axis.TickLabels.Position, Is.EqualTo(AxisTickLabelPosition.Low));

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Maximum));

            // Chart 4
            chart = doc.FirstSection.Body.Shapes[4].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            Assert.That(chart.AxisX, Is.Null);
            Assert.That(chart.AxisY, Is.Null);
            Assert.That(chart.AxisZ, Is.Null);

            // Chart 5
            chart = doc.FirstSection.Body.Shapes[5].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.ReverseOrder, Is.True);

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.Crosses, Is.EqualTo(AxisCrosses.Automatic));
            Assert.That(axis.Scaling.Type, Is.EqualTo(AxisScaleType.Logarithmic));
            Assert.That(axis.Scaling.LogBase, Is.EqualTo(4));

            // Chart 6
            chart = doc.FirstSection.Body.Shapes[6].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.CategoryType, Is.EqualTo(AxisCategoryType.Automatic));
            Assert.That(axis.IsDateCategoryAxis, Is.True);
            Assert.That(axis.MajorUnit, Is.EqualTo(2));
            Assert.That(axis.MajorUnitScale, Is.EqualTo(AxisTimeUnit.Months));
            Assert.That(axis.MinorUnit, Is.EqualTo(1));
            Assert.That(axis.MinorUnitScale, Is.EqualTo(AxisTimeUnit.Months));
            Assert.That(axis.BaseTimeUnit, Is.EqualTo(AxisTimeUnit.Months));

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.MajorTickMark, Is.EqualTo(AxisTickMark.Cross));
            Assert.That(axis.MinorTickMark, Is.EqualTo(AxisTickMark.Outside));
            Assert.That(axis.AxisBetweenCategories, Is.False);

            // Chart 7
            chart = doc.FirstSection.Body.Shapes[7].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisX;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(axis.TickLabels.IsAutoSpacing, Is.False);
            Assert.That(axis.TickLabels.Spacing, Is.EqualTo(2));
            Assert.That(axis.TickMarkSpacing, Is.EqualTo(2));

            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));

            axis = chart.AxisZ;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Series));
            Assert.That(axis.TickLabels.IsAutoSpacing, Is.True);
            Assert.That(axis.TickMarkSpacing, Is.EqualTo(1));

            // Chart 8
            chart = doc.FirstSection.Body.Shapes[8].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, IsNot.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.DisplayUnit.Unit, Is.EqualTo(AxisBuiltInUnit.Custom));
            Assert.That(axis.DisplayUnit.CustomUnit, Is.EqualTo(10));
            Assert.That(axis.DisplayUnit.Label, IsNot.Null());

            // Chart 9 (ChartEx)
            chart = doc.FirstSection.Body.Shapes[9].Chart;
            Assert.That(chart.ChartSpace.FirstChart.ChartType, Is.EqualTo(DmlChartType.ChartExChart));
            axis = chart.AxisY;
            Assert.That(axis.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(axis.DisplayUnit.Unit, Is.EqualTo(AxisBuiltInUnit.Hundreds));
            Assert.That(axis.DisplayUnit.Label, IsNot.Null());
            Assert.That(axis.Scaling.Minimum, Is.EqualTo(new AxisBound(-200)));
            Assert.That(axis.Scaling.Maximum, Is.EqualTo(new AxisBound(200)));
        }

        /// <summary>
        /// Tests that the 'delete' flag of an axis is read/written/set correctly for ISO 29500 charts.
        /// </summary>
        [Test]
        public void TestDeletedAxis()
        {
            const string fileName = @"Model\Charts\TestDeletedAxis";
            Document doc = TestUtil.OpenSaveOpen(fileName, UnifiedScenario.Docx2DocxNoGold);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Assert.That(chart.AxisY.Hidden, Is.True);

            chart.AxisY.Hidden = false;
            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            Assert.That(chart.AxisY.Hidden, Is.False);
        }
    }
}
