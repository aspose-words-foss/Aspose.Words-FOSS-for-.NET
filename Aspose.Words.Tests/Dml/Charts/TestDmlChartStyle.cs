// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/05/2016 by Alexander Zhiltsov

using System.Collections.Generic;
using System.IO;
using System.Xml;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Words.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests color style and style parts of a chart.
    /// </summary>
    [TestFixture]
    public class TestDmlChartStyle
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests reading a color style part of a char.
        /// </summary>
        [Test]
        public void TestReadingColorStyle()
        {
            DmlChartColorStyle colorStyle = LoadColorStyleFromXml();
            CheckColorStyleFromXml(colorStyle);
        }

        /// <summary>
        /// Tests cloning objects of the <see cref="DmlChartColorStyle"/> class.
        /// </summary>
        [Test]
        public void TestCloningColorStyle()
        {
            DmlChartColorStyle colorStyle = LoadColorStyleFromXml();
            CheckColorStyleFromXml(colorStyle); // check to exclude reader errors
            DmlChartColorStyle clonedColorStyle = colorStyle.Clone();

            // Changing properties of the source object to check that the cloned object is not changed.
            colorStyle.Method = "withinLinear";
            colorStyle.Id = "50";

            ((DmlSchemeColor)colorStyle.Colors[0]).Value = ThemeColor.Dark1;
            colorStyle.Colors[1] =  new DmlPresetColor("red");

            DmlChartColorStyleVariation[] variations = colorStyle.Variations;
            DmlLuminanceModulation luminanceModulation =  new DmlLuminanceModulation();
            luminanceModulation.Value = 0.8d;
            variations[0].ColorModifiers.Add(luminanceModulation);
            ((DmlLuminanceModulation)variations[1].ColorModifiers[0]).Value = 0.99d;

            variations[2] = new DmlChartColorStyleVariation();

            colorStyle.Extensions[DmlExtensionUri.ThemeFamily].Uri = "";
            colorStyle.Extensions.Clear();

            CheckColorStyleFromXml(clonedColorStyle);
        }

        /// <summary>
        /// Reads color style from the predefined test XML.
        /// </summary>
        private static DmlChartColorStyle LoadColorStyleFromXml()
        {
            string xml =
                "<cs:colorStyle " +
                    "xmlns:cs=\"http://schemas.microsoft.com/office/drawing/2012/chartStyle\" " +
                    "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" meth=\"cycle\" id=\"10\"> " +
                    "<a:schemeClr val=\"accent1\"/> " +
                    "<a:schemeClr val=\"accent6\"/> " +
                    "<cs:variation/> " +
                    "<cs:variation> " +
                        "<a:lumMod val=\"60000\"/> " +
                    "</cs:variation> " +
                    "<cs:variation> " +
                        "<a:lumMod val=\"80000\"/> " +
                        "<a:lumOff val=\"20000\"/> " +
                    "</cs:variation> " +
                    "<cs:extLst> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</cs:extLst> " +
                "</cs:colorStyle>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartColorStyle colorStyle = DmlChartColorStyleReader.Read(reader);
            return colorStyle;
        }

        /// <summary>
        /// Checks color style that is loaded by the <see cref="LoadColorStyleFromXml"/> method.
        /// </summary>
        private static void CheckColorStyleFromXml(DmlChartColorStyle colorStyle)
        {
            Assert.That(colorStyle, IsNot.Null());
            Assert.That(colorStyle.Method, Is.EqualTo("cycle"));
            Assert.That(colorStyle.Id, Is.EqualTo("10"));

            DmlColor[] colors = colorStyle.Colors;
            Assert.That(colors, IsNot.Null());
            Assert.That(colors.Length, Is.EqualTo(2));

            Assert.That(colors[0].ColorType, Is.EqualTo(DmlColorType.SchemeColor));
            Assert.That(((DmlSchemeColor)colors[0]).Value, Is.EqualTo(ThemeColor.Accent1));

            Assert.That(colors[1].ColorType, Is.EqualTo(DmlColorType.SchemeColor));
            Assert.That(((DmlSchemeColor)colors[1]).Value, Is.EqualTo(ThemeColor.Accent6));

            DmlChartColorStyleVariation[] variations = colorStyle.Variations;
            Assert.That(variations, IsNot.Null());
            Assert.That(variations.Length, Is.EqualTo(3));

            Assert.That(variations[0].ColorModifiers.Count, Is.EqualTo(0));

            Assert.That(variations[1].ColorModifiers.Count, Is.EqualTo(1));
            DmlColorModifier modifier = (DmlColorModifier)variations[1].ColorModifiers[0];
            Assert.That(modifier.ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceModulation));
            Assert.That(((DmlLuminanceModulation)modifier).Value, Is.EqualTo(0.6d));

            Assert.That(variations[2].ColorModifiers.Count, Is.EqualTo(2));
            modifier = (DmlColorModifier)variations[2].ColorModifiers[0];
            Assert.That(modifier.ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceModulation));
            Assert.That(((DmlLuminanceModulation)modifier).Value, Is.EqualTo(0.8d));
            modifier = (DmlColorModifier)variations[2].ColorModifiers[1];
            Assert.That(modifier.ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceOffset));
            Assert.That(((DmlLuminanceOffset)modifier).Value, Is.EqualTo(0.2d));

            Assert.That(colorStyle.Extensions, IsNot.Null());
            Assert.That(colorStyle.Extensions.Count, Is.EqualTo(1));
            Assert.That(colorStyle.Extensions[DmlExtensionUri.ThemeFamily].Uri, Is.EqualTo(DmlExtensionUri.ThemeFamily));
        }

        /// <summary>
        /// Tests reading a style part of a char.
        /// </summary>
        [Test]
        public void TestReadingStyle()
        {
            DmlChartStyle style = LoadStyleFromXml();
            CheckStyleFromXml(style);
        }

        /// <summary>
        /// Tests cloning objects of the <see cref="DmlChartStyle"/> class.
        /// </summary>
        [Test]
        public void TestCloningStyle()
        {
            DmlChartStyle style = LoadStyleFromXml();
            CheckStyleFromXml(style); // check to exclude reader errors
            DmlChartStyle clonedStyle = style.Clone();

            // Changing properties of the source object to check that the cloned object is not changed.
            DmlChartStyleEntry axisTitle = style[DmlChartStyleItem.AxisTitle];

            axisTitle.Modifiers[0] = "";

            axisTitle.LineReference.StyleMatrixIndex *= 2;
            axisTitle.LineWidthScale *= 2;
            axisTitle.FillReference.StyleMatrixIndex *= 2;
            axisTitle.EffectReference.StyleMatrixIndex *= 2;
            axisTitle.FontReference.FontCollectionIndex = DmlFontCollectionIndex.Major;

            ((DmlSolidFill)axisTitle.ShapePr.Outline.Fill).Color.ColorModifiers.Clear();

            axisTitle.DefaultRunPr.Kerning = new DmlTextPoints(0);
            axisTitle.TextBodyPr.RightInset *= 2;
            axisTitle.Extensions[DmlExtensionUri.HiddenEffects].Uri = "";

            DmlChartStyleReference styleReference = style[DmlChartStyleItem.CategoryAxis].FillReference;
            styleReference.Modifiers[0] = "none";
            styleReference.StyleMatrixIndex *= 2;
            DmlSchemeColor schemeColor = (DmlSchemeColor)styleReference.Color;
            ((DmlLuminanceModulation)schemeColor.ColorModifiers[0]).Value *= 2;
            schemeColor.ColorModifiers.RemoveAt(1);

            DmlChartFontReference fontReference = style[DmlChartStyleItem.CategoryAxis].FontReference;
            fontReference.Modifiers[0] = "";
            fontReference.FontCollectionIndex = DmlFontCollectionIndex.Minor;
            DmlChartStyleColor styleColor = (DmlChartStyleColor)fontReference.Color;
            styleColor.Value = "0";
            ((DmlLuminanceModulation)styleColor.ColorModifiers[0]).Value += 0.2d;
            styleColor.ColorModifiers.RemoveAt(1);

            style.DataPointMarkerLayout.Symbol = "dash";
            style.DataPointMarkerLayout.Size *= 2;

            foreach (DmlChartStyleItem index in DmlChartStyle.AllStyleItems)
            {
                style[index].LineReference.StyleMatrixIndex *= 2;
                style[index].FillReference.StyleMatrixIndex *= 2;
                style[index].EffectReference.StyleMatrixIndex *= 2;
            }

            style.Extensions[DmlExtensionUri.ThemeFamily].Uri = "";
            style.Extensions.Clear();

            style.Id = "1";

            CheckStyleFromXml(clonedStyle);
        }

        /// <summary>
        /// Reads a style part from the predefined test XML.
        /// </summary>
        private static DmlChartStyle LoadStyleFromXml()
        {
            string xml = File.ReadAllText(TestUtil.GetInTestDataPath(@"ImportDocx\ChartStylePart.xml"));
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartStyle style = DmlChartStyleReader.Read(reader);
            return style;
        }

        /// <summary>
        /// Checks a style part that is loaded by the <see cref="LoadStyleFromXml"/> method.
        /// </summary>
        private static void CheckStyleFromXml(DmlChartStyle style)
        {
            // Test reading properties of the DmlChartStyleEntry class. And DmlChartStyle.AxisTitle.
            DmlChartStyleEntry axisTitle = style[DmlChartStyleItem.AxisTitle];

            string[] modifiers = axisTitle.Modifiers;
            Assert.That(modifiers.Length, Is.EqualTo(2));
            Assert.That(modifiers[0], Is.EqualTo("allowNoFillOverride"));
            Assert.That(modifiers[1], Is.EqualTo("allowNoLineOverride"));

            Assert.That(axisTitle.LineReference.StyleMatrixIndex, Is.EqualTo(1));
            Assert.That(axisTitle.LineWidthScale, Is.EqualTo(1.5d));
            Assert.That(axisTitle.FillReference.StyleMatrixIndex, Is.EqualTo(2));
            Assert.That(axisTitle.EffectReference.StyleMatrixIndex, Is.EqualTo(3));
            Assert.That(axisTitle.FontReference.FontCollectionIndex, Is.EqualTo(DmlFontCollectionIndex.Minor));

            DefaultShapeProperties shapePr = axisTitle.ShapePr;
            Assert.That(shapePr, IsNot.Null());
            Assert.That(shapePr.Outline.Fill.DmlFillType, Is.EqualTo(DmlFillType.SolidFill));
            Assert.That(((DmlSolidFill)shapePr.Outline.Fill).Color.ColorModifiers.Count, Is.EqualTo(2));

            Assert.That(axisTitle.DefaultRunPr, IsNot.Null());
            Assert.That(axisTitle.DefaultRunPr.Kerning.Value, Is.EqualTo(1100));

            Assert.That(axisTitle.TextBodyPr, IsNot.Null());
           // Assert.AreEqual(10, axisTitle.TextBodyPr.Rotation.Value);
            Assert.That(axisTitle.TextBodyPr.RightInset, Is.EqualTo(36576));

            Assert.That(axisTitle.Extensions, IsNot.Null());
            Assert.That(axisTitle.Extensions[DmlExtensionUri.HiddenEffects].Uri, Is.EqualTo(DmlExtensionUri.HiddenEffects));

            // Test reading properties of the DmlChartStyleReference class. And DmlChartStyle.CategoryAxis.
            DmlChartStyleReference styleReference = style[DmlChartStyleItem.CategoryAxis].FillReference;

            modifiers = styleReference.Modifiers;
            Assert.That(modifiers.Length, Is.EqualTo(2));
            Assert.That(modifiers[0], Is.EqualTo("ignoreCSTransforms"));
            Assert.That(modifiers[1], Is.EqualTo("auto"));

            Assert.That(styleReference.StyleMatrixIndex, Is.EqualTo(1));

            Assert.That(styleReference.Color.ColorType, Is.EqualTo(DmlColorType.SchemeColor));
            DmlSchemeColor schemeColor = (DmlSchemeColor)styleReference.Color;
            List<IDmlColorModifier> colorModifiers = schemeColor.ColorModifiers;
            Assert.That(colorModifiers.Count, Is.EqualTo(2));
            Assert.That(((DmlColorModifier)colorModifiers[0]).ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceModulation));
            Assert.That(((DmlLuminanceModulation)colorModifiers[0]).Value, Is.EqualTo(0.5d));
            Assert.That(((DmlColorModifier)colorModifiers[1]).ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceOffset));
            Assert.That(((DmlLuminanceOffset)colorModifiers[1]).Value, Is.EqualTo(0.5d));

            // Test reading properties of the DmlChartFontReference and DmlChartStyleColor classes.
            DmlChartFontReference fontReference = style[DmlChartStyleItem.CategoryAxis].FontReference;

            modifiers = fontReference.Modifiers;
            Assert.That(modifiers.Length, Is.EqualTo(2));
            Assert.That(modifiers[0], Is.EqualTo("auto"));
            Assert.That(modifiers[1], Is.EqualTo("ignoreCSTransforms"));

            Assert.That(fontReference.FontCollectionIndex, Is.EqualTo(DmlFontCollectionIndex.Major));

            Assert.That(fontReference.Color.ColorType, Is.EqualTo(DmlColorType.ChartStyleColor));
            DmlChartStyleColor styleColor = (DmlChartStyleColor)fontReference.Color;
            Assert.That(styleColor.Value, Is.EqualTo("auto"));
            colorModifiers = styleColor.ColorModifiers;
            Assert.That(colorModifiers.Count, Is.EqualTo(2));
            Assert.That(((DmlColorModifier)colorModifiers[0]).ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceModulation));
            Assert.That(((DmlLuminanceModulation)colorModifiers[0]).Value, Is.EqualTo(0.65d));
            Assert.That(((DmlColorModifier)colorModifiers[1]).ModifierType, Is.EqualTo(DmlColorModifierType.LuminanceOffset));
            Assert.That(((DmlLuminanceOffset)colorModifiers[1]).Value, Is.EqualTo(0.35d));

            // Test reading properties of the DmlChartMarkerLayout class. And DmlChartStyle.DataPointMarkerLayout.
            DmlChartMarkerLayout markerLayout = style.DataPointMarkerLayout;
            Assert.That(markerLayout.Symbol, Is.EqualTo("circle"));
            Assert.That(markerLayout.Size, Is.EqualTo(5));

            // Test reading properties of the DmlChartStyle class (except AxisTitle, CategoryAxis and
            // DataPointMarkerLayout that are not tested above).
            Assert.That(style[DmlChartStyleItem.ChartArea].LineReference.StyleMatrixIndex, Is.EqualTo(110));
            Assert.That(style[DmlChartStyleItem.DataLabel].FillReference.StyleMatrixIndex, Is.EqualTo(50));
            Assert.That(style[DmlChartStyleItem.DataLabelCallout].EffectReference.StyleMatrixIndex, Is.EqualTo(9));
            int nextExpectedMatrixIndex = 10;
            foreach (DmlChartStyleItem index in DmlChartStyle.AllStyleItems)
            {
                if (index >= DmlChartStyleItem.DataPoint)
                {
                    Assert.That(style[index].LineReference.StyleMatrixIndex, Is.EqualTo(nextExpectedMatrixIndex));
                    nextExpectedMatrixIndex++;
                }
            }

            Assert.That(style.Extensions.Count, Is.EqualTo(1));
            Assert.That(style.Extensions[DmlExtensionUri.ThemeFamily].Uri, Is.EqualTo(DmlExtensionUri.ThemeFamily));

            Assert.That(style.Id, Is.EqualTo("201"));
        }

        /// <summary>
        /// Tests writing a color style part of a chart.
        /// </summary>
        [Test]
        public void TestWritingColorStyle()
        {
            Document doc = CreateDocumetnWithChart();
            DmlChartSpace chartSpace = (DmlChartSpace)doc.FirstSection.Body.Shapes[0].DmlNode;

            DmlChartColorStyle colorStyle = new DmlChartColorStyle();
            colorStyle.Id = "3";
            colorStyle.Method = "cycle";

            DmlColor[] colors = new DmlColor[2];
            colors[0] = new DmlSchemeColor(ThemeColor.Light1);
            colors[1] = new DmlPresetColor("azure");
            colorStyle.Colors = colors;

            DmlChartColorStyleVariation[] variations = new DmlChartColorStyleVariation[3];
            variations[0] = CreateVariation();
            DmlLuminanceModulation luminanceModifier = new DmlLuminanceModulation();
            luminanceModifier.Value = 0.5d;
            variations[1] = CreateVariation(luminanceModifier);
            luminanceModifier = new DmlLuminanceModulation();
            luminanceModifier.Value = 0.8d;
            DmlLuminanceOffset luminanceOffset = new DmlLuminanceOffset();
            luminanceOffset.Value = 0.3d;
            variations[2] = CreateVariation(luminanceModifier, luminanceOffset);
            colorStyle.Variations = variations;

            colorStyle.Extensions = new StringToObjDictionary<DmlExtension>();
            // This is not a real extension of a color style.
            colorStyle.Extensions.Add(DmlExtensionUri.ThemeFamily, new DmlExtension(DmlExtensionUri.ThemeFamily,
                "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\" " +
                    "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                        "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                        "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" />" +
                "</a:ext> "));

            chartSpace.ColorStyle = colorStyle;

            DocxExportContext context = new DocxExportContext(doc, ChartColorStylePart,
                OoxmlComplianceCore.IsoTransitional);

            XmlNode styleNode = context.XmlDocument.ChildNodes[0];
            Assert.That(styleNode.Attributes["id"].Value, Is.EqualTo("3"));
            Assert.That(styleNode.Attributes["meth"].Value, Is.EqualTo("cycle"));

            XmlNodeList styleSubNodes = styleNode.ChildNodes;
            Assert.That(styleSubNodes.Count, Is.EqualTo(6));

            Assert.That(styleSubNodes[0].LocalName, Is.EqualTo("schemeClr"));
            Assert.That(styleSubNodes[0].Attributes["val"].Value, Is.EqualTo("lt1"));

            Assert.That(styleSubNodes[1].LocalName, Is.EqualTo("prstClr"));
            Assert.That(styleSubNodes[1].Attributes["val"].Value, Is.EqualTo("azure"));

            Assert.That(styleSubNodes[2].LocalName, Is.EqualTo("variation"));
            Assert.That(styleSubNodes[2].HasChildNodes, Is.False);

            XmlNode variationNode = styleSubNodes[3];
            Assert.That(variationNode.LocalName, Is.EqualTo("variation"));
            XmlNodeList variationChildren = variationNode.ChildNodes;
            Assert.That(variationChildren.Count, Is.EqualTo(1));
            Assert.That(variationChildren[0].LocalName, Is.EqualTo("lumMod"));
            Assert.That(variationChildren[0].Attributes["val"].Value, Is.EqualTo("50000"));

            variationNode = styleSubNodes[4];
            Assert.That(variationNode.LocalName, Is.EqualTo("variation"));
            variationChildren = variationNode.ChildNodes;
            Assert.That(variationChildren.Count, Is.EqualTo(2));
            Assert.That(variationChildren[0].LocalName, Is.EqualTo("lumMod"));
            Assert.That(variationChildren[0].Attributes["val"].Value, Is.EqualTo("80000"));
            Assert.That(variationChildren[1].LocalName, Is.EqualTo("lumOff"));
            Assert.That(variationChildren[1].Attributes["val"].Value, Is.EqualTo("30000"));

            Assert.That(styleSubNodes[5].LocalName, Is.EqualTo("extLst"));
            Assert.That(styleSubNodes[5].ChildNodes[0].ChildNodes[0].LocalName, Is.EqualTo("themeFamily"));
        }

        /// <summary>
        /// Creates a color style variation object and puts the specified color modifiers into it.
        /// </summary>
        private DmlChartColorStyleVariation CreateVariation(params IDmlColorModifier[] modifiers)
        {
            DmlChartColorStyleVariation result = new DmlChartColorStyleVariation();
            foreach (IDmlColorModifier modifier in modifiers)
                result.ColorModifiers.Add(modifier);
            return result;
        }

        /// <summary>
        /// Tests writing a style part of a chart.
        /// </summary>
        [Test]
        public void TestWritingStyle()
        {
            Document doc = CreateDocumetnWithChart();

            DmlChartSpace chartSpace = (DmlChartSpace)doc.FirstSection.Body.Shapes[0].DmlNode;
            chartSpace.DmlChartStyle = new DmlChartStyle();
            PrepareTestChartStyle(chartSpace.DmlChartStyle);

            DocxExportContext context = new DocxExportContext(doc, ChartStylePart,
                OoxmlComplianceCore.IsoTransitional);

            CheckChartStylePart(context.XmlDocument);
        }

        /// <summary>
        /// Puts test data into the specified chart style object.
        /// </summary>
        private static void PrepareTestChartStyle(DmlChartStyle style)
        {
            // For testing properties of the DmlChartStyle class.
            style.Id = "3";

            style[DmlChartStyleItem.DataLabelCallout] = new DmlChartStyleEntry();
            style.DataPointMarkerLayout = new DmlChartMarkerLayout();

            int matrixIndex = 1;
            foreach (DmlChartStyleItem index in DmlChartStyle.AllStyleItems)
            {
                style[index].LineReference.StyleMatrixIndex = matrixIndex;
                matrixIndex++;
            }

            style.Extensions = new StringToObjDictionary<DmlExtension>();
            style.Extensions.Add(DmlExtensionUri.ThemeFamily, new DmlExtension(DmlExtensionUri.ThemeFamily,
                "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\" " +
                    "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                    "<thm15:themeFamily xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" " +
                        "name=\"Office Theme\" id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                        "vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\"/> " +
                "</a:ext> "));

            // For testing properties of the DmlChartStyleEntry class.
            DmlChartStyleEntry styleEntry = style[DmlChartStyleItem.CategoryAxis];

            styleEntry.LineWidthScale = 1.5d;
            styleEntry.FillReference.StyleMatrixIndex = 100;
            styleEntry.EffectReference.StyleMatrixIndex = 101;
            styleEntry.FontReference.FontCollectionIndex = DmlFontCollectionIndex.None;

            styleEntry.ShapePr = new DefaultShapeProperties();
            styleEntry.ShapePr.Fill = new DmlSolidFill(new DmlPresetColor("red"));

            styleEntry.DefaultRunPr = new DmlRunProperties();
            styleEntry.DefaultRunPr.Kerning = new DmlTextPoints(1150);

            styleEntry.TextBodyPr = new DmlTextBodyProperties();
            styleEntry.TextBodyPr.RightInset = 30000;

            styleEntry.Extensions = new StringToObjDictionary<DmlExtension>();
            styleEntry.Extensions.Add(DmlExtensionUri.HiddenEffects, new DmlExtension(DmlExtensionUri.HiddenEffects,
                "<a:ext uri=\"{AF507438-7753-43E0-B8FC-AC1667EBCBE1}\" " +
                    "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                    "<a14:hiddenEffects " +
                        "xmlns:a14=\"http://schemas.microsoft.com/office/drawing/2010/main\"> " +
                        "<a:effectLst> " +
                            "<a:outerShdw algn=\"ctr\" dir=\"2700000\" dist=\"35921\" rotWithShape=\"0\"> " +
                                "<a:schemeClr val=\"bg2\" /> " +
                            "</a:outerShdw> " +
                        "</a:effectLst> " +
                    "</a14:hiddenEffects> " +
                "</a:ext>"));

            styleEntry.Modifiers = new string[] { "allowNoFillOverride", "allowNoLineOverride" };

            // For testing properties of the DmlChartStyleReference class.
            DmlChartStyleReference styleReference = style[DmlChartStyleItem.AxisTitle].LineReference;
            styleReference.Color = new DmlPresetColor("aqua");
            styleReference.Modifiers = new string[] { "ignoreCSTransforms", "auto" };

            // For testing properties of the DmlChartFontReference class.
            DmlChartFontReference fontReference = style[DmlChartStyleItem.AxisTitle].FontReference;
            fontReference.FontCollectionIndex = DmlFontCollectionIndex.Minor;
            fontReference.Color = new DmlSchemeColor(ThemeColor.Text1);
            fontReference.Modifiers = new string[] { "ignoreCSTransforms", "auto" };

            // For testing properties of the DmlChartStyleColor class.
            DmlChartStyleColor styleColor = new DmlChartStyleColor();
            styleColor.Value = "auto";

            DmlLuminanceModulation luminanceModulation = new DmlLuminanceModulation();
            luminanceModulation.Value = 0.5d;
            styleColor.ColorModifiers.Add(luminanceModulation);

            DmlLuminanceOffset luminanceOffset = new DmlLuminanceOffset();
            luminanceOffset.Value = 0.8d;
            styleColor.ColorModifiers.Add(luminanceOffset);

            style[DmlChartStyleItem.AxisTitle].EffectReference.Color = styleColor;

            // For testing properties of the DmlChartMarkerLayout class.
            DmlChartMarkerLayout markerLayout = style.DataPointMarkerLayout;
            markerLayout.Symbol = "cycle";
            markerLayout.Size = 11;
        }

        /// <summary>
        /// Checks XML of a chart style part that is filled by <see cref="PrepareTestChartStyle"/>.
        /// </summary>
        private static void CheckChartStylePart(XmlDocument xmlDocument)
        {
            XmlNode styleNode = xmlDocument.ChildNodes[0];

            // Testing properties of the DmlChartStyle class.
            Assert.That(styleNode.Attributes["id"].Value, Is.EqualTo("3"));

            XmlNodeList styleSubNodes = styleNode.ChildNodes;
            Assert.That(styleSubNodes.Count, Is.EqualTo(32));

            int expectedId = 1;
            for (int i = 0; i < styleSubNodes.Count; i++)
            {
                if ((styleSubNodes[i].LocalName != "dataPointMarkerLayout") &&
                    (styleSubNodes[i].LocalName != "extLst"))
                {
                    Assert.That(styleSubNodes[i].ChildNodes[0].Attributes["idx"].Value, Is.EqualTo(FormatterPal.IntToXml(expectedId)));
                    expectedId++;
                }
            }

            XmlNode extensionsNode = styleNode["cs:extLst"];
            Assert.That(extensionsNode, IsNot.Null());
            Assert.That(extensionsNode.ChildNodes[0].Attributes["uri"].Value, Is.EqualTo(DmlExtensionUri.ThemeFamily));

            // Testing properties of the DmlChartStyleEntry class.
            XmlNode styleEntryNode = styleNode["cs:categoryAxis"];

            Assert.That(styleEntryNode["cs:lineWidthScale"].InnerText, Is.EqualTo("1.5"));
            Assert.That(styleEntryNode["cs:fillRef"].Attributes["idx"].Value, Is.EqualTo("100"));
            Assert.That(styleEntryNode["cs:effectRef"].Attributes["idx"].Value, Is.EqualTo("101"));
            Assert.That(styleEntryNode["cs:fontRef"].Attributes["idx"].Value, Is.EqualTo("none"));

            XmlNode shapePrFillNode = styleEntryNode["cs:spPr"]["a:solidFill"];
            Assert.That(shapePrFillNode, IsNot.Null());
            Assert.That(shapePrFillNode["a:prstClr"], IsNot.Null());
            Assert.That(shapePrFillNode["a:prstClr"].Attributes["val"].Value, Is.EqualTo("red"));

            XmlNode defaultRunPrNode = styleEntryNode["cs:defRPr"];
            Assert.That(defaultRunPrNode, IsNot.Null());
            Assert.That(defaultRunPrNode.Attributes["kern"].Value, Is.EqualTo("1150"));

            XmlNode bodyPrNode = styleEntryNode["cs:bodyPr"];
            Assert.That(bodyPrNode, IsNot.Null());
            Assert.That(bodyPrNode.Attributes["rIns"].Value, Is.EqualTo("30000"));

            extensionsNode = styleEntryNode["cs:extLst"];
            Assert.That(extensionsNode, IsNot.Null());
            Assert.That(extensionsNode.ChildNodes[0].Attributes["uri"].Value, Is.EqualTo(DmlExtensionUri.HiddenEffects));

            Assert.That(styleEntryNode.Attributes["mods"].Value, Is.EqualTo("allowNoFillOverride allowNoLineOverride"));

            // Testing properties of the DmlChartStyleReference class.
            XmlNode axisTitleNode = styleNode["cs:axisTitle"];
            XmlNode styleReferenceNode = axisTitleNode["cs:lnRef"];

            Assert.That(styleReferenceNode.ChildNodes[0].Name, Is.EqualTo("a:prstClr"));
            Assert.That(styleReferenceNode.ChildNodes[0].Attributes["val"].Value, Is.EqualTo("aqua"));

            Assert.That(styleReferenceNode.Attributes["mods"].Value, Is.EqualTo("ignoreCSTransforms auto"));

            // Testing properties of the DmlChartFontReference class.
            XmlNode fontReferenceNode = axisTitleNode["cs:fontRef"];

            Assert.That(fontReferenceNode.ChildNodes[0].Name, Is.EqualTo("a:schemeClr"));
            Assert.That(fontReferenceNode.ChildNodes[0].Attributes["val"].Value, Is.EqualTo("tx1"));

            Assert.That(styleReferenceNode.Attributes["mods"].Value, Is.EqualTo("ignoreCSTransforms auto"));

            // Testing properties of the DmlChartStyleColor class.
            XmlNode styleColorNode = axisTitleNode["cs:effectRef"]["cs:styleClr"];
            Assert.That(styleColorNode.Attributes["val"].Value, Is.EqualTo("auto"));

            Assert.That(styleColorNode.ChildNodes.Count, Is.EqualTo(2));
            Assert.That(styleColorNode.ChildNodes[0].Name, Is.EqualTo("a:lumMod"));
            Assert.That(styleColorNode.ChildNodes[0].Attributes["val"].Value, Is.EqualTo("50000"));
            Assert.That(styleColorNode.ChildNodes[1].Name, Is.EqualTo("a:lumOff"));
            Assert.That(styleColorNode.ChildNodes[1].Attributes["val"].Value, Is.EqualTo("80000"));

            // Testing properties of the DmlChartMarkerLayout class.
            XmlNode markerLayoutNode = styleNode["cs:dataPointMarkerLayout"];
            Assert.That(markerLayoutNode.Attributes["symbol"].Value, Is.EqualTo("cycle"));
            Assert.That(markerLayoutNode.Attributes["size"].Value, Is.EqualTo("11"));
        }

        /// <summary>
        /// Creates a document with a chart.
        /// </summary>
        private static Document CreateDocumetnWithChart()
        {
            Document doc = new Document();
            DocumentBuilder documentBuilder = new DocumentBuilder(doc);

            documentBuilder.InsertChart(ChartType.Area, 200, 200);

            return doc;
        }

        private const string ChartColorStylePart = "word/charts/chart/colors1.xml";
        private const string ChartStylePart = "word/charts/chart/style1.xml";
    }
}
