// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/10/2015 by Alexey Morozov

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Aspose.Images;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Forms2;
using Aspose.Words.RW.Ole;
using Aspose.Words.RW.Ole.Ole2;
using Aspose.Words.Saving;
using NUnit.Framework;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif


namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestShapeOleControls
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests command button OLE control.
        /// </summary>
        [Test]
        public void TestCommandButton()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestCommandButton.docm");

            // Test public access.
            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.CommandButton, "Run macro", "");

            // Test internals.
            CommandButtonControl btn1 = (CommandButtonControl)control;
            Assert.That(btn1.Caption, Is.EqualTo("Run macro"));
            Assert.That(btn1.Enabled, Is.True);

            control = GetForms2OleControl(doc, 1);
            TestPublicAccess(control, Forms2OleControlType.CommandButton, "Disabled button", "");

            CommandButtonControl btn2 = (CommandButtonControl)control;
            Assert.That(btn2.Caption, Is.EqualTo("Disabled button"));
            Assert.That(btn2.Enabled, Is.False);
        }

        /// <summary>
        /// Tests image OLE control.
        /// </summary>
        [Test]
        public void TestImage()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestImage.docm");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.Image, "", "");

            ImageControl image1 = (ImageControl)control;
            Assert.That(image1.Picture, IsNot.Null());
            Assert.That(ImageUtil.GetImageType(image1.Picture), Is.EqualTo(FileFormat.Jpeg));
        }

        /// <summary>
        /// Tests OLE frame control and child controls reading.
        /// </summary>
        [Test]
        public void TestFrame()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestFrame.docm");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.Frame, "Preferred programming language", "");
            Assert.That(control.ChildNodes.Count, Is.EqualTo(3));

            FrameControl frame = (FrameControl)control;
            Assert.That(frame.ChildNodes.Count, Is.EqualTo(3));

            OptionButtonControl option1 = (OptionButtonControl)frame.ChildNodes[0];
            Assert.That(option1.Caption, Is.EqualTo("C#"));
            Assert.That(option1.SelectedInternal, Is.EqualTo(NullableBool.True));

            OptionButtonControl option2 = (OptionButtonControl)frame.ChildNodes[1];
            Assert.That(option2.Caption, Is.EqualTo("Visual Basic"));
            Assert.That(option2.SelectedInternal, Is.EqualTo(NullableBool.False));

            OptionButtonControl option3 = (OptionButtonControl)frame.ChildNodes[2];
            Assert.That(option3.Caption, Is.EqualTo("Delphi"));
            Assert.That(option3.SelectedInternal, Is.EqualTo(NullableBool.False));
        }

        /// <summary>
        /// Tests OLE check box control reading.
        /// </summary>
        [Test]
        public void TestCheckBoxControl()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestCheckbox.docx");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.CheckBox, "Первый", "0");

            CheckBoxControl check1 = (CheckBoxControl)control;
            Assert.That(check1.Caption, Is.EqualTo("Первый"));
            Assert.That(check1.CheckedInternal, Is.EqualTo(NullableBool.False));

            control = GetForms2OleControl(doc, 1);
            TestPublicAccess(control, Forms2OleControlType.CheckBox, "Второй", "1");

            CheckBoxControl check2 = (CheckBoxControl)control;
            Assert.That(check2.Caption, Is.EqualTo("Второй"));
            Assert.That(check2.CheckedInternal, Is.EqualTo(NullableBool.True));

            control = GetForms2OleControl(doc, 2);
            TestPublicAccess(control, Forms2OleControlType.CheckBox, "Третий", "1");

            CheckBoxControl check3 = (CheckBoxControl)control;
            Assert.That(check3.Caption, Is.EqualTo("Третий"));
            Assert.That(check3.CheckedInternal, Is.EqualTo(NullableBool.True));
        }

        /// <summary>
        /// Tests OLE option button reading.
        /// </summary>
        [Test]
        public void TestOptionButton()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestOptionButton.docx");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.OptionButton, "Are you happy?", "1");

            // Check internals.
            OptionButtonControl option = (OptionButtonControl)control;
            Assert.That(option.Caption, Is.EqualTo("Are you happy?"));
            Assert.That(option.SelectedInternal, Is.EqualTo(NullableBool.True));
        }

        /// <summary>
        /// Tests OLE tab strip reading.
        /// </summary>
        [Test]
        public void TestTabStripControl()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestTabStrip.docx");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.TabStrip, "", "");

            TabStripControl tabStrip = (TabStripControl)control;

            // ListIndex is zero based.
            Assert.That(tabStrip.ListIndex, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests many controls reading.
        /// </summary>
        [Test]
        public void TestManyControls()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestManyControls.docx");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.CheckBox, "Check this", "0");

            // Test internals.
            CheckBoxControl checkbox = (CheckBoxControl)control;
            Assert.That(checkbox.CheckedInternal, Is.EqualTo(NullableBool.False));
            Assert.That(checkbox.Enabled, Is.True);

            control = GetForms2OleControl(doc, 1);
            TestPublicAccess(control, Forms2OleControlType.Textbox, "", "Enter max 30 chars text");

            TextBoxControl textBox = (TextBoxControl)control;
            Assert.That(textBox.Text, Is.EqualTo("Enter max 30 chars text"));
            Assert.That(textBox.Enabled, Is.True);
        }

        /// <summary>
        /// Tests MultiPageControl reading.
        /// </summary>
        [Test]
        public void TestMulitPageControl()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestMultiPage.docm");

            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.MultiPage, "", "");
            Assert.That(control.ChildNodes.Count, Is.EqualTo(3));
            TestPublicAccess(control.ChildNodes[0], Forms2OleControlType.TabStrip, "", "");
            TestPublicAccess(control.ChildNodes[1], Forms2OleControlType.Form, "", "");
            TestPublicAccess(control.ChildNodes[2], Forms2OleControlType.Form, "", "");

            // Check internals.
            MultiPageControl multiPage = (MultiPageControl)control;

            // First is always TabStripControl.
            Assert.That(multiPage.ChildNodes[0].Type, Is.EqualTo(Forms2OleControlType.TabStrip));

            // Then few FormControls.
            Assert.That(multiPage.ChildNodes[1].Type, Is.EqualTo(Forms2OleControlType.Form));
            Assert.That(multiPage.ChildNodes[2].Type, Is.EqualTo(Forms2OleControlType.Form));

            // Check state of control on second page.
            OptionButtonControl option = (OptionButtonControl)((FormControl)multiPage.ChildNodes[2]).ChildNodes[0];

            Assert.That(option.Caption, Is.EqualTo("OptionButton1"));
            Assert.That(option.SelectedInternal, Is.EqualTo(NullableBool.True));
        }

        /// <summary>
        /// Tests how controls at few inner levels are read.
        /// </summary>
        [Test]
        public void TestInnerFrames()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestInnerFrames.docx");
            Forms2OleControl topFrame = GetForms2OleControl(doc, 0);
            TestPublicAccess(topFrame, Forms2OleControlType.Frame, "Frame1", "");
            Assert.That(topFrame.ChildNodes.Count, Is.EqualTo(15));

            TestPublicAccess(topFrame.ChildNodes[0], Forms2OleControlType.Label, "Label1", "");
            TestPublicAccess(topFrame.ChildNodes[1], Forms2OleControlType.Textbox, "", "");
            TestPublicAccess(topFrame.ChildNodes[2], Forms2OleControlType.ComboBox, "", "");
            TestPublicAccess(topFrame.ChildNodes[3], Forms2OleControlType.ListBox, "", "");
            TestPublicAccess(topFrame.ChildNodes[4], Forms2OleControlType.CheckBox, "CheckBox1", "0");
            TestPublicAccess(topFrame.ChildNodes[5], Forms2OleControlType.OptionButton, "OptionButton1", "0");
            TestPublicAccess(topFrame.ChildNodes[6], Forms2OleControlType.ToggleButton, "ToggleButton1", "0");
            TestPublicAccess(topFrame.ChildNodes[7], Forms2OleControlType.Frame, "Frame1", "");
            TestPublicAccess(topFrame.ChildNodes[8], Forms2OleControlType.CommandButton, "CommandButton1", "");
            TestPublicAccess(topFrame.ChildNodes[9], Forms2OleControlType.TabStrip, "", "");
            TestPublicAccess(topFrame.ChildNodes[10], Forms2OleControlType.MultiPage, "", "");
            TestPublicAccess(topFrame.ChildNodes[11], Forms2OleControlType.ScrollBar, "", "");
            TestPublicAccess(topFrame.ChildNodes[12], Forms2OleControlType.SpinButton, "", "");
            TestPublicAccess(topFrame.ChildNodes[13], Forms2OleControlType.Image, "", "");
            TestPublicAccess(topFrame.ChildNodes[14], Forms2OleControlType.CheckBox, "CheckBox3", "0");

            Forms2OleControl multiPage = topFrame.ChildNodes[10];
            Assert.That(multiPage.ChildNodes.Count, Is.EqualTo(3));
            TestPublicAccess(multiPage.ChildNodes[0], Forms2OleControlType.TabStrip, "", "");
            TestPublicAccess(multiPage.ChildNodes[1], Forms2OleControlType.Form, "", "");
            TestPublicAccess(multiPage.ChildNodes[2], Forms2OleControlType.Form, "", "");
        }

        /// <summary>
        /// WORDSNET-12855 Support creation of HTML Controls.
        /// Added support for creation this group of OLE controls.
        /// </summary>
        [Test]
        public void TestJira12855()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            using (Stream noImage = ImageUtil.GetNoImageStream())
            {
                const string html1 = "<INPUT TYPE=\"radio\" NAME=\"FuelLine\" VALUE=\"Y\">";
                builder.InsertHtmlOleControl(new HtmlOleControl("DefOcxName1", HtmlOleControlType.Option, html1), noImage);

                const string html2 = "<INPUT TYPE=\"radio\" CHECKED NAME=\"FuelLine\" VALUE=\"Y\">";
                builder.InsertHtmlOleControl(new HtmlOleControl("DefOcxName2", HtmlOleControlType.Option, html2), noImage);

                doc = TestUtil.SaveOpen(doc, @"Model\Shape\Ole\ActiveX\TestJira12855.docx", null, false);   // FOSS: was .doc

                // Test first control.
                Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
                Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.OleControl));

                HtmlOleControl htmlOleControl = (HtmlOleControl)shape.OleFormat.OleControl;
                Assert.That(htmlOleControl.Type, Is.EqualTo(HtmlOleControlType.Option));
                Assert.That(htmlOleControl.Html, Is.EqualTo("<INPUT TYPE=\"radio\" NAME=\"FuelLine\" VALUE=\"Y\">"));

                // Verify control internals.
                OleObject oleObject = ((IEmbeddedObject)htmlOleControl).GetOleObject();
                ObjInfoStream objInfo = ObjInfoStream.Read(oleObject.Data);
                Assert.That(objInfo.Flags1, Is.EqualTo(OdtPersist1.RecomposeOnResize | OdtPersist1.Ocx | OdtPersist1.Stream));

                OcxNameStream ocxName = OcxNameStream.Read(oleObject.Data);
                Assert.That(ocxName.Value, Is.EqualTo("DefOcxName1"));
            }
        }

        /// <summary>
        /// WORDSNET-14293 System.InvalidCastException is thrown while convert Docx to Html.
        /// OleFormat.EmbeddedObject can be of two types: OleObject and OoxmlObject.
        /// Should check actual type of OleFormat.EmbeddedObject before casting it
        /// to OleObject in OleFormat.OleControl property.
        /// </summary>
        [Test]
        public void TestJira14293()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestJira14293.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.That(shape.GraphicData.OleFormat.OleControl, Is.Null);
        }

        // FOSS: TestJira17047 removed — it verified cold-rendered predefined images for HTML OLE radio
        // buttons (via the Pdf validator), checked by image-byte hashes; ActiveX/OLE control image
        // rendering is gone.

        /// <summary>
        /// Tests Forms2Ole controls creation and round-trip to different formats with default values.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestForms2ControlsCreation(SaveFormat sf)
        {
            foreach (Forms2OleControlType type in gAllControlTypes)
            {
                Document doc = new Document();
                InsertControl(doc, type, "ControlName");

                doc = TestUtil.SaveOpen(doc, GetTestName(type, sf, "Def"), SaveOptions.CreateSaveOptions(sf), false);
                Forms2OleControl control = GetForms2OleControl(doc, 0);

                // Verify type and name of the control.
                CheckTypeAndName(control, type, "ControlName");
            }
        }

        /// <summary>
        /// Tests Forms2Ole controls modification and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestForms2ControlsModification(SaveFormat sf)
        {
            foreach (Forms2OleControlType type in gAllControlTypes)
            {
                Document doc = new Document();
                Forms2OleControl originalControl = InsertControl(doc, type, "original name");

                // Change some control properties.
                gControlProps.CopyTo(originalControl.Pr);
                originalControl.Name = "NewName";

                // Round-trip modified control and check properties.
                doc = TestUtil.SaveOpen(doc, GetTestName(type, sf, "Mod"), SaveOptions.CreateSaveOptions(sf), false);
                Forms2OleControl resavedControl = GetForms2OleControl(doc, 0);

                // Verify type and name of the modified control.
                CheckTypeAndName(resavedControl, type, "NewName");
                // Verify additional common properties of the modified control.
                CheckCommonProps(resavedControl);
            }
        }

        /// <summary>
        /// Tests Frame control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestFrameCreation(SaveFormat sf)
        {
            Document doc = new Document();
            FrameControl frame = (FrameControl)InsertControl(doc, Forms2OleControlType.Frame, "Frame1");
            CheckBoxControl checkBox = new CheckBoxControl("CheckBox1", "CheckBox caption.", NullableBool.True);
            frame.ChildNodes.Add(checkBox);

            string testName = GetTestName(Forms2OleControlType.Frame, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            frame = (FrameControl)GetForms2OleControl(doc, 0);
            Assert.That(frame.Name, Is.EqualTo("Frame1"));

            checkBox = (CheckBoxControl)frame.ChildNodes[0];
            Assert.That(checkBox.CheckedInternal, Is.EqualTo(NullableBool.True));
        }

        /// <summary>
        /// Tests TabStrip control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestTabStripCreation(SaveFormat sf)
        {
            Document doc = new Document();
            TabStripControl control = (TabStripControl)InsertControl(doc, Forms2OleControlType.TabStrip, "TabStrip00001");
            control.Tabs.Add(new TabStripTab("first"));

            control.Tabs.Add(new TabStripTab("second"));
            control.Tabs[1].Enabled = false;

            control.Tabs.Add(new TabStripTab("third"));
            control.Tabs[2].Visible = false;
            control.Tabs[2].Enabled = false;

            string testName = GetTestName(Forms2OleControlType.TabStrip, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (TabStripControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("TabStrip00001"));

            Assert.That(control.Tabs.Count, Is.EqualTo(3));

            CheckTab(control.Tabs[0], true, true, "first");
            CheckTab(control.Tabs[1], true, false, "second");
            CheckTab(control.Tabs[2], false, false, "third");
        }

        /// <summary>
        /// Tests CheckBox control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestCheckBoxCreation(SaveFormat sf)
        {
            Document doc = new Document();
            CheckBoxControl control = (CheckBoxControl)InsertControl(doc, Forms2OleControlType.CheckBox, "ChkBox01");

            control.CheckedInternal = NullableBool.True;

            string testName = GetTestName(Forms2OleControlType.CheckBox, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (CheckBoxControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("ChkBox01"));
            Assert.That(control.CheckedInternal, Is.EqualTo(NullableBool.True));
        }

        /// <summary>
        /// Tests Image control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestImageCreation(SaveFormat sf)
        {
            Document doc = new Document();
            ImageControl control = (ImageControl)InsertControl(doc, Forms2OleControlType.Image, "Img01");

            control.Picture = gPicture;
            control.PictureAlignment = PictureAlignment.BottomRight;
            control.PictureSizeMode = PictureSizeMode.Zoom;
            control.PictureTiling = true;

            string testName = GetTestName(Forms2OleControlType.Image, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (ImageControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("Img01"));
            Assert.That(ArrayUtil.IsArrayEqual(control.Picture, gPicture), Is.True);
            Assert.That(control.PictureAlignment, Is.EqualTo(PictureAlignment.BottomRight));
            Assert.That(control.PictureSizeMode, Is.EqualTo(PictureSizeMode.Zoom));
            Assert.That(control.PictureTiling, Is.True);
        }

        /// <summary>
        /// Tests OptionButton control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestOptionButtonCreation(SaveFormat sf)
        {
            Document doc = new Document();
            OptionButtonControl control = (OptionButtonControl)InsertControl(doc, Forms2OleControlType.OptionButton, "OptBtn01");

            control.SelectedInternal = NullableBool.True;
            control.GroupName = "Group1";

            string testName = GetTestName(Forms2OleControlType.OptionButton, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (OptionButtonControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("OptBtn01"));
            Assert.That(control.SelectedInternal, Is.EqualTo(NullableBool.True));
            Assert.That(control.GroupName, Is.EqualTo("Group1"));
        }

        /// <summary>
        /// Tests TextBox control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestTextBoxCreation(SaveFormat sf)
        {
            Document doc = new Document();
            TextBoxControl control = (TextBoxControl)InsertControl(doc, Forms2OleControlType.Textbox, "Text001");

            control.Text = "Test text.";
            control.BackColor = Color.AliceBlue;
            control.ForeColor = Color.BlueViolet;

            control.Width = 144.5;
            control.Height = 72.3;

            string testName = GetTestName(Forms2OleControlType.Textbox, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (TextBoxControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("Text001"));
            Assert.That(control.Text, Is.EqualTo("Test text."));
            Assert.That(control.BackColor.ToArgb(), Is.EqualTo(Color.AliceBlue.ToArgb()));
            Assert.That(control.ForeColor.ToArgb(), Is.EqualTo(Color.BlueViolet.ToArgb()));

            Assert.That(control.Width, Is.EqualTo(144.5).Within(0.1));
            Assert.That(control.Height, Is.EqualTo(72.3).Within(0.1));
        }

        /// <summary>
        /// Tests SpinButton control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestSpinButtonCreation(SaveFormat sf)
        {
            Document doc = new Document();
            SpinButtonControl control = (SpinButtonControl)InsertControl(doc, Forms2OleControlType.SpinButton, "SpIn B T N 9");

            control.Min = 150;
            control.Max = 1000;
            control.Position = 999;

            string testName = GetTestName(Forms2OleControlType.SpinButton, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (SpinButtonControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("SpIn B T N 9"));
            Assert.That(control.Min, Is.EqualTo(150));
            Assert.That(control.Max, Is.EqualTo(1000));
            Assert.That(control.Position, Is.EqualTo(999));
        }

        /// <summary>
        /// Tests ScrollBar control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestScrollBarCreation(SaveFormat sf)
        {
            Document doc = new Document();
            ScrollBarControl control = (ScrollBarControl)InsertControl(doc, Forms2OleControlType.ScrollBar, "ScrL00101");

            control.Min = 100;
            control.Max = 200;
            control.Position = 140;

            string testName = GetTestName(Forms2OleControlType.ScrollBar, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (ScrollBarControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("ScrL00101"));
            Assert.That(control.Min, Is.EqualTo(100));
            Assert.That(control.Max, Is.EqualTo(200));
            Assert.That(control.Position, Is.EqualTo(140));
        }

        /// <summary>
        /// Tests various properties of a scrollable control.
        /// </summary>
        [Test]
        public void TestScrollableControlVariousProperties()
        {
            Document doc = new Document();
            ScrollableControlBase control = (ScrollableControlBase)InsertControl(doc, Forms2OleControlType.SpinButton, "");

            // Check default values.
            Assert.That(control.Min, Is.EqualTo(0));
            Assert.That(control.Max, Is.EqualTo(100));
            Assert.That(control.Position, Is.EqualTo(0));

            // It is allowed to set min > max, the same as in Word.
            control.Min = 150;
            Assert.That(control.Min, Is.EqualTo(150));
            Assert.That(control.Max, Is.EqualTo(100));
            // Position is adjusted automatically to fit in [Min, Max] range.
            Assert.That(control.Position, Is.EqualTo(100));

            control.Max = 1000;
            // Position is adjusted automatically to fit in [Min, Max] range.
            Assert.That(control.Position, Is.EqualTo(150));
        }

        /// <summary>
        /// Tests exception is thrown when Position property is set out of [Min, Max] range.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestScrollableControlPositionOutOfRangeException()
        {
            Document doc = new Document();
            ScrollableControlBase control = (ScrollableControlBase)InsertControl(doc, Forms2OleControlType.SpinButton, "");

            // Check default values.
            Assert.That(control.Min, Is.EqualTo(0));
            Assert.That(control.Max, Is.EqualTo(100));

            // Throws an exception.
            control.Position = 101;
        }

        /// <summary>
        /// Tests ListBox control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestListBoxCreation(SaveFormat sf)
        {
            Document doc = new Document();
            ListBoxControl control = (ListBoxControl)InsertControl(doc, Forms2OleControlType.ListBox, "list1");

            control.Value = "item";
            control.Width = 300;
            control.Height = 450;

            string testName = GetTestName(Forms2OleControlType.ListBox, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (ListBoxControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("list1"));
            Assert.That(control.SelectedValue, Is.EqualTo("item"));
            Assert.That(control.Width, Is.EqualTo(300).Within(0.1));
            Assert.That(control.Height, Is.EqualTo(450).Within(0.1));
        }

        /// <summary>
        /// Tests ComboBox control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestComboBoxCreation(SaveFormat sf)
        {
            Document doc = new Document();
            ComboBoxControl control = (ComboBoxControl)InsertControl(doc, Forms2OleControlType.ComboBox, "cmb00001");

            control.Value = "item";
            control.Width = 116.45;
            control.Height = 75.88;

            string testName = GetTestName(Forms2OleControlType.ComboBox, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (ComboBoxControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("cmb00001"));
            Assert.That(control.SelectedValue, Is.EqualTo("item"));
            Assert.That(control.Width, Is.EqualTo(116.45).Within(0.1));
            Assert.That(control.Height, Is.EqualTo(75.88).Within(0.1));
        }

        /// <summary>
        /// Tests MultiPage control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestMultiPageCreation(SaveFormat sf)
        {
            Document doc = new Document();
            MultiPageControl control = (MultiPageControl)InsertControl(doc, Forms2OleControlType.MultiPage, "m p 01");
            control.Caption = "MP";
            control.BackColor = Color.Bisque;
            control.Width = 222;
            control.Height = 111;

            // Round-trip control.
            string testName = GetTestName(Forms2OleControlType.MultiPage, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (MultiPageControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("m p 01"));
            Assert.That(control.Caption, Is.EqualTo("MP"));
            Assert.That(control.BackColor.ToArgb(), Is.EqualTo(Color.Bisque.ToArgb()));

            Assert.That(control.Width, Is.EqualTo(222).Within(0.1));
            Assert.That(control.Height, Is.EqualTo(111).Within(0.1));
        }

        /// <summary>
        /// Tests MultiPage control simple page manipulations with round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestMultiPageManipulation(SaveFormat sf)
        {
            Document doc = new Document();
            MultiPageControl control = (MultiPageControl)InsertControl(doc, Forms2OleControlType.MultiPage, "MultiPage1");
            control.Width = 200;
            control.Height = 200;

            FormControl page1 = control.Pages.Add(new FormControl("Page with CheckBox"));
            FormControl page2 = control.Pages.Add(new FormControl("Page with Label"));

            CheckBoxControl checkBox = new CheckBoxControl("CheckBox01", "Check me.", NullableBool.True);
            checkBox.BackStyle = BackStyle.Transparent;
            checkBox.Top = 90;
            checkBox.Left = 110;

            LabelControl label = new LabelControl("Label01");
            label.Caption = "Some text.";
            label.Top = 150;
            label.Left = 40;

            page1.ChildNodes.Add(checkBox);
            page2.ChildNodes.Add(label);

            control.Pages.Add(new FormControl("Some extra page."));
            Assert.That(control.Pages.Count, Is.EqualTo(3));

            control.Pages.Remove(control.Pages[control.Pages.Count-1]);
            Assert.That(control.Pages.Count, Is.EqualTo(2));

            // Round-trip control.
            string testName = GetTestName(Forms2OleControlType.MultiPage, sf, "Manipulation");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf));

            control = (MultiPageControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Pages.Count, Is.EqualTo(2));
            Assert.That(control.Pages[0].ChildNodes[0].SitePosition, Is.EqualTo(OlePosition.FromPoints(90, 110)));
            Assert.That(control.Pages[1].ChildNodes[0].SitePosition, Is.EqualTo(OlePosition.FromPoints(150, 40)));
        }

        /// <summary>
        /// Tests MultiPage control nested.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestMultiPageNested(SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Ole\ActiveX\TestMultiPageNested", LoadFormat.Docx, sf);

            MultiPageControl multiPageControl = (MultiPageControl)GetForms2OleControl(doc, 0);
            // Three children: [0] - TabStrip, [1]- Page1, [2] - Page2.
            Assert.That(multiPageControl.ChildNodes.Count, Is.EqualTo(3));

            // Get second page, that contains inner MultiPage control.
            FormControl page = (FormControl)multiPageControl.ChildNodes[2];

            // This is inner MultiPage control, that resides on the second page of the parent MultiPage control.
            multiPageControl = (MultiPageControl)page.ChildNodes[0];
            // Three children: [0] - TabStrip, [1]- Page1, [2] - Page2.
            Assert.That(multiPageControl.ChildNodes.Count, Is.EqualTo(3));

            // Get first page of inner MultiPage control.
            page = (FormControl)multiPageControl.ChildNodes[1];

            // This TextBox resides on the first page of the inner MultiPage control.
            TextBoxControl textBox = (TextBoxControl)page.ChildNodes[0];
            Assert.That(textBox.Name, Is.EqualTo("TextBox1"));

            // Get second page of inner MultiPage control, that is empty.
            page = (FormControl)multiPageControl.ChildNodes[2];
            Assert.That(page.ChildNodes.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests Label control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestLabelCreation(SaveFormat sf)
        {
            Document doc = new Document();
            LabelControl control = (LabelControl)InsertControl(doc, Forms2OleControlType.Label, "lbl01");
            control.Caption = "L a B e L caption";
            control.ForeColor = Color.Chocolate;

            // Round-trip control.
            string testName = GetTestName(Forms2OleControlType.Label, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (LabelControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("lbl01"));
            Assert.That(control.Caption, Is.EqualTo("L a B e L caption"));
            Assert.That(control.ForeColor.ToArgb(), Is.EqualTo(Color.Chocolate.ToArgb()));

            // This is default Label control size.
            Assert.That(control.Width, Is.EqualTo(72).Within(0.1));
            Assert.That(control.Height, Is.EqualTo(18).Within(0.1));
        }

        /// <summary>
        /// Tests CommandButton control creation and round-trip to different formats.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestCommandButtonCreation(SaveFormat sf)
        {
            Document doc = new Document();
            CommandButtonControl control = (CommandButtonControl)InsertControl(doc, Forms2OleControlType.CommandButton, "cmnd0001");
            control.Caption = "C o m m a BTN";
            control.Accelerator = 'S';
            control.BackColor = Color.Chartreuse;
            control.BackStyle = BackStyle.Transparent;
            control.Width = 251.25;
            control.Height = 84;
            control.Locked = true;
            // Round-trip control.
            string testName = GetTestName(Forms2OleControlType.CommandButton, sf, "");
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(sf), false);

            control = (CommandButtonControl)GetForms2OleControl(doc, 0);

            Assert.That(control.Name, Is.EqualTo("cmnd0001"));
            Assert.That(control.Caption, Is.EqualTo("C o m m a BTN"));
            Assert.That(control.Accelerator, Is.EqualTo('S'));
            Assert.That(control.BackColor.ToArgb(), Is.EqualTo(Color.Chartreuse.ToArgb()));
            Assert.That(control.BackStyle, Is.EqualTo(BackStyle.Transparent));
            Assert.That(control.Width, Is.EqualTo(251.25).Within(0.1));
            Assert.That(control.Height, Is.EqualTo(84).Within(0.1));
            Assert.That(control.Locked, Is.True);
            // We did not set it, but this is 'true' by default.
            Assert.That(control.TakeFocusOnClick, Is.True);
        }

        /// <summary>
        /// Tests many complex Forms2Ole controls round-trip to different formats.
        /// </summary>
        // FOSS: only Docx2Docx survives — Doc/Rtf/WordML load+save were removed.
        [TestCase(UnifiedScenario.Docx2Docx)]
        public void TestManyControlsComplex(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Ole\ActiveX\TestManyControls", scenario);
            Assert.That(doc.FirstSection.Body.Shapes.Count, Is.EqualTo(13));
        }

        /// <summary>
        /// Tests OLE controls with triple states.
        /// </summary>
        [Test]
        public void TestTripleStateControls()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\TestTripleStateControls.docx");

            // Check original state of the first checkbox.
            Forms2OleControl control = GetForms2OleControl(doc, 0);
            TestPublicAccess(control, Forms2OleControlType.CheckBox, "Первый", "1");
            CheckBoxControl checkBoxControl = (CheckBoxControl)control;
            Assert.That(checkBoxControl.CheckedInternal, Is.EqualTo(NullableBool.True));
            // Change state.
            checkBoxControl.CheckedInternal = NullableBool.NotDefined;

            // Check original state of the second checkbox.
            control = GetForms2OleControl(doc, 1);
            TestPublicAccess(control, Forms2OleControlType.CheckBox, "Второй", "2");
            checkBoxControl = (CheckBoxControl)control;
            Assert.That(checkBoxControl.CheckedInternal, Is.EqualTo(NullableBool.NotDefined));
            // Change state.
            checkBoxControl.CheckedInternal = NullableBool.False;

            // Check original state of the first option button.
            control = GetForms2OleControl(doc, 2);
            TestPublicAccess(control, Forms2OleControlType.OptionButton, "Опция1", "1");
            OptionButtonControl optionButtonControl = (OptionButtonControl)control;
            Assert.That(optionButtonControl.SelectedInternal, Is.EqualTo(NullableBool.True));
            // Change state.
            optionButtonControl.SelectedInternal = NullableBool.NotDefined;

            // Check original state of the second option button.
            control = GetForms2OleControl(doc, 3);
            TestPublicAccess(control, Forms2OleControlType.OptionButton, "Опция2", "2");
            optionButtonControl = (OptionButtonControl)control;
            Assert.That(optionButtonControl.SelectedInternal, Is.EqualTo(NullableBool.NotDefined));
            // Change state.
            optionButtonControl.SelectedInternal = NullableBool.False;

            // Check original state of the first toggle button.
            control = GetForms2OleControl(doc, 4);
            TestPublicAccess(control, Forms2OleControlType.ToggleButton, "Кнопка1", "1");
            ToggleButtonControl toggleButtonControl = (ToggleButtonControl)control;
            Assert.That(toggleButtonControl.Checked, Is.EqualTo(NullableBool.True));
            // Change state.
            toggleButtonControl.Checked = NullableBool.NotDefined;

            // Check original state of the second toogle button.
            control = GetForms2OleControl(doc, 5);
            TestPublicAccess(control, Forms2OleControlType.ToggleButton, "Кнопка2", "2");
            toggleButtonControl = (ToggleButtonControl)control;
            Assert.That(toggleButtonControl.Checked, Is.EqualTo(NullableBool.NotDefined));
            // Change state.
            toggleButtonControl.Checked = NullableBool.False;

            // Round-trip the document.
            UnifiedScenario scenario = TestUtil.BuildScenario(LoadFormat.Docx, SaveFormat.Docx, false);
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Ole\ActiveX\TestTripleStateControls", scenario);

            // Check new states of the all controls.
            checkBoxControl = (CheckBoxControl)GetForms2OleControl(doc, 0);
            Assert.That(checkBoxControl.CheckedInternal, Is.EqualTo(NullableBool.NotDefined));

            checkBoxControl = (CheckBoxControl)GetForms2OleControl(doc, 1);
            Assert.That(checkBoxControl.CheckedInternal, Is.EqualTo(NullableBool.False));

            optionButtonControl = (OptionButtonControl)GetForms2OleControl(doc, 2);
            Assert.That(optionButtonControl.SelectedInternal, Is.EqualTo(NullableBool.NotDefined));

            optionButtonControl = (OptionButtonControl)GetForms2OleControl(doc, 3);
            Assert.That(optionButtonControl.SelectedInternal, Is.EqualTo(NullableBool.False));

            toggleButtonControl = (ToggleButtonControl)GetForms2OleControl(doc, 4);
            Assert.That(toggleButtonControl.Checked, Is.EqualTo(NullableBool.NotDefined));

            toggleButtonControl = (ToggleButtonControl)GetForms2OleControl(doc, 5);
            Assert.That(toggleButtonControl.Checked, Is.EqualTo(NullableBool.False));
        }

        /// <summary>
        /// WORDSNET-25165 Allow accessing OLE Control Name and GroupName.
        /// The corresponding properties are made public.
        /// </summary>
        [Test]
        public void Test25165()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\Test25165.docx");
            Forms2OleControl control = (Forms2OleControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;

            Assert.That(control.Name, Is.EqualTo("OptionButton1"));
            Assert.That(control.GroupName, Is.EqualTo("TestGroup"));

            // Change properties values.
            control.Name = "MyOption1";
            Assert.That(control.Name, Is.EqualTo("MyOption1"));

            control.GroupName = "MyGroup";
            Assert.That(control.GroupName, Is.EqualTo("MyGroup"));

            // Round-trip the document and check values again.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Ole\ActiveX\Test25165", UnifiedScenario.Docx2DocxNoGold);
            control = (OptionButtonControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;

            Assert.That(control.Name, Is.EqualTo("MyOption1"));
            Assert.That(control.GroupName, Is.EqualTo("MyGroup"));

            // Check all related types and their properties are exposed publicly.
#if !JAVA && !CPLUSPLUS
            Type forms2OleControl = typeof(Forms2OleControl);
            Assert.That(forms2OleControl.IsPublic, Is.True);
            Assert.That(forms2OleControl.GetMember("Name"), IsNot.Empty());
            Assert.That(forms2OleControl.GetMember("GroupName"), IsNot.Empty());
#endif
        }

        /// <summary>
        /// Relates to WORDSNET-25165
        /// Tests default value and setting null value.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException),
#if NETSTANDARD
            ExpectedMessage = "Value cannot be null. (Parameter 'GroupName')")]
#else
            ExpectedMessage = "Value cannot be null.\r\nParameter name: GroupName")]
#endif
        public void Test25165NullGroupName()
        {
            Document doc = new Document();
            Forms2OleControl control = InsertControl(doc, Forms2OleControlType.OptionButton, "MyControl");

            // Check default value.
            Assert.That(control.GroupName, Is.EqualTo(string.Empty));

            // Try to set null.
            control.GroupName = null;
            Assert.That(control.GroupName, IsNot.Null());
        }



        /// <summary>
        /// WORDSNET-13551 Provide ability to modify value of radio button ActiveX control.
        /// Added public property <see cref="OptionButtonControl.Selected"/>.
        /// </summary>
        [Test]
        public void Test13551()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\Test13551.docx");
            OptionButtonControl optionButton1 = (OptionButtonControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;
            OptionButtonControl optionButton2 = (OptionButtonControl)doc.FirstSection.Body.Shapes[1].OleFormat.OleControl;
            OptionButtonControl optionButton3 = (OptionButtonControl)doc.FirstSection.Body.Shapes[2].OleFormat.OleControl;

            Assert.That(optionButton1.Selected, Is.False);
            Assert.That(optionButton2.Selected, Is.False);
            Assert.That(optionButton3.Selected, Is.True);

            // Select another one option button, so that there will be two selected items at the same time.
            optionButton2.Selected = true;
            Assert.That(optionButton1.Selected, Is.False);
            Assert.That(optionButton2.Selected, Is.True);
            Assert.That(optionButton3.Selected, Is.True);

            // Deselect previously selected item.
            optionButton3.Selected = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Ole\ActiveX\Test13551", UnifiedScenario.Docx2DocxNoGold);
            optionButton1 = (OptionButtonControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;
            Assert.That(optionButton1.Selected, Is.False);
            optionButton2 = (OptionButtonControl)doc.FirstSection.Body.Shapes[1].OleFormat.OleControl;
            Assert.That(optionButton2.Selected, Is.True);
            optionButton3 = (OptionButtonControl)doc.FirstSection.Body.Shapes[2].OleFormat.OleControl;
            Assert.That(optionButton3.Selected, Is.False);

#if !JAVA && !CPLUSPLUS
            // Check all related types and their properties are exposed publicly.
            Type optionButtonControlType = typeof(OptionButtonControl);
            Assert.That(optionButtonControlType.IsPublic, Is.True);
            Assert.That(optionButtonControlType.GetMember("Selected"), IsNot.Empty());
#endif
        }

        /// <summary>
        /// WORDSNET-12826 Provide ability to set a given ActiveX checkbox to checked or unchecked.
        /// Added public property <see cref="CheckBoxControl.Checked"/>.
        /// </summary>
        [Test]
        public void Test12826()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\ActiveX\Test12826.docx");
            CheckBoxControl checkBoxControl = (CheckBoxControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;

            Assert.That(checkBoxControl.Checked, Is.False);

            checkBoxControl.Checked = true;
            Assert.That(checkBoxControl.Checked, Is.True);

            // Check roundtrip.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Ole\ActiveX\Test12826", UnifiedScenario.Docx2DocxNoGold);
            checkBoxControl = (CheckBoxControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;
            Assert.That(checkBoxControl.Checked, Is.True);

            checkBoxControl.Checked = false;
            Assert.That(checkBoxControl.Checked, Is.False);

            // Check also undefined state.
            checkBoxControl.CheckedInternal = NullableBool.NotDefined;
            Assert.That(checkBoxControl.Checked, Is.False);

#if !JAVA && !CPLUSPLUS
            // Check all related types and their properties are exposed publicly.
            Type checkBoxType = typeof(CheckBoxControl);
            Assert.That(checkBoxType.IsPublic, Is.True);
            Assert.That(checkBoxType.GetMember("Checked"), IsNot.Empty());
#endif
        }

        /// <summary>
        /// WORDSNET-16356 Add feature to create CommandButton ActiveX.
        /// <see cref="DocumentBuilder.InsertForms2OleControl"/> is made public.
        /// </summary>
        [Test]
        public void Test16356()
        {
            DocumentBuilder builder = new DocumentBuilder();

            CommandButtonControl button1 = new CommandButtonControl() {Caption = "AAA"};
            builder.InsertForms2OleControl(button1);
            Assert.That(button1.Caption, Is.EqualTo("AAA"));

            CommandButtonControl button2 = new CommandButtonControl() { Caption = "BBB" };
            builder.InsertForms2OleControl(button2);
            Assert.That(button2.Caption, Is.EqualTo("BBB"));

            const string outFile = @"Model\Shape\Ole\ActiveX\Test16356";
            Document doc = TestUtil.SaveOpen(builder.Document, outFile, UnifiedScenario.Docx2DocxNoGold);

            button1 = (CommandButtonControl)doc.FirstSection.Body.Shapes[0].OleFormat.OleControl;
            Assert.That(button1.Caption, Is.EqualTo("AAA"));

            button2 = (CommandButtonControl)doc.FirstSection.Body.Shapes[1].OleFormat.OleControl;
            Assert.That(button2.Caption, Is.EqualTo("BBB"));

#if !JAVA && !CPLUSPLUS
            // Check all related types and their properties are exposed publicly.
            Type commandButtonControl = typeof(CommandButtonControl);
            Assert.That(commandButtonControl.IsPublic, Is.True);

            Type documentBuilderType = typeof(DocumentBuilder);
            Assert.That(documentBuilderType.GetMember("InsertForms2OleControl"), IsNot.Empty());
#endif
        }

        /// <summary>
        /// WORDSNET-27435 Add possibility to set caption of CommandButton ActiveX control.
        /// <see cref="Forms2OleControl.Caption"/> setter is made public.
        /// </summary>
        [Test]
        public void Test27435()
        {
            DocumentBuilder builder = new DocumentBuilder();

            CommandButtonControl button = new CommandButtonControl() { Caption = "AAA" };
            builder.InsertForms2OleControl(button);
            Assert.That(button.Caption, Is.EqualTo("AAA"));

#if !JAVA && !CPLUSPLUS
            // Check all related types and their properties are exposed publicly.
            Type control = typeof(Forms2OleControl);
            PropertyInfo property = control.GetProperty("Caption");
            Assert.That(property, IsNot.Null());
            Assert.That(property.GetSetMethod(), IsNot.Null());
#endif
        }


        /// <summary>
        /// Checks some common for all types of controls properties set from <see cref="gControlProps"/>:
        /// </summary>
        private static void CheckCommonProps(Forms2OleControl control)
        {
            Assert.That(OleColor.FromColor(control.BackColor), Is.EqualTo(gBackgroundColor), string.Format("{0}: BackColor", control.Type));
            Assert.That(control.Size, Is.EqualTo(gSize), string.Format("{0}: Size", control.Type));
            Assert.That(control.MousePointer, Is.EqualTo(MousePointer.Custom), string.Format("{0}: MousePointer", control.Type));
            Assert.That(ArrayUtil.IsArrayEqual(gMouseIcon, control.MouseIcon), Is.True, string.Format("{0}: MouseIcon", control.Type));
        }

        /// <summary>
        /// Gets test name depending on Forms2OleControl type relative to "Model\Shape\Ole\ActiveX\" path.
        /// </summary>
        private static string GetTestName(Forms2OleControlType controlType, SaveFormat sf, string suffix)
        {
            const string testFolder = @"Model\Shape\Ole\ActiveX\";

            string ext = FileFormatUtil.SaveFormatToExtension(sf);
            string fileName = string.Format("{0}Control", OleUtil.ToString(controlType));

            return string.Format("{0}{1}{2}{3}", testFolder, fileName, suffix, ext);
        }

        /// <summary>
        /// Creates and inserts Forms2OleControl into a document.
        /// </summary>
        private static Forms2OleControl InsertControl(Document doc, Forms2OleControlType type, string name)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);
            Forms2OleControl forms2OleControl = Forms2OleControl.Create(type, name);
            builder.InsertForms2OleControl(forms2OleControl);

            return forms2OleControl;
        }

        /// <summary>
        /// Checks type and name of Forms2OleControl.
        /// </summary>
        private static void CheckTypeAndName(Forms2OleControl control, Forms2OleControlType expectedType, string expectedName)
        {
            Assert.That(control.Type, Is.EqualTo(expectedType), "Type");
            Assert.That(control.Name, Is.EqualTo(expectedName), "Name");
        }

        /// <summary>
        /// Returns Nth Forms2OleControl from document.
        /// </summary>
        private static Forms2OleControl GetForms2OleControl(Document doc, int idx)
        {
            Shape shape = (Shape)doc.FirstSection.Body.GetChild(NodeType.Shape, idx, true);

            OleControl oleControl = shape.OleFormat.OleControl;
            Assert.That(oleControl is Forms2OleControl, Is.True);

            return (Forms2OleControl)oleControl;
        }

        /// <summary>
        /// Checks publicly accessed attributes of Forms2OleControl.
        /// </summary>
        private static void TestPublicAccess(Forms2OleControl forms2Ole, Forms2OleControlType type, string caption,
            string value)
        {
            Assert.That(forms2Ole.Type, Is.EqualTo(type));
            Assert.That(forms2Ole.Caption, Is.EqualTo(caption));
            Assert.That(forms2Ole.Value, Is.EqualTo(value));
        }

        /// <summary>
        /// Checks Tab of TabStripControl.
        /// </summary>
        private static void CheckTab(TabStripTab tab, bool expectedVisible, bool expectedEnabled, string expectedCaption)
        {
            Assert.That(tab.Visible, Is.EqualTo(expectedVisible), "Visible");
            Assert.That(tab.Enabled, Is.EqualTo(expectedEnabled), "Enabled");
            Assert.That(tab.Caption, Is.EqualTo(expectedCaption), "Caption");
        }

        static TestShapeOleControls()
        {
            gMouseIcon = StreamUtil.CopyFileToByteArray(TestUtil.BuildTestFileName(@"Model\Drawing\Core\icon_02.ico"));
            gPicture = StreamUtil.CopyFileToByteArray(TestUtil.BuildTestFileName(@"Model\Shape\Ole\InsertOleObject\Image.bmp"));

            gControlProps = new Forms2Pr();
            gControlProps.Add(Forms2Attr.BackgroundColor, gBackgroundColor);
            gControlProps.Add(Forms2Attr.ForegroundColor, gForeColor);
            gControlProps.Add(Forms2Attr.Size, gSize);
            gControlProps.Add(Forms2Attr.MouseIcon, gMouseIcon);
            gControlProps.Add(Forms2Attr.MousePointer, MousePointer.Custom);
            gControlProps.Add(Forms2Attr.Picture, gPicture);
            gControlProps.Add(Forms2Attr.PicturePosition, PicturePosition.RightTop);
            gControlProps.Add(Forms2Attr.Value, "1");
            gControlProps.Add(Forms2Attr.Caption, "The Caption!");
            gControlProps.Add(Forms2Attr.GroupName, "Group");
        }

        /// <summary>
        /// Predefined set of Forms2OleControl properties.
        /// </summary>
        private static readonly Forms2Pr gControlProps;
        private static readonly byte[] gMouseIcon;
        private static readonly byte[] gPicture;
        private static readonly OleColor gBackgroundColor = OleColor.FromColor(Color.Green);
        private static readonly OleColor gForeColor = OleColor.FromColor(Color.Red);
        private static readonly OleSize gSize = OleSize.FromPoints(233.0, 115.0);

        /// <summary>
        /// An array of Forms2OleControl types that will be tested.
        /// </summary>
        private static readonly Forms2OleControlType[] gAllControlTypes = new Forms2OleControlType[]
        {
            Forms2OleControlType.OptionButton,
            Forms2OleControlType.Label,
            Forms2OleControlType.Textbox,
            Forms2OleControlType.CheckBox,
            Forms2OleControlType.ToggleButton,
            Forms2OleControlType.SpinButton,
            Forms2OleControlType.ComboBox,
            Forms2OleControlType.Frame,
            Forms2OleControlType.MultiPage,
            Forms2OleControlType.TabStrip,
            Forms2OleControlType.CommandButton,
            Forms2OleControlType.Image,
            Forms2OleControlType.ScrollBar,
            Forms2OleControlType.ListBox
        };
    }
}
