// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/04/2004 by Roman Korchagin

using System;
using System.Data;
using Aspose.Common;
using Aspose.TestFx;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for form fields.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class TestFormFields : UnifiedTestsBase
    {

        /// <summary>
        /// There is a case here when form field bookmark starts not where expected.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormFieldRename(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestFormFieldRename", lf, sf);

            FormFieldCollection formFields = doc.Sections[0].Range.FormFields;
            Assert.That(formFields["Text1"] != null, Is.True);
            Assert.That(formFields["Text2"] != null, Is.True);
            Assert.That(formFields["Text3"] != null, Is.True);
            Assert.That(formFields["Text4"] != null, Is.True);
            Assert.That(formFields["Text5"] != null, Is.True);
            Assert.That(formFields["Text6"] != null, Is.True);
            Assert.That(formFields["Text7"] != null, Is.True);
        }


        /// <summary>
        /// Text input with multiple paragraphs was not supported.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormFieldMultiParagraph(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestFormFieldMultiParagraph", lf, sf);

            Assert.That(doc.Range.FormFields.Count, Is.EqualTo(2));
            //Check that loading multiparagraph text works.
            Assert.That(doc.Range.FormFields[0].Result, Is.EqualTo("Line1\rLine2"));
            Assert.That(doc.Range.FormFields[1].Result, Is.EqualTo("Line2\rLine3"));

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(3));
            Assert.That(paras[0].GetText(), Is.EqualTo("\x0013 FORMTEXT \x0001\x0014Line1\r"));
            Assert.That(paras[1].GetText(), Is.EqualTo("Line2\x0015 \x0013 FORMTEXT \x0001\x0014Line2\r"));
            Assert.That(paras[2].GetText(), Is.EqualTo("Line3\x0015\x000c"));

            //Check that when inserting text that contains fewer paragraphs, it actually removes paragraphs from the
            doc.Range.FormFields[0].Result = "X";
            doc.Range.FormFields[1].Result = "Y";
            paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(1));
            Assert.That(paras[0].GetText(), Is.EqualTo("\x0013 FORMTEXT \x0001\x0014X\x0015 \x0013 FORMTEXT \x0001\x0014Y\x0015\x000c"));

            //Check that inserting text with multiple paragraph works.
            doc.Range.FormFields[0].Result = "Line1\rLine2";
            paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(2));
            Assert.That(paras[0].GetText(), Is.EqualTo("\x0013 FORMTEXT \x0001\x0014Line1\r"));
            Assert.That(paras[1].GetText(), Is.EqualTo("Line2\x0015 \x0013 FORMTEXT \x0001\x0014Y\x0015\x000c"));

            TestUtil.SaveOpen(doc, @"Model\FormField\TestFormFieldMultiParagraph Modified", lf, sf);
        }



        /// <summary>
        /// Long default text form field value was causing crashes in pre Word 2003.
        /// So I changed that we limit field default value to 255 chars.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormFieldLong(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertTextInput(
                "xxx",
                TextFormFieldType.Regular,
                "",
                "[begin]" + new string('x', 800) + "[end]",
                2000);

            FormField formField = (FormField)doc.SelectSingleNode("//FormField");
            formField.FormFieldPr.TextInputDefault = "[begin]" + new string('y', 900) + "[end]";

            doc = TestUtil.SaveOpen(doc, @"Model\FormField\TestFormFieldLong", lf, sf);

            formField = (FormField)doc.SelectSingleNode("//FormField");
            Assert.That(formField.Result.Length, Is.EqualTo(812));
            // The default value got truncated.
            Assert.That(formField.FormFieldPr.TextInputDefault.Length, Is.EqualTo(255));
        }

        /// <summary>
        /// Test various form field properties.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormFieldProperties(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestFormFieldProperties", lf, sf);

            FormField f = doc.Range.FormFields[0];
            Assert.That(f.StatusText, Is.EqualTo(""));
            Assert.That(f.HelpText, Is.EqualTo(""));
            Assert.That(f.OwnStatus, Is.EqualTo(false));
            Assert.That(f.OwnHelp, Is.EqualTo(false));
            Assert.That(f.Enabled, Is.EqualTo(true));
            Assert.That(f.CalculateOnExit, Is.EqualTo(false));

            f = doc.Range.FormFields[1];
            Assert.That(f.StatusText, Is.EqualTo("My own status bar."));
            Assert.That(f.HelpText, Is.EqualTo("RK"));
            Assert.That(f.OwnStatus, Is.EqualTo(true));
            Assert.That(f.OwnHelp, Is.EqualTo(false));
            Assert.That(f.Enabled, Is.EqualTo(false));

            f = doc.Range.FormFields[2];
            Assert.That(f.StatusText, Is.EqualTo("АВИАПОЧТОЙ"));
            Assert.That(f.HelpText, Is.EqualTo("Own help."));
            Assert.That(f.OwnStatus, Is.EqualTo(false));
            Assert.That(f.OwnHelp, Is.EqualTo(true));
            Assert.That(f.CalculateOnExit, Is.EqualTo(true));
        }

        /// <summary>
        /// Tests support of text input form field formats and types. Text input type controls how formatting is performed.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormat(LoadFormat lf, SaveFormat sf)
        {
            SystemPal.SaveCulture();
            try
            {
                // RK Have to use standard culture because form field result is formatted using current culture.
                SystemPal.SetStandardCulture();

                Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestFormat", lf, sf);

                FormField f = doc.Range.FormFields[0];
                f.SetTextInputValue("Hello");
                Assert.That(f.Result, Is.EqualTo("HELLO"));

                f = doc.Range.FormFields[1];
                // RK Different casing is written by different MS Word versions.
                Assert.That(StringUtil.EqualsIgnoreCase("Title case", f.TextInputFormat), Is.True);
                Assert.That(f.TextInputType, Is.EqualTo(TextFormFieldType.Regular));
                f.SetTextInputValue("hello hello");
                Assert.That(f.Result, Is.EqualTo("Hello Hello"));

                f = doc.Range.FormFields[2];
                Assert.That(f.TextInputFormat, Is.EqualTo("$#,##0.00;($#,##0.00)"));
                Assert.That(f.TextInputType, Is.EqualTo(TextFormFieldType.Number));
                f.SetTextInputValue(-1234);
                Assert.That(f.Result, Is.EqualTo("($1,234.00)"));

                f = doc.Range.FormFields[3];
                Assert.That(f.TextInputFormat, Is.EqualTo("M/d/yyyy h:mm am/pm"));
                Assert.That(f.TextInputType, Is.EqualTo(TextFormFieldType.Date));
                f.SetTextInputValue(new DateTime(2005, 12, 15, 20, 9, 10));
                Assert.That(f.Result, Is.EqualTo("12/15/2005 8:09 pm"));
                f = doc.Range.FormFields[4];
                Assert.That(f.TextInputFormat, Is.EqualTo("0.00%"));
                Assert.That(f.TextInputType, Is.EqualTo(TextFormFieldType.Number));
                f.SetTextInputValue(0.175);
                Assert.That(f.Result, Is.EqualTo("17.50%"));

                f = doc.Range.FormFields[6];
                Assert.That(f.TextInputFormat, Is.EqualTo("#,##0"));
                Assert.That(f.TextInputType, Is.EqualTo(TextFormFieldType.Calculated));
                Assert.That(f.TextInputDefault, Is.EqualTo("=100+1000"));
                Assert.That(f.Result, Is.EqualTo("1,100"));
                f.TextInputDefault = "=100 * 100 + 12.5";
                Assert.That(f.Result, Is.EqualTo("10,013")); // Cool! Calculated like in MS Word immediately.
                f = doc.Range.FormFields[7];
                // RK This is a bug in Word 2007. It writes incorrect value when saving to DOCX.
                TextFormFieldType inputType = (lf != LoadFormat.Docx) ? TextFormFieldType.CurrentDate : TextFormFieldType.CurrentTime;
                Assert.That(f.TextInputType, Is.EqualTo(inputType));
                Assert.That(f.Result, Is.EqualTo("27-Feb-10"));

                f = doc.Range.FormFields[8];
                // RK This is a bug in Word 2007. It writes incorrect value when saving to DOCX.
                inputType = (lf != LoadFormat.Docx) ? TextFormFieldType.CurrentTime : TextFormFieldType.CurrentDate;
                Assert.That(f.TextInputType, Is.EqualTo(inputType));
                Assert.That(f.Result, Is.EqualTo("01:29"));
                // Check that calculated form fields are indeed updated.
                doc.UpdateFields();

                f = doc.Range.FormFields[6];
                Assert.That(f.Result, Is.EqualTo("10,013"));
                f = doc.Range.FormFields[7];
                Assert.That(f.Result, Is.EqualTo("5-Jan-06"));  // This is a hardcoded test constant when updating to current date time.
                f = doc.Range.FormFields[8];
                Assert.That(f.Result, Is.EqualTo("19:09"));     // This is a hardcoded test constant when updating to current date time.

                TestUtil.SaveOpen(doc, @"Model\FormField\TestFormat Modified", lf, sf);
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDropDown(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestDropDown", lf, sf);

            FormField f = doc.Range.FormFields[0];
            Assert.That(f.DropDownItems.Count, Is.EqualTo(2));
            Assert.That(f.DropDownSelectedIndex, Is.EqualTo(0));
            Assert.That(f.Result, Is.EqualTo("AAA"));

            f.DropDownSelectedIndex = f.DropDownItems.Add("CCC");
            Assert.That(f.DropDownSelectedIndex, Is.EqualTo(2));
            Assert.That(f.Result, Is.EqualTo("CCC"));

            //At the moment deleting/inserting items does not update the selected index.
            f.DropDownItems.RemoveAt(2);
            Assert.That(f.DropDownSelectedIndex, Is.EqualTo(2));
            Assert.That(f.Result, Is.EqualTo(""));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCheckBox(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestCheckBox", lf, sf);

            FormField f = doc.Range.FormFields[0];
            Assert.That(f.Checked, Is.EqualTo(false));
            Assert.That(f.IsCheckBoxExactSize, Is.EqualTo(false));
            Assert.That(f.Result, Is.EqualTo("0"));

            f = doc.Range.FormFields[1];
            Assert.That(f.Checked, Is.EqualTo(true));
            Assert.That(f.Result, Is.EqualTo("1"));

            f = doc.Range.FormFields[2];
            Assert.That(f.Checked, Is.EqualTo(true));
        }

        /// <summary>
        /// User noted that FormField.Remove does not remove the form field.
        /// Indeed it deleted only the FormField node, but did not delete all other nodes
        /// of the form field. As a temp workaround added FormField.Remove, but
        /// maybe should later rework FormField to make it a wrapper, not a node.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRemoveFormField(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestRemoveFormField", lf, sf);
            DocumentBuilder builder = new DocumentBuilder(doc);

            doc.Range.FormFields.Remove("Text1");
            builder.MoveToBookmark("Text1");
            builder.Write("John Doe");

            doc = TestUtil.SaveOpen(doc, @"Model\FormField\TestRemoveFormField Modified", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("John Doe\r\x000c"));
        }


        /// <summary>
        /// WORDSNET-1014 Aspose.Words does not show formfield name if the enclosing bookmark is deleted.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRemoveBookmarkFromFormField(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestRemoveBookmarkFromFormField", lf, sf);
            Assert.That(doc.Range.Bookmarks["Text1"], IsNot.Null());

            FormField field = doc.Range.FormFields[0];
            Assert.That(field.Name, Is.EqualTo("Text1"));

            // Remove the bookmark.
            doc.Range.Bookmarks.Remove("Text1");

            // Check the name of the form field still stays.
            Assert.That(field.Name, Is.EqualTo("Text1"));
        }

        /// <summary>
        /// WORDSNET-3564 Text inserted by FormField.SetTextInputValue does not have an expected formatting.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect3564(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\FormField\TestDefect3564", lf, sf);
            FormFieldCollection formFields = doc.FirstSection.Range.FormFields;

            FormField formField1 = formFields["OSEBA_NAS_POS_SIF_D"];
            FormField formField2 = formFields["OSEBA_NAS_POS_NAZ_D"];
            FormField formField3 = formFields["O_DOSEG_MOBI__1"];
            FormField formField4 = formFields["O_DOSEG_SMS"];
            FormField formField5 = formFields["SEMINAR_DATUM"];

            formField1.SetTextInputValue("3000");
            formField2.SetTextInputValue("Celje");
            formField3.SetTextInputValue("041555555");
            formField4.SetTextInputValue("x");
            formField5.SetTextInputValue("18.1.2009");

            Run run1 = (Run)formField1.NextSibling.NextSibling;
            Run run2 = (Run)formField2.NextSibling.NextSibling;
            Run run3 = (Run)formField3.NextSibling.NextSibling;
            Run run4 = (Run)formField4.NextSibling.NextSibling;
            Run run5 = (Run)formField5.NextSibling.NextSibling;

            // Comment these lines to save/show result document.
            Assert.That(run1.Font.Bold, Is.True, "The run font should be bold.");
            Assert.That(run2.Font.Bold, Is.True, "The run font should be bold.");
            Assert.That(run3.Font.Bold, Is.False, "The run font should NOT be bold.");
            Assert.That(run4.Font.Name, Is.EqualTo("Wingdings"), "The run font should be 'Wingdings'.");
            Assert.That(run5.Font.Bold, Is.True, "The run font should be bold.");

            TestUtil.SaveOpen(doc, @"Model\FormField\TestDefect3564 Modified", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormFieldSettings(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\FormField\TestFormFieldSettings", lf, sf);
        }

        /// <summary>
        /// WORDSNET-6137 “NullReferenceException” occurs during Saving Document.
        /// The error occurred because a form field spans several paragraphs and the last paragraph containing FieldEnd
        /// is deleted by the user. I made it resilient to avoid throwing. But we still write an incomplete field for the time being.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void TestDefect6137(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\FormField\TestDefect6137", lf);
            Assert.That(doc.GetTextLength(), Is.EqualTo(1475));
            doc.FirstSection.Body.LastParagraph.Remove();

            TestUtil.SaveOpen(doc, @"Model\FormField\TestDefect6137", lf, sf);
            Assert.That(doc.GetTextLength(), Is.EqualTo(1423));
        }

        /// <summary>
        /// Test tries to add to DropDown form field more than maximum elements count using DropDownItems.Add(...).
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDropDownAdd()
        {
            DocumentBuilder builder = new DocumentBuilder();
            FormField comboBox = builder.InsertComboBox("name", new string[] { "0" }, 0);
            const int count = DropDownItemCollection.MaxItemsCount + 1;
            for (int i = 1; i < count; i++)
                comboBox.DropDownItems.Add(i.ToString());
        }

        /// <summary>
        /// Test tries to create DropDown form field based on array with more than maximum elements count.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDropDownInsert()
        {
            const int count = DropDownItemCollection.MaxItemsCount + 1;
            string[] arItem = new string[count];
            for (int i = 0; i < count; i++)
                arItem[i] = i.ToString();
            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertComboBox("name", arItem, 0);
        }
    }
}
