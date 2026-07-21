// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2012 by Denis Darkin
using System;
using System.Collections.Generic;
using Aspose.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;
using List = Aspose.Words.Lists.List;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test various operations related to managing styles in the collection using public API. Add more operations to lists when supported:
    /// - Setting Name property of a style
    /// - Performing AddCopy on a style
    /// - Comparing styles.
    /// </summary>
    [TestFixture]
    public class TestStyleManagement
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }


        /// <summary>
        /// Verify that paragraph style istd is updated in list levels, when the style changes the istd due to name change.
        /// </summary>
        [Test]
        public void TestListStyleUpdated()
        {
            Document src = TestUtil.Open(@"Model\Revision\TestStyleRevisions.docx");
            StyleCollection sourceStyles = src.Styles;
            Document target = new Document();
            Style addedStyle = target.Styles.AddCopy(sourceStyles["Intense Quote"]);
            Aspose.Words.Lists.List list = target.Lists.FetchListByListId(addedStyle.ParaPr.ListId);
            Assert.That(list.ListLevels[0].ParaStyleIstd, Is.EqualTo(addedStyle.Istd));
            Assert.That(list.ListLevels[0].ParaStyleIstd, IsNot.EqualTo(sourceStyles["Intense Quote"].Istd));
        }


        /// <summary>
        /// Verify renaming a style to built-in (with istd change), changes istd in styles that were based on it.
        /// </summary>
        [Test]
        public void TestRenameStyleBasedOn()
        {
            DocumentBuilder builder = new DocumentBuilder();
            StyleCollection styles = builder.Document.Styles;
            Style baseStyle = Style.Create(StyleType.Paragraph, styles.GetNextFreeIstd(), StyleIdentifier.User, "BaseStyle");
            baseStyle.RunPr.Color = DrColor.Coral;
            styles.Add(baseStyle);
            builder.ParagraphFormat.Istd = baseStyle.Istd;
            builder.Writeln("My text of Normal style. ");

            Style childStyle = Style.Create(StyleType.Paragraph, styles.GetNextFreeIstd(), StyleIdentifier.User, "DerivedStyle");
            styles.Add(childStyle);
            childStyle.BasedOnIstd = baseStyle.Istd;
            childStyle.RunPr.Italic = AttrBoolEx.True;
            builder.ParagraphFormat.Istd = childStyle.Istd;
            builder.Writeln("My text of Derived style.");

            baseStyle.Name = "BaseStyleNew";
            Assert.That(childStyle.BasedOnIstd, Is.EqualTo(baseStyle.Istd));

            baseStyle.Name = "Normal";
            Assert.That(childStyle.BasedOnIstd, Is.EqualTo(baseStyle.Istd));
            Assert.That(childStyle.BasedOnIstd, Is.EqualTo(0));

            TestUtil.SaveOpen(builder.Document, @"Model\Style\RenameBasedOnStyle.docx");
        }



        /// <summary>
        /// WORDSNET-7523 Style.Name throws NullReferenceException during copying style name from another Document.
        /// StyleCollection.ProcessListStyle had a wrong assumption that para.ListFormat.List.Style is always non-null.
        /// </summary>
        [Test]
        public void TestJira7523()
        {
            const string testName = @"Model\Style\TestJira7523.docx";
            Document target = TestUtil.Open(testName);
            Document source = new Document();

            Style listStyle = source.Styles["No List"];
            Style targetListStyle = target.Styles.AddCopy(listStyle);
            targetListStyle.Name = listStyle.Name;

            // Exception would have happened before this line.
            Assert.That(((Paragraph)target.GetChild(NodeType.Paragraph, 0, true)).ListFormat.List.Style, Is.Null);
        }

        /// <summary>
        /// WORDSNET-7404 Style.Name do not change the style name.
        /// StyleIndex.BuiltInStyleNameEnglish handled Built-in style names incorrectly.
        /// </summary>
        [Test]
        public void TestJira7404()
        {
            const string testName = @"Model\Style\TestJira7404";
            Document sourceDoc = TestUtil.Open(testName + "Source.docx");
            const string sourceName = "Heading 1";
            Style sourceStyle = sourceDoc.Styles[sourceName];

            Document targetDoc = TestUtil.Open(testName + "Target.docx");
            Style addedStyle = targetDoc.Styles.AddCopy(sourceStyle);
            Assert.That(addedStyle.BuiltIn, Is.False);
            int styleCount = targetDoc.Styles.Count;

            addedStyle.Name = sourceName;
            Assert.That(targetDoc.Styles.Count, Is.EqualTo(styleCount - 1)); // make sure we removed "Heading 1_0" when renaming to "Heading 1"
            Assert.That(addedStyle.BuiltIn, Is.True);
            Assert.That(addedStyle.Name, Is.EqualTo(sourceName));
        }

        /// <summary>
        /// Tests setting empty name for the style throws an exception.
        /// </summary>
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Style name can not be empty.")]
        [Test]
        public void TestStyleSetNullName()
        {
            StyleCollection collection = new Document().Styles;
            Style result = collection.Add(StyleType.Paragraph, "myStyle");
            result.Name = "";
        }

        /// <summary>
        /// Tests changing 'Normal' is correct.
        /// </summary>
        [Test]
        public void TestBuiltInStyles()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Document doc = builder.Document;
            StyleCollection styles = doc.Styles;

            styles["Normal"].RunPr.SetAttr(FontAttr.Italic, AttrBoolEx.True);

            Style result = styles.Add(StyleType.Paragraph, "myStyle");
            result.RunPr.SetAttr(FontAttr.Italic, AttrBoolEx.True);

            result.Name = "Normal";
            Assert.That(result.BuiltIn, Is.True);
            Assert.That(((AttrBoolEx)styles["Normal"].RunPr[FontAttr.Italic]).ToBool(), Is.True);
            builder.CurrentParagraph.ParagraphFormat.Style = result;
            builder.Write("Test");
            Assert.That((int)((Paragraph)doc.GetChild(NodeType.Paragraph, 0, true)).ParaPr[ParaAttr.Istd], Is.EqualTo(0));
            TestUtil.SaveOpen(doc, @"Model\Style\TestSetUserToBuiltInStyle.docx");

            result.Name = "myStyle";
            Assert.That(result.BuiltIn, Is.False);
            Assert.That(((AttrBoolEx)styles["myStyle"].RunPr[FontAttr.Italic]).ToBool(), Is.True);
            Assert.That((int)((Paragraph)doc.GetChild(NodeType.Paragraph, 0, true)).ParaPr[ParaAttr.Istd], Is.EqualTo(15));
            TestUtil.SaveOpen(doc, @"Model\Style\TestSetBuiltInToUserStyle.docx");

            result.Name = "Normal";
            Assert.That(result.BuiltIn, Is.True);
            Assert.That(((AttrBoolEx)styles["Normal"].RunPr[FontAttr.Italic]).ToBool(), Is.True);
            Assert.That((int)((Paragraph)doc.GetChild(NodeType.Paragraph, 0, true)).ParaPr[ParaAttr.Istd], Is.EqualTo(0));
            TestUtil.SaveOpen(doc, @"Model\Style\TestSetUserToBuiltInStyle2.docx");
        }

        /// <summary>
        /// Test "overwrite" style by renaming to existing name.
        /// Verify collection access.
        /// </summary>
        [Test]
        public void TestStyleSetExistingName()
        {
            Document doc = new Document();
            StyleCollection collection = doc.Styles;
            collection.Add(StyleType.Paragraph, "myStyle");
            Style result = collection.Add(StyleType.Paragraph, "myStyle1");
            result.Name = "myStyle";
            Assert.That(collection["myStyle1"], Is.Null);
            Assert.That(collection["myStyle"], IsNot.Null());
        }

        /// <summary>
        /// Change style name and verify that it is successfully set.
        /// </summary>
        [Test]
        public void TestStyleSetRunRegularStyle()
        {
            const string testName = @"Model\Style\TestStyleSetRunRegularStyle";
            DocumentBuilder builder = new DocumentBuilder();
            StyleCollection collection = builder.Document.Styles;
            Style result = collection.Add(StyleType.Paragraph, "myStyle");
            builder.CurrentParagraph.ParagraphFormat.Style = result;
            builder.Write("test");
            Assert.That(((Paragraph)builder.Document.GetChild(NodeType.Paragraph, 0, true)).ParagraphStyle.Name, Is.EqualTo("myStyle"));
            TestUtil.SaveOpen(builder.Document, testName + @"1.docx");

            const string newName = "newName";
            result.Name = newName;
            Assert.That(collection[newName].Istd, Is.EqualTo(result.Istd));
            Assert.That(result.Name, Is.EqualTo(newName));
            Assert.That(((Paragraph)builder.Document.GetChild(NodeType.Paragraph, 0, true)).ParagraphStyle.Name, Is.EqualTo(newName));
            TestUtil.SaveOpen(builder.Document, testName + @"2.docx");
        }

        /// <summary>
        /// Tests renaming custom style to 'Normal'.
        /// </summary>
        [Test]
        public void TestStyleSetRunBuiltInStyle()
        {
            const string testName = @"Model\Style\TestStyleSetRunBuiltInStyle";
            DocumentBuilder builder = new DocumentBuilder();
            StyleCollection collection = builder.Document.Styles;
            Style result = collection.Add(StyleType.Paragraph, "myStyle");
            builder.CurrentParagraph.ParagraphFormat.Style = result;
            builder.Write("test");
            Assert.That(((Paragraph)builder.Document.GetChild(NodeType.Paragraph, 0, true)).ParagraphStyle.Name, Is.EqualTo("myStyle"));
            TestUtil.SaveOpen(builder.Document, testName + @"Regular.docx");

            const string newName = "Normal";
            result.Name = newName;
            Assert.That(collection[newName].Istd, Is.EqualTo(result.Istd));
            Assert.That(result.Name, Is.EqualTo(newName));
            Assert.That(((Paragraph)builder.Document.GetChild(NodeType.Paragraph, 0, true)).ParagraphStyle.Name, Is.EqualTo(newName));
            TestUtil.SaveOpen(builder.Document, testName + @"BuiltIn.docx");
        }

        /// <summary>
        /// Tests copying 'Normal' into the same document.
        /// </summary>
        [Test]
        public void TestAddCopyBuiltInStyleSameDoc()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            styles["Normal"].RunPr.SetAttr(FontAttr.Italic, AttrBoolEx.True);
            Style normal1 = styles.AddCopy(styles["Normal"]);
            Assert.That(normal1.Name, Is.EqualTo("Normal_0"));
            Assert.That(normal1.BuiltIn, Is.False);
            Assert.That(StyleIndex.Normal, IsNot.EqualTo(normal1.Istd));
            Assert.That(styles["Normal_0"], IsNot.Null());
            Assert.That(styles["Normal_0"].RunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));
            TestUtil.SaveOpen(doc, @"Model\Style\TestAddCopyBuiltInStyleSameDoc.docx");
        }

        /// <summary>
        /// Tests copying 'Normal' into another document.
        /// </summary>
        [Test]
        public void TestAddCopyBuiltInStyleDifferentDoc()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            Style normal1 = styles.AddCopy(new Document().Styles["Normal"]);
            Assert.That(normal1.Name, Is.EqualTo("Normal_0"));
            Assert.That(normal1.BuiltIn, Is.False);
            Assert.That(StyleIndex.Normal, IsNot.EqualTo(normal1.Istd));
            Assert.That(styles["Normal_0"], IsNot.Null());
            TestUtil.SaveOpen(doc, @"Model\Style\TestAddCopyBuiltInStyleDifferentDoc.docx");
        }

        /// <summary>
        /// AddCopy a style with revisions and make sure it is in the collection.
        /// </summary>
        [Test]
        public void TestAddStyleWithRevisions()
        {
            Document src = TestUtil.Open(@"Model\Revision\TestStyleRevisions.docx");
            RevisionCollection revisions = src.Revisions;
            Style revisedStyle = revisions[revisions.Count - 2].ParentStyle;
            Document doc = new Document();

            Assert.That(doc.Revisions.Count, Is.EqualTo(0));
            doc.Styles.AddCopy(revisedStyle);
            Assert.That(doc.Revisions.Count, Is.EqualTo(1));
            Assert.That(doc.Revisions[0].Author, Is.EqualTo("Denis"));
        }

        /// <summary>
        /// Tests copying style and renaming it to 'Normal'.
        /// </summary>
        [Test]
        public void TestAddCopyParaStyle()
        {
            Document doc = new Document();
            Style style = doc.Styles.Add(StyleType.Paragraph, "MyStyle");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;


            DocumentBuilder builder = new DocumentBuilder();
            Document document = builder.Document;
            Style importedStyle = document.Styles.AddCopy(style);
            builder.CurrentParagraph.ParagraphFormat.Style = importedStyle;
            builder.Writeln("This is centered text.");

            TestUtil.SaveOpen(document, @"Model\Style\AddCopyParaStyle.docx");

            importedStyle.Name = "Normal";
            Assert.That(importedStyle.BuiltIn, Is.True);
            Assert.That(document.Styles["Normal"].ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            TestUtil.SaveOpen(builder.Document, @"Model\Style\AddCopyParaStyleRenamed.docx");

            importedStyle.Name = "MyStyle2";
            Assert.That(importedStyle.BuiltIn, Is.False);
            Assert.That(document.Styles["MyStyle2"].ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            TestUtil.SaveOpen(builder.Document, @"Model\Style\AddCopyParaStyleRenamed2.docx");
        }

        [Test]
        public void TestAddCopyComplexStyle()
        {
            Document src = TestUtil.Open(@"Model\Revision\TestStyleRevisions.docx");
            StyleCollection sourceStyles = src.Styles;
            DocumentBuilder builder = new DocumentBuilder();

            foreach (Style style in sourceStyles)
            {
                Style importedStyle = builder.Document.Styles.GetByName(style.Name, false);
                if (importedStyle == null)
                    importedStyle = builder.Document.Styles.AddCopy(style);

                switch (importedStyle.Type)
                {
                        case StyleType.Paragraph:
                            WriteCleanPara(builder);
                            builder.CurrentParagraph.ParagraphFormat.StyleName = importedStyle.Name;
                            builder.Writeln(String.Format("Testing {0} style.", style.Name));
                            break;
                        case StyleType.Character:
                            WriteCleanPara(builder);
                            builder.PushFont();
                            builder.Font.Style = importedStyle;
                            builder.Writeln(String.Format("Testing {0} style.", importedStyle.Name));
                            builder.PopFont();
                            break;
                        case StyleType.List:
                            WriteCleanPara(builder);
                            builder.ListFormat.ListId = importedStyle.ParaPr.ListId;
                            builder.Writeln(String.Format("Testing {0} style.", importedStyle.Name));
                            builder.ListFormat.RemoveNumbers();
                            break;
                        case StyleType.Table:
                            WriteCleanPara(builder);
                            Table t = builder.StartTable();
                            builder.InsertCell();
                            if (t.FirstRow.TablePr.ContainsKey(TableAttr.Istd))
                                t.FirstRow.TablePr[TableAttr.Istd] = importedStyle.Istd;
                            else
                                t.FirstRow.TablePr.Add(TableAttr.Istd, importedStyle.Istd);
                            builder.Writeln(String.Format("Testing {0} style.", importedStyle.Name));
                            builder.EndRow();
                            builder.EndTable();
                            break;
                        default:
                            Assert.Fail();
                            break;
                }
            }

            TestUtil.SaveOpen(builder.Document, @"Model\Style\AddCopyComplex.docx");
        }

        /// <summary>
        /// WORDSNET-7403 Style.BaseStyleName throws Exception System.InvalidOperationException.
        /// andrnosk: The problem occurs because we suppose that BaseStyleName property can be changed
        /// only for character and paragraph styles, but according to MS Word behavior,
        /// this property also can be modified for table styles.
        /// Fixed by adding StyleType.Table to the condition.
        /// </summary>
        [Test]
        public void TestJira7403()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira7403.docx");

            Style tableStyle = doc.Styles["TableStyle"];
            tableStyle.BaseStyleName = "Table Professional";

            Assert.That(tableStyle.BaseStyleName, Is.EqualTo("Table Professional"));
        }

        /// <summary>
        /// Tests styles equality for cloned document with all MS Word 2007 styles.
        /// </summary>
        [Test]
        public void TestCompareAllStyles2007()
        {
            Document dst = TestUtil.Open(@"Model\Style\AllStyles2007.docx");
            Document src = (Document)dst.Clone(true);

            foreach (Style style in dst.Styles)
            {
                Style style1 = style;
                Style style2 = src.Styles.GetByIstd(style.Istd, false);

                Assert.That(style1.Equals(style2), Is.True);
            }
        }

        /// <summary>
        /// Tests styles equality for cloned document with all MS Word 2003 styles.
        /// </summary>
        [Test]
        public void TestCompareAllStyles2003()
        {
            Document dst = TestUtil.Open(@"Model\Style\AllStyles2003.docx");
            Document src = (Document)dst.Clone(true);

            foreach (Style style in dst.Styles)
            {
                Style style1 = style;
                Style style2 = src.Styles.GetByIstd(style.Istd, false);

                Assert.That(style1.Equals(style2), Is.True);
            }
        }

        /// <summary>
        /// Tests styles equality with different defaults.
        /// </summary>
        [Test]
        public void TestCompareStylesWithDifferentDefaults()
        {
            Document dst = TestUtil.Open(@"Model\Style\StyleNormal1.docx");
            Document src = TestUtil.Open(@"Model\Style\StyleNormal2.docx"); //Default font size was changed.

            Style style1 = dst.Styles["MyNormal"];
            Style style2 = src.Styles["MyNormal"];

            Assert.That(StyleType.Paragraph, Is.EqualTo(style1.Type));
            Assert.That(style1.BuiltIn, Is.False);

            // We are not comparing defaults when comparing styles, so these styles are equal.
            Assert.That(style1.Equals(style2), Is.True);
        }

        /// <summary>
        /// Tests styles equality with different base styles.
        /// </summary>
        [Test]
        public void TestCompareStylesWithDifferentBaseStyle()
        {
            Document dst = TestUtil.Open(@"Model\Style\TestStyles.docx");
            Document src = (Document)dst.Clone(true);

            Style style1 = dst.Styles["MyStyle"];
            Style style2 = src.Styles["MyStyle"];

            Assert.That(style1.Equals(style2), Is.True);

            // Change base style font size.
            Style baseStyle1 = style1.GetBaseStyle();
            baseStyle1.Font.Size = 12;

            Assert.That(style1.Equals(style2), Is.False);
        }

        /// <summary>
        /// Tests styles equality with different linked styles.
        /// </summary>
        [Test]
        public void TestCompareLinkedStyles()
        {
            Document dst = TestUtil.Open(@"Model\Style\TestStyles.docx");
            Document src = (Document)dst.Clone(true);

            Style style1 = dst.Styles["MyStyle"];
            Style style2 = src.Styles["MyStyle"];

            Assert.That(style1.Equals(style2), Is.True);

            // Change linked style font size.
            Style linkedStyle1 = style1.GetLinkedStyle();
            linkedStyle1.Font.Size = 12;

            Assert.That(style1.Equals(style2), Is.False);
        }

        /// <summary>
        /// Tests equality of table styles.
        /// </summary>
        [Test]
        public void TestCompareTableStyles()
        {
            Document dst = TestUtil.Open(@"Model\Style\TestStyles.docx");
            Document src = (Document)dst.Clone(true);

            Style style1 = dst.Styles["TableStyle1"];
            Style style2 = src.Styles["TableStyle1"];

            Assert.That(style1.Equals(style2), Is.True);

            // Change something inside table style1.
            Assert.That(StyleType.Table, Is.EqualTo(style1.Type));
            ((TableStyle)style1).RowPr.HeightRule = HeightRule.AtLeast;

            // Styles are not same now.
            Assert.That(style1.Equals(style2), Is.False);
        }

        /// <summary>
        /// Tests different list styles are imported correctly.
        /// </summary>
        [Test]
        public void TestListLevelApi()
        {
            Document dst = TestUtil.Open(@"Model\Style\StyleNormal1.docx");
            Document src = TestUtil.Open(@"Model\Style\StyleNormal2.docx"); // Style defaults are different.
            dst.AppendDocument(src, ImportFormatMode.KeepDifferentStyles);

            // There are two different styles: 'MyNormal' and 'ListParagraph'.
            // In addition, note that we compare completely expanded attributes
            // when take decision about styles equality.
            Assert.That(dst.Styles.Count, Is.EqualTo(src.Styles.Count + 2)); // List styles are different
            Assert.That(dst.Styles["List Paragraph"].RunPr[FontAttr.Size], Is.Null);
            Assert.That(dst.Styles["List Paragraph_0"].RunPr[FontAttr.Size], Is.EqualTo(28));
        }

        /// <summary>
        /// Tests equality of character styles.
        /// </summary>
        [Test]
        public void TestComparisonCharacterStyles()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");
            StyleCollection stylesA = docA.Styles;
            StyleCollection stylesB = docB.Styles;

            //Paragraph style should not affect character styles.
            stylesB["Normal"].ParaPr.LeftIndent = 8;
            Assert.That(stylesA["Normal"].Equals(stylesB["Normal"]), Is.False);

            // Based on 'Default paragraph font'.
            Assert.That(stylesA["CharStyle1"].Equals(stylesB["CharStyle1"]), Is.True);
            Assert.That(stylesA["CharStyle1"].Equals(stylesB["CharStyle1Changed"]), Is.False);

            // Based on 'CharStyle1'.
            Assert.That(stylesA["CharStyle2"].Equals(stylesB["CharStyle2"]), Is.True);
            Assert.That(stylesA["CharStyle2"].Equals(stylesB["CharStyle2Changed"]), Is.False);
            Assert.That(stylesA["CharStyle2"].Equals(stylesB["CharStyle2OnlyBaseChanged"]), Is.False);
        }

        /// <summary>
        /// Tests equality of linked styles.
        /// </summary>
        [Test]
        public void TestComparisonLinkedStyles()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");

            Assert.That(docA.Styles["ParaLinkedStyle3"].Equals(docB.Styles["ParaLinkedStyle3"]), Is.True);
            Assert.That(docA.Styles["ParaLinkedStyle3"].Equals(docB.Styles["ParaLinkedStyle3OnlyLinkedChanged"]), Is.False);
        }

        /// <summary>
        /// Tests list styles equality.
        /// </summary>
        [Test]
        public void TestComparisonListStyles()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");

            // Styles have different ListId.
            Assert.That(docB.Styles["ListStyle1"].ParaPr.ListId, IsNot.EqualTo(docA.Styles["ListStyle1"].ParaPr.ListId));

            // But they compared correctly.
            Assert.That(docA.Styles["ListStyle1"].Equals(docB.Styles["ListStyle1"]), Is.True);
        }

        /// <summary>
        /// Test comparison of Lists.
        /// </summary>
        [Test]
        public void TestComparisonLists()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");
            List listA = doc.Lists.GetListByListId(1);
            List listB = doc.Lists.GetListByListId(2);
            List listC = doc.Lists.AddCopy(listA);

            // Check ListLevels.
            Assert.That(listA.ListDef.Levels[0].Equals(listC.ListDef.Levels[0]), Is.True);
            Assert.That(listB.ListDef.Levels[0].Equals(listC.ListDef.Levels[0]), Is.False);

            // Check ListDefs.
            Assert.That(listA.ListDef.Equals(listC.ListDef), Is.True);
            Assert.That(listB.ListDef.Equals(listC.ListDef), Is.False);

            // Check Lists.
            Assert.That(listA.Equals(listC), Is.True);
            Assert.That(listB.Equals(listC), Is.False);
        }

        /// <summary>
        /// Tests comparison of list styles with dead loop in ListLevel.
        /// </summary>
        [Test]
        public void TestComparisonListStylesDeadLoop()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");
            StyleCollection stylesA = docA.Styles;
            StyleCollection stylesB = docB.Styles;

            // Make dead loop.
            docA.Lists.GetListByListId(stylesA["ListStyle1"].ParaPr.ListId).ListLevels[0].LinkedStyle = stylesA["ListStyle1"];
            docB.Lists.GetListByListId(stylesB["ListStyle1"].ParaPr.ListId).ListLevels[0].LinkedStyle = stylesB["ListStyle1"];
            Assert.That(stylesA["ListStyle1"].Equals(stylesB["ListStyle1"]), Is.True);

            docB.Lists.GetListByListId(stylesB["ListStyle1"].ParaPr.ListId).ListLevels[0].RunPr.Color = DrColor.Aqua;
            Assert.That(stylesA["ListStyle1"].Equals(stylesB["ListStyle1"]), Is.False);
        }

        /// <summary>
        /// Tests equality styles with recursive loop to the next paragraph style.
        /// </summary>
        [Test]
        public void TestComparisonNextStylesDeadLoop()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");
            // Check styles with recursive loop to the next styles:
            // docA: S1->S2->S1,
            // docB: S1->S2->S3->S1 and S2 in docA not equal to S1 in docB.
            Assert.That(docA.Styles["S1"].Equals(docB.Styles["S1"]), Is.False);
            // Check styles with recursive loop to the next styles:
            // docA: C1->C2->C1,
            // docB: C1->C2->C3->C1 and all styles in both documents are equal.
            Assert.That(docA.Styles["C1"].Equals(docB.Styles["C1"]), Is.True);
        }

        /// <summary>
        /// Tests equality of non-modifiable styles.
        /// </summary>
        [Test]
        public void TestComparisonNonModifiableStyles()
        {
            Document docA = new Document();
            Document docB = new Document();

            docA.Styles["Default Paragraph Font"].RunPr.Size = 10;
            docB.Styles["Default Paragraph Font"].RunPr.Size = 20;
            Assert.That(docA.Styles["Default Paragraph Font"].Equals(docB.Styles["Default Paragraph Font"]), Is.True);

            docA.Styles["Table Normal"].ParaPr.Alignment = ParagraphAlignment.Center;
            docB.Styles["Table Normal"].ParaPr.Alignment = ParagraphAlignment.Right;
            Assert.That(docA.Styles["Table Normal"].Equals(docB.Styles["Table Normal"]), Is.True);
        }

        /// <summary>
        /// Tests styles equality when one of the styles is null.
        /// </summary>
        [Test]
        public void TestComparisonOneOfStylesNull()
        {
            Document doc = new Document();

            Assert.That(doc.Styles["Normal"].Equals(null), Is.False);
        }

        /// <summary>
        /// Tests styles equality when one of the base styles is null.
        /// </summary>
        [Test]
        public void TestComparisonOneOfBaseStylesNull()
        {
            Document DocA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document DocB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");
            StyleCollection stylesA = DocA.Styles;
            StyleCollection stylesB = DocB.Styles;

            stylesA["ListStyle1"].BasedOnIstd = stylesA["List Paragraph"].Istd;
            stylesB["ListStyle1"].BasedOnIstd = 0;
            Assert.That(stylesA["ListStyle1"].Equals(stylesB["ListStyle1"]), Is.False);

            stylesA["ListStyle1"].BasedOnIstd = 0;
            stylesB["ListStyle1"].BasedOnIstd = stylesB["List Paragraph"].Istd;
            Assert.That(stylesA["ListStyle1"].Equals(stylesB["ListStyle1"]), Is.False);
        }

        /// <summary>
        /// Tests equality of paragraph styles.
        /// </summary>
        [Test]
        public void TestComparisonParaStyles()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");

            // Based on 'Normal'.
            Assert.That(docA.Styles["ParaStyle1"].Equals(docB.Styles["ParaStyle1"]), Is.True);
            Assert.That(docA.Styles["ParaStyle1"].Equals(docB.Styles["ParaStyle1Changed"]), Is.False);

            // Based on 'ParaStyle1'.
            Assert.That(docA.Styles["ParaStyle2"].Equals(docB.Styles["ParaStyle2"]), Is.True);
            Assert.That(docA.Styles["ParaStyle2"].Equals(docB.Styles["ParaStyle2Changed"]), Is.False);
        }

        /// <summary>
        /// Tests equality of table styles.
        /// </summary>
        [Test]
        public void TestComparisonTableStyles()
        {
            Document docA = TestUtil.Open(@"Model\Style\TestStyleComparingA.docx");
            Document docB = TestUtil.Open(@"Model\Style\TestStyleComparingB.docx");
            StyleCollection stylesA = docA.Styles;
            StyleCollection stylesB = docB.Styles;

            Assert.That(stylesA["Table Normal"].Equals(stylesB["TableStyle1"]), Is.False);

            //Paragraph style should not affect table styles.
            stylesB["Normal"].ParaPr.LeftIndent = 8;
            Assert.That(stylesA["Normal"].Equals(stylesB["Normal"]), Is.False);

            // Based on 'Table Normal'.
            Assert.That(stylesA["TableStyle1"].Equals(stylesB["TableStyle1"]), Is.True);
            Assert.That(stylesA["TableStyle1"].Equals(stylesB["TableStyle1ChangedTblPr"]), Is.False);
            Assert.That(stylesA["TableStyle1"].Equals(stylesB["TableStyle1ChangedPPr"]), Is.False);
            Assert.That(stylesA["TableStyle1"].Equals(stylesB["TableStyle1ChangedRPr"]), Is.False);

            // Based on 'TableStyle1'.
            Assert.That(stylesA["TableStyle2"].Equals(stylesB["TableStyle2"]), Is.True);
            Assert.That(stylesA["TableStyle2"].Equals(stylesB["TableStyle2Changed"]), Is.False);
            Assert.That(stylesA["TableStyle2"].Equals(stylesB["TableStyle2BaseOnlyChanged"]), Is.False);
        }

        /// <summary>
        /// Tests 'Pair' correctly added and searched in HashSet.
        /// </summary>
        [Test]
        public void TestPair()
        {
            Document doc = new Document();

            Style styleA = doc.Styles["Normal"];
            Style styleB = doc.Styles["Table Normal"];
            Style styleC = doc.Styles["Default Paragraph Font"];

            Collections.Generic.HashSetGeneric<Pair> hashSet = new Collections.Generic.HashSetGeneric<Pair>();

            hashSet.Add(new Pair(styleA, styleB));

            Assert.That(hashSet.Contains(new Pair(styleA, styleB)), Is.True);
            Assert.That(hashSet.Contains(new Pair(styleA, styleC)), Is.False);
        }








        /// <summary>
        /// WORDSNET-14655 Incorrect formatting of 'Normal (Web)' after import from another document.
        /// This is duplicate for WORDSNET-14588
        /// </summary>
        [Test]
        public void TestJira14655()
        {
            Document srcDoc = TestUtil.Open(@"Model\Style\TestJira14655Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\Style\TestJira14655Dst.docx");

            DocumentBuilder builder = new DocumentBuilder(dstDoc);
            builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            // Problematic style.
            Style style = dstDoc.Styles["Normal (Web)"];

            // Check attributes are expanded and collapsed.
            ParaPr paraPr = style.ParaPr;
            Assert.That((bool)paraPr[ParaAttr.SpaceAfterAuto], Is.True);

            RunPr runPr = style.RunPr;
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(24));
            Assert.That(runPr[FontAttr.SizeBi], Is.EqualTo(24));
        }

        // FOSS: TestJira15319 removed — its two .doc inputs contain private data and have been
        // removed from TestData (the Doc reader is gone in FOSS regardless). Not restored.







        /// <summary>
        /// Related to WORDSNET-16783
        /// Tests how style formatting is copied.
        /// </summary>
        [Test]
        public void TestJira16783A()
        {
            string templateFileName = TestUtil.BuildTestFileName(@"Model\Style\TestJira16783A Sample.docx");
            Document target = TestUtil.Open(@"Model\Style\TestJira16783A Target.docx");

            target.CopyStylesFromTemplate(templateFileName);

            Style style1 = target.Styles.GetByName("Style1", false);
            Assert.That(style1.RunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(style1.ParaPr[ParaAttr.Alignment], Is.EqualTo(ParagraphAlignment.Center));

            Style style2 = target.Styles.GetByName("Style2", false);
            Assert.That(style2.BasedOnIstd, Is.EqualTo(style1.Istd));
            Assert.That(style2.RunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));

            Style style3 = target.Styles.GetByName("Style3", false);
            Assert.That(style3.BasedOnIstd, Is.EqualTo(style2.Istd));
            Assert.That(style3.RunPr[FontAttr.Underline], Is.EqualTo(Underline.Single));
        }

        /// <summary>
        /// Related to WORDSNET-16783
        /// Tests how style formatting is copied.
        /// </summary>
        [Test]
        public void TestJira16783B()
        {
            string templateFileName = TestUtil.BuildTestFileName(@"Model\Style\TestJira16783B Sample.docx");
            Document target = TestUtil.Open(@"Model\Style\TestJira16783B Target.docx");

            target.CopyStylesFromTemplate(templateFileName);

            Style style1 = target.Styles.GetByName("Style1", false);
            Assert.That(style1.RunPr[FontAttr.Bold], Is.Null);

            Style style2 = target.Styles.GetByName("Style2", false);
            Assert.That(style2.BasedOnIstd, Is.EqualTo(style1.Istd));
            Assert.That(style2.RunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));

            Style style3 = target.Styles.GetByName("Style3", false);
            Assert.That(style3.BasedOnIstd, Is.EqualTo(style2.Istd));
            Assert.That(style3.RunPr[FontAttr.Underline], Is.EqualTo(Underline.Single));
        }

        /// <summary>
        /// Related to WORDSNET-16783
        /// Tests that CopyAll method successfully passes circular dependency.
        /// </summary>
        [Test]
        public void TestJira16783Circular()
        {
            Document sample = TestUtil.Open(@"Model\Style\TestJira16783A Sample.docx");
            Document target = TestUtil.Open(@"Model\Style\TestJira16783A Target.docx");

            Style style2 = sample.Styles.GetByName("Style2", false);
            Style style3 = sample.Styles.GetByName("Style3", false);

            // Style3 is based on Style2. Make circular dependency intentionally.
            style2.BasedOnIstd = style3.Istd;
            Assert.That(style3.BasedOnIstd, Is.EqualTo(style2.Istd));
            Assert.That(style2.BasedOnIstd, Is.EqualTo(style3.Istd));

            target.CopyStylesFromTemplate(sample);

            style2 = target.Styles.GetByName("Style2", false);
            style3 = target.Styles.GetByName("Style3", false);

            Assert.That(style3.BasedOnIstd, Is.EqualTo(style2.Istd));
            Assert.That(style2.BasedOnIstd, Is.EqualTo(style3.Istd));
        }






        /// <summary>
        /// Relates to WORDSNET-17618
        /// Tests warning is issued when style that is being copied
        /// matches to a destination style of a different type.
        /// </summary>
        [Test]
        public void TestJira17618Warning()
        {
            Document srcDoc = new Document();
            Document dstDoc = new Document();

            // Add styles with the same names but different types.
            srcDoc.Styles.Add(StyleType.Character, "Style1");
            dstDoc.Styles.Add(StyleType.Paragraph, "Style1");

            dstDoc.WarningCallback = new WarningInfoCollection();
            dstDoc.CopyStylesFromTemplate(srcDoc);

            // Check warning.
            Assert.That(TestUtil.ContainsWarning((WarningInfoCollection)dstDoc.WarningCallback, WarningType.DataLoss,
                WarningSource.Unknown, string.Format(WarningStrings.CannotCopyStyle, "Style1")), Is.True);
            // Check style was not overridden.
            Assert.That(dstDoc.Styles["Style1"].Type, Is.EqualTo(StyleType.Paragraph));
        }

        /// <summary>
        /// WORDSNET-17878 Copied styles don't match the source styles.
        /// Styles after copying lost numbering. So styles int he destination don't match the source styles.
        /// Preserve numbering to fix the problem.
        /// </summary>
        [Test]
        public void TestJira17878()
        {
            Document srcDoc = TestUtil.Open(@"Model\Style\TestJira17878Source.docx");
            Document targetDoc = TestUtil.Open(@"Model\Style\TestJira17878Target.docx");

            targetDoc.CopyStylesFromTemplate(srcDoc);

            // Check list count in the target document.
            ListCollection lists = targetDoc.Lists;
            Assert.That(lists.Count, Is.EqualTo(4));

            // Check that list and its definition exist in the appropriate collections.
            Style style = targetDoc.Styles.GetByName("My Custom Style 1", false);
            List list = lists.GetListByListId((int)style.ParaPr[ParaAttr.ListId]);

            Assert.That(lists.GetIndexOfListDefByListDefId(list.ListDefId), Is.GreaterThan(-1), "List definition for \"My Custom Style 1\" was not found.");

            style = targetDoc.Styles.GetByName("My Custom Style 2", false);
            list = lists.GetListByListId((int)style.ParaPr[ParaAttr.ListId]);

            Assert.That(lists.GetIndexOfListDefByListDefId(list.ListDefId), Is.GreaterThan(-1), "List definition for \"My Custom Style 2\" was not found.");
        }



        /// <summary>
        /// Related to WORDSNET-20094 Tests comparision of table styles.
        /// </summary>
        [Test]
        public void TestComparingTableStyles()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestComparingTableStyles.docx");
            Style tableStyle1 = doc.Styles["TableStyle1"];
            Style tableStyle2 = doc.Styles["TableStyle2"];
            Style tableStyle3 = doc.Styles["TableStyle3"];

            Assert.That(tableStyle1.Equals(tableStyle2), Is.True);
            Assert.That(DuplicateStyleRemover.GetStyleHashCodeForEquality(tableStyle2), Is.EqualTo(DuplicateStyleRemover.GetStyleHashCodeForEquality(tableStyle1)));
            Assert.That(tableStyle1.Equals(tableStyle3), Is.False);
            Assert.That(tableStyle2.Equals(tableStyle3), Is.False);
        }


        /// <summary>
        /// Checks numbered style.
        /// </summary>
        private static void CheckNumberedStyle(Style style, int expectedLeftIndent)
        {
            // It is very important that the name of a linked style in list level will be the same as name of style
            // that refers to this list. Note, even if AW shows correct ListLabel for a paragraph with style
            // that violates this condition, Word does not recognize such list as a correct one and
            // does not show list label for that style.
            ListLevel listLevel = GetListLevel(style);
            Assert.That(listLevel.LinkedStyle.Name, Is.EqualTo(style.Name));
            // Check also indent to verify that we have copied a correct ListDef.
            Assert.That(listLevel.ParaPr.LeftIndent, Is.EqualTo(expectedLeftIndent));
        }

        /// <summary>
        /// Returns ListLevel of the specified style.
        /// </summary>
        private static ListLevel GetListLevel(Style style)
        {
            List list = style.Document.Lists.GetListByListId(style.ParaPr.ListId);
            ListLevel listLevel = list.ListLevels[style.ParaPr.ListLevel];

            return listLevel;
        }

        private static void WriteCleanPara(DocumentBuilder builder)
        {
            builder.CurrentParagraph.ParagraphFormat.StyleName = "Normal";
            builder.InsertParagraph();
        }

        /// <summary>
        /// Adds copy of all "List Bullet" and "List Number" styles from one document into another.
        /// </summary>
        private static void CopyListBulletAndNumberStyles(Document srcDoc, Document dstDoc)
        {
            Style dstStyle;

            // Copy all "List Bullet" styles.
            foreach (Style srcStyle in srcDoc.Styles)
                if (srcStyle.Name.StartsWith("List Bullet"))
                {
                    dstStyle = dstDoc.Styles.AddCopy(srcStyle);
                    dstStyle.Name = srcStyle.Name;
                }

            // Copy "List Number" style.
            Style srcListNumberStyle = srcDoc.Styles["List Number"];
            dstStyle = dstDoc.Styles.AddCopy(srcListNumberStyle);
            dstStyle.Name = srcListNumberStyle.Name;
        }

        /// <summary>
        /// Checks list level attributes.
        /// </summary>
        private static void CheckListLevel(ListLevel listLevel, double numberPosition, double tabPosition, double textPosition)
        {
            Assert.That(listLevel.NumberPosition, Is.EqualTo(numberPosition));
            Assert.That(listLevel.TabPosition, Is.EqualTo(tabPosition));
            Assert.That(listLevel.TextPosition, Is.EqualTo(textPosition));
        }

        /// <summary>
        /// Replaces style with the specified name.
        /// </summary>
        /// <returns>Replaced style.</returns>
        /// <remarks>
        /// This is helper method for <see cref="TestJira14595"/>
        /// that replaces style using customer's scenario.
        /// </remarks>
        private static Style ReplaceStyle(string styleName, StyleCollection srcStyles, StyleCollection dstStyles)
        {
            const string tmpStyleName = "tmpName";
            const string tmpLinkedStyleName = "tmpLinkedName";

            Style dstStyle = dstStyles[styleName];

            // Rename existing style to allow new added style have a good name.
            if (dstStyle != null)
            {
                dstStyle.Name = tmpStyleName;

                // If style has linked style, then rename it as well.
                Style dstLinkedStyle = dstStyles[dstStyle.LinkedStyleName];
                if (dstLinkedStyle != null)
                    dstLinkedStyle.Name = tmpLinkedStyleName;
            }

            Style srcStyle = srcStyles[styleName];
            Style newStyle = dstStyles.AddCopy(srcStyle);

            // Rename imported style to temporary name to override old style.
            newStyle.Name = tmpStyleName;
            // Recover original name.
            newStyle.Name = styleName;

            // Rename linked style the same way as main style to override it too.
            string linkedStyleName = newStyle.LinkedStyleName;
            Style newLinkedStyle = dstStyles[linkedStyleName];
            if (newLinkedStyle != null)
            {
                newLinkedStyle.Name = tmpLinkedStyleName;
                newLinkedStyle.Name = linkedStyleName;
            }

            return newStyle;
        }
    }
}
