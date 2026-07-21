// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov
using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.Tests.Model;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlTextShapeReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlTextShapeReader
    {
      
        [Test]
        public void Build_BadXml_NullReturned()
        {
            // Arrange
            string xml =
                "<a:Badsp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "</a:Badsp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Build_UseSpRectAndTransform_PropertiesDefined()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<xfrm>" +
                        "<ext cx=\"12\" cy=\"32\"/>" +
                        "<off x=\"54\" y=\"72\"/>" +
                    "</xfrm>" +
                    "<useSpRect/>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlTransform transform = result.Transform;
            Assert.That(transform.Width, Is.EqualTo(12));
            Assert.That(transform.Height, Is.EqualTo(32));
            Assert.That(transform.X, Is.EqualTo(54));
            Assert.That(transform.Y, Is.EqualTo(72));
            Assert.That(result.UseShapeTextRectangle, Is.True);
        }

        [Test]
        public void Build_ParagraphStructure_ObjectsCreated()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r/>" +
                            "<br/>" +
                            "<fld/>" +
                            "<m/>" +
                        "</p>" +
                        "<p/>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            List<DmlParagraph> paragraphs = result.TextBody.Paragraphs;
            Assert.That(paragraphs.Count, Is.EqualTo(2));
            DmlParagraph paragraph = (DmlParagraph)paragraphs[0];
            Assert.That(paragraph.Elements[0] is DmlRun, Is.True);
            Assert.That(paragraph.Elements[1] is DmlTextBreakLine, Is.True);
            Assert.That(paragraph.Elements[2] is DmlTextField, Is.True);
            Assert.That(paragraph.Elements[3] is DmlTextMath, Is.True);
        }

        [Test]
        public void Build_TextBodyProperties_PropertiesReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<bodyPr anchor=\"ctr\" anchorCtr=\"1\" lIns=\"1\" tIns=\"2\" rIns=\"3\" bIns=\"4\" "+
                                "compatLnSpc=\"1\" forceAA=\"1\" fromWordArt=\"1\" horzOverflow=\"clip\" "+
                                "numCol=\"4\" rot=\"540000\" rtlCol=\"1\" spcCol=\"3\" spcFirstLastPara=\"1\" "+
                                "upright=\"1\" vert=\"wordArtVertRtl\" vertOverflow=\"clip\" wrap=\"none\">" +
                        "</bodyPr>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlTextBodyProperties props = result.TextBody.Properties;
            Assert.That(props.Anchor, Is.EqualTo(DmlTextAnchoringType.Center));
            Assert.That(props.AnchorCenter, Is.EqualTo(true));
            Assert.That(props.LeftInset, Is.EqualTo(1));
            Assert.That(props.TopInset, Is.EqualTo(2));
            Assert.That(props.RightInset, Is.EqualTo(3));
            Assert.That(props.BottomInset, Is.EqualTo(4));
            Assert.That(props.UseCompatibleLineSpacing, Is.EqualTo(true));
            Assert.That(props.ForceAntiAlias, Is.EqualTo(true));
            Assert.That(props.FromWordArt, Is.EqualTo(true));
            Assert.That(props.TextHorizontalOverflow, Is.EqualTo(DmlTextHorizontalOverflowType.Clip));
            Assert.That(props.ColumnNumber, Is.EqualTo(4));
            Assert.That(props.Rotation, Is.EqualTo(new DmlAngle(540000)));
            Assert.That(props.ColumnOrder, Is.EqualTo(DmlTextColumnOrder.RightToLeft));
            Assert.That(props.SpaceBetweenColumns, Is.EqualTo(3));
            Assert.That(props.AreFirstAndLastParagraphsUseSpacing, Is.EqualTo(true));
            Assert.That(props.IsTextUpright, Is.EqualTo(true));
            Assert.That(props.TextOrientation, Is.EqualTo(ShapeTextOrientation.WordArtVerticalRightToLeft));
            Assert.That(props.TextVerticalOverflow, Is.EqualTo(DmlTextVerticalOverflowType.Clip));
            Assert.That(props.TextWrappingType, Is.EqualTo(TextBoxWrapMode.None));
        }

        [Test]
        public void Build_ListStyles_ListStylesReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<lstStyle>" +
                            "<defPPr lvl=\"8\"/>" +
                            "<lvl1pPr lvl=\"0\"/>" +
                            "<lvl2pPr lvl=\"1\"/>" +
                            "<lvl3pPr lvl=\"2\"/>" +
                            "<lvl4pPr lvl=\"3\"/>" +
                            "<lvl5pPr lvl=\"4\"/>" +
                            "<lvl6pPr lvl=\"5\"/>" +
                            "<lvl7pPr lvl=\"6\"/>" +
                            "<lvl8pPr lvl=\"7\"/>" +
                            "<lvl9pPr lvl=\"8\"/>" +
                        "</lstStyle>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlTextListStyles styles = result.TextBody.TextListStyles;
            Assert.That(styles.DefaultParagraphProperties.Level, Is.EqualTo(8));
            for (int i = 0; i < 9; i++)
            {
                DmlParagraphProperties props = styles.GetTextListStyle(i);
                Assert.That(props.Level, Is.EqualTo(i));
            }

        }

        [Test]
        public void Build_RunDefined_RunCreated()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr>" +
                                "</rPr>" +
                                "<t>" +
                                    "Sample text" +
                                "</t>" +
                                "<t>" +
                                    "Sample text" +
                                "</t>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlParagraph paragraph = (DmlParagraph)result.TextBody.Paragraphs[0];
            DmlRun run = (DmlRun)paragraph.Elements[0];
            Assert.That(run.Text, Is.EqualTo("Sample text" + "Sample text"));
            Assert.That(run.RunProperties, IsNot.Null());
        }

        [Test]
        public void Build_BreakLineDefined_BreakLineCreated()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<br>" +
                                "<rPr>" +
                                "</rPr>" +
                            "</br>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlParagraph paragraph = (DmlParagraph)result.TextBody.Paragraphs[0];
            DmlTextBreakLine breakLine = (DmlTextBreakLine)paragraph.Elements[0];
            Assert.That(breakLine.RunProperties, IsNot.Null());
        }

        [Test] 
        public void Build_TextFieldDefined_TextFieldCreated()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<fld id=\"{7E6A6A03-33E7-441B-A8C6-65955AE3D3C3}\" type=\"yellow\">" +
                                "<rPr>" +
                                "</rPr>" +
                                "<pPr>" +
                                "</pPr>" +
                                "<t>" +
                                    "Sample text" +
                                "</t>" +
                                "<t>" +
                                    "Sample text" +
                                "</t>" +
                            "</fld>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlParagraph paragraph = (DmlParagraph)result.TextBody.Paragraphs[0];
            DmlTextField field = (DmlTextField)paragraph.Elements[0];
            Assert.That(field.RunProperties, IsNot.Null());
            Assert.That(field.ParagraphProperties, IsNot.Null());
            Assert.That(field.Text, Is.EqualTo("Sample text" + "Sample text"));
            Assert.That(field.Id , Is.EqualTo(new Guid("7E6A6A03-33E7-441B-A8C6-65955AE3D3C3")));
            Assert.That(field.Type, Is.EqualTo("yellow"));
        }

        [Test]
        public void Build_ParagraphProperties_PropertiesReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<pPr algn=\"dist\" eaLnBrk=\"0\" defTabSz=\"7\" " +
                                 "fontAlgn=\"ctr\" hangingPunct=\"1\" indent=\"8\" latinLnBrk=\"0\" lvl=\"6\" "+
                                 "marL=\"5\" marR=\"4\" rtl=\"1\">" +
                                 "<defRPr/>" +
                                 "<lnSpc><spcPct val=\"101000\"/></lnSpc>" +
                                 "<spcAft><spcPts val=\"100\"/></spcAft>" +
                                 "<spcBef><spcPts val=\"200\"/></spcBef>" +
                                 "<tabLst> + " +
                                    "<tab pos = \"82550\" algn = \"dec\" />" +
                                    "<tab pos = \"179388\" algn = \"ctr\" /> + " +
                                 "</tabLst>" +
                            "</pPr>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlParagraph paragraph = (DmlParagraph)result.TextBody.Paragraphs[0];
            DmlParagraphProperties props = paragraph.Properties;
            Assert.That(props.Alignment, Is.EqualTo(ParagraphAlignment.Distributed));
            Assert.That(props.IsEastAsianLineBreakAllowed, Is.EqualTo(false));
            Assert.That(props.DefaultTabSize, Is.EqualTo(7));
            Assert.That(props.FontAlignment, Is.EqualTo(DmlFontAlignment.Center));
            Assert.That(props.IsHangingPunctuationAllowed, Is.EqualTo(true));
            Assert.That(props.TextIdentation, Is.EqualTo(8));
            Assert.That(props.IsLatinLineBreakAllowed, Is.EqualTo(false));
            Assert.That(props.Level, Is.EqualTo(6));
            Assert.That(props.LeftMargin, Is.EqualTo(5));
            Assert.That(props.RightMargin, Is.EqualTo(4));
            Assert.That(props.RightToLeftFlowDirection, Is.EqualTo(true));
            Assert.That(props.DefaultRunProperties, IsNot.Null());
            Assert.That(((DmlPercentageTextSpacing)props.LineSpacing).Value, Is.EqualTo(DmlPercentageUtil.FromDmlPercent(101000)));
            Assert.That(((DmlPointsTextSpacing)props.SpaceAfter).Value, Is.EqualTo(new DmlTextPoints(100)));
            Assert.That(((DmlPointsTextSpacing)props.SpaceBefore).Value, Is.EqualTo(new DmlTextPoints(200)));
            Assert.That(props.TabList, IsNot.Null());
            Assert.That(props.TabList.Count, Is.EqualTo(2));
            Assert.That(props.TabList[0].PositionTwips, Is.EqualTo(82550));
            Assert.That(props.TabList[0].Alignment, Is.EqualTo(TabAlignment.Decimal));
            Assert.That(props.TabList[1].PositionTwips, Is.EqualTo(179388));
            Assert.That(props.TabList[1].Alignment, Is.EqualTo(TabAlignment.Center));

        }

        [Test]
        public void Build_RunProperties_PropertiesReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr altLang=\"ru-RU\" b=\"1\" baseline=\"100\" bmk=\"e\" cap=\"all\" " +
                                    "dirty=\"1\" err=\"1\" i=\"true\" kern=\"100\" kumimoji=\"1\" lang=\"ru\" "+
                                    "noProof=\"1\" normalizeH=\"1\" smtClean=\"1\" smtId=\"4\" spc=\"101\" "+
                                    "strike=\"sngStrike\" sz=\"102\" u=\"dotDotDashHeavy\">" +
                                    "<solidFill><schemeClr val=\"accent1\" /></solidFill>"+
                                    "<ln/>" +
                                    "<highlight><srgbClr val=\"FFFF00\"/></highlight>" +
                                    "<uFill><solidFill><prstClr val=\"blue\" /></solidFill></uFill>" +
                                    "<uLn><solidFill><prstClr val=\"red\" /></solidFill></uLn>" +
                                    "<hlinkClick id=\"rId5\" tgtFrame=\"hlinkClickTgtFrame\" tooltip=\"hlinkClickTooltip\"/>" +
                                    "<hlinkMouseOver id=\"rId6\" tgtFrame=\"hoverTgtFrame\" tooltip=\"hoverTooltip\"/>" +
                                    "<rtl/>" +
                                "</rPr>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlParagraph paragraph = (DmlParagraph)result.TextBody.Paragraphs[0];
            DmlRun run = (DmlRun)paragraph.Elements[0];
            DmlRunProperties props = run.RunProperties;
            Assert.That(props.AlternativeLanguage, Is.EqualTo(Language.RussianRussia));
            Assert.That(props.Bold, Is.EqualTo(true));
            Assert.That(props.Baseline, Is.EqualTo(DmlPercentageUtil.FromDmlPercent(100)));
            Assert.That(props.BookmarkLinkTarget, Is.EqualTo("e"));
            Assert.That(props.Capitalization, Is.EqualTo(DmlCapitalization.All));
            Assert.That(props.IsDirty, Is.EqualTo(true));
            Assert.That(props.HasSpellingError, Is.EqualTo(true));
            Assert.That(props.Italics, Is.EqualTo(true));
            Assert.That(props.Kerning, Is.EqualTo(new DmlTextPoints(100)));
            Assert.That(props.Kumimoji, Is.EqualTo(true));
            Assert.That(props.Language, Is.EqualTo(Language.Russian));
            Assert.That(props.NoProofing, Is.EqualTo(true));
            Assert.That(props.NormalizeHeights, Is.EqualTo(true));
            Assert.That(props.SmartTagsClean, Is.EqualTo(true));
            Assert.That(props.SmartTagID, Is.EqualTo(4));
            Assert.That(props.Spacing, Is.EqualTo(new DmlTextPoints(101)));
            Assert.That(props.Strikethrough, Is.EqualTo(DmlTextStrike.Single));
            Assert.That(props.FontSize, Is.EqualTo(new DmlTextPoints(102)));
            Assert.That(props.Underline, Is.EqualTo(Underline.DotDotDashHeavy));
            Assert.That(props.Fill is DmlSolidFill, Is.True);
            Assert.That(props.Outline, IsNot.Null());
            Assert.That(props.HighlightColor is DmlHexRgbColor, Is.True);
            Assert.That(props.HlinkClick, IsNot.Null());
            DmlHlink hlinkClick = props.HlinkClick;
            Assert.That(hlinkClick.Id, Is.EqualTo("reltaget/rId5"));
            Assert.That(hlinkClick.TargetFrame, Is.EqualTo("hlinkClickTgtFrame"));
            Assert.That(hlinkClick.Tooltip, Is.EqualTo("hlinkClickTooltip"));
            DmlHlink hlinkHover = props.HlinkHover;
            Assert.That(hlinkHover.Id, Is.EqualTo("reltaget/rId6"));
            Assert.That(hlinkHover.TargetFrame, Is.EqualTo("hoverTgtFrame"));
            Assert.That(hlinkHover.Tooltip, Is.EqualTo("hoverTooltip"));
            Assert.That(props.UnderlineFill is DmlSolidFill, Is.True);
            Assert.That(props.UnderlineStroke.Fill is DmlSolidFill, Is.True);
            Assert.That(props.RightToLeftFlowDirection, Is.True);
        }

        [Test]
        public void Build_RunPropertiesWithEasternAsianFont_FontReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr>" +
                                    "<ea typeface=\"Sample Font\" charset=\"1\" panose=\"0A\" pitchFamily=\"1\" />" +
                                "</rPr>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlFont font = GetFont(result, DmlFontType.EastAsian);
            CheckFontProperties(font);
        }

        [Test]
        public void Build_RunPropertiesWithLatinFont_FontReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr>" +
                                    "<latin typeface=\"Sample Font\" charset=\"1\" panose=\"0A\" pitchFamily=\"1\" />" +
                                "</rPr>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlFont font = GetFont(result, DmlFontType.Latin);
            CheckFontProperties(font);
        }

        [Test]
        public void Build_RunPropertiesWithSymbolFont_FontReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr>" +
                                    "<sym typeface=\"Sample Font\" charset=\"1\" panose=\"0A\" pitchFamily=\"1\"/>" +
                                "</rPr>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlFont font = GetFont(result, DmlFontType.Symbol);
            CheckFontProperties(font);
        }

        [Test]
        public void Build_RunPropertiesWithComplexScriptFont_FontReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr>" +
                                    "<cs typeface=\"Sample Font\" charset=\"1\" panose=\"0A\" pitchFamily=\"1\"/>" +
                                "</rPr>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            // Act
            DmlTextShape result = Read(xml);
            // Assert
            DmlFont font = GetFont(result, DmlFontType.ComplexScript);
            CheckFontProperties(font);
        }

        private DmlFont GetFont(DmlTextShape textShape, DmlFontType fontType)
        {
            DmlParagraph paragraph = (DmlParagraph)textShape.TextBody.Paragraphs[0];
            DmlRun run = (DmlRun)paragraph.Elements[0];
            DmlRunProperties props = run.RunProperties;
            return props.GetFont(fontType);
        }

        private void CheckFontProperties(DmlFont font)
        {
            Assert.That(font.TextTypeface, Is.EqualTo("Sample Font"));
            Assert.That(font.SimilarCharacterSet, Is.EqualTo(1));
            Assert.That(font.PanoseSetting, Is.EqualTo("0A"));
            Assert.That(font.SimilarFontFamily, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-11899 DmlTextShapeReader.ReadFont hanged if there was an unknown attribute in font element.  
        /// WarnUnexcepted should be used instead of WarnUnexceptedAndIgnoreElement in attribute cycle.
        /// </summary>
        [Test]
        public void RunPropertiesWithFontUnknownAttribute_FontReaded()
        {
            // Arrange
            string xml =
                "<txSp>" +
                    "<txBody>" +
                        "<p>" +
                            "<r>" +
                                "<rPr>" +
                                    "<cs charset1=\"1\" />" +   // unknown attribute charset1
                                "</rPr>" +
                            "</r>" +
                        "</p>" +
                    "</txBody>" +
                "</txSp>";
            TestMaxWarningCountCallback warnCounter = new TestMaxWarningCountCallback(1);
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml, warnCounter);
            // Act
            DmlTextShapeReader.Read(reader);
            // Assert
            Assert.That(warnCounter.Count, Is.EqualTo(1), "Number of unknown nodes");
        }

        internal DmlTextShape Read(string xml)
        {
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            return DmlTextShapeReader.Read(reader);
        }
    }
}
